#define CDK_IMPL

#include "CSAnimationFloatLinear.h"

#include "CSBuffer.h"

CSAnimationFloatLinear::CSAnimationFloatLinear(float startValueOrigin, float startValueVar, float endValueOrigin, float endValueVar, bool smooth) :
    startValueOrigin(startValueOrigin),
    startValueVar(startValueVar), 
    endValueOrigin(endValueOrigin),
    endValueVar(endValueVar),
    smooth(smooth)
{
}

CSAnimationFloatLinear::CSAnimationFloatLinear(const CSAnimationFloatLinear* other) : 
    startValueOrigin(other->startValueOrigin),
    startValueVar(other->startValueVar),
    endValueOrigin(other->endValueOrigin),
    endValueVar(other->endValueVar),
    smooth(other->smooth)
{
}

CSAnimationFloatLinear::CSAnimationFloatLinear(CSBuffer* buffer) : 
    startValueOrigin(buffer->readFloat()),
    startValueVar(buffer->readFloat()), 
    endValueOrigin(buffer->readFloat()),
    endValueVar(buffer->readFloat()),
    smooth(buffer->readBoolean())
{
}

float CSAnimationFloatLinear::value(float t) const {
    if (smooth) t = CSMath::smoothStep(t);
    return CSMath::lerp(startValueOrigin, endValueOrigin, t);
}

float CSAnimationFloatLinear::value(float t, float r) const {
    if (smooth) t = CSMath::smoothStep(t);
    float s = startValueVar ? CSMath::lerp(startValueOrigin - startValueVar, startValueOrigin + startValueVar, r) : startValueOrigin;
    float e = endValueVar ? CSMath::lerp(endValueOrigin - endValueVar, endValueOrigin + endValueVar, r) : endValueOrigin;
    return CSMath::lerp(s, e, t);
}