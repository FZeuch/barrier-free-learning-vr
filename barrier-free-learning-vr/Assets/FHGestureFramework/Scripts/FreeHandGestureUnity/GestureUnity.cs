using FreeHandGestureFramework;
using FreeHandGestureFramework.DataTypes;

namespace FreeHandGestureUnity
{
    public class GestureU: Gesture
    {
        public GestureU() : base() {}
        public GestureU(string name) : base(name) {}
        public GestureU(GestureU orig) : base(orig) {}
        public new Path3D Path
        {
            get
            {
                if (_path.GetType().Equals(typeof(Path3DU)))
                {
                    return _path; //Path3DU.Path will give back a copy of the path. This prevents that a copy of a copy of the array will be created.
                }
                else
                {
                    return base.Path;
                }
            }
        }
    }
}