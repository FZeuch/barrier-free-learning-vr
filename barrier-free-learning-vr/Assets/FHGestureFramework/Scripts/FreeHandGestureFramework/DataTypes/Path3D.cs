using StandardTools;
using System;

namespace FreeHandGestureFramework.DataTypes
{
    ///<summary>The Path3D class is used to store a three dimensional path of
    ///positions and their time stamps.</summary>
    public class Path3D
    {
        ///<value>This value is used to initialize TemporalResolution, if the default constructor is used.</value>
        public const int DEFAULT_TEMPORAL_RESOLUTION = 25;
        private const int ARRAY_GROWTH = 40; 
        ///<value>Defines the temporal resolution of the path in milliseconds. The AddNode method only adds a node, if the time
        ///stamp of the previous added node lies that many or more milliseconds in the past. This is not true for the very first
        ///node.</value>
        public int TemporalResolution;
        ///<value>A self-growing array of three dimensional positions. Use the AddNode method to add a new position.</value>
        public Position3D[] Path;
        ///<value>The time stamps linked to the positions in Path. The values are the milliseconds passed since
        ///the adding of the first position with the AddNode method.</value>
        public int[] TimeStamps;
        ///<value>The index of the last added position. -1 if no position was added yet.</value>
        public int HighestIndex = -1;

        public Path3D(): this(DEFAULT_TEMPORAL_RESOLUTION) {}
        public Path3D(int temporalResolution)
        {
            TemporalResolution = System.Math.Abs(temporalResolution);
            Path = new Position3D[ARRAY_GROWTH];
            TimeStamps = new int[ARRAY_GROWTH];
        }
        public Path3D(Path3D original)
        {
            TemporalResolution = original.TemporalResolution;
            Path = new Position3D[original.Path.Length];
            TimeStamps = new int[Path.Length];
            for (int i = 0; i < Path.Length; i++)
            {
                Path[i] = original.Path[i];
                TimeStamps[i] = original.TimeStamps[i];
            }
            HighestIndex = original.HighestIndex;
        }
        ///<summary>The time the first position was added.</summary>
        public DateTime BeginTime {get; private set;}
        ///<summary>Add a position/time stamp tuple to the path. Automatically grows the Path and TimeStamps arrays if necesssary.</summary>
        ///<param name="position">The three dimensional position to add.</param>
        ///<param name="timeStamp">The time stamp corresponding with the position to add.</param>
        public void AddNode(Position3D position, DateTime timeStamp)
        {
            if (position == null) return;

            int millisSinceBegin = TimeTools.GetDifferenceInMilliseconds(timeStamp, BeginTime);
            if (HighestIndex == -1)
            { //This is the first entry
                BeginTime = timeStamp;  
                millisSinceBegin = 0;              
            }
            else if (millisSinceBegin - TimeStamps[HighestIndex] < TemporalResolution)
            {   //Ignore request to add node if last entry was less than TemporalResolution ms before
                return;
            }
            
            if (HighestIndex == Path.Length-1)
            { //we need to enlarge the array
                Array.Resize<Position3D>(ref Path, Path.Length + ARRAY_GROWTH);
                Array.Resize<int>(ref TimeStamps, TimeStamps.Length + ARRAY_GROWTH);
            }

            HighestIndex++;
            Path[HighestIndex] = position;
            TimeStamps[HighestIndex] = millisSinceBegin;
        }
    }
}