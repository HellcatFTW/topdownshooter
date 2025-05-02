using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDownShooter.Entities;

namespace TopDownShooter
{
    public class Effect
    {
        protected Texture2DAnimated animatedTexture;
        protected Vector2 position = new();
        protected float rotation;
        protected float scale;
        protected const float layerDepth = LayerDepths.Effects;
        public bool Ended { get => ended; }
        protected bool ended;
        protected Effect()
        { }
        public Effect(Vector2 position, float rotation, float scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
        public virtual void Update(GameTime gameTime)
        {
            animatedTexture.Update(gameTime);
            if (animatedTexture.Ended)
            {
                ended = true;
            }
        }
        public virtual void Draw()
        {
            animatedTexture.Draw(position - World.cameraPos, rotation, scale, layerDepth);
        }
        public static T NewEffect<T>(Vector2 position, float rotation, float scale, WeakReference<Entity> parentWeakRef = null) where T : Effect, new()
        {
            T effect;
            if (typeof(T) == typeof(ShellExplosion) && parentWeakRef != null)
            {
                effect = (T)(Effect)new ShellExplosion()
                {
                    position = position,
                    rotation = rotation,
                    scale = scale,
                    ParentWeakRef = parentWeakRef
                };
            }
            else
            {
                effect = new()
                {
                    position = position,
                    rotation = rotation,
                    scale = scale
                };
            }
            Main.instance.RegisterEffect(effect);

            return effect;
        }
    }
}
