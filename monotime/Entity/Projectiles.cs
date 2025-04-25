
using System.Net.NetworkInformation;

namespace TopDownShooter.Entity
{
    public sealed class PlayerShell : Projectile
    {
        protected override float Speed { get; set; }
        public override float Damage { get => damage; }
        private float damage;
        public override bool IsHostile { get => isHostile; }
        private bool isHostile;
        private Texture2DAnimated impactFX;
        public PlayerShell() : base(Globals.Content.Load<Texture2D>("PlayerShell"))
        {
            Speed = 10f;
            damage = 50f;
            isHostile = false;
            impactFX = new Texture2DAnimated(Globals.Content.Load<Texture2D>("ShellImpactFX"), 4, 10, false);
        }

        public override void Update()
        {
            position += velocity * Speed;

            base.Update();
        }
        public override void Kill(Vector2 impactNormal, Vector2 startPoint, bool FlipMTVWhenDrawing)
        {
            float impactFXRotation = impactNormal.ToRotation() + MathHelper.PiOver2 + (FlipMTVWhenDrawing ? 0 : MathHelper.Pi);
            Vector2 origin = new Vector2(impactFX.Width / 2, impactFX.Height / 2);
            Effect.NewEffect<ShellExplosion>(startPoint - World.cameraPos, impactFXRotation, 1f);
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
        private Texture2D impactFX;
        public EnemyShell() : base(Globals.Content.Load<Texture2D>("EnemyShell"))
        {
            Speed = 5f;
            damage = 25f;
            isHostile = true;
            impactFX = Globals.Content.Load<Texture2D>("ShellImpactFX");
        }
        public override void Update()
        {
            position += velocity * Speed;

            base.Update();
        }
        public override void Kill(Vector2 impactNormal, Vector2 startPoint, bool FlipMTVWhenDrawing)
        {
            float impactFXRotation = impactNormal.ToRotation() + MathHelper.PiOver2 + (FlipMTVWhenDrawing ? 0 : MathHelper.Pi);
            Vector2 origin = new Vector2(impactFX.Width / 2, impactFX.Height / 2);
            Effect.NewEffect<ShellExplosion>(startPoint - World.cameraPos, impactFXRotation, 1f);
            base.Kill(impactNormal, startPoint, FlipMTVWhenDrawing);
        }
    }
}
