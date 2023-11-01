#define CDK_IMPL

#include "CSString.h"
#include "CSStringContentTable.h"
#include "CSStringBuilder.h"
#include "CSStringDisplay.h"
#include "CSLocaleString.h"
#include "CSArray.h"
#include "CSEncoder.h"
#include "CSBuffer.h"
#include "CSFile.h"
#include "CSCrypto.h"

string::string(const char* str) : _content(str ? CSObject::retain(CSStringContentTable::sharedTable()->get(str)) : NULL) {
	
}

string::string(const char* str, int length) : _content(CSObject::retain(CSStringContentTable::sharedTable()->get(std::string(str, length)))) {

}

string::string(CSBuffer* buffer) {
	int length = buffer->readLength();
	_content = length ? CSObject::retain(CSStringContentTable::sharedTable()->get(std::string((const char*)buffer->bytes(), length))) : NULL;
}

#define NUMBER_TO_STRING(value, comma, format, outlen, buflen);	\
	char out[outlen] = {};	\
	char dgsp = CSLocaleString::digitGroupSeperator();	\
	if (comma && dgsp) {	\
		char buf[buflen];	\
		sprintf(buf, format, value);	\
		char* outp = out;	\
		int c = 2 - strlen(buf) % 3;	\
		for (char* bufp = buf; *bufp != 0; bufp++) {	\
			*outp++ = *bufp;	\
			if (c == 1) {	\
				*outp++ = dgsp;	\
			}	\
			c = (c + 1) % 3;	\
		}	\
		*--outp = 0;	\
	}	\
	else {	\
		sprintf(out, format, value);	\
	}

string::string(int value, bool comma) {
	NUMBER_TO_STRING(value, comma, "%d", 15, 12);
	_content = CSObject::retain(CSStringContentTable::sharedTable()->get(out));
}

string::string(uint value, bool comma) {
	NUMBER_TO_STRING(value, comma, "%u", 14, 11);
	_content = CSObject::retain(CSStringContentTable::sharedTable()->get(out));
}

string::string(int64 value, bool comma) {
	NUMBER_TO_STRING(value, comma, "%" PRId64, 28, 22);
	_content = CSObject::retain(CSStringContentTable::sharedTable()->get(out));
}

string::string(uint64 value, bool comma) {
	NUMBER_TO_STRING(value, comma, "%" PRIu64, 27, 21);
	_content = CSObject::retain(CSStringContentTable::sharedTable()->get(out));
}

string::~string() {
	CSObject::release(_content);
}

string string::encode(const void* data, int length, CSStringEncoding encoding) {
	return CSEncoder::encode(data, length, encoding);
}

string string::format(const char* format, ...) {
	va_list ap;
	va_start(ap, format);
	string result = formatAndArguments(format, ap);
	va_end(ap);
	return result;
}

string string::formatAndArguments(const char* format, va_list args) {
	va_list copy;
	va_copy(copy, args);
	int length = vsnprintf(NULL, 0, format, copy);
	va_end(copy);

	char* buf = (char*)fmalloc(length);
	vsnprintf(buf, length, format, args);
	string result(buf, length);
	free(buf);

	return result;
	//return string(cstringWithFormatAndArguments(args));
}

string string::contentOfFile(const char* path, CSStringEncoding encoding) {
	int length = 0;
	void* data = CSFile::createRawWithContentOfFile(path, 0, length, false);
	if (data) {
		string result = CSEncoder::encode(data, length, encoding);
		free(data);
		return result;
	}
	return string();
}

int string::intValue(const char* str) {
	return atoi(str);
}

int64 string::longValue(const char* str) {
	return atoll(str);
}

uint string::hexIntValue(const char* str) {
	char* p;
	return strtoul(str, &p, 16);
}

float string::floatValue(const char* str) {
	char* p;
	return strtof(str, &p);
}

double string::doubleValue(const char* str) {
	char* p;
	return strtod(str, &p);
}

bool string::boolValue(const char* str) {
	return strcmp(str, "0") && stricmp(str, "false");
}

