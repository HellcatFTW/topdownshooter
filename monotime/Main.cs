using TopDownShooter.Managers;

namespace TopDownShooter
{
	public class Main : Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Vector2 cameraPos;
		private const float movementSpeed = 3f;
		private Vector2 inputAxis { get => GameManager.inputManager.inputAxis; }

		public Main()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			graphics.PreferredBackBufferWidth = 1920;
			graphics.PreferredBackBufferHeight = 1080;
		}

		protected override void Initialize()
		{
			Globals.Content = Content;
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Globals.SpriteBatch = spriteBatch;
		}

		protected override void Update(GameTime gameTime)
		{
			ProcessInput();

			GameManager.Update();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.Black);
			


			spriteBatch.Begin();
			GameManager.Draw(cameraPos);
			spriteBatch.End();
			
			base.Draw(gameTime);
		}

		public void ProcessInput()
		{
			Vector2 tempVector = Vector2.Zero;

			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			tempVector -= new Vector2(inputAxis.X, -inputAxis.Y);

            cameraPos += tempVector.SafeNormalize(Vector2.Zero) * movementSpeed;
		}
	}
}
