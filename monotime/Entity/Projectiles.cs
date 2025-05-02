
using System;
using System.Net.NetworkInformation;

namespace TopDownShooter.Entities
{
    public sealed class PlayerShell : Projectile
    {
        protected override float Speed { get; set; }
        public override float Damage { get => damage; }
        private float damage;
        public override bool IsHostile { get => isHostile; }
        private bool isHostile;
        public PlayerShell() : base(Globals.Content.Load<Texture2D>("PlayerShell"))
        {
            Speed = 10f;
            damage = 50f;
            isHostile = false;
        }

        public override void Update()
        {
            position += velocity * Speed;

            base.Update();
        }
        public override void Kill(Vector2 impactNormal, Vector2 startPoint, bool FlipMTVWhenDrawing, WeakReference<Entity> parentWeakRef)
        {
            float impactFXRotation = impactNormal.ToRotation() + MathHelper.PiOver2 + (FlipMTVWhenDrawing ? 0 : MathHelper.Pi);
            Effect.NewEffect<ShellExplosion>(startPoint, impactFXRotation, 1f, parentWeakRef);
            base.Kill(impactNormal, startPoint, FlipMTVWhenDrawing);
        }
    }

    public sealed class EnemyShell : Projectile
    {
        protected override float Speed { get; set; }
        public override float Damage { get => damage; }
        private float damage;
        public override bool IsHostile { get => isHostile; }
        private bool isHostile;
        public EnemyShell() : base(Globals.Content.Load<Texture2D>("EnemyShell"))
        {
            Speed = 5f;
            damage = 25f;
            isHostile = true;
        }
        public override void Update()
        {
            position += velocity * Speed;

            base.Update();
        }
        public override void Kill(Vector2 impactNormal, Vector2 startPoint, bool FlipMTVWhenDrawing, WeakReference<Entity> parentWeakRef)
        {
            float impactFXRotation = impactNormal.ToRotation() + MathHelper.PiOver2 + (FlipMTVWhenDrawing ? 0 : MathHelper.Pi);
            Effect.NewEffect<ShellExplosion>(startPoint, impactFXRotation, 1f, parentWeakRef);
            base.Kill(impactNormal, startPoint, FlipMTVWhenDrawing);
        }
    }
}
