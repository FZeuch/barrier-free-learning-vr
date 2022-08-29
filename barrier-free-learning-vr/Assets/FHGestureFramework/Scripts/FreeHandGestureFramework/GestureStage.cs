using System.Collections.Generic;
using StandardTools;
using System;
using System.Xml.Serialization;
using FreeHandGestureFramework.DataTypes;

namespace FreeHandGestureFramework
{
    [XmlInclude(typeof(FreeHandGestureUnity.GestureStageU))]
    public class GestureStage
    {
        private const float DEFAULT_RECOGNITION_CONFIDENCE = 0.95f;
        private const float DEFAULT_RELEASE_CONFIDENCE = 0.75f;
        ///<value>The left hand pose that has to be performed by the user in order to recognize
        ///this gesture stage. In case of more than one pose, the stage will be considered
        ///recognized if one of the poses is recognized. If RightHandPoses also contains at
        ///least one pose, there are two cases: <list type="bullet"><item><description>
        ///If TwoHandedGesture is false, the stage will be recognized if a left OR right 
        ///hand pose is recognized</description></item><item><description> If TwoHandedGesture
        ///is true, the stage will be recognized if a left AND a right pose is recognized.</description></item></list>
        ///For more recognition options, refer to the description of RelHandsPosition,
        ///RelPosToLastPoseLeft and RelPosToLastPoseRight.</value>
        public List<WeightedHandPose> LeftHandPoses = new List<WeightedHandPose>();
        ///<value>The right hand pose that has to be performed by the user in order to recognize
        ///this gesture stage. In case of more than one pose, the stage will be considered
        ///recognized if one of the poses is recognized. If LeftHandPoses also contains at
        ///least one pose, there are two cases: <list type="bullet"><item><description>
        ///If TwoHandedGesture is false, the stage will be recognized if a left OR right 
        ///hand pose is recognized</description></item><item><description> If TwoHandedGesture
        ///is true, the stage will be recognized if a left AND a right pose is recognized.</description></item></list>
        ///For more recognition options, refer to the description of RelHandsPosition,
        ///RelPosToLastPoseLeft and RelPosToLastPoseRight.</value>
        public List<WeightedHandPose> RightHandPoses = new List<WeightedHandPose>();
        ///<value>Set to true, if both hands are relevant for gesture recognition.
        ///In that case, one of the left hand poses AND one of the right hand poses
        ///must be recognized for this gesture stage to be recognized.
        ///For more recognition options, refer to the description of RelHandsPosition,
        ///RelPosToLastPoseLeft and RelPosToLastPoseRight.</value>
        public bool TwoHandedGesture = false;
        ///<value>Set RelHandsPosition, if both hands must be held in a specified position
        ///relative to each other for this stage to be recognized (e.g. right hand 10cm right of left hand, or right hand 10-20cm
        ///above left hand). The directions are considered in looking direction.
        ///RelHandsPosition is only evaluated during stage recognition if TwoHandedGesture is set to true.
        ///If both LeftHandPoses and RightHandPoses are empty, ONLY the relative positions and not the poses of
        ///each hand will be evaluated. If only one of them is empty, one hand position must
        ///be recognized, then the relative position of both hands is evaluated.</value>
        public Deviation3D RelHandsPosition = null;
        ///<value>Set RelPosToLastPoseLeft, if the left hand must be placed in a specified position
        ///relative to the left hand position at the time the last stage was recognized.
        ///<list type="bullet"><item><description>If LeftHandPoses contains at least one pose, 
        ///a left hand pose must be recognized AND the hand must be in the specified relative position 
        ///for the stage to be recognized.</description></item>
        ///<item><description>If LeftHandPoses is empty, ONLY the relative position and not the hand pose is evaluated.</description></item>
        ///<item><description>If TwoHandedGesture is true, relative hands positions of both hands will be evaluated.</description></item></list></value>
        public Deviation3D RelPosToLastPoseLeft = null;
        ///<value>Set RelPosToLastPoseRight, if the right hand must be placed in a specified position
        ///relative to the right hand position at the time the last stage was recognized.
        ///<list type="bullet"><item><description>If RightHandPoses contains at least one pose, 
        ///a right hand pose must be recognized AND the hand must be in the specified relative position 
        ///for the stage to be recognized.</description></item>
        ///<item><description>If RightHandPoses is empty, ONLY the relative position and not the hand pose is evaluated.</description></item>
        ///<item><description>If TwoHandedGesture is true, relative hands positions of both hands will be evaluated.</description></item></list></value>
        public Deviation3D RelPosToLastPoseRight = null;
        ///<value>Set to true, if a path (a series of three dimensional positions) should
        ///be recorded during the hold phase of this gesture stage.</value>
        public bool RecordPath = false;
        ///<value>If set to true, and TwoHandedGesture is false, the ManipulationHand value will be set automatically to
        ///the hand which was recognized last. So, if you hava a thumbs up gesture, that can be performed either with the
        ///right or with the left hand, ManipulationHand will be set to left, if the last thumb you held up is your left thumb.
        ///The ManipulationHand value will not be set to another value but RelevantHand.Left or RelevantHand.Right.</value>
        public bool AutoSetManipulationHand = true;
        ///<value>Defines which hand is used for manipulation. This can be important in different situations.
        ///For example, imagine you have a button widget. It could be pressed by the right, the left or both hands
        ///to toggle. Another example could be a geture that is performed by holding a pose with the left hand and
        ///drawing with the right hand. In this case ManipulationHand must be set to RRelevantHand.Right.
        ///ManipulationHand can be set to Left, Right, Both and Any.</value>
        public RelevantHand ManipulationHand = RelevantHand.Right;
        ///<value>The bone of the right hand that is used for manipulation. This could for example be the tip
        ///of the index finger. If set to -1, the center of all bones is used.</value>
        public int RightManipulationBone = -1;
        ///<value>The bone of the left hand that is used for manipulation. This could for example be the tip
        ///of the index finger. If set to -1, the center of all bones is used.</value>
        public int LeftManipulationBone = -1;
        ///<value>If set to a positive value, any point that has a distance less or equal RightManipulationSphere
        ///to the point determined by RightManipulationBone, the right hand is considered "touching" the point.</value>
        public float RightManipulationSphere = 0.0f;
        ///<value>If set to a positive value, any point that has a distance less or equal LeftManipulationSphere
        ///to the point determined by LeftManipulationBone, the left hand is considered "touching" the point.</value>
        public float LeftManipulationSphere = 0.0f; 
        ///<value>An array of TransitionConditions, that define which sorts of transition conditions will lead to a
        ///stage transition. A gesture consist of one or more stages. There will be a transition to another stage
        ///if the corresponding TransitionConditions value is true, and the condition is met. The index of the
        ///next stage for a specific condition is defined in the TransitionTo array. If the target stage
        ///is -1, instead of doing a stage transition, the gesture will stop.</value>
        public bool[] TransitionConditions = new bool[Enum.GetValues(typeof(TransitionCondition)).Length];
        ///<value>Defines for each possible transition condition which would be the index of the following stage
        ///if that specific condition is met. A gesture consist of one or more stages. There will be a transition to another stage
        ///if the corresponding TransitionConditions value is true, and the condition is met. The index of the
        ///next stage for a specific condition is defined in the TransitionTo array. If the target stage
        ///is -1, instead of doing a stage transition, the gesture will stop.</value>
        public int[] TransitionTo = new int[Enum.GetValues(typeof(TransitionCondition)).Length];
        ///<value>This value is set automatically every time the confidence for this stage is calculated 
        ///(which might for example happen during GestureHandler.Update()). It will be set to
        ///<list type="bullet"><item><description>RelevantHand.Both if the stage is two handed and the confidence rating
        ///is equal or higher RecognitionConfidence</description></item>
        ///<item><description>RelevantHand.Left if the stage is one handed and the confidence for the left hand
        ///is equal or higher RecognitionConfidence and higher than the right hand confidence</description></item>
        ///<item><description>RelevantHand.Right if the stage is one handed and the confidence for the right hand
        ///is equal or higher RecognitionConfidence and higher than the left hand confidence</description></item>
        ///<item><description>RelevantHand.Any otherwise</description></item></list></value>
        internal RelevantHand LastHandRecognized = RelevantHand.Any;
        ///<value>When a stage starts, the LeftPosWhenPrevStageStarted and RightPosWhenPrevStageStarted
        ///values are set for all possible following stages. These stages can then use these values in order to
        ///detect a hand position change in their Confidence() methods if that is relevant for recognition.</value>
        internal Position3D LeftPosWhenPrevStageStarted = null;
        ///<value>When a stage starts, the LeftPosWhenPrevStageStarted and RightPosWhenPrevStageStarted
        ///values are set for all possible following stages. These stages can then use these values in order to
        ///detect a hand position change in their Confidence() methods if that is relevant for recognition.</value>
        internal Position3D RightPosWhenPrevStageStarted = null;
        public GestureStage(){}
        public GestureStage(GestureStage orig)
        {
            foreach(WeightedHandPose lp in orig.LeftHandPoses) LeftHandPoses.Add(new WeightedHandPose(lp));
            foreach(WeightedHandPose rp in orig.RightHandPoses) RightHandPoses.Add(new WeightedHandPose(rp));
            TwoHandedGesture = orig.TwoHandedGesture;
            RelHandsPosition = orig.RelHandsPosition == null ? null : new Deviation3D(orig.RelHandsPosition);
            RelPosToLastPoseLeft = orig.RelPosToLastPoseLeft == null ? null : new Deviation3D(orig.RelPosToLastPoseLeft);
            RelPosToLastPoseRight = orig.RelPosToLastPoseRight == null ? null : new Deviation3D(orig.RelPosToLastPoseRight);
            RecordPath = orig.RecordPath;
            AutoSetManipulationHand = orig.AutoSetManipulationHand;
            ManipulationHand = orig.ManipulationHand;
            RightManipulationBone = orig.RightManipulationBone;
            LeftManipulationBone = orig.LeftManipulationBone;
            RightManipulationSphere = orig.RightManipulationSphere;
            LeftManipulationSphere = orig.LeftManipulationSphere;
            TransitionConditions = orig.TransitionConditions;
            TransitionTo = orig.TransitionTo;
            _recognitionConfidence = orig._recognitionConfidence;
            _releaseConfidence = orig._releaseConfidence;
            _dwellTime = orig._dwellTime;
            _releaseWaitingTime = orig._releaseWaitingTime;
        }
        private event Delegates.FreeHandGestureEventHandler RaiseStageRecognizedEvent;
        private event Delegates.FreeHandGestureEventHandler RaiseStageDwellingEvent;
        private event Delegates.FreeHandGestureEventHandler RaiseStageStartEvent;
        private event Delegates.FreeHandGestureEventHandler RaiseStageHoldingEvent;
        private event Delegates.FreeHandGestureEventHandler RaiseStageTransitionEvent;
        private event Delegates.FreeHandGestureEventHandler RaiseStageReleasedEvent;

