#define CDK_IMPL

#include "CSTexture.h"

#include "CSFile.h"
#include "CSBuffer.h"
#include "CSRaster.h"
#include "CSGraphicsContext.h"

static void applyTextureDescription(CSGraphicsApi* api, const CSTextureDescription& desc);

CSTexture::CSTexture(const CSTextureDescription& desc, bool allocate) : _description(desc) {
    _description.validate();

    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) {
        _object = api->genTexture();

        api->bindTexture(_description.target, _object);

        applyTextureDescription(api, _description);
    }, this);
    if (command) command->addFence(this, CSGBatchFlagReadWrite);

    if (allocate) this->allocate();
}

CSTexture::CSTexture(const CSTextureDescription& desc, const void* raw, int rawLength) : _description(desc) {
    _description.validate();

    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) {
        _object = api->genTexture();

        api->bindTexture(_description.target, _object);

        applyTextureDescription(api, _description);
    }, this);
    if (command) command->addFence(this, CSGBatchFlagReadWrite);

    upload(raw, rawLength, 0, _description.width, _description.height, _description.depth);
}

CSTexture::CSTexture(CSBuffer* buffer) {
    _description.width = buffer->readShort();
    _description.height = buffer->readShort();
    _description.depth = buffer->readShort();
    _description.target = (CSTextureTarget)buffer->readShort();
    _description.wrapS = (CSTextureWrapMode)buffer->readShort();
    _description.wrapT = (CSTextureWrapMode)buffer->readShort();
    _description.wrapR = (CSTextureWrapMode)buffer->readShort();
    _description.minFilter = (CSTextureMinFilter)buffer->readShort();
    _description.magFilter = (CSTextureMagFilter)buffer->readShort();
    _description.borderColor = CSColor(buffer);
    _description.mipmapCount = buffer->readByte();
    _description.samples = 1;

    int encodingCount = buffer->readByte();

    int i;
    for (i = 0; i < encodingCount; i++) {
        CSRawFormat format = (CSRawFormat)buffer->readShort();

        int dataLength = buffer->readInt();
        int nextPos = buffer->position() + dataLength;

        if (!format.isSupported()) {
            buffer->setPosition(nextPos);
            continue;
        }

        _description.format = format;

        int faceCount = buffer->readByte();

#ifdef CS_ASSERT_DEBUG
        switch (_description.target) {
            case CSTextureTargetCubeMap:
                CSAssert(faceCount == 6, "invalid data");
                break;
            case CSTextureTarget2D:
            case CSTextureTarget3D:
                CSAssert(faceCount == 1, "invalid data");
                break;
            default:
                CSAssert(false, "invalid data");
                break;
        }
#endif
        _allocated = true;

        CSGraphicsApi* api;
        if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
            load(api, buffer);
        }
        else {
            void* data = malloc(dataLength);

            if (data) {
                buffer->read(data, dataLength);
                buffer = new CSBuffer(data, dataLength, false);

                CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, buffer](CSGraphicsApi* api) {
                    load(api, buffer);
                }, this, buffer);
                if (command) command->addFence(this, CSGBatchFlagReadWrite);

                buffer->release();
            }
            else uploadFail();
        }
        buffer->setPosition(nextPos);
        break;
    }
    for (; i < encodingCount; i++) {
        buffer->readShort();
        buffer->skip(buffer->readInt());
    }
    if (!_allocated) {
        CSErrorLog("load fail:target:%d format:%d size:%d, %d, %d", _description.target, _description.format, _description.width, _description.height, _description.depth);
    }
}

CSTexture::~CSTexture() {
    int object = _object;

    if (object) {      //실제로 생성이 되지 않았을 수도 있음
        CSGraphicsContext::sharedContext()->invoke(false, [object](CSGraphicsApi* api) { api->deleteTexture(object); });
    }
}

