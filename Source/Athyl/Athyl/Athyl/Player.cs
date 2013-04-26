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
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Common.PolygonManipulation;


namespace Athyl
{
    class Player
    {
        #region Properties
        public DrawableGameObject torso;
        public bool OnGround { get; set; }
        public bool OnWall { get; set; }
        public bool Crouching { get; set; }
        public bool WallJumped { get; set; }
        public int numFootContacts { get; set; }
        public int numSideContacts { get; set; }

        public int playerHP = 150;
        public int playerAthyl = 500;
        public int playerXP = 0;
        public int playerLevel = 1;
        public Direction direction;
        public DrawableGameObject wheel;
        public enum stance { melee, midRange, longRange };
        public bool Dead { get; set; }
        //Håller koll på åt vilket håll man stod sist, så att direction ställs rätt om man släpper upp/ner
        public bool lastDirection;
        public float Difficulty { get; set; }


        protected RevoluteJoint axis;
        protected AngleJoint angleJoint;
        protected Texture2D myTexture;
        protected Vector2 torsoSize;
        protected Vector2 jumpForce = new Vector2(0, -2.8f);
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
        private int RestartFrame;

        private float TimePerFrame;
        private float TotalElapsed;
        private int totalXP;
        private bool hasLeveledRecently = false;
        private bool isFalling = false;
        private float tempfallDamage = 0;
        private Joint j;
        private bool liftObject = false;
        public Int16 skillPoints = 0;
        private List<DrawableGameObject> shots = new List<DrawableGameObject>();
        #endregion
        #region ConstructorAndLoad
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
            Load(texture, 2, 11, 1, 1);

            int wheelSize = (int)size.X-4;
            this.torsoSize = size - new Vector2(0, (wheelSize / 2));
            this.game = game;
            this.world = world;
            lastDirection = false;
            WallJumped = false;

            // Create the feet of the body, here implemented as high friction wheels 
            wheel = new DrawableGameObject(world, game.Content.Load<Texture2D>("wheel1"), wheelSize, mass, userdata + "wheel");
            wheel.Position = startPosition;
            wheel.body.Friction = 10000f;
            //wheel.body.Mass = 1;
            
            //create torso
            torso = new DrawableGameObject(world, texture, torsoSize, 60, userdata);
            torso.Position = wheel.Position -new Vector2(0.0f, torsoSize.Y/2-5);
            torso.body.Restitution = 0;
            torso.body.CollisionCategories = Category.Cat1;
            //torso.body.FixedRotation = true;
            //torso.body.Mass = 0;

            // Create a joint to keep the torso upright
            JointFactory.CreateFixedAngleJoint(world, torso.body);

            //angleJoint = JointFactory.CreateAngleJoint(world, wheel.body, wheel.body);

            // Connect the feet to the torso
            axis = JointFactory.CreateRevoluteJoint(world, torso.body, wheel.body, Vector2.Zero);
            axis.CollideConnected = false;

            axis.MotorEnabled = true;
            axis.MotorSpeed = 0;
            axis.MotorTorque = 3;
            axis.MaxMotorTorque = 10;
            torso.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);

            //xpRequiredPerLevel
            numFootContacts = 0;
            projectile = new Projectile(game);
            skillTree = new Skilltree();

            playerLevel = 1;
            playerXP = 0;

            Dead = false;


            Difficulty = 5;