        private float _recognitionConfidence = DEFAULT_RECOGNITION_CONFIDENCE;
        ///<value>A value between 0 and 1 that defines the stage's recognition confidence.
        ///A gesture stage will be considered recognized, if its confidence rating
        ///is equal or higher RecognitionConfidence.</value>
        public float RecognitionConfidence
        {
            get {return _recognitionConfidence;}
            set {_recognitionConfidence = MathTools.Clamp01(value);}
        }
        private float _releaseConfidence = DEFAULT_RELEASE_CONFIDENCE;
        ///<value>A value between 0 and 1 that defines the stage's release confidence.
        ///A gesture stage will be considered released, if its confidence rating
        ///is less or equal ReleaseConfidence.</value>
        public float ReleaseConfidence
        {
            get {return _releaseConfidence;}
            set {_releaseConfidence = MathTools.Clamp01(value);}
        }
        private int _dwellTime = 0; //in milliseconds
        public int DwellTime
        {
            get {return _dwellTime;}
            set {_dwellTime = MathTools.Positive(value);}
        }
        private int _releaseWaitingTime = 0; //in milliseconds
        public int ReleaseWaitingTime
        {
            get {return _releaseWaitingTime;}
            set {_releaseWaitingTime = MathTools.Positive(value);}
        }
        [XmlIgnore]
        public DateTime StageRecognitionTime {get; internal set;} = DateTime.MinValue;
        public bool CheckTransitionCondition(TransitionCondition condition) {return TransitionConditions[(int)condition];}
        public int GetNextStageIndex(TransitionCondition condition) {return TransitionTo[(int)condition];}
        public void SetTransitionCondition(TransitionCondition condition, bool val, int destinationStage) 
        {
            TransitionConditions[(int)condition]=val;
            TransitionTo[(int)condition] = destinationStage;
        }
        public float Confidence(Hands handsToCompare, Position3D lookingDirection=null)
        {
            //1.If both hand poses are empty:
            //  1.1 if one handed gesture, calculate confidence with RelPosToLastPos (highest of both)
            //  1.2 if two handed, calculate confidence with RelPosToLastPos and/or RelHandsPosition
            //2.Else calculate the confidence values for each hand as a combination of hand pose confidences and RelPosToLastPos.
            //  If one of the items is null, only the other is evaluated.
            //  2.1 If one handed, total confidence is highest of both values 
            //  2.2 If two handed, total confidence is  
            //      2.2.1 the lowest of two confidences if both hand pose lists are not empty. Combine with RelHandsPosition if set.
            //      2.2.2 the confidence value of the hand which has a hand pose. Combine with RelHandsPosition if set.

            bool noLeftPosesInThisStage = LeftHandPoses == null || LeftHandPoses.Count == 0;
            bool noRightPosesInThisStage = RightHandPoses == null || RightHandPoses.Count == 0;
            bool leftHandToCompare = !(handsToCompare?.Left?.Bones == null || handsToCompare.Left.Bones.Length == 0);
            bool rightHandToCompare = !(handsToCompare?.Right?.Bones == null || handsToCompare.Right.Bones.Length == 0);
            
            //calculate confidence values concerning the relative positions of the hands for later use
            float confidenceRelPosToLastPoseLeft = 1.0f;
            if (RelPosToLastPoseLeft!=null)
            {
                if(LeftPosWhenPrevStageStarted == null || leftHandToCompare == false) confidenceRelPosToLastPoseLeft=0;
                else confidenceRelPosToLastPoseLeft=RelPosToLastPoseLeft.CalcPositionalDeviation(LeftPosWhenPrevStageStarted, 
                                                                                        GetHandPosition(handsToCompare.Left,true), 
                                                                                        lookingDirection);
            }

            float confidenceRelPosToLastPoseRight = 1.0f;
            if (RelPosToLastPoseRight!=null)
            {
                if(RightPosWhenPrevStageStarted == null || rightHandToCompare == false) confidenceRelPosToLastPoseRight=0;
                else confidenceRelPosToLastPoseRight=RelPosToLastPoseRight.CalcPositionalDeviation(RightPosWhenPrevStageStarted,
                                                                                        GetHandPosition(handsToCompare.Right,false),
                                                                                        lookingDirection);
            }

            float confidenceRelHandsPositions = 1.0f;
            if (RelHandsPosition!=null)
            {
                if(leftHandToCompare&&rightHandToCompare)
                {
                    confidenceRelHandsPositions = RelHandsPosition.CalcPositionalDeviation(GetHandPosition(handsToCompare.Left, true), 
                                                                                 GetHandPosition(handsToCompare.Right, true),
                                                                                 lookingDirection);
                }
                else confidenceRelHandsPositions = 0;
            }

            if(noLeftPosesInThisStage && noRightPosesInThisStage)
            {
                //In case this stage contains no left or right hand poses at all, confidence values will be calculated
                //(A) with the relative hand position(s) compared to the hand position(s) at the beginning of the previous stage
                //    if TwoHandedGesture is false and RelPosToLastPose.. value(s) exist(s)
                //(B1) with the position of both hands compared to RelHandsPosition if TwoHandedGesture is true, RelHandsPosition 
                //     is not null and RelPosToLastPoseLeft and RelPosToLastPoseRight are null
                //(B2) with the relative hand positions of both hands compared to both hand positions at the beginning of the previous
                //     stage if TwoHandedGesture is true, RelPosToLastPose values exist and RelHandsPosition is null
                //(B3) with the position of both hands compared to RelHandsPosition, combined with the relative hand position(s)
                //     compared to the hand position(s) at the beginning of the previous stage if TwoHandedGesture is true,
                //     RelHandsPosition is not null and RelPosToLastPoseLeft or RelPosToLastPoseRight is not null.
                //(B4) as 0 if TwoHandedGesture is true, but RelHandsPosition is null and at least one RelPosToLastPose.. is null
                if(TwoHandedGesture == false)
                {
                    if(RelPosToLastPoseLeft!=null && RelPosToLastPoseRight!= null)
                        return Math.Max(confidenceRelPosToLastPoseLeft,confidenceRelPosToLastPoseRight); //case (A)
                    else 
                        return Math.Min(confidenceRelPosToLastPoseLeft,confidenceRelPosToLastPoseRight);
                }
                else //TwoHandedGesture==true
                {
                    if(RelHandsPosition==null)
                    {
                        if(RelPosToLastPoseLeft==null || RelPosToLastPoseRight==null) return 0; //case (B4)
                        else return confidenceRelPosToLastPoseLeft*confidenceRelPosToLastPoseRight; //case (B2)
                    }
                    else //RelHandsPosition!=null
                    {
                        if (RelPosToLastPoseLeft==null && RelPosToLastPoseRight==null) return confidenceRelHandsPositions;//case (B1)
                        else return confidenceRelHandsPositions*confidenceRelPosToLastPoseLeft*confidenceRelPosToLastPoseRight;//case (B3)
                    }
                }
            }

            else //either a left or a right hand pose exists in this stage
            {
                //In case this stage contains a left and/or a right hand pose, confidence values will be calculated
                //with the confidence values for each hand pose combined with the already calculated confidenceRelPosToLastPose
                //values:
                //(C)Return the higher value, if one handed gesture.
                //(D)Return the lower value combined with the already calculated confidenceRelHandsPositions value if two handed

                //calculate left and right hand confidences for later use and set LastHandRecognized
                float leftHandConfidence;
                if (noLeftPosesInThisStage) leftHandConfidence = 0;
                else if (leftHandToCompare)
                {
                    leftHandConfidence = 0.0f;
                    foreach(WeightedHandPose pose in LeftHandPoses)
                    {
                        float conf = Confidence(pose, handsToCompare.Left, lookingDirection);
                        if (conf > leftHandConfidence) leftHandConfidence = conf;
                    }
                }
                else leftHandConfidence = 1.0f;

                float rightHandConfidence;
                if (noRightPosesInThisStage) rightHandConfidence = 0;
                else if(rightHandToCompare)
                {
                    rightHandConfidence = 0.0f;
                    foreach(WeightedHandPose pose in RightHandPoses)
                    {
                        float conf = Confidence(pose, handsToCompare.Right, lookingDirection);
                        if (conf > rightHandConfidence) rightHandConfidence = conf;
                    }
                }
                else rightHandConfidence = 1.0f;

                LastHandRecognized=RelevantHand.Any;//any means that no hand was recognized.
                if (TwoHandedGesture &&
                    ((leftHandConfidence>rightHandConfidence && rightHandConfidence>=RecognitionConfidence) ||
                    (rightHandConfidence>=leftHandConfidence && leftHandConfidence>=RecognitionConfidence)))
                    LastHandRecognized=RelevantHand.Both;//both hands were recognized
                else if(!TwoHandedGesture && leftHandConfidence>rightHandConfidence && leftHandConfidence>=RecognitionConfidence)
                    LastHandRecognized=RelevantHand.Left;//left hand was recognized
                else if(!TwoHandedGesture && rightHandConfidence>=leftHandConfidence && rightHandConfidence>=RecognitionConfidence)
                    LastHandRecognized=RelevantHand.Right;//right hand was recogniized

                leftHandConfidence *= confidenceRelPosToLastPoseLeft;
                rightHandConfidence *= confidenceRelPosToLastPoseRight;
                if (!TwoHandedGesture) return  Math.Max(leftHandConfidence, rightHandConfidence);//case (C)
                else return Math.Min(leftHandConfidence, rightHandConfidence) * confidenceRelHandsPositions;//case (D)
            }
        }

