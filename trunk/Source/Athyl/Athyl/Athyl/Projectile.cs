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
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"),new Vector2(10,2), 10, "shot");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bullets.Add(bullet);
            if (direction)
                bullets[bullets.Count-1].body.ApplyLinearImpulse(new Vector2(0.1f, 0.0f));
            else
                bullets[bullets.Count-1].body.ApplyLinearImpulse(new Vector2(-0.1f, 0.0f));
        }

        /// <summary>
        /// Updates and draws the projectiles
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (DrawableGameObject d in bullets){
                d.Draw(spriteBatch);
              //  Console.WriteLine(game.world.BodyList.Count);
            }
            
        }
    }
}
