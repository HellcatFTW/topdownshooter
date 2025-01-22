

namespace TopDownShooter.World
{
    public class Tile
    {
        private readonly Vector2 position;

        private readonly Texture2D texture;

        public Tile(Vector2 position, Texture2D texture)
        {
            this.position = position;
            this.texture = texture;
        }

        public void Draw(Vector2 cameraPos)
        {
            Globals.SpriteBatch.Draw(texture, position + cameraPos, Color.White);
        }
    }
}
