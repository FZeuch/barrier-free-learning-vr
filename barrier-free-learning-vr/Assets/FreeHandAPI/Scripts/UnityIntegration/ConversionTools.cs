using FreeHandGestureFramework.EnumsAndTypes;
using UnityEngine;

namespace FreeHandGestureUnity
{
    internal static class ConversionTools
    {
        internal static Position3D Vector3ToPosition3D(Vector3 vector)
        {
            if (vector==null) return null;
            return new Position3D(vector.x, vector.y, vector.z);
        }
        internal static Vector3 Position3DToVector3(Position3D position)
        {
            if (position==null) return new Vector3();
            return new Vector3(position.X, position.Y, position.Z);
        }
        internal static Position3D[] Vector3ArrayToPosition3D(Vector3[] vectors)
        {
            if (vectors==null) return null;
            Position3D[] positions = new Position3D[vectors.Length];
            for(int i=0; i<vectors.Length; i++) positions[i]=Vector3ToPosition3D(vectors[i]);
            return positions;
        }
        internal static Vector3[] Position3DArrayToVector3(Position3D[] positions)
        {
            if (positions==null) return null;
            Vector3[] vectors = new Vector3[positions.Length];
            for(int i=0; i<vectors.Length; i++) vectors[i]=Position3DToVector3(positions[i]);
            return vectors;
        }
    }
}