#define CDK_IMPL

#include "CSStrokeShader.h"

#include "CSFile.h"

#include "CSGBuffers.h"
#include "CSVertexArrays.h"
#include "CSShaderCode.h"

CSStrokeShader::CSStrokeShader() {
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/stroke_vs.glsl"));
    string geometryShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/stroke_gs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/stroke_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, CSShaderCode::VSBase, vertexShaderCode);
    _programs.attach(CSShaderTypeGeometry, geometryShaderCode);
    _programs.attach(CSShaderTypeFragment, CSShaderCode::Base, fragmentShaderCode);
}

static const float StrokeAlpha[] = {
    0.0f,
    0.0009215682f,
    0.001943143f,
    0.002864719f,
    0.003886295f,
    0.004907869f,
    0.005929442f,
    0.006851016f,
    0.007872589f,
    0.00889409f,
    0.00991559f,
    0.01093709f,
    0.01195859f,
    0.01298009f,
    0.01400159f,
    0.01502309f,
    0.0160446f,
    0.0170661f,
    0.0180876f,
    0.0191091f,
    0.0201306f,
    0.0212521f,
    0.0222736f,
    0.0232951f,
    0.0243166f,
    0.0254381f,
    0.0264596f,
    0.0275811f,
    0.0286026f,
    0.0297241f,
    0.0307456f,
    0.03186711f,
    0.03288864f,
    0.03401016f,
    0.03513168f,
    0.0361532f,
    0.03727472f,
    0.03839624f,
    0.03951776f,
    0.04063928f,
    0.0416608f,
    0.04278232f,
    0.04390384f,
    0.04502536f,
    0.04624689f,
    0.04736841f,
    0.04848993f,
    0.04961145f,
    0.05073297f,
    0.05185449f,
    0.05307601f,
    0.05419753f,
    0.05541906f,
    0.05654058f,
    0.0576621f,
    0.05888362f,
    0.06010514f,
    0.06122667f,
    0.06244819f,
    0.06366971f,
    0.06479123f,
    0.06601276f,
    0.06723428f,
    0.0684558f,
    0.06967747f,
    0.07089959f,
    0.07212169f,
    0.0733438f,
    0.0745659f,
    0.07578801f,
    0.07701012f,
    0.07833223f,
    0.07955433f,
    0.08077644f,
    0.08209856f,
    0.08332066f,
    0.08464277f,
    0.08586487f,
    0.08718698f,
    0.0885091f,
    0.0897312f,
    0.09105331f,
    0.09237541f,
    0.09369753f,
    0.09501964f,
    0.09634174f,
    0.09766385f,
    0.09898596f,
    0.1004081f,
    0.1017302f,
    0.1030523f,
    0.1044744f,
    0.1057965f,
    0.1072186f,
    0.1085407f,
    0.1099628f,
    0.1112849f,
    0.1127071f,
    0.1141292f,
    0.1155513f,
    0.1169734f,
    0.1183955f,
    0.1198176f,
    0.1212397f,
    0.1227618f,
    0.1241839f,
    0.1256061f,
    0.1271282f,
    0.1285503f,
    0.1300724f,
    0.1315945f,
    0.1331166f,
    0.1345387f,
    0.1360608f,
    0.1375829f,
    0.1392051f,
    0.1407272f,
    0.1422493f,
    0.1437714f,
    0.1453935f,
    0.1469156f,
    0.1485377f,
    0.1501599f,
    0.151682f,
    0.1533041f,
    0.1549262f,
    0.1565483f,
    0.1582704f,
    0.159892f,
    0.161513f,
    0.1632339f,
    0.1648548f,
    0.1665758f,
    0.1682967f,
    0.1700177f,
    0.1717386f,
    0.1734596f,
    0.1751805f,
    0.1769015f,
    0.1787224f,
    0.1804433f,
    0.1822643f,
    0.1840852f,
    0.1859062f,
    0.1877272f,
    0.1895481f,
    0.191369f,
    0.19319f,
    0.195111f,
    0.1970319f,
    0.1988528f,
    0.2007738f,
    0.2026947f,
    0.2047157f,
    0.2066366f,
    0.2085576f,
    0.2105785f,
    0.2125995f,
    0.2146205f,
    0.2166414f,
    0.2186624f,
    0.2207833f,
    0.2228043f,
    0.2249252f,
    0.2270462f,
    0.2291671f,
    0.2312881f,
    0.233509f,
    0.23573f,
    0.2378509f,
    0.2400719f,
    0.2423929f,
    0.2446138f,
    0.2469348f,
    0.2492557f,
    0.2515765f,
    0.2538971f,
    0.2562177f,
    0.2586383f,
    0.2610589f,
    0.2634795f,
    0.2660001f,
    0.2685207f,
    0.2710412f,
    0.2735618f,
    0.2760824f,
    0.278703f,
    0.2813236f,
    0.2840441f,
    0.2866647f,
    0.2893852f,
    0.2921058f,
    0.2949263f,
    0.2977469f,
    0.3005674f,
    0.303488f,
    0.3064085f,
    0.309329f,
    0.3123495f,
    0.3153701f,
    0.3184906f,
    0.3216111f,
    0.3247316f,
    0.3279521f,
    0.3311726f,
    0.334493f,
    0.3379135f,
    0.341234f,
    0.3447544f,
    0.3482749f,
    0.3517953f,
    0.3554158f,
    0.3591362f,
    0.3628566f,
    0.366677f,
    0.3705975f,
    0.3746178f,
    0.3786382f,
    0.3827586f,
    0.386979f,
    0.3912993f,
    0.3957196f,
    0.40014f,
    0.4047603f,
    0.4094806f,
    0.4143009f,
    0.4192211f,
    0.4243414f,
    0.4295616f,
    0.4348818f,
    0.440402f,
    0.4460222f,
    0.4519423f,
    0.4579624f,
    0.4642825f,
    0.4707026f,
    0.4775226f,
    0.4845426f,
    0.4918625f,
    0.4994825f,
    0.5075045f,
    0.5159268f,
    0.5247492f,
    0.5341717f,
    0.5441943f,
    0.554917f,
    0.5665398f,
    0.5790628f,
    0.592886f,
    0.6083095f,
    0.6257333f,
    0.6460576f,
    0.6705826f,
    0.7023088f,
    0.7497376f,
    1.0f
};

