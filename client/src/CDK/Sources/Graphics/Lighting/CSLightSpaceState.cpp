#define CDK_IMPL

#include "CSLightSpaceState.h"

#include "CSGraphics.h"

void CSLightSpaceState::flush(CSGraphics* graphics) const {
    CSDelegateRenderCommand* command = graphics->command([](CSGraphicsApi*) {}, this);
    if (lightBuffer) command->addFence(lightBuffer, CSGBatchFlagRead);
    if (directionalLightBuffer) command->addFence(directionalLightBuffer, CSGBatchFlagRead);
    for (int i = 0; i < CSLightSpace::MaxDirectionalLightCount; i++) {
        if (directionalShadowMaps[i][0]) command->addFence(directionalShadowMaps[i][0], CSGBatchFlagRead);
        if (directionalShadowMaps[i][1]) command->addFence(directionalShadowMaps[i][1], CSGBatchFlagRead);
    }
    if (pointLightBuffer) command->addFence(pointLightBuffer, CSGBatchFlagRead);
    if (pointLightClusterMap) command->addFence(pointLightClusterMap, CSGBatchFlagRead);
    for (int i = 0; i < CSLightSpace::MaxPointShadowCount; i++) {
        if (pointShadowMaps[i]) command->addFence(pointShadowMaps[i], CSGBatchFlagRead);
    }
    if (spotLightBuffer) command->addFence(spotLightBuffer, CSGBatchFlagRead);
    if (spotLightClusterMap) command->addFence(spotLightClusterMap, CSGBatchFlagRead);
    if (spotShadowBuffer) command->addFence(spotShadowBuffer, CSGBatchFlagRead);
    for (int i = 0; i < CSLightSpace::MaxSpotShadowCount; i++) {
        if (spotShadowMaps[i]) command->addFence(spotShadowMaps[i], CSGBatchFlagRead);
    }
    if (envMap) command->addFence(envMap, CSGBatchFlagRead);
    if (brdfMap) command->addFence(brdfMap, CSGBatchFlagRead);
}
