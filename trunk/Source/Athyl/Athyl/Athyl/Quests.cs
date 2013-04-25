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
using System.Diagnostics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Common.PolygonManipulation;

namespace Athyl
{
    class Quests
    {
        DrawableGameObject boulder;

        public Quests(World world, Game1 game)
        {

            boulder = new DrawableGameObject(world, game.Content.Load<Texture2D>("wheel1"), 30, 10, "boulder");
            boulder.body.BodyType = BodyType.Dynamic;
            boulder.Position = new Vector2(110, 1300);
            
        }


        public void DrawQuest(SpriteBatch spriteBatch)
        {
            boulder.Draw(spriteBatch);
        }


    }
}
