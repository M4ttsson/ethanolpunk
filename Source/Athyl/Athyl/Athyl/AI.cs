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
        public bool dead = false;

        private DateTime lastCheck;
        private delegate void BehaviorDel();
        private BehaviorDel behavior;
        private int patrolLength = 0;
        private int seenPos = 0;



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
            speed = 1f;
            //jumpForce = new Vector2(0, -5f);

            Load(texture, 2, 5, 1, 1);
            direction = Direction.Right;
            lastCheck = DateTime.Now;
            //enemyBody.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);

            //not used yet
            OnGround = true;

            //sets behavior for this AI
            switch (behaviors)
            {
                case Behavior.Patrol:
                    behavior = Patrol;
                    break;

                case Behavior.PatrolDistance:
                    behavior = PatrolDistance;
                    break;

                case Behavior.Turret:
                    fireRate = 0.2f;
                    enemyHP += 100;
                    behavior = Turret;
                    break;

                case Behavior.Boss:
                    fireRate = 0.05f;
                    enemyHP = 1510;
                    jumpForce = new Vector2(0, -12);
                    behavior = Boss;
                    jumpInterval = 2f;
                    direction = Direction.Left;
                    break;

                case Behavior.None:
                    behavior = None;
                    break;
            }




        }
        #endregion
        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "player" && fixtureB.UserData.ToString() == "enemy")
                {
                    axis.MotorSpeed = 0;
                    hit = true;
                    enemyHP -= 5;
                    return true;
                }
            }

            return false;
        }

        Vector2 right = new Vector2(2, 0);
        Vector2 left = new Vector2(-2, 0);

        /* Idea for AI
         * 
         * 
         *  1) Is Y value somewhat near the players Y value
            2) If yes, move towards player
         *     
            3) If no, raycast a line straight up
            4) If said line hits a box (anything that is collisionable) 
            5) Raycast to the left and right of the AI (imagine a line that changes rotation, the rotation point being in the middle of the AI)
            6) Find the X position of the side of the box that is last in the row of boxes
            7) If the left X position is further away than the right X position, move to the right X position + 20 or so (or vice versa). 
            8) Jump up on the row of boxes
            9) Go to step 1
         * 
         * 
         * 
        */






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
                //projectile.NewEnemyBullet(torso.body.Position, direction, world, projectileSpeed);
                projectile.NewEnemyBullet(torso.body.Position, direction, world, projectileSpeed, wheel.body);
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


            if (torso.body.FixtureList[0].UserData.ToString() == "boss")
                towardsPlayer(aPlayer);

            //Adds patrol to the AI
            behavior();


        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //DrawFrame(spriteBatch, wheel.Position + new Vector2(-55.0f/2, -110f));
            //enemyBody.Draw(spriteBatch, new Vector2(enemyBody.Size.X, enemyBody.Size.Y + wheel.Size.Y));
            //wheel.Draw(spriteBatch);

            base.Draw(spriteBatch);
        }

        protected override void UpdateFrame(float elapsed)
        {
            base.UpdateFrame(0.2f);
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

            base.Move(movement);

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

            if ((DateTime.Now - lastCheck).TotalSeconds >= 0.8)
            {
                direction = (direction == Direction.Left) ? Direction.Right : Direction.Left;
                lastCheck = DateTime.Now;
            }
        }

        private void Boss()
        {
            if ((DateTime.Now - lastCheck).TotalSeconds >= 1)
            {
                int distance = 0;
                if (CheckDirection(direction, 72, out distance) == 2)
                {
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
                        if (direction == Direction.Left)
                            Move(Movement.Left);
                        else
                            Move(Movement.Right);
                    }
                }
                lastCheck = DateTime.Now;
            }
            attackPlayer();
        }

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
                                if (fix.UserData.ToString() == "player")
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
                                if (fix.UserData.ToString() == "player")
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
