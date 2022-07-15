using FreeHandGestureFramework.EnumsAndTypes;
using UnityEngine;

namespace FreeHandGestureUnity.OculusQuest
{
    //The tools and classes in this namespace assume that you to have installed the
    //Oculus Integration Asset for Unity.
    public static class ConversionToolsOQ2
    {
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
    }


    public class HandPoseUOQ : HandPoseU
    {
        public HandPoseUOQ(OVRSkeleton skeleton) : base(ConversionToolsOQ2.GetSkeletonBonesLocalSpace(skeleton)) {}
    }
    public class HandPoseInWorldSpaceUOQ : HandPoseInWorldSpaceU
    {
        public HandPoseInWorldSpaceUOQ(OVRSkeleton skeleton) 
            : base(ConversionToolsOQ2.GetSkeletonBonesLocalSpace(skeleton), skeleton.transform) {}
                   
    }
    public class WeightedHandPoseUOQ : WeightedHandPoseU
    {
        public readonly Deviation DEFAULT_HAND_ORIENTATION_TOLERANCES = new Deviation(30,45);
        public WeightedHandPoseUOQ() : base() {}
        public WeightedHandPoseUOQ(OVRSkeleton skeleton) : base(ConversionToolsOQ2.GetSkeletonBonesLocalSpace(skeleton)) {}
        public WeightedHandPoseUOQ(OVRSkeleton skeleton, Vector3 lookingDirection) : this(skeleton)
        {
            Position3D localRotations = Position3D.GetLocalRotation(ConversionTools.Vector3ToPosition3D(skeleton.transform.rotation.eulerAngles),
                                                                    ConversionTools.Vector3ToPosition3D(lookingDirection));
            HandOrientation = new Deviation3D(localRotations, DEFAULT_HAND_ORIENTATION_TOLERANCES);
        }
        public WeightedHandPoseUOQ(OVRSkeleton skeleton, float[] boneWeights, Deviation[] deviations) : base(ConversionToolsOQ2.GetSkeletonBonesLocalSpace(skeleton), boneWeights, deviations){}
    }

    public class HandsUOQ2 : HandsU
    {
        public HandsUOQ2(OVRSkeleton left, OVRSkeleton right) : base(new HandPoseInWorldSpaceUOQ(left), new HandPoseInWorldSpaceUOQ(right)){}
    }

}