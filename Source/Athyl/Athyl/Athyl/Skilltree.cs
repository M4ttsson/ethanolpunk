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

        public int playerMaxHP;
        public float playerSpeed;
        public Vector2 playerJumpForce;
        public int ethanolConsumption;
        public float attackSpeed;

        public Skilltree(Player playerInfo)
        {
            this.fireRate = 0.1f;
            this.projectileSpeed = 0.025f;
            this.damage = 34;
            this.playerInfo = playerInfo;
            this.playerLevel = playerInfo.playerLevel;
        }

        //En enkel levelingfunktion, skjuta snabbare, snabbare kulor och mer skada
        public void Update()
        {
            if (playerLevel < playerInfo.playerLevel)
            {
                this.damage = damage * 1.5f;
                this.fireRate = fireRate * 0.9f;
                this.projectileSpeed = projectileSpeed * 1.5f;
                this.playerLevel = playerInfo.playerLevel;
            }
        }

        public void CloseRange()
        {
            this.damage = 50;
            fireRate = 0.9f;
            this.playerSpeed = 2.0f;
            this.playerJumpForce = new Vector2(0, -3);
            ethanolConsumption = 0;
        }

        public void MidRange()
        {
            damage = 34;
            playerSpeed = 1.0f;
            playerJumpForce = new Vector2(0, -2f);
            ethanolConsumption = 2;
            fireRate = 0.1f;

        }

        public void LongRange()
        {
            playerSpeed = 0.5f;
            ethanolConsumption = 10;
            fireRate = 0.7f;
            damage = 100;
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
