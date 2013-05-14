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

using NLog;


namespace Athyl
{
    class Player
    {
        #region Properties
        
        public Direction direction;
        public DrawableGameObject wheel;
        public DrawableGameObject torso;
        public Projectile projectile;
        public Skilltree skillTree;
        public Int16 skillPoints = 10;

        public bool OnGround { get; set; }
        public bool OnWall { get; set; }
        public bool Crouching { get; set; }
        public bool WallJumped { get; set; }
        public bool NextLevel { get; set; }
        public bool Dead { get; set; }
        public bool lastDirection;

        public int numFootContacts { get; set; }
        public int numSideContacts { get; set; }
        public int xpRequiredPerLevel;
        public int playerXP = 0;
        public int playerLevel = 1;
        public int ColFrame;

        public float Difficulty { get; set; }
        public float playerHP;
        public float playerHpPc;
        public float playerAthyl = 500;
        public float playerAthylPc;
        
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Sounds playerSounds;
        private Joint j;
        private List<DrawableGameObject> shots = new List<DrawableGameObject>();
        private Texture2D crossHair;
        private Rectangle crossHairPosition;
        private SpriteFont font;
        private MouseState mouse;
        private delegate void StancesDel();
        private StancesDel StanceDelegate;
        private DateTime lastBullet;
        private DateTime lastJump;

        private int frameRow;
        private int frameColumn;
        private int RestartFrame;
        private int totalXP;

        private bool hasLeveledRecently = false;
        private bool isFalling = false;
        private bool liftObject = false;
        private bool sniping = false;

        private float TimePerFrame;
        private float TotalElapsed;
        private float tempfallDamage = 0;
        private float aimingAngle = 0;
        private float Damage;

        protected RevoluteJoint axis;
        protected AngleJoint angleJoint;
        protected Texture2D myTexture;
        protected World world;
        protected Game1 game;
        protected Vector2 torsoSize;
        protected Vector2 jumpForce = new Vector2(0, -2.8f);
        protected int RowFrame;

        private Stances stance;
        public Stances Stance
        {
            get { return stance; }
            set
            {
                stance = value;
                ChangeStance(stance);
            }
        }

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

            playerSounds = new Sounds(game);
            Load(texture, 2, 7, 1, 1);

            int wheelSize = (int)size.X;
            this.torsoSize = size - new Vector2(0, (wheelSize / 2));
            this.game = game;
            this.world = world;
            lastDirection = false;
            WallJumped = false;

            // Create the feet of the body, here implemented as high friction wheels 
            wheel = new DrawableGameObject(world, game.Content.Load<Texture2D>("wheel1"), wheelSize, 4.0f, userdata + "wheel");
            wheel.Position = startPosition;
            wheel.body.Friction = 10000f;

            //create torso
            torso = new DrawableGameObject(world, texture, torsoSize, 6.0f, userdata);
            torso.Position = wheel.Position - new Vector2(0.0f, torsoSize.Y / 2 - 5);
            torso.body.Restitution = 0;
            torso.body.CollisionCategories = Category.Cat1;
            torso.body.FixedRotation = true;
            torso.body.Rotation = 0;

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
            projectile = new Projectile(game, this);
            skillTree = new Skilltree(this);

            Stance = Stances.CloseRange;
            playerLevel = 1;
            playerXP = 0;

            Dead = false;
            playerHP = skillTree.maxHp;
            playerHpPc = skillTree.maxHp / playerHP;
            playerAthyl = skillTree.maxAthyl;
            playerAthylPc = skillTree.maxAthyl / playerAthyl;
            Difficulty = 5;

            font = game.Content.Load<SpriteFont>("font");
            crossHair = game.Content.Load<Texture2D>("crosshair");

            torso.body.OnCollision += InteractWithQuestItems;

            if (playerLevel == 1)
            {
                xpRequiredPerLevel += 10;
            }
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
        public enum Stances
        {
            LongRange,
            MidRange,
            CloseRange
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
                this.frameColumn = 7;
                this.myTexture = game.Content.Load<Texture2D>("Player/Gilliam");
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
            this.frameColumn = 1;
            this.ColFrame = 0;
            this.myTexture = game.Content.Load<Texture2D>("Player/Ducking");
            this.TimePerFrame = (float)1 / 1f;
            this.RestartFrame = 0;
            torso.Size = new Vector2(74, 40);
            axis.MotorSpeed = 0;
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

