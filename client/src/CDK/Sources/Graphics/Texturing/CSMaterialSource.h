#ifndef __CDK__CSMaterialSource__
#define __CDK__CSMaterialSource__

#include "CSArray.h"
#include "CSAnimationColor.h"
#include "CSAnimationLoop.h"
#include "CSGraphics.h"

class CSMaterialSource : public CSResource {
public:
    struct Map {
    private:
        enum Type : byte {
            None,
            Embed,
            MaterialRef,
            TextureRef
        } _type = None;
        const CSTexture* _texture = NULL;
        const CSArray<ushort>* _indices = NULL;
    public:
        Map() = default;
        Map(const CSTexture* texture);
        Map(CSBuffer* buffer);
        Map(const Map& other);
        ~Map();

        Map& operator=(const Map&);

        int resourceCost() const;
        void preload() const;
        const CSTexture* content(int i) const;
    };
	struct Local {
		CSInstanceBlendLayer blendLayer = CSInstanceBlendLayerMiddle;
        CSMaterial::Shader shader = CSMaterial::ShaderLight;
        CSBlendMode blendMode = CSBlendNone;
        CSCullMode cullMode = CSCullBack;
        byte materialMapComponents = 0;
        bool depthTest = true;
        bool alphaTest = false;
        float depthBias = 0;
        float alphaTestBias = 0;
        float displacementScale = 0;
        float distortionScale = 0;
        Map colorMap;
        Map normalMap;
        Map materialMap;
        Map emissiveMap;
        CSPtr<CSAnimationColor> color;
        float colorDuration = 0;
        CSAnimationLoop colorLoop;
        bool bloom = true;
        bool reflection = true;
        bool receiveShadow = true;
        bool receiveShadow2D = false;
        float metallic = 0.5f;
        float roughness = 0.5f;
        float ambientOcclusion = 1;
        float rim = 0;
        CSPtr<CSAnimationColor> emission;
        float emissionDuration = 0;
        CSAnimationLoop emissionLoop;
		CSVector2 uvScroll = CSVector2::Zero;

        Local() = default;
        Local(CSBuffer* buffer);
	};
private:
    const CSArray<ushort>* _origin = NULL;
    Local* _local = NULL;
public:
    CSMaterialSource();
    CSMaterialSource(CSBuffer* buffer);
    CSMaterialSource(const CSMaterialSource* other);
private:
    ~CSMaterialSource();
public:
    static inline CSMaterialSource* material() {
        return autorelease(new CSMaterialSource());
    }
    static inline CSMaterialSource* materialWithBuffer(CSBuffer* buffer) {
        return autorelease(new CSMaterialSource(buffer));
    }
    static inline CSMaterialSource* materialWithMaterial(const CSMaterialSource* other) {
        return autorelease(new CSMaterialSource(other));
    }

    Local* local() {
        return _local;
    }
    const Local* local() const {
        return _local;
    }

    inline CSResourceType resourceType() const override {
        return CSResourceTypeMaterial;
    }
    int resourceCost() const override;

    uint showFlags() const;
    CSInstanceBlendLayer blendLayer() const;
    bool getMaterial(float progress, int random, CSMaterial& result) const;
    inline bool apply(CSGraphics* graphics, CSInstanceLayer layer, float progress, int random, const CSArray<CSVertexArrayInstance>** instances, bool push) const {
        return apply(this, graphics, layer, progress, random, instances, push);
    }
    static bool apply(const CSMaterialSource* source, CSGraphics* graphics, CSInstanceLayer layer, float progress, int random, const CSArray<CSVertexArrayInstance>** instances, bool push);
    void preload() const;
    void flush() const;
private:
    const Local* retrieve() const;
};

#endif
