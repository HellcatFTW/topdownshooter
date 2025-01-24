

namespace TopDownShooter.Level
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
            Globals.SpriteBatch.Draw(texture, position - cameraPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepths.Background);
        }
    }
}