CSTexture* CSTexture::createWithBitmap(CSTextureDescription desc, const void* data, int dataLength) {
    int width, height;
    void* raw = CSRaster::createRawWithBitmap(data, dataLength, width, height);
    if (!raw) {
        CSErrorLog("bitmap texture load fail");
        return NULL;
    }
    void* cnvraw;

    int pixelCount = width * height;

    switch (desc.format) {
        case CSRawFormat::Alpha8:
            dataLength = pixelCount;
            cnvraw = malloc(dataLength);
            if (!cnvraw) {
                free(raw);
                CSErrorLog("bitmap texture not enough memory");
                return NULL;
            }
            for (int i = 0; i < pixelCount; i++) {
                uint p = ((uint*)raw)[i];
                ((byte*)cnvraw)[i] = (p >> 24) & 0xff;
            }
            free(raw);
            raw = cnvraw;
            break;
        case CSRawFormat::Luminance8:
            dataLength = pixelCount;
            cnvraw = malloc(dataLength);
            if (!cnvraw) {
                free(raw);
                CSErrorLog("bitmap texture not enough memory");
                return NULL;
            }
            for (int i = 0; i < pixelCount; i++) {
                uint p = ((uint*)raw)[i];
                ((byte*)cnvraw)[i] = ((p & 0xff) + ((p >> 8) & 0xff) + ((p >> 16) & 0xff)) / 3;
            }
            free(raw);
            raw = cnvraw;
            break;
        case CSRawFormat::Luminance8Alpha8:
            dataLength = pixelCount * 2;
            cnvraw = malloc(dataLength);
            if (!cnvraw) {
                free(raw);
                CSErrorLog("bitmap texture not enough memory");
                return NULL;
            }
            for (int i = 0; i < pixelCount; i++) {
                uint p = ((uint*)raw)[i];
                ((ushort*)cnvraw)[i] = (((p & 0xff) + ((p >> 8) & 0xff) + ((p >> 16) & 0xff)) / 3) << 8 | ((p >> 24) & 0xff);
            }
            free(raw);
            raw = cnvraw;
            break;
        case CSRawFormat::R8:
        case CSRawFormat::R8ui:
            dataLength = pixelCount;
            cnvraw = malloc(dataLength);
            if (!cnvraw) {
                free(raw);
                CSErrorLog("bitmap texture not enough memory");
                return NULL;
            }
            for (int i = 0; i < pixelCount; i++) {
                uint p = ((uint*)raw)[i];
                ((byte*)cnvraw)[i] = p & 0xff;
            }
            free(raw);
            raw = cnvraw;
            break;
        case CSRawFormat::Rg8:
        case CSRawFormat::Rg8ui:
            dataLength = pixelCount * 2;
            cnvraw = malloc(dataLength);
            if (!cnvraw) {
                free(raw);
                CSErrorLog("bitmap texture not enough memory");
                return NULL;
            }
            for (int i = 0; i < pixelCount; i++) {
                uint p = ((uint*)raw)[i];
                ((ushort*)cnvraw)[i] = p & 0xffff;
            }
            free(raw);
            raw = cnvraw;
            break;
        case CSRawFormat::Rgb5:
            dataLength = pixelCount * 2;
            cnvraw = malloc(dataLength);
            if (!cnvraw) {
                free(raw);
                CSErrorLog("bitmap texture not enough memory");
                return NULL;
            }
            for (int i = 0; i < pixelCount; i++) {
                uint p = ((uint*)raw)[i];
                ((ushort*)cnvraw)[i] = (((p & 0xff) >> 3) << 11)
                    | ((((p >> 8) & 0xff) >> 2) << 5)
                    | (((p >> 16) & 0xff) >> 3);
            }
            free(raw);
            raw = cnvraw;
            break;
        case CSRawFormat::Rgb8:
        case CSRawFormat::Rgb8ui:
            dataLength = pixelCount * 3;
            cnvraw = malloc(dataLength);
            if (!cnvraw) {
                free(raw);
                CSErrorLog("bitmap texture not enough memory");
                return NULL;
            }
            for (int i = 0; i < pixelCount; i++) {
                uint p = ((uint*)raw)[i];
                ((byte*)cnvraw)[i * 3] = p & 0xff;
                ((byte*)cnvraw)[i * 3 + 1] = (p >> 8) & 0xff;
                ((byte*)cnvraw)[i * 3 + 2] = (p >> 16) & 0xff;
            }
            free(raw);
            raw = cnvraw;
            break;
        case CSRawFormat::Rgba4:
            dataLength = pixelCount * 2;
            cnvraw = malloc(dataLength);
            if (!cnvraw) {
                free(raw);
                CSErrorLog("bitmap texture not enough memory");
                return NULL;
            }
            for (int i = 0; i < pixelCount; i++) {
                uint p = ((uint*)raw)[i];
                ((ushort*)cnvraw)[i] = (((p & 0xff) >> 4) << 12)
                    | ((((p >> 8) & 0xff) >> 4) << 8)
                    | ((((p >> 16) & 0xff) >> 4) << 4)
                    | (((p >> 24) & 0xff) >> 4);
            }
            free(raw);
            raw = cnvraw;
            break;
        case CSRawFormat::Rgb5A1:
            dataLength = pixelCount * 2;
            cnvraw = malloc(dataLength);
            if (!cnvraw) {
                free(raw);
                CSErrorLog("bitmap texture not enough memory");
                return NULL;
            }
            for (int i = 0; i < pixelCount; i++) {
                uint p = ((uint*)raw)[i];
                ((ushort*)cnvraw)[i] = (((p & 0xff) >> 3) << 11)
                    | ((((p >> 8) & 0xff) >> 3) << 6)
                    | ((((p >> 16) & 0xff) >> 3) << 1)
                    | (((p >> 24) & 0xff) >> 7);
            }
            free(raw);
            raw = cnvraw;
            break;
        case CSRawFormat::Rgba8:
        case CSRawFormat::Rgba8ui:
            dataLength = pixelCount * 4;
            break;
        default:
            CSAssert(false, "unsupported bitmap texture:%d", desc.format);
            free(raw);
            return NULL;
    }
    desc.width = width;
    desc.height = height;
    CSTexture* result = new CSTexture(desc, raw, dataLength);
    free(raw);
    return result;
}

