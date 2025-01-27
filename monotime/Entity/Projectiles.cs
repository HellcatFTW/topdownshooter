
namespace TopDownShooter.Entity
{
    internal sealed class Projectiles
    {
    }

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
    }
}
