#ifndef __CDK__CSImage__
#define __CDK__CSImage__

#include "CSTexture.h"

#include "CSDictionary.h"

#include "CSZRect.h"

class CSImage : public CSResource {
private:
    struct Entry {
        CSRect frame;
        CSVector2 pivot;

        Entry(CSBuffer* buffer);
        Entry(const CSRect& frame, const CSVector2& pivot);
    };

    const CSTexture* _texture;
    bool _own;
    float _contentScale;
    Entry _defaultEntry;
    CSArray<CSImage>* _subImages = NULL;
    CSDictionary<string, Entry>* _localeEntries = NULL;
public:
    CSImage(CSBuffer* buffer);
private:
    CSImage(CSBuffer* buffer, const CSTexture* texture);
public:
    CSImage(const CSTexture* texture, float contentScale, const CSVector2& pivot);
    CSImage(const CSTexture* texture, float contentScale, const CSRect& frame, const CSVector2& pivot);
    ~CSImage();

    static inline CSImage* imageWithBuffer(CSBuffer* buffer) {
        return autorelease(new CSImage(buffer));
    }
    static inline CSImage* image(const CSTexture* texture, float contentScale, const CSVector2& pivot) {
        return autorelease(new CSImage(texture, contentScale, pivot));
    }
    static inline CSImage* image(const CSTexture* texture, float contentScale, const CSRect& frame, const CSVector2& pivot) {
        return autorelease(new CSImage(texture, contentScale, frame, pivot));
    }
    
    inline const CSTexture* texture() const {
        return _texture;
    }
private:
    const Entry& entry() const;
public:
    inline CSResourceType resourceType() const override {
        return CSResourceTypeImage;
    }
    int resourceCost() const override;

    inline void setContentScale(float contentScale) {
        _contentScale = contentScale;
    }
    inline float contentScale() const {
        return _contentScale;
    }
    inline void setDefaultFrame(const CSRect& frame) {
        _defaultEntry.frame = frame;
    }
    inline void setDefaultPivot(const CSVector2& pivot) {
        _defaultEntry.pivot = pivot;
    }
    inline const CSRect& frame() const {
        return entry().frame;
    }
    inline const CSVector2& pivot() const {
        return entry().pivot;
    }
    inline float width() const {
        return entry().frame.width * _contentScale;
    }
    inline float height() const {
        return entry().frame.height * _contentScale;
    }
    void clearLocales();
    void removeLocale(const string& locale);
    void setLocale(const string& locale, const CSRect& frame, const CSVector2& pivot);
    
    void clearSub();
    CSImage* addSub(float contentScale, const CSRect& frame, const CSVector2& pivot);
    inline CSImage* addSub(const CSRect& frame, const CSVector2& pivot) {
        return addSub(_contentScale, frame, pivot);
    }
    CSImage* insertSub(int index, float contentScale, const CSRect& frame, const CSVector2& pivot);
    inline CSImage* insertSub(int index, const CSRect& frame, const CSVector2& pivot) {
        return insertSub(index, _contentScale, frame, pivot);
    }
    void removeSub(int index);
    CSImage* sub(int index);
    inline const CSImage* sub(int index) const {
        return const_cast<CSImage*>(this)->sub(index);
    }
    inline int subCount() const {
        return _subImages ? _subImages->count() : 0;
    }

    CSZRect displayRect(const CSVector3& point, CSAlign align, float scale = 1) const;
};

#endif
