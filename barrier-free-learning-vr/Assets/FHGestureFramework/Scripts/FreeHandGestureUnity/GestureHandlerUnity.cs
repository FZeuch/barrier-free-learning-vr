using System.Collections.Generic;
using System.Xml.Serialization;
using FreeHandGestureFramework;
using FreeHandGestureFramework.DataTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    public sealed class GestureHandlerU
    {
        private GestureHandler GH = null;
        private GestureHandlerU() {}
        public static GestureHandlerU Instance 
        {
            get 
            { 
                 if (GestureHandler.Instance == null) return null;
                 else return Nested.instance; 
            } 
        }
        public static GestureU[] PredefinedGestures  {get; private set;} = new GestureU[0];
        public int BoneCount {get{return GH.HandSkeleton.GetBonesCount();}}
        public string[] BoneNames {get {return GH.HandSkeleton.GetBoneNames();}}
        public float[] DefaultBoneWeights {get {return GH.HandSkeleton.GetDefaultBoneWeights();}} 
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

        public int ActiveGesture {get{return GH.ActiveGesture;}}
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
        public static GestureU GetPredefinedGesture(string name)
        {
            foreach(GestureU g in PredefinedGestures) if (g.Name == name) return new GestureU(g);
            return null;
        }
        public void AddGesture(GestureU gesture)
        {
            if (GH.Gestures == null) return;
            else GH.Gestures.Add(gesture);
        }
        public void RemoveGesture(GestureU gesture)
        {
            if (GH.Gestures == null) return;
            else GH.Gestures.Remove(gesture);
        }
        public bool IsActive () {return GH.IsActive();}
        public void SetActive(bool value) {GH.SetActive(value);}
        public GestureU GetActiveGesture()
        {
            return (GestureU)GH.GetActiveGesture();
        }
        public GestureU GetGesture(string name)
        {
            return (GestureU)GH.GetGesture(name);
        }
        public Deviation GetStandardDeviation() {return GH.HandSkeleton.GetStandardDeviation();}
        public Deviation[] GetDefaultDeviations() {return GH.HandSkeleton.GetDefaultDeviations();}
        public void Update(HandsU currentHands) {GH.Update(currentHands);}
        public void Update(HandsU currentHands, Vector3 lookingDirection) {GH.Update(currentHands, ConversionTools.Vector3ToPosition3D(lookingDirection));}
        public void GiveCue(HandsU currentHands) {GH.GiveCue(currentHands);}
        public void AddEventListener(GestureEventTypes type, Delegates.FreeHandGestureEventHandler listener)
        {
            GH.AddEventListener(type, listener);
        }
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