character string::characterAtIndex(int index) const {
	CSAssert(_content, "out of range");
	const CSCharSequence* characters = _content->characters();

	int from = characters->from(index);
	int to = characters->to(index);

	int length = CSMath::min(to - from, CSMaxComposedCharLength);
	character result;
	const char* src = _content->cstring();
	strncpy(result.str, src + from, length);
	return result;
}

int string::length() const {
	if (!_content) return 0;
	const CSCharSequence* characters = _content->characters();
	return characters->count();
}

bool string::startsWith(const char* str) const {
	return _content && _content->string().find(str) == 0;
}

bool string::endsWith(const char* str) const {
	return _content && _content->string().find(str) == _content->clength() - strlen(str);
}

int string::indexOf(const char* str) const {
	return indexOf(str, 0);
}

int string::indexOf(const char* str, int from) const {
	if (!_content) return -1;

	const CSCharSequence* characters = _content->characters();

	from = characters->from(from);

	const char* s = _content->cstring();
	const char* p = strstr(s + from, str);

	if (!p) return -1;

	int ci = p - s;

	for (int i = from; i < characters->count(); i++) {
		if (characters->to(i) >= ci) return i;
	}

	return -1;
}

string string::substringWithRange(int index, int length) const {
	CSAssert(_content && length > 0, "out of range");
	const CSCharSequence* characters = _content->characters();

	int from = characters->from(index);
	int to = characters->to(index + length - 1);

	return string::string(_content->cstring() + from, to - from);
}

string string::substringFromIndex(int index) const {
	CSAssert(_content, "out of range");
	const CSCharSequence* characters = _content->characters();

	return string::string(_content->cstring() + characters->from(index));
}

CSArray<string>* string::split(const char* delimiter) const {
	CSArray<string>* result = CSArray<string>::array();
	if (_content) {
		char* str = strdup(_content->cstring());
		char* tok = strtok(str, delimiter);

		while (tok) {
			result->addObject(string(tok));
			tok = strtok(NULL, delimiter);
		}
		free(str);
	}
	return result;
}

CSData* string::data(CSStringEncoding encoding) const {
	if (_content) {
		int length = 0;
		void* bytes = CSEncoder::decode(_content->cstring(), length, encoding);
		if (bytes) return CSData::dataWithBytes(bytes, length, false);
	}
	return NULL;
}

void string::replace(const char* from, const char* to) {
	if (_content) {
		CSStringBuilder strbuf(*this);
		strbuf.replace(from, to);
		CSObject::retain(_content, strbuf.toString());
	}
}

void string::trim(bool left, bool right) {
	if (_content) {
		CSStringBuilder strbuf(*this);
		strbuf.trim(left, right);
		CSObject::retain(_content, strbuf.toString());
	}
}

string string::replace(const char* str, const char* from, const char* to) {
	CSStringBuilder strbuf(str);
	strbuf.replace(from, to);
	return strbuf.toString();
}

string string::trim(const char* str, bool left, bool right) {
	CSStringBuilder strbuf(str);
	strbuf.trim(left, right);
	return strbuf.toString();
}

void string::clear() {
	CSObject::release(_content);
}

void string::append(const char* str) {
	CSStringBuilder strbuf(*this);
	strbuf.append(str);
	CSObject::retain(_content, strbuf.toString());
}

void string::appendFormatAndArguments(const char* format, va_list args) {
	CSStringBuilder strbuf(*this);
	strbuf.appendFormatAndArguments(format, args);
	CSObject::retain(_content, strbuf.toString());
}

void string::appendFormat(const char* format, ...) {
	va_list ap;
	va_start(ap, format);
	appendFormatAndArguments(format, ap);
	va_end(ap);
}

void string::insert(int index, const char* str) {
	if (index > 0) {
		CSAssert(_content, "out of range");
		const CSCharSequence* characters = _content->characters();
		index = characters->from(index);
	}
	CSStringBuilder strbuf(*this, true);
	strbuf.insert(index, str);
	CSObject::retain(_content, strbuf.toString());
}

