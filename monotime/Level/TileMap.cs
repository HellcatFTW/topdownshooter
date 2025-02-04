using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDownShooter.Entity;

namespace TopDownShooter.Level
{
    public class TileMap
    {
        private static TileTypes[,] tileIndex;
        public int Width { get => tileIndex.GetUpperBound(0) * groundTexture.Width; }
        public int Height { get => tileIndex.GetUpperBound(1) * groundTexture.Height; }

        public List<HitBox> tileHitBoxes = new();

        private Texture2D groundTexture;
        private Texture2D wallTexture;

        public TileMap()
        {
            string levelString = "Level" + World.level;
            Texture2D mapTexture = Globals.Content.Load<Texture2D>(levelString);
            Color[] colorArray = new Color[mapTexture.Width * mapTexture.Height];

            groundTexture = Globals.Content.Load<Texture2D>("GroundTile");
            wallTexture = Globals.Content.Load<Texture2D>("WallTile");

            mapTexture.GetData(colorArray);

            tileIndex = new TileTypes[mapTexture.Width, mapTexture.Height];

            for (int x = 0; x < mapTexture.Width; x++)
            {
                for (int y = 0; y < mapTexture.Height; y++)
                {
                    Color currentColor = colorArray[x * mapTexture.Width + y];

                    if (currentColor.R <= 50 && currentColor.G >= 200 && currentColor.B <= 50)
                    {
                        tileIndex[x, y] = TileTypes.Ground;
                    }
                    else if (currentColor.R >= 200 && currentColor.G <= 50 && currentColor.B <= 50)
                    {
                        tileIndex[x, y] = TileTypes.Wall;
                    }
                    else if (currentColor.R >= 200 && currentColor.G >= 200 && currentColor.B >= 200)
                    {
                        tileIndex[x, y] = TileTypes.Air;
                    }
                }
            }

            SetHitboxes();
        }
        private void SetHitboxes()
        {
            int tileWidth = groundTexture.Width;
            int tileHeight = groundTexture.Height;

            for (int x = 0; x <= tileIndex.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= tileIndex.GetUpperBound(1); y++)
                {
                    Vector2 tilePosition = new Vector2(y * tileHeight, x * tileWidth); // these are backwards because the map is flipped weird.

                    if (tileIndex[x, y] == TileTypes.Wall)
                    {
                        tileHitBoxes.Add(new HitBox(tilePosition + new Vector2(tileWidth / 2, tileHeight / 2), wallTexture.Bounds, 0f));
                    }
                }
            }
        }
        public void Draw()
        {
            int tileWidth = groundTexture.Width;
            int tileHeight = groundTexture.Height;

            for (int x = 0; x <= tileIndex.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= tileIndex.GetUpperBound(1); y++)
                {
                    Vector2 tilePosition = new Vector2(y * tileHeight, x * tileWidth) - World.cameraPos; // these are backwards because the map is flipped weird.

                    switch (tileIndex[x, y])
                    {
                        case TileTypes.Ground:
                            Globals.SpriteBatch.Draw(groundTexture, tilePosition, Color.White);
                            break;

                        case TileTypes.Wall:
                            Globals.SpriteBatch.Draw(wallTexture, tilePosition, Color.White);
                            break;

                        case TileTypes.Air:
                            break;

                    }
                }
            }
            if (World.DebugMode && tileHitBoxes.Count > 0)
            {
                foreach (HitBox hitBox in tileHitBoxes)
                {
                    Utils.DrawHitbox(hitBox, Color.Green);
                }
            }
        }
        private enum TileTypes : int
        {
            Ground = 0,
            Wall = 1,
            Air = 2,
        }
    }

}
