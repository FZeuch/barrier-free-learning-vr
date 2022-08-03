using FreeHandGestureFramework.EnumsAndTypes;
using System;

namespace FreeHandGestureFramework
{
    public abstract class HandSkeletonBase
    {
        public HandSkeletonBase()
        {
            _boneNames = new string[GetBonesCount()];
            _defaultBoneWeights = new float[GetBonesCount()];
            _defaultDeviations = new Deviation[GetBonesCount()];
        }
        protected string[] _boneNames;

        protected float[] _defaultBoneWeights;
        protected Deviation[] _defaultDeviations;
        public abstract int GetBonesCount();
        public abstract Deviation GetStandardDeviation();
        public Deviation[] GetDefaultDeviations()
        {
            Deviation[] devs = new Deviation[_defaultDeviations.Length];
            for(int j = 0; j< _defaultDeviations.Length; j++) devs[j]=new Deviation(_defaultDeviations[j]);
            return devs;
        }
        public string[] GetBoneNames(){return _boneNames;}//TODO:copy array?
        public float[] GetDefaultBoneWeights(){return (float[]) _defaultBoneWeights.Clone();}
    }


    public class HandSkeletonOculusQuest: HandSkeletonBase
    {
        private Deviation StandardDeviation;
        public HandSkeletonOculusQuest(): base()
        {
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

            _defaultBoneWeights[0] = 1.0f;
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

            StandardDeviation = new Deviation(0.0f, 0.04f, 2.0f); //TODO: experimentieren

            for (int i=0; i<GetBonesCount(); i++) _defaultDeviations[i] = GetStandardDeviation();
        }

        public override int GetBonesCount()
        {
            return 24;
        }

        public override Deviation GetStandardDeviation()
        {
            return new Deviation(StandardDeviation); 
        }
    }

}
