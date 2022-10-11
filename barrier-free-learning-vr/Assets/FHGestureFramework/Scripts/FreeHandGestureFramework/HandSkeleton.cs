using FreeHandGestureFramework.DataTypes;

namespace FreeHandGestureFramework
{
    ///<summary>The base class for a hand skeleton model.
    ///Specific models must be derived from this class.</summary>
    public abstract class HandSkeletonBase
    {
        public HandSkeletonBase()
        {
            _boneNames = new string[GetBonesCount()];
            _defaultBoneWeights = new float[GetBonesCount()];
            _defaultDeviations = new Deviation[GetBonesCount()];
        }
        ///<value>The names of the individual bones.</value>
        protected string[] _boneNames;
        ///<value>The default bone weights of the individual bones.</value>
        protected float[] _defaultBoneWeights;
        ///<value>The default tolerance values of the individual bones.</value>
        protected Deviation[] _defaultDeviations;
        ///<value>Returns the number of bones of a specific hand skeleton model.</value>
        public abstract int GetBonesCount();
        ///<summary>Returns the standard tolerance used for a specific hand skeleton model.</summary>
        public abstract Deviation GetStandardDeviation();
        ///<summary>Returns a copy of the default tolerance values of the individual bones.</summary>
        public Deviation[] GetDefaultDeviations()
        {
            Deviation[] devs = new Deviation[_defaultDeviations.Length];
            for(int j = 0; j< _defaultDeviations.Length; j++) devs[j]=new Deviation(_defaultDeviations[j]);
            return devs;
        }
        ///<summary>Returns the names of the individual bones.</summary>
        public string[] GetBoneNames(){return _boneNames;}
        ///<summary>Returns the default bone weights.</summary>
        public float[] GetDefaultBoneWeights(){return (float[]) _defaultBoneWeights.Clone();}
    }

    ///<summary>The hand skeleton model for the Oculus Quest 1 and 2</summary>
    public class HandSkeletonOculusQuest: HandSkeletonBase
    {
        private Deviation StandardDeviation;
        public HandSkeletonOculusQuest(): base()
        {
            //initialize bone names
            _boneNames[0] = "Hand_WristRoot";      // root frame of the hand, where the wrist is located
            _boneNames[1] = "Hand_ForearmStub";    // frame for user's forearm
            _boneNames[2] = "Hand_Thumb0";         // thumb trapezium bone
            _boneNames[3] = "Hand_Thumb1";         // thumb metacarpal bone
            _boneNames[4] = "Hand_Thumb2";         // thumb proximal phalange bone
            _boneNames[5] = "Hand_Thumb3";         // thumb distal phalange bone
            _boneNames[6] = "Hand_Index1";         // index proximal phalange bone
            _boneNames[7] = "Hand_Index2";         // index intermediate phalange bone
            _boneNames[8] = "Hand_Index3";         // index distal phalange bone
            _boneNames[9] = "Hand_Middle1";        // middle proximal phalange bone
            _boneNames[10] = "Hand_Middle2";       // middle intermediate phalange bone
            _boneNames[11] = "Hand_Middle3";       // middle distal phalange bone
            _boneNames[12] = "Hand_Ring1";         // ring proximal phalange bone
            _boneNames[13] = "Hand_Ring2";         // ring intermediate phalange bone
            _boneNames[14] = "Hand_Ring3";         // ring distal phalange bone
            _boneNames[15] = "Hand_Pinky0";        // pinky metacarpal bone
            _boneNames[16] = "Hand_Pinky1";        // pinky proximal phalange bone
            _boneNames[17] = "Hand_Pinky2";        // pinky intermediate phalange bone
            _boneNames[18] = "Hand_Pinky3";        // pinky distal phalange bone
            _boneNames[19] = "Hand_ThumbTip";      // tip of the thumb
            _boneNames[20] = "Hand_IndexTip";      // tip of the index finger
            _boneNames[21] = "Hand_MiddleTip";     // tip of the middle finger
            _boneNames[22] = "Hand_RingTip";       // tip of the ring finger
            _boneNames[23] = "Hand_PinkyTip";      // tip of the pinky

            //initialize default bone weights
            _defaultBoneWeights[0] = 0.0f;
            _defaultBoneWeights[1] = 0.0f;
            _defaultBoneWeights[2] = 0.0f;
            _defaultBoneWeights[3] = 0.5f;
            _defaultBoneWeights[4] = 0.5f;
            _defaultBoneWeights[5] = 0.0f;
            _defaultBoneWeights[6] = 0.5f;
            _defaultBoneWeights[7] = 0.5f;
            _defaultBoneWeights[8] = 0.0f;
            _defaultBoneWeights[9] = 0.5f;
            _defaultBoneWeights[10] = 0.5f;
            _defaultBoneWeights[11] = 0.0f;
            _defaultBoneWeights[12] = 0.5f;
            _defaultBoneWeights[13] = 0.5f;
            _defaultBoneWeights[14] = 0.0f;
            _defaultBoneWeights[15] = 0.4f;
            _defaultBoneWeights[16] = 0.3f;
            _defaultBoneWeights[17] = 0.3f;
            _defaultBoneWeights[18] = 0.0f;
            _defaultBoneWeights[19] = 1.0f;
            _defaultBoneWeights[20] = 1.0f;
            _defaultBoneWeights[21] = 1.0f;
            _defaultBoneWeights[22] = 1.0f;
            _defaultBoneWeights[23] = 1.0f;

            //initialize deviations
            StandardDeviation = new Deviation(0.0f, 0.04f, 2.0f);

            for (int i=0; i<GetBonesCount(); i++) _defaultDeviations[i] = GetStandardDeviation();
        }
        ///<summary>Returns 24, the bone count for the Oculus Quest hand model.</summary>
        public override int GetBonesCount()
        {
            return 24;
        }
        ///<summary>Returns the standard tolerance used for the oculus quest.</summary>
        public override Deviation GetStandardDeviation()
        {
            return new Deviation(StandardDeviation); 
        }
    }

