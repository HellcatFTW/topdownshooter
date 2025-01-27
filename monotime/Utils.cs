using System;
using System.Collections.Generic;
using TopDownShooter.Entity;

namespace TopDownShooter
{
    public static class Utils
    {
        #region Float
        public static float ToRotation(this Vector2 v) => (float)Math.Atan2(v.Y, v.X);
        #endregion
        #region Vector2
        public static Vector2 ToRotationVector2(this float f) => new Vector2((float)Math.Cos(f), (float)Math.Sin(f));
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

        public static Vector2 SafeNormalize(this Vector2 v, Vector2 defaultValue = default(Vector2))
        {
            if (v == Vector2.Zero || v.HasNaNs())
                return defaultValue;

            return Vector2.Normalize(v);
        }

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
        #endregion

        #region Collision Math

        /// <summary>
        /// Takes 2 arrays of vertices for 2 convex shapes, returns a vector if the 2 shapes are intersecting. The vector is the minimum translation vector required to move <paramref name="ConvexShape2Vertices"/> so that it is no longer
        /// intersecting <paramref name="ConvexShape1Vertices"/>.
        /// </summary>
        /// <param name="ConvexShape1Vertices"></param>
        /// <param name="ConvexShape2Vertices"></param>
        /// <returns></returns>
        public static Vector2? SeparatingAxisTheorem(Vector2[] ConvexShape1Vertices, Vector2[] ConvexShape2Vertices)
        {
            Vector2[] axesToTest = new Vector2[ConvexShape1Vertices.Length + ConvexShape2Vertices.Length];
            
            Vector2[] normals1 = GetNormals(ConvexShape1Vertices);
            Vector2[] normals2 = GetNormals(ConvexShape2Vertices);

            for (int i = 0; i < ConvexShape1Vertices.Length; i++)
            {
                axesToTest[i] = normals1[i];
            }
            for (int i = ConvexShape1Vertices.Length; i < ConvexShape1Vertices.Length + ConvexShape2Vertices.Length; i++)
            {
                axesToTest[i] = normals2[i - ConvexShape1Vertices.Length];
            }
            
            float minimumProjectionDifference = float.PositiveInfinity;
            Vector2 minimumTranslationAxis = Vector2.Zero;

            foreach (Vector2 axis in axesToTest) // Which axis we are currently testing
            {
                float minimumOfFirstShape = float.PositiveInfinity;
                float maximumOfFirstShape = float.NegativeInfinity;

                float minimumOfSecondShape = float.PositiveInfinity;
                float maximumOfSecondShape = float.NegativeInfinity;

                foreach (Vector2 vertex in ConvexShape1Vertices) // get the lowest and highest result of projecting vertices of shape 1 onto the axis we are testing
                {
                    float projectionLength = ProjectionLength(vertex, axis);
                    maximumOfFirstShape = Math.Max(maximumOfFirstShape, projectionLength);
                    minimumOfFirstShape = Math.Min(minimumOfFirstShape, projectionLength);
                }
                foreach (Vector2 vertex in ConvexShape2Vertices) // get the lowest and highest result of projecting vertices of shape 2 onto the axis we are testing
                {
                    float projectionLength = ProjectionLength(vertex, axis);
                    maximumOfSecondShape = Math.Max(maximumOfSecondShape, projectionLength);
                    minimumOfSecondShape = Math.Min(minimumOfSecondShape, projectionLength);
                }

                if (minimumOfFirstShape < minimumOfSecondShape) // if first shape projection starts lower than second shape projection
                {
                    if (maximumOfFirstShape < minimumOfSecondShape)
                    {
                        return null; // 100% no collision
                    }

                    if ((maximumOfFirstShape - minimumOfSecondShape) < minimumProjectionDifference) // if the intersection distance on current axis is lower than the minimum, store the axis responsible and the resulting minimum.
                    {
                        minimumProjectionDifference = (maximumOfFirstShape - minimumOfSecondShape);
                        minimumTranslationAxis = axis;
                    }
                }
                else // if second shape projection starts lower than first shape projection
                {
                    if (maximumOfSecondShape < minimumOfFirstShape)
                    {
                        return null; // 100% no collision
                    }

                    if ((maximumOfSecondShape - minimumOfFirstShape) < minimumProjectionDifference) // if the intersection distance on current axis is lower than the minimum, store the axis responsible and the resulting minimum.
                    {
                        minimumProjectionDifference = (maximumOfSecondShape - minimumOfFirstShape);
                        minimumTranslationAxis = -axis;
                    }
                }
            }

            return minimumProjectionDifference * minimumTranslationAxis; // Return minimum translation vector. move object by this amount and it's not intersecting anymore.
        }
        /// <summary>
        /// Returns length of vector onto axis.
        /// </summary>
        /// <param name="vectorToProject"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float ProjectionLength(Vector2 vectorToProject, Vector2 axis)
        {
            return Vector2.Dot(vectorToProject, axis.SafeNormalize());
        }
        public static Vector2[] GetNormals(Vector2[] vertices)
        {
            var edges = GetEdges(vertices);
            Vector2[] normals = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                normals[i] = edges[i].RotatedBy(MathHelper.PiOver2).SafeNormalize();
            }
            return normals;
        }
        public static Vector2[] GetEdges(Vector2[] vertices)
        {
            Vector2[] edges = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                edges[i] = vertices[(i + 1) % vertices.Length] - vertices[i];
            }
            return edges;
        }
        #endregion
    }
}
