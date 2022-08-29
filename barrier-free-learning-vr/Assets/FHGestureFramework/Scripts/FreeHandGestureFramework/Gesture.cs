using System.Collections.Generic;
using StandardTools;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FreeHandGestureFramework.DataTypes;

namespace FreeHandGestureFramework
{
    public class Gesture
    {
        ///<value>The name of the gesture</value>
        public string Name;
        ///<value>The list of gesture stages. Every gesture starts with stage 0. To get to the next gesture
        ///stage, or to stop the gesture, every stage has one or more transition conditions. Each of them
        ///defines to which stage to go (or if to exit the gesture at all) in case that specific condition is met.</value>
        public List<GestureStage> Stages = new List<GestureStage>();
        internal int CurrentStage = -1;//-1 if gesture is not active

        public Gesture(): this("") {}

        public Gesture(string name)
        {
            Name = name;
        }
        public Gesture(Gesture orig)
        {
            Name=orig.Name;
            foreach(GestureStage s in orig.Stages) Stages.Add(new GestureStage(s));
            _path.TemporalResolution = orig._path.TemporalResolution;
        }
        ///<summary>Returns the number of stages in the stage list.</summary>
        public int StageCount 
        {
            get {return Stages.Count;}
        }
        internal Path3D _path = new Path3D();//Can contain a recorded path of a hand movement.
        ///<summary>Returns the path of this gesture. Every gesture stage that has RecordPath enabled will record a series
        ///of three dimensional positions following the manipulation hand during the hold phase of the stage.
        ///The return value represents the path as recorded so far. It will not update when further positions are added by the runtime.</summary>
        [XmlIgnore]
        public Path3D Path 
        {
            get {return new Path3D(_path);} //return copy because _path still might change
        }
        ///<summary>The time of the last recognition of the gesture.</summary>
        [XmlIgnore]
        public DateTime GestureRecognitionTime {get; internal set;} = DateTime.MinValue;
        ///<summary>Deserializes a complete gesture from an XML file.</summary>
        ///<param name="filename">The filename of the xml file.</param>
        public static T CreateFromXML<T>(string filename) where T:Gesture
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T gesture;
            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                gesture = (T)serializer.Deserialize(reader);
            }
            return gesture;
        }
        ///<summary>Returns the active stage of this gesture. If the gesture is not
        ///active, null is returned.</summary>
        public GestureStage GetCurrentStage()
        {
            if (CurrentStage == -1) return null;
            else return Stages[CurrentStage];
        }
        ///<summary>If this gesture is active, GetCurrentStageIndex returns the index of the active stage which
        ///can be used in the stage list. If the gesture is inactive, null is returned.</summary>
        public int GetCurrentStageIndex() {return CurrentStage;}
        ///<summary>Sets the path's temporal resolution in milliseconds. So, if this gesture is active, and the 
        ///current stage has its RecordPath field set to true, a path of three dimensional positions will
        ///be recorded following the manipulation hand(s). A new position will only be recorded if the 
        ///given amount of milliseconds has passed since the last recorded position.</summary>
        //TODO:deefine exact behaviour of path recording
        public void SetPathTemporalResolution(int milliseconds) 
        {
            _path.TemporalResolution = MathTools.Positive(milliseconds);
        }
        ///<summary>Returns the path's temporal resolution in milliseconds. So, if this gesture is active, and the 
        ///current stage has its RecordPath field set to true, a path of three dimensional positions will
        ///be recorded following the manipulation hand(s). A new position will only be recorded if the 
        ///amount of milliseconds has passed since the last recorded position.</summary>
        public int GetPathTemporalResolution() {return _path.TemporalResolution;}
        ///<summary>If this gesture is active, the confidence value of the stage following the active stage
        ///if the NextPoseRecognized transition condition is met will be returned. If NextPoseRecognized
        ///is not set as a transition condition or the target stage is out of range, 0 will be returned.
        ///If the gesture is not active, the confidence value of the first stage Stages[0] is returned.</summary>
        ///<param name="currentHands">The current poses of left and right hand which the user holds at that moment</param>
        ///<param name="lookingDirection">In some cases, the looking direction is important for the recognition of
        ///the next gesture stage (e.g. if moving to the next stage involves the movement of the hand in a given
        ///direction relative to the looking direction). The looking direction vector might for example be the
        ///forward direction vector of the (left eye of the) camera.</param>
        public float Confidence(Hands currentHands, Position3D lookingDirection) //confidence of next stage
        {
            if(CurrentStage<0) return Stages[0].Confidence(currentHands, lookingDirection);
            int targetStage=Stages[CurrentStage].GetNextStageIndex(TransitionCondition.NextPoseRecognized);
            if(Stages[CurrentStage].CheckTransitionCondition(TransitionCondition.NextPoseRecognized) 
                && targetStage>=0 && targetStage<Stages.Count) return Stages[targetStage].Confidence(currentHands,lookingDirection);
            return 0;
        }
        ///<summary>Returns the time in milliseconds since this gesture started.
        ///If the gesture is not active, -1 is returned.</summary>
        public int GetMillisSinceGestureRecognized() {return GetMillisSinceGestureRecognized(DateTime.Now);}
        ///<summary>Returns the time in milliseconds from the time this gesture started until the specified timeStamp.
        ///If the gesture is not active, -1 is returned.</summary>
        public int GetMillisSinceGestureRecognized(DateTime timeStamp)
        {
            if (CurrentStage < 0) return -1; //gesture not active
            else return TimeTools.GetDifferenceInMilliseconds(GestureRecognitionTime, timeStamp);
        }
        ///<summary>Returns the time in milliseconds since the active stage started.
        ///If the gesture is not active, -1 is returned.</summary>
        public int GetMillisSinceStageRecognized() {return GetMillisSinceStageRecognized(DateTime.Now);}
        ///<summary>Returns the time in milliseconds from the time the active stage started until the time specified in timeStamp.
        ///If the gesture is not active, -1 is returned.</summary>
        public int GetMillisSinceStageRecognized(DateTime timeStamp)
        {
            if (CurrentStage < 0) return -1; //gesture not active
            else return TimeTools.GetDifferenceInMilliseconds(Stages[CurrentStage].StageRecognitionTime, timeStamp);
        }
        ///<summary>Serializes a complete gesture to an XML file.</summary>
        ///<param name="filename">The file name of the XML file to be created.</param>
        public void SerializeXML(string filename)
        {
            Stream fs = new FileStream(filename, FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
            serializer.Serialize(writer, this);
            writer.Close();
        }
        


    }
}