void string::insertFormatAndArguments(int index, const char* format, va_list args) {
	if (index > 0) {
		CSAssert(_content, "out of range");
		const CSCharSequence* characters = _content->characters();
		index = characters->from(index);
	}
	CSStringBuilder strbuf(*this, true);
	strbuf.insertFormatAndArguments(index, format, args);
	CSObject::retain(_content, strbuf.toString());
}

void string::insertFormat(int index, const char* format, ...) {
	va_list ap;
	va_start(ap, format);
	insertFormatAndArguments(index, format, ap);
	va_end(ap);
}

void string::removeWithRange(int index, int length) {
	CSAssert(_content && length > 0, "out of range");
	const CSCharSequence* characters = _content->characters();
	int from = characters->from(index);
	int to = characters->to(index + length - 1);

	CSStringBuilder strbuf(*this, true);
	strbuf.removeWithRange(from, to - from);
	CSObject::retain(_content, strbuf.toString());
}

void string::removeAtIndex(int i) {
	removeWithRange(i, 1);
}

bool string::AESEncrypt(const char* key) {
	if (!_content) return false;
	CSData* data = CSData::dataWithBytes(_content->cstring(), _content->clength());
	if (!data || !data->AESEncrypt(key, false)) return false;
	const CSStringContent* content = data->base64EncodedString();
	if (!content) return false;
	CSObject::retain(_content, content);
	return true;
}

bool string::AESDecrypt(const char* key) {
	if (!_content) return false;
	CSData* data = CSData::dataWithBase64EncodedString(_content->cstring());
	if (!data || !data->AESDecrypt(key, false)) return false;
	CSObject::retain(_content, CSStringContentTable::sharedTable()->get(std::string((const char*)data->bytes(), data->length())));
	return true;
}

string string::AESEncrypt(const char* str, const char* key) {
	CSData* data = CSData::dataWithBytes(str, strlen(str));
	return data->AESEncrypt(key, false) ? data->base64EncodedString() : string();
}

string string::AESDecrypt(const char* str, const char* key) {
	CSData* data = CSData::dataWithBase64EncodedString(str);
	return data && data->AESDecrypt(key, false) ? string((const char*)data->bytes(), data->length()) : string();
}

char* string::cstring(const uchar* str, bool free) {
	return cstring(str, u_strlen(str), free);
}

char* string::cstring(const uchar* str, int ulength, bool free) {
	int capacity = ulength * 4 + 1;
	char* rtn = (char*)calloc(sizeof(char), capacity);
	UErrorCode err = U_ZERO_ERROR;
	u_strToUTF8WithSub(rtn, capacity, NULL, str, ulength, 0x0FFFD, NULL, &err);
	CSAssert(!err, "ucstring error:%d", err);
	if (free) autofree(rtn);
	return rtn;
}

uchar* string::ucstring(const char* str, bool free) {
	return ucstring(str, strlen(str), free);
}

uchar* string::ucstring(const char* str, int clength, bool free) {
	int capacity = clength + 1;
	uchar* rtn = (uchar*)calloc(sizeof(uchar), capacity);
	UErrorCode err = U_ZERO_ERROR;
	u_strFromUTF8WithSub(rtn, capacity, NULL, str, clength, 0x0FFFD, NULL, &err);
	CSAssert(!err, "ucstring error:%d", err);
	if (free) autofree(rtn);
	return rtn;
}

char* string::cstringWithFormatAndArguments(const char* format, va_list args) {
	va_list copy;
	va_copy(copy, args);
	int length = vsnprintf(NULL, 0, format, copy) + 1;
	va_end(copy);

	char* buf = (char*)fmalloc(length);
	vsprintf(buf, format, args);
	autofree(buf);
	return buf;
}

char* string::cstringWithFormat(const char* format, ...) {
	va_list ap;
	va_start(ap, format);
	char* buf = cstringWithFormatAndArguments(format, ap);
	va_end(ap);
	return buf;
}