        ///<summary>This method subscribes a listener method of type FreeHandGestureEventHandler to an
        ///event specified by type. The subscribed method will then be called if the gesture stage
        ///raises that event. E.g. if you add a listener method and set type to Recognized,
        ///the listener will be called at the time the gesture stage was recognized.
        ///NOTE: You may consider adding event listeners to the GestureHandler instance instead of different
        ///gesture stages. In that case, the listener method would be called if ANY stage was recognized.</summary>
        public void AddEventListener (GestureEventTypes type, Delegates.FreeHandGestureEventHandler listener)
        {
            switch (type)
            {
                case GestureEventTypes.Recognized: RaiseStageRecognizedEvent += listener; break;
                case GestureEventTypes.Dwelling: RaiseStageDwellingEvent += listener; break;
                case GestureEventTypes.Start: RaiseStageStartEvent += listener; break;
                case GestureEventTypes.Holding: RaiseStageHoldingEvent += listener; break;
                case GestureEventTypes.StageTransition: RaiseStageTransitionEvent += listener; break;
                case GestureEventTypes.Released: RaiseStageReleasedEvent += listener; break;
            }
        }

        public void RemoveEventListener (GestureEventTypes type, Delegates.FreeHandGestureEventHandler listener)
        {
            switch (type)
            {
                case GestureEventTypes.Recognized: RaiseStageRecognizedEvent -= listener; break;
                case GestureEventTypes.Dwelling: RaiseStageDwellingEvent -= listener; break;
                case GestureEventTypes.Start: RaiseStageStartEvent -= listener; break;
                case GestureEventTypes.Holding: RaiseStageHoldingEvent -= listener; break;
                case GestureEventTypes.StageTransition: RaiseStageTransitionEvent -= listener; break;
                case GestureEventTypes.Released: RaiseStageReleasedEvent -= listener; break;
            }
        }

