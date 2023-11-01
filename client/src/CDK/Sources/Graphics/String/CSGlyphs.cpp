#define CDK_IMPL

#include "CSGlyphs.h"

#include "CSRaster.h"

#include "icu_compat.h"

#define CSGlyphsTextureSize     2048
#define CSCharacterPadding      1

CSGlyphs::Key::Key(const uchar* cc, int cclen, const CSFont* font) :
    _fontName(font->name()),
    _fontStyle(font->style()),
    _fontSize(font->size())
{
    CSAssert(cclen <= CSMaxComposedCharLength);
    u_strncpy(_cc, cc, cclen);
    _cc[cclen] = 0;
}

CSGlyphs::Key::Key(const Key& other) :
    _fontName(other._fontName),
    _fontStyle(other._fontStyle),
    _fontSize(other._fontSize)
{
    u_strcpy(_cc, other._cc);
}

CSGlyphs::Key& CSGlyphs::Key::operator =(const Key& other) {
    u_strcpy(_cc, other._cc);
    _fontName = other._fontName;
    _fontStyle = other._fontStyle;
    _fontSize = other._fontSize;
    return *this;
}

uint CSGlyphs::Key::hash() const {
    CSHash hash;
    const uchar* p = _cc;
    while (*p) hash.combine(*p);
    hash.combine(_fontName);
    hash.combine(_fontStyle);
    hash.combine(_fontSize);
    return hash;
}

bool CSGlyphs::Key::operator ==(const Key& other) const {
    return u_strcmp(_cc, other._cc) == 0 && _fontName == other._fontName && _fontStyle == other._fontStyle && _fontSize == other._fontSize;
}

CSGlyphs::Image::Image(const CSImage* image, float offy) : image(image), offy(offy) {
    image->retain();
}

CSGlyphs::Image::~Image() {
    image->release();
}

void CSGlyphs::Scratch::clear() {
    textures.removeAllObjects();
    x = 0;
    y = 0;
    h = 0;
}

//============================================================================================================================================================

CSGlyphs* CSGlyphs::_instance = NULL;

CSGlyphs::CSGlyphs() :
    _images(8192),
    _sizes(8192),
    _vacants(64)
{
    
}

void CSGlyphs::initialize() {
    if (!_instance) _instance = new CSGlyphs();
}

void CSGlyphs::finalize() {
    if (_instance) {
        delete _instance;
        _instance = NULL;
    }
}

void CSGlyphs::clear() {
    _images.removeAllObjects();
    _scratch[0].clear();
    _scratch[1].clear();
}

const CSVector2& CSGlyphs::size(const uchar* cc, int cclen, const CSFont* font) {
	if (cclen > CSMaxComposedCharLength) return CSVector2::Zero;

    Key key(cc, cclen, font);
    
    const CSVector2* size = _sizes.tryGetObjectForKey(key);
    
    return size ? *size : (_sizes.setObject(key) = CSRaster::characterSize(cc, cclen, font));
}

bool CSGlyphs::addTexture(bool color) {
    int bpp = color ? 4 : 1;
    void* emptyRaw = calloc(CSGlyphsTextureSize * CSGlyphsTextureSize, bpp);
    if (!emptyRaw) return false;
    CSTextureDescription desc;
    desc.width = CSGlyphsTextureSize;
    desc.height = CSGlyphsTextureSize;
    desc.format = color ? CSRawFormat::Rgba8 : CSRawFormat::Alpha8;
    desc.wrapS = CSTextureWrapClampToEdge;
    desc.wrapT = CSTextureWrapClampToEdge;
    desc.minFilter = CSTextureMinFilterLinear;
    desc.magFilter = CSTextureMagFilterLinear;
    CSTexture* texture = new CSTexture(desc, emptyRaw, CSGlyphsTextureSize * CSGlyphsTextureSize * bpp);        //TODO:CHECK ZERO FILL?
    free(emptyRaw);
    _scratch[color].textures.addObject(texture);
    texture->release();
    CSLog("image added(%s):%d / %d", color ? "color" : "grayscale", _scratch[0].textures.count(), _scratch[1].textures.count());
    return true;
}

const CSImage* CSGlyphs::image(const uchar* cc, int cclen, const CSFont* font, float& offy) {
	if (cclen > CSMaxComposedCharLength) return NULL;

    Key key(cc, cclen, font);

    if (_vacants.containsObject(key)) return NULL;

    Image* e = _images.tryGetObjectForKey(key);
    
    if (e) {
        offy = e->offy;
        return e->image;
    }

    if (!u_isgraph(string::toCodePoint(cc))) {
        _vacants.addObject(key);
        return NULL;
    }

    int w = 0;
    int h = 0;
    void* data = CSRaster::createRawWithCharacter(cc, cclen, font, w, h, offy);
    if (!data) {
        if (w == 0 || h == 0) _vacants.addObject(key);
        return NULL;
    }
        
    bool color = false;

    int dataLen = w * h;

    {
        for (int i = 0; i < dataLen; i++) {
            byte* components = &((byte*)data)[i * 4];
            byte a = components[3];
            if (components[0] != a || components[1] != a || components[2] != a) {
                color = true;
                break;
            }
        }
            
        //premultiplied 지만 반투명 글자가 없다고 생각하고 무시
            
        if (color) dataLen *= 4;
        else {
            byte* temp = (byte*)fmalloc(dataLen);
            if (!temp) {
                free(data);
                return NULL;
            }
            for (int i = 0; i < dataLen; i++) {
                temp[i] = ((byte*)data)[i * 4 + 3];
            }
            free(data);
            data = temp;
        }
    }
        
    Scratch& t = _scratch[color];
        
    if (t.x + w > CSGlyphsTextureSize) {
        t.y += t.h + CSCharacterPadding;
        t.x = 0;
        t.h = 0;
    }
    if (t.y + h > CSGlyphsTextureSize) {
        if (!addTexture(color)) {
            free(data);
            return NULL;
        }
        t.x = 0;
        t.y = 0;
        t.h = 0;
    }
    else if (!t.textures.count()) {
        if (!addTexture(color)) {
            free(data);
            return NULL;
        }
    }
        
    CSTexture* texture = t.textures.lastObject();

    texture->uploadSub(data, dataLen, 0, t.x, t.y, 0, w, h, 0);

    free(data);

    const CSImage* image = CSImage::image(texture, 1, CSRect(t.x, t.y, w, h), CSVector2::Zero);
        
    new (&_images.setObject(key)) Image(image, offy);
        
    if (t.h < h) t.h = h;
    t.x += w + CSCharacterPadding;

    return image;
}

