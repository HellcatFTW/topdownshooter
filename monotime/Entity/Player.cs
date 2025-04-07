using Microsoft.Xna.Framework;
using System;

namespace TopDownShooter.Entity
{
    public sealed class Player : Entity, IHealth
    {
        private readonly Texture2D TankHullTexture;
        private readonly Texture2D TankTurretTexture;
        private readonly float turretOriginOffset;
        private readonly float turretPositionOffset;

        private float turretRotation = 0f;
        private float hullRotation = 0f;

        private const float movementSpeed = 3f;
        private const float turnRate = .03f;

        private const float shootCooldown = 1.2f;
        private float shootTimer = 0;

        private Vector2 DirectionToMouse { get => (World.MouseWorld - position).SafeNormalize(Vector2.Zero); }
        public float Health { get => health; }
        private float health = 100f;

        public Player()
        {
            TankHullTexture = Globals.Content.Load<Texture2D>("TankHull");
            TankTurretTexture = Globals.Content.Load<Texture2D>("TankTurret");
            turretOriginOffset = 9f;

            position = Vector2.Zero + new Vector2(World.map.Width,World.map.Height) / 2;
            World.cameraPos = position - new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2);
            rotation = MathHelper.PiOver2;
            hitBox = new HitBox(position, TankHullTexture.Bounds, 0);
        }
        public override void Update()
        {
            if(Health <= 0)
            {
                isActive = false;
            }

            ClearNoPushList();

            MoveAndTurn();

            hitBox.Value.SetHitboxRotation(rotation);

            if (shootTimer > 0f)
            {
                shootTimer -= (float)Globals.gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Input.MouseLeft && shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootCooldown;
            }
        }
        public override void Draw()
        {
            Vector2 turretOrigin = new Vector2(TankTurretTexture.Width / 2, TankTurretTexture.Height / 2 + turretOriginOffset); // 9 pixel offset to accommodate for the barrel and have proper rotation
            Vector2 hullOrigin = new Vector2(TankHullTexture.Width / 2, TankTurretTexture.Height / 2);

            Globals.SpriteBatch.Draw(TankHullTexture, position - World.cameraPos, null, Color.White, hullRotation, hullOrigin, 1, SpriteEffects.None, LayerDepths.Entities);

            Globals.SpriteBatch.Draw(TankTurretTexture, position - World.cameraPos, null, Color.White, turretRotation, turretOrigin, 1, SpriteEffects.None, LayerDepths.Entities);
            if (World.debugMode && hitBox != null)
            {
                Utils.DrawHitbox(hitBox.Value);
            }
        }
        public override void MoveBy(Vector2 vector)
        {
            base.MoveBy(vector);
            World.cameraPos = position - new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2);
        }
        public void MoveAndTurn()
        {
            rotation += Input.GetAxis.Y >= 0 ? Input.GetAxis.X * turnRate : Input.GetAxis.X * -turnRate;
            hullRotation = rotation - MathHelper.PiOver2;
            turretRotation = DirectionToMouse.ToRotation() + MathHelper.PiOver2;

            velocity = Vector2.Zero;
            if (Input.GetAxis.Y > 0)
            {
                velocity += rotation.ToRotationVector2().WithRotation(MathHelper.Pi) * movementSpeed;
            }
            if (Input.GetAxis.Y < 0)
            { 
                velocity += rotation.ToRotationVector2() * movementSpeed;
            }
            MoveBy(velocity);
        }
        public void Shoot()
        {
            Projectile.NewProjectile<PlayerShell>(position + DirectionToMouse * 50f, DirectionToMouse);
        }

        public void OnHit(Projectile projectile)
        {
            health -= projectile.Damage;
        }
    }
}
