using FreeHandGestureFramework.DataTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    ///<summary>In this class, some Unity specific values are used instead of
    ///Free Hand Gesture Framework specific values in its base class, like UnityEngine.Vector3
    ///instead of FreeHandGestureFramework.Position3D.</summary> 
    public class RotationQuatU : RotationQuat
    {
        public RotationQuatU() : base() {}
        public RotationQuatU(UnityEngine.Quaternion q) : base(q.x, q.y, q.z, q.w) {}
        public RotationQuatU(Vector3 eulerAngles) : base(eulerAngles.x, eulerAngles.y, eulerAngles.z) {}
        ///<summary>Returns the Euler angle equivalent (the rotation around X, Y and Z axis) for this Quaternion,
        ///like calculated by the UnityEngine.Quaternion.eulerAngles method.</summary> 
        public new Vector3 GetEulerAnglesLikeInUnity() {return ConversionTools.Position3DToVector3(base.GetEulerAnglesLikeInUnity());}
        ///<summary>Returns the Euler angle equivalent (the rotation around X, Y and Z axis) for this Quaternion.</summary> 
        public new Vector3 GetEulerAngles() {return ConversionTools.Position3DToVector3(base.GetEulerAngles());}
    }
}