using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Athyl
{
    class Skilltree
    {
        Player playerInfo;
        public float fireRate;
        public float projectileSpeed;
        Int16 meleefirebreath, meleeAtkSpd, meleeMoveSpd, meleeJmpHeight, midFireBurst, midFireRate, midEthanoltank, midBulletPassthrough, longShield, longEthanol, longAccuracy, longHeadshotBonus;
        public float damage;
        int playerLevel;
        
        public Skilltree(Player playerInfo)
        {
            fireRate = 0.1f;
            projectileSpeed = 0.025f;
            damage = 34;
            this.playerInfo = playerInfo;
            playerLevel = playerInfo.playerLevel;
        }

        //En enkel levelingfunktion, skjuta snabbare, snabbare kulor och mer skada
        public void Update()
        {
            if (playerLevel < playerInfo.playerLevel)
            {
                damage = damage * 1.5f;
                fireRate = fireRate * 0.9f;
                projectileSpeed = projectileSpeed * 1.5f;
                playerLevel = playerInfo.playerLevel;
            }
        }

        public void CloseRange()
        {
        }

        public void MidRange()
        {
        }

        public void LongRange()
        {

        }

        /*
        #region MeleeStance

        public void fireBreath(Int16 points)
        {
            meleefirebreath += points;
            

        }

        public void increaseAtkSpd(Int16 points)
        {
            meleeAtkSpd = points;

            //firerate increases by 3% per level
            fireRate += (fireRate / 33) * meleeAtkSpd;
        }

        public void increaseMovementSpd(Int16 points)
        {
            meleeMoveSpd = points;
            playerInfo.speed = (playerInfo.speed / 33) * meleeMoveSpd;
        }

        public void increaseJumpHeight(Int16 points)
        {
            meleeJmpHeight = points;
            playerInfo.jumpForce += new Vector2(0,(playerInfo.jumpForce.Y / 20) * meleeJmpHeight); 
        }




        #endregion

        #region MidRange
        public void fireBurst(Int16 points)
        {
            midFireBurst = points;


        }

        //rate of fire
        public void increaseROF(Int16 points)
        {
            midFireRate = points;
            fireRate += (fireRate / 25) * midFireRate;
        }

        public void increaseAmmoCap(Int16 points)
        {
            midEthanoltank = points;
            playerInfo.playerAthyl += (playerInfo.playerAthyl / 10) * midEthanoltank;
        }

        public void bulletPenetration(Int16 points)
        {
            midBulletPassthrough = points;

        }
        #endregion

        #region Longrange
        public void shield(Int16 points)
        {
        }

        public void increaseSniperAmmo(Int16 points)
        {
            longEthanol = points;

            playerInfo.playerAthyl += (playerInfo.playerAthyl / 33) * longEthanol;
            playerInfo.playerHP -= (playerInfo.playerHP / 50) * longEthanol;
        }

        public void sniperAccuracy(Int16 points)
        {

        }

        public void headshotBonus(Int16 points)
        {

        }


        #endregion
        */
    }
}
