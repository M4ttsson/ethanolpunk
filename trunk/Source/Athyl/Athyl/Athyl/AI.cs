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

        public static DrawableGameObject enemyBody;
        DrawableGameObject wheel;
        RevoluteJoint axis;
        const float speed = 1.0f;
        Random randomX = new Random(Environment.TickCount);

        bool hit = false;

        public AI(World world, Texture2D texture, Vector2 size, float mass, float wheelSize)
        {
            Vector2 torsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);
           

            

            //create torso
            enemyBody = new DrawableGameObject(world, texture, mass / 2.0f, "enemy");
            enemyBody.Position = new Vector2(randomX.Next(50, 600), 50);

            // Create the feet of the body, here implemented as high friction wheels 
            wheel = new DrawableGameObject(world, texture, wheelSize, mass / 2.0f, "enemy");
            wheel.Position = enemyBody.Position + new Vector2(0, torsoSize.Y / 2.0f);
            wheel.body.Friction = 3.0f;

            // Create a joint to keep the torso upright
            JointFactory.CreateFixedAngleJoint(world, enemyBody.body);

            // Connect the feet to the torso
            axis = JointFactory.CreateRevoluteJoint(world, enemyBody.body, wheel.body, Vector2.Zero);
            axis.CollideConnected = false;

            axis.MotorEnabled = true;
            axis.MotorSpeed = 0;
            axis.MotorTorque = 3;
            axis.MaxMotorTorque = 10;

            
            //enemyBody.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
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
                if (enemyBody.Position.X < aPlayer.torso.Position.X)
                {

                    axis.MotorSpeed = MathHelper.TwoPi * speed;
                }

                else if (enemyBody.Position.X > aPlayer.torso.Position.X)
                {
                    axis.MotorSpeed = -MathHelper.TwoPi * speed;

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
            enemyBody.Draw(spriteBatch, new Vector2(enemyBody.Size.X, enemyBody.Size.Y + wheel.Size.Y));
            //wheel.Draw(spriteBatch);
        }

    }
}
