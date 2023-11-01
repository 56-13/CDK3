#ifdef CDK_WINDOWS

#define CDK_IMPL

#include "CSEncoder.h"

string CSEncoder::encode(const void* bytes, int length, CSStringEncoding encoding) {
	switch (encoding) {
		case CSStringEncodingUTF8:
			return string((const char*)bytes, length);
		case CSStringEncodingUTF16:
		case CSStringEncodingUTF16LE:
			{
				length /= sizeof(uchar);
				char* cstr = string::cstring((const uchar*)bytes, length, false);
				if (!cstr) {
					length = 0;
					return NULL;
				}
				string str(cstr);
				free(cstr);

				return str;
			}
		case CSStringEncodingUTF16BE:
			{
				uchar* dest = (uchar*)malloc(length);
				if (!dest) {
					length = 0;
					return NULL;
				}
				const uchar* src = (const uchar*)bytes;

				length /= sizeof(uchar);

				for (int i = 0; i < length; i++) {
					dest[i] = (uchar)(((src[i] << 8) & 0xFF00) | ((src[i] >> 8) & 0x00FF));
				}
				char* cstr = string::cstring(dest, length, false);
				free(dest);
				string str(cstr);
				free(cstr);

				return str;
			}
	}
	return string();
}

void* CSEncoder::decode(const char* str, int& length, CSStringEncoding encoding) {
	length = strlen(str);

	if (length) {
		switch (encoding) {
			case CSStringEncodingUTF8:
				{
					void* buf = fmalloc(length);
					memcpy(buf, str, length);
					return buf;
				}
			case CSStringEncodingUTF16:
			case CSStringEncodingUTF16LE:
				{
					uchar* ustr = string::ucstring(str, false);
					length = u_strlen(ustr) * sizeof(uchar);
					return ustr;
				}
			case CSStringEncodingUTF16BE:
				{
					uchar* ustr = string::ucstring(str, false);
					length = u_strlen(ustr);
					for (int i = 0; i < length; i++) {
						ustr[i] = (uchar)(((ustr[i] << 8) & 0xFF00) | ((ustr[i] >> 8) & 0x00FF));
					}
					length *= sizeof(uchar);
					return ustr;
				}
		}
	}
	return NULL;
}

#endif