
namespace TopDownShooter.World
{
    public class Map
    {
        private Vector2 mapSize = new Vector2(40,40);
        //private Vector2 mapCenter;
        private Texture2D texture;
        private Tile[,] tiles;
        public Map()
        {
            tiles = new Tile[(int)mapSize.X, (int)mapSize.Y];
            texture = Globals.Content.Load<Texture2D>("GroundTile");
            //mapCenter = (mapSize / 2) * new Vector2(texture.Bounds.Size.X, texture.Bounds.Size.Y);

            for (int x = 0; x < mapSize.X; x++)
            {
                for (int y = 0; y < mapSize.Y; y++)
                {
                   tiles[x,y] = new Tile(new Vector2(x * texture.Width, y * texture.Height), texture);
                }
            }
        }
        public void Draw(Vector2 cameraPos)
        {
            for (int x = 0; x < mapSize.X; x++)
            {
                for (int y = 0; y < mapSize.Y; y++)
                {
                    tiles[x, y].Draw(cameraPos);
                }
            }
        }
    }
}
