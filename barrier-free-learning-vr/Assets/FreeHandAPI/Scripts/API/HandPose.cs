using StandardTools;
using FreeHandGestureFramework.EnumsAndTypes;
using System;
using System.Xml.Serialization;

namespace FreeHandGestureFramework
{
    public class HandPose
    {
        public Position3D[] Bones;
        public HandPose(Position3D[] bones)
        {
            FreeHandRuntime fhr = FreeHandRuntime.Instance;
            if (fhr!=null)
            {
                int bonesCount = FreeHandRuntime.Instance.HandSkeleton.GetBonesCount();
                if (bones != null && bones.Length == bonesCount)
                {
                    Bones = bones;
                }
                else //not the correct number of bones for this HandSkeleton
                {
                    throw new System.ArgumentException(string.Format("A {0} HandPose needs an array of bones with the exact length of {1}.", FreeHandRuntime.Instance.Platform.ToString(), bonesCount));
                }
            }
            else
            {
                    throw new System.ArgumentException("FreeHandRuntime is not initialized. Please set the platform.");
            }
        }
    }

    public class HandPoseInWorldSpace : HandPose
    {
        public Position3D Position;
        public Position3D Rotation;
        public Position3D Scale;
        public HandPoseInWorldSpace(Position3D[] bones, Position3D position, Position3D rotation, Position3D scale) : base(bones)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Position3D LocalToWorld(Position3D point)
        {
            return Position+point.Rotate(Rotation.X, Rotation.Y, Rotation.Z).Scale(Scale.X, Scale.Y, Scale.Z);
        }
        //TODO: implement WorldToLocal
    }
    [XmlInclude(typeof(FreeHandGestureUnity.WeightedHandPoseU)), XmlInclude(typeof(FreeHandGestureUnity.OculusQuest.WeightedHandPoseUOQ))]
    public class WeightedHandPose : HandPose
    {
        public WeightedHandPose() : this(new Position3D[FreeHandRuntime.Instance.HandSkeleton.GetBonesCount()]) {}
        public WeightedHandPose(Position3D[] bones): this(bones, FreeHandRuntime.Instance?.HandSkeleton.GetDefaultBoneWeights(), FreeHandRuntime.Instance?.HandSkeleton.GetDefaultDeviations()) {}
        public WeightedHandPose(Position3D[] bones, float[] boneWeights, Deviation[] deviations): base(bones)
        {
            int bonesCount = FreeHandRuntime.Instance.HandSkeleton.GetBonesCount();
            if (boneWeights==null || boneWeights.Length!=bonesCount
                || deviations==null || deviations.Length!=bonesCount)
            {
                throw new System.ArgumentException(string.Format("boneWeights and deviations must be arrays with the exact length of {0}.", bonesCount));
            }
            else
            {
                _boneWeights = boneWeights;
                _deviations = deviations;
            }
        }
        public WeightedHandPose(WeightedHandPose orig) : this(new Position3D[orig.Bones.Length])
        {
            for (int j = 0; j<Bones.Length; j++) Bones[j] = new Position3D(orig.Bones[j]);
            for (int j = 0; j<_boneWeights.Length; j++) _boneWeights[j] = orig._boneWeights[j]; 
            for (int j = 0; j<_deviations.Length; j++) _deviations[j] = new Deviation(orig._deviations[j]);
            HandOrientation = orig.HandOrientation==null?null:new Deviation3D(orig.HandOrientation);
        }


        private float[] _boneWeights;
        public float[] BoneWeights
        {
            get {return _boneWeights;}
            set 
            {
                if (value != null && value.Length == _boneWeights.Length)
                {
                    for (int i=0; i<_boneWeights.Length; i++) _boneWeights[i] = MathTools.Clamp01(value[i]);
                }
            }
        }

        private Deviation[] _deviations;
        public Deviation[] Deviations
        {
            get {return _deviations;}
            set {if (value != null && value.Length == _deviations.Length) _deviations = value;}
        }
        ///<value>If not null, HandOrientation defines the orientation of the hand which will be considered during gesture
        ///stage recognition. HandOrientation.RelativeVector should hold the hand rotation's Euler angles around the x, y and z axis.
        ///The tolerance values are degree values. 
        ///HandOrientation.ToleranceUp and HandOrientation.ToleranceDown correspond to the pitch (the rotation arount the x axis).
        ///HandOrientation.ToleranceLeft and HandOrientation.ToleranceRight correspond to the roll (the rotation arount the z axis).
        ///HandOrientation.ToleranceForward and HandOrientation.ToleranceBack correspond to the yaw (the rotation arount the y axis).
        /// E.g., if RelativeVector is set so that the thumb looks "up",
        ///and HandOrientation.ToleranceLeft is set to 30, it means that the rotation around the hand model's z axis
        ///can be up to 30 degrees less than defined in RelativeVector.X.
        ///This could mean that the hand can rotate up to 30 degrees to the left so that the thumb looks a little bit
        ///to the left, and the hand pose is still recognized. Note that if HandOrientation is null, hand orientation
        ///will not be considered at all during recognition.</value>
        public Deviation3D HandOrientation = null;

    }

    public class Hands
    {
        public HandPoseInWorldSpace Left;
        public HandPoseInWorldSpace Right;
        public Hands(): this(null, null){}
        public Hands(HandPoseInWorldSpace left, HandPoseInWorldSpace right)
        {
            Left = left;
            Right = right;
        }
    }
}