
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

        public abstract void Update();

        public abstract void Draw();
    }
}
