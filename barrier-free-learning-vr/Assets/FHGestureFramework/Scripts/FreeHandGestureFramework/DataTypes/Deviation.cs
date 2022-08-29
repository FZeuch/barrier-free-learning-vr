using System;
using StandardTools;

namespace FreeHandGestureFramework.DataTypes
{
    ///<summary>The Deviation class is used to determine how much a distance between two
    ///float values matters. It defines two points A and B. The CalcDeviation method can be used to 
    ///get a value between 0 and 1: if the distance is less or equal A, 1 is returned. If it is greater 
    ///than A and less than B, it returns a value between 0 and 1, getting smaller while nearing B.
    ///If it is greater or equal B, 0 is returned.</summary>
    ///<remarks>The shape propertiy can be used to alter
    ///the decreasing behaviour of CalcDeviation for input values between A and B.</remarks> 
    public class Deviation
    {
        ///<value>This shape is used, if no shape value is defined while creating a new Deviation class.</value>
        public const float DEFAULT_SHAPE = 1.0f; 
        private float _A;
        private float _B;
        private float _shape = DEFAULT_SHAPE;
        public Deviation(): this (0.0f, 0.0f, DEFAULT_SHAPE) {}
        public Deviation(float A, float B): this(A, B, DEFAULT_SHAPE) {}
        public Deviation(float A, float B, float shape)
        {
            this.A = A;
            this.B = B;
            Shape = shape;
        }
        public Deviation(Deviation orig): this(orig._A, orig._B, orig._shape){}
        ///<value>The CalcDeviation method will return 1 for all input values less or equal A.</value>
        public float A 
        {
            get {return _A;}
            set
            {
                if (value < 0) _A = 0;
                else _A = value; 
                if (_B<_A)_B = value;
            }
        }
        ///<value>The CalcDeviation method will return a value between 0 and 1 for input values
        ///between A and B. The return value gets lower while nearing B.
        ///Use the shape value to define the decreasing behaviour of CalcDeviation 
        ///for input values between A and B.</value>
        public float B
        {
            get {return _B;}
            set
            {
                if (value < _A) _B = _A;
                else _B = value; 
            }
        }
        ///<value>The Shape property defines the decreasing behaviour of CalcDeviation 
        ///for input values between A and B. Be x the distance between A and the
        ///input value, A &lt; input value &lt; B. The following formula will be used
        ///in CalcDeviation: <code>f(x) = 1-(x/(B-A))^Shape</code>.
        ///The zero of this function is always at B. For a Shape value of 1,
        ///f(x) shows a linear behaviour. For Shape values greater than 1, the function
        ///graph of f(x) drops more slowly for lower x, then increasingly faster.
        ///A Shape value between 0 and 1 will show the opposite behaviour.</value>
        public float Shape 
        {
            get
            {
                return _shape;
            }
            set 
            {
                if (value < 0) _shape = 0;
                else _shape = value;
            }
        }
        ///<value>The CalcDeviation method will return 1 for input values less than A,
        ///a value between 0 and 1 for input values between A and B, and 0 for input values
        ///greater than B. The return value gets lower while nearing B.
        ///Use the shape value to define the decreasing behaviour of CalcDeviation 
        ///for input values between A and B.</value>
        public float CalcDeviation(float value)
        {
            value = Math.Abs(value);
            if (value <= _A) return 1.0f;//in range [0,_A], the return value is constant 1
            else if (value < _B)
            {
                //see documentation comments of Shape property
                float x = value-_A;
                if (Shape == 1.0f)
                {   //fast execution if shape is 1 
                    return 1-(x/(_B-_A)); 
                }
                else
                {
                    return (float)(1.0 - Math.Pow(x/(_B-_A),Shape)); 
                }
            }
            else return 0.0f; // value >= _B
        }
    }

