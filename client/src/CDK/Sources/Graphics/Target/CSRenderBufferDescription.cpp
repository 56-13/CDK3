#define CDK_IMPL

#include "CSRenderBufferDescription.h"

#include "CSEntry.h"

void CSRenderBufferDescription::validate() const {
	CSAssert(width > 0 && height > 0 && format != CSRawFormat::None && !format.encoding().compressed && samples >= 1);
}

uint CSRenderBufferDescription::hash() const {
	CSHash hash;
	hash.combine(width);
	hash.combine(height);
	hash.combine(format);
	hash.combine(samples);
	return hash;
}

bool CSRenderBufferDescription::operator ==(const CSRenderBufferDescription& other) const {
	return width == other.width &&
		height == other.height &&
		format == other.format &&
		samples == other.samples;
}
