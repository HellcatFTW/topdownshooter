using System;
using TopDownShooter.Entities;

namespace TopDownShooter
{
    public sealed class ShellExplosion : Effect
    {
        private Func<Vector2> calculatePosition;
        private WeakReference<Entity> parentWeakRef;
        public WeakReference<Entity> ParentWeakRef {
            get => parentWeakRef;
            set
            {
                parentWeakRef = value;
                if (parentWeakRef.TryGetTarget(out Entity parent))
                {
                    Vector2 parentOffset = position - parent.Position;
                    calculatePosition = () => parent.Position + parentOffset;
                }
                else
                {
                    calculatePosition = () => position;
                }
            } 
        }
        public ShellExplosion()
        {
            animatedTexture = new(Globals.Content.Load<Texture2D>("nuke"), 9, 30, false);
        }
        public override void Update(GameTime gameTime)
        {
            position = calculatePosition();
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
