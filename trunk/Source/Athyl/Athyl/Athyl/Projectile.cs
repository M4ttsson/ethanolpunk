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
        private float damage;
        public enum projectiletype { small, medium, large }
        List<DrawableGameObject> bullets = new List<DrawableGameObject>();
        Game1 game;
        List<DrawableGameObject> removeList = new List<DrawableGameObject>();
        List<Body> removeListbody = new List<Body>();
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
        public void NewBullet(Vector2 position, bool direction, World world){
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"),new Vector2(10,4), 10, "shot");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bullets.Add(bullet);
            if (direction)
                bullets[bullets.Count-1].body.ApplyLinearImpulse(new Vector2(0.1f, 0.0f));
            else
                bullets[bullets.Count-1].body.ApplyLinearImpulse(new Vector2(-0.1f, 0.0f));
            bullets[bullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (contact.IsTouching())
            {
                if (fixtureA.UserData.ToString() == "shot" && fixtureB.UserData.ToString() == "enemy" )
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
                    Console.WriteLine("removed");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates and draws the projectiles
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            
            for(int i = 0; i < bullets.Count; i++){
                bullets[i].Draw(spriteBatch);
                if (bullets[i].body.Position.X > ConvertUnits.ToSimUnits(game.graphics.PreferredBackBufferWidth) || bullets[i].body.Position.X < 0
                    || bullets[i].body.Position.Y > ConvertUnits.ToSimUnits(game.graphics.PreferredBackBufferHeight) || bullets[i].body.Position.Y < 0)
                {
                    removeList.Add(bullets[i]);
                    game.world.RemoveBody(bullets[i].body);
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
                game.world.RemoveBody(i);
                
            }
            removeListbody.Clear();
            removeList.Clear();         
        }
    }
}
