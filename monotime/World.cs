using TopDownShooter.Entity;
using TopDownShooter.Level;
using System.Collections.Generic;

namespace TopDownShooter
{
    public static class World
    {
        public static readonly Map map = new();
        public static readonly Player player = new();

        public static Vector2 cameraPos;

        public static Vector2 mouseWorld;

        public static List<Projectile> projectiles = new List<Projectile>();
        public static void Update()
        {
            MouseState mouse = Mouse.GetState();

            mouseWorld = mouse.Position.ToVector2() + cameraPos;

            player.Update();

            UpdateProjectiles();
            RemoveProjectiles();
        }
        public static void Draw()
        {
            map.Draw(cameraPos);
            player.Draw();
            DrawProjectiles();
        }
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
    }
}
