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
    class AI
    {
        private int framecount;
        private Texture2D myTexture;
        private float TimePerFrame;
        private int Frame;
        private float TotalElapsed;
        private int runDirection;

        public DrawableGameObject enemyBody;
        DrawableGameObject wheel;
        RevoluteJoint axis;
        const float speed = 1.0f;
        Random randomX = new Random(Environment.TickCount);
        Vector2 jumpForce = new Vector2(0, -2f);
        DateTime previousJump;
        const float jumpInterval = 0.5f;

        bool hit = false;
        bool seen = false;

        public AI(World world, Texture2D texture, Vector2 size, float mass, float wheelSize)
        {
            Load(texture, 22, 1);
            Vector2 torsoSize = new Vector2(size.X, size.Y-wheelSize+5);

            //create torso
            enemyBody = new DrawableGameObject(world, texture, mass, "enemy");
            enemyBody.Position = new Vector2(randomX.Next(50, 300), 400);
            enemyBody.body.Restitution = 0;


            // Create the feet of the body, here implemented as high friction wheels 
            wheel = new DrawableGameObject(world, texture, wheelSize, mass / 2.0f, "enemy");
            wheel.Position = enemyBody.Position + new Vector2(0, torsoSize.Y/2+5);
            wheel.body.Friction = 3.0f;
            wheel.body.Restitution = 0;
            

            // Create a joint to keep the torso upright
            JointFactory.CreateFixedAngleJoint(world, enemyBody.body);

            // Connect the feet to the torso
            axis = JointFactory.CreateRevoluteJoint(world, enemyBody.body, wheel.body, Vector2.Zero);
            axis.CollideConnected = false;

            axis.MotorEnabled = true;
            axis.MotorSpeed = 0;
            axis.MotorTorque = 3;
            axis.MaxMotorTorque = 10;
            

            
            enemyBody.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "enemy")
                {
                    axis.MotorSpeed = 0;
                    hit = true;
                    return true;
                }
            }

            return false;
        }

        Vector2 right = new Vector2(2, 0);
        Vector2 left = new Vector2(-2, 0);

        public void towardsPlayer(Player aPlayer)
        {
            if (!hit)
            {
                if (((int)enemyBody.Position.X - (int)aPlayer.torso.Position.X) < -250 && !seen)
                {
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                }

                else if (((int)enemyBody.Position.X - (int)aPlayer.torso.Position.X) > 250 && !seen)
                {
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                }

                else if (enemyBody.Position.X < aPlayer.torso.Position.X)
                {
                    axis.MotorSpeed = MathHelper.TwoPi * speed;
                    UpdateFrame(0.2f);
                    seen = true;
                }

                else if (enemyBody.Position.X > aPlayer.torso.Position.X)
                {
                    axis.MotorSpeed = -MathHelper.TwoPi * speed;
                    UpdateFrame(0.2f);
                    seen = true;
                }

                if (enemyBody.Position.X > aPlayer.torso.Position.X && enemyBody.Position.Y > aPlayer.torso.Position.Y+10)
                {
                    
                    if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
                    {
                        axis.MotorSpeed = -MathHelper.TwoPi * speed;
                        enemyBody.body.ApplyLinearImpulse(jumpForce);
                        previousJump = DateTime.Now;
                        UpdateFrame(0.2f);
                    }
                    
                }

                else if (enemyBody.Position.X < aPlayer.torso.Position.X && enemyBody.Position.Y > aPlayer.torso.Position.Y+10 && seen)
                {
                    if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
                    {
                         axis.MotorSpeed = MathHelper.TwoPi * speed;
                         enemyBody.body.ApplyLinearImpulse(jumpForce);
                         previousJump = DateTime.Now;
                         UpdateFrame(0.2f);
                    }
                    
                }
            }
        }

        public void attackPlayer()
        {

        }

        public void UpdateEnemy(Player aPlayer)
        {
            towardsPlayer(aPlayer);
            //attackPlayer();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawFrame(spriteBatch, wheel.Position + new Vector2(-55.0f/2, -110f));
            //enemyBody.Draw(spriteBatch, new Vector2(enemyBody.Size.X, enemyBody.Size.Y + wheel.Size.Y));
            wheel.Draw(spriteBatch);
        }
        public void Load(Texture2D texture, int FrameCount, int FramesPerSec)
        {
            framecount = FrameCount;
            myTexture = texture;
            TimePerFrame = (float)1 / FramesPerSec;
            Frame = 11;
            TotalElapsed = 0;
        }

        public void UpdateFrame(float elapsed)
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
        }
    }
}
