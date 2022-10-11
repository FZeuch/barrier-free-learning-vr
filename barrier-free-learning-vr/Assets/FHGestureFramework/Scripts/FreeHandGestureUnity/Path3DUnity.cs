using FreeHandGestureFramework.DataTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    ///<summary>This class uses UnityEngine.Vector3 instances in its Path attribute
    ///instead of FreeHandGestureFramework.Path in its base class Path3D.</summary>
    public class Path3DU: Path3D
    {
        public new Vector3[] Path
        {
            get{return ConversionTools.Position3DArrayToVector3(base.Path);}
            set{base.Path = ConversionTools.Vector3ArrayToPosition3D(value);}
        }
    }

}