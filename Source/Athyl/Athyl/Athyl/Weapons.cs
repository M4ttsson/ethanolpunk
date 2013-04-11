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

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
namespace Athyl
{
    class Weapons
    {

        Vector2 weaponPosition;
        public Texture2D weaponTexture;
        string currentTextureString = "AK47";

        int bulletSpeed;
        int clipAmmo;
        float reloadSpeed;
        float bulletSpread;
        public int weaponId;
        //Hits per Second
        float meleeAtkSpeed;
        
        bool hasPiercing;


        public Weapons()
        {
            bulletSpeed = 0;
            clipAmmo = 0;
            reloadSpeed = 0;
            bulletSpread = 0;
            meleeAtkSpeed = 0;
            hasPiercing = false;
        }

        public Weapons(Game1 game, int weaponIdParam, Texture2D Texture) // Vector2 position
        {
            weaponIdParam = this.weaponId;

            if (weaponIdParam == 0)
                sniper_r700();
            else if (weaponIdParam == 1)
                assaultRifle_ak47();
            else if (weaponIdParam == 2)
                furiousFisting();
            
            Texture = game.Content.Load<Texture2D>(currentTextureString);
            weaponTexture = Texture;

        }

        public void sniper_r700()
        {
            currentTextureString = "Sniper";

            bulletSpeed = 350;
            clipAmmo = 6;
            reloadSpeed = 0.8f;
            bulletSpread = 0.5f;
            
            /*
             * if(shotHasBeenFired() == true && clipAmmo != 0)
               {
             *      clipAmmo--
             *      reload();
             * }
             * 
             * 
             * */
        }

        public void assaultRifle_ak47()
        {
            bulletSpeed = 275;
            clipAmmo = 31;
            reloadSpeed = 2.5f;
            bulletSpread = 1.5f;
            currentTextureString = "AK47";
        }

        public void furiousFisting()
        {
            bulletSpeed = 100;
            reloadSpeed = -1;
            clipAmmo = -1;
            bulletSpread = 1.5f;

        }





        public void UpdateWeapon(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();

            //weaponPosition.X = player.torso.Position.X;
            //weaponPosition.Y = player.torso.Position.Y;

            if (kbState.IsKeyDown(Keys.D1))
            {
                weaponId = 0;
                sniper_r700();
            }

            else if (kbState.IsKeyDown(Keys.D2))
            {
                weaponId = 1;
                assaultRifle_ak47();
            }

            else if (kbState.IsKeyDown(Keys.D3))
            {

               // weaponId = 2;
               // currentTextureString = "M4A1";

            }

        }


        public void DrawWeapon(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(weaponTexture, weaponPosition, Color.White);
        }

    }
}