        public Position3D GetHandPosition(HandPoseInWorldSpace pose, bool left=false)
        {
            if (pose==null || pose.Bones==null || pose.Bones.Length == 0) return null;
            if (left && LeftManipulationBone>0 && LeftManipulationBone<pose.Bones.Length) return pose.LocalToWorld(pose.Bones[LeftManipulationBone]);
            if (!left && RightManipulationBone>0 && RightManipulationBone<pose.Bones.Length) return pose.LocalToWorld(pose.Bones[RightManipulationBone]);
            //return the center of the bounding box around all bones
            float xmin=float.MaxValue, xmax=float.MinValue;
            float ymin=float.MaxValue, ymax=float.MinValue;
            float zmin=float.MaxValue, zmax=float.MinValue;
            foreach(Position3D p in pose.Bones)
            {
                if(p.X<xmin) xmin=p.X; else if(p.X>xmax) xmax=p.X;
                if(p.Y<ymin) ymin=p.Y; else if(p.Y>ymax) ymax=p.Y;
                if(p.Z<zmin) zmin=p.Z; else if(p.Z>zmax) zmax=p.Z;
            }
            return pose.Position+new Position3D(xmin+(xmax-xmin)/2, ymin+(ymax-ymin)/2, zmin+(zmax-zmin)/2);//TODO:test
        }

