using System.Collections.Generic;
using System.Xml.Serialization;
using FreeHandGestureFramework;
using FreeHandGestureFramework.DataTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    ///<summary>In this class, some Unity specific values are used instead of
    ///Free Hand Gesture Framework specific values in FreeHandGestureFramework.GestureHandler.</summary>

    public sealed class GestureHandlerU
    {
        private GestureHandler GH = null;
        private GestureHandlerU() {}
        ///<summary>Returns the only instance of GestureHandlerU. If the platform is not yet set by calling 
        ///SetPlatform(), null is returned.</summary>
        public static GestureHandlerU Instance 
        {
            get 
            { 
                 if (GestureHandler.Instance == null) return null;
                 else return Nested.instance; 
            } 
        }
        ///<summary>An array containing all predefined gestures for the chosen platform.</summary>
        public static GestureU[] PredefinedGestures  {get; private set;} = new GestureU[0];
        ///<summary>Returns the bone count of the chosen platform's hand model.</summary>
        public int BoneCount {get{return GH.HandSkeleton.GetBonesCount();}}
        ///<summary>Returns the bone names of the chosen platform's hand model.</summary>
        public string[] BoneNames {get {return GH.HandSkeleton.GetBoneNames();}}
        ///<summary>Returns the default bone weights of the chosen platform's hand model.</summary>
        public float[] DefaultBoneWeights {get {return GH.HandSkeleton.GetDefaultBoneWeights();}} 
        ///<summary>The array of gestures GestureHandlerU will try to recognize while active</summary>
        public GestureU[] Gestures
        {
            get
            {
                if (GH.Gestures == null) return null;
                var array = new GestureU[GH.Gestures.Count];
                for (int i = 0; i<array.Length; i++) array[i] = (GestureU)GH.Gestures[i];                
                return array;
            }
            set
            {
                var list = new List<Gesture>();
                if (value != null) foreach(var entry in value) list.Add((Gesture)entry);
                GH.StopCurrentGesture();
                GH.SetActive(false);
                GH.Gestures = list;
                GH.SetActive(true);
            }
        }
        ///<summary>If a gesture is active, ActiveGesture returns the index of the active gesture which can be
        ///used with the gesture list. If no gesture is active, -1 is returned.</summary>
        public int ActiveGesture {get{return GH.ActiveGesture;}}
        ///<summary>Sets the (hardware) platform for this instance of GestureHandlerU.
        ///This method has to be called once before Instance() will return the GestureHandlerU instance.</summary> 
        ///<param name="platform">The platform from the list of supported platforms.</param>
        ///<param name="customHandSkeleton">If you want to use a custom platform, the hand skeleton model must be handed over here. Otherwise, this value will be ignored.</param>
        public static void SetPlatform(Platforms platform, HandSkeletonBase customHandSkeleton = null)
        {
            GestureHandler.SetPlatform(platform, customHandSkeleton);
            Instance.GH = GestureHandler.Instance;
            
            //now the platform is set, the predefined gestures for the specific platform can be loaded
            TextAsset[] xmls = Resources.LoadAll<TextAsset>("PredefinedGestures/"+Instance.GH.Platform.ToString()); //e.g. "PredefinedGestures/OculusQuest"
            PredefinedGestures = new GestureU[xmls.Length];
            XmlSerializer serializer = new XmlSerializer(typeof(GestureU));
            for (int i=0; i<xmls.Length; i++)
            {
                using(var reader = new System.IO.StringReader(xmls[i].text))
                {
                    PredefinedGestures[i] = (GestureU)serializer.Deserialize(reader);
                }
            }
        }
        ///<summary>Returns the predefined gesture identifierd by its name, or null if no such gesture exists.</summary>
        ///<param name="name">The name of the gesture as defined in GestureU.Name.</param>
        public static GestureU GetPredefinedGesture(string name)
        {
            foreach(GestureU g in PredefinedGestures) if (g.Name == name) return new GestureU(g);
            return null;
        }
        ///<summary>Adds a gesture to the gesture list.</summary>
        ///<param name="gesture">The gesture to add</param>
        public void AddGesture(GestureU gesture)
        {
            if (GH.Gestures == null) return;
            else GH.Gestures.Add(gesture);
        }
        ///<summary>Removes a gesture from the gesture list.</summary>
        ///<param name="gesture">The gesture to remove</param>
        public void RemoveGesture(GestureU gesture)
        {
            if (GH.Gestures == null) return;
            else GH.Gestures.Remove(gesture);
        }
        ///<summary>Returns true, if the runtime is active and false otherwise. When the runtime
        ///is not active, no gestures will be recognized.</summary>
        public bool IsActive () {return GH.IsActive();}
        ///<summary>Set the runtime active or inactive. When the runtime
        ///is not active, no gestures will be recognized.</summary>
        public void SetActive(bool value) {GH.SetActive(value);}
        ///<summary>If there is an active gesture, that gesture is returned. Otherwise, null is returned.
        ///Note that only one gesture can be active.</summary>
        public GestureU GetActiveGesture()
        {
            return (GestureU)GH.GetActiveGesture();
        }
        ///<summary>Returns the first gesture in the gesture list that has the given name.
        ///If no gesture is found, null is returned.</summary>
        public GestureU GetGesture(string name)
        {
            return (GestureU)GH.GetGesture(name);
        }
        ///<summary>Returns the standard deviation of the chosen platform'shand model.</summary>
        public Deviation GetStandardDeviation() {return GH.HandSkeleton.GetStandardDeviation();}
        ///<summary>Returns the standard deviations of the chosen platform'shand model.</summary>
        public Deviation[] GetDefaultDeviations() {return GH.HandSkeleton.GetDefaultDeviations();}
        ///<summary>In the Update method, the runtime will try to recognize a gesture from the gesture list
        ///or, if a gesture is already active, to recognize a transition to the next stage,
        ///using the current right and left hand data, which must be provided.</summary>
        ///<param name="currentHands">The current poses of left and right hand which the user holds at that moment</param>
        public void Update(HandsU currentHands) {GH.Update(currentHands);}
        ///<summary>In the Update method, the runtime will try to recognize a gesture from the gesture list
        ///or, if a gesture is already active, to recognize a transition to the next stage,
        ///using the current right and left hand data, which must be provided.</summary>
        ///<param name="currentHands">The current poses of left and right hand which the user holds at that moment</param>
        ///<param name="lookingDirection">In some cases, the looking direction is important for the recognition of
        ///the next gesture stage (e.g. if moving to the next stage involves the movement of the hand in a given
        ///direction relative to the looking direction).</param>
        public void Update(HandsU currentHands, Vector3 lookingDirection) {GH.Update(currentHands, ConversionTools.Vector3ToPosition3D(lookingDirection));}
        ///<summary>If the active gesture stage has the Cue value set in TransitionConditions, the runtime can
        ///move to the next gesture stage if this method is called. Otherwise, or if there is no active gesture,
        ///calling GiveCue() has no ecffect.
        ///Note that for each transition condition, you can set a stage to move to if the condition is met.</summary>
        ///<param name="currentHands">The current hand poses of the user. Will be used in order to create instances of FreeHandAPIEventArgs, 
        ///which will be passed to the event listeners of the events invoked after GiveCue() is called.</param>
        public void GiveCue(HandsU currentHands) {GH.GiveCue(currentHands);}
        ///<summary>This method subscribes a listener method of type FreeHandGestureEventHandler to an
        ///event specified by type. The subscribed method will then be called if ANY of the gesture
        ///stages raises that event. E.g. if you add a listener method and set type to Recognized,
        ///the listener will be called every time a gesture stage is recognized.
        ///The listener will receive an instance of FreeHandAPIEventArgs, which contains all relevant data
        ///(e.g. gesture and stage index).</summary>
        ///<param name="type">The type of the event.</param>
        ///<param name="listener">The callback routine which will be called when the specified event is raised.</param>
        public void AddEventListener(GestureEventTypes type, Delegates.FreeHandGestureEventHandler listener)
        {
            GH.AddEventListener(type, listener);
        }
        ///<summary>This method subscribes a listener method of type FreeHandGestureEventHandler to an
        ///event specified by type. The subscribed method will then be called if the specified gesture stage
        ///raises that event.</summary>
        ///<param name="type">The type of the event.</param>
        ///<param name="listener">The callback routine which will be called when the specified event is raised.</param>
        ///<param name="gestureIndex">The index of the gesture in the gesture list that contains the gesture stage to which the listerer will be added.</param>
        ///<param name="stageIndex">The index of the gesture stage to which the listener will be added.</param>
        public void AddEventListener (GestureEventTypes type, Delegates.FreeHandGestureEventHandler listener, int gestureIndex, int stageIndex)
        {
            GH.AddEventListener(type, listener, gestureIndex, stageIndex);
        }
        ///<summary>Sets the currently performed gesture - if there is one - inactive.</summary>
        public void StopCurrentGesture(){GH.StopCurrentGesture();}
        private class Nested
        {
            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            // (see https://csharpindepth.com/articles/BeforeFieldInit)
            static Nested()
            {
            }

            internal static readonly GestureHandlerU instance = new GestureHandlerU();
        }  

    }
}