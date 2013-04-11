﻿using System;
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
        Body weaponBody;
        int bulletSpeed;
        int clipAmmo;
        float reloadSpeed;
        float bulletSpread;
        public int weaponId;
        //Hits per Second
        float meleeAtkSpeed;
        bool hasPiercing;
        DrawableGameObject drawGame;

        public Weapons()
        {


            //drawGame = new DrawableGameObject
            bulletSpeed = 0;
            clipAmmo = 0;
            reloadSpeed = 0;
            bulletSpread = 0;
            meleeAtkSpeed = 0;
            hasPiercing = false;
            weaponId = 1;
            //weaponBody  =

        }
        public Weapons( World world, Game1 game, int weaponIdParam, Texture2D Texture) // Vector2 position
        {
            this.weaponId = weaponIdParam;

            if (weaponIdParam == 0)
                sniper_r700();
            else if (weaponIdParam == 1)
                assaultRifle_ak47();
            else if (weaponIdParam == 2)
                furiousFisting();
            /*
            weaponBody.BodyType = BodyType.Dynamic;
            weaponBody.IsBullet = true;
            weaponBody.Position.Equals(weaponTexture);
            weaponBody.ApplyAngularImpulse(bulletSpeed);
            weaponBody.IsSensor = true;
            */

            Texture = game.Content.Load<Texture2D>(currentTextureString);
            weaponTexture = Texture;
            Console.WriteLine(currentTextureString);

        }

        public void changeTexture(Game1 game, Texture2D Texture)
        {
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
            currentTextureString = "FuriousFist";
            bulletSpeed = 100;
            reloadSpeed = -1;
            clipAmmo = -1;
            bulletSpread = 1.5f;

        }


        public void UpdateWeapon(GameTime gameTime, Game1 game, Player player)
        {
            

            KeyboardState kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.P))
            {
                weaponBody.ApplyAngularImpulse(bulletSpeed);
            }

            changeTexture(game, weaponTexture); 
            if (kbState.IsKeyDown(Keys.D1))
            {
                sniper_r700();
            }

            else if (kbState.IsKeyDown(Keys.D2))
            {
                assaultRifle_ak47();
            }

            else if (kbState.IsKeyDown(Keys.D3))
            {

                furiousFisting();

            }

            weaponPosition.X = player.torso.Position.X + 30;
            weaponPosition.Y = player.torso.Position.Y;
        }


        public void DrawWeapon(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(weaponTexture, weaponPosition, Color.White);
        }

    }
}