        internal void RaiseEvent(GestureEventTypes type, FreeHandEventArgs args)
        {
            switch (type)
            {
                case GestureEventTypes.Recognized: RaiseStageRecognizedEvent?.Invoke(this, args); break;
                case GestureEventTypes.Dwelling: RaiseStageDwellingEvent?.Invoke(this, args); break;
                case GestureEventTypes.Start: RaiseStageStartEvent?.Invoke(this, args); break;
                case GestureEventTypes.Holding: RaiseStageHoldingEvent?.Invoke(this, args); break;
                case GestureEventTypes.StageTransition: RaiseStageTransitionEvent?.Invoke(this, args); break;
                case GestureEventTypes.Released: RaiseStageReleasedEvent?.Invoke(this, args); break;
            }
        }

        private float Confidence(WeightedHandPose gesturePose, HandPoseInWorldSpace comparePose, Position3D lookingDirection)
        {
            float totalWeight = 0.0f;
            int boneCount = gesturePose.Bones.Length;
            for (int i=0; i<boneCount; i++) totalWeight += gesturePose.BoneWeights[i];
            float totalConfidence = 0.0f;
            
            for (int i=0; i<boneCount; i++)
            {
                //distance between the bones of the gesture and of the hand position
                float distance = gesturePose.Bones[i].Distance(comparePose.Bones[i]);
                float confidenceForThisBone = gesturePose.Deviations[i].CalcDeviation(distance);
                //Add this bone's impact to the total confidence. 
                totalConfidence += confidenceForThisBone * gesturePose.BoneWeights[i] / totalWeight;
            }
            //consider hand orientation
            float orientationConf = 1;
            if (gesturePose.HandOrientation!=null && lookingDirection!=null)
            {
                orientationConf = gesturePose.HandOrientation.CalcSimpleDeviation(RotationQuat.GetRotationInLookDir(comparePose.Rotation, lookingDirection).GetEulerAngles(), 360);
            }
            return totalConfidence*orientationConf;
        }
        
    }
}