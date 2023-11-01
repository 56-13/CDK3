using System.Collections.Generic;

namespace CDK.Assets.Scenes
{
    public class SceneBuildParam
    {
        private Dictionary<string, int> _ids;
        private int _idSeed;

        public SceneBuildParam(int idSeed)
        {
            _ids = new Dictionary<string, int>();
            _idSeed = idSeed;   
        }

        public int KeyToId(string key)
        {
            if (key == null) return 0;
            if (_ids.TryGetValue(key, out var id)) return id;
            _idSeed++;
            _ids.Add(key, _idSeed);
            return _idSeed;
        }
    }
}
