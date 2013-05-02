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
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Common.PolygonManipulation;


namespace Athyl
{
class Drops
{

        public DrawableGameObject ethanolBox;
        public DrawableGameObject hpBox;
        Texture2D ethanolTexture, hpTexture;
        Player playerz;
        World world;
        Game1 gamez;
        bool ethanolDrop, hpDrop;

        public Drops(Game1 game, World world, Player player)
        {
            ethanolTexture = game.Content.Load<Texture2D>("ethanol");
            hpTexture = game.Content.Load<Texture2D>("hpBox");

            ethanolBox = new DrawableGameObject(world, ethanolTexture, new Vector2(16, 16), 0, "athyl", 0);
            hpBox = new DrawableGameObject(world, hpTexture, new Vector2(16, 16), 0, "hpBox", 0);
            ethanolBox.body.OnCollision += PickupsForPlayer;
            hpBox.body.OnCollision += PickupsForPlayer;

            this.playerz = player;
            this.gamez = game;
            this.world = world;
            ethanolBox.body.BodyType = BodyType.Dynamic;
            hpBox.body.BodyType = BodyType.Dynamic;

            ethanolDrop = false;
            hpDrop = false;
            //ethanolBox.body.CollisionCategories = Category.Cat8;
            //hpBox.body.CollisionCategories = Category.Cat8;
            //ethanolBox.body.CollidesWith = Category.Cat1 & Category.Cat5 & Category.Cat6 & Category.Cat7;
            //hpBox.body.CollidesWith = Category.Cat1 & Category.Cat5 & Category.Cat6 & Category.Cat7;
        }


        public void DropPickups(AI ai)
        {
            Random r = new Random();

            if (!ai.dead)
            {


                int random = r.Next(0, 100);
                if (random % 2 == 1)
                {

                    ethanolBox.Position = ai.wheel.Position;
                    ethanolBox.body.Enabled = true;
                    ethanolBox.Id++;
                    ethanolDrop = true;
                }

                else
                {

                    hpBox.Position = ai.wheel.Position;
                    hpBox.body.Enabled = true;
                    hpBox.Id++;
                    hpDrop = true;

                }

            }
        }


        bool PickupsForPlayer(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {

            if (contact.IsTouching())
            {
                if ((fixtureA.UserData.ToString() == "athyl" && fixtureB.UserData.ToString() == "playerwheel") || (fixtureA.UserData.ToString() == "playerwheel" && fixtureB.UserData.ToString() == "athyl"))
                {


                    playerz.playerAthyl += 50;
                    ethanolBox.body.Enabled = false;
                    ethanolDrop = false;
                    return true;
                }


                else if ((fixtureA.UserData.ToString() == "hpBox" && fixtureB.UserData.ToString() == "playerwheel") || (fixtureA.UserData.ToString() == "playerwheel" && fixtureB.UserData.ToString() == "hpBox"))
                {
                    playerz.playerHP += 25;
                    hpBox.body.Enabled = false;
                    hpDrop = false;
                    return true;
                }
            }
            return false;
        }

        public void Update(AI ai, World world)
        {
            DropPickups(ai);
            this.world = world;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (ethanolDrop)
            {
                ethanolBox.Draw(spriteBatch);
            }

            if (hpDrop)
            {
                hpBox.Draw(spriteBatch);
            }
        }

    }
}