char* string::cstringWithNumber(int value, bool comma) {
	NUMBER_TO_STRING(value, comma, "%d", 15, 12);
	char* p = strdup(out);
	autofree(p);
	return p;
}

char* string::cstringWithNumber(uint value, bool comma) {
	NUMBER_TO_STRING(value, comma, "%u", 14, 11);
	char* p = strdup(out);
	autofree(p);
	return p;
}

char* string::cstringWithNumber(int64 value, bool comma) {
	NUMBER_TO_STRING(value, comma, "%" PRId64, 28, 22);
	char* p = strdup(out);
	autofree(p);
	return p;
}

char* string::cstringWithNumber(uint64 value, bool comma) {
	NUMBER_TO_STRING(value, comma, "%" PRIu64, 27, 21);
	char* p = strdup(out);
	autofree(p);
	return p;
}

const uchar* string::ucstring() const {
	return _content ? _content->display()->content() : NULL;
}

uint string::toCodePoint(const uchar* cc, int& i) {
	int c1 = cc[i++];
	if (c1 >= 0xd800 && c1 < 0xdc00) {
		int c2 = cc[i++];
		return ((c1 & 0x3ff) << 10) + (c2 & 0x3ff) + 0x10000;
	}
	return c1;
}

uint string::toCodePoint(const char* cc, int& i) {
	byte u0 = cc[i++];
	if (u0 >= 0 && u0 <= 127) return u0;
	byte u1 = cc[i++];
	if (u0 >= 192 && u0 <= 223) return (u0 - 192) * 64 + (u1 - 128);
	if (u0 == 0xed && (u1 & 0xa0) == 0xa0) return -1;
	byte u2 = cc[i++];
	if (u0 >= 224 && u0 <= 239) return (u0 - 224) * 4096 + (u1 - 128) * 64 + (u2 - 128);
	byte u3 = cc[i++];
	if (u0 >= 240 && u0 <= 247) return (u0 - 240) * 262144 + (u1 - 128) * 4096 + (u2 - 128) * 64 + (u3 - 128);
	return -1;
}

string string::sha1() const {
	return _content ? CSCrypto::sha1(_content->cstring(), _content->clength()) : string();
}

string string::sha1(const char* str) {
	return CSCrypto::sha1(str, strlen(str));
}

void string::writeTo(CSBuffer* buffer, const char* str) {
	int len = strlen(str);
	buffer->writeLength(len);
	buffer->write(str, len);
}

void string::writeTo(CSBuffer* buffer) const {
	if (_content) {
		buffer->writeLength(_content->clength());
		buffer->write(_content->cstring(), _content->clength());
	}
	else {
		buffer->writeLength(0);
	}
}

bool string::writeToFile(const char* path, const char* str, CSStringEncoding encoding, bool compression) {
	if (encoding == CSStringEncodingUTF8) {
		return CSFile::writeRawToFile(path, str, strlen(str) + 1, compression);
	}
	else {
		int length = 0;
		void* bytes = CSEncoder::decode(str, length, encoding);
		if (bytes) return CSFile::writeRawToFile(path, bytes, length, compression);
	}
	return false;
}

bool string::writeToFile(const char* path, CSStringEncoding encoding, bool compression) const {
	if (_content) {
		if (encoding == CSStringEncodingUTF8) {
			return CSFile::writeRawToFile(path, _content->cstring(), _content->clength() + 1, compression);
		}
		else {
			int length = 0;
			void* bytes = CSEncoder::decode(_content->cstring(), length, encoding);
			if (bytes) return CSFile::writeRawToFile(path, bytes, length, compression);
		}
	}
	return false;
}

string& string::operator =(const char* str) {
	CSObject::retain(_content, str ? CSStringContentTable::sharedTable()->get(str) : NULL);
	return *this;
}

string& string::operator +=(const char* str) {
	CSStringBuilder strbuf(*this, true);
	strbuf.append(str);
	CSObject::retain(_content, strbuf.toString());
	return *this;
}

string string::operator +(const char* str) const {
	CSStringBuilder strbuf(*this, true);
	strbuf.append(str);
	return strbuf.toString();
}
