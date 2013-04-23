using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Athyl
{
    class Map
    {
        public static float progress;
        public static float done;
        public static float BoundsX { get; private set; }
        public static float BoundsY { get; private set; }
        private World world;
        private Game1 game;
        private Color[] colors;
        private Color[,] colors2D;
        private List<DrawableGameObject> body = new List<DrawableGameObject>();
        private List<Texture2D> lvl1 = new List<Texture2D>();
        private List<Texture2D> lvl2 = new List<Texture2D>();
        private List<Texture2D> lvl3 = new List<Texture2D>();
        private DrawableGameObject b;

        public Map(World world, Game1 game)
        {
            this.world = world;
            this.game = game;

            progress = 1;

            InializeMap();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (DrawableGameObject b in body)
            {
                b.Draw(spriteBatch);
            }
        }

        public void InializeMap()
        {
            ReadingTilesTextureToList();

            ReadMapLayout(game.Content.Load<Texture2D>("Small_World"));

            DrawTilesOnPlace();
        }

        private void ReadingTilesTextureToList()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    lvl1.Add(game.Content.Load<Texture2D>("Lvl1Tiles/Lvl1" + (i + 1) + "x" + (j + 1)));
                    lvl2.Add(game.Content.Load<Texture2D>("Lvl2Tiles/Lvl2" + (i + 1) + "x" + (j + 1)));
                    lvl3.Add(game.Content.Load<Texture2D>("Lvl3Tiles/Lvl3" + (i + 1) + "x" + (j + 1)));
                }
            }
        }

        /// <summary>
        /// Reads a color coded map from a texture 2d.
        /// </summary>
        /// <param name="map">Texture to read from</param>
        private void ReadMapLayout(Texture2D map)
        {
            //loading bar length
            done = map.Height * map.Width;

            //map bounds
            BoundsX = map.Width * 32;
            BoundsY = map.Height * 32;

            //This Reads the map layout
            colors = new Color[map.Width * map.Height];
            map.GetData<Color>(colors);

            colors2D = new Color[map.Width, map.Height];

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    colors2D[x, y] = colors[x + y * map.Width];
                }
            }
        }

        private void DrawTilesOnPlace()
        {
            Vector2 bodySize = new Vector2(33f, 33f);
            for (int y = 0; y < 68; y++)
            {
                for (int x = 0; x < 320; x++)
                {
                    if (colors2D[x, y] == new Color(255, 0, 0))             //Red  LeftUpperCorner
                    {
                        CreateDrawableGameObject(x, y, 0, bodySize, true);
                    }
                    if (colors2D[x, y] == new Color(255, 255, 0))           //Yellow    Ground
                    {
                        CreateDrawableGameObject(x, y, 1, bodySize, true);
                    }
                    if (colors2D[x, y] == new Color(0, 255, 0))             //Green     RightUpperCorner
                    {
                        CreateDrawableGameObject(x, y, 2, bodySize, true);
                    }
                    if (colors2D[x, y] == new Color(0, 0, 255))             //DarkBlue  LeftWall
                    {
                        CreateDrawableGameObject(x, y, 3, bodySize, true);
                    }
                    if (colors2D[x, y] == new Color(0, 0, 0))               //Black     Middle
                    {
                        CreateDrawableGameObject(x, y, 4, bodySize, false);
                    }
                    if (colors2D[x, y] == new Color(0, 246, 255))           //LightBlue     RightWall
                    {
                        CreateDrawableGameObject(x, y, 5, bodySize, true);
                    }
                    if (colors2D[x, y] == new Color(96, 57, 19))            //Brown     LeftDownCorner
                    {
                        CreateDrawableGameObject(x, y, 6, bodySize, true);
                    }
                    if (colors2D[x, y] == new Color(83, 71, 65))            //Gray      Ceiling
                    {
                        CreateDrawableGameObject(x, y, 7, bodySize, true);
                    }
                    if (colors2D[x, y] == new Color(77, 0, 68))             //Purple    RightDownCorner
                    {
                        CreateDrawableGameObject(x, y, 8, bodySize, true);
                    }
                    progress++;
                }
                progress++;
            }
        }

        private void CreateDrawableGameObject(int x, int y, int tileNumber, Vector2 BodySize, bool Collidable)
        {
            if(y <= 22)
                b = new DrawableGameObject(world, lvl1[tileNumber], BodySize, 0, "ground");
            else if (y>22 && y <=45)
                b = new DrawableGameObject(world, lvl2[tileNumber], BodySize, 0, "ground");
            else
                b = new DrawableGameObject(world, lvl3[tileNumber], BodySize, 0, "ground");

            b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
            b.body.BodyType = BodyType.Static;
            if (!Collidable)
            {
                for (int i = 0; i < b.body.FixtureList.Count; i++)
                {
                    b.body.DestroyFixture(b.body.FixtureList[i]);
                }

                b.body.CollisionCategories = Category.None;
            }
            body.Add(b);
        }
    }
}
