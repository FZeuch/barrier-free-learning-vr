using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using FreeHandGestureFramework;
using FreeHandGestureFramework.EnumsAndTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    public sealed class FreeHandRuntimeU
    {
        private FreeHandRuntime Fhr = null;
        private FreeHandRuntimeU() {}
        public static FreeHandRuntimeU Instance 
        {
            get 
            { 
                 if (FreeHandRuntime.Instance == null) return null;
                 else return Nested.instance; 
            } 
        }
        public static GestureU[] PredefinedGestures  {get; private set;} = new GestureU[0];
        public int BoneCount {get{return Fhr.HandSkeleton.GetBonesCount();}}
        public string[] BoneNames {get {return Fhr.HandSkeleton.GetBoneNames();}}
        public float[] DefaultBoneWeights {get {return Fhr.HandSkeleton.GetDefaultBoneWeights();}} 
        public GestureU[] Gestures
        {
            get
            {
                if (Fhr.Gestures == null) return null;
                var array = new GestureU[Fhr.Gestures.Count];
                for (int i = 0; i<array.Length; i++) array[i] = (GestureU)Fhr.Gestures[i];                
                return array;
            }
            set
            {
                var list = new List<Gesture>();
                if (value != null) foreach(var entry in value) list.Add((Gesture)entry);
                Fhr.StopCurrentGesture();
                Fhr.SetActive(false);
                Fhr.Gestures = list;
                Fhr.SetActive(true);
            }
        }

        public int ActiveGesture {get{return Fhr.ActiveGesture;}}
        public static void SetPlatform(Platforms platform, HandSkeletonBase customHandSkeleton = null)
        {
            FreeHandRuntime.SetPlatform(platform, customHandSkeleton);
            Instance.Fhr = FreeHandRuntime.Instance;
            
            //now the platform is set, the predefined gestures for the specific platform can be loaded
            TextAsset[] xmls = Resources.LoadAll<TextAsset>("PredefinedGestures/"+Instance.Fhr.Platform.ToString()); //e.g. "PredefinedGestures/OculusQuest"
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
            if (Fhr.Gestures == null) return;
            else Fhr.Gestures.Add(gesture);
        }
        public void RemoveGesture(GestureU gesture)
        {
            if (Fhr.Gestures == null) return;
            else Fhr.Gestures.Remove(gesture);
        }
        public bool IsActive () {return Fhr.IsActive();}
        public void SetActive(bool value) {Fhr.SetActive(value);}
        public GestureU GetActiveGesture()
        {
            return (GestureU)Fhr.GetActiveGesture();
        }
        public GestureU GetGesture(string name)
        {
            return (GestureU)Fhr.GetGesture(name);
        }
        public Deviation GetStandardDeviation() {return Fhr.HandSkeleton.GetStandardDeviation();}
        public Deviation[] GetDefaultDeviations() {return Fhr.HandSkeleton.GetDefaultDeviations();}
        public void Update(HandsU currentHands) {Fhr.Update(currentHands);}
        public void Update(HandsU currentHands, Vector3 lookingDirection) {Fhr.Update(currentHands, ConversionTools.Vector3ToPosition3D(lookingDirection));}
        public void GiveCue(HandsU currentHands) {Fhr.GiveCue(currentHands);}
        public void AddEventListener(GestureEventTypes type, Delegates.FreeHandGestureEventHandler listener)
        {
            Fhr.AddEventListener(type, listener);
        }
        public void StopCurrentGesture(){Fhr.StopCurrentGesture();}
        private class Nested
        {
            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            // (see https://csharpindepth.com/articles/BeforeFieldInit)
            static Nested()
            {
            }

            internal static readonly FreeHandRuntimeU instance = new FreeHandRuntimeU();
        }  

    }
}