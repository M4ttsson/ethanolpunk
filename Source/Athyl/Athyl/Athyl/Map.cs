using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        List<DrawableGameObject> ground = new List<DrawableGameObject>();

        public Map(World world, Texture2D texture)
        {
            for (int i = 0; i < 32; i++)
            {
                DrawableGameObject floor = new DrawableGameObject(world, texture, new Vector2(40, 40), 100, "ground");
                floor.Position = new Vector2(i * 40, 680);
                floor.body.BodyType = BodyType.Static;
                ground.Add(floor);
            }
        }

        public void Draw(SpriteBatch spriteBatch){
            foreach (DrawableGameObject dgo in ground)
                dgo.Draw(spriteBatch);
        }

    }
}
