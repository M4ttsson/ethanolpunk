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
        private World world;
        private Texture2D texture;
        public Map(World world, Texture2D texture)
        {
            this.world = world;
            this.texture = texture;
            InializeMap();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (DrawableGameObject dgo in ground)
                dgo.Draw(spriteBatch);
        }

        public void InializeMap()
        {

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
        }
    }
}
