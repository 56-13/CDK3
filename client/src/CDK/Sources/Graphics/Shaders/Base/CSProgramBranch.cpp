#define CDK_IMPL

#include "CSProgramBranch.h"

#include "CSGraphicsContext.h"

CSProgramBranch::Branch::Branch(const char* name, int value, int valueCount, int mask) : 
    name(strdup(name)), 
    value(value), 
    valueCount(valueCount), 
    mask(mask) 
{
}

CSProgramBranch::Branch::~Branch() {
    free(name);
}

void CSProgramBranch::Branch::append(CSStringBuilder& appendix) const {
    if (value) appendix.appendFormat("#define %s %d\n", name, value);
}

//======================================================================================

CSProgramBranch::~CSProgramBranch() {
	release(_vertexShaders);
    release(_geometryShaders);
    release(_tessEvaluationShaders);
    release(_tessControlShaders);
    release(_computeShaders);
}

void CSProgramBranch::attach(CSShaderType type, const char* const* fragments, int count) {
    CSStringBuilder code;
    for (int i = 0; i < count; i++) code.append(fragments[i]);

    switch (type) {
        case CSShaderTypeVertex:
            if (!_vertexShaders) _vertexShaders = new CSDictionary<int64, CSShader>();
            _vertexShaderCode = code.toString();
            break;
        case CSShaderTypeFragment:
            if (!_fragmentShaders) _fragmentShaders = new CSDictionary<int64, CSShader>();
            _fragmentShaderCode = code.toString();
            break;
        case CSShaderTypeGeometry:
            if (!_geometryShaders) _geometryShaders = new CSDictionary<int64, CSShader>();
            _geometryShaderCode = code.toString();
            break;
        case CSShaderTypeTessEvaluation:
            if (!_tessEvaluationShaders) _tessEvaluationShaders = new CSDictionary<int64, CSShader>();
            _tessEvaluationShaderCode = code.toString();
            break;
        case CSShaderTypeTessControl:
            if (!_tessControlShaders) _tessControlShaders = new CSDictionary<int64, CSShader>();
            _tessControlShaderCode = code.toString();
            break;
        case CSShaderTypeCompute:
            if (!_computeShaders) _computeShaders = new CSDictionary<int64, CSShader>();
            _computeShaderCode = code.toString();
            break;
    }
}

void CSProgramBranch::attach(CSShaderType type, const char* source) {
    attach(type, &source, 1);
}

void CSProgramBranch::attach(CSShaderType type, const char* source0, const char* source1) {
    const char* sources[] = { source0, source1 };
    attach(type, sources, 2);
}

void CSProgramBranch::attach(CSShaderType type, const char* source0, const char* source1, const char* source2) {
    const char* sources[] = { source0, source1, source2 };
    attach(type, sources, 3);
}

void CSProgramBranch::attach(CSShaderType type, const char* source0, const char* source1, const char* source2, const char* source3) {
    const char* sources[] = { source0, source1, source2, source3 };
    attach(type, sources, 4);
}

void CSProgramBranch::addBranch(const char* name, int value, int valueCount, int mask) {
    new (&_branches.addObject()) Branch(name, value, valueCount, mask);
}

void CSProgramBranch::addBranch(const char* name, bool value, int mask) {
    addBranch(name, value ? 1 : 0, 2, mask);
}

void CSProgramBranch::addLink(int mask) {
    _links |= mask;
}

static void putToKey(int64& key, int value, int valueCount) {
    CSAssert(value >= 0 && value < valueCount);
    key *= valueCount;
    key += value;
}

CSShader* CSProgramBranch::getShader(CSDictionary<int64, CSShader>* shaders, int64 key, const string& code, CSShaderType type, int mask) {
    if (!code || (_links & mask) == 0) return NULL;

    CSShader* shader = shaders->objectForKey(key);

    if (!shader) {
        CSStringBuilder appendix;
        foreach (const Branch&, branch, &_branches) {
            if (branch.mask & mask) branch.append(appendix);
        }
        shader = new CSShader(type, appendix.toString()->cstring(), code.cstring());            //TODO
        shaders->setObject(key, shader);
        shader->release();
    }
    return shader;
}

CSProgram* CSProgramBranch::endBranch() {
    int64 pkey = 0;
    foreach (const Branch&, branch, &_branches) putToKey(pkey, branch.value, branch.valueCount);

    CSProgram* program = _programs.objectForKey(pkey);

    if (!program) {
        int64 vkey = 0;
        int64 fkey = 0;
        int64 gkey = 0;
        int64 tekey = 0;
        int64 tckey = 0;
        int64 ckey = 0;

        foreach (const Branch&, branch, &_branches) {
            if (branch.mask & MaskVertex) putToKey(vkey, branch.value, branch.valueCount);
            if (branch.mask & MaskFragment) putToKey(fkey, branch.value, branch.valueCount);
            if (branch.mask & MaskGeometry) putToKey(gkey, branch.value, branch.valueCount);
            if (branch.mask & MaskTessEvalution) putToKey(tekey, branch.value, branch.valueCount);
            if (branch.mask & MaskTessControl) putToKey(tckey, branch.value, branch.valueCount);
            if (branch.mask & MaskCompute) putToKey(ckey, branch.value, branch.valueCount);
        }

        CSShader* vertexShader = getShader(_vertexShaders, vkey, _vertexShaderCode, CSShaderTypeVertex, MaskVertex);
        CSShader* fragmentShader = getShader(_fragmentShaders, fkey, _fragmentShaderCode, CSShaderTypeFragment, MaskFragment);
        CSShader* geometryShader = getShader(_geometryShaders, gkey, _geometryShaderCode, CSShaderTypeGeometry, MaskGeometry);
        CSShader* tessEvaluationShader = getShader(_tessEvaluationShaders, tekey, _tessEvaluationShaderCode, CSShaderTypeTessEvaluation, MaskTessEvalution);
        CSShader* tessControlShader = getShader(_tessControlShaders, tckey, _tessControlShaderCode, CSShaderTypeTessControl, MaskTessControl);
        CSShader* computeShader = getShader(_computeShaders, tckey, _computeShaderCode, CSShaderTypeCompute, MaskCompute);

        program = new CSProgram();
        if (vertexShader) program->attach(vertexShader);
        if (fragmentShader) program->attach(fragmentShader);
        if (geometryShader) program->attach(geometryShader);
        if (tessEvaluationShader) program->attach(tessEvaluationShader);
        if (tessControlShader) program->attach(tessControlShader);
        if (computeShader) program->attach(computeShader);

        program->link();

        _programs.setObject(pkey, program);

        program->release();
    }
    _branches.removeAllObjects();
    _links = 0;

    return program;
}
