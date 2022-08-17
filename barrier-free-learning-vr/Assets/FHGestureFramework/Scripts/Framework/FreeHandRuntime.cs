using System.Collections.Generic;
using System;
using FreeHandGestureFramework.EnumsAndTypes;
using StandardTools;

namespace FreeHandGestureFramework
{
    public sealed class FreeHandRuntime
    {
        //This is a thread-safe singleton class.

        ///<summary>Defines the (hardware) platform for the FreeHandRuntime instance.
        ///This value must be set, otherwise FreeHandRuntime.Instance will return null.</summary> 
        public Platforms Platform;
        ///<summary>The list of gestures FreeHandRuntime will try to recognize while active</summary>
        public List<Gesture> Gestures;
        private static bool PlatformSet = false;//The platform must be set before Instance returns this instance
        private bool RuntimeIsActive;//If not active, no gesture recognition is done
        private DateTime TimeOfLastUpdate;//Time stamp of the last call of Update()

        private FreeHandRuntime() 
        {
            Initialize();
        }  
        
        //The EVENTS which this class is able to raise
        private event Delegates.FreeHandGestureEventHandler RaiseGestureRecognizedEvent; //Will be raised when any gesture is recognized
        private event Delegates.FreeHandGestureEventHandler RaiseGestureDwellingEvent; //Will be raised during each Update() if active stage of active gesture is dwelling
        private event Delegates.FreeHandGestureEventHandler RaiseGestureStartEvent; //Will be raised if the active gesture starts (after uninterrupted dwell time or instantly if there is no dwell time)
        private event Delegates.FreeHandGestureEventHandler RaiseGestureHoldingEvent; //Will be raised if active stage of active gesture is performed.
        private event Delegates.FreeHandGestureEventHandler RaiseGestureStageTransitionEvent; //Will be raised if one of the transition conditions for the next stage/end of the active gesture is met.
        private event Delegates.FreeHandGestureEventHandler RaiseGestureReleasedEvent; //Will be raised if the active gesture stops.
        
