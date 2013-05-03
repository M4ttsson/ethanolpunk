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
        float meleeAtkSpd, meleeMoveSpd, meleeDmg, meleeEthanolConsumption, midFireRate, midMoveSpd, midDmg, midEthanolConsumption, longMoveSpd, longEthanolConsumption, longAtkSpd, longDmg;
        Vector2 meleeJmpHeight;
        Vector2 midJmpHeight;
        Vector2 longJmpHeight;
        public float damage;
        int playerLevel;

        public int playerMaxHP;
        public float playerSpeed;
        public Vector2 playerJumpForce;
        public float ethanolConsumption;
        public float attackSpeed;

        public Skilltree(Player playerInfo)
        {
            this.fireRate = 0.1f;
            this.projectileSpeed = 0.025f;
            this.damage = 34;
            this.playerInfo = playerInfo;
            this.playerLevel = playerInfo.playerLevel;

            meleeAtkSpd = 0.3f;
            meleeMoveSpd = 2.0f; 
            meleeDmg = 50; 
            meleeJmpHeight = new Vector2(0, -3);
            meleeEthanolConsumption = 0;
            midFireRate = 0.1f;
            midMoveSpd = 1.0f;
            midDmg = 34;
            midJmpHeight = new Vector2(0, -2f);
            midEthanolConsumption = 2;
            longMoveSpd = 0.5f;
            longEthanolConsumption = 10;
            longAtkSpd = 0.7f;
            longDmg = 100;
            longJmpHeight = new Vector2(0, -2f);
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
            damage = meleeDmg;
            fireRate = meleeAtkSpd;
            playerSpeed = meleeMoveSpd;
            playerJumpForce = meleeJmpHeight;
            ethanolConsumption = meleeEthanolConsumption;
        }

        public void LevelCloseRange()
        {
            meleeDmg = meleeDmg * 1.5f;
            meleeAtkSpd = meleeAtkSpd * 1.1f;
            meleeMoveSpd = meleeMoveSpd * 1.1f;
            meleeJmpHeight = meleeJmpHeight + new Vector2(0.0f, 0.1f);
            meleeEthanolConsumption += 0.1f;
        }

        public void MidRange()
        {
            damage = midDmg;
            playerSpeed = midMoveSpd;
            playerJumpForce = midJmpHeight;
            ethanolConsumption = midEthanolConsumption;
            fireRate = midFireRate;

        }

        public void LevelMidRange()
        {
            midDmg = midDmg * 1.1f;
            midMoveSpd = midMoveSpd * 1.04f;
            midFireRate = midFireRate * 1.5f;
            midJmpHeight = midJmpHeight + new Vector2(0.0f, 0.3f);
            midEthanolConsumption = midEthanolConsumption * 1.1f;
        }

        public void LongRange()
        {
            damage = longDmg;
            playerSpeed = longMoveSpd;
            fireRate = longAtkSpd;
            ethanolConsumption = longEthanolConsumption;
            playerJumpForce = longJmpHeight;

        }

        public void LevelLongRange()
        {
            longDmg = longDmg * 1.3f;
            longMoveSpd = longMoveSpd * 1.01f;
            longAtkSpd = longAtkSpd * 1.01f;
            longEthanolConsumption = longEthanolConsumption * 1.4f;
            longJmpHeight = longJmpHeight + new Vector2(0.0f, 0.1f);
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
