
namespace TopDownShooter.Entity
{
    public abstract class Projectile : Entity
    {
        protected Texture2D texture;
        protected abstract float Speed { get; set; }
        public abstract float Damage { get; }
        public abstract bool IsHostile { get; }
        protected float lifeTime = 10f;
        protected float lifeTimer = 0f;

        public Projectile(Texture2D texture)
        {
            this.texture = texture;
            hitBox = new HitBox(position, texture.Bounds, 0);
        }
        public override void Update()
        {
            lifeTimer += (float)Globals.gameTime.ElapsedGameTime.TotalSeconds;
            if (lifeTimer >= lifeTime)
            {
                isActive = false;
            }
            hitBox = new HitBox(position, texture.Bounds, rotation);
        }
        public override void Draw()
        {
            rotation = velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            Globals.SpriteBatch.Draw(texture, position - World.cameraPos, null, Color.White, rotation, origin, 1f, SpriteEffects.None, LayerDepths.Projectiles);
        }
        public static T NewProjectile<T>(Vector2 position, Vector2 velocity) where T : Projectile, new()
        {
            T projectile = new();
            projectile.position = position;
            projectile.velocity = velocity;

            World.RegisterProjectile(projectile);

            return projectile;
        }

        public virtual void Kill()
        {
            isActive = false;
        }
    }
}