        ///<summary>Returns the only instance of FreeHandRuntime. If the platform is not yet set by calling 
        ///SetPlatform(), null is returned.</summary>
        public static FreeHandRuntime Instance 
        { 
            get 
            { 
                if(!PlatformSet)return null;
                else return Nested.instance; 
            } 
        }
        ///<summary>If a gesture is active, ActiveGesture returns the index of the active gesture which can be
        ///used wich the gesture list. If no gesture is active, -1 is returned.</summary>
        public int ActiveGesture {get; private set;}
        ///<summary>Returns the hand skeleton model of the current platform.</summary>
        public HandSkeletonBase HandSkeleton {get; private set;}
        ///<summary>Sets the (hardware) platform for this instance of FreeHandRuntime.
        ///This method has to be called once before Instance() will return the FreeHandRuntime instance.</summary> 
        ///<param name="platform">The platform from the list of supported platforms.</param>
        ///<param name="customHandSkeleton">If you want to use a custom platform, the hand skeleton model must be handed over here. Otherwise, this value will be ignored.</param>
        public static void SetPlatform(Platforms platform, HandSkeletonBase customHandSkeleton = null)
        {
            if (platform == Platforms.Custom && customHandSkeleton == null) 
            {
                throw new System.ArgumentException("When initializing FreeHandRuntime to a custom platform, customHandSkeletons must not be null. Please use a custom class derived from FreeHandGestureFramework.HandSkeletonBase.");
            }

            if (PlatformSet == true) //In case SetPlatform() was called in order to CHANGE the platform
            {
                Instance.Initialize();
            }
            PlatformSet = true;
            Instance.Platform = platform;

            //Set the HandSkeleton for the corresponding platform
            switch(platform)
            {
            case Platforms.Custom:
                Instance.HandSkeleton = customHandSkeleton;
                break;
            case Platforms.OculusQuest:
                Instance.HandSkeleton = new HandSkeletonOculusQuest();
                break;
            case Platforms.HoloLens2:
                Instance.HandSkeleton = new HandSkeletonHoloLens2();
                break;
            case Platforms.ViveFocus:
                Instance.HandSkeleton = new HandSkeletonViveFocus();
                break;
            }
        }
        ///<summary>Returns true, if the runtime is active and false otherwise. When the runtime
        ///is not active, no gestures will be recognized.</summary>
        public bool IsActive() {return RuntimeIsActive;}
        ///<summary>Set the runtime active or inactive.When the runtime
        ///is not active, no gestures will be recognized.</summary>
        public void SetActive(bool val) 
        {
            if (val == false) StopCurrentGesture();
            RuntimeIsActive = val;
        } 
        ///<summary>If there is an active gesture, that gesture is returned. Otherwise, null is returned.
        ///Note that only one gesture can be active.</summary>
        public Gesture GetActiveGesture()
        {
            if (ActiveGesture == -1) return null;
            else return Gestures[ActiveGesture];
        }
        ///<summary>In the Update method, the runtime will try to recognize a gesture from the gesture list
        ///or, if a gesture is already active, to recognize a transition to the next stage,
        ///using the current right and left hand data, which must be provided.</summary>
        ///<param name="currentHands">The current poses of left and right hand which the user holds at that moment</param>
        ///<param name="lookingDirection">In some cases, the looking direction is important for the recognition of
        ///the next gesture stage (e.g. if moving to the next stage involves the movement of the hand in a given
        ///direction relative to the looking direction). The looking direction vector might for example be the
        ///forward direction vector of the (left eye of the) camera.</param>
        public void Update(Hands currentHands, Position3D lookingDirection = null)
        {
            if (!RuntimeIsActive || Gestures.Count == 0 || currentHands==null) return;//no gesture recognition if inactive or no hands are provided

            DateTime timeStamp = DateTime.Now;//for later use
            
            if (ActiveGesture == -1)
            {
                //No gesture is active, try to recognize a gesture
                float highestConfidence = 0; //the highest confidence rating of all recognized gestures
                int highestConfIndex = -1;   //...and the index of the gesture that has that confidence reting
                foreach(Gesture g in Gestures)
                {
                    GestureStage gs = g.Stages[0];
                    if (gs != null)
                    {
                        float confidence = gs.Confidence(currentHands,lookingDirection);//confidence value of first gesture stage
                        if (confidence >= gs.RecognitionConfidence && confidence > highestConfidence)
                        {
                            //This gesture was recognized and has the highest recognition conf. of all recognized gestures so far.
                            highestConfidence = confidence;
                            highestConfIndex = Gestures.IndexOf(g);
                        }
                    }
                }
                //If hand poses were recognized, we now know which gesture has the highest confidence rating

                if(highestConfIndex!=-1)
                {
                    ActivateGesture(highestConfIndex, currentHands, timeStamp);
                }
            }

            else //A gesture is active, check for next gesture stage, gesture release, or dwell time over.
            {
                Gesture activeGesture = Gestures[ActiveGesture];
                GestureStage activeStage = Gestures[ActiveGesture].GetCurrentStage();

                //case 1: during stage dwell time
                if (activeStage.DwellTime > 0 && activeGesture.GetMillisSinceStageRecognized(timeStamp) < activeStage.DwellTime)
                {
                    //Check if hand pose was released during dwell time
                    if (activeStage.ReleaseConfidence > activeStage.Confidence(currentHands, lookingDirection))
                    {
                        //release conditions met during dwell time, abort dwell and stop gesture
                        StopGesture(currentHands, timeStamp);
                    }
                    else //release conditions not met during dwell time, get on dwelling
                    {
                        RaiseEvent(GestureEventTypes.Dwelling, currentHands,timeStamp);
                    }
                }

                //case 2: dwell time just over
                else if (activeStage.DwellTime > 0 && (activeGesture.GetMillisSinceStageRecognized(TimeOfLastUpdate) < activeStage.DwellTime))
                {
                    StartStage(currentHands, timeStamp);
                }

                //case 3: dwell time is over or no dwell time
                else
                {
                    //Check if a stage transition condition is met
                    bool transitionConditionMet = false;
                    int targetStage = -1;
                    int currentStageIndex = activeGesture.GetCurrentStageIndex();
                    if (activeStage.CheckTransitionCondition(TransitionCondition.Start))
                    {
                        //conditon is met when the stage "starts" (dwell time over or recognition if dwell time is 0)
                        transitionConditionMet = true;
                        targetStage = activeStage.GetNextStageIndex(TransitionCondition.Start);
                    }
                    else if (activeStage.CheckTransitionCondition(TransitionCondition.NextPoseRecognized)
                            && activeStage.GetNextStageIndex(TransitionCondition.NextPoseRecognized)>0
                            && activeStage.GetNextStageIndex(TransitionCondition.NextPoseRecognized)<activeGesture.StageCount
                            && activeGesture.Stages[activeStage.GetNextStageIndex(TransitionCondition.NextPoseRecognized)].Confidence(currentHands ,lookingDirection)
                               >=activeGesture.Stages[activeStage.GetNextStageIndex(TransitionCondition.NextPoseRecognized)].RecognitionConfidence)
                    {
                        //condition is met if the pose that is targeted by the TransitionTo[NextPoseRecognized] value
                        //is recognized (confidence value higher than recognition confidence)
                        transitionConditionMet = true;
                        targetStage=activeStage.GetNextStageIndex(TransitionCondition.NextPoseRecognized);
                    }
                    else if (activeStage.CheckTransitionCondition(TransitionCondition.Release)
                            && activeGesture.GetMillisSinceStageRecognized(TimeOfLastUpdate) > activeStage.DwellTime + activeStage.ReleaseWaitingTime
                            && activeStage.Confidence(currentHands, lookingDirection) <= activeStage.ReleaseConfidence)
                    {
                        //condition is met if the pose of this stage is released (confidence <= release confidence)
                        //and a specified amount of time has passed (ReleaseWaitingTime)
                        transitionConditionMet = true;
                        targetStage = activeStage.GetNextStageIndex(TransitionCondition.Release);
                    }
                    
                    //If condition is met, do transition to next stage or stop gesture if this is last stage
                    if (transitionConditionMet) 
                    {
                        DoStageTransition(currentHands, timeStamp, targetStage);
                    }
                    else //no transition condition is met, gesture is performed
                    {
                        RecordPath(currentHands, timeStamp);
                        RaiseEvent(GestureEventTypes.Holding, currentHands,timeStamp);
                    }
                }

                TimeOfLastUpdate = timeStamp;
            }
        }

