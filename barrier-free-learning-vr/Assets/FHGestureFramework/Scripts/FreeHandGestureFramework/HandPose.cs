using StandardTools;
using FreeHandGestureFramework.DataTypes;
using System.Xml.Serialization;

namespace FreeHandGestureFramework
{
    ///<summary>Base class for classes defining a hand pose with skeletal data.</summary>
    public class HandPose
    {
        ///<value>The array of bone positions.</value>
        public Position3D[] Bones;
        public HandPose(Position3D[] bones)
        {
            GestureHandler gh = GestureHandler.Instance;
            if (gh!=null)
            {
                int bonesCount = GestureHandler.Instance.HandSkeleton.GetBonesCount();
                if (bones != null && bones.Length == bonesCount)
                {
                    Bones = bones;
                }
                else //not the correct number of bones for this HandSkeleton
                {
                    throw new System.ArgumentException(string.Format("A {0} HandPose needs an array of bones with the exact length of {1}.", GestureHandler.Instance.Platform.ToString(), bonesCount));
                }
            }
            else
            {
                    throw new System.ArgumentException("GestureHandler is not initialized. Please set the platform.");
            }
        }
    }

    ///<summary>This class represents a skeletal hand pose along with its position, rotation and scale in world space.</summary>
    public class HandPoseInWorldSpace : HandPose
    {
        ///<value>The anchor position in world space.</value>
        public Position3D Position;
        ///<value>The anchor rotation in world space.</value>
        public RotationQuat Rotation;
        ///<value>The anchor scale in world space.</value>
        public Position3D Scale;
        public HandPoseInWorldSpace(Position3D[] bones, Position3D position, RotationQuat rotation, Position3D scale) : base(bones)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
        ///<summary>Returns the world coordinates of a position inside the local coordinate system of this hand pose's anchor.</summary>
        ///<param name="point">A point in the local coordinate system of this hand pose's anchor. This will normally be a member
        ///of this object's Bones array.</param>
        public Position3D LocalToWorld(Position3D point)
        {
            return Position+point.Rotate(Rotation).Scale(Scale.X, Scale.Y, Scale.Z);
        }
    }

    ///<summary>This class represents a skeletal hand pose with individual bone weights and tolerance values for each bone.</summary>
    [XmlInclude(typeof(FreeHandGestureUnity.WeightedHandPoseU)), XmlInclude(typeof(FreeHandGestureUnity.OculusQuest.WeightedHandPoseUOQ))]
    public class WeightedHandPose : HandPose
    {
        public WeightedHandPose() : this(new Position3D[GestureHandler.Instance.HandSkeleton.GetBonesCount()]) {}
        public WeightedHandPose(Position3D[] bones): this(bones, GestureHandler.Instance?.HandSkeleton.GetDefaultBoneWeights(), GestureHandler.Instance?.HandSkeleton.GetDefaultDeviations()) {}
        public WeightedHandPose(Position3D[] bones, float[] boneWeights, Deviation[] deviations): base(bones)
        {
            int bonesCount = GestureHandler.Instance.HandSkeleton.GetBonesCount();
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
        ///<value>Weight values between 0 and 1 which define the weight of a bone during confidence calculation.</value>
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
        ///<value>Tolerance values for each single bone that define how exact a user's bone position
        ///must match the on defined by this hand pose.</value>
        public Deviation[] Deviations
        {
            get {return _deviations;}
            set {if (value != null && value.Length == _deviations.Length) _deviations = value;}
        }
        ///<value>If not null, HandOrientation defines the orientation of the hand which will be considered during gesture
        ///stage recognition. HandOrientation.RelativeVector should hold the hand rotation's Euler angles around the x, y and z axis.
        ///The tolerance values are degree values. If null, hand orientation will no be considered during confidence calculation.</value>
        public Deviation3D HandOrientation = null;
        ///<summary>Set the hand orientation with one tolerance value used for all tolerances.</summary>
        ///<param name="rotation">The rotational information.</param>
        ///<param name="tolerance">The Deviation class used for all six tolerance values. A and B are angle values.</param>
        public void SetHandOrientation(RotationQuat rotation, Deviation tolerance)
        {
            HandOrientation = new Deviation3D(rotation.GetEulerAngles(), tolerance);
        }
        ///<summary>Set the hand orientation with individual tolerance values.</summary>
        ///<param name="rotation">The rotational information.</param>
        ///<param name="tolNegX">The tolerance for a negative rotation around the X axis. A and B are angle values.</param>
        ///<param name="tolPosX">The tolerance for a positive rotation around the X axis. A and B are angle values.</param>
        ///<param name="tolNegY">The tolerance for a negative rotation around the Y axis. A and B are angle values.</param>
        ///<param name="tolPosY">The tolerance for a positive rotation around the Y axis. A and B are angle values.</param>
        ///<param name="tolNegZ">The tolerance for a negative rotation around the Z axis. A and B are angle values.</param>
        ///<param name="tolPosZ">The tolerance for a positive rotation around the Z axis. A and B are angle values.</param>
        public void SetHandOrientation(RotationQuat rotation, Deviation tolNegX, Deviation tolPosX, Deviation tolNegY, Deviation tolPosY, Deviation tolNegZ, Deviation tolPosZ)
        {
            HandOrientation = new Deviation3D(rotation.GetEulerAngles(), tolNegX, tolPosX, tolNegY, tolPosY, tolNegZ, tolPosZ);
        }
    }

    ///<summary>This class represents a user's skeletal left and right hand poses along with their positions, rotations and scales 
    ///in world space.</summary>
    public class Hands
    {
        ///<value>The left hand pose.</value>
        public HandPoseInWorldSpace Left;
        ///<summary>The right hand pose.</value>
        public HandPoseInWorldSpace Right;
        public Hands(): this(null, null){}
        public Hands(HandPoseInWorldSpace left, HandPoseInWorldSpace right)
        {
            Left = left;
            Right = right;
        }
    }
}