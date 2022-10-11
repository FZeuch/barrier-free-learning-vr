using FreeHandGestureFramework;
using FreeHandGestureFramework.DataTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    ///<summary>In this class, some Unity specific values are used instead of
    ///Free Hand Gesture Framework specific values in its base class, like UnityEngine.Vector3
    ///instead of FreeHandGestureFramework.Position3D.</summary> 
    public class HandPoseU : HandPose
    {
        public HandPoseU(Vector3[] bones): base(ConversionTools.Vector3ArrayToPosition3D(bones)) {}
        public Vector3[] GetVector3Bones() {return ConversionTools.Position3DArrayToVector3(Bones);}
    }

    ///<summary>In this class, some Unity specific values are used instead of
    ///Free Hand Gesture Framework specific values in its base class, like UnityEngine.Vector3
    ///instead of FreeHandGestureFramework.Position3D.</summary> 
    public class HandPoseInWorldSpaceU : HandPoseInWorldSpace
    {
        public HandPoseInWorldSpaceU(Vector3[] bones, Transform worldCoords) 
            : base(ConversionTools.Vector3ArrayToPosition3D(bones),
                   ConversionTools.Vector3ToPosition3D(worldCoords.position),
                   ConversionTools.QuaternionToRotationQuat(worldCoords.rotation),
                   ConversionTools.Vector3ToPosition3D(worldCoords.lossyScale)){}
        ///<summary>Get bone positions as Vector3 values.</summary>
        public Vector3[] GetVector3Bones() {return ConversionTools.Position3DArrayToVector3(Bones);}
    }

    ///<summary>In this class, some Unity specific values are used instead of
    ///Free Hand Gesture Framework specific values in its base class, like UnityEngine.Vector3
    ///instead of FreeHandGestureFramework.Position3D.</summary> 
    public class WeightedHandPoseU : WeightedHandPose
    {
        public WeightedHandPoseU() : base() {}
        public WeightedHandPoseU(Vector3[] bones): base(ConversionTools.Vector3ArrayToPosition3D(bones)) {}
        public WeightedHandPoseU(Vector3[] bones, float[] boneWeights, Deviation[] deviations): base(ConversionTools.Vector3ArrayToPosition3D(bones), boneWeights, deviations) {}
        ///<summary>Get bone positions as Vector3 values.</summary>
        public Vector3[] GetVector3Bones() {return ConversionTools.Position3DArrayToVector3(Bones);}

    }

    ///<summary>This class uses HandPoseInWorldSpaceU instances in its constructors instead of
    ///HandPoseInWorldSpace instances in its base class Hands.</summary>
    public class HandsU : Hands
    {
        public HandsU(): base(null, null){}
        public HandsU(HandPoseInWorldSpaceU left, HandPoseInWorldSpaceU right): base(left, right){}
    }

}

