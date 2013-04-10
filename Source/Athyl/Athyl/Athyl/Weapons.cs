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

        public Weapons(int weaponId, Texture2D Texture, Vector2 position)
        {
            this.weaponId = weaponId;

            if (weaponId == 0)
                sniper_r700();
            else if (weaponId == 1)
                assaultRifle_ak47();
            else if (weaponId == 2)
                furiousFisting();

        }

        public void sniper_r700()
        {
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
        }

        public void furiousFisting()
        {

            bulletSpeed = 100;
            reloadSpeed = -1;
            clipAmmo = -1;
            bulletSpread = 1.5f;

        }


    }
}