    ///<summary>The hand skeleton model for the Microsoft HoloLens 2</summary>
    public class HandSkeletonHoloLens2: HandSkeletonBase
    {
        private Deviation StandardDeviation;
        public HandSkeletonHoloLens2(): base()
        {
            //initialize bone names
            _boneNames[0] = "Wrist";
            _boneNames[1] = "Thumb Metacarpal";
            _boneNames[2] = "Thumb Proximal";
            _boneNames[3] = "Thumb Distal";
            _boneNames[4] = "Thumb Tip";
            _boneNames[5] = "Index Metacarpal";
            _boneNames[6] = "Index Proximal";
            _boneNames[7] = "Index intermediate";
            _boneNames[8] = "Index Distal";
            _boneNames[9] = "Index Tip";
            _boneNames[10] = "Middle Metacarpal";
            _boneNames[11] = "Middle Proximal";
            _boneNames[12] = "Middle intermediate";
            _boneNames[13] = "Middle Distal";
            _boneNames[14] = "Middle Tip";
            _boneNames[15] = "Ring Metacarpal";
            _boneNames[16] = "Ring Proximal";
            _boneNames[17] = "Ring intermediate";
            _boneNames[18] = "Ring Distal";
            _boneNames[19] = "Ring Tip";
            _boneNames[20] = "Little Metacarpal";
            _boneNames[21] = "Little Proximal";
            _boneNames[22] = "Little intermediate";
            _boneNames[23] = "Little Distal";
            _boneNames[24] = "Little Tip";

            //initialize bone names
            _defaultBoneWeights[0] = 0.0f;
            _defaultBoneWeights[1] = 0.5f;
            _defaultBoneWeights[2] = 0.7f;
            _defaultBoneWeights[3] = 0.8f;
            _defaultBoneWeights[4] = 1.0f;
            _defaultBoneWeights[5] = 0.5f;
            _defaultBoneWeights[6] = 0.6f;
            _defaultBoneWeights[7] = 0.7f;
            _defaultBoneWeights[8] = 0.8f;
            _defaultBoneWeights[9] = 1.0f;
            _defaultBoneWeights[10] = 0.5f;
            _defaultBoneWeights[11] = 0.6f;
            _defaultBoneWeights[12] = 0.7f;
            _defaultBoneWeights[13] = 0.8f;
            _defaultBoneWeights[14] = 1.0f;
            _defaultBoneWeights[15] = 0.5f;
            _defaultBoneWeights[16] = 0.6f;
            _defaultBoneWeights[17] = 0.7f;
            _defaultBoneWeights[18] = 0.8f;
            _defaultBoneWeights[19] = 1.0f;
            _defaultBoneWeights[20] = 0.5f;
            _defaultBoneWeights[21] = 0.6f;
            _defaultBoneWeights[22] = 0.7f;
            _defaultBoneWeights[23] = 0.8f;
            _defaultBoneWeights[24] = 1.0f;

            //initialize deviations
            StandardDeviation = new Deviation(0.0f, 0.04f, 2.0f);

            for (int i=0; i<GetBonesCount(); i++) _defaultDeviations[i] = GetStandardDeviation();
        }
        ///<summary>Returns 25, the bone count for the HoloLens 2 hand model.</summary>
        public override int GetBonesCount()
        {
            return 25;
        }
        ///<summary>Returns the standard tolerance used for the HoloLens2.</summary>
        public override Deviation GetStandardDeviation()
        {
            return new Deviation(StandardDeviation); 
        }
    }

