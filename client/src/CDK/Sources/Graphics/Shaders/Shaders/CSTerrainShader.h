#ifndef __CDK__CSTerrainShader__
#define __CDK__CSTerrainShader__

#include "CSProgramBranch.h"
#include "CSRenderState.h"
#include "CSVertexArray.h"

class CSTerrain;
struct CSTerrainSurface;
struct CSTerrainWater;

class CSTerrainShader {
private:
    CSProgramBranch _programs;
    CSProgramBranch _shadowPrograms;
    CSProgramBranch _waterPrograms;
public:
    CSTerrainShader();

    CSTerrainShader(const CSTerrainShader&) = delete;
    CSTerrainShader& operator =(const CSTerrainShader&) = delete;

    void drawShadow(CSGraphicsApi* api, const CSRenderState& state, const CSMatrix& world, const CSTerrain* terrain, const CSVertexArray* vertices);
    void drawSurface(CSGraphicsApi* api, const CSRenderState& state, const CSMatrix& world, const CSColor& color, const CSTerrain* terrain, const CSTerrainSurface* surface, const CSVertexArray* vertices);
    void drawWater(CSGraphicsApi* api, const CSRenderState& state, const CSMatrix& world, const CSColor& color, const CSTerrain* terrain, const CSTerrainWater& water, float progress, const CSTexture* destMap, const CSTexture* depthMap);
};

#endif
