using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace Athyl
{
    class Player
    {

       public DrawableGameObject torso;
        DrawableGameObject wheel;
        RevoluteJoint axis;

        public enum stance { melee, midRange, longRange };

        DateTime previousJump;
        const float jumpInterval = 0.5f;
        Vector2 jumpForce = new Vector2(0, -0.6f);
        Texture2D projectile;

        bool OnGround;

        List<DrawableGameObject> shots = new List<DrawableGameObject>();

        const float speed = 3.0f;

        public Player(World world, Texture2D texture, Vector2 size, float mass, float wheelSize, Vector2 startPosition)
        {
            Vector2 torsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);

            projectile = texture;

            //create torso
            //torso = new DrawableGameObject(world, texture, torsoSize, mass / 2.0f, "player");
            torso = new DrawableGameObject(world, texture, 60, "player");
            torso.Position = startPosition;

            // Create the feet of the body, here implemented as high friction wheels 
            wheel = new DrawableGameObject(world, texture, wheelSize, mass / 2.0f, "player");
            wheel.Position = torso.Position + new Vector2(0, torsoSize.Y / 2.0f);
            wheel.body.Friction = 3.0f;

            // Create a joint to keep the torso upright
            JointFactory.CreateFixedAngleJoint(world, torso.body);

            // Connect the feet to the torso
            axis = JointFactory.CreateRevoluteJoint(world, torso.body, wheel.body, Vector2.Zero);
            axis.CollideConnected = false;

            axis.MotorEnabled = true;
            axis.MotorSpeed = 0;
            axis.MotorTorque = 3;
            axis.MaxMotorTorque = 10;
            
            //torso.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);

            //torso.body.OnSeparation += new OnSeparationEventHandler(body_OnSeparation);

            previousJump = DateTime.Now;
        }

        //Om den inte är på ett golv, sätt OnGround till false. Hur?

        void body_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
           // throw new NotImplementedException();
            if (fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "ground")
            {
                OnGround = false;
                Debug.WriteLine(OnGround);
            }
        }

        public void Jump()
        {
            if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
            {
                torso.body.ApplyLinearImpulse(jumpForce);
                previousJump = DateTime.Now;
            }
        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "ground")
                {
                    OnGround = true;
                    Debug.WriteLine(OnGround);
                    return true;
                }
            }
            return false;
        }

       

        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Left:
                    if(!OnGround)
                    {
                        //torso.body.ApplyForce(new Vector2(-speed/2, 0));
                    }
                    //else
                        axis.MotorSpeed = -MathHelper.TwoPi * speed;
                    break;

                case Movement.Right:
                    if(!OnGround)
                    {
                        //torso.body.ApplyForce(new Vector2(speed/2, 0));
                    }
                    //else
                        axis.MotorSpeed = MathHelper.TwoPi * speed;
                    break;

                case Movement.Stop:
                    axis.MotorSpeed = 0;
                    break;       
            }
        }

        public void useWeapon(World world)
        {
            DrawableGameObject shot = new DrawableGameObject(world, projectile, 10, 5, "shot");
            shot.body.IsBullet = true;
        }

        public void changeStance()
        {
        }
        public enum Movement
        {
            Left,
            Right,
            Stop
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            torso.Draw(spriteBatch, new Vector2(torso.Size.X, torso.Size.Y + wheel.Size.Y));
            //wheel.Draw(spriteBatch);
        }

    }
}
