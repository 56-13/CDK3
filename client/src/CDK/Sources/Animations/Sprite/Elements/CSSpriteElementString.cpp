#define CDK_IMPL

#include "CSSpriteElementString.h"
#include "CSSprite.h"
#include "CSResourcePool.h"
#include "CSBuffer.h"

CSSpriteElementString::Content::Content() : _type(TypeNone) {
		
}

CSSpriteElementString::Content::Content(const string& content) : _type(TypeString) {
	_var.content = retain(content.content());
}

CSSpriteElementString::Content::Content(const CSLocaleString* content) : _type(TypeLocaleString) {
	_var.localeContent = retain(content);
}

CSSpriteElementString::Content::Content(const CSArray<ushort>* indices, bool locale) : _type(locale ? TypeLocaleStringRef : TypeStringRef) {
	_var.indices = retain(indices);
}

CSSpriteElementString::Content::Content(CSBuffer* buffer) : _type((Type)buffer->readByte()) {
	switch (_type) {
		case TypeString:
			_var.content = retain(buffer->readString().content());
			break;
		case TypeLocaleString:
			_var.localeContent = retain(buffer->readLocaleString());
			break;
		case TypeStringRef:
		case TypeLocaleStringRef:
			_var.indices = retain(buffer->readArray<ushort>());
			break;
	}
}

CSSpriteElementString::Content::~Content() {
	switch (_type) {
		case TypeString:
			release(_var.content);
			break;
		case TypeLocaleString:
			release(_var.localeContent);
			break;
		case TypeStringRef:
		case TypeLocaleStringRef:
			release(_var.indices);
			break;
	}
}

CSSpriteElementString::Content::Content(const Content& other) : _type(other._type) {
	switch (_type) {
		case TypeString:
			_var.content = retain(other._var.content);
			break;
		case TypeLocaleString:
			_var.localeContent = retain(other._var.localeContent);
			break;
		case TypeStringRef:
		case TypeLocaleStringRef:
			_var.indices = retain(other._var.indices);
			break;
	}
}

CSSpriteElementString::Content& CSSpriteElementString::Content::operator=(const Content& other) {
	if (_type == other._type) {
		switch (_type) {
			case TypeString:
				retain(_var.content, other._var.content);
				break;
			case TypeLocaleString:
				retain(_var.localeContent, other._var.localeContent);
				break;
			case TypeStringRef:
			case TypeLocaleStringRef:
				retain(_var.indices, other._var.indices);
				break;
		}
	}
	else {
		this->~Content();
		new (this) Content(other);
	}
	return *this;
}

CSSpriteElementString::Content::operator string() const {
	switch (_type) {
		case TypeString:
			return _var.content;
		case TypeLocaleString:
			return _var.localeContent->value();
		case TypeStringRef:
			return static_assert_cast<const CSStringContent*>(CSResourcePool::sharedPool()->load(CSResourceTypeString, _var.indices));
		case TypeLocaleStringRef:
			{
				const CSLocaleString* lstr = static_assert_cast<const CSLocaleString*>(CSResourcePool::sharedPool()->load(CSResourceTypeLocaleString, _var.indices));
				return lstr ? lstr->value() : NULL;
			}
	}
	return NULL;
}

int CSSpriteElementString::Content::resourceCost() const {
	switch (_type) {
		case TypeString:
			if (_var.content) return _var.content->resourceCost();
			break;
		case TypeLocaleString:
			return _var.localeContent->resourceCost();
		case TypeStringRef:
		case TypeLocaleStringRef:
			if (_var.indices) return sizeof(CSArray<ushort>) + _var.indices->capacity() * sizeof(ushort);
	}
	return 0;
}

void CSSpriteElementString::Content::preload() const {
	switch (_type) {
		case TypeStringRef:
			CSResourcePool::sharedPool()->load(CSResourceTypeString, _var.indices);
			break;
		case TypeLocaleStringRef:
			CSResourcePool::sharedPool()->load(CSResourceTypeLocaleString, _var.indices);
			break;
	}
}

//=====================================================================================

CSSpriteElementString::CSSpriteElementString() : font(CSGraphics::defaultFont()) {

}