CSTexture* CSTexture::createWithContentOfFile(const CSTextureDescription& desc, const char* path) {
    int length = 0;
    void* source = CSFile::createRawWithContentOfFile(path, 0, length, false);
    if (!source) return NULL;
    CSTexture* result = createWithBitmap(desc, source, length);
    free(source);
    return result;
}

void CSTexture::load(CSGraphicsApi* api, CSBuffer* buffer) {
    _object = api->genTexture();

    api->bindTexture(_description.target, _object);

    applyTextureDescription(api, _description);

    const CSRawFormatEncoding& encoding = _description.format.encoding();

    switch (_description.target) {
        case CSTextureTargetCubeMap:
            for (int face = 0; face < 6; face++) {
                int levelCount = buffer->readByte();

                if (encoding.compressed && _description.mipmapCount > levelCount) {
                    _description.mipmapCount = levelCount;
                }

                for (int level = 0; level < levelCount; level++) {
                    int subwidth = buffer->readShort();
                    int subheight = buffer->readShort();
                    int subdepth = buffer->readShort();

                    upload(api, buffer, (CSTextureTarget)(CSTextureTargetCubeMapPositiveX + face), subwidth, subheight, subdepth, _description.format, level, 1);
                }
            }
            break;
        case CSTextureTarget2D:
        case CSTextureTarget3D:
            {
                int levelCount = buffer->readByte();

                if (encoding.compressed && _description.mipmapCount > levelCount) {
                    _description.mipmapCount = levelCount;
                }

                int subwidth = buffer->readShort();
                int subheight = buffer->readShort();
                int subdepth = buffer->readShort();

                for (int level = 0; level < levelCount; level++) {
                    upload(api, buffer, _description.target, subwidth, subheight, subdepth, _description.format, level, 1);
                }
            }
            break;
        default:
            CSAssert(false, "invalid target:%d", _description.target);
            break;
    }

    if (_allocated && !encoding.compressed && _description.mipmapCount > 1) {
        if (!api->generateMipmap(_description.target, _description.mipmapCount - 1)) {
            CSWarnLog("generate mipmap fail");
        }
    }
}

