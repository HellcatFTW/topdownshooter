
namespace TopDownShooter.Managers
{
    public class InputManager
    {
        public Vector2 inputAxis;
        public void Update()
        {
            inputAxis = Vector2.Zero;
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.W))
            {
                inputAxis.Y += 1;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                inputAxis.X -= 1;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                inputAxis.Y -= 1;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                inputAxis.X += 1;
            }
        }
    }
}
