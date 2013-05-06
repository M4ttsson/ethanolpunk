﻿using System;
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
        public List<DrawableGameObject> hpList = new List<DrawableGameObject>();
        public List<DrawableGameObject> ethList = new List<DrawableGameObject>();
        public bool ethanolDrop, hpDrop;

        public Drops(Game1 game, World world, Player player)
        {
            ethanolTexture = game.Content.Load<Texture2D>("Flask");
            hpTexture = game.Content.Load<Texture2D>("MedKit");

            ethanolBox = new DrawableGameObject(world, ethanolTexture, new Vector2(32, 32), 1, "athyl", 0);
            hpBox = new DrawableGameObject(world, hpTexture, new Vector2(32, 32), 1, "hpBox", 0);
            ethanolBox.body.OnCollision += PickupsForPlayer;
            hpBox.body.OnCollision += PickupsForPlayer;


            hpBox.body.FixedRotation = true;
            ethanolBox.body.FixedRotation = true;
            this.playerz = player;
            this.gamez = game;
            this.world = world;
            ethanolBox.body.BodyType = BodyType.Dynamic;
            hpBox.body.BodyType = BodyType.Dynamic;

            ethanolDrop = false;
            hpDrop = false;


        }


        public void DropPickups(AI ai)
        {
            Random r = new Random();

            if (!ai.dead)
            {


                int random = r.Next(0, 100);
                if (random % 5 == 0)
                {
                    //Spawn hpBox
                    hpList.Add(hpBox);
                    hpBox.body.Position = ai.torso.body.Position;
                    
                        
                    
                }

                else if(random % 3 == 0)
                {
                    //SpawnEthanol
                    ethList.Add(ethanolBox);
                    ethanolBox.body.Position = ai.torso.body.Position;
                }


                ethanolBox.body.IgnoreCollisionWith(ai.torso.body);
                hpBox.body.IgnoreCollisionWith(ai.torso.body);
                ethanolBox.body.IgnoreCollisionWith(ai.wheel.body);
                hpBox.body.IgnoreCollisionWith(ai.wheel.body);
            }
        }


      public bool PickupsForPlayer(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {

            if (contact.IsTouching())
            {
                if ((fixtureA.UserData.ToString() == "athyl" && fixtureB.UserData.ToString() == "playerwheel") || (fixtureA.UserData.ToString() == "playerwheel" && fixtureB.UserData.ToString() == "athyl"))
                {


                    playerz.playerAthyl += 50;
                    ethanolDrop = true;
                }


                else if ((fixtureA.UserData.ToString() == "hpBox" && fixtureB.UserData.ToString() == "playerwheel") || (fixtureA.UserData.ToString() == "playerwheel" && fixtureB.UserData.ToString() == "hpBox"))
                {


                    playerz.playerHP += 25;
                    hpDrop = true;
                }

                else
                {

                    ethanolDrop = false;
                    hpDrop = false;

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
            foreach (DrawableGameObject d in ethList)
            {
                d.Draw(spriteBatch);
            }

            foreach (DrawableGameObject d in hpList)
            {
                d.Draw(spriteBatch);
            }
        }

    }
}