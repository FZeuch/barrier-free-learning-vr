using FreeHandGestureFramework.DataTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    ///<summary>In this class, some Unity specific values are used instead of
    ///Free Hand Gesture Framework specific values in its base class, like UnityEngine.Vector3
    ///instead of FreeHandGestureFramework.Position3D.</summary> 
    public class FreeHandAPIEventArgsU: FreeHandEventArgs
    {
        public new Vector3 LeftHandPosition
        {
            get{return ConversionTools.Position3DToVector3(base.LeftHandPosition);}
            set{base.LeftHandPosition = ConversionTools.Vector3ToPosition3D(value);}
        }
        public new Vector3 RightHandPosition
        {
            get{return ConversionTools.Position3DToVector3(base.RightHandPosition);}
            set{base.RightHandPosition = ConversionTools.Vector3ToPosition3D(value);}
        }
    }
}