
using System.Collections.Generic;
using System;
using System.Reflection.Metadata;
using static TopDownShooter.Utils;

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
        public HitBox? HitBoxBounds { get => hitBox; }
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

            return HitBox.Intersect(HitBoxBounds?.Vertices, entityToCheck.HitBoxBounds?.Vertices);
        }
    }
    public struct HitBox
    {
        public Vector2[] Vertices { get => vertices; }
        private Vector2[] vertices = new Vector2[4];

        public float rotation;
        private Vector2 origin;

        public HitBox(Vector2 position, Rectangle rectangle, float rotation)
        {
            float height = rectangle.Height;
            float width = rectangle.Width;

            float x = position.X;
            float y = position.Y;

            Vector2 Vertex1 = new Vector2(x, y);
            Vector2 Vertex2 = new Vector2(x + width, y);
            Vector2 Vertex3 = new Vector2(x + width, y + height);
            Vector2 Vertex4 = new Vector2(x, y + height);

            this.rotation = 0;
            origin = new Vector2(width / 2, height / 2);

            vertices[0] = Vertex1 - origin;
            vertices[1] = Vertex2 - origin;
            vertices[2] = Vertex3 - origin;
            vertices[3] = Vertex4 - origin;

            ApplyRotationTransform(rotation);
        }
        public void ApplyRotationTransform(float radians)
        {
            Vector2 offset = vertices[0] + origin;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i].RotatedBy(radians - rotation, offset);
            }
            rotation = radians;
        }
        public static bool Intersect(Vector2[] hitbox1, Vector2[] hitbox2)
        {
            return SeparatingAxisTheorem(hitbox1, hitbox2) != null;
        }
        public static Vector2? MinimumTranslationVector(Vector2[] hitbox1, Vector2[] hitbox2)
        {
            return SeparatingAxisTheorem(hitbox1, hitbox2);
        }
    }
    public interface IHealth
    {
        public float Health { get;}
        public void OnHit(Projectile projectile);
    }
    
}
