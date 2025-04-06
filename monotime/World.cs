using System;
using System.Collections.Generic;
using TopDownShooter.Entity;
using TopDownShooter.Level;

namespace TopDownShooter
{
    public static class World
    {
        public static int level = 1;
        public static TileMap map;
        public static Player player;

        public static Vector2 cameraPos;

        private static Vector2 mouseWorld;
        private static Vector2 mouseScreen;
        public static Vector2 MouseWorld { get => mouseWorld; }
        public static Vector2 MouseScreen { get => mouseScreen; }

        private static float enemySpawnCooldown = 5f;
        private static float enemySpawnTimer = 0f;
        private const int globalEnemySpawnLimit = 5;

        public static bool debugMode = false;
        private static bool paused = false;
        public static bool Paused { get { return paused; } }

        private static List<Projectile> projectiles = new List<Projectile>();
        private static List<Enemy> enemies = new List<Enemy>();
        public static void Update()
        {
            MouseState mouse = Mouse.GetState();

            mouseWorld = mouse.Position.ToVector2() + cameraPos;
            mouseScreen = mouse.Position.ToVector2();

            if (player is null || map is null || paused)
            {
                return;
            }

            player.Update();

            SpawnEnemies();

            UpdateProjectiles();
            UpdateEnemies();

            ProcessCollisions();

            RemoveProjectiles();
            RemoveEnemies();
        }
        public static void Draw()
        {
            if (player is null || map is null)
            {
                return;
            }
            map.Draw();
            player.Draw();
            DrawEnemies();
            DrawProjectiles();
        }
        public static void ChangeLevel(int newLevel)
        {
            level = newLevel;
            map = new TileMap();
            player = new Player();
        }
        public static void Pause()
        {
            paused = true;
        }
        public static void Resume()
        {
            paused = false;
        }
        public static void Reset()
        {
            paused = false;
            map = null;
            player = null;
            projectiles = new();
            enemies = new();
        }
        private static void SpawnEnemies()
        {
            if (enemySpawnTimer > 0f)
            {
                enemySpawnTimer -= (float)Globals.gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (enemySpawnTimer <= 0f && enemies.Count < globalEnemySpawnLimit)
            {
                Enemy.NewEnemy<EnemyTank>();
                enemySpawnTimer = enemySpawnCooldown;
            }
        }
        #region Collisions
        public static void ProcessCollisions()
        {
            ProcessPlayerShootingEnemy();
            ProcessEnemyShootingPlayer();

            for (int i = 0; i < 8; i++)
            {
                ProcessProjectileToWallCollisions();
                ProcessPlayerToWallCollisions();
                ProcessEnemyToWallCollisions();


                ProcessEnemyToEnemyCollisions(i);
                ProcessPlayerToEnemyCollisions();
            }
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

                if (enemy.noPushList.Count > 0)
                {
                    Vector2? newmtv = null;
                    Vector2 adjustedMTV = Vector2.Zero;

                    foreach (Vector2 noPushVec in enemy.noPushList)
                    {
                        Vector2 reject = Utils.Reject(noPushVec, mtv.Value);
                        newmtv = HitBox.MinimumTranslationVector(enemy.Hitbox.Value, player.Hitbox.Value, out _, out _);
                        adjustedMTV += reject;
                    }
                    if (!Utils.IsZero(adjustedMTV))
                    {
                        enemy.MoveBy(adjustedMTV);
                        if (newmtv != null)
                        {
                            player.MoveBy(newmtv.Value);
                        }
                    }
                    else
                    {
                        player.MoveBy(-mtv.Value);
                    }
                }
                else
                {
                    enemy.MoveBy(mtv.Value);
                }
            }
        }
        public static void ProcessEnemyToEnemyCollisions(int currentStep)
        {
            int start = 0;
            int end = enemies.Count;
            int step = 1;

            int innerStart = enemies.Count - 1;
            int innerEnd = -1;
            int step2 = -1;

            if (currentStep % 2 == 0)
            {
                start = enemies.Count - 1;
                end = -1;
                step = -1;

                innerStart = 0;
                innerEnd = enemies.Count;
                step2 = 1;
            }

            for (int i = start; i != end; i+=step)
            {
                for (int j = innerStart; j != innerEnd; j+=step2)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    var enemy = enemies[i];
                    var other = enemies[j];

                    if (enemy.Hitbox == null)
                    {
                        continue;
                    }
                    if (other.Hitbox == null)
                    {
                        continue;
                    }
                    Vector2? mtv = HitBox.MinimumTranslationVector(enemy.Hitbox.Value, other.Hitbox.Value, out _, out _);

                    if (mtv == null)
                    {
                        continue;
                    }


                    if (other.noPushList.Count > 0)
                    {
                        Vector2? newmtv = null;
                        Vector2 adjustedMTV = Vector2.Zero;

                        foreach (Vector2 noPushVec in other.noPushList)
                        {
                            Vector2 reject = Utils.Reject(noPushVec, mtv.Value);
                            newmtv = HitBox.MinimumTranslationVector(other.Hitbox.Value, enemy.Hitbox.Value, out _, out _);
                            adjustedMTV += reject;
                            if (Utils.IsZero(reject))
                            {
                                enemy.noPushList.Add(noPushVec);
                            }
                        }
                        if (!Utils.IsZero(adjustedMTV))
                        {
                            other.MoveBy(adjustedMTV);
                            if (newmtv != null)
                            {
                                enemy.MoveBy(newmtv.Value);
                            }
                        }
                        else
                        {
                            enemy.MoveBy(-mtv.Value);
                        }
                    }
                    else
                    {
                        enemy.MoveBy(-mtv.Value / 2);
                        other.MoveBy(mtv.Value / 2);
                    }
                }
            }
        }
        public static void ProcessPlayerToWallCollisions()
        {
            if (player.Hitbox == null)
            {
                return;
            }
            foreach (HitBox tileHitbox in map.GetNearestHitboxes(player.Position))
            {
                Vector2? mtv = HitBox.MinimumTranslationVector(tileHitbox, player.Hitbox.Value, out _, out _);
                if (mtv == null)
                {
                    continue;
                }
                player.MoveBy(mtv.Value);
            }
        }
        public static void ProcessProjectileToWallCollisions()
        {
            foreach (Projectile projectile in projectiles)
            {
                foreach (HitBox tileHitbox in map.GetNearestHitboxes(projectile.Position))
                {
                    if (projectile.Hitbox == null)
                    {
                        continue;
                    }
                    if (HitBox.Intersect(tileHitbox, projectile.Hitbox.Value))
                    {
                        projectile.Kill();
                    }
                }
            }
        }

        public static void ProcessEnemyToWallCollisions()
        {
            foreach (Enemy enemy in enemies)
            {
                foreach (HitBox tileHitbox in map.GetNearestHitboxes(enemy.Position))
                {
                    if (enemy.Hitbox == null)
                    {
                        continue;
                    }
                    Vector2? mtv = HitBox.MinimumTranslationVector(tileHitbox, enemy.Hitbox.Value, out _, out _);
                    if (mtv == null)
                    {
                        continue;
                    }
                    enemy.MoveBy(mtv.Value);
                    enemy.noPushList.Add(mtv.Value);
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
