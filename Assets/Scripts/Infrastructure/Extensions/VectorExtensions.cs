using UnityEngine;

namespace Infrastructure.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 ToXY (this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        } 

        public static Vector3 AddZ (this Vector2 vector2, float z = 0)
        {
            return new Vector3(vector2.x, vector2.y, z);
        }
    }
}