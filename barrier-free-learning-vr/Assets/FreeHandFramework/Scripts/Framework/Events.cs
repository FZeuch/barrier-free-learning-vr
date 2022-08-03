using System;

namespace FreeHandGestureFramework.EnumsAndTypes
{
    //This file contains event-related classes using the C# event system.

    public static class Delegates 
    {
        ///<value>This delegate must be used for callback routines in the context of FreeHand Gesture Runtime Events (see GestureEventTypes)</value>
        public delegate void FreeHandGestureEventHandler(object sender, FreeHandEventArgs args);

    }
    ///<summary>The GestureEventTypes enum contains all types of events that can be invoked by the FreeHand Gesture Runtime.</summary>
    public enum GestureEventTypes
    {
        ///<value>The Recognized event is invoked, when a gesture stage is recognized. Note that, due to dwelling, the stage might not have started yet.</value>
        Recognized,
        ///<value>The Dwelling event is invoked during every update, if a gesture stage was recognized, but dwell time is not over yet.</value>
        Dwelling,
        ///<value>The Start event is invoked when a gesture stage starts.</value>
        Start,
        ///<value>The Holding event is invoked during every update, after a gesture stage has started.</value>
        Holding,
        ///<value>The StageTransition event is invoked, if a stage transition occurs. Note that the event is invoked on the "from" stage, not the destination stage.</value>
        StageTransition,
        ///<value>The Released event is invoked, if the hand pose of a gesture stage was released.</value>
        Released
    }
    ///<summary>This class is passed as an argument to callback routines 
    ///when the runtime raises an event.</summary>
    public class FreeHandEventArgs : EventArgs
    {
        ///<value>The name of the gesture</value>
        public string GestureName;
        ///<value>The type of the event that was invoked</value>
        public GestureEventTypes EventType;
        ///<value>The index of the gesture that raised the event inside the FreeHandRuntime.Gestures list.</value>
        public int GestureIndex;
        ///<value>The index of the stage that raised the event.</value>
        public int StageIndex;
        ///<value>The time in milliseconds that has passed since the active stage was recognized.</value>
        public int TimeSinceStageRecognized;
        ///<value>The time in milliseconds that has passed since the first stage of the active gesture was recognized.</value>
        public int TimeSinceGestureRecognized;
        ///<value>The dwell time of the stage that raised the event.</value>
        public int DwellTime;
        ///<value>If both hands were tracked, this value contains the distance between the left and the right manipulation bone. It contains the maximum float value otherwise.</value>
        public float DistanceBetweenHands;
        ///<value>If the right hand was tracked, this value contains the world space coordinates of the right hand, or null if the hand was untracked.</value>
        public Position3D RightHandPosition;
        ///<value>If the left hand was tracked, this value contains the world space coordinates of the left hand, or null if the hand was untracked.</value>
        public Position3D LeftHandPosition;
        ///<value>The path of the gesture as recorded so far. If path recoeding is turned off, the field contains an empty Path3D.</value>
        public Path3D Path;
    }
}