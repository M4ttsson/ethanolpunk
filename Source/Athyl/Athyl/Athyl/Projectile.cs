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

namespace Athyl
{
    class Projectile
    {
        #region Properties
        private int projectileVelocity;
        private Vector2 projectileDirection;
        public float damage = 34;
        public enum projectiletype { small, medium, large }
        List<DrawableGameObject> bullets = new List<DrawableGameObject>();
        List<DrawableGameObject> meleeBullets = new List<DrawableGameObject>();
        List<DrawableGameObject> meleeremoveList = new List<DrawableGameObject>();
        List<Body> meleeremoveListbody = new List<Body>();
        Game1 game;
        //Lista av saker som ska tas bort i "bullets"
        List<DrawableGameObject> removeList = new List<DrawableGameObject>();
        //Lista av saker som ska tas bort i world
        List<Body> removeListbody = new List<Body>();
        bool friendly;
        float bulletLifeTime;
        float bulletWasFired;
        Random random = new Random();
        #endregion
        #region Constructor
        public Projectile(Game1 game)
        {
            this.game = game;

            bulletLifeTime = 5.0f;
        }
        #endregion
        #region AddBullet
        /// <summary>
        /// Adds a new bullet to the list
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="world"></param>
        public void NewBullet(Vector2 position, Player.Direction direction , World world, float speed, Body wheel, float damage, bool sniper)
        {
            this.damage = damage;

            float spread = 0;
            float spreadDiagonal = 0;
            if (!sniper)
            {
                //Allows the linear bullets to have some spread!
                spread = random.Next(-2, 2);

                //Allows diagonal bullets to have some spread!
                spreadDiagonal = random.Next(1, 5);
                spread /= 133;

            }
            
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"),new Vector2(10,4), 10, "shot");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bulletWasFired = Game1.runTime;
            bullet.body.IsSensor = true;
            bullet.body.IgnoreCollisionWith(wheel);

            switch (direction)
            {
                case Player.Direction.Right:
                    bullet.body.Rotation = MathHelper.ToRadians(180);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed, spread * 0.2f));
                    break;


