using UnityEngine;

namespace Core.Shared.Extentions
{
    public static class VectorExtentions 
    {
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new(vector.x, vector.y);
        }
        public static Vector3 ToVector3(this Vector2 vector, float z=0)
        {
            return new(vector.x, vector.y, z);
        }
        public static Vector2Int Vector2ToInt(this Vector2 vector)
        {
            return new(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

    }
}
