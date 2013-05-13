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
        //Int16 meleefirebreath, meleeAtkSpd, meleeMoveSpd, meleeJmpHeight, midFireBurst, midFireRate, midEthanoltank, midBulletPassthrough, longShield, longEthanol, longAccuracy, longHeadshotBonus;
        float meleeAtkSpd, meleeMoveSpd, meleeDmg, meleeEthanolConsumption, meleePlayerDmg, midFireRate, midMoveSpd, midDmg, midEthanolConsumption, midPlayerDmg, longMoveSpd, longEthanolConsumption, longAtkSpd, longDmg, longPlayerDmg, meleeMaxAthyl, midMaxAthyl, longMaxAthyl;
        Vector2 meleeJmpHeight;
        Vector2 midJmpHeight;
        Vector2 longJmpHeight;
        public float damage;
        int playerLevel;
        public int playerMaxHP = 100;
        public int playerMaxAthyl = 500;
        public float playerSpeed;
        public Vector2 playerJumpForce;
        public float ethanolConsumption;
        public float attackSpeed;
        public float maxHp = 100;
        public float maxAthyl;
        public float playerDmg = 1.0f;

        public Skilltree(Player playerInfo)
        {
            this.fireRate = 0.1f;
            this.projectileSpeed = 0.039f;
            this.damage = 34;
            this.playerInfo = playerInfo;
            this.playerLevel = playerInfo.playerLevel;
            

            meleeAtkSpd = 0.3f;
            meleeMoveSpd = 1.0f; 
            meleeDmg = 75; 
            meleeJmpHeight = new Vector2(0, -4.0f);
            meleeEthanolConsumption = 0;
            //Sets hp to 200
            meleePlayerDmg = 0.75f;
            meleeMaxAthyl = playerMaxAthyl;

            midFireRate = 0.13f;
            midMoveSpd = 1.0f;
            midDmg = 50;
            midJmpHeight = new Vector2(0, -3.8f);
            midEthanolConsumption = 2;
            //Sets hp to 150
            midPlayerDmg = 0.5f;
            midMaxAthyl = playerMaxAthyl;

            longMoveSpd = 0.6f;
            longEthanolConsumption = 10;
            longAtkSpd = 0.7f;
            longDmg = 200;
            longJmpHeight = new Vector2(0, -2.5f);
            //Sets hp to 100
            longPlayerDmg = 1.0f;
            longMaxAthyl = playerMaxAthyl;
        }

        

        public void CloseRange()
        {
            damage = meleeDmg;
            fireRate = meleeAtkSpd;
            playerSpeed = meleeMoveSpd;
            playerJumpForce = meleeJmpHeight;
            ethanolConsumption = meleeEthanolConsumption;
            playerDmg = meleePlayerDmg;
            maxAthyl = meleeMaxAthyl;
        }

        public void LevelCloseRange()
        {
            meleeDmg = meleeDmg * 1.5f;
            meleeAtkSpd = meleeAtkSpd * 0.99f;
            meleeMoveSpd = meleeMoveSpd * 1.1f;
            meleeJmpHeight = meleeJmpHeight - new Vector2(0.0f, 0.1f);
            meleeEthanolConsumption += 0.1f;
            meleePlayerDmg = meleePlayerDmg * 0.9f;
        }

        public void MidRange()
        {
            damage = midDmg;
            playerSpeed = midMoveSpd;
            playerJumpForce = midJmpHeight;
            ethanolConsumption = midEthanolConsumption;
            fireRate = midFireRate;
            playerDmg = midPlayerDmg;
            maxAthyl = midMaxAthyl;
        }

        public void LevelMidRange()
        {
            midDmg = midDmg * 1.1f;
            midMoveSpd = midMoveSpd * 1.04f;
            midFireRate = midFireRate - 0.01f;
            midJmpHeight = midJmpHeight - new Vector2(0.0f, 0.05f);
            midEthanolConsumption = midEthanolConsumption * 1.1f;
            midPlayerDmg = midPlayerDmg * 0.9f;
        }

        public void LongRange()
        {
            damage = longDmg;
            playerSpeed = longMoveSpd;
            fireRate = longAtkSpd;
            ethanolConsumption = longEthanolConsumption;
            playerJumpForce = longJmpHeight;
            playerDmg = longPlayerDmg;
            maxAthyl = longMaxAthyl;
        }

        public void LevelLongRange()
        {
            longDmg = longDmg * 1.3f;
            longMoveSpd = longMoveSpd * 1.01f;
            longAtkSpd = longAtkSpd * 0.99f;
            longEthanolConsumption = longEthanolConsumption * 1.4f;
            longJmpHeight = longJmpHeight - new Vector2(0.0f, 0.05f);
            longPlayerDmg = longPlayerDmg * 0.9f;
        }

        
        #region MeleeStance
        
        //public void fireBreath(Int16 points)
        //{
        //    meleefirebreath += points;
        //}

        public void increaseAtkSpd()
        {
            //firerate increases by 3% per level
             meleeAtkSpd += (fireRate / 33) * playerInfo.skillPoints/playerInfo.skillPoints;
             playerInfo.skillPoints--;
        }

        public void increaseMovementSpd()
        {

            meleeMoveSpd += (playerSpeed / 33) * playerInfo.skillPoints / playerInfo.skillPoints;
            playerInfo.skillPoints--;
        }

        public void increaseJumpHeight()
        {


            meleeJmpHeight += new Vector2(0, (playerJumpForce.Y / 20) * playerInfo.skillPoints / playerInfo.skillPoints);
            playerInfo.skillPoints--;
        }




        #endregion

        #region MidRange
        //public void fireBurst(Int16 points)
        //{
        //    midFireBurst = points;
        //}

        //rate of fire
        public void increaseROF()
        {

            fireRate += (fireRate / 25) * playerInfo.skillPoints / playerInfo.skillPoints;
            playerInfo.skillPoints--;
        }

        public void increaseAmmoCap()
        {

            playerInfo.playerAthyl += (playerInfo.playerAthyl / 10) * playerInfo.skillPoints / playerInfo.skillPoints;
            playerInfo.skillPoints--;
        }

        //public void bulletPenetration()
        //{
        //    midBulletPassthrough = points;
        //}
        #endregion

        #region Longrange
        //public void shield(Int16 points)
        //{
        //}

        public void increaseSniperAmmo()
        {
            playerInfo.playerAthyl += (playerInfo.playerAthyl / 33) * playerInfo.skillPoints / playerInfo.skillPoints;
            playerInfo.playerHP -= (playerInfo.playerHP / 50) * playerInfo.skillPoints / playerInfo.skillPoints;
            playerInfo.skillPoints--;
        }


        //Senare
        public void headshotBonus(Int16 points)
        {

        }
        

        #endregion
        
    }
}
