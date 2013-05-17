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
        #region Properties
        //For the normal enemies, the HP should be 100
        public int enemyHP = 100;
        private int runDirection;
        private DateTime previousJump;
        private DateTime lastBullet;
        private float projectileSpeed = 0.02f;
        private float jumpInterval = 1f;
        private float fireRate = 0.5f;
        private bool hit = false;
        private bool seen = false;
        private int checkUp = 0, checkDown = 0;
        public bool dead = false;

        private float speed;
        private DateTime lastCheck;
        private Behavior behaviors;
        private delegate void BehaviorDel();
        private BehaviorDel behaviorDel;
        private int patrolLength = 0;
        private int seenPos = 0;
        private float damage;
        

        //test (leave alone)
        Texture2D bossRay;
        Rectangle bossRectRay;
        private bool atSpawn;

        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="world"></param>
        /// <param name="texture"></param>
        /// <param name="size"></param>
        /// <param name="startPosition"></param>
        /// <param name="mass"></param>
        /// <param name="wheelSize"></param>
        /// <param name="game"></param>
        /// <param name="behaviors"></param>
        /// <param name="userdata"></param>
        public AI(World world, Texture2D texture, Vector2 size, Vector2 startPosition, float mass, float wheelSize, Game1 game, Behavior behaviors, string userdata)
            : base(world, texture, size, mass, startPosition, game, userdata)
        {
            //Load(texture, 2, 11, 1,0);
            torso.body.CollisionCategories = Category.Cat2;
            speed = 0.5f;
            //jumpForce = new Vector2(0, -5f);
            
            Load(texture, 2, 5, 1, 1);
            direction = Direction.Right;
            lastCheck = DateTime.Now;

            //not used yet
            OnGround = true;

     
            //sets behavior for this AI
            switch (behaviors)
            {
                case Behavior.Patrol:
                    behaviorDel = Patrol;
                    damage = 34;
                    break;

                case Behavior.PatrolDistance:
                    behaviorDel = PatrolDistance;
                    damage = 40;
                    break;

                case Behavior.Turret:
                    fireRate = 0.04f;
                    enemyHP += 100;
                    behaviorDel = Turret;
                    damage = 45;
                    Load(texture, 1, 8, 1, 0);
                    break;

                case Behavior.Boss:
                    Load(texture, 2, 7, 1, 1);
                    fireRate = 0.5f;
                    enemyHP = 1512;
                    jumpForce = new Vector2(0, -21);
                    behaviorDel = Boss;
                    jumpInterval = 2f;
                    direction = Direction.Left;
                    damage = 70;
                    speed = 0.5f;
                    atSpawn = true;
                    break;

                case Behavior.None:
                    behaviorDel = None;
                    break;
            }

            this.behaviors = behaviors;



            //test (leave in peace)
            bossRay = game.Content.Load<Texture2D>("ProgressBar");
            bossRectRay = new Rectangle((int)wheel.Position.X, (int)wheel.Position.Y, 20, 20);

        }
        #endregion
       

        Vector2 right = new Vector2(2, 0);
        Vector2 left = new Vector2(-2, 0);


        public void towardsPlayer(Player aPlayer)
        {
            if (!hit)
            {
                if (((int)torso.Position.X - (int)aPlayer.torso.Position.X) < -1000 && !seen)
                {
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                }

                else if (((int)torso.Position.X - (int)aPlayer.torso.Position.X) > 1000 && !seen)
                {
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                }

                else if (torso.Position.X < aPlayer.torso.Position.X)
                {
                    axis.MotorSpeed = MathHelper.TwoPi * speed;
                    direction = Direction.Right;
                    UpdateFrame(0.2f);
                    seen = true;
                }

                else if (torso.Position.X > aPlayer.torso.Position.X)
                {
                    axis.MotorSpeed = -MathHelper.TwoPi * speed;
                    direction = Direction.Left;
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
            useWeapon(world);
        }

        public override void Jump()
        {
            if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
            {
                torso.body.ApplyLinearImpulse(jumpForce);
                previousJump = DateTime.Now;
                UpdateFrame(0.2f);
            }
        }

        public override void useWeapon(World world)
        {
            if ((DateTime.Now - lastBullet).TotalSeconds >= fireRate)
            {
                if (behaviors == Behavior.Turret)
                {
                    if (ColFrame == 0)
                    {
                        projectile.NewEnemyBullet(torso.body.Position, Direction.Right, world, projectileSpeed, wheel.body, damage);
                    }
                    else if (ColFrame == 4)
                    {
                        projectile.NewEnemyBullet(torso.body.Position, Direction.Left, world, projectileSpeed, wheel.body, damage);
                    }
                }
                else
                {
                    projectile.NewEnemyBullet(torso.body.Position, direction, world, projectileSpeed, wheel.body, damage);
                }

                //projectile.NewEnemyBullet(torso.body.Position, direction, world, projectileSpeed);
                lastBullet = DateTime.Now;


            }


        }


        private void RemoveDeadAI()
        {

            if (enemyHP <= 0)
            {
                dead = true;

            }
        }

        /// <summary>
        /// returns false if there are no CollisionCategorie 2 objects between startPosition and endPosition, else return true
        /// less accuracy is more accuracy ;)
        /// </summary>
        /// <param name="aPlayer"></param>
        /// <param name="world"></param>
        /// <returns></returns>
        private bool rayCast(Vector2 startPosition, Vector2 endPosition, int accuracy, World world)
        {
            Vector2 startRay = startPosition;
            Vector2 endRay = endPosition;
            Vector2 direction = new Vector2(endRay.X - startRay.X, endRay.Y - startRay.Y);
            direction.Normalize();

            while (new Vector2((int)startRay.X, (int)startRay.Y) != new Vector2((int)endRay.X, (int)endRay.Y))
            {
                startRay = startRay + direction * accuracy;

                Fixture fixture = world.TestPoint(ConvertUnits.ToSimUnits(startRay));

                if (fixture != null && fixture.CollisionCategories == Category.Cat2)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateEnemy(Player aPlayer, World world, List<Drops> drops)
        {
            //towardsPlayer(aPlayer);

            /*if(seen)
                attackPlayer();*/

            foreach (Drops d in drops)
            {

                torso.body.IgnoreCollisionWith(d.hpBox.body);
                torso.body.IgnoreCollisionWith(d.ethanolBox.body);
                wheel.body.IgnoreCollisionWith(d.hpBox.body);
                wheel.body.IgnoreCollisionWith(d.ethanolBox.body);
            }

            //Adds patrol to the AI
            behaviorDel();


        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //DrawFrame(spriteBatch, wheel.Position + new Vector2(-55.0f/2, -110f));
            //enemyBody.Draw(spriteBatch, new Vector2(enemyBody.Size.X, enemyBody.Size.Y + wheel.Size.Y));
            //wheel.Draw(spriteBatch);
            spriteBatch.Draw(bossRay, bossRectRay, Color.White);
            base.Draw(spriteBatch);
        }

        protected override void UpdateFrame(float elapsed)
        {
            if (behaviors == Behavior.Turret)
            {
                TotalElapsed += elapsed;
                if (TotalElapsed > TimePerFrame)
                {
                    ColFrame++;
                    if (ColFrame == frameColumn)
                        ColFrame = RestartFrame;
                    TotalElapsed -= TimePerFrame;
                }
                
            }
            else
            {
                base.UpdateFrame(0.2f);
            }
        }

        #region Behavior

        //enum to store all behavior functions to use for the delegate
        public enum Behavior
        {
            Patrol,
            PatrolDistance,
            Turret,
            Boss,
            None
        }

        public override void Move(Player.Movement movement)
        {

            switch (movement)
            {
                case Movement.Left:
                    axis.MotorSpeed = -MathHelper.TwoPi * speed;
                    UpdateFrame(0.2f);
                    direction = Direction.Left;
                    lastDirection = true;
                    break;

                case Movement.Right:
                    axis.MotorSpeed = MathHelper.TwoPi * speed;
                    UpdateFrame(0.2f);
                    direction = Direction.Right;
                    lastDirection = false;
                    break;

                case Movement.Stop:
                    axis.MotorSpeed = 0;
                    UpdateFrame(0.2f);
                    break;
            }

        }



        /// <summary>
        /// Enemy patrols back and forth between two walls and stops and shoot if the player is in its direction
        /// </summary>
        private void Patrol()
        {

            if ((DateTime.Now - lastCheck).TotalSeconds >= 0.5)
            {
                int distance = 0;
                switch (CheckDirection(direction, 500, out distance))
                {
                    case 0:
                        if (direction == Direction.Left)
                            Move(Movement.Left);
                        else
                            Move(Movement.Right);
                        break;

                    case 1:
                        Move(Movement.Stop);
                        attackPlayer();
                        break;

                    case 2:
                        if (distance >= 50)
                        {
                            if (direction == Direction.Left)
                                Move(Movement.Left);
                            else
                                Move(Movement.Right);
                        }
                        else
                        {
                            direction = lastDirection ? Direction.Right : Direction.Left;
                        }
                        break;
                }
                lastCheck = DateTime.Now;
            }

            UpdateFrame(0.2f);
        }

        private void PatrolDistance()
        {
            if ((DateTime.Now - lastCheck).TotalSeconds >= 0.5)
            {
                int distance = 0;
                switch (CheckDirection(direction, 500, out distance))
                {
                    case 0:
                        if (direction == Direction.Left)
                            Move(Movement.Left);
                        else
                            Move(Movement.Right);
                        patrolLength++;

                        if (patrolLength > 100)
                        {
                            direction = lastDirection ? Direction.Right : Direction.Left;
                            patrolLength = 0;
                        }

                        break;

                    case 1:
                        if (distance >= 50)
                        {
                            if (direction == Direction.Left)
                                Move(Movement.Left);
                            else
                                Move(Movement.Right);
                            attackPlayer();
                            patrolLength++;
                        }
                        else
                        {
                            Move(Movement.Stop);
                            attackPlayer();
                        }

                        break;

                    case 2:
                        if (distance >= 50)
                        {
                            if (direction == Direction.Left)
                                Move(Movement.Left);
                            else
                                Move(Movement.Right);
                        }
                        else
                        {
                            direction = lastDirection ? Direction.Right : Direction.Left;
                        }
                        break;
                }

            }
        }

        /// <summary>
        /// Enemy stands still and shoot in both directions
        /// </summary>
        private void Turret()
        {
            attackPlayer();

            if ((DateTime.Now - lastCheck).TotalSeconds >= 0.72)
            {
                direction = (direction == Direction.Left) ? Direction.Right : Direction.Left;
                lastCheck = DateTime.Now;
            }
            UpdateFrame(0.1f);
        }

        #region BossBehavior

        /// <summary>
        /// Chases the player while shooting. If player evades the boss returns to its starting position.
        /// </summary>
        private void Boss()
        {
            if ((DateTime.Now - lastCheck).TotalSeconds >= 0.2)
            {
                //bossRectRay.Width = 1;
                //bossRectRay.X = (int)wheel.Position.X;
                //bossRectRay.Y = (int)(wheel.Position.Y);

                if(direction == Direction.Right && torso.Position.X >= 9730)
                {
                    Move(Movement.Stop);
                    atSpawn = true;
                    direction = Direction.Left;
                }
                else if (direction == Direction.Right && !atSpawn)
                {
                    int distance = 0;
                    int ret = CheckDirection(Direction.Downright, 100, out distance);
                    if (ret == 2)
                    {
                        if (distance < 100)
                        {
                            Jump();
                        }
                    }
                }
                else if (direction != Direction.Right)
                {
                    int forward = CheckForward();
                    if (forward == 0 || forward == 2)
                    {
                        int up = CheckUp();
                        if (up == 0 || up == 2)
                        {
                            int down = CheckDown();

                            //nothing is found. Go back to start.
                            if ((down == 0 || down == 2) && !atSpawn)
                            {
                                seen = false;
                                Move(Movement.Right);
                            }
                        }
                    }
                }


               
                //Console.WriteLine(atSpawn);
                lastCheck = DateTime.Now;
            }
        }

        /// <summary>
        /// Checks forward for the player (Intended for boss)
        /// </summary>
        /// <returns>Returns 1 for player, 2 for wall and 0 for nothing</returns>
        private int CheckForward()
        {
            int distance = 0;
            switch (CheckDirection(direction, 500, out distance))
            {
                case 0:
                    //bossRectRay.X -= distance;
                    if (!seen)
                        Move(Movement.Stop);
                    // bossRectRay.Width = distance;
                    return 0;

                case 1:
                    //move towards player and shoot
                    seen = true;
                    attackPlayer();
                    if (distance >= 150 && direction == Direction.Left)
                    {
                        Move(Movement.Left);
                        atSpawn = false;
                    }
                    else if (distance >= 150)
                    {
                        //Move(Movement.Right);
                    }
                    else
                    {
                        Move(Movement.Stop);
                    }
                    //bossRectRay.X -= distance;
                    //bossRectRay.Width = distance;
                    return 1;

                case 2:
                    if (distance >= 275 && direction == Direction.Left)
                    {
                        Move(Movement.Left);
                        atSpawn = false;
                    }
                    else if (distance >= 275)
                    {
                        //Move(Movement.Right);
                    }
                    else
                    {
                        Move(Movement.Stop);
                    }
                    //bossRectRay.X -= distance;
                    //bossRectRay.Width = distance;
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Checks for the player 50 pixels up from the torso in forward direction(Intended for boss)
        /// </summary>
        /// <returns>Returns 1 for player, 2 for wall and 0 for nothing</returns>
        private int CheckUp()
        {
            int distance = 0;
            switch (CheckDirection(Direction.Upleft, 500, out distance))
            {
                case 0:
                    bossRectRay.X -= distance;
                    bossRectRay.Y = (int)torso.Position.Y - 50;
                    if(!seen)
                        Move(Movement.Stop);
                    // bossRectRay.Width = distance;
                    return 0;

                case 1:
                    //move towards player and shoot
                    seen = true;
                    attackPlayer();
                    if (distance >= 150 && direction == Direction.Left)
                    {
                        Move(Movement.Left);
                        atSpawn = false;
                    }
                    else if (distance >= 150)
                    {
                        //Move(Movement.Right);
                    }
                    else
                    {
                        Move(Movement.Stop);
                    }
                    bossRectRay.X -= distance;
                    bossRectRay.Y = (int)torso.Position.Y - 50;
                    //bossRectRay.Width = distance;
                    return 1;

                case 2:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Checks for the player 40 pixels down from the wheel in forward direction(Intended for boss)
        /// </summary>
        /// <returns>Returns 1 for player, 2 for wall and 0 for nothing</returns>
        private int CheckDown()
        {
            int distance = 0;
            switch (CheckDirection(Direction.Downleft, 500, out distance))
            {
                case 0:
                    //bossRectRay.X -= distance;
                    if (!seen)
                        Move(Movement.Stop);
                    // bossRectRay.Width = distance;
                    return 0;

                case 1:
                    //move towards player and shoot
                    seen = true;
                    attackPlayer();
                    if (distance >= 150 && direction == Direction.Left)
                    {
                        Move(Movement.Left);
                        atSpawn = false;
                    }
                    else if (distance >= 150)
                    {
                        //Move(Movement.Right);
                    }
                    else
                    {
                        Move(Movement.Stop);
                    }
                    //bossRectRay.X -= distance;
                    //bossRectRay.Width = distance;
                    return 1;

                case 2:
                    if (distance >= 275 && direction == Direction.Left)
                    {
                        Move(Movement.Left);
                        atSpawn = false;
                    }
                    else if (distance >= 275)
                    {
                        //Move(Movement.Right);
                    }
                    else
                    {
                        Move(Movement.Stop);
                    }
                    //bossRectRay.X -= distance;
                    //bossRectRay.Width = distance;
                    return 2;
                default:
                    return 0;
            }


        }
         

        #endregion

        /// <summary>
        /// No actions
        /// </summary>
        private void None()
        {
        }

        /// <summary>
        /// Check for obstacles in given direction
        /// </summary>
        /// <param name="direction">Direction to search in</param>
        /// <param name="lenght">Lenght of the search</param>
        /// <returns>Returns 0 if nothing is found.
        /// Returns 1 if the player is found.
        /// Returns 2 if ground is found.</returns>
        private int CheckDirection(Direction direction, int lenght, out int distance)
        {
            switch (direction)
            {
                case Direction.Left:
                    for (int i = 1; i <= lenght; i++)
                    {
                        
                        Fixture fix = world.TestPoint(ConvertUnits.ToSimUnits(new Vector2(wheel.Position.X - (i + 10), wheel.Position.Y)));

                        if (fix != null)
                        {
                            try
                            {
                                if (fix.UserData.ToString() == "player" || fix.UserData.ToString() == "playerwheel")
                                {
                                    distance = i;
                                    return 1;
                                }
                                if (fix.UserData.ToString() == "ground")
                                {
                                    distance = i;
                                    return 2;
                                }
                            }
                            catch (Exception)
                            { Console.WriteLine("No user data"); }
                        }
                    }
                    distance = lenght;
                    return 0;

                case Direction.Right:
                    for (int i = 1; i <= lenght; i++)
                    {

                        Fixture fix = world.TestPoint(ConvertUnits.ToSimUnits(new Vector2(wheel.Position.X + (i + 10), wheel.Position.Y)));

                        if (fix != null)
                        {
                            try
                            {
                                if (fix.UserData.ToString() == "player" || fix.UserData.ToString() == "playerwheel")
                                {
                                    distance = i;
                                    return 1;
                                }
                                if (fix.UserData.ToString() == "ground")
                                {
                                    distance = i;
                                    return 2;
                                }
                            }
                            catch (Exception)
                            { Console.WriteLine("No user data"); }
                        }
                    }
                    distance = lenght;
                    return 0;

                case Direction.Upleft:
                    for (int i = 1; i <= lenght; i++)
                    {

                        Fixture fix = world.TestPoint(ConvertUnits.ToSimUnits(new Vector2(torso.Position.X - (i + 10), torso.Position.Y - 50)));

                        if (fix != null)
                        {
                            try
                            {
                                if (fix.UserData.ToString() == "player" || fix.UserData.ToString() == "playerwheel")
                                {
                                    distance = i;
                                    return 1;
                                }
                                if (fix.UserData.ToString() == "ground")
                                {
                                    distance = i;
                                    return 2;
                                }
                            }
                            catch (Exception)
                            { Console.WriteLine("No user data"); }
                        }
                    }
                    distance = lenght;
                    return 0;

                case Direction.Downleft:
                    for (int i = 1; i <= lenght; i++)
                    {

                        Fixture fix = world.TestPoint(ConvertUnits.ToSimUnits(new Vector2(wheel.Position.X - (i + 10), wheel.Position.Y + 40)));

                        if (fix != null)
                        {
                            try
                            {
                                if (fix.UserData.ToString() == "player" || fix.UserData.ToString() == "playerwheel")
                                {
                                    distance = i;
                                    return 1;
                                }
                                if (fix.UserData.ToString() == "ground")
                                {
                                    distance = i;
                                    return 2;
                                }
                            }
                            catch (Exception)
                            { Console.WriteLine("No user data"); }
                        }
                    }
                    distance = lenght;
                    return 0;

                case Direction.Downright:
                    for (int i = 1; i <= lenght; i++)
                    {

                        Fixture fix = world.TestPoint(ConvertUnits.ToSimUnits(new Vector2(wheel.Position.X + (i + 10), wheel.Position.Y + 40)));

                        if (fix != null)
                        {
                            try
                            {
                                if (fix.UserData.ToString() == "player" || fix.UserData.ToString() == "playerwheel")
                                {
                                    distance = i;
                                    return 1;
                                }
                                if (fix.UserData.ToString() == "ground")
                                {
                                    distance = i;
                                    return 2;
                                }
                            }
                            catch (Exception)
                            { Console.WriteLine("No user data"); }
                        }
                    }
                    distance = lenght;
                    return 0;

                default:
                    distance = lenght;
                    return 0;
            }
        }

        #endregion

    }
}
