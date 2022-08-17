using StandardTools;
using System;

namespace FreeHandGestureFramework.EnumsAndTypes
{
    public class Path3D
    {
        public const int DEFAULT_TEMPORAL_RESOLUTION = 25;
        private const int ARRAY_GROWTH = 40; 
        public int TemporalResolution; //in milliseconds
        public Position3D[] Path;
        public int[] TimeStamps;
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

        public DateTime BeginTime {get; private set;}
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