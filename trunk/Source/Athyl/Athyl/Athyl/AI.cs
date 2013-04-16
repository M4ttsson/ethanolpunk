using System;
using System.Collections.Generic;
using System.Linq;
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
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Collision.Shapes;

namespace Athyl
{
    class AI : Player
    {
        //For the normal enemies, the HP should be 100
        public int enemyHP = 100;
        private int runDirection;
        private DateTime previousJump;
        private DateTime lastBullet;
        private float projectileSpeed = 0.02f;
        private const float jumpInterval = 1f;
        private float fireRate = 0.1f;
        private bool hit = false;
        private bool seen = false;
        public bool dead = false;
        public AI(World world, Texture2D texture, Vector2 size, Vector2 startPosition, float mass, float wheelSize, Game1 game)
            : base(world, texture, size, mass, startPosition, game, "enemy")
        {
            Load(texture, 2, 11, 1);

            speed = 1f;
            jumpForce = new Vector2(0, -5f);

            //enemyBody.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);

        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureA.CollisionCategories == Category.Cat1)
                return true;

            //if (contact.IsTouching())
            //{
            //    if (fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "enemy")
            //    {
            //        axis.MotorSpeed = 0;
            //        hit = true;
            //        enemyHP -= 5;
            //        return true;
            //    }
            //}
            else
                return false;
        }

        Vector2 right = new Vector2(2, 0);
        Vector2 left = new Vector2(-2, 0);

        /* Idea for AI
         * 
         * 
         *  1) Is Y value somewhat near the players Y value
            2) If yes, move towards player
         *     
            3) If no, raycast a line straight up
            4) If said line hits a box (anything that is collisionable) 
            5) Raycast to the left and right of the AI (imagine a line that changes rotation, the rotation point being in the middle of the AI)
            6) Find the X position of the side of the box that is last in the row of boxes
            7) If the left X position is further away than the right X position, move to the right X position + 20 or so (or vice versa). 
            8) Jump up on the row of boxes
            9) Go to step 1
         * 
         * 
         * 
        */
        public void towardsPlayer(Player aPlayer)
        {
            if (!hit)
            {
                if (((int)torso.Position.X - (int)aPlayer.torso.Position.X) < -250 && !seen)
                {
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                }

                else if (((int)torso.Position.X - (int)aPlayer.torso.Position.X) > 250 && !seen)
                {
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                }

                else if (torso.Position.X < aPlayer.torso.Position.X)
                {
                    axis.MotorSpeed = MathHelper.TwoPi * speed;
                    UpdateFrame(0.2f);
                    seen = true;
                }

                else if (torso.Position.X > aPlayer.torso.Position.X)
                {
                    axis.MotorSpeed = -MathHelper.TwoPi * speed;
                    UpdateFrame(0.2f);
                    seen = true;
                }

                if (torso.Position.X > aPlayer.torso.Position.X && torso.Position.Y > aPlayer.torso.Position.Y + 10)
                {

                    if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
                    {
                        axis.MotorSpeed = -MathHelper.TwoPi * speed;
                        torso.body.ApplyLinearImpulse(jumpForce);
                        previousJump = DateTime.Now;
                        UpdateFrame(0.2f);
                    }


                }

                else if (torso.Position.X < aPlayer.torso.Position.X && torso.Position.Y > aPlayer.torso.Position.Y + 10 && seen)
                {
                    if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
                    {
                        axis.MotorSpeed = MathHelper.TwoPi * speed;
                        torso.body.ApplyLinearImpulse(jumpForce);
                        previousJump = DateTime.Now;
                        UpdateFrame(0.2f);
                    }

                }
            }
        }

        public void attackPlayer()
        {
            
            
        }


        private void RemoveDeadAI()
        {

            if (enemyHP <= 0)
            {
                dead = true;

            }
        }

