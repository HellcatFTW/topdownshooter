﻿namespace TopDownShooter.Entities
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
        public static T NewEnemy<T>() where T : Enemy, new()
        {
            Vector2 spawnPosition = World.map.GenerateSpawn();
            T enemy = new T();
            enemy.SetPosition(spawnPosition);
            World.RegisterEnemy(enemy);
            return enemy;
        }

        public abstract void OnHit(Projectile projectile);
    }
}