void CSTexture::allocate() {
    if (!_allocated) {
        _description.mipmapCount = 1;

        _allocated = true;
        
        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this] (CSGraphicsApi* api) {
            api->bindTexture(_description.target, _object);

            switch (_description.target) {
                case CSTextureTarget2D:
                case CSTextureTarget3D:
                case CSTextureTarget2DMultisample:
                    upload(api, NULL, _description.target, _description.width, _description.height, _description.depth, _description.format, 0, _description.samples);
                    break;
                case CSTextureTargetCubeMap:
                    for (int face = 0; face < 6; face++) {
                        upload(api, NULL, (CSTextureTarget)(CSTextureTargetCubeMapPositiveX + face), _description.width, _description.height, _description.depth, _description.format, 0, _description.samples);
                    }
                    break;
                default:
                    CSAssert(false, "invalid target:%d", _description.target);
                    break;
            }
        }, this);
        if (command) command->addFence(this, CSGBatchFlagReadWrite);
    }
}

int CSTexture::getCost2D() const {
    const CSRawFormatEncoding& encoding = _description.format.encoding();

    if (encoding.compressed) {
        int width = _description.width;
        int height = _description.height;
        int cost = 0;
        for (int i = 0; i < _description.mipmapCount; i++) {
            int blockWidth = (width + encoding.compressBlock - 1) / encoding.compressBlock;
            int blockHeight = (height + encoding.compressBlock - 1) / encoding.compressBlock;
            cost += blockWidth * blockHeight * encoding.compressBlockLength;

            width = CSMath::max(1, width >> 1);
            height = CSMath::max(1, height >> 1);
        }
        return cost;
    }
    else {
        int current = _description.width * _description.height * encoding.pixelBpp;
        int cost = current;
        for (int i = 1; i < _description.mipmapCount; i++) {
            current >>= 2;
            cost += current;
        }
        return cost;
    }
}

int CSTexture::getCost3D() const {
    const CSRawFormatEncoding& encoding = _description.format.encoding();

    if (encoding.compressed) {
        int width = _description.width;
        int height = _description.height;
        int depth = _description.depth;
        int cost = 0;
        for (int i = 0; i < _description.mipmapCount; i++) {
            int blockWidth = (width + encoding.compressBlock - 1) / encoding.compressBlock;
            int blockHeight = (height + encoding.compressBlock - 1) / encoding.compressBlock;
            int blockDepth = (depth + encoding.compressBlock - 1) / encoding.compressBlock;
            cost += blockWidth * blockHeight * blockDepth * encoding.compressBlockLength;

            width = CSMath::max(1, width >> 1);
            height = CSMath::max(1, height >> 1);
            depth = CSMath::max(1, depth >> 1);
        }
        return cost;
    }
    else
    {
        int current = _description.width * _description.height * _description.depth * encoding.pixelBpp;
        int cost = current;
        for (int i = 1; i < _description.mipmapCount; i++) {
            current >>= 3;
            cost += current;
        }
        return cost;
    }
}

int CSTexture::resourceCost() const {
    int cost = sizeof(CSTexture);

    if (_allocated) {
        switch (_description.target) {
            case CSTextureTarget2D:
                cost += getCost2D();
                break;
            case CSTextureTarget3D:
                cost += getCost3D();
                break;
            case CSTextureTargetCubeMap:
                cost += getCost2D() * 6;
                break;
            case CSTextureTarget2DMultisample:
                cost += getCost2D() * _description.samples;
                break;
            default:
                CSAssert(false, "invalid target:%d", _description.target);
                break;
        }
    }

    return cost;
}