    ///<summary>The hand skeleton model for the HTC Vive Focus.</summary>
    public class HandSkeletonViveFocus: HandSkeletonBase
    {
        private Deviation StandardDeviation;
        public HandSkeletonViveFocus(): base()
        {
            //initialize bone names
            _boneNames[0] = "Wrist";
            _boneNames[1] = "Thumb Metacarpal";
            _boneNames[2] = "Thumb Proximal";
            _boneNames[3] = "Thumb Distal";
            _boneNames[4] = "Thumb Tip";
            _boneNames[5] = "Index Proximal";
            _boneNames[6] = "Index intermediate";
            _boneNames[7] = "Index Distal";
            _boneNames[8] = "Index Tip";
            _boneNames[9] = "Middle Proximal";
            _boneNames[10] = "Middle intermediate";
            _boneNames[11] = "Middle Distal";
            _boneNames[12] = "Middle Tip";
            _boneNames[13] = "Ring Proximal";
            _boneNames[14] = "Ring intermediate";
            _boneNames[15] = "Ring Distal";
            _boneNames[16] = "Ring Tip";
            _boneNames[17] = "Little Proximal";
            _boneNames[18] = "Little intermediate";
            _boneNames[19] = "Little Distal";
            _boneNames[20] = "Little Tip";

            //initialize bone weights
            _defaultBoneWeights[0] = 0.0f;
            _defaultBoneWeights[1] = 0.5f;
            _defaultBoneWeights[2] = 0.7f;
            _defaultBoneWeights[3] = 0.8f;
            _defaultBoneWeights[4] = 1.0f;
            _defaultBoneWeights[5] = 0.5f;
            _defaultBoneWeights[6] = 0.7f;
            _defaultBoneWeights[7] = 0.8f;
            _defaultBoneWeights[8] = 1.0f;
            _defaultBoneWeights[9] = 0.5f;
            _defaultBoneWeights[10] = 0.7f;
            _defaultBoneWeights[11] = 0.8f;
            _defaultBoneWeights[12] = 1.0f;
            _defaultBoneWeights[13] = 0.5f;
            _defaultBoneWeights[14] = 0.7f;
            _defaultBoneWeights[15] = 0.8f;
            _defaultBoneWeights[16] = 1.0f;
            _defaultBoneWeights[17] = 0.5f;
            _defaultBoneWeights[18] = 0.7f;
            _defaultBoneWeights[19] = 0.8f;
            _defaultBoneWeights[20] = 1.0f;

            //initialize deviations
            StandardDeviation = new Deviation(0.0f, 0.04f, 2.0f);

            for (int i=0; i<GetBonesCount(); i++) _defaultDeviations[i] = GetStandardDeviation();
        }
        ///<summary>Returns 21, the bone count for the Vieve Focus hand model.</summary>
        public override int GetBonesCount()
        {
            return 21;
        }
        ///<summary>Returns the standard tolerance used for the Vive Focus.</summary>
        public override Deviation GetStandardDeviation()
        {
            return new Deviation(StandardDeviation); 
        }
    }

}