        ///<summary>This method subscribes a listener method of type FreeHandGestureEventHandler to an
        ///event specified by type. The subscribed method will then be called if ANY of the gesture
        ///stages raises that event. E.g. if you add a listener method and set type to Recognized,
        ///the listener will be called every time a gesture (or next gesture stage) is recognized.
        ///The listener will receive an instance of FreeHandAPIEventArgs, which contains all relevant data
        ///(e.g. gesture and stage index). NOTE: You may consider adding event listeners directly
        ///to a gesture stage, so that the listener method will only be called if that specific stage raises
        ///the event.</summary>
        ///<param name="type">The type of the event.</param>
        ///<param name="listener">The callback routine which will be called when the specified event is raised.</param>
        public void AddEventListener (GestureEventTypes type, Delegates.FreeHandGestureEventHandler listener)
        {
            switch (type)
            {
                case GestureEventTypes.Recognized: RaiseGestureRecognizedEvent += listener; break;
                case GestureEventTypes.Dwelling: RaiseGestureDwellingEvent += listener; break;
                case GestureEventTypes.Start: RaiseGestureStartEvent += listener; break;
                case GestureEventTypes.Holding: RaiseGestureHoldingEvent += listener; break;
                case GestureEventTypes.StageTransition: RaiseGestureStageTransitionEvent += listener; break;
                case GestureEventTypes.Released: RaiseGestureReleasedEvent += listener; break;
            }
        }
        ///<summary>Removes an event listener from an event specified by type.</summary>
        ///<param name="type">The type of the event.</param>
        ///<param name="listener">The callback routine which should be removed.</param>
        public void RemoveEventListener (GestureEventTypes type, Delegates.FreeHandGestureEventHandler listener)
        {
            switch (type)
            {
                case GestureEventTypes.Recognized: RaiseGestureRecognizedEvent -= listener; break;
                case GestureEventTypes.Dwelling: RaiseGestureDwellingEvent -= listener; break;
                case GestureEventTypes.Start: RaiseGestureStartEvent -= listener; break;
                case GestureEventTypes.Holding: RaiseGestureHoldingEvent -= listener; break;
                case GestureEventTypes.StageTransition: RaiseGestureStageTransitionEvent -= listener; break;
                case GestureEventTypes.Released: RaiseGestureReleasedEvent -= listener; break;
            }
        }
        ///<summary>If the active gesture stage has the Cue value set in TransitionConditions, the runtime can
        ///move to the next gesture stage if this method is called. Otherwise, or if there is no active gesture,
        ///calling GiveCue() has no ecffect.
        ///Note that for each transition condition, you can set a stage to move to if the condition is met.</summary>
        ///<param name="currentHands">The current hand poses of the user. Will be used in order to create instances of FreeHandAPIEventArgs, 
        ///which will be passed to the event listeners of the events invoked after GiveCue() is called.</param>
        public void GiveCue(Hands currentHands)
        {
            DateTime timeStamp = DateTime.Now;
            if (ActiveGesture < 0)
            {
                return;
            }
            GestureStage currentStage = Gestures[ActiveGesture].GetCurrentStage();
            if (currentStage!=null && currentStage.CheckTransitionCondition(TransitionCondition.Cue))
            {
                DoStageTransition(currentHands, timeStamp, currentStage.GetNextStageIndex(TransitionCondition.Cue));
            }
        }
        ///<summary>Returns the first gesture in the gesture list that has the given name.
        ///If no gesture is found, null is returned.</summary>
        public Gesture GetGesture(string name)
        {
            foreach (Gesture g in Gestures)
            {
                if (g.Name.Equals(name)) return g;
            }
            return null;
        }
        public void StopCurrentGesture()
        {
            if(ActiveGesture!=-1) StopGesture(new Hands(), DateTime.Now);
        }
        private void Initialize()
        {
            Gestures = new List<Gesture>();
            RuntimeIsActive = false;
            TimeOfLastUpdate = DateTime.MinValue;
            ActiveGesture = -1;
        }
        private void ActivateGesture(int index, Hands currentHands, DateTime timeStamp) //called after a gesture is recognized
        {
            ActiveGesture = index;
            Gestures[index].CurrentStage = 0;
            Gestures[index].GestureRecognitionTime = timeStamp;
            Gestures[index].Stages[0].StageRecognitionTime = timeStamp;
            Gesture gestureToBeActivated = Gestures[index];
            
            if(gestureToBeActivated.Stages[0].AutoSetManipulationHand)
            {
                gestureToBeActivated.Stages[0].ManipulationHand = gestureToBeActivated.Stages[0].LastHandRecognized;
            }

            RaiseEvent(GestureEventTypes.Recognized, currentHands, timeStamp);

            if (gestureToBeActivated.Stages[0].DwellTime <= 0 && GetActiveGesture()==gestureToBeActivated)
            {
                StartStage(currentHands, timeStamp);
            }
        }
        private void StartStage(Hands currentHands, DateTime timeStamp) //called when the dwell time of a recognized gesture is over or dwell time is 0
        {
            //Set Hand Positions at time this stage starts in all possible target stages
            int transitionConditionCount = Enum.GetValues(typeof(TransitionCondition)).Length;
            GestureStage currentStage = Gestures[ActiveGesture].GetCurrentStage();
            for (int i=0; i<transitionConditionCount; i++)
            {
                int targetStage = currentStage.GetNextStageIndex((TransitionCondition)i);
                if (targetStage>=0 && targetStage<Gestures[ActiveGesture].Stages.Count)
                {
                    Gestures[ActiveGesture].Stages[targetStage].LeftPosWhenPrevStageStarted=Gestures[ActiveGesture].GetCurrentStage().GetHandPosition(currentHands.Left,true);
                    Gestures[ActiveGesture].Stages[targetStage].RightPosWhenPrevStageStarted=Gestures[ActiveGesture].GetCurrentStage().GetHandPosition(currentHands.Right,false);
                }
            }

            RecordPath(currentHands, timeStamp);
            
            //Raise start event
            RaiseEvent(GestureEventTypes.Start, currentHands, timeStamp);
        }
        private void DoStageTransition(Hands currentHands, DateTime timeStamp, int targetStage) //called when in an active gesture stage a transition condition is met
        {
            if (targetStage<0 || targetStage>=Gestures[ActiveGesture].Stages.Count)
            {
                //target stage does not exist
                RaiseEvent(GestureEventTypes.StageTransition, currentHands,timeStamp);
                StopGesture(currentHands, timeStamp);
            }
            else //target stage does exist
            {
                RaiseEvent(GestureEventTypes.StageTransition, currentHands,timeStamp);//invoke stage transition event on the "from" stage not the "to" stage
                Gestures[ActiveGesture].CurrentStage = targetStage;
                GestureStage currentStage=Gestures[ActiveGesture].GetCurrentStage();
                currentStage.StageRecognitionTime = timeStamp;
                RaiseEvent(GestureEventTypes.Recognized, currentHands, timeStamp);//invoke recognized event on the "to" stage
                if(Gestures[ActiveGesture].GetCurrentStage().DwellTime<=0) StartStage(currentHands, timeStamp);//start event on target stage if no dwell time

                if (currentStage.AutoSetManipulationHand)
                {
                    currentStage.ManipulationHand = currentStage.LastHandRecognized;
                }

            }
        }
        private void StopGesture(Hands currentHands, DateTime timeStamp) //called when during a stage transition no target stage is defined, or if a stage is aborted during dwell time
        {
            var args = CreateFreeHandAPIEventArgs(currentHands, timeStamp, GestureEventTypes.Released);
            Gestures[ActiveGesture].CurrentStage = -1;
            Gestures[ActiveGesture]._path = new Path3D(Gestures[ActiveGesture]._path.TemporalResolution);
            ActiveGesture = -1;
            RaiseEvent(GestureEventTypes.Released, args);
        }
        private void RecordPath(Hands currentHands, DateTime timeStamp)
        {
            GestureStage s = GetActiveGesture()?.GetCurrentStage();
            if (s==null || s.RecordPath==false) return;//no active stage or path should not be recorded
            int millisSinceStageRecognized = GetActiveGesture().GetMillisSinceStageRecognized();
            if (millisSinceStageRecognized<s.DwellTime) return;//dwell time not over yet, record only during hold phase
            
            //Define the hand position to record according to ManipulationHand
            Position3D handPosition;
            switch(s.ManipulationHand)
            {
                case RelevantHand.Left: handPosition = s.GetHandPosition(currentHands.Left, true); break;
                case RelevantHand.Right: handPosition = s.GetHandPosition(currentHands.Right); break;
                case RelevantHand.Both: 
                    if (currentHands.Left==null || currentHands.Right == null) handPosition = null;
                    else handPosition = (1/2)*(s.GetHandPosition(currentHands.Left, true)+s.GetHandPosition(currentHands.Right)); 
                    break;
                case RelevantHand.Any: 
                    if (currentHands.Left!= null) handPosition = s.GetHandPosition(currentHands.Left, true);
                    else if (currentHands.Right!=null) handPosition = s.GetHandPosition(currentHands.Right);
                    else handPosition = null;
                    break;
                default: handPosition = null; break;
            }

            GetActiveGesture()._path.AddNode(handPosition, timeStamp);
        }
    
