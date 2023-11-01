#define CDK_IMPL

#include "CSImage.h"

#include "CSBuffer.h"

CSImage::Entry::Entry(CSBuffer* buffer) : frame(buffer), pivot(buffer) {
    
}

CSImage::Entry::Entry(const CSRect& frame, const CSVector2& pivot) : frame(frame), pivot(pivot) {
    
}

CSImage::CSImage(CSBuffer* buffer) : _texture(new CSTexture(buffer)), _own(true), _contentScale(buffer->readFloat()), _defaultEntry(buffer) {
    CSAssert(_texture->target() == CSTextureTarget2D, "image target should be 2D");
    int subCount = buffer->readLength();
    if (subCount) {
        _subImages = new CSArray<CSImage>(subCount);
        for (int i = 0; i < subCount; i++) {
            CSImage* subImage = new CSImage(buffer, _texture);
            _subImages->addObject(subImage);
            subImage->release();
        }
    }
}

CSImage::CSImage(CSBuffer* buffer, const CSTexture* texture) : _texture(retain(texture)), _own(false), _contentScale(buffer->readFloat()), _defaultEntry(buffer) {
    CSAssert(_texture->target() == CSTextureTarget2D, "image target should be 2D");
    int localeCount = buffer->readLength();
    if (localeCount) {
        _localeEntries = new CSDictionary<string, Entry>();
        for (int i = 0; i < localeCount; i++) {
            CSArray<string> locales(buffer);
            Entry e(buffer);

            foreach (const string&, locale, &locales) _localeEntries->setObject(locale, e);
        }
    }
}

CSImage::CSImage(const CSTexture* texture, float contentScale, const CSVector2& pivot) : CSImage(texture, contentScale, CSRect(0, 0, texture->width(), texture->height()), pivot) {

}

CSImage::CSImage(const CSTexture* texture, float contentScale, const CSRect& frame, const CSVector2& pivot) : _texture(retain(texture)), _own(false), _contentScale(contentScale), _defaultEntry(frame, pivot) {
    CSAssert(_texture->target() == CSTextureTarget2D, "image target should be 2D");
}

CSImage::~CSImage() {
    _texture->release();
    release(_subImages);
    release(_localeEntries);
}

int CSImage::resourceCost() const {
    int cost = sizeof(CSImage);
    if (_own) _texture->resourceCost();
    if (_localeEntries) cost += sizeof(CSDictionary<string, Entry>) + _localeEntries->capacity() * (8 + sizeof(Entry));
    if (_subImages) {
        cost += sizeof(CSArray<CSImage>) + _subImages->capacity() * 8;
        foreach (const CSImage*, sub, _subImages) cost += sub->resourceCost();
    }
    return cost;
}

const CSImage::Entry& CSImage::entry() const {
    if (_localeEntries) {
        const string& locale = CSLocaleString::locale();
        if (locale) {
            const Entry* e = _localeEntries->tryGetObjectForKey(locale);
            if (e) return *e;
        }
    }
    return _defaultEntry;
}

void CSImage::clearLocales() {
    release(_localeEntries);
}

void CSImage::removeLocale(const string& locale) {
    if (_localeEntries) _localeEntries->removeObject(locale);
}

void CSImage::setLocale(const string& locale, const CSRect& frame, const CSVector2& pivot) {
    if (!_localeEntries) _localeEntries = new CSDictionary<string, Entry>();

    _localeEntries->setObject(locale, Entry(frame, pivot));
}

void CSImage::clearSub() {
    release(_subImages);
}

CSImage* CSImage::addSub(float contentScale, const CSRect& frame, const CSVector2& pivot) {
    if (!_subImages) _subImages = new CSArray<CSImage>();

    CSImage* subImage = new CSImage(_texture, contentScale, frame, pivot);
    _subImages->addObject(subImage);
    subImage->release();
    return subImage;
}

CSImage* CSImage::insertSub(int index, float contentScale, const CSRect& frame, const CSVector2& pivot) {
    if (!_subImages) _subImages = new CSArray<CSImage>();

    CSImage* subImage = new CSImage(_texture, contentScale, frame, pivot);
    _subImages->insertObject(index, subImage);
    subImage->release();
    return subImage;
}

void CSImage::removeSub(int index) {
    CSAssert(_subImages);
    _subImages->removeObjectAtIndex(index);
}

CSImage* CSImage::sub(int index) {
    CSAssert(_subImages);
    return _subImages->objectAtIndex(index);
}

CSZRect CSImage::displayRect(const CSVector3& point, CSAlign align, float scale) const {
    const Entry& e = entry();

    CSZRect rect(
        point,
        e.frame.width * _contentScale * scale,
        e.frame.height * _contentScale * scale);

    if (align & CSAlignCenter) rect.x -= rect.width * 0.5f + e.pivot.x * _contentScale * scale;
    else if (align & CSAlignRight) rect.x -= rect.width;
    if (align & CSAlignMiddle) rect.y -= rect.height * 0.5f + e.pivot.y * _contentScale * scale;
    else if (align & CSAlignBottom) rect.y -= rect.height;
    return rect;
}

