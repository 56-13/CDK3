#define CDK_IMPL

#include "CSTextures.h"

#include "CSStringLiteral.h"

#include "CSResourcePool.h"

#include "CSFile.h"
#include "CSBuffer.h"

CSTexture* CSTextures::brdf() {
    static const string& key = S("BRDF");

    CSTexture* texture = static_assert_cast<CSTexture*>(CSResourcePool::sharedPool()->get(key));

    if (!texture) {
        CSBuffer* buffer = CSBuffer::createWithContentOfFile(CSFile::bundlePath("brdf.dat"));
        texture = new CSTexture(buffer);
        buffer->release();
        CSResourcePool::sharedPool()->add(key, texture, 0, false);
        texture->release();
    }
    return texture;
}

CSTexture* CSTextures::get(const CSObject* key, int life, bool recycle, const CSTextureDescription& desc, bool allocate) {
    desc.validate();

    auto match = [desc](const CSResource* candidate) -> bool {
        return candidate->resourceType() == CSResourceTypeTexture && static_cast<const CSTexture*>(candidate)->description() == desc;
    };
    CSResource* resource;
    if (CSResourcePool::sharedPool()->recycle(match, key, life, resource)) {
        CSTexture* texture = static_cast<CSTexture*>(resource);
        if (allocate) texture->allocate();
        return texture;
    }
    CSTexture* newTexture = new CSTexture(desc, allocate);
    CSResourcePool::sharedPool()->add(key, newTexture, life, recycle);
    newTexture->release();
    return newTexture;
}
