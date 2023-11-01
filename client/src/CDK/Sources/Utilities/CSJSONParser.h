#ifndef __CDK__CSJSONParser__
#define __CDK__CSJSONParser__

#include "CSJSONObject.h"
#include "CSJSONArray.h"

class CSJSONParser {
public:
    static CSJSONObject* parse(const char* json);
};

#endif
