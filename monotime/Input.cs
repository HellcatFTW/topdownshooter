namespace TopDownShooter
{
    public static class Input
    {
        public static Vector2 GetAxis { get => inputAxis; }
        private static Vector2 inputAxis;
        public static bool MouseLeft { get => mouseLeft; }
        private static bool mouseLeft;
        public static bool MouseRight { get => mouseRight; }
        private static bool mouseRight;
        
        public static void Update()
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            inputAxis = Vector2.Zero;

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

            mouseLeft = (mouse.LeftButton == ButtonState.Pressed);
            mouseRight = (mouse.RightButton == ButtonState.Pressed);
        }
    }
}
