using System;
using TopDownShooter.Entities;

namespace TopDownShooter
{
    public sealed class ShellExplosion : Effect
    {
        private readonly Entity parent;
        private readonly Vector2 parentOffset;
        public ShellExplosion(Vector2 position, float rotation, float scale, WeakReference<Entity> parentWeakRef)
        {
            animatedTexture = new(Globals.Content.Load<Texture2D>("ShellImpactFX"), 4, 30, false);
            parentWeakRef.TryGetTarget(out parent);
            if (parent != null)
            {
                parentOffset = position - parent.Position;
            }
            this.position = parent.Position + parentOffset;
            this.rotation = rotation;
            this.scale = scale;
        }
        public override void Update(GameTime gameTime)
        {
            if (parent != null)
            {
                position = parent.Position + parentOffset;
            }

            animatedTexture.Update(gameTime);
            if (animatedTexture.Ended)
            {
                ended = true;
            }
        }
        public override void Draw()
        {
            animatedTexture.Draw(position - World.cameraPos, rotation, scale, layerDepth);
        }
    }
    public sealed class TankExplosion : Effect
    {
        private Vector2 origin;
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