CSSpriteElementString::CSSpriteElementString(CSBuffer* buffer) :
	content(buffer),
	startLength(buffer->readInt()),
	endLength(buffer->readInt()),
	position(buffer),
	x(CSAnimationFloat::factorWithBuffer(buffer)),
	y(CSAnimationFloat::factorWithBuffer(buffer)),
	z(CSAnimationFloat::factorWithBuffer(buffer)),
	align((CSAlign)buffer->readByte()),
	scaling((Scaling)buffer->readByte())
{
	switch (scaling) {
		case ScalingHorizontal:
			scalingWidth = buffer->readFloat();
			break;
		case ScalingBoth:
			scalingWidth = buffer->readFloat();
			scalingHeight = buffer->readFloat();
			break;
	}
	if (buffer->readBoolean()) {
		const CSFont* originFont = CSGraphics::defaultFont();

		string name = buffer->readString();
		float size = buffer->readFloat();
		int style = buffer->readByte();

		if (size <= 0) {
			size = originFont->size();
		}
		if (style < 0) {
			style = originFont->style();
		}
		font = name ? CSFont::font(name, size, (CSFontStyle)style) : originFont->derivedFont(size, (CSFontStyle)style);
	}
	else font = CSGraphics::defaultFont();

	material = CSMaterialSource::materialWithBuffer(buffer);
}

CSSpriteElementString::CSSpriteElementString(const CSSpriteElementString* other) :
	content(other->content),
	startLength(other->startLength),
	endLength(other->endLength),
	position(other->position),
	x(CSAnimationFloat::factorWithFactor(other->x)),
	y(CSAnimationFloat::factorWithFactor(other->y)),
	z(CSAnimationFloat::factorWithFactor(other->z)),
	align(other->align),
	scaling(other->scaling),
	scalingWidth(other->scalingWidth),
	scalingHeight(other->scalingHeight),
	font(other->font),
	material(CSMaterialSource::materialWithMaterial(other->material)) 
{

}

int CSSpriteElementString::resourceCost() const {
	int cost = sizeof(CSSpriteElementString);
	cost += content.resourceCost();
	if (x) cost += x->resourceCost();
	if (y) cost += y->resourceCost();
	if (z) cost += z->resourceCost();
	if (material) cost += material->resourceCost();
	return cost;
}

void CSSpriteElementString::preload() const {
	content.preload();
	if (material) material->preload();
}

CSVector3 CSSpriteElementString::getPosition(float progress, int random) const {
	CSVector3 pos = position;
	if (x) pos.x += x->value(progress, CSRandom::toFloatSequenced(random, 0));
	if (y) pos.y += y->value(progress, CSRandom::toFloatSequenced(random, 1));
	if (z) pos.z += z->value(progress, CSRandom::toFloatSequenced(random, 2));
	return pos;
}

bool CSSpriteElementString::addAABB(TransformParam& param, CSABoundingBox& result) const {
	const string& str = content;

	if (str) {
		CSZRect rect;
		rect.origin() = getPosition(param.progress, param.random);
		switch (scaling) {
			case ScalingNone:
				rect.size() = CSGraphics::stringSize(str, font);
				break;
			case ScalingHorizontal:
				rect.size() = CSGraphics::stringSize(str, font, scalingWidth);
				break;
			case ScalingBoth:
				rect.size() = CSGraphics::stringSize(str, font);
				if (rect.width > scalingWidth) rect.size() *= scalingWidth / rect.width;
				if (rect.height > scalingHeight) rect.size() *= scalingHeight / rect.height;
				break;
		}

		CSGraphics::applyAlign(rect.origin(), rect.size(), align);
					
		result.append(CSVector3::transformCoordinate(rect.leftTop(), param.transform));
		result.append(CSVector3::transformCoordinate(rect.rightTop(), param.transform));
		result.append(CSVector3::transformCoordinate(rect.leftBottom(), param.transform));
		result.append(CSVector3::transformCoordinate(rect.rightBottom(), param.transform));

		return true;
	}
	return false;
}

void CSSpriteElementString::getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
	const string& str = content;

	if (str) {
		if ((param.inflags & CSSceneObject::UpdateFlagTransform) || (x && x->animating()) || (y && y->animating()) || (z && z->animating())) {
			outflags |= CSSceneObject::UpdateFlagAABB;
		}
	}
}

void CSSpriteElementString::draw(DrawParam& param) const {
	if (CSMaterialSource::apply(material, param.graphics, param.layer, param.parent->progress(), param.random, NULL, false)) {
		const string& str = content;

		if (str) {
			int length = endLength > startLength ? (int)CSMath::round(CSMath::lerp(startLength, endLength, param.progress)) : startLength;

			if (length > 0) {
				CSVector3 pos = getPosition(param.progress, param.random);

				param.graphics->setFont(font);
				switch (scaling) {
					case ScalingNone:
						param.graphics->drawString(CSGraphics::StringParam(str, 0, length), pos, align);
						break;
					case ScalingBoth:
						if (scalingWidth > 0 && scalingHeight > 0) {
							param.graphics->drawStringScaled(CSGraphics::StringParam(str, 0, length), CSZRect(pos, scalingWidth, scalingHeight), align);
						}
						break;
					case ScalingHorizontal:
						if (scalingWidth > 0) {
							param.graphics->drawStringScaled(CSGraphics::StringParam(str, 0, length), pos, align, scalingWidth);
						}
						break;
				}
			}
		}
	}
}

