#define CDK_IMPL

#include "CSTextureScratch.h"

CSTextureScratch* CSTextureScratch::create(const CSTextureDescription& desc) {
    CSTexture* texture = new CSTexture(desc, true);
    CSTextureScratch* result = create(texture, 0);
    texture->release();
    return result;
}

CSTextureScratch* CSTextureScratch::create(CSTexture* texture, int level) {
    CSAssert(texture->target() == CSTextureTarget2D && texture->allocated());
    const CSRawFormatEncoding& encoding = texture->format().encoding();
    CSAssert(!encoding.compressed);

    int width = texture->width();
    int height = texture->height();

    void* raw = calloc(width * height, encoding.pixelBpp);
    if (!raw) return NULL;

    CSTextureScratch* scratch = new CSTextureScratch();
    scratch->_texture = retain(texture);
    scratch->_raw = raw;
    scratch->_width = width;
    scratch->_height = height;
    scratch->_bpp = encoding.pixelBpp;
    scratch->_level = level;
    scratch->_updateLeft = 0xffff;
    scratch->_updateRight = 0;
    scratch->_updateTop = 0xffff;
    scratch->_updateBottom = 0;
    return scratch;
}

CSTextureScratch::~CSTextureScratch() {
    upload();
    _texture->release();
    free(_raw);
}

void* CSTextureScratch::get(int x, int y) const {
    if (x >= 0 && x < _width && y >= 0 && y < _height) {
        int off = (y * _width + x) * _bpp;
        return (byte*)_raw + off;
    }
    return NULL;
}

void CSTextureScratch::set(int x, int y, void* data) {
    if (x >= 0 && x < _width && y >= 0 && y < _height) {
        int off = (y * _width + x) * _bpp;
        memcpy((byte*)_raw + off, data, _bpp);

        _updateLeft = CSMath::min((int)_updateLeft, x);
        _updateRight = CSMath::max((int)_updateRight, x);
        _updateTop = CSMath::min((int)_updateTop, y);
        _updateBottom = CSMath::max((int)_updateBottom, y);
    }
}

void CSTextureScratch::upload() const {
    if (_updateLeft <= _updateRight && _updateTop <= _updateBottom) {
        CSAssert(_texture->allocated() && _width == _texture->width() && _height == _texture->height());

        int width = _updateRight - _updateLeft + 1;
        int height = _updateBottom - _updateTop + 1;

        bool done;

        if (width != _width || height != _height) {
            void* raw = malloc(width * height * _bpp);
            if (!raw) return;
            if (width != _width) {
                for (int y = 0; y < height; y++) {
                    memcpy((byte*)raw + (y * width * _bpp), (byte*)_raw + (((_updateTop + y) * _width + _updateLeft) * _bpp), width * _bpp);
                }
            }
            else memcpy(raw, (byte*)_raw + (_updateTop * _width * _bpp), width * height * _bpp);

            done =_texture->uploadSub(raw, width * height * _bpp, _level, _updateLeft, _updateTop, 0, width, height, 0);

            free(raw);
        }
        else {
            done = _texture->uploadSub(_raw, width * height * _bpp, _level, 0, 0, 0, width, height, 0);
        }
        if (done) {
            _updateLeft = 0xffff;
            _updateRight = 0;
            _updateTop = 0xffff;
            _updateBottom = 0;
        }
    }
}

