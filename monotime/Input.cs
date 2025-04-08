using System.Collections.Generic;
using System.Linq;

namespace TopDownShooter
{
    public static class Input
    {
        public static Vector2 GetAxis { get => inputAxis; }
        private static Vector2 inputAxis;
        public static bool MouseLeft { get => mouseLeft; }
        private static bool mouseLeft;
        private static bool oldMouseLeft;
        public static bool MouseRight { get => mouseRight; }
        private static bool mouseRight;
        private static bool oldMouseRight;
        public static bool MouseLeftJustPressed { get => mouseLeftJustPressed; }
        private static bool mouseLeftJustPressed;
        public static bool MouseRightJustPressed { get => mouseRightJustPressed; }
        private static bool mouseRightJustPressed;

        private static HashSet<Keys> currentlyPressedKeys = new();
        private static HashSet<Keys> previouslyPressedKeys = new();
        private static HashSet<Keys> justPressedKeys = new();

        public static void Update()
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            currentlyPressedKeys.Clear();
            justPressedKeys.Clear();

            mouseLeft = mouse.LeftButton == ButtonState.Pressed;
            mouseRight = mouse.RightButton == ButtonState.Pressed;

            foreach (Keys key in keyboard.GetPressedKeys())
            { 
                currentlyPressedKeys.Add(key);
                if (!previouslyPressedKeys.Contains(key))
                {
                    justPressedKeys.Add(key);
                }
            }

            mouseRightJustPressed = !oldMouseRight && mouseRight;
            mouseLeftJustPressed = !oldMouseLeft && mouseLeft;

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

            oldMouseLeft = MouseLeft;
            oldMouseRight = MouseRight;

            previouslyPressedKeys = currentlyPressedKeys;
        }
        public static bool HasKeyJustBeenPressed(this KeyboardState kb, Keys key)
        { 
            return justPressedKeys.Contains(key);
        }
    }
}