    public class Deviation3D
    {
        public Position3D RelativeVector {get; set;}
        public Deviation[] Tolerances = new Deviation[6];
        private Position3D _lastDirectionRight = new Position3D(1,0,0);
        public Deviation3D() : this(new Position3D(), new Deviation(), new Deviation(), new Deviation(), new Deviation(), new Deviation(), new Deviation()) {}
        public Deviation3D(Position3D relativeVector, Deviation tolerance)
            : this (relativeVector, tolerance, tolerance, tolerance, tolerance, tolerance, tolerance) {}
        public Deviation3D(Position3D relativeVector, Deviation tolX, Deviation tolY, Deviation tolZ) 
            : this(relativeVector, tolX, tolX, tolY, tolY, tolZ, tolZ) {}
        public Deviation3D (Position3D relativeVector, Deviation tolNegX, Deviation tolPosX, Deviation tolNegY, Deviation tolPosY, Deviation tolNegZ, Deviation tolPosZ)
        {
            if (relativeVector==null || relativeVector == new Position3D(0,0,0)) throw new System.ArgumentException("relativeVector must not be null or null vector!");

            RelativeVector = relativeVector;
            Tolerances[0] = tolNegX;
            Tolerances[1] = tolPosX;
            Tolerances[2] = tolNegY;
            Tolerances[3] = tolPosY;
            Tolerances[4] = tolNegZ;
            Tolerances[5] = tolPosZ;
        }
        public Deviation3D (Deviation3D orig): this(new Position3D(orig.RelativeVector), new Deviation(orig.Tolerances[0]), new Deviation(orig.Tolerances[1]), new Deviation(orig.Tolerances[2]), new Deviation(orig.Tolerances[3]), new Deviation(orig.Tolerances[4]), new Deviation(orig.Tolerances[5])){}
        public Deviation GetTolerance (DirectionLeftHanded d) {return Tolerances[(int)d];}
        public Deviation GetTolerance (DirectionLeftHandedAxes d) {return Tolerances[(int)d];}
        public Deviation GetTolerance (DirectionRPYLeftHanded d) {return Tolerances[(int)d];}
        public void SetTolerance (DirectionLeftHanded dir, Deviation dev) {Tolerances[(int)dir] = dev;}
        public void SetTolerance (DirectionLeftHandedAxes dir, Deviation dev) {Tolerances[(int)dir] = dev;}
        public void SetTolerance (DirectionRPYLeftHanded dir, Deviation dev) {Tolerances[(int)dir] = dev;}
        ///<summary>The CalcPositionalDeviation method returns a value between 0 and 1 that determines
        ///if pointB is close enough to relativeVector added to pointA taking tolerance values
        ///into account. relativeVector's coordinate system corresponds to the lookingDirection.
        ///E.g., if relativeVector is (-1,0,0), CalcDeviation checks if pointB is one unit in looking
        ///direction left to pointA, or within the tolerance values, which also correspond to
        ///the looking direction.</summary>
        public float CalcPositionalDeviation(Position3D pointA, Position3D pointB, Position3D lookDirection = null)
        {
            if (lookDirection == null || lookDirection.IsZero()) lookDirection = RelativeVector;
            
            //CALCULATE NORMALIZED DIRECTION VECTORS.

            //Forward direction is the looking direction.
            Position3D directionForward = lookDirection.NormalizedVector();

            //Right direction is a vector that is orthogonal to directionForward and has y=0,
            //so the scalar product of directionForward and directionRight must be 0. 
            //(x1*x2)+(y1*0)+(z1*z2)=0 <=> x1*x2=-(z1*z2) => (1) x2=-(z1*z2)/x1 | x1!=0, x2 arbitrary, z1=0 or z2=0 | x1=0
            //                         <=> z1*z2=-(x1*x2) => (2) z2=-(x1*x2)/z1 | z1!=0, z2 arbitrary, x2=0 or x2=0 | z1=0
            //substituting (2) in (1), we find that
            //(A) x1!=0 and z1!=0 => x2 arbitrary, z2=-(x1*x2)/z1
            //(B) x1=0 and z1!=0 => x2 arbitrary z2=0 ( =-(x1*x2)/z1 )
            //(C) x1!=0 and z1=0 => x2=0, z2 arbitrary
            //(D) x1=0 and z1=0 => x2 and z2 arbitrary
            Position3D directionRight = new Position3D(0,0,0);
            if (directionForward.Z != 0) //cases (A) and (B)
            {
                directionRight.X = directionForward.Z; //In order to point right not left, directionRight.X must be positive if directionForward.Z > 0
                directionRight.Z = -(directionForward.X*directionRight.X)/directionForward.Z;
            }
            else if(directionForward.X != 0) //case (C)
            {
                directionRight.X = 0;
                directionRight.Z = -directionForward.X; //In order to point right and not left, directionRight.Z must be positive if directionForward.X < 0
            }
            else //case (D)
            {
                directionRight = _lastDirectionRight;//special case. To avoid random x and z vals, last directionRight is used
            }
            directionRight=directionRight.NormalizedVector();
            _lastDirectionRight=directionRight;

            //Up direction is the negative cross product of right and forward direction vectors
            Position3D directionUp = -Position3D.CrossProduct(directionRight, directionForward);
            directionUp=directionUp.NormalizedVector();

            //CALCULATE DEVIATION VALUES FOR EACH DIRECTION

            //Add relativeVector to PointA. Point A is in world space, but relativeVector corresponds to the looking direction.
            Position3D targetPoint=pointA;
            targetPoint+=RelativeVector.X*directionRight;
            targetPoint+=RelativeVector.Y*directionUp;
            targetPoint+=RelativeVector.Z*directionForward;

            //We use Hessian normal form to describe three planes through targetPoint. The Hessian normal form uses a normalized
            //vector and a distance value d to define a plane. We use the three direction vectors as plane normals.
            //For each of them, we have to calculate d by solving targetPoint*directionVector-d=0 <=> d=targetPoint*directionVector
            //(* is the scalar product). If d is negative, the direction vector points away from the plane (in Hessian normal
            //form it must point in direction of the plane). In this case, both the direction vector and d must be negated to 
            //define the plane. This also means, that the plane normal points in opposite direction of the direction vector,
            //so that e.g. right might become left. For that reason, we use a sign variable for each plane, so that later
            //we can distinguish left from right, up from down and back from forward.
            int directionXsign=1;
            Position3D planeVectorX = directionRight;
            float planeDistanceX=targetPoint*planeVectorX;
            if(planeDistanceX<0)
            {
                directionXsign=-1;
                planeDistanceX=-planeDistanceX;
                planeVectorX=-planeVectorX;
            }

            int directionYsign=1;
            Position3D planeVectorY = directionUp;
            float planeDistanceY=targetPoint*planeVectorY;
            if(planeDistanceY<0)
            {
                directionYsign=-1;
                planeDistanceY=-planeDistanceY;
                planeVectorY=-planeVectorY;
            }

            int directionZsign=1;
            Position3D planeVectorZ = directionForward;
            float planeDistanceZ=targetPoint*planeVectorZ;
            if(planeDistanceZ<0)
            {
                directionZsign=-1;
                planeDistanceZ=-planeDistanceZ;
                planeVectorZ=-planeVectorZ;
            }

            //Calculate the distances of pointB to each of these planes: pointB*directionVector-distance.
            //A positive value means that the point is on the side of the plane the direction vector is pointing
            //(e.g. right direction). A negative value means the point is on the side of the plane in opposite
            //direction to the direction vector (e.g. left direction). A value of 0 means the point is on the plane.
            float pointBDistanceX=pointB*planeVectorX-planeDistanceX;
            float pointBDistanceY=pointB*planeVectorY-planeDistanceY;
            float pointBDistanceZ=pointB*planeVectorZ-planeDistanceZ;

            //Calculate deviation values for each direction
            float deviationRight=pointBDistanceX*directionXsign>=0 ? Tolerances[1].CalcDeviation(pointBDistanceX)
                                                            :Tolerances[0].CalcDeviation(-pointBDistanceX);
            float deviationUp=pointBDistanceY*directionYsign>=0 ? Tolerances[3].CalcDeviation(pointBDistanceY)
                                                            :Tolerances[2].CalcDeviation(-pointBDistanceY);
            float deviationForward=pointBDistanceZ*directionZsign>=0 ? Tolerances[5].CalcDeviation(pointBDistanceZ)
                                                            :Tolerances[4].CalcDeviation(-pointBDistanceZ);

            //COMBINE ALL THREE DEVIATION VALUES
            return deviationForward*deviationRight*deviationUp;
        }
        ///<summary>The CalcSimpleDeviation method returns a value between 0 and 1 that describes the similarity of p and
        ///RelativeVector. When comparing p to RelativeVector, a value between 0 and 1 is calculated for each axis.
        ///All three values will be multiplied.
        ///</summary>
        public float CalcSimpleDeviation(Position3D p)
        {
            //Calculate deviation values for each direction
            float deviationX=p.X>=RelativeVector.X ? Tolerances[1].CalcDeviation(p.X-RelativeVector.X)
                                                            :Tolerances[0].CalcDeviation(RelativeVector.X-p.X);
            float deviationY=p.Y>=RelativeVector.Y ? Tolerances[3].CalcDeviation(p.Y-RelativeVector.Y)
                                                            :Tolerances[2].CalcDeviation(RelativeVector.Y-p.Y);
            float deviationZ=p.Z>=RelativeVector.Z ? Tolerances[5].CalcDeviation(p.Z-RelativeVector.Z)
                                                            :Tolerances[4].CalcDeviation(RelativeVector.Z-p.Z);

            //COMBINE ALL THREE DEVIATION VALUES
            return deviationX*deviationY*deviationZ;
        }
        public float CalcSimpleDeviation(Position3D p, int modValue)
        {
            //Calculate deviation values for each direction
            float deviationX=Math.Max(Tolerances[0].CalcDeviation(MathTools.PositiveModValue(RelativeVector.X-p.X,modValue)),
                                      Tolerances[1].CalcDeviation(MathTools.PositiveModValue(p.X-RelativeVector.X,modValue)));
            float deviationY=Math.Max(Tolerances[2].CalcDeviation(MathTools.PositiveModValue(RelativeVector.Y-p.Y,modValue)),
                                      Tolerances[3].CalcDeviation(MathTools.PositiveModValue(p.Y-RelativeVector.Y,modValue)));
            float deviationZ=Math.Max(Tolerances[4].CalcDeviation(MathTools.PositiveModValue(RelativeVector.Z-p.Z,modValue)),
                                      Tolerances[5].CalcDeviation(MathTools.PositiveModValue(p.Z-RelativeVector.Z,modValue)));

            //COMBINE ALL THREE DEVIATION VALUES
            return deviationX*deviationY*deviationZ;
        }
    }
}