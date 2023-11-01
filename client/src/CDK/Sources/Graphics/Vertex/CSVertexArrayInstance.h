#ifndef __CDK__CSVertexArrayInstance__
#define __CDK__CSVertexArrayInstance__

#include "CSMatrix.h"
#include "CSColor.h"

struct CSVertexArrayInstance {
    CSMatrix model;
    CSColor color;

    static const CSVertexArrayInstance Origin;

    inline CSVertexArrayInstance(const CSMatrix& model, const CSColor& color) : model(model), color(color) {}
};

#endif