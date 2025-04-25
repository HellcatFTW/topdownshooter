using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace TopDownShooter
{
    /// <summary>
    /// Horizontal spritesheet with 1px spacing between frames
    /// </summary>
    public class Texture2DAnimated
    {
        private int frameCount;

        private Texture2D sheet;

        private float totalElapsedTime;

        private float fps;

        private int frame;

        private const int frameSpacing = 1;
        private int frameWidth { get { return (sheet.Width / frameCount) - (frameCount - frameSpacing); } }

        private bool loop;
        private float timePerFrame { get => (float)1 / fps;  }
        public bool Ended { get => ended; }
        private bool ended;

        public Rectangle CurrentFrameRect { get { return CalculateFrameRect(); } }
        public float Width { get => frameWidth; }
        public float Height { get => sheet.Height;  }

        public Texture2DAnimated(Texture2D sheet, int frameCount, float fps, bool loop)
        {
            this.sheet = sheet;
            this.frameCount = frameCount;
            this.fps = fps;
        }
        public static implicit operator Texture2D(Texture2DAnimated animatedTexture)
        {
            return animatedTexture.sheet;
        }
        private Rectangle CalculateFrameRect()
        {
            int frameWidth = (sheet.Width - (frameCount * frameSpacing)) / frameCount;
            Rectangle sourcerect = new Rectangle(frameWidth * frame + (frame * frameSpacing), 0, frameWidth, sheet.Height);
            return sourcerect;
        }
        public void Update(GameTime gameTime)
        {
            totalElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (totalElapsedTime > timePerFrame)
            {
                frame++;
                if (loop)
                {
                    if (frame == frameCount - 1)
                    {
                        frame = 0;
                    }
                }
                else
                {
                    if (frame == frameCount - 1)
                    {
                        frame = 0;
                        ended = true;
                        return;
                    }
                }
                totalElapsedTime -= timePerFrame;
            }
        }
        public void Draw(Vector2 position, float rotation, float scale, float layerDepth)
        {
            Globals.SpriteBatch.Draw(sheet, position, CurrentFrameRect, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }
    }
}
