
namespace TopDownShooter.Entity
{
    internal sealed class Enemies
    {
    }

    public class EnemyTank : Enemy
    {
        private readonly Texture2D TankHullTexture;
        private readonly Texture2D TankTurretTexture;

        protected float turretRotation = 0f;
        protected float hullRotation = 0f;

        protected const float shootCooldown = 2f;
        protected float shootTimer = 0;
        private Vector2 DirectionToPlayer { get => (World.player.Position - position).SafeNormalize(Vector2.Zero); }
        protected override float MovementSpeed { get; set; } = 1;

        public EnemyTank()
        {
            TankHullTexture = Globals.Content.Load<Texture2D>("TankHull");
            TankTurretTexture = Globals.Content.Load<Texture2D>("TankTurret");
        }
        public override void Update()
        {
            //TODO: - Enemies should choose some direction to move in relative to the player that is not *exactly* the opposite way.
            //      - movement should be in 4-6 second long periods. 
            //      - preferably switch movement directions, don't use same one back to back.

            Vector2 velocity = Vector2.Zero;
            velocity += DirectionToPlayer;
            position += velocity.SafeNormalize(Vector2.Zero) * MovementSpeed;

            turretRotation = DirectionToPlayer.ToRotation() + MathHelper.PiOver2;
            hullRotation = velocity.ToRotation() + MathHelper.PiOver2;

            if (shootTimer >= 0f)
            {
                shootTimer -= (float)Globals.gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootCooldown;
            }
        }
        public override void Draw()
        {
            Vector2 turretOrigin = new Vector2(TankTurretTexture.Width / 2, TankTurretTexture.Height / 2 + 9f); // 9 pixel offset to accommodate for the barrel and have proper rotation
            Vector2 hullOrigin = new Vector2(TankHullTexture.Width / 2, TankTurretTexture.Height / 2);

            Globals.SpriteBatch.Draw(TankHullTexture, position - World.cameraPos, null, Color.White, hullRotation, hullOrigin, 1, SpriteEffects.None, LayerDepths.Entities);

            Globals.SpriteBatch.Draw(TankTurretTexture, position - World.cameraPos, null, Color.White, turretRotation, turretOrigin, 1, SpriteEffects.None, LayerDepths.Entities);
        }
        public override void Shoot()
        {
            Vector2 directionToPlayer = (World.player.Position - position).SafeNormalize(Vector2.Zero);

            Projectile.NewProjectile<EnemyShell>(position + directionToPlayer * 50f, directionToPlayer);
        }
    }
}
