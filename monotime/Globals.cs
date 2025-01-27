
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace TopDownShooter
{
    public static class Globals
    {
        public static GameTime gameTime { get; set; }
        public static GraphicsDeviceManager graphics {  get; set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static Random Random { get => random; }
        private static Random random = new();
    }
}
