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
        private List<DrawableGameObject> ground = new List<DrawableGameObject>();
        private List<DrawableGameObject> testmap = new List<DrawableGameObject>();
        private World world;
        private Game1 game;
        private Texture2D texture;
        private Color[] colors;
        private Color[,] colors2D;
        List<DrawableGameObject> body = new List<DrawableGameObject>();
        List<Texture2D> asphalt = new List<Texture2D>();
        List<Texture2D> BW_bg = new List<Texture2D>();

        public Map(World world, Game1 game)
        {
            this.world = world;
            this.game = game;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    asphalt.Add(game.Content.Load<Texture2D>("Asphalt/GroundAspalt" + (i + 1) + "x" + (j + 1)));
                }
            }

            progress = 1;
            done = 432;

            InializeMap();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //foreach (DrawableGameObject dgo in testmap)
            //    dgo.Draw(spriteBatch);

            foreach (DrawableGameObject b in body)
            {
                Rectangle destination = new Rectangle
                 (
                     (int)ConvertUnits.ToDisplayUnits(b.Position.X),
                     (int)ConvertUnits.ToDisplayUnits(b.Position.Y),
                     40,
                     40
                 );
                b.Draw(spriteBatch);
                //spriteBatch.Draw(texture, destination, null, Color.White, b.body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), SpriteEffects.None, 0);
            }
        }

        public void InializeMap()
        {
            ReadMap(game.Content.Load<Texture2D>("Small_World"));
        }

        private void ReadMap(Texture2D map)
        {
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

            for (int y = 0; y < 135; y++)
            {
                for (int x = 0; x < 640; x++)
                {
                    if (colors2D[x, y] == new Color(255, 0, 0))
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[0], new Vector2(17, 17), 0, "ground");
                        b.Position = new Vector2(x * 16 + 8, y * 16 + 8);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(255, 255, 0))
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[1], new Vector2(17, 17), 0, "ground");
                        b.Position = new Vector2(x * 16 + 8, y * 16 + 8);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(0, 255, 0))
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[2], new Vector2(17, 17), 0, "ground");
                        b.Position = new Vector2(x * 16 + 8, y * 16 + 8);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(0, 0, 255))
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[3], new Vector2(17, 17), 0, "ground");
                        b.Position = new Vector2(x * 16 + 8, y * 16 + 8);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(0, 0, 0))
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[4], new Vector2(17, 17), 0, "ground");
                        b.Position = new Vector2(x * 16 + 8, y * 16 + 8);
                        b.body.BodyType = BodyType.Static;
                        for (int i = 0; i < b.body.FixtureList.Count; i++)
                        {
                            b.body.DestroyFixture(b.body.FixtureList[i]);
                        }

                        b.body.CollisionCategories = Category.None;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(0, 246, 255))
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[5], new Vector2(17, 17), 0, "ground");
                        b.Position = new Vector2(x * 16 + 8, y * 16 + 8);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(96, 57, 19))
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[6], new Vector2(17, 17), 0, "ground");
                        b.Position = new Vector2(x * 16 + 8, y * 16 + 8);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(83, 71, 65))
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[7], new Vector2(17, 17), 0, "ground");
                        b.Position = new Vector2(x * 16 + 8, y * 16 + 8);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                    if (colors2D[x, y] == new Color(77, 0, 68))
                    {
                        DrawableGameObject b = new DrawableGameObject(world, asphalt[8], new Vector2(17, 17), 0, "ground");
                        b.Position = new Vector2(x * 16 + 8, y * 16 + 8);
                        b.body.BodyType = BodyType.Static;
                        body.Add(b);
                    }
                }
                progress++;
            }
        }
    }
}
