using System;

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

        protected readonly float shootCooldown;
        protected float shootTimer = 0;
        private Vector2 DirectionToPlayer { get => (World.player.Position - position).SafeNormalize(Vector2.Zero); }
        protected override float MovementSpeed { get; set; } = 1;

        private const float movementSwitchCooldown = 6f;
        private float movementSwitchTimer = 0;
        private bool movementIsSet = false;
        private int movementMode;
        private int TotalMovementModes { get => Enum.GetValues(typeof(MovementModes)).Length; }
        public override float Health { get => health; }
        private float health = 100;

        public EnemyTank()
        {
            TankHullTexture = Globals.Content.Load<Texture2D>("TankHull");
            TankTurretTexture = Globals.Content.Load<Texture2D>("TankTurret");

            hitBox = new HitBox(position, TankHullTexture.Bounds, 0);

            shootCooldown = Globals.Random.Next(2, 6);
        }
        public override void Update()
        {
            if(health <= 0)
            {
                isActive = false;
                return;
            }

            AI();

            turretRotation = DirectionToPlayer.ToRotation() + MathHelper.PiOver2;
            hullRotation = velocity.ToRotation() + MathHelper.PiOver2;
            rotation = hullRotation;

            hitBox = new HitBox(position, TankHullTexture.Bounds, rotation);

            if (shootTimer > 0f)
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
        private void AI()
        {
            if (movementSwitchTimer >= 0)
            {
                movementSwitchTimer -= (float)Globals.gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (movementSwitchTimer < 0)
            {
                movementMode = Globals.Random.Next(0, TotalMovementModes);
                movementIsSet = false;
                movementSwitchTimer = movementSwitchCooldown;
            }

            switch (movementMode)
            {
                case ((int)MovementModes.None):
                    //do nothing?
                    velocity = Vector2.Zero;
                    break;
                case ((int)MovementModes.ShowingSideToPlayer):
                    // set movement 90 degrees off directionToPlayer for one frame, dont update
                    if (!movementIsSet)
                    {
                        velocity = Vector2.Zero;
                        velocity += DirectionToPlayer.RotatedBy(MathHelper.PiOver2);
                        movementIsSet = true;
                    }

                    break;
                case ((int)MovementModes.TowardPlayer):
                    // offset some +-45 degrees from direction to player, dont update
                    if (!movementIsSet)
                    {
                        velocity = Vector2.Zero;
                        velocity += DirectionToPlayer.RotatedBy(Globals.Random.Next(0, 2) == 0 ? MathHelper.PiOver4 : -MathHelper.PiOver4);
                        movementIsSet = true;
                    }

                    break;
                default:
                    throw new Exception("the movement code!! oh the horror");
            }

            position += velocity.SafeNormalize(Vector2.Zero) * MovementSpeed;
        }

        public override void OnHit(Projectile projectile)
        {
            health -= projectile.Damage;
        }
    }
    public enum MovementModes : int
    {
        None = 0,
        ShowingSideToPlayer = 1,
        TowardPlayer = 2,
    }
}