        private FreeHandEventArgs CreateFreeHandAPIEventArgs(Hands currentHands, DateTime timeStamp, GestureEventTypes type)
        {
            FreeHandEventArgs f = new FreeHandEventArgs();
            Gesture g = Gestures[ActiveGesture];
            GestureStage s = g.GetCurrentStage();

            f.EventType = type;
            f.GestureIndex = ActiveGesture;
            f.StageIndex = g.GetCurrentStageIndex();
            f.DwellTime = s.DwellTime;
            f.GestureName = g.Name;
            f.LeftHandPosition = s.GetHandPosition(currentHands.Left, true);
            f.RightHandPosition = s.GetHandPosition(currentHands.Right, false);
            f.TimeSinceStageRecognized = g.GetMillisSinceStageRecognized(timeStamp);
            f.TimeSinceGestureRecognized = g.GetMillisSinceGestureRecognized(timeStamp);

            if (f.LeftHandPosition!=null && f.RightHandPosition!=null) f.DistanceBetweenHands = f.LeftHandPosition.Distance(f.RightHandPosition);
            else f.DistanceBetweenHands=float.MaxValue;

            f.Path = g.Path;

            return f;
        }

        private void RaiseEvent(GestureEventTypes type, Hands currentHands, DateTime timeStamp)
        {
            RaiseEvent (type, CreateFreeHandAPIEventArgs(currentHands, timeStamp, type));
        }
        private void RaiseEvent(GestureEventTypes type, FreeHandEventArgs args)
        {
            //Invoke events of FreeHandRuntime class
            switch (type)
            {
                case GestureEventTypes.Recognized: RaiseGestureRecognizedEvent?.Invoke(this, args); break;
                case GestureEventTypes.Dwelling: RaiseGestureDwellingEvent?.Invoke(this, args); break;
                case GestureEventTypes.Start: RaiseGestureStartEvent?.Invoke(this, args); break;
                case GestureEventTypes.Holding: RaiseGestureHoldingEvent?.Invoke(this, args); break;
                case GestureEventTypes.StageTransition: RaiseGestureStageTransitionEvent?.Invoke(this, args); break;
                case GestureEventTypes.Released: RaiseGestureReleasedEvent?.Invoke(this, args); break;
            }
            //Let the active stage raise their events too
            if (args.GestureIndex>=0 && args.GestureIndex<Gestures.Count && args.StageIndex>=0 && args.StageIndex < Gestures[args.GestureIndex].StageCount)
                Gestures[args.GestureIndex].Stages[args.StageIndex].RaiseEvent(type, args);
        }
        private class Nested
        {
            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            // (see https://csharpindepth.com/articles/BeforeFieldInit)
            static Nested()
            {
            }

            internal static readonly FreeHandRuntime instance = new FreeHandRuntime();
        }  

    }

}