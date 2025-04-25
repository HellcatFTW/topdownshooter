using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TopDownShooter
{
    public sealed class Main : Game
    {
        public static Main instance { get; private set; }
        private GraphicsDeviceManager graphics;
        private SafeSpriteBatch spriteBatch;
        private HashSet<Action> drawCalls = new();
        private HashSet<Effect> Effects = new();

        public Main()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
        }

        protected override void Initialize()
        {
            Globals.graphics = graphics;
            Globals.Content = Content;
            Globals.ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Globals.ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            UI.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SafeSpriteBatch(new SpriteBatch(GraphicsDevice));
            Globals.SpriteBatch = spriteBatch;
        }

        protected override void Update(GameTime gameTime)
        {
            ProcessInput();

            Globals.gameTime = gameTime;

            foreach (Effect effect in Effects)
            {
                effect.Update(gameTime);
                if (effect.Ended)
                { 
                    Effects.Remove(effect);
                }
            }

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
            foreach (Effect effect in Effects)
            {
                effect.Draw();
            }
            UI.Draw();
            foreach (Action drawCall in drawCalls)
            { 
                drawCall.Invoke();
                drawCalls.Remove(drawCall);
            }
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
        public void EnqueueDraw(Action drawCall)
        {
            drawCalls.Add(drawCall);
        }
        public void RegisterEffect(Effect effect)
        { 
            Effects.Add(effect);
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
        public const float Effects = 0.3f;
        public const float UI = .4f;
    }
}
