using FreeHandGestureFramework.DataTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
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