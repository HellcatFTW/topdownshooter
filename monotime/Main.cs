using System;
using System.Linq;

namespace TopDownShooter
{
    public sealed class Main : Game
    {
        public static Main instance { get; private set; }
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public Main()
        {
            instance = this;
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
                switch (UI.ActiveLayout)
                {
                    case LayoutIndex.MainMenu:
                        Exit();
                        break;
                    case LayoutIndex.LevelSelect:
                        UI.SwitchToMainMenu(null, null);
                        break;
                    case LayoutIndex.HUD:
                        UI.SwitchToPauseMenu(null, null);
                        break;
                }
            }
        }
        public void ExitWrapper(object sender, EventArgs e)
        {
            Exit();
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
