#define CDK_IMPL

#include "CSMaterialSource.h"
#include "CSResourcePool.h"
#include "CSBuffer.h"
#include "CSRandom.h"
#include "CSSceneObject.h"

CSMaterialSource::Map::Map(const CSTexture* texture) : _type(Embed), _texture(retain(texture)) {

}

CSMaterialSource::Map::Map(CSBuffer* buffer) : _type((Type)buffer->readByte()) {
	switch (_type) {
		case Embed:
			_texture = new CSTexture(buffer);
			break;
		case MaterialRef:
		case TextureRef:
			_indices = retain(buffer->readArray<ushort>());
			break;
	}
}

CSMaterialSource::Map::Map(const Map& other) : _type(other._type), _texture(retain(other._texture)), _indices(retain(other._indices)) {

}

CSMaterialSource::Map::~Map() {
	release(_texture);
	release(_indices);
}

CSMaterialSource::Map& CSMaterialSource::Map::operator=(const Map& other) {
	_type = other._type;
	retain(_texture, other._texture);
	retain(_indices, other._indices);
	return *this;
}

int CSMaterialSource::Map::resourceCost() const {
	switch (_type) {
		case Embed:
			return _texture->resourceCost();
		case MaterialRef:
		case TextureRef:
			if (_indices) return sizeof(CSArray<ushort>) + _indices->capacity() * sizeof(ushort);
			break;
	}
	return 0;
}

void CSMaterialSource::Map::preload() const {
	switch (_type) {
		case MaterialRef:
			{
				const CSMaterialSource* material = static_assert_cast<const CSMaterialSource*>(CSResourcePool::sharedPool()->load(CSResourceTypeMaterial, _indices));
				if (material) material->preload();
			}
			break;
		case TextureRef:
			CSResourcePool::sharedPool()->load(CSResourceTypeTexture, _indices);
			break;
	}
}

const CSTexture* CSMaterialSource::Map::content(int i) const {
	switch (_type) {
		case Embed:
			return _texture;
		case MaterialRef:
			{
				const CSMaterialSource* material = static_assert_cast<const CSMaterialSource*>(CSResourcePool::sharedPool()->load(CSResourceTypeMaterial, _indices));
				if (material) {
					const Local* local = material->retrieve();
					if (local) {
						switch (i) {
							case 0:
								return local->colorMap.content(i);
							case 1:
								return local->normalMap.content(i);
							case 2:
								return local->materialMap.content(i);
							case 3:
								return local->emissiveMap.content(i);
						}
					}
				}
			}
			break;
		case TextureRef:
			return static_assert_cast<const CSTexture*>(CSResourcePool::sharedPool()->load(CSResourceTypeTexture, _indices));
	}
	return NULL;
}

CSMaterialSource::Local::Local(CSBuffer* buffer) :
	blendLayer((CSInstanceBlendLayer)buffer->readByte()),
	shader((CSMaterial::Shader)buffer->readByte()),
	blendMode((CSBlendMode)buffer->readByte()),
	cullMode((CSCullMode)buffer->readByte()),
	materialMapComponents(buffer->readByte()),
	depthTest(buffer->readBoolean()),
	alphaTest(buffer->readBoolean()),
	depthBias(buffer->readFloat()),
	alphaTestBias(buffer->readFloat()),
	displacementScale(buffer->readFloat()),
	distortionScale(buffer->readFloat()),
	colorMap(buffer),
	normalMap(buffer),
	materialMap(buffer),
	emissiveMap(buffer),
	color(CSAnimationColor::colorWithBuffer(buffer)),
	colorDuration(buffer->readFloat()),
	colorLoop(buffer),
	bloom(buffer->readBoolean()),
	reflection(buffer->readBoolean()),
	receiveShadow(buffer->readBoolean()),
	receiveShadow2D(buffer->readBoolean()),
	metallic(buffer->readFloat()),
	roughness(buffer->readFloat()),
	ambientOcclusion(buffer->readFloat()),
	rim(buffer->readFloat()),
	emission(CSAnimationColor::colorWithBuffer(buffer)),
	emissionDuration(buffer->readFloat()),
	emissionLoop(buffer),
	uvScroll(buffer)
{

}

CSMaterialSource::CSMaterialSource() : _local(new Local()) {

}

CSMaterialSource::CSMaterialSource(CSBuffer* buffer) {
	if (buffer->readBoolean()) _local = new Local(buffer);
	else _origin = retain(buffer->readArray<ushort>());
}

CSMaterialSource::CSMaterialSource(const CSMaterialSource* other) {
	const Local* local = other->retrieve();
	if (local) _local = new Local(*local);
	else _local = new Local();
}

CSMaterialSource::~CSMaterialSource() {
	if (_local) delete _local;
	else release(_origin);
}

