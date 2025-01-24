
namespace TopDownShooter.Entity
{
    internal sealed class Projectiles
    {
    }

    public sealed class PlayerShell : Projectile
    {
        protected override float Speed { get; set; }
        protected override float Damage { get; set; }
        protected override bool IsHostile { get; set; }
        public PlayerShell() : base(Globals.Content.Load<Texture2D>("PlayerShell"))
        {
            Speed = 10f;
            Damage = 50f;
            IsHostile = false;
        }

        public override void Update()
        {
            position += velocity * Speed;

            base.Update();
        }
        public override void Draw()
        {
            base.Draw();
        }
    }

    public sealed class EnemyShell : Projectile
    {
        protected override float Speed { get; set; }
        protected override float Damage { get; set; }
        protected override bool IsHostile { get; set; }

        public EnemyShell() : base(Globals.Content.Load<Texture2D>("EnemyShell"))
        {
            Speed = 5f;
            Damage = 25f;
            IsHostile = true;
        }
        public override void Update()
        {
            position += velocity * Speed;

            base.Update();
        }
        public override void Draw()
        {
            base.Draw();
        }
    }
}
