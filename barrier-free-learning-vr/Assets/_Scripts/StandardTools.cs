using System;

namespace StandardTools
{
    public static class MathTools
    {
        public static float Clamp01(float value)
        {
            if (value<0) return 0;
            else if (value>1) return 1;
            else return value;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value<min) return min;
            else if (value>max) return max;
            else return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value<min) return min;
            else if (value>max) return max;
            else return value;
        }
        public static float Positive(float value)
        {
            if (value <0) return 0;
            else return value;
        }
        public static int Positive(int value)
        {
            if (value <0) return 0;
            else return value;
        }
        public static float ToRadian(float degree)
        {
            return (float)((degree/180)*Math.PI);
        }
        public static float ToDegree(float radian)
        {
            return (float)((radian/Math.PI)*180);
        }
        ///<summary>Returns an Euler angle value between 0 and 360 that describes the angle between (0,1) and (x,y).
        ///So a vector pointing "up" will return 0, a vector pointing "left" will return 90 etc.
        ///Note that (x,y) must be a normalized vector (a vector of lenght 1)</summary>
        ///<param name="x">The x value of the normalized vector.</param>
        ///<param name="y">The y value of the normalized vector.</param>
        public static float GetCounterClockwiseAngle(float x, float y)
        {
            float degrees = ToDegree((float)(Math.Atan2(y,x)-(.5*Math.PI)));
            if(degrees<0) degrees += 360;
            return degrees;
        }
        
        ///<summary>Returns val % mod. If the result is negative, mod is added, so the return value is always positive.</summary>
        public static float PositiveModValue(float val, float mod)
        {
            float returnval = val % mod;
            if(returnval<0) returnval+=mod;
            return returnval;
        }
    }

    public static class TimeTools
    {
        public static int GetDifferenceInMilliseconds (DateTime time1, DateTime time2)
        {
            if (time1.CompareTo(time2) < 0) //time1 earlier than time2
            {
                return (int) time2.Subtract(time1).TotalMilliseconds;
            }
            else //time1 > time2
            {
                return (int) time1.Subtract(time2).TotalMilliseconds;
            }
            
        }
    }
}