int CSMaterialSource::resourceCost() const {
	int cost = sizeof(CSMaterialSource);
	if (_local) {
		cost += sizeof(Local);
		cost += _local->colorMap.resourceCost();
		cost += _local->normalMap.resourceCost();
		cost += _local->materialMap.resourceCost();
		cost += _local->emissiveMap.resourceCost();
		if (_local->color) cost += _local->color->resourceCost();
		if (_local->emission) cost += _local->emission->resourceCost();
	}
	else if (_origin) cost += sizeof(CSArray<ushort>) + _origin->capacity() * sizeof(ushort);
	return cost;
}

const CSMaterialSource::Local* CSMaterialSource::retrieve() const {
	if (_local) return _local;
	const CSMaterialSource* origin = static_assert_cast<CSMaterialSource*>(CSResourcePool::sharedPool()->load(CSResourceTypeMaterial, _origin));
	return origin->retrieve();
}

uint CSMaterialSource::showFlags() const {
	const CSMaterialSource::Local* local = retrieve();
	
	if (!local) return 0;
	
	uint showFlags = 0;
	if (_local->shader == CSMaterial::ShaderDistortion) showFlags |= CSSceneObject::ShowFlagDistortion;
	return showFlags;
}

CSInstanceBlendLayer CSMaterialSource::blendLayer() const {
	const CSMaterialSource::Local* local = retrieve();

	return local ? local->blendLayer : CSInstanceBlendLayerMiddle;
}

bool CSMaterialSource::getMaterial(float progress, int random, CSMaterial& result) const {
	const CSMaterialSource::Local* local = retrieve();

	if (!local) return false;

	result.shader = local->shader;
	result.blendMode = local->blendMode;
	result.cullMode = local->cullMode;
	result.materialMapComponents = local->materialMapComponents;
	result.depthTest = local->depthTest;
	result.alphaTest = local->alphaTest;
	result.depthBias = local->depthBias;
	result.alphaTestBias = local->alphaTestBias;
	result.displacementScale = local->displacementScale;
	result.distortionScale = local->distortionScale;
	result.colorMap = local->colorMap.content(0);
	result.normalMap = local->normalMap.content(1);
	result.materialMap = local->materialMap.content(2);
	result.emissiveMap = local->emissiveMap.content(3);

	if (local->color) {
		if (local->colorDuration) {
			int randomSeq0, randomSeq1;
			float cp = local->colorLoop.getProgress(progress / local->colorDuration, &randomSeq0, &randomSeq1);
			randomSeq0 *= 7;
			randomSeq1 *= 7;
			CSColor cr(
				CSRandom::toFloatSequenced(random, randomSeq0, randomSeq1, cp),
				CSRandom::toFloatSequenced(random, randomSeq0 + 1, randomSeq1 + 1, cp),
				CSRandom::toFloatSequenced(random, randomSeq0 + 2, randomSeq1 + 2, cp),
				CSRandom::toFloatSequenced(random, randomSeq0 + 3, randomSeq1 + 3, cp));

			result.color = local->color->value(cp, cr, CSColor::White);
		}
		else {
			CSColor cr(
				CSRandom::toFloatSequenced(random, 0),
				CSRandom::toFloatSequenced(random, 1),
				CSRandom::toFloatSequenced(random, 2),
				CSRandom::toFloatSequenced(random, 3));

			result.color = local->color->value(1, cr, CSColor::White);
		}
	}
	else result.color = CSColor::White;

	result.bloom = local->bloom;
	result.reflection = local->reflection;
	result.receiveShadow = local->receiveShadow;
	result.receiveShadow2D = local->receiveShadow2D;
	result.metallic = local->metallic;
	result.roughness = local->roughness;
	result.ambientOcclusion = local->ambientOcclusion;
	result.rim = local->rim;

	if (local->emission) {
		if (local->emissionDuration) {
			int randomSeq0, randomSeq1;
			float cp = local->emissionLoop.getProgress(progress / local->emissionDuration, &randomSeq0, &randomSeq1);
			randomSeq0 *= 7;
			randomSeq1 *= 7;
			CSColor cr(
				CSRandom::toFloatSequenced(random, randomSeq0, randomSeq1, cp),
				CSRandom::toFloatSequenced(random, randomSeq0 + 1, randomSeq1 + 1, cp),
				CSRandom::toFloatSequenced(random, randomSeq0 + 2, randomSeq1 + 2, cp),
				CSRandom::toFloatSequenced(random, randomSeq0 + 3, randomSeq1 + 3, cp));

			result.emission = (CSColor3)local->emission->value(cp, cr, CSColor::Black);
		}
		else {
			CSColor cr(
				CSRandom::toFloatSequenced(random, 0),
				CSRandom::toFloatSequenced(random, 1),
				CSRandom::toFloatSequenced(random, 2),
				CSRandom::toFloatSequenced(random, 3));

			result.emission = (CSColor3)local->emission->value(1, cr, CSColor::Black);
		}
	}
	else result.emission = CSColor3::Black;

	result.uvOffset = local->uvScroll * progress;

	return true;
}

