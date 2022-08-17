using FreeHandGestureFramework.EnumsAndTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    public class RotationQuatU : RotationQuat
    {
        public RotationQuatU() : base() {}
        public RotationQuatU(UnityEngine.Quaternion q) : base(q.x, q.y, q.z, q.w) {}
        public RotationQuatU(Vector3 eulerAngles) : base(eulerAngles.x, eulerAngles.y, eulerAngles.z) {}
        public new Vector3 GetEulerAnglesLikeInUnity() {return ConversionTools.Position3DToVector3(base.GetEulerAnglesLikeInUnity());}
        public new Vector3 GetEulerAngles() {return ConversionTools.Position3DToVector3(base.GetEulerAngles());}
    }
}