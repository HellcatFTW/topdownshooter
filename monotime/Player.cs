
using TopDownShooter.Managers;

namespace TopDownShooter
{
	public class Player
	{
		private Vector2 position;

		private readonly Texture2D TankHull;
        private readonly Texture2D TankTurret;

		private Vector2 directionToMouse;
		private float turretRotation = 0f;
		private float hullRotation = 0f;

        private Vector2 inputAxis { get => GameManager.inputManager.inputAxis; }

        public Player()
		{
            TankHull = Globals.Content.Load<Texture2D>("TankHull");
            TankTurret = Globals.Content.Load<Texture2D>("TankTurret");

			position = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2);
        }
		public void Update()
		{
        }
		public void Draw()
		{
            MouseState mouse = Mouse.GetState();

            if (inputAxis != Vector2.Zero)
            {
                hullRotation = (float)System.Math.Atan2(inputAxis.Y, -inputAxis.X) - MathHelper.PiOver2;
            }

            Vector2 turretOrigin = new Vector2(TankTurret.Width / 2, TankTurret.Height / 2 + 9f); // 9 pixel offset to accommodate for the barrel
            Vector2 hullOrigin = new Vector2(TankHull.Width / 2, TankTurret.Height / 2);

            directionToMouse = new Vector2(mouse.X - position.X, mouse.Y - position.Y);
            turretRotation = (float)System.Math.Atan2(directionToMouse.Y, directionToMouse.X);
            turretRotation += MathHelper.PiOver2;
			

            Globals.SpriteBatch.Draw(TankHull, position, null, Color.White, hullRotation, hullOrigin, 1, SpriteEffects.None, 0);

			Globals.SpriteBatch.Draw(TankTurret, position, null, Color.White, turretRotation, turretOrigin, 1, SpriteEffects.None, 0);
		}
	}
}
