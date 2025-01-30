
namespace TopDownShooter.Entity
{
    public abstract class Enemy : Entity, IHealth
    {
        protected abstract float MovementSpeed { get; set; }
        public abstract float Health { get; }

        public Enemy()
        {
        }
        public abstract override void Update();
        public abstract override void Draw();
        public abstract void Shoot();
        public void Kill()
        {
            isActive = false;
        }
        public static T NewEnemy<T>() where T : Enemy, new()
        {
            const int spawnDistance = 600;
            Vector2 spawnPosition = new Vector2(World.player.Position.X + Globals.Random.Next(0, spawnDistance), World.player.Position.Y + Globals.Random.Next(0, spawnDistance));

            T enemy = new T();
            enemy.SetPosition(spawnPosition);
            World.RegisterEnemy(enemy);
            return enemy;
        }

        public abstract void OnHit(Projectile projectile);
    }
}
