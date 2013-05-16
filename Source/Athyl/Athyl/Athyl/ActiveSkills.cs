using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Athyl
{
    class Shield
    {
        private DrawableGameObject shieldGfx;
        private int shieldHP;
        private int shieldDuration;
        public bool shieldActivate = false;

        public Shield(Game1 game)
        {
            shieldGfx = new DrawableGameObject(game.world, game.Content.Load<Texture2D>("Projectiles/Shield"), new Vector2(32, 32),  90, "shield");
            shieldGfx.body.FixedRotation = true;
        }


        public void UseShield(Player player, int shieldDuration)
        {
            this.shieldDuration = shieldDuration;

            if(player.direction == Player.Direction.Right)
            {
            shieldGfx.body.Position = new Vector2(player.torso.body.Position.X + 32, player.torso.body.Position.Y);
            }

            else
            {
                //Remember to update frameeee
                shieldGfx.body.Position = new Vector2(player.torso.body.Position.X - 32, player.torso.body.Position.Y);

            }
            shieldDuration = 20;
            shieldHP = (int)(player.playerHP * 0.75f);

            //Place shield, shield is drawable gameobject
            //Shield has static rotation
            //If shield collides with enemy bullet, subtract from an int called ShieldHp
            //If shield HP = 0 || shield duration <= 0
            //remove shield
        }


        public void UseFireBreath()
        {

        }

        public void UseFireBurst()
        {


        }


        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (shieldActivate)
            {
                shieldGfx.Draw(spriteBatch);
            }
        }
    }
}
