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

        public int Direction = 0;
        public int playerHP = 100;
        public int playerAthyl = 500;
        public int playerXP = 0;
        public int playerLevel = 1;

        public DrawableGameObject wheel;
        public enum stance { melee, midRange, longRange };


        protected RevoluteJoint axis;
        protected Texture2D myTexture;
        protected Vector2 jumpForce = new Vector2(0, -6.0f);
        protected float speed = 1.5f;
        protected int ColFrame;
        protected int RowFrame;
        DateTime lastBullet;
        protected World world;

        protected Game1 game;
        protected Projectile projectile;
        private Skilltree skillTree;

        private int xpRequiredPerLevel;
        private int frameRow;
        private int frameColumn;

        //should be private
        private float TimePerFrame;
        private float TotalElapsed;
        private int totalXP;
        private bool hasLeveledRecently = false;
        private bool hasJumped = false;
        private float tempfallDamage = 0;
        private int skillPoints = 0;
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

            int wheelSize = 55;
            this.game = game;
            this.world = world;

            // Create the feet of the body, here implemented as high friction wheels 
            wheel = new DrawableGameObject(world, game.Content.Load<Texture2D>("wheel1"), wheelSize, mass, userdata + "wheel");
            wheel.Position = startPosition;
            wheel.body.Friction = 10000f;
            wheel.body.Restitution = 0.0f;

            //create torso
            torso = new DrawableGameObject(world, texture, new Vector2(size.X, size.Y-10), 60, userdata);
            torso.Position = wheel.Position - new Vector2(0.0f, wheelSize-15);
            torso.body.Restitution = 0;
            
            // Create a joint to keep the torso upright
            JointFactory.CreateFixedAngleJoint(world, torso.body);

            // Connect the feet to the torso
            axis = JointFactory.CreateRevoluteJoint(world, torso.body, wheel.body, Vector2.Zero);
            axis.CollideConnected = false;

            axis.MotorEnabled = true;
            axis.MotorSpeed = 0;
            axis.MotorTorque = 3;
            axis.MaxMotorTorque = 10;
            torso.body.OnCollision +=new OnCollisionEventHandler(body_OnCollision);


            //xpRequiredPerLevel
            numFootContacts = 0;
            projectile = new Projectile(game);
            skillTree = new Skilltree();

            playerLevel = 1;
            playerXP = 0;

            

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
                if (fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "enemy")
                {
                    playerHP -= 3;
                    
                }

                return true;
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
                            torso.body.ApplyForce(new Vector2(-speed * 2, 0));

                        }
                        else if (torso.body.LinearVelocity.X >= -speed * 2)
                        {
                            torso.body.ApplyForce(new Vector2(-speed * 2, 0));
                        }
                    }
                    else
                    {
                        axis.MotorSpeed = -MathHelper.TwoPi * speed;
                        UpdateFrame(0.2f);
                    }
                    Direction = 1;
                    break;

                case Movement.Right:
                    if (!OnGround)
                    {
                        if (torso.body.LinearVelocity.X < 0)
                        {
                            torso.body.LinearVelocity = new Vector2(0, torso.body.LinearVelocity.Y);
                            torso.body.ApplyForce(new Vector2(speed * 2, 0));

                        }
                        else if (torso.body.LinearVelocity.X <= speed * 2)
                        {
                            torso.body.ApplyForce(new Vector2(speed * 2, 0));
                        }
                    }
                    else
                    {
                        axis.MotorSpeed = MathHelper.TwoPi * speed;
                        UpdateFrame(0.2f);
                    }
                    Direction = 0;
                    break;

                case Movement.Stop:
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                    break;
            }
        }
        public void UpdatePlayer()
        {
            xpRequiredPerLevel = (int)((playerLevel * (float)Math.Log(playerLevel, 2)));
            //Console.WriteLine(playerLevel);
            Console.WriteLine(totalXP);
            //Console.WriteLine(xpRequiredPerLevel);
            if (playerXP >= xpRequiredPerLevel && playerXP != 0)
            {
                skillPoints++;
                playerLevel++;
                playerXP = 0;

            }
        
           
            


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
                projectile.NewBullet(torso.body.Position, Direction, world, skillTree.projectileSpeed);
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
            DrawFrame(spriteBatch, torso.Position - new Vector2(torso.Size.X / 2, torso.Size.Y / 2));
            foreach (DrawableGameObject d in shots)
            {
                d.Draw(spriteBatch);
            }
            projectile.Draw(spriteBatch);
            //torso.Draw(spriteBatch);
            //wheel.Draw(spriteBatch);
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
                0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 1.0f);
        }

        public void DrawPlayerReserves(SpriteBatch spriteBatch)
        {

        }
    }
}