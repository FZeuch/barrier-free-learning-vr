using FreeHandGestureFramework;
using FreeHandGestureFramework.DataTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    public class HandPoseU : HandPose
    {
        public HandPoseU(Vector3[] bones): base(ConversionTools.Vector3ArrayToPosition3D(bones)) {}
        public Vector3[] GetVector3Bones() {return ConversionTools.Position3DArrayToVector3(Bones);}
    }

    public class HandPoseInWorldSpaceU : HandPoseInWorldSpace
    {
        public HandPoseInWorldSpaceU(Vector3[] bones, Transform worldCoords) 
            : base(ConversionTools.Vector3ArrayToPosition3D(bones),
                   ConversionTools.Vector3ToPosition3D(worldCoords.position),
                   ConversionTools.QuaternionToRotationQuat(worldCoords.rotation),
                   ConversionTools.Vector3ToPosition3D(worldCoords.lossyScale)){}
        public Vector3[] GetVector3Bones() {return ConversionTools.Position3DArrayToVector3(Bones);}
    }

    public class WeightedHandPoseU : WeightedHandPose
    {
        public WeightedHandPoseU() : base() {}
        public WeightedHandPoseU(Vector3[] bones): base(ConversionTools.Vector3ArrayToPosition3D(bones)) {}
        public WeightedHandPoseU(Vector3[] bones, float[] boneWeights, Deviation[] deviations): base(ConversionTools.Vector3ArrayToPosition3D(bones), boneWeights, deviations) {}
        public Vector3[] GetVector3Bones() {return ConversionTools.Position3DArrayToVector3(Bones);}

    }

    public class HandsU : Hands
    {
        public HandsU(): base(null, null){}
        public HandsU(HandPoseInWorldSpaceU left, HandPoseInWorldSpaceU right): base(left, right){}
    }

}

