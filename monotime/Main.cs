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
            spriteBatch = new SafeSpriteBatch(new SpriteBatch(GraphicsDevice));
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
    public class SafeSpriteBatch
    {
        private readonly SpriteBatch spriteBatch;
        private bool beginCalled;

        public SafeSpriteBatch(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }
        public void Begin()
        {
            beginCalled = true;
            spriteBatch.Begin();
        }
        public void End()
        {
            beginCalled = false;
            spriteBatch.End();
        }
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(spriteFont, text, position, color);
        }
        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            if (beginCalled)
            {
                spriteBatch.Draw(texture, position, color);
            }
            else
            {
                Main.instance.EnqueueDraw(() => spriteBatch.Draw(texture, position, color));
            }
        }
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            if (beginCalled)
            {
                spriteBatch.Draw(texture, destinationRectangle, color);
            }
            else
            {
                Main.instance.EnqueueDraw(() => spriteBatch.Draw(texture, destinationRectangle, color));
            }
        }
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            if (beginCalled)
            {
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
            }
            else
            {
                Main.instance.EnqueueDraw(() => spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color));
            }
        }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            if (beginCalled)
            {
                spriteBatch.Draw(texture, position, sourceRectangle, color);
            }
            else
            {
                Main.instance.EnqueueDraw(() => spriteBatch.Draw(texture, position, sourceRectangle, color));
            }
        }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            if (beginCalled)
            {
                spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
            }
            else
            {
                Main.instance.EnqueueDraw(() => spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth));
            }
        }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            if (beginCalled)
            {
                spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
            }
            else
            {
                Main.instance.EnqueueDraw(() => spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth));
            }
        }
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            if (beginCalled)
            {
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
            }
            else
            {
                Main.instance.EnqueueDraw(() => spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth));
            }
        }
    }
}
