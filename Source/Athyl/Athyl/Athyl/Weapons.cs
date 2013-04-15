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

        public Vector2 weaponPosition;
        public Texture2D weaponTexture;
        string currentTextureString = "AK47";
        //Body weaponBody;
        int bulletSpeed;
        int clipAmmo;
        float reloadSpeed;
        float bulletSpread;
        public int weaponId;
        //Hits per Second
        float meleeAtkSpeed;
        bool hasPiercing;
        DrawableGameObject drawGame;
        DrawableGameObject bulletBody;
        Game1 game;
        SpriteEffects myEffect = SpriteEffects.FlipVertically;

        public Weapons(World world)
        {


            //drawGame = new DrawableGameObject
            bulletSpeed = 0;
            clipAmmo = 0;
            reloadSpeed = 0;
            bulletSpread = 0;
            meleeAtkSpeed = 0;
            hasPiercing = false;
            weaponId = 1;
            //bulletBody = new DrawableGameObject(world, weaponTexture, new Vector2(10), 1, "bullet");

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


            
           /* weaponBody.BodyType = BodyType.Dynamic;
            weaponBody.IsBullet = true;
            weaponBody.Position = new Vector2(
            weaponBody.ApplyAngularImpulse(bulletSpeed);
            weaponBody.IsSensor = true;*/

            this.game = game;
            Texture = game.Content.Load<Texture2D>(currentTextureString);
            weaponTexture = Texture;
            Console.WriteLine(currentTextureString);

            bulletBody = new DrawableGameObject(world, weaponTexture, new Vector2(10), 1, "bullet");
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


        public void UpdateWeapon(GameTime gameTime)
        {
            

            KeyboardState kbState = Keyboard.GetState();
            changeTexture(game, weaponTexture); 
            if (kbState.IsKeyDown(Keys.P))
            {
              //  weaponBody.ApplyAngularImpulse(bulletSpeed);
            }

            
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
        }


        public void DrawWeapon(SpriteBatch spriteBatch)
        {
            
          /*  if (!game.player.Direction)
            {
                spriteBatch.Draw(weaponTexture, new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0), Color.White, 0, new Vector2(0), myEffect, 0);
            }*/
            spriteBatch.Draw(weaponTexture, weaponPosition, Color.White);
        }

    }
}