void CSTexture::uploadFail() {
    CSErrorLog("upload fail:target:%d format:%d size:%d, %d, %d", _description.target, _description.format, _description.width, _description.height, _description.depth);
    _allocated = false;
}

void CSTexture::upload(CSGraphicsApi* api, CSBuffer* buffer, CSTextureTarget target, int width, int height, int depth, CSRawFormat format, int level, int samples) {
    const CSRawFormatEncoding& encoding = format.encoding();

    if (encoding.compressed) {
        CSAssert(buffer, "no compressed content");

        switch (target) {
            case CSTextureTarget2D:
            case CSTextureTargetCubeMapPositiveX:
            case CSTextureTargetCubeMapNegativeX:
            case CSTextureTargetCubeMapPositiveY:
            case CSTextureTargetCubeMapNegativeY:
            case CSTextureTargetCubeMapPositiveZ:
            case CSTextureTargetCubeMapNegativeZ:
                {
                    int blockWidth = (width + encoding.compressBlock - 1) / encoding.compressBlock;
                    int blockHeight = (height + encoding.compressBlock - 1) / encoding.compressBlock;
                    width = blockWidth * encoding.compressBlock;
                    height = blockHeight * encoding.compressBlock;
                    int dataLength = blockWidth * blockHeight * encoding.compressBlockLength;

                    if (!api->compressedTexImage2D(target, level, format, width, height, dataLength, buffer->bytes())) uploadFail();

                    buffer->skip(dataLength);
                }
                break;
            case CSTextureTarget3D:
                {
                    int blockWidth = (width + encoding.compressBlock - 1) / encoding.compressBlock;
                    int blockHeight = (height + encoding.compressBlock - 1) / encoding.compressBlock;
                    int blockDepth = (depth + encoding.compressBlock - 1) / encoding.compressBlock;
                    width = blockWidth * encoding.compressBlock;
                    height = blockHeight * encoding.compressBlock;
                    depth = blockDepth * encoding.compressBlock;
                    int dataLength = blockWidth * blockHeight * blockDepth * encoding.compressBlockLength;

                    if (!api->compressedTexImage2D(target, level, format, width, height, dataLength, buffer->bytes())) uploadFail();

                    buffer->skip(dataLength);
                }
                break;
            default:
                CSAssert(false, "unsupported compressed target:%d", target);
                break;
        }
    }
    else {
        switch (target) {
            case CSTextureTarget2D:
            case CSTextureTargetCubeMapPositiveX:
            case CSTextureTargetCubeMapNegativeX:
            case CSTextureTargetCubeMapPositiveY:
            case CSTextureTargetCubeMapNegativeY:
            case CSTextureTargetCubeMapPositiveZ:
            case CSTextureTargetCubeMapNegativeZ:
                if (buffer) {
                    int dataLength = width * height * encoding.pixelBpp;
                    if (!api->texImage2D(target, level, format, width, height, buffer->bytes())) uploadFail();
                    buffer->skip(dataLength);
                }
                else if (!api->texImage2D(target, level, format, width, height, NULL)) uploadFail();
                break;
            case CSTextureTarget2DMultisample:
                CSAssert(buffer == NULL, "unsupported raw target with data:%d", target);
                if (!api->texImage2DMultisample(target, samples, format, width, height)) uploadFail();
                break;
            case CSTextureTarget3D:
                if (buffer) {
                    int dataLength = width * height * depth * encoding.pixelBpp;
                    if (!api->texImage3D(target, level, format, width, height, depth, buffer->bytes())) uploadFail();
                    buffer->skip(dataLength);
                }
                else if (!api->texImage3D(target, level, format, width, height, depth, NULL)) uploadFail();
                break;
            default:
                CSAssert(false, "unsupported raw target:%d", target);
                break;
        }
    }
}

