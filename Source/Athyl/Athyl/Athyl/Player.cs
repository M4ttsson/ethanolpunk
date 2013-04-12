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

        private int framecount;
        private Texture2D myTexture;
        private float TimePerFrame;
        private int Frame;
        private float TotalElapsed;
        private KeyboardState KBstate;
        private int hashCode = 0;

        //public DrawableGameObject torso;

        DrawableGameObject wheel;
        RevoluteJoint axis;
        Body foot;


        public enum stance { melee, midRange, longRange };

        Vector2 jumpForce = new Vector2(0, -2.5f);

        Texture2D projectile;

        public bool OnGround { get; set; }
        public int numFootContacts { get; set; }

        List<DrawableGameObject> shots = new List<DrawableGameObject>();

        const float speed = 3.0f;

        public Player(World world, Texture2D texture, Vector2 size, float mass, float wheelSize, Vector2 startPosition)
        {
            projectile = texture;
            Load(texture, 22, 1);


            //Vector2 torsoSize = new Vector2(size.X, size.Y);
            Vector2 torsoSize = new Vector2(myTexture.Width / framecount, myTexture.Height);

            //create torso
            //torso = new DrawableGameObject(world, texture, torsoSize, mass / 2.0f, "player");
            torso = new DrawableGameObject(world, texture, 60, "player");
            torso.Position = startPosition;
            torso.body.Restitution = 0;
            
            // Create the feet of the body, here implemented as high friction wheels 
            wheel = new DrawableGameObject(world, texture, wheelSize, mass, "wheel");
            wheel.Position = torso.Position + new Vector2(0, torsoSize.Y/2);
            wheel.body.Friction = 10000f;
            wheel.body.Restitution = 0;

            // Create a joint to keep the torso upright
            JointFactory.CreateFixedAngleJoint(world, torso.body);

            // Connect the feet to the torso
            axis = JointFactory.CreateRevoluteJoint(world, torso.body, wheel.body, Vector2.Zero);
            axis.CollideConnected = false;

            axis.MotorEnabled = true;
            axis.MotorSpeed = 0;
            axis.MotorTorque = 3;
            axis.MaxMotorTorque = 10;

            numFootContacts = 0;

            //foot = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(50), ConvertUnits.ToSimUnits(10), 1, "foot");
            //foot.IsSensor = true;
            //foot.Position = ConvertUnits.ToSimUnits(wheel.Position - new Vector2(0, wheelSize / 2 - 5));
            //JointFactory.CreateWeldJoint(wheel.body, foot, wheel.Position - new Vector2(0, wheelSize / 2 - 5));
            
        }

        public void Jump()
        {
            if (OnGround)
            {
                torso.body.ApplyLinearImpulse(jumpForce);
            }
        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "ground")
                {
                    OnGround = true;
                    //Debug.WriteLine(OnGround);
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
                    if (!OnGround)
                    {
                        if (torso.body.LinearVelocity.X > 0)
                        {
                            torso.body.LinearVelocity = new Vector2(0, torso.body.LinearVelocity.Y);
                            torso.body.ApplyForce(new Vector2(-speed, 0));
                        }
                        else if (torso.body.LinearVelocity.X >= -speed)
                        {
                            torso.body.ApplyForce(new Vector2(-speed, 0));
                        }
                    }
                    else
                    {
                        axis.MotorSpeed = -MathHelper.TwoPi * speed;
                        UpdateFrame(0.2f);
                    }
                    break;

                case Movement.Right:
                    if (!OnGround)
                    {
                        if (torso.body.LinearVelocity.X < 0)
                        {
                            torso.body.LinearVelocity = new Vector2(0, torso.body.LinearVelocity.Y);
                            torso.body.ApplyForce(new Vector2(speed, 0));
                        }
                        else if (torso.body.LinearVelocity.X <= speed)
                        {
                            torso.body.ApplyForce(new Vector2(speed, 0));
                        }
                    }
                    else
                    {
                        axis.MotorSpeed = MathHelper.TwoPi * speed;
                        UpdateFrame(0.2f);
                    }
                    break;

                case Movement.Stop:
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
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
            DrawFrame(spriteBatch, wheel.Position + new Vector2(-55/2, -110.0f));
           // DrawFrame(spriteBatch, new Vector2(torso.Position.X, torso.Position.Y - torso.Size.Y/2));
            //torso.Draw(spriteBatch);//, new Vector2(torso.Size.X, torso.Size.Y));
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
            KBstate = Keyboard.GetState();
                        
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
                }
                hashCode = KBstate.GetHashCode();
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
                }
                hashCode = KBstate.GetHashCode();
            }

            else if (axis.MotorSpeed == 0)
            {
                if (hashCode == 128)
                    Frame = 11;
                else if (hashCode == 32)
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
                0.0f, new Vector2(0.0f,0.0f), 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}