        /// <summary>
        /// Skickar en ray från startPosition i riktning mot endPosition med steg av 
        /// "accuracy" som ger true om kollision med ett object av given kategori, annars false
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="accuracy"></param>
        /// <param name="collisionCategory"></param>
        /// <returns></returns>
        private bool rayCast(Vector2 startPosition, Vector2 endPosition, int accuracy, Category collisionCategory)
        {
            Vector2 startRayPoint = new Vector2((int)startPosition.X, (int)startPosition.Y);
            Vector2 endRayPoint = new Vector2((int)endPosition.X, (int)endPosition.Y);
            Vector2 direction = endRayPoint - startRayPoint;
            direction.Normalize();

            if (startRayPoint != endRayPoint)
            {
                startRayPoint = startRayPoint + (direction * accuracy);

                Fixture fixture = world.TestPoint(ConvertUnits.ToSimUnits(startRayPoint));

                if (fixture != null && fixture.CollisionCategories == collisionCategory)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Skickar två parallella rays från startPosition i riktning mot endPosition med steg av "accuracy" 
        /// som ger true om kollision med ett object av given kategori, annars false
        /// distanceBetweenRays ger avståndet mellan de parallella raysen.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="accuracy"></param>
        /// <param name="collisionCategory"></param>
        /// <param name="distanceBetweenRays"></param>
        /// <returns></returns>
        private bool tripleRayCast(Vector2 startPosition, Vector2 endPosition, int accuracy, Category collisionCategory, int distanceBetweenRays)
        {
            int dist = distanceBetweenRays / 2;

            if (rayCast(startPosition - new Vector2(dist, 0), endPosition - new Vector2(dist, 0), accuracy, collisionCategory))
                return rayCast(startPosition - new Vector2(dist, 0), endPosition - new Vector2(dist, 0), accuracy, collisionCategory);
            else if (rayCast(startPosition + new Vector2(dist, 0), endPosition + new Vector2(dist, 0), accuracy, collisionCategory))
                return rayCast(startPosition + new Vector2(dist, 0), endPosition + new Vector2(dist, 0), accuracy, collisionCategory);
            else if (rayCast(startPosition, endPosition, accuracy, collisionCategory))
                return rayCast(startPosition, endPosition, accuracy, collisionCategory);
            else
                return false;
        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            KeyboardState kbState = Keyboard.GetState();
            if (contact.IsTouching())
            {
                try
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
                catch (Exception ex)
                {
                    logger.Error(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
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
            if (playerAthyl >= 0 && (DateTime.Now - lastBullet).TotalSeconds >= skillTree.fireRate)
            {

                if (stance == Stances.CloseRange)
                {
                    playerSounds.PlaySoundFX("Music/Kapow");
                    if (direction == Direction.Right
                        || direction == Direction.Upright
                        || direction == Direction.Downright)
                    {
                        projectile.NewMeleeBullet(torso.body.Position + ConvertUnits.ToSimUnits(new Vector2(22,14)), Direction.Right, world, skillTree.projectileSpeed, wheel.body, skillTree.damage);
                        playerAthyl -= skillTree.ethanolConsumption;
                        lastBullet = DateTime.Now;
                    }

                    else if (direction == Direction.Left
                        || direction == Direction.Upleft
                        || direction == Direction.Downleft)
                    {
                        projectile.NewMeleeBullet(torso.body.Position + ConvertUnits.ToSimUnits(new Vector2(-22, 14)), Direction.Left, world, skillTree.projectileSpeed, wheel.body, skillTree.damage);
                        playerAthyl -= skillTree.ethanolConsumption;
                        lastBullet = DateTime.Now;
                    }

                    else if (direction == Direction.Up
                        || direction == Direction.Down)
                    {
                        if (lastDirection)
                        {
                            projectile.NewMeleeBullet(torso.body.Position + ConvertUnits.ToSimUnits(new Vector2(-22, 14)), Direction.Left, world, skillTree.projectileSpeed, wheel.body, skillTree.damage);
                            playerAthyl -= skillTree.ethanolConsumption;
                            lastBullet = DateTime.Now;
                        }

                        else
                        {
                            projectile.NewMeleeBullet(torso.body.Position + ConvertUnits.ToSimUnits(new Vector2(22, 14)), Direction.Right, world, skillTree.projectileSpeed, wheel.body, skillTree.damage);
                            playerAthyl -= skillTree.ethanolConsumption;
                            lastBullet = DateTime.Now;
                        }
                    }
                }

                else if (playerAthyl > 0)
                {
                    bool sniper = (Stance == Stances.LongRange) ? true : false;

                    if (sniper)
                    {
                        playerSounds.PlaySoundFX("Music/Sniper");
                        if (Crouching)
                        {
                            Vector2 direction = new Vector2((crossHairPosition.X + 16) - torso.Position.X, (crossHairPosition.Y + 16) - torso.Position.Y);
                            direction.Normalize();
                            projectile.NewBullet(torso.body.Position + ConvertUnits.ToSimUnits(new Vector2(0, 0)), direction, world, wheel.body, torso.body, skillTree.damage);
                        }

                        else
                        {
                            projectile.NewBullet(torso.body.Position + ConvertUnits.ToSimUnits(new Vector2(0, 14)), direction, world, skillTree.projectileSpeed, wheel.body, torso.body, skillTree.damage, sniper);
                        }
                    }

                    else
                    {
                        playerSounds.PlaySoundFX("Music/Pewpew");
                        projectile.NewBullet(torso.body.Position + ConvertUnits.ToSimUnits(new Vector2(0, 14)), direction, world, skillTree.projectileSpeed, wheel.body, torso.body, skillTree.damage, sniper);
                    }
                    playerAthyl -= skillTree.ethanolConsumption;
                    lastBullet = DateTime.Now;
                }
            }
        }
        /// <summary>
        /// Allows the player to jump. Walljump if on wall
        /// </summary>
        public virtual void Jump()
        {
            if (OnGround && !Crouching)
            {
                torso.body.ApplyLinearImpulse(skillTree.playerJumpForce);
                lastJump = DateTime.Now;
            }

            //Walljump
            else if (OnWall && !OnGround && !WallJumped && lastJump.Millisecond + 100  <= DateTime.Now.Millisecond)
            {
                if (direction == Direction.Right)
                {
                    torso.body.ApplyLinearImpulse(new Vector2(skillTree.playerJumpForce.Y/2, skillTree.playerJumpForce.Y));
                    direction = Direction.Left;
                    lastDirection = true;
                }

                else if (direction == Direction.Left)
                {
                    torso.body.ApplyLinearImpulse(new Vector2(-skillTree.playerJumpForce.Y/2, skillTree.playerJumpForce.Y));
                    direction = Direction.Right;
                    lastDirection = false;
                }
                WallJumped = true;
            }
        }

        public virtual void Move(Movement movement)
        {
            if (wheel.body.Enabled)
            {
                switch (movement)
                {
                    case Movement.Left:
                        if (!OnGround)
                        {
                            if (torso.body.LinearVelocity.X > 0)
                            {
                                torso.body.LinearVelocity = new Vector2(0, torso.body.LinearVelocity.Y);
                                torso.body.ApplyForce(new Vector2(skillTree.playerJumpForce.Y/2, 0));

                            }

                            else if (torso.body.LinearVelocity.X >= skillTree.playerJumpForce.Y / 2)
                            {
                                torso.body.ApplyForce(new Vector2(skillTree.playerJumpForce.Y / 2, 0));
                            }
                        }

                        else
                        {
                            axis.MotorSpeed = -MathHelper.TwoPi * skillTree.playerSpeed;
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
                                torso.body.ApplyForce(new Vector2(-skillTree.playerJumpForce.Y / 2, 0));

                            }

                            else if (torso.body.LinearVelocity.X <= -skillTree.playerJumpForce.Y / 2)
                            {
                                torso.body.ApplyForce(new Vector2(-skillTree.playerJumpForce.Y / 2, 0));
                            }
                        }

                        else
                        {
                            axis.MotorSpeed = MathHelper.TwoPi * skillTree.playerSpeed;
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
        }

        #endregion
        #region Stances

        /// <summary>
        /// Runs when Stance property is changed. Changes delegate and stance specific changes
        /// </summary>
        /// <param name="stance">Stance to change to</param>
        private void ChangeStance(Stances stance)
        {
            switch (stance)
            {
                case Stances.CloseRange:
                    skillTree.CloseRange();
                    StanceDelegate = CloseRange;
                    playerHP = skillTree.maxHp * playerHpPc;
                    playerAthyl = skillTree.maxAthyl * playerAthylPc;
                    break;

                case Stances.MidRange:
                    skillTree.MidRange();
                    StanceDelegate = MidRange;
                    playerHP = skillTree.maxHp * playerHpPc;
                    playerAthyl = skillTree.maxAthyl * playerAthylPc;
                    break;

                case Stances.LongRange:
                    skillTree.LongRange();
                    StanceDelegate = LongRange;
                    playerHP = skillTree.maxHp * playerHpPc;
                    playerAthyl = skillTree.maxAthyl * playerAthylPc;
                    break;
            }
        }

        private void CloseRange()
        {
        }

        private void MidRange()
        {
        }

        private void LongRange()
        {
            if (Crouching)
            {
                if (!sniping)
                {
                    switch (direction)
                    {
                        case Direction.Right:
                            crossHairPosition = new Rectangle((int)torso.Position.X + 400, (int)torso.Position.Y - 50, 32, 32);
                            break;

                        case Direction.Left:
                            crossHairPosition = new Rectangle((int)torso.Position.X - 400, (int)torso.Position.Y - 50, 32, 32);
                            break;
                    }
                    sniping = true;
                }

                KeyboardState ks = Keyboard.GetState();

                if (ks.IsKeyDown(InputClass.upKey) && crossHairPosition.Y > torso.Position.Y - 375)
                {
                    if (!lastDirection)
                    {
                        aimingAngle += 0.1f;
                        crossHairPosition.X -= Convert.ToInt32(aimingAngle);
                        crossHairPosition.Y -= 5;
                    }
                    else
                    {
                        aimingAngle += 0.1f;
                        crossHairPosition.X += Convert.ToInt32(aimingAngle);
                        crossHairPosition.Y -= 5;
                    }

                }

                else if (ks.IsKeyDown(InputClass.downKey) && crossHairPosition.Y < torso.Position.Y + 250)

                {
                    if (!lastDirection)
                    {
                        aimingAngle -= 0.1f;
                        crossHairPosition.X += Convert.ToInt32(aimingAngle);
                        crossHairPosition.Y += 5;
                    }
                    else
                    {
                        aimingAngle -= 0.1f;
                        crossHairPosition.X -= Convert.ToInt32(aimingAngle);
                        crossHairPosition.Y += 5;
                    }
                }
            }

            else
            {
                aimingAngle = 0;
                sniping = false;
            }
        }

        #endregion
        #region DrawsAndUpdate

        public void LevelUp(Stances stance)
        {
            switch (stance)
            {
                case Stances.CloseRange:
                    skillTree.LevelCloseRange();
                    NextLevel = false;
                    break;
                    
                case Stances.MidRange:
                    skillTree.LevelMidRange();
                    NextLevel = false;
                    break;

                case Stances.LongRange:
                    skillTree.LevelLongRange();
                    NextLevel = false;
                    break;
            }
        }

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
             mouse =  Mouse.GetState();
           
                if ((fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "boulder") || (fixtureA.UserData.ToString() == "playerwheel" && fixtureB.UserData.ToString() == "boulder"))
                {
                    if (fixtureB.Body.BodyType != BodyType.Static)
                    {
                        if (kbState.IsKeyDown(Keys.Space))
                        {                 
                            if (!liftObject && Math.Abs((fixtureB.Body.Position - fixtureA.Body.Position).X) < 20)
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
            if (WallJumped)         //Ser till att gubben vänder sig åt rätt håll vid walljump
            {
                if (direction == Direction.Left)
                    RowFrame = 1;
                else if (direction == Direction.Right)
                    RowFrame = 0;
            }

            if (playerHP > skillTree.maxHp)
            {
                playerHP = skillTree.maxHp;
            }

            if (playerAthyl > 500)
            {
                playerAthyl = 500;
            }

            KeyboardState kbState = Keyboard.GetState();
            if (kbState.IsKeyUp(Keys.Space) && liftObject)
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

            if (tripleRayCast(wheel.Position, wheel.Position + new Vector2(0, 1), 40, Category.Cat5, (int)torso.Size.X)
                || tripleRayCast(wheel.Position, wheel.Position + new Vector2(0, 1), 45, Category.Cat2, (int)torso.Size.X))  //Kollar om player står på backen eller på en AI.
            {
                OnGround = true;
                WallJumped = false;
            }

            else
            {
                OnGround = false;
            }

            if (!OnGround)
            {
                if (rayCast(torso.Position, torso.Position + new Vector2(1, 0), (int)torso.Size.X / 2 + 5, Category.Cat5)
                    || rayCast(torso.Position, torso.Position + new Vector2(1, 0), (int)torso.Size.X / 2 + 5, Category.Cat6)
                    || rayCast(torso.Position, torso.Position + new Vector2(1, 0), (int)torso.Size.X / 2 + 5, Category.Cat7))//Kollar om spelare kolliderar med en vägg till höger
                {
                    OnWall = true;
                    direction = Direction.Left;
                }

                else if (rayCast(torso.Position, torso.Position + new Vector2(-1, 0), (int)torso.Size.X / 2 + 5, Category.Cat5)
                        || rayCast(torso.Position, torso.Position + new Vector2(-1, 0), (int)torso.Size.X / 2 + 5, Category.Cat6)
                        || rayCast(torso.Position, torso.Position + new Vector2(-1, 0), (int)torso.Size.X / 2 + 5, Category.Cat7))//Kollar om spelare kolliderar med en vägg till Vänster
                {
                    OnWall = true;
                    direction = Direction.Right;
                }

                else
                    OnWall = false;
            }

            else
            {
                wheel.body.Friction = 10000f; 
                if (rayCast(wheel.Position, wheel.Position + new Vector2(1, 0), (int)torso.Size.X / 2 + 5, Category.Cat5)
                 && direction == Direction.Right)
                {
                    axis.MotorSpeed = 0;
                }
                else if (rayCast(wheel.Position, wheel.Position + new Vector2(-1, 0), (int)torso.Size.X / 2 + 5, Category.Cat5)
                    && direction == Direction.Left)
                {
                    axis.MotorSpeed = 0;
                }
            }

            if (playerLevel > 1)
            {
                xpRequiredPerLevel = (int)((playerLevel * (float)Math.Log(playerLevel, 2)) * 15);
            }

            else
            {
                xpRequiredPerLevel = 10;
            }
    
            AnimatePlayer();

            if (playerXP >= xpRequiredPerLevel && playerXP != 0)
            {           
                playerAthyl = skillTree.playerMaxAthyl;
                skillPoints++;
                playerLevel++;
                playerXP = playerXP - xpRequiredPerLevel;
                NextLevel = true;
                playerHP = skillTree.playerMaxHP;
            }

            //Falldamage on player.
            if (torso.body.LinearVelocity.Y > 11 && !OnGround)
            {
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

                else if (Dead && ColFrame != frameColumn-1)
                    UpdateFrame(0.2f);
                playerHP = 0;
            }

            //Update the health percentage
            if (playerHP > skillTree.maxHp)
                playerHP = skillTree.maxHp;
            playerHpPc = playerHP / skillTree.maxHp;

            if (playerAthyl > skillTree.maxAthyl)
                playerAthyl = skillTree.maxAthyl;
            playerAthylPc = playerAthyl / skillTree.maxAthyl;
            
            
            
            //run stance specific updates
            StanceDelegate();
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {            
            projectile.Draw(spriteBatch, torso.Position);
            DrawFrame(spriteBatch, torso.Position - new Vector2(torso.Size.X / 2, torso.Size.Y / 2));

            //torso.Draw(spriteBatch);
            //wheel.Draw(spriteBatch);

            if (Crouching && stance == Stances.LongRange)
            {                
                spriteBatch.Draw(crossHair, crossHairPosition, Color.White);
            }

            if (NextLevel)
            {
                spriteBatch.DrawString(font, "Press N to level up", new Vector2(-(int)Camera.transform.Translation.X + 590, -(int)Camera.transform.Translation.Y + 650), Color.White);
            }
        }
        #endregion
    }
}