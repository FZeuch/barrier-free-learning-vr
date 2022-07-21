using System;
using StandardTools;

namespace FreeHandGestureFramework.EnumsAndTypes
{
    public class Position3D
    {
        public float X;
        public float Y;
        public float Z;
        public Position3D(): this(0.0f, 0.0f, 0.0f) {}
        public Position3D(Position3D orig): this(orig.X, orig.Y, orig.Z){}
        public Position3D(float x, float y, float z)
        {
            X=x;
            Y=y;
            Z=z;
        }
        ///<summary>Vector negation</summary>
        public static Position3D operator -(Position3D u) => new Position3D(-u.X, -u.Y, -u.Z);
        ///<summary>Vector additon</summary>
        public static Position3D operator +(Position3D u, Position3D v)
            => new Position3D(u.X+v.X, u.Y+v.Y, u.Z+v.Z);
        ///<summary>Vector substraction</summary>
        public static Position3D operator -(Position3D u, Position3D v)
            => new Position3D(u.X-v.X, u.Y-v.Y, u.Z-v.Z);
        ///<summary>Scalar multipilication</summary>
        public static Position3D operator *(float r, Position3D u)
            => new Position3D(r*u.X, r*u.Y, r*u.Z);
        ///<summary>Scalar product</summary>
        public static float operator *(Position3D u, Position3D v)
            => (u.X*v.X)+(u.Y*v.Y)+(u.Z*v.Z);        
        ///<summary>Cross product</summary>
        public static Position3D CrossProduct(Position3D u, Position3D v)
        {
            Position3D w = new Position3D();
            w.X = (u.Y*v.Z)-(u.Z*v.Y);
            w.Y = (u.Z*v.X)-(u.X*v.Z);
            w.Z = (u.X*v.Y)-(u.Y*v.X);
            return w;
        }
        ///<summary>Returns a normalized vector that points to the right relative to the lookingDirection vector.
        ///The vector returned is always parallel to the floor layer (it has a Y value of 0).</summary>
        ///<param name="lookingDirection">The looking direction from which the right direction is derived.</param>
        public static Position3D GetRightDirection(Position3D lookingDirection)
        {
            if(lookingDirection==null) return null;
            return new Position3D(lookingDirection.X,0,lookingDirection.Y).Rotate(0,90,0,false).NormalizedVector();
        }
        ///<summary>Returns a normalized vector that points up relative to the lookingDirection vector.
        ///The vector returned is the same as the "up" vector in world space, wich is constant
        ///(0,1,0).</summary>
        ///<param name="lookingDirection">The looking direction from which the up direction is derived.
        ///Practically, the returned vector is independent of lookingDirection. It was kept as an optional
        ///parameter in order to be in line with the GetRightDirection and GetForwardDirection methods.</param>
        public static Position3D GetUpDirection(Position3D lookingDirection=null)
        {
            return new Position3D(0,1,0);
        }
        ///<summary>Returns a normalized vector that points forward relative to the lookingDirection vector.
        ///The vector returned is always parallel to the floor layer (it has a Y value of 0).</summary>
        ///<param name="lookingDirection">The looking direction from which the forward direction is derived.</param>
        public static Position3D GetForwardDirection(Position3D lookingDirection)
        {
            if(lookingDirection==null) return null;
            return new Position3D(lookingDirection.X,0,lookingDirection.Y).NormalizedVector();
        } 
        ///<summary>Returns true, if X, Y and Z equal 0. Note that the values need to be zero exactly without tolerance.</summary>
        public bool IsZero()
        {
            return X==0 && Y==0 && Z==0;
        }
        ///<summary>Returns the dostance between this point and another.</summary>
        public float Distance (Position3D other)
        {
            float deltaX = X - other.X;//values will be squared, so no need to call Math.Abs()
            float deltaY = Y - other.Y;
            float deltaZ = Z - other.Z;
            return (float) Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
        }
        ///<summary>Returns the lenght of the vector (the distance between (0,0,0) and (X,Y,Z))</summary>
        public float VectorLength()
        {
            return (float) Math.Sqrt((X*X) + (Y*Y) + (Z*Z));
        }
        ///<summmary>Returns a vector of length 1 which has the same direction as the original vector.
        ///The original vector will not be modified.</summary>
        public Position3D NormalizedVector()
        {
            return (1/VectorLength())*this;
        }
        ///<summary>Rotates the vector around the z, x and y axis in that order.</summary>
        ///<param name="ax">The rotation angle around the x axis in degrees</param>
        ///<param name="ay">The rotation angle around the y axis in degrees</param>
        ///<param name="az">The rotation angle around the z axis in degrees</param>
        ///<param name="valsAreRadian">Set valsAreRadian to true, if ax, ay and az are radian values</param>
        public Position3D Rotate (float ax, float ay, float az, bool valsAreRadian=false)
        {
            if(!valsAreRadian)
            {
                //We will use cos and sin which use radian values, not degrees
                ax = MathTools.ToRadian(ax);
                ay = MathTools.ToRadian(ay);
                az = MathTools.ToRadian(az);
            }

            Position3D rotatedz = new Position3D();
            rotatedz.X = (float)((X*Math.Cos(az))-(Y*Math.Sin(az)));
            rotatedz.Y = (float)((Y*Math.Cos(az))+(X*Math.Sin(az)));
            rotatedz.Z = Z;
            Position3D rotatedzx = new Position3D();
            rotatedzx.X = rotatedz.X;
            rotatedzx.Y = (float)((rotatedz.Y*Math.Cos(ax))-(rotatedz.Z*Math.Sin(ax)));
            rotatedzx.Z = (float)((rotatedz.Z*Math.Cos(ax))+(rotatedz.Y*Math.Sin(ax)));
            var test=Math.Sin(1.570796);
            //Rotation around y axis
            Position3D rotatedzxy = new Position3D();
            rotatedzxy.X = (float)((rotatedzx.X*Math.Cos(ay))+(rotatedzx.Z*Math.Sin(ay)));
            rotatedzxy.Y = rotatedzx.Y;
            rotatedzxy.Z = (float)((rotatedzx.Z*Math.Cos(ay))-(rotatedzx.X*Math.Sin(ay)));
            
            return rotatedzxy;
        }
        ///<summary>Rotates the vector using a Quaternion.</summary>
        ///<param name="rq">The rotation quaternion</param>
        public Position3D Rotate (RotationQuat rq)
        {
            RotationQuat pos = new RotationQuat(X,Y,Z,0);
            RotationQuat rq_ = new RotationQuat(-rq.X, -rq.Y, -rq.Z, rq.W);
            RotationQuat rotated = rq*pos*rq_;
            return new Position3D(rotated.X, rotated.Y, rotated.Z);
        }
        ///<summary>Returns a scaled vector</summary>
        ///<param name="sx">The scale factor of the x coordinate</param>
        ///<param name="sy">The scale factor of the y coordinate</param>
        ///<param name="sz">The scale factor of the z coordinate</param>
        public Position3D Scale (float sx, float sy, float sz)
        {
            return new Position3D (X*sx, Y*sy, Z*sz);
        }
        ///<summary>Returns a scaled vector</summary>
        ///<param name="factor">The scale factor</param>
        public Position3D Scale(float factor)
        {
            return new Position3D (X*factor, Y*factor, Z*factor);
        }
        public override string ToString()
        {
            return String.Format("({0:0.0}/{1:0.0}/{2:0.0})",X,Y,Z);
        }
    }
}