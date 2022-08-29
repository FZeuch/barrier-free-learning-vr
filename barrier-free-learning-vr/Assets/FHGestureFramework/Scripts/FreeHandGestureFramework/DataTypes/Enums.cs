namespace FreeHandGestureFramework.DataTypes
{
    ///<summary>In the Platforms enum, all hardware platforms that are supported by the Free Hand Gesture Runtime 
    ///are listed. In order to use the runtime, you have to initialize it to a specific
    ///platform using the GestureHandler.SetPlatform method. If you want to use a custom platform
    ///please provide a custom HandSkeleton to the SetPlatform method.</summary>
    public enum Platforms
    {
        OculusQuest,
        HoloLens2,
        ViveFocus,
        Custom
    }
    ///<summary>The TransitionCondition enum is used in GestureStage to define on which condition a
    ///stage transition will occur.</summary>
    public enum TransitionCondition 
    {
        ///<value>Perform a stage transition when GestureHandler.GiveCue() method is called.</value> 
        Cue = 0,
        ///<value>Perform a stage transition when the next pose is recognized.</value>
        NextPoseRecognized = 1,
        ///<value>Perform a stage transition when the pose of the current stage is released.</value>
        Release = 2,
        ///<value>Perform a stage transition instantly when the current stage starts.</value>
        Start = 3
    } 

    ///<summary>The RelevantHand enum is used to define which hand(s) is/are of importance in a 
    ///certain context.</summary>
    public enum RelevantHand
    {
        Left,
        Right,
        Any,
        Both
    }

    public enum DirectionLeftHandedAxes
    {
        NegativeX = 0,
        PositiveX = 1,
        NegativeY = 2,
        PositiveY = 3,
        NegativeZ = 4,
        PositiveZ = 5
    }
    public enum DirectionLeftHanded
    {
        Left = 0,
        Right = 1,
        Down = 2,
        Up = 3,
        Back = 4,
        Forward = 5
    }
    public enum DirectionRPYLeftHanded
    {
        PositivePitch = 0,
        NegativePitch = 1,       //negative pitch (lowering the "nose" of an aeroplane) is a positive rotation around the y axis in a left handed coordinate system
        NegativeYaw = 2,       
        PositiveYaw = 3,
        PositiveRoll = 4,
        NegativeRoll = 5       //negative roll (lifting the right wing of an aeroplane) is a positive rotation around the x axis in a lefthanded coordinate system
    }
}