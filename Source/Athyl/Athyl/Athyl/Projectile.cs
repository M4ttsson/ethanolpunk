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
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Bullet"),new Vector2(10,4), 10, "shot");
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
            List<int> removeList = new List<int>();
            for(int i = 0; i < bullets.Count; i++){
                bullets[i].Draw(spriteBatch);
                if (bullets[i].body.Position.X > ConvertUnits.ToSimUnits(game.graphics.PreferredBackBufferWidth) || bullets[i].body.Position.X < 0
                    || bullets[i].body.Position.Y > ConvertUnits.ToSimUnits(game.graphics.PreferredBackBufferHeight) || bullets[i].body.Position.Y < 0)
                {
                    removeList.Add(i);
                    game.world.BodyList.Remove(bullets[i].body);
                }
                Console.WriteLine(game.world.BodyList.Count);
            }
            foreach (int i in removeList)
            {
                bullets.RemoveAt(i);
                //Console.WriteLine(bullets.Count);
            }
           
            
        }
    }
}