void CSTexture::upload(const void* raw, int rawLength, int level, int width, int height, int depth) {
    const CSRawFormatEncoding& encoding = _description.format.encoding();

    CSAssert(!encoding.compressed, "compressed format unable to upload dynamically");

    CSGraphicsApi* api;

    switch (_description.target) {
        case CSTextureTarget2D:
            CSAssert(rawLength == width * height * encoding.pixelBpp, "invalid data length");

            if (level == 0) {
                _description.width = width;
                _description.height = height;
                _allocated = true;
            }
            else {
                CSAssert(_allocated && width == (_description.width >> level) && height == (_description.height >> level), "should be allocated and matched with size");
            }
            if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
                api->bindTexture(CSTextureTarget2D, _object);
                if (!api->texImage2D(CSTextureTarget2D, level, _description.format, width, height, raw)) uploadFail();
            }
            else {
                CSData* copyRaw = NULL;
                if (raw) {
                    copyRaw = CSData::dataWithBytes(raw, rawLength);
                    if (!copyRaw) {
                        uploadFail();
                        return;
                    }
                }
                CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, level, copyRaw, width, height](CSGraphicsApi* api) {
                    api->bindTexture(CSTextureTarget2D, _object);
                    if (!api->texImage2D(CSTextureTarget2D, level, _description.format, width, height, copyRaw->bytes())) uploadFail();
                }, this, copyRaw);
                if (command) command->addFence(this, CSGBatchFlagWrite);
            }
            break;
        case CSTextureTarget3D:
            CSAssert(rawLength == width * height * depth * encoding.pixelBpp, "invalid data length");

            if (level == 0) {
                _description.width = width;
                _description.height = height;
                _description.depth = depth;
                _allocated = true;
            }
            else {
                CSAssert(_allocated && width == (_description.width >> level) && height == (_description.height >> level) && depth == (_description.depth >> level), "should be allocated and matched with size");
            }
            if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
                api->bindTexture(CSTextureTarget3D, _object);
                if (!api->texImage3D(CSTextureTarget3D, level, _description.format, width, height, depth, raw)) uploadFail();
            }
            else {
                CSData* copyRaw = NULL;
                if (raw) {
                    copyRaw = CSData::dataWithBytes(raw, rawLength);
                    if (!copyRaw) {
                        uploadFail();
                        return;
                    }
                }
                CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, level, copyRaw, width, height, depth](CSGraphicsApi* api) {
                    api->bindTexture(CSTextureTarget3D, _object);
                    if (!api->texImage3D(CSTextureTarget3D, level, _description.format, width, height, depth, copyRaw->bytes())) uploadFail();
                }, this, copyRaw);
                if (command) command->addFence(this, CSGBatchFlagWrite);
            }
            break;
        default:
            CSAssert(false, "invalid target:%d", _description.target);
            break;
    }
}

bool CSTexture::uploadSub(const void* raw, int rawLength, int level, int x, int y, int z, int width, int height, int depth) {
    const CSRawFormatEncoding& encoding = _description.format.encoding();

    CSAssert(_allocated, "should be allocated and matched with size");
    CSAssert(!encoding.compressed, "compressed format unable to upload dynamically");
    CSAssert(x >= 0 && y >= 0 && x + width <= (_description.width >> level) && y + height <= (_description.height >> level), "invalid upload area");

    CSGraphicsApi* api;

    switch (_description.target) {
        case CSTextureTarget2D:
            CSAssert(rawLength == width * height * encoding.pixelBpp, "invalid data length");

            if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
                api->bindTexture(CSTextureTarget2D, _object);
                api->texSubImage2D(CSTextureTarget2D, level, _description.format, CSBounds2(x, y, width, height), raw);
            }
            else {
                CSData* copyRaw = NULL;
                if (raw) {
                    copyRaw = CSData::dataWithBytes(raw, rawLength);
                    if (!copyRaw) return false;
                }
                CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, level, copyRaw, x, y, width, height](CSGraphicsApi* api) {
                    api->bindTexture(CSTextureTarget2D, _object);
                    api->texSubImage2D(CSTextureTarget2D, level, _description.format, CSBounds2(x, y, width, height), copyRaw->bytes());
                }, this, copyRaw);
                if (command) command->addFence(this, CSGBatchFlagWrite);
            }
            return true;
        case CSTextureTarget3D:
            CSAssert(rawLength == width * height * depth * encoding.pixelBpp, "invalid data length");
            CSAssert(z >= 0 && z + depth <= (_description.depth >> level), "invalid upload area");

            if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
                api->bindTexture(CSTextureTarget3D, _object);
                api->texSubImage3D(CSTextureTarget3D, level, _description.format, CSBounds3(x, y, z, width, height, depth), raw);
            }
            else {
                CSData* copyRaw = NULL;
                if (raw) {
                    copyRaw = CSData::dataWithBytes(raw, rawLength);
                    if (!copyRaw) return false;
                }
                CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, level, copyRaw, x, y, z, width, height, depth](CSGraphicsApi* api) {
                    api->bindTexture(CSTextureTarget3D, _object);
                    api->texSubImage3D(CSTextureTarget3D, level, _description.format, CSBounds3(x, y, z, width, height, depth), copyRaw->bytes());
                }, this, copyRaw);
                if (command) command->addFence(this, CSGBatchFlagWrite);
            }
            return true;
        default:
            CSAssert(false, "invalid target:%d", _description.target);
            return false;
    }
}