bool CSMaterialSource::apply(const CSMaterialSource* source, CSGraphics* graphics, CSInstanceLayer layer, float progress, int random, const CSArray<CSVertexArrayInstance>** instances, bool push) {
	if (source && !source->_local) {
		const CSMaterialSource* origin = static_assert_cast<CSMaterialSource*>(CSResourcePool::sharedPool()->load(CSResourceTypeMaterial, source->_origin));

		apply(origin, graphics, layer, progress, random, instances, push);
	}
	else {
		switch (layer) {
			case CSInstanceLayerNone:
				{
					float a = graphics->color().a;
					if ((source && source->_local->shader == CSMaterial::ShaderDistortion) || a <= 0) break;
					if (push) graphics->push();
					CSMaterial& material = graphics->material();
					if (!source || !source->getMaterial(progress, random, material)) material = CSMaterial::Default;
					material.receiveShadow = false;
					material.receiveShadow2D = false;
					if (a < 1 && material.blendMode == CSBlendNone) material.blendMode = CSBlendAlpha;
				}
				return true;
			case CSInstanceLayerShadow:
				{
					if ((source && !source->_local->receiveShadow)) break;
					if (push) graphics->push();
					CSMaterial& material = graphics->material();
					if (!source || !source->getMaterial(progress, random, material)) material = CSMaterial::Default;
				}
				return true;
			case CSInstanceLayerShadow2D:
				{
					if (!source || !source->_local->receiveShadow2D || graphics->color().a <= 0) break;
					if (push) graphics->push();
					CSMaterial& material = graphics->material();
					if (!source || !source->getMaterial(progress, random, material)) material = CSMaterial::Default;
				}
				return true;
			case CSInstanceLayerBase:
				{
					float a = graphics->color().a;
					if ((source && source->_local->shader == CSMaterial::ShaderDistortion) || a <= 0) break;
					bool blending = (source && source->_local->blendMode != CSBlendNone) || a < 1;			//default blend should be none
					if (blending) break;
					if (instances && *instances) {
						*instances = (*instances)->where([](const CSVertexArrayInstance& i) -> bool { return i.color.a >= 1; });
						if ((*instances)->count() == 0) break;
					}
					if (push) graphics->push();
					CSMaterial& material = graphics->material();
					if (!source || !source->getMaterial(progress, random, material)) material = CSMaterial::Default;
				}
				return true;
			case CSInstanceLayerBlendBottom:
			case CSInstanceLayerBlendMiddle:
			case CSInstanceLayerBlendTop:
				{
					float a = graphics->color().a;
					if ((source && source->_local->shader == CSMaterial::ShaderDistortion) || a <= 0) break;
					CSInstanceBlendLayer blendLayer = source ? source->_local->blendLayer : CSInstanceBlendLayerMiddle;
					if (blendLayer != layer) break;
					bool blending = (source && source->_local->blendMode != CSBlendNone) || a < 1;			//default blend should be none

					if (instances && *instances) {
						if (!blending) {
							*instances = (*instances)->where([](const CSVertexArrayInstance& i) -> bool { return i.color.a < 1; });
							if ((*instances)->count() == 0) break;
						}
						const CSVector3& cp = graphics->camera().position();
						*instances = (*instances)->orderByDescending<float>([cp](const CSVertexArrayInstance& i) -> float {
							return CSVector3::distanceSquared(cp, i.model.translationVector());
						});
					}
					else if (!blending) break;

					if (push) graphics->push();
					CSMaterial& material = graphics->material();
					if (!source || !source->getMaterial(progress, random, material)) material = CSMaterial::Default;
					if (material.blendMode == CSBlendNone) material.blendMode = CSBlendAlpha;
				}
				return true;
			case CSInstanceLayerDistortion:
				{
					if (!source || source->_local->shader != CSMaterial::ShaderDistortion) break;
					if (push) graphics->push();
					CSMaterial& material = graphics->material();
					if (!source || !source->getMaterial(progress, random, material)) material = CSMaterial::Default;
				}
				return true;
		}
	}
	return false;
}

void CSMaterialSource::preload() const {
	if (_local) {
		_local->colorMap.preload();
		_local->normalMap.preload();
		_local->materialMap.preload();
		_local->emissiveMap.preload();
	}
	else {
		const CSMaterialSource* origin = static_assert_cast<CSMaterialSource*>(CSResourcePool::sharedPool()->load(CSResourceTypeMaterial, _origin));

		if (origin) origin->preload();
	}
}

void CSMaterialSource::flush() const {
	if (_local) {
		const CSTexture* colorMap = _local->colorMap.content(0);
		const CSTexture* normalMap = _local->normalMap.content(1);
		const CSTexture* materialMap = _local->materialMap.content(2);
		const CSTexture* emissiveMap = _local->emissiveMap.content(3);
		if (colorMap) colorMap->flush();
		if (normalMap) normalMap->flush();
		if (materialMap) materialMap->flush();
		if (emissiveMap) emissiveMap->flush();
	}
	else {
		const CSMaterialSource* origin = static_assert_cast<CSMaterialSource*>(CSResourcePool::sharedPool()->load(CSResourceTypeMaterial, _origin));

		if (origin) origin->flush();
	}
}
