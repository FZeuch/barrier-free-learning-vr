using System.Numerics;
using System;
using StandardTools;

namespace FreeHandGestureFramework.EnumsAndTypes
{
    public class RotationQuat
    {
        public float X;
        public float Y;
        public float Z;
        public float W;
        public RotationQuat() : this(0, 0, 0, 1 /*Quaternion identity*/) {}
        public RotationQuat(Quaternion q) : this(q.X, q.Y, q.Z, q.W) {}
        public RotationQuat(Position3D eulerAngles) : this(eulerAngles.X, eulerAngles.Y, eulerAngles.Z) {}
        public RotationQuat(float eulerX, float eulerY, float eulerZ)
        {
            eulerX=MathTools.ToRadian(eulerX);
            eulerY=MathTools.ToRadian(eulerY);
            eulerZ=MathTools.ToRadian(eulerZ);

            double cx = Math.Cos(eulerX * 0.5);
            double sx = Math.Sin(eulerX * 0.5);
            double cy = Math.Cos(eulerY * 0.5);
            double sy = Math.Sin(eulerY * 0.5);
            double cz = Math.Cos(eulerZ * 0.5);
            double sz = Math.Sin(eulerZ * 0.5);

            Quaternion qx = new Quaternion((float)sx,0,0,(float)cx);
            Quaternion qy = new Quaternion(0,(float)sy,0,(float)cy);
            Quaternion qz = new Quaternion(0,0,(float)sz,(float)cz);
            Quaternion q = qy*qx*qz; //zxy rotation (quaternions multiply from right to left)

            X=q.X;
            Y=q.Y;
            Z=q.Z;
            W=q.W;
        }
        public RotationQuat(float x, float y, float z, float w)
        {
            X=x;
            Y=y;
            Z=z;
            W=w;            
        }
        public static RotationQuat operator *(RotationQuat rq1, RotationQuat rq2)
            => new RotationQuat(new Quaternion(rq1.X, rq1.Y, rq1.Z, rq1.W) * new Quaternion(rq2.X, rq2.Y, rq2.Z, rq2.W));
        ///<summary>Transforms a the rotation of an object 
        ///to a rotation relative to the looking direction.</summary>
        ///<param name="rotationInWorldSpace">A quaternion that contains an object's rotation.
        ///<param name="lookingDirection">The looking direction vector.</param>
        public static RotationQuat GetRotationInLookDir(RotationQuat rotationInWorldSpace, Position3D lookingDirection)
        {
            Position3D lookDirWithoutY=new Position3D(lookingDirection.X,0,lookingDirection.Z).NormalizedVector();
            float lookingDirectionRotationY=MathTools.GetCounterClockwiseAngle(lookDirWithoutY.Z, lookDirWithoutY.X);
            //RotationQuat q1 = new RotationQuat(rotationInWorldSpace);
            RotationQuat rotAroundYInWorldSpace = new RotationQuat(0,-lookingDirectionRotationY,0);
            return rotAroundYInWorldSpace * rotationInWorldSpace;
        }

        public Position3D GetEulerAnglesLikeInUnity()
        {
            Position3D p = new Position3D (
                (float)Math.Asin(2*((W*X)-(Z*Y))),
                (float)Math.Atan2(2*((W*Y)+(X*Z)), 1-(2*(Math.Pow(Y,2)+Math.Pow(X,2)))),
                (float)Math.Atan2(2*((W*Z)+(X*Y)), 1-(2*(Math.Pow(X,2)+Math.Pow(Z,2))))
            );
            return new Position3D(
                MathTools.PositiveModValue(MathTools.ToDegree(p.X),360), 
                MathTools.PositiveModValue(MathTools.ToDegree(p.Y),360), 
                MathTools.PositiveModValue(MathTools.ToDegree(p.Z),360));
        }  
        public Position3D GetEulerAngles() 
        {
            Position3D angles = new Position3D();

            // roll (x-axis rotation)
            double sinr_cosp = 2 * ((W * X) + (Y * Z));
            double cosr_cosp = 1 - 2 * ((X * X) + (Y * Y));
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = 2 * ((W * Y) - (Z * X));
            if (Math.Abs(sinp) >= 1)
                angles.Y = (float)(Math.Sign(sinp)*Math.PI); // use 90 degrees if out of range
            else
                angles.Y = (float)Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny_cosp = 2 * ((W * Z) + (X * Y));
            double cosy_cosp = 1 - 2 * ((Y * Y) + (Z * Z));
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return new Position3D(
                MathTools.PositiveModValue(MathTools.ToDegree(angles.X),360), 
                MathTools.PositiveModValue(MathTools.ToDegree(angles.Y),360), 
                MathTools.PositiveModValue(MathTools.ToDegree(angles.Z),360));
        } 
    }
}