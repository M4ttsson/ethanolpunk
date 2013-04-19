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
        private int projectileVelocity;
        private Vector2 projectileDirection;
        public float damage = 34;
        public enum projectiletype { small, medium, large }
        List<DrawableGameObject> bullets = new List<DrawableGameObject>();
        Game1 game;
        List<DrawableGameObject> removeList = new List<DrawableGameObject>();
        List<Body> removeListbody = new List<Body>();
        bool friendly;
        Random random = new Random();
        public Projectile(Game1 game)
        {
            this.game = game;
            

        }


        /// <summary>
        /// Adds a new bullet to the list
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="world"></param>
        public void NewBullet(Vector2 position, int direction, World world, float speed)
        {
            float spread = random.Next(-2, 3);
            spread /= 100;
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"),new Vector2(10,4), 10, "shot");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            
            bullet.body.IsSensor = true;
            //right
            if (direction == 0)
            {
                bullet.body.Rotation = MathHelper.ToRadians(180);
                bullet.body.FixedRotation = true;
                bullets.Add(bullet);
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed, spread * 0.2f));
            }
            //left
            else if(direction == 1)
            {
                bullet.body.FixedRotation = true;
                bullets.Add(bullet);
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed * -1, spread * 0.2f));
            }
            //up
            else if (direction == 2)
            {
                bullet.body.Rotation = MathHelper.ToRadians(90);
                bullet.body.FixedRotation = true;
                bullets.Add(bullet);
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(spread * 0.2f, speed));
            }
            //Down
            else if (direction == 3)
            {
                bullet.body.Rotation = MathHelper.ToRadians(270);
                bullet.body.FixedRotation = true;
                bullets.Add(bullet);
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(spread * 0.2f, speed * -1.0f));
            }

            //Upright
            else if (direction == 4)
            {
                bullet.body.Rotation = MathHelper.ToRadians(180);
                bullet.body.FixedRotation = true;
                bullets.Add(bullet);
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2, speed / 2 * -1.0f));
                
            }
            //Upleft
            else if (direction == 5)
            {
                bullet.body.FixedRotation = true;
                bullets.Add(bullet);
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2 * -1.0f, speed / 2 * -1.0f));
                
            }
            //Downright
            else if (direction == 6)
            {
                bullet.body.Rotation = MathHelper.ToRadians(90);
                bullet.body.FixedRotation = true;
                bullets.Add(bullet);
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed/2 , speed/2));
            }
            //Downleft  
            else if (direction == 7)
            {
                bullet.body.Rotation = MathHelper.ToRadians(270);
                bullet.body.FixedRotation = true;
                bullets.Add(bullet);
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2 * -1.0f, speed / 2));
            }


            bullets[bullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        public void NewEnemyBullet(Vector2 position, int direction, World world, float speed)
        {
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"), new Vector2(10, 4), 10, "hostile");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bullet.body.FixedRotation = true;
            bullets.Add(bullet);
            if (direction == 0)
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(0.05f, 0.0f));
            else
                bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(-0.05f, 0.0f));
            bullets[bullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "shot" && fixtureB.UserData.ToString() == "enemy")
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
            }

            return false;
        }

        /// <summary>
        /// Updates and draws the projectiles
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 playerPos)
        {
            
            for(int i = 0; i < bullets.Count; i++){
                bullets[i].Draw(spriteBatch);
                if (bullets[i].body.Position.X > playerPos.X + 1280 || bullets[i].body.Position.X < playerPos.X - 1280
                    || bullets[i].body.Position.Y > playerPos.Y + 720 || bullets[i].body.Position.Y < playerPos.Y - 720)
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
            foreach (DrawableGameObject i in removeList)
            {
                bullets.Remove(i);
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
        }
    }
}
