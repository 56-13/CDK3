using System;
using System.Text;
using System.Collections.Generic;

namespace CDK.Drawing
{
    [Flags]
    public enum ProgramBranchMask
    {
        Vertex = 1,
        Fragment = 2,
        Geometry = 4,
        TessEvalution = 8,
        TessControl = 16,
        Compute = 32
    }

    public class ProgramBranch : IDisposable
    {
        private struct Branch
        {
            public string Name;
            public int Value;
            public int ValueCount;
            public ProgramBranchMask Mask;

            public void Append(StringBuilder appendix)
            {
                if (Value != 0) appendix.Append($"#define {Name} {Value}\n");
            }
        }
        private List<Branch> _branches;
        private ProgramBranchMask _links;
        private Dictionary<long, Shader> _vertexShaders;
        private Dictionary<long, Shader> _fragmentShaders;
        private Dictionary<long, Shader> _geometryShaders;
        private Dictionary<long, Shader> _tessEvaluationShaders;
        private Dictionary<long, Shader> _tessControlShaders;
        private Dictionary<long, Shader> _computeShaders;
        private Dictionary<long, Program> _programs;
        private string _vertexShaderCode;
        private string _fragmentShaderCode;
        private string _geometryShaderCode;
        private string _tessEvaluationShaderCode;
        private string _tessControlShaderCode;
        private string _computeShaderCode;

        public ProgramBranch()
        {
            _programs = new Dictionary<long, Program>();

            _branches = new List<Branch>();
        }

        public void Dispose()
        {
            if (_vertexShaders != null) foreach (var e in _vertexShaders) e.Value.Dispose();
            if (_geometryShaders != null) foreach (var e in _geometryShaders) e.Value.Dispose();
            if (_tessEvaluationShaders != null) foreach (var e in _tessEvaluationShaders) e.Value.Dispose();
            if (_tessControlShaders != null) foreach (var e in _tessControlShaders) e.Value.Dispose();
            if (_computeShaders != null) foreach (var e in _computeShaders) e.Value.Dispose();
            foreach (var e in _programs) e.Value.Dispose();
        }

        private string MergeCode(string[] codeFragments)
        {
            var strbuf = new StringBuilder();
            foreach (var codeFragment in codeFragments)
            {
                strbuf.Append(codeFragment);
                strbuf.Append("\n");
            }
            return strbuf.ToString();
        }

        public void Attach(ShaderType type, params string[] codeFragments)
        {
            var code = codeFragments.Length > 1 ? MergeCode(codeFragments) : codeFragments[0];

            switch (type)
            {
                case ShaderType.VertexShader:
                    if (_vertexShaders == null) _vertexShaders = new Dictionary<long, Shader>();
                    _vertexShaderCode = code;
                    break;
                case ShaderType.FragmentShader:
                    if (_fragmentShaders == null) _fragmentShaders = new Dictionary<long, Shader>();
                    _fragmentShaderCode = code;
                    break;
                case ShaderType.GeometryShader:
                    if (_geometryShaders == null) _geometryShaders = new Dictionary<long, Shader>();
                    _geometryShaderCode = code;
                    break;
                case ShaderType.TessEvaluationShader:
                    if (_tessEvaluationShaders == null) _tessEvaluationShaders = new Dictionary<long, Shader>();
                    _tessEvaluationShaderCode = code;
                    break;
                case ShaderType.TessControlShader:
                    if (_tessControlShaders == null) _tessControlShaders = new Dictionary<long, Shader>();
                    _tessControlShaderCode = code;
                    break;
                case ShaderType.ComputeShader:
                    if (_computeShaders == null) _computeShaders = new Dictionary<long, Shader>();
                    _computeShaderCode = code;
                    break;
            }
        }

        public void AddBranch(string name, int value, int valueCount, ProgramBranchMask mask)
        {
            _branches.Add(new Branch()
            {
                Name = name,
                Value = value,
                ValueCount = valueCount,
                Mask = mask
            });
        }

