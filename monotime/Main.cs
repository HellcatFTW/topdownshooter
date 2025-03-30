using System;

namespace TopDownShooter
{
    public sealed class Main : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Globals.graphics = graphics;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
        }

        protected override void Initialize()
        {
            Globals.Content = Content;
            Globals.ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Globals.ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            UI.Initialize();
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

            Globals.gameTime = gameTime;

            Input.Update();
            World.Update();
            UI.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            World.Draw();
            UI.Draw();
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void ProcessInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }
    }
    public static class LayerDepths
    {
        public const float Background = 0f;
        public const float Entities = .1f;
        public const float Projectiles = .2f;
        public const float UI = .3f;
    }
}
