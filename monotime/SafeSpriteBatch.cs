using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownShooter
{
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
            if (beginCalled)
            {
                spriteBatch.DrawString(spriteFont, text, position, color);
            }
            else
            {
                Main.instance.EnqueueDraw(() => spriteBatch.DrawString(spriteFont, text, position, color));
            }
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
