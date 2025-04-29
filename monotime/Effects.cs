using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownShooter
{
    public sealed class ShellExplosion : Effect
    {
        public ShellExplosion()
        {
            animatedTexture = new(Globals.Content.Load<Texture2D>("ShellImpactFX"), 4, 30, false);
        }
    }
    public sealed class TankExplosion : Effect
    {
        Vector2 origin;
        public TankExplosion()
        {
            animatedTexture = new(Globals.Content.Load<Texture2D>("Explosion"), 7, 24, false);
            origin = new Vector2(animatedTexture.Width, animatedTexture.Height);
        }

        public override void Draw()
        {
            animatedTexture.Draw(position - World.cameraPos, rotation, origin, scale, layerDepth);
        }
    }
}
