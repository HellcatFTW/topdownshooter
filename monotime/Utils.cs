using System;
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
        public static Vector2 WithRotation(this Vector2 spinningpoint, double radians, Vector2 center = default(Vector2))
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            Vector2 vector = spinningpoint - center;
            Vector2 result = Vector2.Zero;
            result.X = vector.X * cos - vector.Y * sin;
            result.Y = vector.X * sin + vector.Y * cos;
            return result + center;
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

        public static bool IsZero(this Vector2 vector)
        {
            if (Math.Abs(vector.X) < 0.001 && Math.Abs(vector.Y) < 0.001)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Note: A is the normal. B is the MTV.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Vector2 Reject(this Vector2 A , Vector2 B)
        {
            float projLength = ProjectionLength(B, A); // Project B onto A
            Vector2 parallelComponent = A.WithMagnitude(projLength); //  Get vector parallel to A

            return B - parallelComponent; // subtract parallel component of B from it
        }
        #endregion
        #region Color
        public static Color WithOpacity(this Color col, float val)
        {
            return new Color(col.R, col.G, col.B, (byte)MathHelper.Clamp(val * 255f, 0f, 255f));
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
        public static Vector2? SeparatingAxisTheorem(Vector2[] ConvexShape1Vertices, Vector2[] ConvexShape2Vertices, out Vector2? MTVStartingPoint, out bool FlipMTVWhenDrawing)
        {
            AxisBoolPair[] axesToTest = new AxisBoolPair[ConvexShape1Vertices.Length + ConvexShape2Vertices.Length];
            MTVStartingPoint = null;
            FlipMTVWhenDrawing = false;

            Vector2[] normals1 = GetNormals(ConvexShape1Vertices);
            Vector2[] normals2 = GetNormals(ConvexShape2Vertices);

            for (int i = 0; i < ConvexShape1Vertices.Length; i++)
            {
                axesToTest[i].Axis = normals1[i];
                axesToTest[i].isFromFirstShape = true;
            }
            for (int i = ConvexShape1Vertices.Length; i < ConvexShape1Vertices.Length + ConvexShape2Vertices.Length; i++)
            {
                axesToTest[i].Axis = normals2[i - ConvexShape1Vertices.Length];
                axesToTest[i].isFromFirstShape = false;
            }

            float minimumProjectionDifference = float.PositiveInfinity;
            Vector2 minimumTranslationAxis = Vector2.Zero;

            foreach (AxisBoolPair axisBoolPair in axesToTest) // Which axis we are currently testing
            {
                MinMaxPair firstShapeValues = new MinMaxPair();
                MinMaxPair secondShapeValues = new MinMaxPair();

                Vector2 axis = axisBoolPair.Axis;
                bool isCurrentAxisFromFirstShape = axisBoolPair.isFromFirstShape;

                foreach (Vector2 vertex in ConvexShape1Vertices) // get the lowest and highest result of projecting vertices of shape 1 onto the axis we are testing
                {
                    float projectionLength = ProjectionLength(vertex, axis);
                    firstShapeValues.UpdateValues(projectionLength, vertex);
                }
                foreach (Vector2 vertex in ConvexShape2Vertices) // get the lowest and highest result of projecting vertices of shape 2 onto the axis we are testing
                {
                    float projectionLength = ProjectionLength(vertex, axis);
                    secondShapeValues.UpdateValues(projectionLength, vertex);
                }

                if (firstShapeValues.minimum < secondShapeValues.minimum) // if first shape projection starts lower than second shape projection
                {
                    if (firstShapeValues.maximum < secondShapeValues.minimum)
                    {
                        return null; // 100% no collision
                    }

                    if ((firstShapeValues.maximum - secondShapeValues.minimum) < minimumProjectionDifference) // if the intersection distance on current axis is lower than the minimum, store the axis responsible and the resulting minimum.
                    {
                        minimumProjectionDifference = (firstShapeValues.maximum - secondShapeValues.minimum);
                        minimumTranslationAxis = axis;

                        if (isCurrentAxisFromFirstShape)
                        {
                            MTVStartingPoint = secondShapeValues.vertexForMinimum;
                            FlipMTVWhenDrawing = false;
                        }
                        else
                        {
                            MTVStartingPoint = firstShapeValues.vertexForMaximum;
                            FlipMTVWhenDrawing = true;
                        }
                    }
                }
                else // if second shape projection starts lower than first shape projection
                {
                    if (secondShapeValues.maximum < firstShapeValues.minimum)
                    {
                        return null; // 100% no collision
                    }

                    if ((secondShapeValues.maximum - firstShapeValues.minimum) < minimumProjectionDifference) // if the intersection distance on current axis is lower than the minimum, store the axis responsible and the resulting minimum.
                    {
                        minimumProjectionDifference = (secondShapeValues.maximum - firstShapeValues.minimum);
                        minimumTranslationAxis = -axis;

                        if (isCurrentAxisFromFirstShape)
                        {
                            MTVStartingPoint = secondShapeValues.vertexForMaximum;
                            FlipMTVWhenDrawing = false;
                        }
                        else
                        {
                            MTVStartingPoint = firstShapeValues.vertexForMinimum;
                            FlipMTVWhenDrawing = true;
                        }
                    }
                }
            }

            if (IsZero(minimumProjectionDifference * minimumTranslationAxis))
            {
                return null; // prevents weird case of mtv being 0,0
            }

            return minimumProjectionDifference * minimumTranslationAxis; // Return minimum translation vector. move object by this amount and it's not intersecting anymore.
        }
        private struct MinMaxPair
        {
            public float minimum;
            public float maximum;

            public Vector2 vertexForMinimum;
            public Vector2 vertexForMaximum;

            public MinMaxPair()
            {
                minimum = float.PositiveInfinity;
                maximum = float.NegativeInfinity;

                vertexForMinimum = new Vector2();
                vertexForMaximum = new Vector2();
            }
            public void UpdateValues(float value, Vector2 vertex)
            {
                if (value < minimum)
                {
                    minimum = value;
                    vertexForMinimum = vertex;
                }
                if (value > maximum)
                {
                    maximum = value;
                    vertexForMaximum = vertex;
                }
            }
        }
        private struct AxisBoolPair
        {
            public Vector2 Axis;
            public bool isFromFirstShape;

            public AxisBoolPair()
            {
                Axis = new Vector2();
                isFromFirstShape = false;
            }
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
                normals[i] = edges[i].WithRotation(MathHelper.PiOver2).SafeNormalize();
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

        public static void DrawHitbox(HitBox hitbox, Color color = default(Color))
        {
            if (color == default(Color))
            {
                color = Color.DarkRed;
            }
            color = color.WithOpacity(.7f);

            Texture2D pixelTexture = GenerateTexture(1, 1, color);

            Rectangle rect = new(x: (int)hitbox.Vertices[0].X - ((int)World.cameraPos.X),
                                 y: (int)hitbox.Vertices[0].Y - ((int)World.cameraPos.Y),
                                 width: (int)(hitbox.Vertices[0] - hitbox.Vertices[1]).Length(),
                                 height: (int)(hitbox.Vertices[0] - hitbox.Vertices[^1]).Length());

            Globals.SpriteBatch.Draw(pixelTexture, rect, null, color, hitbox.Rotation, Vector2.Zero, SpriteEffects.None, LayerDepths.UI);
        }
        public static void DrawLine(Vector2 vectorToDraw, Vector2 startingPoint, Color color = default(Color))
        {
            if (color == default(Color))
            {
                color = Color.Red;
            }

            float rotation = vectorToDraw.ToRotation();

            Texture2D pixelTexture = GenerateTexture(1, 1, color);

            Vector2 startingDrawPos = startingPoint - World.cameraPos;
            int startX = ((int)startingDrawPos.X);
            int startY = ((int)startingDrawPos.Y);

            Rectangle rect = new(x: startX,
                                 y: startY,
                                 width: ((int)(vectorToDraw).Length()),
                                 height: 1);

            Globals.SpriteBatch.Draw(pixelTexture, rect, null, color, rotation, Vector2.Zero, SpriteEffects.None, LayerDepths.UI);
        }
        public static void DrawPixel(Vector2 position, Color color = default(Color))
        {
            if (color == default(Color))
            {
                color = Color.White;
            }
            Texture2D pixelTexture = GenerateTexture(1, 1, color);
            Globals.SpriteBatch.Draw(pixelTexture, position, color);
        }
        public static void DrawText(string text, Vector2 position, Color color = default(Color))
        {
            Globals.SpriteBatch.DrawString(Globals.Content.Load<SpriteFont>("Arial"), text, position, color);
        }
        public static Texture2D GenerateTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(Globals.graphics.GraphicsDevice, width, height);

            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Length; pixel++)
            {
                data[pixel] = color;
            }
            texture.SetData(data);
            return texture;
        }
    }
}
