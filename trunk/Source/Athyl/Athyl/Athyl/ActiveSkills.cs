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
    class ActiveSkills
    {
        public DrawableGameObject shieldGfx;
        
        private bool shieldOnGround = false;
        private Player player;
        private int rowFrame = 0;
        private World world;
        private Texture2D shieldTexture;

        public int shieldHP;
        public bool removeShield = false;
        public int shieldCooldown = 40;
        public bool shieldActivate = false;
        public Player.Direction direction = Player.Direction.Right;
        public double shieldDuration = 10;

        public ActiveSkills()
        {
            this.shieldGfx = null;
            this.shieldOnGround = false;
            this.player = null;
            this.rowFrame = 0;
            this.world = null;
            this.shieldTexture = null;
            this.shieldHP = 0;
            this.removeShield = false;
            this.shieldCooldown = 0;
            this.shieldActivate = false;
            this.direction = Player.Direction.Right;
            this.shieldDuration = 0;
        }

        public ActiveSkills(World world, Game1 game, Player player, Player.Direction direction)
        {
            this.shieldTexture = game.Content.Load<Texture2D>("Projectiles/Shield");
            this.direction = direction;
            this.player = player;
            this.world = world;
            this.shieldHP = (int)((player.skillTree.playerInfo.playerHP / 10) * player.skillTree.ShieldPoint);
            this.shieldDuration += 2 * player.skillTree.ShieldPoint;

            shieldGfx = new DrawableGameObject(world, shieldTexture, new Vector2(shieldTexture.Width, shieldTexture.Height), 90, "shield");

            if (direction == Player.Direction.Right)
            {
                shieldGfx.Position = new Vector2(player.torso.Position.X + shieldGfx.texture.Width * 1.5f, player.torso.Position.Y);
            }
            else
            {
                shieldGfx.Position = new Vector2(player.torso.Position.X - shieldGfx.texture.Width * 1.5f, player.torso.Position.Y);
            }

            
            shieldGfx.body.FixedRotation = true;
            shieldGfx.body.BodyType = BodyType.Dynamic;
        }


        public void UseShield(Player player)
        {

            //shieldGfx.body.IgnoreCollisionWith(player.torso.body);
            //shieldGfx.body.IgnoreCollisionWith(player.wheel.body);
            shieldActivate = true;
            this.player = player;
            
            shieldHP = (int)(player.playerHP * 0.75f);
        }


        public bool makeShieldStatic(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {

            if (contact.IsTouching())
            {
                if ((fixtureA.UserData.ToString() == "shield" && fixtureB.UserData.ToString() == "ground"))
                {
                    JointFactory.CreateFixedDistanceJoint(world, shieldGfx.body, ConvertUnits.ToSimUnits(Vector2.Zero), ConvertUnits.ToSimUnits(shieldGfx.Position));
                }

                if ((fixtureA.UserData.ToString() == "shield" && fixtureB.UserData.ToString() == "hostile") || (fixtureA.UserData.ToString() == "hostile" && fixtureB.UserData.ToString() == "shield"))
                {
                    shieldHP -= 10;
                    Console.WriteLine(shieldHP);
                }

                return true;
            }

            return false;
        }


        public void UseFireBreath()
        {

        }

        public void UseFireBurst()
        {


        }


        public void Update(World world, GameTime gameTime)
        {
            if (shieldDuration <= 0 || shieldHP <= 0)
            {
                shieldActivate = false;
                world.RemoveBody(shieldGfx.body);
                removeShield = true; 
            }

            if (shieldActivate)
            {
                
                shieldDuration -= gameTime.ElapsedGameTime.TotalSeconds;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (direction == Player.Direction.Left)
            {
                spriteBatch.Draw(shieldTexture, shieldGfx.Position - new Vector2(shieldTexture.Width / 2, shieldTexture.Height / 2.3f), new Rectangle(0, 0, shieldTexture.Width, shieldTexture.Height), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
            }
            else
            {
                spriteBatch.Draw(shieldTexture, shieldGfx.Position - new Vector2(shieldTexture.Width / 2, shieldTexture.Height / 2.3f), new Rectangle(0, 0, shieldTexture.Width, shieldTexture.Height), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 1.0f);
            }


        }
    }
}
