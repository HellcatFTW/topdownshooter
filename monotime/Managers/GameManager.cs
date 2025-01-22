using TopDownShooter.World;

namespace TopDownShooter.Managers
{
    public static class GameManager
    {
        public static readonly Map map = new();
        public static readonly Player player = new();
        public static readonly InputManager inputManager = new();
        public static void Update()
        {
            player.Update();
            inputManager.Update();
        }
        public static void Draw(Vector2 cameraPos)
        {
            map.Draw(cameraPos);
            player.Draw();
        }
    }
}