            torso.body.OnCollision += InteractWithQuestItems;

        }

        protected void Load(Texture2D texture, int FrameRow, int FrameColumn, int FramesPerSec, int RestartFrame)
        {
            this.frameRow = FrameRow;
            this.frameColumn = FrameColumn;
            this.myTexture = texture;
            this.TimePerFrame = (float)1 / FramesPerSec;
            this.RestartFrame = RestartFrame;
            this.ColFrame = 0;
            this.RowFrame = 0;
            this.TotalElapsed = 0;
        }
        #endregion
        #region enums
        public enum Direction
        {
            Right,
            Left,
            Up,
            Down,
            Upright,
            Upleft,
            Downright,
            Downleft
        }
        public enum Movement
        {
            Left,
            Right,
            Stop

        }
        #endregion
        #region Animation
        //Animates player in right order depending on which state he/she is
        public void AnimatePlayer()
        {
            if (OnGround && Crouching && !Dead)
                AnimateCrouch();
            else if (!OnGround && !OnWall && !Dead)
                AnimateJump();
            else if (OnWall && !OnGround && !Dead)
                AnimateWallJump();
            else if (Dead)
                AnimateDeath();
            else
            {
                this.frameRow = 2;
                this.frameColumn = 11;
                this.myTexture = game.Content.Load<Texture2D>("Player/TestGubbar");
                this.TimePerFrame = (float)1 / 1f;
                this.RestartFrame = 1;
                torso.Size = torsoSize;
                torso.Position = wheel.Position - new Vector2(0.0f, torsoSize.Y / 2 - 5);
            }
        }

        public void AnimateDeath()
        {
            this.frameRow = 2;
            this.frameColumn = 3;
            this.myTexture = game.Content.Load<Texture2D>("Player/die");
            this.TimePerFrame = (float)1 / 1f;
            this.RestartFrame = 0;
        }

        public void AnimateCrouch()
        {
                this.frameRow = 2;
                this.frameColumn = 2;
                this.ColFrame = 0;
                this.myTexture = game.Content.Load<Texture2D>("Player/Ducking");
                this.TimePerFrame = (float)1 / 1f;
                this.RestartFrame = 0;
                torso.Size = wheel.Size - new Vector2(10, 10);
                torso.Position = wheel.Position + new Vector2(0,10);
                //axis = JointFactory.CreateRevoluteJoint(world, torso.body, wheel.body, Vector2.Zero);
        }

        public void AnimateJump()
        {
                this.frameRow = 2;
                this.frameColumn = 1;
                this.ColFrame = 0;
                this.myTexture = game.Content.Load<Texture2D>("Player/Jump");
                this.TimePerFrame = (float)1 / 1f;
                this.RestartFrame = 0;
        }

        public void AnimateWallJump()
        {
            if (OnWall && !OnGround)
            {
                this.frameRow = 2;
                this.frameColumn = 1;
                this.ColFrame = 0;
                this.myTexture = game.Content.Load<Texture2D>("Player/walljump");
                this.TimePerFrame = (float)1 / 1f;
                this.RestartFrame = 0;
            }
        }
        #endregion
        #region CollisionAndRaycast
        //Skickar en ray från startPosition i riktning mot endPosition med steg av "accuracy" som ger true om kollision med ett object av given kategori, annars false
        private bool rayCast(Vector2 startPosition, Vector2 endPosition, int accuracy, Category collisionCategory)
        {
            Vector2 startRayPoint = new Vector2((int)startPosition.X, (int)startPosition.Y);
            Vector2 endRayPoint = new Vector2((int)endPosition.X, (int)endPosition.Y);
            Vector2 direction = endRayPoint - startRayPoint;
            direction.Normalize();

            if (startRayPoint != endRayPoint )
            {
                startRayPoint = startRayPoint + (direction * accuracy);

                Fixture fixture = world.TestPoint(ConvertUnits.ToSimUnits(startRayPoint));

                if (fixture != null && fixture.CollisionCategories == collisionCategory)
                    return true;
            }
            return false;
        }

        //Skickar två parallella rays från startPosition i riktning mot endPosition med steg av "accuracy" som ger true om kollision med ett object av given kategori, annars false
        //distanceBetweenRays ger avståndet mellan de parallella raysen.
        private bool doubleRayCast(Vector2 startPosition, Vector2 endPosition, int accuracy, Category collisionCategory, int distanceBetweenRays)
        {
            int dist = distanceBetweenRays/2;

            if (rayCast(startPosition - new Vector2(dist, 0), endPosition - new Vector2(dist, 0), accuracy, collisionCategory))
                return rayCast(startPosition - new Vector2(dist, 0), endPosition - new Vector2(dist, 0), accuracy, collisionCategory);
            else if (rayCast(startPosition + new Vector2(dist, 0), endPosition + new Vector2(dist, 0), accuracy, collisionCategory))
                return rayCast(startPosition + new Vector2(dist, 0), endPosition + new Vector2(dist, 0), accuracy, collisionCategory);
            else
                return false;
        }
        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            KeyboardState kbState = Keyboard.GetState();
            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "enemy")
                {
                    playerHP -= 3;

                    if (direction == Direction.Right)
                    {
                        torso.body.ApplyLinearImpulse(new Vector2(-0.8f, -0.9f));
                    }
                    else if (direction == Direction.Left)
                    {
                        torso.body.ApplyLinearImpulse(new Vector2(0.8f, -0.9f));
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region Actions
        /// <summary>
        /// Fires the weapon
        /// </summary>
        /// <param name="world"></param>
        public virtual void useWeapon(World world)
        {
            if (playerAthyl > 0 && (DateTime.Now - lastBullet).TotalSeconds >= skillTree.fireRate)
            {
                projectile.NewBullet(torso.body.Position, direction, world, skillTree.projectileSpeed, wheel.body);
                playerAthyl -= 1;
                lastBullet = DateTime.Now;
            }
        }
        public virtual void Jump()
        {
            if (OnGround && !Crouching)
            {
                torso.body.ApplyLinearImpulse(jumpForce);
            }
            //Walljump
            else if (OnWall && !OnGround && !WallJumped)
            {
                if (direction == Direction.Right)
                {
                    torso.body.ApplyLinearImpulse(new Vector2(-0.8f, -3.1f));
                    direction = Direction.Left;
                }
                else if (direction == Direction.Left)
                {
                    torso.body.ApplyLinearImpulse(new Vector2(0.8f, -3.1f));
                    direction = Direction.Right;
                }
                WallJumped = true;
            }
        }
        public virtual void Move(Movement movement)
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
                    direction = Direction.Left;
                    lastDirection = true;
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
                    direction = Direction.Right;
                    lastDirection = false;
                    break;

                case Movement.Stop:
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                    break;
            }
        }
        #endregion
        #region DrawsAndUpdate
        /// <summary>
        /// Uppdaterar rörelsen i animeringen
        /// </summary>
        /// <param name="elapsed"></param>
        protected virtual void UpdateFrame(float elapsed)
        {
            TotalElapsed += elapsed;
            if (TotalElapsed > TimePerFrame)
            {
                ColFrame++;
                if (ColFrame == frameColumn)
                    ColFrame = RestartFrame;
                TotalElapsed -= TimePerFrame;
            }

            if (axis.MotorSpeed > 0 && !Dead)
            {
                RowFrame = 0;
            }
            else if (axis.MotorSpeed < 0 && !Dead)
            {
                RowFrame = 1;
            }
            else if (axis.MotorSpeed == 0 && !Dead)
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

        bool InteractWithQuestItems(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            KeyboardState kbState = Keyboard.GetState();

            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "boulder")
                {
                    if (fixtureB.Body.BodyType != BodyType.Static)
                    {
                        if (kbState.IsKeyDown(Keys.F) && !liftObject)
                        {
                            fixtureB.Body.IgnoreGravity = true;
                            fixtureB.Body.Position = new Vector2(fixtureA.Body.Position.X, fixtureA.Body.Position.Y);
                            j = JointFactory.CreateWeldJoint(world, fixtureA.Body, fixtureB.Body, Vector2.Zero);
                            fixtureB.Body.IgnoreCollisionWith(torso.body);
                            fixtureB.Body.IgnoreCollisionWith(wheel.body);

                            liftObject = true;

                        }

                    }



                   
                    return true;
                }
            }
            return false;
        }

        private static bool FindQuestItem(Body b)
        {
            foreach (Fixture fix in b.FixtureList)
            {
                if (fix.UserData.ToString() == "boulder")
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdatePlayer()
        {
            KeyboardState kbState = Keyboard.GetState();
            if (kbState.IsKeyUp(Keys.F) && liftObject)
            {

                int index = world.BodyList.FindIndex(FindQuestItem);

                world.BodyList[index].RestoreCollisionWith(torso.body);
                world.BodyList[index].IgnoreGravity = false;


                world.RemoveJoint(j);


                if (direction == Direction.Right)
                {
                    world.BodyList[index].ApplyForce(new Vector2(8, -15));
                }
                if (direction == Direction.Left)
                {
                    world.BodyList[index].ApplyForce(new Vector2(-8, -15));
                }

                liftObject = false;
                
            }

            if (doubleRayCast(wheel.Position, wheel.Position + new Vector2(0, 1), 30, Category.Cat5, 38))  //Kollar om player står på backen.
            {
                OnGround = true;
                WallJumped = false;
            }

            if (!OnGround)
            {
                if (rayCast(wheel.Position, wheel.Position + new Vector2(1, 0), 40, Category.Cat5))         //Kollar om spelare kolliderar med höger vägg
                    OnWall = true;
                else if (rayCast(wheel.Position, wheel.Position + new Vector2(1, 0), 40, Category.Cat6))    //Kollar om spelare kolliderar med höger vägg
                    OnWall = true;
                else if (rayCast(wheel.Position, wheel.Position + new Vector2(1, 0), 40, Category.Cat7))    //Kollar om spelare kolliderar med höger vägg
                    OnWall = true;
                else if (rayCast(wheel.Position, wheel.Position + new Vector2(-1, 0), 40, Category.Cat5))   //Kollar om spelare kolliderar med vänster vägg
                    OnWall = true;
                else if (rayCast(wheel.Position, wheel.Position + new Vector2(-1, 0), 40, Category.Cat6))   //Kollar om spelare kolliderar med vänster vägg
                    OnWall = true;
                else if (rayCast(wheel.Position, wheel.Position + new Vector2(-1, 0), 40, Category.Cat7))   //Kollar om spelare kolliderar med vänster vägg
                    OnWall = true;
                else if (rayCast(wheel.Position, wheel.Position + new Vector2(-1, 0), 40, Category.Cat12))  //Kollar om spelare kolliderar med Osynlig vägg
                    OnWall = false;
                else if (rayCast(wheel.Position, wheel.Position + new Vector2(1, 0), 40, Category.Cat12))   //Kollar om spelare kolliderar med Osynlig vägg
                    OnWall = false;
                else if (doubleRayCast(wheel.Position, wheel.Position + new Vector2(0, -1), 80, Category.Cat7, 40)) //Kollar om player slår i taket.
                    OnWall = false;
            }

            xpRequiredPerLevel = (int)((playerLevel * (float)Math.Log(playerLevel, 2)) * 15);
            //Console.WriteLine(playerLevel);
            //Console.WriteLine(totalXP);
            //Console.WriteLine(xpRequiredPerLevel);
            AnimatePlayer();
            if (playerXP >= xpRequiredPerLevel && playerXP != 0)
            {
                skillPoints++;
                playerLevel++;
                playerXP = playerXP - xpRequiredPerLevel;

            }
            /*
            if (torso.body.Position.X > 40 || torso.body.Position.Y > 10)
            {
                game.Restart();
            }

            */

            //check if player foot is touching the ground
            if (numFootContacts < 1)
            {
                OnGround = false;
            }
            else
            {
                OnGround = true;
                //WallJumped = false;
            }


            //Console.WriteLine(OnGround);
            //check if player is touching a wall
            if (numSideContacts < 1)
            {
                OnWall = false;
            }
            else
            {
                OnWall = true;
            }

            //Falldamage on player.
            if (torso.body.LinearVelocity.Y > 11 && !OnGround)
            {
                //playerHP -= (int)torso.body.LinearVelocity.Y * 2;
                isFalling = true;
                tempfallDamage = torso.body.LinearVelocity.Y * 2;
            }

            if (isFalling == true && OnGround == true)
            {
                playerHP -= (int)(tempfallDamage * 1.5);
                isFalling = false;
            }



            //player off screen
            if (torso.Position.X > Map.BoundsX)
            {

                playerHP = 0;
            }
            else if (torso.Position.X < -10)
                playerHP = 0;
            if (torso.Position.Y > Map.BoundsY)
                playerHP = 0;

            if (playerHP <= 0)
            {
                if (!Dead)
                    Dead = true;
                else if (Dead && ColFrame != frameColumn - 1)
                    UpdateFrame(0.2f);
                playerHP = 0;
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            DrawFrame(spriteBatch, torso.Position - new Vector2(torso.Size.X / 2, torso.Size.Y / 2));

            projectile.Draw(spriteBatch, torso.Position);
            //torso.Draw(spriteBatch);
            //wheel.Draw(spriteBatch);
        }
        #endregion
    }
}