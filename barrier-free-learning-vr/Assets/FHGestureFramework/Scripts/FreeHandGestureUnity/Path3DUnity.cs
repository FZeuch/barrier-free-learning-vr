using FreeHandGestureFramework.DataTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    public class Path3DU: Path3D
    {
        public new Vector3[] Path
        {
            get{return ConversionTools.Position3DArrayToVector3(base.Path);}
            set{base.Path = ConversionTools.Vector3ArrayToPosition3D(value);}
        }
    }

}