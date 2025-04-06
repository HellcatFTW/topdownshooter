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

        public HitBox?[,] hitboxIndex;

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
            hitboxIndex = new HitBox?[mapTexture.Width, mapTexture.Height];

            for (int x = 0; x < mapTexture.Width; x++)
            {
                for (int y = 0; y < mapTexture.Height; y++)
                {
                    Color currentColor = colorArray[y * mapTexture.Width + x];

                    if (currentColor.R == 0 && currentColor.G == 255 && currentColor.B == 0) 
                    {
                        tileIndex[x, y] = TileTypes.Ground;
                    }
                    else if (currentColor.R == 255 && currentColor.G == 0 && currentColor.B == 0)
                    {
                        tileIndex[x, y] = TileTypes.Wall;
                    }
                    else if (currentColor.R == 255 && currentColor.G == 255 && currentColor.B == 255)
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
                    Vector2 tilePosition = new Vector2(x * tileHeight, y * tileWidth);

                    if (tileIndex[x, y] == TileTypes.Wall)
                    {
                        hitboxIndex[x, y] = new HitBox(tilePosition + new Vector2(tileWidth / 2, tileHeight / 2), wallTexture.Bounds, 0f);
                    }
                    else
                    {
                        hitboxIndex[x, y] = null;
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
                    Vector2 tilePosition = new Vector2(x * tileWidth, y * tileHeight) - World.cameraPos;

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
            if (World.DebugMode && hitboxIndex.Length > 0)
            {
                foreach (HitBox hitBox in hitboxIndex)
                {
                    Utils.DrawHitbox(hitBox, Color.Green);
                }
            }
        }
        /// <summary>
        /// Returns the hitboxes surrounding (and including) entity position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public List<HitBox> GetNearestHitboxes(Vector2 position)
        { 
            Point entityIndex = WorldToTileCoordinates(position);
            int entityX = entityIndex.X;
            int entityY = entityIndex.Y;

            List<HitBox> nearHitboxes = new List<HitBox>();

            for (int x = entityX - 1; x <= entityX + 1; x++)
            {
                if (x < hitboxIndex.GetLowerBound(0) || x > hitboxIndex.GetUpperBound(0))
                { 
                    continue;
                }
                for (int y = entityY - 1; y <= entityY + 1; y++)
                {
                    if (y < hitboxIndex.GetLowerBound(1) || y > hitboxIndex.GetUpperBound(1))
                    {
                        continue;
                    }
                    if (hitboxIndex[x, y] != null)
                    {
                        nearHitboxes.Add(hitboxIndex[x, y].Value);
                    }
                }
            }

            return nearHitboxes;
        }
        public Point WorldToTileCoordinates(Vector2 WorldPosition)
        {
            int tileWidth = groundTexture.Width;
            int tileHeight = groundTexture.Height;

            return new Point(((int)(WorldPosition.X / tileWidth)), ((int)(WorldPosition.Y / tileHeight)));
        }
        public Vector2 TileToWorldCoordinates(Point TilePosition)
        {
            int tileWidth = groundTexture.Width;
            int tileHeight = groundTexture.Height;

            return new Vector2(TilePosition.X * tileWidth, TilePosition.Y * tileHeight);
        }
        private bool ValidatePosition(Point position)
        {
            var xCoord = (int)MathHelper.Clamp(position.X, tileIndex.GetLowerBound(0), tileIndex.GetUpperBound(0));
            var yCoord = (int)MathHelper.Clamp(position.Y, tileIndex.GetLowerBound(1), tileIndex.GetUpperBound(1));
            TileTypes tile = tileIndex[xCoord, yCoord];
            return tile != TileTypes.Wall && tile != TileTypes.Air;
        }
        public Vector2 GenerateSpawn()
        {
            const int minimumSpawnDistanceX = 10;
            const int minimumSpawnDistanceY = 6;
            Point spawnPos;

            Point playerPos = WorldToTileCoordinates(World.player.Position);
            Point offset;

            do
            {
                offset = new Point((Globals.Random.Next(minimumSpawnDistanceX, minimumSpawnDistanceX + Globals.Random.Next(0, 3))), (Globals.Random.Next(minimumSpawnDistanceY, minimumSpawnDistanceY + Globals.Random.Next(0,2))));
                if (Globals.Random.NextSingle() < 0.5)
                {
                    spawnPos = playerPos + offset;
                }
                else
                {
                    spawnPos = playerPos - offset;
                }
            }
            while (!ValidatePosition(spawnPos));

            var xCoord = (int)MathHelper.Clamp(spawnPos.X, tileIndex.GetLowerBound(0), tileIndex.GetUpperBound(0));
            var yCoord = (int)MathHelper.Clamp(spawnPos.Y, tileIndex.GetLowerBound(1), tileIndex.GetUpperBound(1));

            return TileToWorldCoordinates(new Point(xCoord,yCoord));
        }
        private enum TileTypes : int
        {
            Ground = 0,
            Wall = 1,
            Air = 2,
        }
    }

}
