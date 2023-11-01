#ifdef CDK_IMPL

#ifndef __CDK__CSGlyphs__
#define __CDK__CSGlyphs__

#include "CSDictionary.h"
#include "CSSet.h"
#include "CSImage.h"
#include "CSFont.h"

class CSGlyphs {
private:
    struct Key {
    private:
        uchar _cc[CSMaxComposedCharLength + 1];
        string _fontName;
        CSFontStyle _fontStyle;
        float _fontSize;
    public:
        Key(const uchar* cc, int cclen, const CSFont* font);
        Key(const Key& other);

        Key& operator =(const Key& other);

        uint hash() const;
        bool operator ==(const Key& other) const;
    };
    struct Image {
        const CSImage* image;
        float offy;

        Image(const CSImage* image, float offy);
        ~Image();

        Image(const Image&) = delete;
        Image& operator =(const Image&) = delete;
    };
    struct Scratch {
        CSArray<CSTexture> textures;
        ushort x = 0;
        ushort y = 0;
        ushort h = 0;

        void clear();
    };

    CSDictionary<Key, Image> _images;
    CSDictionary<Key, CSVector2> _sizes;
    CSSet<Key> _vacants;
    Scratch _scratch[2];

    static CSGlyphs* _instance;

    CSGlyphs();
    ~CSGlyphs() = default;
public:
    static void initialize();
    static void finalize();

    static inline CSGlyphs* sharedGlyphs() {
        return _instance;
    }

    void clear();
    const CSVector2& size(const uchar* cc, int cclen, const CSFont* font);
    const CSImage* image(const uchar* cc, int cclen, const CSFont* font, float& offy);
private:
    bool addTexture(bool color);
};

#endif

#endif