        public void AddBranch(string name, bool value, ProgramBranchMask mask)
        {
            AddBranch(name, value ? 1 : 0, 2, mask);
        }

        public void AddLink(ProgramBranchMask mask)
        {
            _links |= mask;
        }

        private void PutToKey(ref long key, int value, int valueCount)
        {
            Debug.Assert(value >= 0 && value < valueCount);
            key *= valueCount;
            key += value;
        }

        private Shader GetShader(Dictionary<long, Shader> shaders, long key, string code, ShaderType type, ProgramBranchMask mask)
        {
            if (code == null || (_links & mask) == 0) return null;
            if (!shaders.TryGetValue(key, out var shader))
            {
                var appendix = new StringBuilder();
                foreach (var branch in _branches)
                {
                    if ((branch.Mask & mask) != 0) branch.Append(appendix);
                }
                shader = new Shader(type, appendix.ToString(), code);
                shaders.Add(key, shader);
            }
            return shader;
        }

        public Program EndBranch() => EndBranch(out _);

        public Program EndBranch(out bool isNew)
        {
            var pkey = 0L;

            foreach (var branch in _branches)
            {
                PutToKey(ref pkey, branch.Value, branch.ValueCount);
            }

            isNew = !_programs.TryGetValue(pkey, out var program);

            if (isNew)
            {
                var vkey = 0L;
                var fkey = 0L;
                var gkey = 0L;
                var tekey = 0L;
                var tckey = 0L;
                var ckey = 0L;

                foreach (var branch in _branches)
                {
                    if ((branch.Mask & ProgramBranchMask.Vertex) != 0) PutToKey(ref vkey, branch.Value, branch.ValueCount);
                    if ((branch.Mask & ProgramBranchMask.Fragment) != 0) PutToKey(ref fkey, branch.Value, branch.ValueCount);
                    if ((branch.Mask & ProgramBranchMask.Geometry) != 0) PutToKey(ref gkey, branch.Value, branch.ValueCount);
                    if ((branch.Mask & ProgramBranchMask.TessEvalution) != 0) PutToKey(ref tekey, branch.Value, branch.ValueCount);
                    if ((branch.Mask & ProgramBranchMask.TessControl) != 0) PutToKey(ref tckey, branch.Value, branch.ValueCount);
                    if ((branch.Mask & ProgramBranchMask.Compute) != 0) PutToKey(ref ckey, branch.Value, branch.ValueCount);
                }

                var vertexShader = GetShader(_vertexShaders, vkey, _vertexShaderCode, ShaderType.VertexShader, ProgramBranchMask.Vertex);
                var fragmentShader = GetShader(_fragmentShaders, fkey, _fragmentShaderCode, ShaderType.FragmentShader, ProgramBranchMask.Fragment);
                var geometryShader = GetShader(_geometryShaders, gkey, _geometryShaderCode, ShaderType.GeometryShader, ProgramBranchMask.Geometry);
                var tessEvaluationShader = GetShader(_tessEvaluationShaders, tekey, _tessEvaluationShaderCode, ShaderType.TessEvaluationShader, ProgramBranchMask.TessEvalution);
                var tessControlShader = GetShader(_tessControlShaders, tckey, _tessControlShaderCode, ShaderType.TessControlShader, ProgramBranchMask.TessControl);
                var computeShader = GetShader(_computeShaders, tckey, _computeShaderCode, ShaderType.ComputeShader, ProgramBranchMask.Compute);

                program = new Program();
                if (vertexShader != null) program.Attach(vertexShader);
                if (fragmentShader != null) program.Attach(fragmentShader);
                if (geometryShader != null) program.Attach(geometryShader);
                if (tessEvaluationShader != null) program.Attach(tessEvaluationShader);
                if (tessControlShader != null) program.Attach(tessControlShader);
                if (computeShader != null) program.Attach(computeShader);

                program.Link();

                _programs.Add(pkey, program);
            }
            _branches.Clear();
            _links = 0;

            return program;
        }
    }
}
