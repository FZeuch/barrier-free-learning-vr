#define OCULUSQUEST //TODO:löschen für reines Framework
using FreeHandGestureFramework.DataTypes;
using UnityEngine;
namespace FreeHandGestureUnity.OculusQuest
{
    //The tools and classes in this namespace assume that you to have installed the
    //Oculus Integration Asset for Unity.
    public static class ConversionToolsOQ
    {
#if OCULUSQUEST
        internal static Vector3[] GetSkeletonBonesLocalSpace(OVRSkeleton skeleton)
        {
            if (skeleton == null || skeleton.Bones.Count == 0) return null;
            Vector3[] bones = new Vector3[skeleton.Bones.Count];
            for (int i = 0; i<skeleton.Bones.Count; i++)
            {
                // Bone Vectors must be transformed from world space to local space to be comparable to a captured gesture.
                bones[i] = skeleton.transform.InverseTransformPoint(skeleton.Bones[i].Transform.position);
            }
            return bones;
        }
#endif
    }


    public class HandPoseUOQ : HandPoseU
    {
#if OCULUSQUEST
        public HandPoseUOQ(OVRSkeleton skeleton) : base(ConversionToolsOQ.GetSkeletonBonesLocalSpace(skeleton)) {}
#else
        public HandPoseUOQ(Vector3[] bones) : base(bones) {}
#endif
    }
    public class HandPoseInWorldSpaceUOQ : HandPoseInWorldSpaceU
    {
#if OCULUSQUEST
        public HandPoseInWorldSpaceUOQ(OVRSkeleton skeleton) 
            : base(ConversionToolsOQ.GetSkeletonBonesLocalSpace(skeleton), skeleton.transform) {}
#else
        public HandPoseInWorldSpaceUOQ(Vector3[] bones, Transform worldCoords) : base(bones, worldCoords) {}
#endif                   
    }
    public class WeightedHandPoseUOQ : WeightedHandPoseU
    {
#if OCULUSQUEST
        public readonly Deviation DEFAULT_HAND_ORIENTATION_TOLERANCES = new Deviation(25,35);
        public WeightedHandPoseUOQ() : base() {}
        public WeightedHandPoseUOQ(OVRSkeleton skeleton) : base(ConversionToolsOQ.GetSkeletonBonesLocalSpace(skeleton)) {}
        public WeightedHandPoseUOQ(OVRSkeleton skeleton, Vector3 lookingDirection) : this(skeleton)
        {
            RotationQuat localRotations = RotationQuat.GetRotationInLookDir(ConversionTools.QuaternionToRotationQuat(skeleton.transform.rotation),
                                                                    ConversionTools.Vector3ToPosition3D(lookingDirection));
            SetHandOrientation(localRotations, DEFAULT_HAND_ORIENTATION_TOLERANCES);
        }
        public WeightedHandPoseUOQ(OVRSkeleton skeleton, float[] boneWeights, Deviation[] deviations) : base(ConversionToolsOQ.GetSkeletonBonesLocalSpace(skeleton), boneWeights, deviations){}
#endif
    }

    public class HandsUOQ : HandsU
    {
#if OCULUSQUEST
        public HandsUOQ(OVRSkeleton left, OVRSkeleton right) : base(new HandPoseInWorldSpaceUOQ(left), new HandPoseInWorldSpaceUOQ(right)){}
#endif
    }
}
