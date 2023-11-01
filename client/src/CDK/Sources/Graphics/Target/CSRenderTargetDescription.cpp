#define CDK_IMPL

#include "CSRenderTargetDescription.h"

CSRenderTargetDescription::Attachment::Attachment() {
    memset(this, 0, sizeof(CSRenderTargetDescription::Attachment));

    samples = 1;
}

uint CSRenderTargetDescription::Attachment::hash() const {
    CSHash hash;
    hash.combine(attachment);
    hash.combine(format);
    hash.combine(samples);
    hash.combine(texture);
    hash.combine(textureLayer);
    hash.combine(textureTarget);
    hash.combine(textureWrapS);
    hash.combine(textureWrapT);
    hash.combine(textureWrapR);
    hash.combine(textureMinFilter);
    hash.combine(textureMagFilter);
    hash.combine(textureBorderColor);
    return hash;
}

CSRenderTargetDescription::CSRenderTargetDescription(const CSRenderTargetDescription& other) : width(other.width), height(other.height), attachments(other.attachments.capacity()) {
    attachments.addObjectsFromArray(&other.attachments);
}

CSRenderTargetDescription& CSRenderTargetDescription::operator=(const CSRenderTargetDescription& other) {
    width = other.width;
    height = other.height;
    attachments.removeAllObjects();
    attachments.addObjectsFromArray(&other.attachments);
    return *this;
}

void CSRenderTargetDescription::validate() const {
#ifdef CS_ASSERT_DEBUG
    CSAssert(width > 0 && height > 0);
    foreach (const Attachment&, a, &attachments) CSAssert(a.format != CSRawFormat::None && !a.format.encoding().compressed && a.samples >= 1);
#endif
}

uint CSRenderTargetDescription::hash() const {
    CSHash hash;
    hash.combine(width);
    hash.combine(height);
    hash.combine(attachments.sequentialHash());
    return hash;
}

bool CSRenderTargetDescription::operator ==(const CSRenderTargetDescription& other) const {
    return width == other.width && 
        height == other.height && 
        attachments.count() == other.attachments.count() && 
        memcmp(attachments.pointer(), other.attachments.pointer(), attachments.count() * sizeof(Attachment)) == 0;
}