        /// <summary>
        /// returns false if no ground between player and AI,else return true
        /// </summary>
        /// <param name="aPlayer"></param>
        /// <param name="world"></param>
        /// <returns></returns>
        private bool rayCast(Player aPlayer, World world)
        {
            Vector2 startRay = new Vector2(torso.Position.X, torso.Position.Y);
            Vector2 endRay = new Vector2(aPlayer.torso.Position.X, aPlayer.torso.Position.Y);
            Vector2 direction = new Vector2(endRay.X - startRay.X, endRay.Y - startRay.Y);
            direction.Normalize();

            while (new Vector2((int) startRay.X, (int) startRay.Y) != new Vector2((int) endRay.X, (int) endRay.Y))
            {
                startRay = startRay + direction;

                Fixture fixture = world.TestPoint(ConvertUnits.ToSimUnits(startRay));

                if (fixture != null && fixture.CollisionCategories == Category.Cat2)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateEnemy(Player aPlayer, World world)
        {
            towardsPlayer(aPlayer);

            //interuptRay =  rayCast(aPlayer, world);
            //attackPlayer();
        }

        /*
        public void DamageAI(Projectile p)
        {
            if (p.AiIsHit == true)
            {
                enemyHP -= 10;
                Console.WriteLine("AIiSHiT");
            }
        }
        */

        public override void Draw(SpriteBatch spriteBatch)
        {
            //DrawFrame(spriteBatch, wheel.Position + new Vector2(-55.0f/2, -110f));
            //enemyBody.Draw(spriteBatch, new Vector2(enemyBody.Size.X, enemyBody.Size.Y + wheel.Size.Y));
            //wheel.Draw(spriteBatch);

            base.Draw(spriteBatch);
        }

        protected override void UpdateFrame(float elapsed)
        {
            base.UpdateFrame(0.2f);
            /*if (axis.MotorSpeed > 0)
            {
                if (ColFrame < 11)
                    ColFrame = 12;

                TotalElapsed += elapsed;
                if (TotalElapsed > TimePerFrame)
                {
                    ColFrame++;
                    if (ColFrame == 22)
                        ColFrame = 12;
                    TotalElapsed -= TimePerFrame;
                    runDirection = (int)axis.MotorSpeed;
                }
            }

            else if (axis.MotorSpeed < 0)
            {
                if (ColFrame > 11)
                    ColFrame = 9;

                TotalElapsed += elapsed;
                if (TotalElapsed > TimePerFrame)
                {
                    ColFrame--;
                    if (ColFrame == 0)
                        ColFrame = 9;
                    TotalElapsed -= TimePerFrame;
                    runDirection = (int)axis.MotorSpeed;
                }
            }

            else if (axis.MotorSpeed == 0)
            {
                if (runDirection > 0)
                    ColFrame = 12;
                else if (runDirection < 0)
                    ColFrame = 10;
            }*/
        }

        //private override Load(Texture2D texture, int FrameCount, int FramesPerSec)
        //{
        //    framecount = FrameCount;
        //    myTexture = texture;
        //    TimePerFrame = (float)1 / FramesPerSec;
        //    Frame = 11;
        //    TotalElapsed = 0;
        //}

        /*public void UpdateFrame(float elapsed)
        {
            if (axis.MotorSpeed > 0)
            {
                if (Frame < 11)
                    Frame = 12;

                TotalElapsed += elapsed;
                if (TotalElapsed > TimePerFrame)
                {
                    Frame++;
                    if (Frame == 22)
                        Frame = 12;
                    TotalElapsed -= TimePerFrame;
                    runDirection = (int)axis.MotorSpeed;
                }
            }

            else if (axis.MotorSpeed < 0)
            {
                if (Frame > 11)
                    Frame = 9;

                TotalElapsed += elapsed;
                if (TotalElapsed > TimePerFrame)
                {
                    Frame--;
                    if (Frame == 0)
                        Frame = 9;
                    TotalElapsed -= TimePerFrame;
                    runDirection = (int)axis.MotorSpeed;
                }
            }

            else if (axis.MotorSpeed == 0)
            {
                if (runDirection > 0)
                    Frame = 11;
                else if (runDirection < 0)
                    Frame = 10;
            }
        }

        public void DrawFrame(SpriteBatch Batch, Vector2 screenpos)
        {
            DrawFrame(Batch, Frame, screenpos);
        }
        public void DrawFrame(SpriteBatch Batch, int Frame, Vector2 screenpos)
        {
            int FrameWidth = myTexture.Width / framecount;
            Rectangle sourcerect = new Rectangle(FrameWidth * Frame, 0,
                FrameWidth, myTexture.Height);
            Batch.Draw(myTexture, screenpos, sourcerect, Color.White,
                0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 1.0f);
        }*/
    }
}