void CSTexture::resize(int width, int height) {
    _description.width = width;
    _description.height = height;

    if (_allocated) {
        _allocated = false;

        allocate();
    }
}

bool CSTexture::reload(CSTextureDescription desc) {
    if (desc.target == 0) desc.target = _description.target;
    else if (desc.target != _description.target) return false;

    if (desc.width == 0) desc.width = _description.width;
    else if (desc.width != _description.width) return false;

    if (desc.height == 0) desc.height = _description.height;
    else if (desc.height != _description.height) return false;

    if (desc.depth == 0) desc.depth = _description.depth;
    else if (desc.depth != _description.depth) return false;

    if (desc.format == 0) desc.format = _description.format;
    else if (desc.format != _description.format) return false;

    if (desc.wrapS == 0) desc.wrapS = _description.wrapS;
    if (desc.wrapT == 0) desc.wrapT = _description.wrapT;
    if (desc.wrapR == 0) desc.wrapR = _description.wrapR;
    if (desc.minFilter == 0) desc.minFilter = _description.minFilter;
    if (desc.magFilter == 0) desc.magFilter = _description.magFilter;

    bool generateMipmap = false;
    if (_description.mipmapCount > 1 || desc.format.encoding().compressed) desc.mipmapCount = _description.mipmapCount;
    else if (_allocated && desc.mipmapCount > 1) {
        desc.mipmapCount = CSMath::min((int)desc.mipmapCount, desc.maxMipmapCount());
        if (desc.mipmapCount > 1) generateMipmap = true;
    }
    else desc.mipmapCount = 1;

    if (_description != desc) {
        _description = desc;

        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, generateMipmap](CSGraphicsApi* api) {
            api->bindTexture(_description.target, _object);
            applyTextureDescription(api, _description);
            if (generateMipmap && !api->generateMipmap(_description.target, _description.mipmapCount - 1)) {
                CSWarnLog("generate mipmap fail");
            }
        }, this);
        if (command) command->addFence(this, CSGBatchFlagReadWrite);
    }

    return true;
}

static void applyTextureDescription(CSGraphicsApi* api, const CSTextureDescription& desc) {
    if (desc.target == CSTextureTarget2DMultisample) return;      //Texture2DMultisample not support TexParameter

    api->texParameterWrap(desc.target, 0, desc.wrapS);
    api->texParameterWrap(desc.target, 1, desc.wrapT);
    api->texParameterWrap(desc.target, 2, desc.wrapR);
    api->texParameterMinFilter(desc.target, desc.minFilter);
    api->texParameterMagFilter(desc.target, desc.magFilter);
    api->texParameterBorderColor(desc.target, desc.borderColor);
}
