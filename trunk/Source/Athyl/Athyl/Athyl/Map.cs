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
        private List<Texture2D> asphalt = new List<Texture2D>();

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
                    asphalt.Add(game.Content.Load<Texture2D>("Asphalt/GroundAspalt" + (i + 1) + "x" + (j + 1)));
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
            for (int y = 0; y < 68; y++)
            {
                for (int x = 0; x < 320; x++)
                {
                    if (colors2D[x, y] == new Color(255, 0, 0))             //Red  LeftUpperCorner
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[0], new Vector2(33, 33), 0, "ground");
                        b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        b.body.CollisionCategories = Category.Cat2;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(255, 255, 0))           //Yellow    Ground
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[1], new Vector2(33, 33), 0, "ground");
                        b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(0, 255, 0))             //Green     RightUpperCorner
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[2], new Vector2(33, 33), 0, "ground");
                        b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(0, 0, 255))             //DarkBlue  LeftWall
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[3], new Vector2(33, 33), 0, "ground");
                        b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(0, 0, 0))               //Black     Middle
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[4], new Vector2(33, 33), 0, "ground");
                        b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        for (int i = 0; i < b.body.FixtureList.Count; i++)
                        {
                            b.body.DestroyFixture(b.body.FixtureList[i]);
                        }

                        b.body.CollisionCategories = Category.None;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(0, 246, 255))           //LightBlue     RightWall
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[5], new Vector2(33, 33), 0, "ground");
                        b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(96, 57, 19))            //Brown     LeftDownCorner
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[6], new Vector2(33, 33), 0, "ground");
                        b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(83, 71, 65))            //Gray      Ceiling
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[7], new Vector2(33, 33), 0, "ground");
                        b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(77, 0, 68))             //Purple    RightDownCorner
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[8], new Vector2(33, 33), 0, "ground");
                        b.Position = new Vector2(x * 32 + 16, y * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    progress++;
                }
                progress++;
            }
        }
    }
}
