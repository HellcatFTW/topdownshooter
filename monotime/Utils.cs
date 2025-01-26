using System;
using System.Diagnostics.Contracts;

namespace TopDownShooter
{
    public static class Utils
    {
        [Pure]
        public static float ToRotation(this Vector2 v) => (float)Math.Atan2(v.Y, v.X);
        [Pure]
        public static Vector2 ToRotationVector2(this float f) => new Vector2((float)Math.Cos(f), (float)Math.Sin(f));

        [Pure]
        public static Vector2 RotatedBy(this Vector2 spinningpoint, double radians, Vector2 center = default(Vector2))
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            Vector2 vector = spinningpoint - center;
            Vector2 result = center;
            result.X += vector.X * cos - vector.Y * sin;
            result.Y += vector.X * sin + vector.Y * cos;
            return result;
        }
        public static bool HasNaNs(this Vector2 vec)
        {
            if (!float.IsNaN(vec.X))
                return float.IsNaN(vec.Y);

            return true;
        }

        [Pure]
        public static Vector2 SafeNormalize(this Vector2 v, Vector2 defaultValue)
        {
            if (v == Vector2.Zero || v.HasNaNs())
                return defaultValue;

            return Vector2.Normalize(v);
        }

        [Pure]
        /// <summary>
        /// Takes 2 vectors, and transfers the magnitude of the first to the second while maintaining direction.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector2 TransferMagnitude(Vector2 from, Vector2 to)
        {
            float len = from.Length();
            Vector2 result = to.SafeNormalize(Vector2.Zero) * len;

            return result;
        }
        [Pure]
        public static Vector2 WithMagnitude(this Vector2 vector, float value)
        {
            return vector.SafeNormalize(Vector2.Zero) * value;
        }
        /// <summary>
        /// Directly sets magnitude of a vector to value.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetMagnitude(this ref Vector2 vector, float value)
        {
            vector = vector.WithMagnitude(value);
        }
    }
}
