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
        public DrawableGameObject boulder;
        World world;
        public Quests(World world, Game1 game)
        {

            boulder = new DrawableGameObject(world, game.Content.Load<Texture2D>("boulder"), 28, 100, "boulder");
            boulder.body.BodyType = BodyType.Dynamic;
            boulder.Position = new Vector2(160, 1300);
            boulder.body.Friction = 4000;

        }


        public bool InteractWithQuestItems(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {

            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "button" && fixtureB.UserData.ToString() == "boulder")
                {
                    fixtureA.Body.BodyType = BodyType.Static;
                    fixtureB.Body.BodyType = BodyType.Static;

                }






                return true;
            }

            return false;
        }


        public void DrawQuest(SpriteBatch spriteBatch)
        {
            boulder.Draw(spriteBatch);
        }


    }
}
