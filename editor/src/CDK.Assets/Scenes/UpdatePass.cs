using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDK.Assets.Scenes
{
    public struct UpdatePass
    {
        private int _pass;

        public const int Max = 8;

        public bool AddPrecedence(SceneObject obj)
        {
            if (_pass < Max)
            {
                var pass = obj.GetUpdatePass();
                if (obj.Located) pass++;
                if (pass > _pass)
                {
                    _pass = pass;
                    if (_pass < Max) return true;
                    else _pass = Max;
                }
            }
            return false;
        }

        public bool Remaining => _pass < Max;

        public static implicit operator int(UpdatePass pass) => pass._pass;
    }
}