struct StrokeUniformData{
    CSColor color;
    CSVector2 scale;

    inline StrokeUniformData() {
        memset(this, 0, sizeof(StrokeUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(color);
        hash.combine(scale);
        return hash;
    }
    inline bool operator ==(const StrokeUniformData& other) const {
        return color == other.color && scale == other.scale;
    }
    inline bool operator !=(const StrokeUniformData& other) const {
        return !(*this == other);
    }
};

void CSStrokeShader::setup(CSGraphicsApi* api, const CSRenderState& state, CSPrimitiveMode mode, const CSGBuffer* boneBuffer, bool usingInstance) {
   StrokeUniformData data;

    switch (state.strokeMode) {
        case CSStrokeLighten:
            api->applyBlendMode(CSBlendAdd);
            data.color.r = state.strokeColor.r * state.strokeColor.a / 3;
            data.color.g = state.strokeColor.g * state.strokeColor.a / 3;
            data.color.b = state.strokeColor.b * state.strokeColor.a / 3;
            data.color.a = 1.0f;
            break;
        default:
            api->applyBlendMode(CSBlendAlpha);
            data.color = state.strokeColor;

            if (data.color.a > 0 && data.color.a < 1) {
                float fa = data.color.a * 255;
                int ia = fa;
                data.color.a = CSMath::lerp(StrokeAlpha[ia], StrokeAlpha[ia + 1], fa - ia);
            }
            break;
    }

    data.scale = CSVector2(state.strokeWidth * 2.0f / state.target->viewport().width, state.strokeWidth * 2.0f / state.target->viewport().height);

    _programs.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskGeometry | CSProgramBranch::MaskFragment);

    switch (mode) {
        case CSPrimitivePoints:
            _programs.addBranch("UsingPrimitive", 1, 4, CSProgramBranch::MaskGeometry);
            break;
        case CSPrimitiveLines:
        case CSPrimitiveLineLoop:
        case CSPrimitiveLineStrip:
            _programs.addBranch("UsingPrimitive", 2, 4, CSProgramBranch::MaskGeometry);
            break;
        case CSPrimitiveTriangles:
        case CSPrimitiveTriangleStrip:
        case CSPrimitiveTriangleFan:
            _programs.addBranch("UsingPrimitive", 3, 4, CSProgramBranch::MaskGeometry);
            break;
    }

    _programs.addBranch("UsingMap", state.material.colorMap, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingColorMap", state.material.colorMap, CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingDisplacementMap", state.material.displacementScale != 0, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingBone", boneBuffer != NULL, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingInstance", usingInstance, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingUVOffset", state.material.uvOffset != CSVector2::Zero, CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingPerspective", state.camera.fov() != 0, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingDepthBias", state.material.depthBias != 0, CSProgramBranch::MaskVertex);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    state.applyUniforms(api, boneBuffer);

    CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, CSRenderState::UniformBlockBindingStroke, dataBuffer->object());
}
