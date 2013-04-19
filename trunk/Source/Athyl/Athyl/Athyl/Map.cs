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
        private List<DrawableGameObject> ground = new List<DrawableGameObject>();
        private List<DrawableGameObject> testmap = new List<DrawableGameObject>();
        private World world;
        private Game1 game;
        private Texture2D texture;
        private Texture2D map;
        private Color[] colors;
        private Color[,] colors2D;
        List<Body> body = new List<Body>();

        public Map(World world, Game1 game)
        {
            this.world = world;
            this.game = game;
            this.texture = game.Content.Load<Texture2D>("middleground");
            InializeMap();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (DrawableGameObject dgo in testmap)
                dgo.Draw(spriteBatch);
            foreach (DrawableGameObject dgo in ground)
                dgo.Draw(spriteBatch);

            foreach (Body b in body)
            {
                Rectangle destination = new Rectangle
                 (
                     (int)ConvertUnits.ToDisplayUnits(b.Position.X),
                     (int)ConvertUnits.ToDisplayUnits(b.Position.Y),
                     40,
                     40
                 );
                spriteBatch.Draw(texture, destination, null, Color.White, b.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), SpriteEffects.None, 0);
            }
        }

        public void InializeMap()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    DrawableGameObject testBg = new DrawableGameObject(world, game.Content.Load<Texture2D>("Map " + (j+1) + "x" + (i+1)), 0, "test");
                    testBg.body.BodyType = BodyType.Static;
                    testBg.Position = new Vector2(1280 * i +(1280/2), 720 * j +(720/2));
                    testBg.body.CollidesWith = Category.None;
                    testmap.Add(testBg);
                }
            }

            for (int i = 0; i < 32; i++)
            {
                DrawableGameObject floor = new DrawableGameObject(world, texture, new Vector2(42, 40), 100, "ground");
                floor.Position = new Vector2(i * 40 + 20, 700);
                floor.body.BodyType = BodyType.Static;
                floor.body.CollisionCategories = Category.Cat2;
                ground.Add(floor);
                //Debug.WriteLine(floor.Position);
            }

            for (int i = 0; i < 10; i++)
            {
                DrawableGameObject floor = new DrawableGameObject(world, texture, new Vector2(42, 40), 100, "ground");
                floor.Position = new Vector2(i * 40 + 20, 500);
                floor.body.BodyType = BodyType.Static;
                floor.body.CollisionCategories = Category.Cat2;
                ground.Add(floor);
            }

            for (int i = 0; i < 10; i++)
            {
                DrawableGameObject floor = new DrawableGameObject(world, texture, new Vector2(42, 40), 100, "ground");
                floor.Position = new Vector2(i * 40 + 900, 500);
                floor.body.BodyType = BodyType.Static;
                floor.body.CollisionCategories = Category.Cat2;
                ground.Add(floor);
            }
      
            for (int i = 0; i < 4; i++)
            {
                DrawableGameObject floor = new DrawableGameObject(world, texture, new Vector2(42, 40), 100, "ground");
                floor.Position = new Vector2(i * 40 + 460, 300);
                floor.body.BodyType = BodyType.Static;
                floor.body.CollisionCategories = Category.Cat2;
                ground.Add(floor);
            }
            
            for (int i = 0; i < 10; i++)
            {
                DrawableGameObject floor = new DrawableGameObject(world, texture, new Vector2(42, 40), 100, "ground");
                floor.Position = new Vector2(i * 40 + 20, 100);
                floor.body.BodyType = BodyType.Static;
                floor.body.CollisionCategories = Category.Cat2;
                ground.Add(floor);
            }

            for (int i = 0; i < 10; i++)
            {
                DrawableGameObject floor = new DrawableGameObject(world, texture, new Vector2(42, 40), 100, "ground");
                floor.Position = new Vector2(i * 40 + 900, 100);
                floor.body.BodyType = BodyType.Static;
                floor.body.CollisionCategories = Category.Cat2;
                ground.Add(floor);
            }
            ReadMap();
        }

        private void ReadMap()
        {
            map = game.Content.Load<Texture2D>("Map 1x1");
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



            for (int y = 0; y < 18; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    if (colors2D[x * 40, y * 40] == Color.Black)
                    {
                        Body b = BodyFactory.CreateRectangle(world, 0.4f, 0.4f, 1, "ground");
                        b.Position = ConvertUnits.ToSimUnits(x * 40 +20, y * 40+20);
                        b.BodyType = BodyType.Static;
                        body.Add(b);

                    }
                }
            }
        }
    }
}
