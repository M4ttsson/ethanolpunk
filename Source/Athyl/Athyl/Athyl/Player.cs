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
        public bool OnGround { get; set; }
        public int numFootContacts { get; set; }

        public bool Direction = true;
        public int playerHP = 100;
        public int playerAthyl = 200;

        public enum stance { melee, midRange, longRange };

        protected DrawableGameObject wheel;
        protected RevoluteJoint axis;
        protected Texture2D myTexture;
        protected Vector2 jumpForce = new Vector2(0, -3.0f);
        protected float speed = 3.0f;
        protected int ColFrame;
        protected int RowFrame;
        DateTime lastBullet;

        protected Game1 game;
        protected Projectile projectile;
        private Skilltree skillTree;

        private int frameRow;
        private int frameColumn;

        //should be private
        private float TimePerFrame;
        private float TotalElapsed;
      
        private bool hasJumped = false;
        private float tempfallDamage = 0;

        private List<DrawableGameObject> shots = new List<DrawableGameObject>();

        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="world">The game world</param>
        /// <param name="texture">Player texture</param>
        /// <param name="size">Player size</param>
        /// <param name="mass">Weight in kg</param>
        /// <param name="wheelSize">Diameter of wheel</param>
        /// <param name="startPosition">Startposition</param>
        public Player(World world, Texture2D texture, Vector2 size, float mass, Vector2 startPosition, Game1 game, string userdata)
        {

            Load(texture, 2, 11, 1);

            int wheelSize = 45;
            
            this.game = game;

            //Vector2 torsoSize = new Vector2(size.X, size.Y);
            //Vector2 torsoSize = new Vector2(myTexture.Width / framecount, myTexture.Height-wheelSize);
            Vector2 torsoSize = new Vector2(size.X, size.Y-wheelSize+5);

            //create torso
            //torso = new DrawableGameObject(world, texture, torsoSize, mass / 2.0f, "player");
            torso = new DrawableGameObject(world, texture, new Vector2(55.0f,120.0f), 60, userdata);
            torso.Position = startPosition;
            torso.body.Restitution = 0;
            
            // Create the feet of the body, here implemented as high friction wheels 
            wheel = new DrawableGameObject(world, game.Content.Load<Texture2D>("wheel1"), wheelSize, mass, userdata + "wheel");
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
            skillTree = new Skilltree();

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
            

            if (playerHP <= 0)
            {
                playerHP = 0;
                
            }




        }
        /// <summary>
        /// Fires the weapon
        /// </summary>
        /// <param name="world"></param>
        public void useWeapon(World world)
        {
            if (playerAthyl > 0 && (DateTime.Now - lastBullet).TotalSeconds >= skillTree.fireRate)
            {
                projectile.NewBullet(torso.body.Position, Direction, world);
                playerAthyl -= 1;
                lastBullet = DateTime.Now;

            }
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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            DrawFrame(spriteBatch, wheel.Position + new Vector2(-55/2, -110.0f+(45/2)));
            foreach (DrawableGameObject d in shots)
            {
                d.Draw(spriteBatch);
            }
            projectile.Draw(spriteBatch);
            //DrawFrame(spriteBatch, new Vector2(torso.Position.X, torso.Position.Y - torso.Size.Y/2));
            //torso.Draw(spriteBatch);//, new Vector2(torso.Size.X, torso.Size.Y));
            wheel.Draw(spriteBatch);
        }

        protected void Load(Texture2D texture, int FrameRow, int FrameColumn, int FramesPerSec)
        {
            frameRow = FrameRow;
            frameColumn = FrameColumn;
            myTexture = texture;
            TimePerFrame = (float)1 / FramesPerSec;
            ColFrame = 0;
            RowFrame = 0;
            TotalElapsed = 0;
        }

        protected virtual void UpdateFrame(float elapsed)
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

        protected void DrawFrame(SpriteBatch spriteBatch, Vector2 screenpos)
        {
            DrawFrame(spriteBatch, ColFrame, RowFrame, screenpos);
        }

        private void DrawFrame(SpriteBatch spriteBatch, int colFrame, int rowFrame, Vector2 screenpos)
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