
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
        public HitBox? Hitbox { get => hitBox; }
        protected HitBox? hitBox;
        public bool IsActive { get => isActive; }
        protected bool isActive = true;
        public abstract void Update();

        public abstract void Draw();

        public virtual void MoveBy(Vector2 vector)
        {
            position += vector;
            if (hitBox == null)
            {
                return;
            }
            hitBox.Value.MoveVerticesBy(vector);
        }
        public virtual void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
            if (hitBox == null)
            {
                return;
            }
            hitBox.Value.SetHitboxPosition(newPosition);
        }
        public bool CheckCollisionWith(Entity entityToCheck)
        {
            if (hitBox == null || entityToCheck.hitBox == null)
            { 
                return false; 
            }

            return HitBox.Intersect(Hitbox.Value, entityToCheck.Hitbox.Value);
        }
    }
    public struct HitBox
    {
        public Vector2[] Vertices { get => vertices; }
        private Vector2[] vertices = new Vector2[4];

        public float Rotation { get => (vertices[1] - vertices[0]).ToRotation(); }
        private Vector2 Origin { get => (vertices[2] + vertices[0]) / 2; }

        public HitBox(Vector2 position, Rectangle rectangle, float rotation)
        {
            float height = rectangle.Height;
            float width = rectangle.Width;

            float x = position.X;
            float y = position.Y;

            Vector2 Vertex0 = new Vector2(x, y);
            Vector2 Vertex1 = new Vector2(x + width, y);
            Vector2 Vertex2 = new Vector2(x + width, y + height);
            Vector2 Vertex3 = new Vector2(x, y + height);

            Vector2 origin = new Vector2(width / 2, height / 2);

            vertices[0] = Vertex0 - origin;
            vertices[1] = Vertex1 - origin;
            vertices[2] = Vertex2 - origin;
            vertices[3] = Vertex3 - origin;

            SetHitboxRotation(rotation);
        }
        public void SetHitboxRotation(float radians)
        {
            Vector2 oldOrigin = Origin;
            float difference = radians - Rotation;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i].WithRotation(difference, oldOrigin);
            }
        }
        public void SetHitboxPosition(Vector2 newPosition)
        {
            float oldRotation = Rotation;
            float width = (vertices[0] - vertices[1]).Length();
            float height = (vertices[0] - vertices[^1]).Length();
            Vector2 oldOrigin = new Vector2(width / 2, height / 2); 

            vertices[0] = newPosition - oldOrigin;
            vertices[1] = vertices[0] + new Vector2(width, 0);
            vertices[2] = vertices[0] + new Vector2(width, height);
            vertices[3] = vertices[0] + new Vector2(0, height);

            SetHitboxRotation(oldRotation);
        }
        public void MoveVerticesBy(Vector2 mtv)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += mtv;
            }
        }
        public static bool Intersect(HitBox hitbox1, HitBox hitbox2)
        {
            return SeparatingAxisTheorem(hitbox1.Vertices, hitbox2.Vertices, out _) != null;
        }
        public static Vector2? MinimumTranslationVector(HitBox hitbox1, HitBox hitbox2, out Vector2? MTVStartingPoint)
        {
            return SeparatingAxisTheorem(hitbox1.Vertices, hitbox2.Vertices, out MTVStartingPoint);
        }
    }
    public interface IHealth
    {
        public float Health { get;}
        public void OnHit(Projectile projectile);
    }
    
}
