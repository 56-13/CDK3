#ifndef __CDK__CDK__
#define __CDK__CDK__

#include "CSConfig.h"
// Core
#include "CSTypes.h"
#include "CSFixed.h"
#include "CSHalf.h"
#include "CSStringLiteral.h"
#include "CSLocaleString.h"
#include "CSValue.h"
#include "CSTime.h"
#include "CSHandler.h"
#include "CSThread.h"
#include "CSResourcePool.h"

// Collection
#include "CSArray.h"
#include "CSSet.h"
#include "CSDictionary.h"
#include "CSSortedSet.h"
#include "CSSortedDictionary.h"
#include "CSQueue.h"

// Graphics
#include "CSGEnums.h"
#include "CSRect.h"
#include "CSZRect.h"
#include "CSColor.h"
#include "CSQuaternion.h"
#include "CSMatrix.h"
#include "CSInt4.h"
#include "CSFixed4.h"
#include "CSHalf4.h"
#include "CSBounds3.h"
#include "CSCamera.h"
#include "CSPlane.h"
#include "CSRay.h"
#include "CSSegment.h"
#include "CSTriangle.h"
#include "CSABoundingBox.h"
#include "CSOBoundingBox.h"
#include "CSBoundingSphere.h"
#include "CSBoundingCapsule.h"
#include "CSBoundingMesh.h"
#include "CSBoundingFrustum.h"

#include "CSShaderCode.h"
#include "CSShaders.h"
#include "CSRenderers.h"

#include "CSGBuffers.h"
#include "CSVertexArrays.h"
#include "CSTextures.h"
#include "CSRenderBuffers.h"
#include "CSRenderTargets.h"

#include "CSMesh.h"
#include "CSMeshBuilder.h"
#include "CSLightSpace.h"
#include "CSTextureScratch.h"

#include "CSGraphicsContext.h"
#include "CSGraphics.h"

// Audio
#include "CSAudio.h"

// Animation
#include "CSAnimationFloat.h"
#include "CSAnimationFloatConstant.h"
#include "CSAnimationFloatLinear.h"
#include "CSAnimationFloatCurve.h"
#include "CSAnimationColor.h"
#include "CSAnimationColorConstant.h"
#include "CSAnimationColorLinear.h"
#include "CSAnimationColorChannel.h"

#include "CSAnimation.h"
#include "CSAnimationDerivationMulti.h"
#include "CSAnimationDerivationLinked.h"
#include "CSAnimationDerivationEmission.h"
#include "CSAnimationDerivationRandom.h"

#include "CSParticle.h"
#include "CSParticleShape.h"
#include "CSParticleShapeSphere.h"
#include "CSParticleShapeHamisphere.h"
#include "CSParticleShapeCone.h"
#include "CSParticleShapeBox.h"
#include "CSParticleShapeRect.h"
#include "CSParticleShapeCircle.h"

#include "CSSprite.h"
#include "CSSpriteElementImage.h"
#include "CSSpriteElementMesh.h"
#include "CSSpriteElementString.h"
#include "CSSpriteElementLine.h"
#include "CSSpriteElementGradientLine.h"
#include "CSSpriteElementRect.h"
#include "CSSpriteElementGradientRect.h"
#include "CSSpriteElementRoundRect.h"
#include "CSSpriteElementGradientRoundRect.h"
#include "CSSpriteElementArc.h"
#include "CSSpriteElementGradientArc.h"
#include "CSSpriteElementSphere.h"
#include "CSSpriteElementBox.h"
#include "CSSpriteElementExtern.h"
#include "CSSpriteElementTranslate.h"
#include "CSSpriteElementRotate.h"
#include "CSSpriteElementScale.h"
#include "CSSpriteElementInvert.h"
#include "CSSpriteElementColor.h"
#include "CSSpriteElementStroke.h"
#include "CSSpriteElementBrightness.h"
#include "CSSpriteElementContrast.h"
#include "CSSpriteElementSaturation.h"
#include "CSSpriteElementBlur.h"
#include "CSSpriteElementLens.h"
#include "CSSpriteElementWave.h"

#include "CSTrail.h"

// Scenes
#include "CSScene.h"
#include "CSCameraObject.h"
#include "CSDirectionalLightObject.h"
#include "CSPointLightObject.h"
#include "CSSpotLightObject.h"
#include "CSGround.h"
#include "CSTerrain.h"

// IO
#include "CSBytes.h"
#include "CSBuffer.h"
#include "CSLogger.h"

// Platform
#include "CSDevice.h"
#include "CSFile.h"
#include "CSLog.h"
#include "CSSignal.h"

// Network
#include "CSSocket.h"
#include "CSURLConnection.h"

// Utilities
#include "CSMath.h"
#include "CSRandom.h"
#include "CSJSONParser.h"
#include "CSJSONWriter.h"
#include "CSIAPDelegate.h"
#include "CSLocalNotification.h"

// Diagnostics
#include "CSDiagnostics.h"

// Security
#include "CSSecret.h"
#include "CSSecretValue.h"

// Interface
#include "CSLayer.h"
#include "CSButton.h"
#include "CSScrollPane.h"
#include "CSListBox.h"
#include "CSTextBox.h"
#include "CSTextField.h"
#include "CSWebView.h"
#include "CSVideoView.h"
#include "CSTicker.h"

// Application
#include "CSApplication.h"

#endif
