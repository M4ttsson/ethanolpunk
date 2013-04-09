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
        int weaponId;
        int clipAmmo;
        float reloadSpeed;
        float bulletSpread;

        //Hits per Second
        float meleeAtkSpeed;
        
        bool hasPiercing;


        public Weapons()
        {

        }


        public void shootBullet(DrawableGameObject weapon)
        {

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
             * if(clipAmmo == 0)
             * {
             *     reloadTotal();
             * }
             * 
             * */
        }

        
        public void assaultRifle_m4a1()
        {

        }

        public void assaultRifle_ak47()
        {

        }

        public void furiousFisting()
        {
            meleeAtkSpeed = 3;
        }
    }
}
