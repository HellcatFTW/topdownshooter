using TopDownShooter.Entity;
using TopDownShooter.Level;
using System.Collections.Generic;
using System;

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

        private static List<Projectile> projectiles = new List<Projectile>();
        private static List<Enemy> enemies = new List<Enemy>();
        public static void Update()
        {
            MouseState mouse = Mouse.GetState();

            mouseWorld = mouse.Position.ToVector2() + cameraPos;

            player.Update();


            //TODO: implement spawn chances
            if (enemySpawnTimer > 0f)
            {
                enemySpawnTimer -= (float)Globals.gameTime.ElapsedGameTime.TotalSeconds;
            }
            if(enemySpawnTimer <= 0f)
            {
                Enemy.NewEnemy<EnemyTank>();
                enemySpawnTimer = enemySpawnCooldown;
            }



            UpdateProjectiles();
            UpdateEnemies(); 
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
                if (!enemies[i].isActive)
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
                if (!projectiles[i].isActive)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}