                case Player.Direction.Left:
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed * -1, spread * 0.2f));
                    break;

                case Player.Direction.Down:
                    bullet.body.Rotation = MathHelper.ToRadians(270);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(spread * 0.2f, speed));
                    break;

                case Player.Direction.Up:
                    bullet.body.Rotation = MathHelper.ToRadians(90);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(spread * 0.2f, speed * -1.0f));
                    break;

                case Player.Direction.Upright:
                    bullet.body.Rotation = MathHelper.ToRadians(135);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2, speed / 2 * -1.0f) * spreadDiagonal);                
                    break;

                case Player.Direction.Upleft:
                    bullet.body.Rotation = MathHelper.ToRadians(45);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2 * -1.0f, speed / 2 * -1.0f) * spreadDiagonal);
                    break;

                case Player.Direction.Downright:
                    bullet.body.Rotation = MathHelper.ToRadians(-135);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2, speed / 2) * spreadDiagonal);
                    break;

                case Player.Direction.Downleft:
                    bullet.body.Rotation = MathHelper.ToRadians(-45);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2 * -1.0f, speed / 2) * spreadDiagonal);
                    break;

                default:
                    break;

            }
            bullets[bullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        /// <summary>
        /// Adds a melee bullet
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="world"></param>
        /// <param name="speed"></param>
        /// <param name="wheel"></param>
        /// <param name="damage"></param>
        public void NewMeleeBullet(Vector2 position, Player.Direction direction , World world, float speed, Body wheel, float damage)
        {
            this.damage = damage;
   
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"),new Vector2(10,4), 10, "melee");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bulletWasFired = Game1.runTime;
            bullet.body.IsSensor = true;
            bullet.body.IgnoreCollisionWith(wheel);

            switch (direction)
            {
                case Player.Direction.Right:
                    bullet.body.Rotation = MathHelper.ToRadians(180);
                    bullet.body.FixedRotation = true;
                    meleeBullets.Add(bullet);
                    meleeBullets[meleeBullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed, 0));
                    break;

                case Player.Direction.Left:
                    bullet.body.FixedRotation = true;
                    meleeBullets.Add(bullet);
                    meleeBullets[meleeBullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed * -1, 0));
                    break;

                default:
                    break;

            }
            meleeBullets[meleeBullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        /// <summary>
        /// prototyp, kulor som fienden skjuter
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="world"></param>
        /// <param name="speed"></param>
        public void NewEnemyBullet(Vector2 position, Player.Direction direction, World world, float speed, Body wheel)
        {
            //Allows the linear bullets to have some spread!
            float spread = random.Next(-2, 2);

            //Allows diagonal bullets to have some spread!
            float spreadDiagonal = random.Next(1, 5);
            spread /= 133;
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"), new Vector2(10, 4), 10, "hostile");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bulletLifeTime = 5;
            bulletWasFired = Game1.runTime;
            bullet.body.IsSensor = true;
            bullet.body.IgnoreCollisionWith(wheel);

            switch (direction)
            {
                case Player.Direction.Right:
                    bullet.body.Rotation = MathHelper.ToRadians(180);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed, spread * 0.2f));
                    break;


                case Player.Direction.Left:
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed * -1, spread * 0.2f));
                    break;
            }
            bullets[bullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
            //DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"), new Vector2(10, 4), 10, "hostile");
            //bullet.body.IsBullet = true;
            //bullet.body.Position = position;
            //bullet.body.IgnoreGravity = true;
            //bullet.body.FixedRotation = true;
            //bullets.Add(bullet);
            //if (direction == Player.Direction.Right)
            //    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed, 0.0f));
            //else
            //    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed * -1, 0.0f));
            //bullets[bullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }
        #endregion
        #region Collisionanddraw
        /// <summary>
        /// Kollision med kulor, tar bort något träffas
        /// olik logik beroende på om det är fiende eller spelare som krockar
        /// </summary>
        /// <param name="fixtureA"></param>
        /// <param name="fixtureB"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "shot" && fixtureB.UserData.ToString() == "enemy" || 
                    fixtureA.UserData.ToString() == "shot" && fixtureB.UserData.ToString() == "boss")
                {
                    if(!removeListbody.Contains(fixtureA.Body))
                        removeListbody.Add(fixtureA.Body);
                    foreach (DrawableGameObject i in bullets)
                    {
                        if (i.body.BodyId == fixtureA.Body.BodyId)
                        {
                            if (!removeList.Contains(i))
                                removeList.Add(i);
                        }
                    }
                    game.damageList.Add(new Damage(fixtureB.Body.BodyId, damage));
                    return true;
                }

                else if (fixtureA.UserData.ToString() == "melee" && fixtureB.UserData.ToString() == "enemy" ||
                    fixtureA.UserData.ToString() == "melee" && fixtureB.UserData.ToString() == "boss")
                {
                    if (!meleeremoveListbody.Contains(fixtureA.Body))
                        meleeremoveListbody.Add(fixtureA.Body);
                    foreach (DrawableGameObject i in bullets)
                    {
                        if (i.body.BodyId == fixtureA.Body.BodyId)
                        {
                            if (!meleeremoveList.Contains(i))
                                meleeremoveList.Add(i);
                        }
                    }
                    game.damageList.Add(new Damage(fixtureB.Body.BodyId, damage));
                    return true;
                }

                else if (fixtureA.UserData.ToString() == "hostile" && fixtureB.UserData.ToString() == "player")
                {
                    if (!removeListbody.Contains(fixtureA.Body))
                        removeListbody.Add(fixtureA.Body);
                    foreach (DrawableGameObject i in bullets)
                    {
                        if (i.body.BodyId == fixtureA.Body.BodyId)
                        {
                            if (!removeList.Contains(i))
                                removeList.Add(i);
                        }
                    }
                    game.damageList.Add(new Damage(fixtureB.Body.BodyId, damage));
                    return true;
                }
                else if (fixtureA.UserData.ToString() == "shot" && fixtureB.UserData.ToString() == "ground")
                {
                    if (!removeListbody.Contains(fixtureA.Body))
                        removeListbody.Add(fixtureA.Body);
                    foreach (DrawableGameObject i in bullets)
                    {
                        if (i.body.BodyId == fixtureA.Body.BodyId)
                        {
                            if (!removeList.Contains(i))
                                removeList.Add(i);
                        }
                    }
                    //Console.WriteLine("removed");
                    return true;
                }
                else if (fixtureA.UserData.ToString() == "hostile" && fixtureB.UserData.ToString() == "ground")
                {
                    if (!removeListbody.Contains(fixtureA.Body))
                        removeListbody.Add(fixtureA.Body);
                    foreach (DrawableGameObject i in bullets)
                    {
                        if (i.body.BodyId == fixtureA.Body.BodyId)
                        {
                            if (!removeList.Contains(i))
                                removeList.Add(i);
                        }
                    }
                    //Console.WriteLine("removed");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Ritar projektiler. Tar bort alla kulor som träffar något eller har varit aktiva för länge
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 playerPos)
        {
            
            for(int i = 0; i < bullets.Count; i++){
                bullets[i].Draw(spriteBatch);
                if (bulletWasFired + bulletLifeTime <= Game1.runTime)
                {
                    if (!removeList.Contains(bullets[i]))
                        removeList.Add(bullets[i]);
                    try
                    {
                        game.world.RemoveBody(bullets[i].body);
                    }
                    catch (Exception)
                    {
                    }
                }
              //  Console.WriteLine(game.world.BodyList.Count);
            }

            for (int i = 0; i < meleeBullets.Count; i++)
            {
                meleeBullets[i].Draw(spriteBatch);
                if (bulletWasFired + 0.1f <= (float)Game1.runTime)
                {
                    if (!meleeremoveList.Contains(meleeBullets[i]))
                        meleeremoveList.Add(meleeBullets[i]);
                    try
                    {
                        game.world.RemoveBody(meleeBullets[i].body);
                    }
                    catch (Exception)
                    {
                    }
                }
                //  Console.WriteLine(game.world.BodyList.Count);
            }

            foreach (DrawableGameObject i in removeList)
            {
                bullets.Remove(i);
                //Console.WriteLine(bullets.Count);
            }

            foreach (DrawableGameObject i in meleeremoveList)
            {
                meleeBullets.Remove(i);
                //Console.WriteLine(bullets.Count);
            }
            

            foreach (Body i in removeListbody)
            {
                try
                {
                    game.world.RemoveBody(i);
                }
                catch (Exception)
                {
                }
            }
            removeListbody.Clear();
            removeList.Clear();
            meleeremoveList.Clear();
        }
        #endregion
    }
}
