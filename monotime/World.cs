using System;
using System.Collections.Generic;
using TopDownShooter.Entity;
using TopDownShooter.Level;

namespace TopDownShooter
{
    public static class World
    {
        public static readonly Map map = new();
        public static readonly Player player = new();

        public static Vector2 cameraPos;

        private static Vector2 mouseWorld;
        public static Vector2 MouseWorld { get => mouseWorld; }

        private static float enemySpawnCooldown = 5f;
        private static float enemySpawnTimer = 0f;
        private const int globalEnemySpawnLimit = 5;

        public static bool DebugMode = true;

        private static List<Projectile> projectiles = new List<Projectile>();
        private static List<Enemy> enemies = new List<Enemy>();
        public static void Update()
        {
            MouseState mouse = Mouse.GetState();

            mouseWorld = mouse.Position.ToVector2() + cameraPos;

            player.Update();


            ////TODO: implement spawn chances of different enemies
            if (enemySpawnTimer > 0f)
            {
                enemySpawnTimer -= (float)Globals.gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (enemySpawnTimer <= 0f && enemies.Count < globalEnemySpawnLimit)
            {
                Enemy.NewEnemy<EnemyTank>();
                enemySpawnTimer = enemySpawnCooldown;
            }


            UpdateProjectiles();
            UpdateEnemies();

            ProcessCollisions();

            RemoveProjectiles();
            RemoveEnemies();
        }
        public static void Draw()
        {
            map.Draw(cameraPos);
            player.Draw();
            DrawEnemies();
            DrawProjectiles();
        }

        #region Collisions
        public static void ProcessCollisions()
        {
            ProcessPlayerShootingEnemy();
            ProcessEnemyShootingPlayer();
            ProcessPlayerToEnemyCollisions();
            ProcessEnemyToEnemyCollisions();
        }

        public static void ProcessPlayerShootingEnemy()
        {
            foreach (Enemy enemy in enemies)
            {
                foreach (Projectile projectile in projectiles)
                {
                    if (projectile.IsHostile)
                    {
                        continue;
                    }
                    if (enemy.CheckCollisionWith(projectile))
                    {
                        enemy.OnHit(projectile);
                        projectile.Kill();
                    }
                }
            }
        }
        public static void ProcessEnemyShootingPlayer()
        {
            foreach (Projectile projectile in projectiles)
            {
                if (!projectile.IsHostile)
                {
                    continue;
                }
                if (player.CheckCollisionWith(projectile))
                {
                    player.OnHit(projectile);
                    projectile.Kill();
                }
            }
        }

        public static void ProcessPlayerToEnemyCollisions()
        {
            if (player.Hitbox == null)
            {
                return;
            }
            foreach (Enemy enemy in enemies)
            {
                if (enemy.Hitbox == null)
                {
                    continue;
                }

                Vector2? mtv = HitBox.MinimumTranslationVector(player.Hitbox.Value, enemy.Hitbox.Value, out _, out _);

                if (mtv == null)
                {
                    continue;
                }

                enemy.MoveBy(mtv.Value);
            }
        }
        public static void ProcessEnemyToEnemyCollisions()
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.Hitbox == null)
                { 
                    continue;
                }
                foreach (Enemy otherEnemy in enemies)
                {
                    if (enemy == otherEnemy)
                    {
                        continue;
                    }
                    if (otherEnemy.Hitbox == null)
                    {
                        continue;
                    }
                    Vector2? mtv = HitBox.MinimumTranslationVector(enemy.Hitbox.Value, otherEnemy.Hitbox.Value, out _, out _);
                    if (mtv == null)
                    {
                        continue;
                    }

                    otherEnemy.MoveBy(mtv.Value);
                }
            }
        }
        #endregion

        #region Enemy logic
        public static void RegisterEnemy(Enemy enemy)
        {
            enemies.Add(enemy);
        }
        private static void UpdateEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update();
            }
        }
        private static void DrawEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw();
            }
        }
        private static void RemoveEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].IsActive)
                {
                    enemies.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Projectile logic
        public static void RegisterProjectile(Projectile projectile)
        {
            projectiles.Add(projectile);
        }
        private static void UpdateProjectiles()
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Update();
            }
        }
        private static void DrawProjectiles()
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw();
            }
        }
        private static void RemoveProjectiles()
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (!projectiles[i].IsActive)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}
