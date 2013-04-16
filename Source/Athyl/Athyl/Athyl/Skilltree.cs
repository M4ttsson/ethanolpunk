using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Athyl
{
    class Skilltree
    {
        Player playerInfo;
        public float fireRate;
        public float projectileSpeed;

        public Skilltree()
        {
            fireRate = 0.1f;
            projectileSpeed = 0.02f;
        }

        #region MeleeStance

        public void fireBreath()
        {

        }

        public void increaseAtkSpd()
        {

        }

        public void increaseMovementSpd()
        {
        }

        public void increaseJumpHeight()
        {
        }




        #endregion

        #region MidRange
        public void fireBurst()
        {
        }

        //rate of fire
        public void increaseROF()
        {
            fireRate -= 0.005f;
        }

        //reloadspeed
        public void decreaseRLDSPD()
        {
        }

        public void increaseAmmoCap()
        {

        }

        public void bulletPenetration()
        {
        }
        #endregion

        #region Longrange
        public void shield()
        {

        }

        public void LaserSight()
        {
        }

        public void increaseSniperAmmo()
        {
        }

        public void increaseSniperMovSpd()
        {
        }

        public void shieldCdReduce()
        {
        }

        public void sniperAccuracy()
        {
        }

        public void headshotBonus()
        {
        }



        #endregion





    }
}
