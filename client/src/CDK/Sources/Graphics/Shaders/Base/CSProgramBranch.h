#ifndef __CDK__CSProgramBranch__
#define __CDK__CSProgramBranch__

#include "CSProgram.h"
#include "CSDictionary.h"
#include "CSStringBuilder.h"

class CSGraphicsApi;

class CSProgramBranch : public CSObject {
public:
	enum Mask {
		MaskVertex = 1,
		MaskFragment = 2,
		MaskGeometry = 4,
		MaskTessEvalution = 8,
		MaskTessControl = 16,
		MaskCompute = 32
	};
private:
	struct Branch {
	public:
		char* name;
		int value;
		int valueCount;
		int mask;

		Branch(const char* name, int value, int valueCount, int mask);
		~Branch();

		Branch(const Branch& other) = delete;
		Branch& operator=(const Branch& other) = delete;

		void append(CSStringBuilder& appendix) const;
	};
	CSDictionary<int64, CSProgram> _programs;
	CSArray<Branch> _branches;
	int _links = 0;
	CSDictionary<int64, CSShader>* _vertexShaders = NULL;
	CSDictionary<int64, CSShader>* _fragmentShaders = NULL;
	CSDictionary<int64, CSShader>* _geometryShaders = NULL;
	CSDictionary<int64, CSShader>* _tessEvaluationShaders = NULL;
	CSDictionary<int64, CSShader>* _tessControlShaders = NULL;
	CSDictionary<int64, CSShader>* _computeShaders = NULL;
	string _vertexShaderCode;
	string _fragmentShaderCode;
	string _geometryShaderCode;
	string _tessEvaluationShaderCode;
	string _tessControlShaderCode;
	string _computeShaderCode;
public:
	CSProgramBranch() = default;
	~CSProgramBranch();

	static inline CSProgramBranch* programBranch() {
		return autorelease(new CSProgramBranch());
	}
private:
	void attach(CSShaderType type, const char* const* fragments, int count);
public:
	void attach(CSShaderType type, const char* source);
	void attach(CSShaderType type, const char* source0, const char* source1);
	void attach(CSShaderType type, const char* source0, const char* source1, const char* source2);
	void attach(CSShaderType type, const char* source0, const char* source1, const char* source2, const char* source3);
	void addBranch(const char* name, int value, int valueCount, int mask);
	void addBranch(const char* name, bool value, int mask);
	void addLink(int mask);
private:
	CSShader* getShader(CSDictionary<int64, CSShader>* shaders, int64 key, const string& code, CSShaderType type, int mask);
public:
	CSProgram* endBranch();
};

#endif
