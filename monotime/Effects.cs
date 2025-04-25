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
}
