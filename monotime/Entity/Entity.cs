
using System.Collections.Generic;
using System;
using System.Reflection.Metadata;

namespace TopDownShooter.Entity
{
    public abstract class Entity
    {
        public Vector2 Position { get => position; }
        protected Vector2 position;
        public float Rotation { get => rotation; }
        protected float rotation;
        public Vector2 Velocity { get => velocity; }
        protected Vector2 velocity;
        #pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public HitBox? HitBox { get => hitBox; }
        protected HitBox? hitBox;
        #pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public bool IsActive { get => isActive; }
        protected bool isActive = true;
        public abstract void Update();

        public abstract void Draw();

        public bool CheckCollisionWith(Entity entityToCheck)
        {
            if (hitBox == null || entityToCheck.hitBox == null)
            { 
                return false; 
            }
            
            return HitBox.Intersect(hitBox, entityToCheck.hitBox);
        }
    }
    public class HitBox
    {
        // from https://github.com/tedigc/SeparatingAxisTheorem, not mine.

        private Vector2 v1;
        private Vector2 v2;
        private Vector2 v3;
        private Vector2 v4;

        public Vector2[] Vertices { get => vertices; }
        private Vector2[] vertices = new Vector2[4];

        public float rotation;
        private Vector2 origin;

        private readonly int edgeCount;

        public HitBox(Vector2 position, Rectangle rectangle,float rotation)
        {
            float height = rectangle.Height;
            float width = rectangle.Width;

            float x = position.X;
            float y = position.Y;

            v1 = new Vector2(x, y);
            v2 = new Vector2(x + width, y);
            v3 = new Vector2(x + width, y + height);
            v4 = new Vector2(x, y + height);

            this.rotation = 0;
            origin = new Vector2(width / 2, height / 2);

            vertices[0] = v1 - origin;
            vertices[1] = v2 - origin;
            vertices[2] = v3 - origin;
            vertices[3] = v4 - origin;

            edgeCount = vertices.Length;

            Rotate(rotation);
        }

        public Vector2 GetEdge(int index)
        {
            Vector2 v1 = vertices[index];
            Vector2 v2 = vertices[(index + 1) % vertices.Length];
            return v1 - v2;
        }
        public Vector2 GetEdgeNormal(int index)
        {
            Vector2 edge = GetEdge(index);
            return new Vector2(edge.Y, -edge.X);
        }
        public List<Vector2> GetEdgeNormals()
        {
            List<Vector2> normals = new List<Vector2>();
            for (int i = 0; i < edgeCount; i++)
            {
                normals.Add(GetEdgeNormal(i));
            }
            return normals;
        }

        public static bool Intersect(HitBox polygon1, HitBox polygon2)
        {
            List<Vector2> normals = new List<Vector2>();
            normals.AddRange(polygon1.GetEdgeNormals());
            normals.AddRange(polygon2.GetEdgeNormals());
            foreach (Vector2 axis in normals)
            {
                var (min1, max1) = GetMinMaxProjections(polygon1, axis);
                var (min2, max2) = GetMinMaxProjections(polygon2, axis);
                float intervalDistance = min1 < min2 ? min2 - max1 : min1 - max2;
                if (intervalDistance >= 0) return false;
            }
            return true;
        }

        private static (float, float) GetMinMaxProjections(HitBox polygon, Vector2 axis)
        {
            float min = Int32.MaxValue;
            float max = Int32.MinValue;
            foreach (Vector2 vertex in polygon.vertices)
            {
                Vector2 projection = Project(vertex, axis);
                float scalar = Scalar(projection, axis);
                if (scalar < min) min = scalar;
                if (scalar > max) max = scalar;
            }
            return (min, max);
        }
        private static Vector2 Project(Vector2 vertex, Vector2 axis)
        {
            float dot = Vector2.Dot(vertex, axis);
            float mag2 = axis.LengthSquared();
            return dot / mag2 * axis;
        }

        private static float Scalar(Vector2 vertex, Vector2 axis)
        {
            return Vector2.Dot(vertex, axis);
        }

        public void Rotate(float angle)
        {
            float diff = angle - rotation;
            rotation = angle;

            Vector2 offset = vertices[0] + origin;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector2.Transform(vertices[i] - offset, Matrix.CreateRotationZ(diff)) + offset;
            }
        }

        private static Vector2 FindClosestVertex(HitBox polygon, Vector2 vertex)
        {
            float shortestDistance = Int32.MaxValue;
            Vector2 closestVertex = polygon.vertices[0];
            foreach (Vector2 polygonVertex in polygon.vertices)
            {
                float currentDistance = Vector2.DistanceSquared(vertex, polygonVertex);
                if (currentDistance < shortestDistance)
                {
                    closestVertex = polygonVertex;
                    shortestDistance = currentDistance;
                }
            }
            return closestVertex;
        }
    }
    public interface IHealth
    {
        public float Health { get;}
        public void OnHit(Projectile projectile);
    }
    
}
