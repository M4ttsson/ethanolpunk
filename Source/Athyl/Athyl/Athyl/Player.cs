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

        private int frameRow;
        private int frameColumn;
        private Texture2D myTexture;
        private float TimePerFrame;
        private int ColFrame;
        private int RowFrame;
        private float TotalElapsed;
        public bool Direction = true;
        public int playerHP = 100;
        private bool hasJumped = false;
        private float tempfallDamage = 0;
        //public DrawableGameObject torso;

        DrawableGameObject wheel;
        RevoluteJoint axis;
        Body foot;
        DrawableGameObject shot = null;
        Projectile projectile;


        public enum stance { melee, midRange, longRange };

        Vector2 jumpForce = new Vector2(0, -3.0f);

       

        public bool OnGround { get; set; }
        public int numFootContacts { get; set; }

        List<DrawableGameObject> shots = new List<DrawableGameObject>();

        const float speed = 3.0f;
        Game1 game;

        public Player(World world, Texture2D texture, Vector2 size, float mass, float wheelSize, Vector2 startPosition, Game1 game)
        {

            Load(texture, 3, 11, 1);

            this.game = game;

            //Vector2 torsoSize = new Vector2(size.X, size.Y);
            //Vector2 torsoSize = new Vector2(myTexture.Width / framecount, myTexture.Height-wheelSize);
            Vector2 torsoSize = new Vector2(size.X, size.Y-wheelSize+5);

            //create torso
            //torso = new DrawableGameObject(world, texture, torsoSize, mass / 2.0f, "player");
            torso = new DrawableGameObject(world, texture, 60, "player");
            torso.Position = startPosition;
            torso.body.Restitution = 0;
            
            // Create the feet of the body, here implemented as high friction wheels 
            wheel = new DrawableGameObject(world, texture, wheelSize, mass, "wheel");
            wheel.Position = torso.Position + new Vector2(0, torsoSize.Y/2);
            wheel.body.Friction = 10000f;
            wheel.body.Restitution = 0.0f;

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
            projectile = new Projectile(game);

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
                    Direction = false;
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
                    Direction = true;
                    break;

                case Movement.Stop:
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                    break;       
            }
        }
        public void UpdatePlayer()
        {

            //check if player foot is touching the ground
            if (numFootContacts < 1)
            {
                OnGround = false;
            }
            else
            {
                OnGround = true;
            }





            //Falldamage on player.
            if (torso.body.LinearVelocity.Y > 10 && !OnGround)
            {
                //playerHP -= (int)torso.body.LinearVelocity.Y * 2;
                hasJumped = true;
                tempfallDamage = torso.body.LinearVelocity.Y;
            }


            if (hasJumped == true && OnGround == true)
            {
                playerHP -= (int)(tempfallDamage * 1.5);
                hasJumped = false;
            }

            if (playerHP < 0)
            {
                playerHP = 0;
            }
        }
        public void useWeapon(World world)
        {

            projectile.NewBullet(torso.body.Position, Direction, world);
            /*shot = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"), 10, 1, "shot");

            shot = new DrawableGameObject(world, projectile, 10, 40, "shot");

            shot.body.IsBullet = true;
            //shot.body.IsSensor = true;
            shot.body.Position = torso.body.Position;
            shot.body.IgnoreGravity = true;
            if(Direction)
                shot.body.ApplyLinearImpulse(new Vector2(0.1f, 0.0f));
            else
                shot.body.ApplyLinearImpulse(new Vector2(-0.1f, 0.0f));
            shots.Add(shot);*/

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
            foreach (DrawableGameObject d in shots)
            {
                d.Draw(spriteBatch);
            }
            projectile.Draw(spriteBatch);
            //DrawFrame(spriteBatch, new Vector2(torso.Position.X, torso.Position.Y - torso.Size.Y/2));
            //torso.Draw(spriteBatch);//, new Vector2(torso.Size.X, torso.Size.Y));
            //wheel.Draw(spriteBatch);
        }

        public void Load(Texture2D texture, int FrameRow, int FrameColumn, int FramesPerSec)
        {
            frameRow = FrameRow;
            frameColumn = FrameColumn;
            myTexture = texture;
            TimePerFrame = (float)1 / FramesPerSec;
            ColFrame = 0;
            RowFrame = 0;
            TotalElapsed = 0;
        }

        public void UpdateFrame(float elapsed)
        {
            TotalElapsed += elapsed;
            if (TotalElapsed > TimePerFrame)
            {
                ColFrame++;
                if (ColFrame == 11)
                    ColFrame = 1;
                TotalElapsed -= TimePerFrame;
            }

            if (axis.MotorSpeed > 0)
            {
                RowFrame = 0;
            }
            else if (axis.MotorSpeed < 0)
            {
                RowFrame = 1;
            }
            else if (axis.MotorSpeed == 0)
            {
                ColFrame = 0;
            }
                        
            //if (axis.MotorSpeed > 0)
            //{                
            //    if (Frame < 11)
            //        Frame = 12;

            //    TotalElapsed += elapsed;
            //    if (TotalElapsed > TimePerFrame)
            //    {
            //        Frame++;
            //        if (Frame == 22)
            //            Frame = 12;
            //        TotalElapsed -= TimePerFrame;
            //    }
            //    Direction = true;
            //}

            //else if (axis.MotorSpeed < 0)
            //{
            //    if (Frame > 11)
            //        Frame = 9;

            //    TotalElapsed += elapsed;
            //    if (TotalElapsed > TimePerFrame)
            //    {
            //        Frame--;
            //        if (Frame == 0)
            //            Frame = 9;
            //        TotalElapsed -= TimePerFrame;
            //    }
            //    Direction = false;
            //}

            //else if (axis.MotorSpeed == 0)
            //{
            //    if (Direction)
            //        Frame = 11;
            //    else
            //        Frame = 10;
            //}
        }

        public void DrawFrame(SpriteBatch spriteBatch, Vector2 screenpos)
        {
            DrawFrame(spriteBatch, ColFrame, RowFrame, screenpos);
        }
        public void DrawFrame(SpriteBatch spriteBatch, int colFrame, int rowFrame, Vector2 screenpos)
        {
            int FrameWidth = myTexture.Width / frameColumn;
            int FrameHeight = myTexture.Height / frameRow;
            Rectangle sourcerect = new Rectangle(FrameWidth * colFrame, FrameHeight * rowFrame,
               FrameWidth, FrameHeight);
            spriteBatch.Draw(myTexture, screenpos, sourcerect, Color.White,
                0.0f, new Vector2(0.0f,0.0f), 1.0f, SpriteEffects.None, 1.0f);
        }

        public void DrawPlayerReserves(SpriteBatch spriteBatch)
        {

        }
    }
}