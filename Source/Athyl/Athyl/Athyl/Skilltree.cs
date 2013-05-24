using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Athyl
{
    class Skilltree
    {
        public Player playerInfo;
        private int playerLevel;
        private Vector2 meleeJmpHeight;
        private Vector2 midJmpHeight;
        private Vector2 longJmpHeight;
        public Vector2 playerJumpForce;

        private float meleeFireRate, meleeMoveSpd, meleeDmg, meleeEthanolConsumption, meleePlayerDmg;
        private float midFireRate, midMoveSpd, midDmg, midEthanolConsumption, midPlayerDmg;
        private float longFireRate, longMoveSpd, longDmg, longEthanolConsumption, longPlayerDmg;
        private int meleeMaxAthyl, midMaxAthyl, longMaxAthyl;

        private ActiveSkills activeSkills;

        public Int16 firebreathPoint, AtkSpdPoint, AtkDmgPoint, DodgePoint;
        public Int16 FireBurstPoint, FastShotPoint, AthylPoint, PassthroughPoint;
        public Int16 ShieldPoint, KevlarPoint, AimPoint, ShieldCDPoint;



        //public int playerMaxHP;
        //public int playerMaxAthyl;
        public int maxHp;
        public int maxAthyl;

        public float fireRate;
        public float projectileSpeed;
        public float damage;
        public float playerSpeed;
        public float ethanolConsumption;
        public float playerDmg = 1.0f;
        public Skilltree(Player playerInfo, Game1 game)
        {
            activeSkills = new ActiveSkills();

            this.fireRate = 0;
            this.projectileSpeed = 0.039f;
            this.damage = 0;
            this.playerInfo = playerInfo;
            this.playerLevel = playerInfo.playerLevel;

            this.maxHp = 100;               //Set Hp and Athyl on player
            this.maxAthyl = 500;
            //this.playerMaxAthyl = this.maxAthyl;
            //this.playerMaxHP = this.maxHp;

            firebreathPoint = AtkSpdPoint = AtkDmgPoint = DodgePoint = 0;           //Close talentpoints
            FireBurstPoint = FastShotPoint = AthylPoint = PassthroughPoint = 0;     //Mid talentpoints
            ShieldPoint = KevlarPoint = AimPoint = ShieldCDPoint = 0;                   //Long talentpoints

            //Set default values for melee
            meleeFireRate = 0.3f;
            meleeMoveSpd = 1.4f; 
            meleeDmg = 75; 
            meleeJmpHeight = new Vector2(0, -28.0f);
            meleeEthanolConsumption = 0;
            meleePlayerDmg = 0.5f;
            meleeMaxAthyl = maxAthyl;

            //Set default values for melee
            midFireRate = 0.13f;
            midMoveSpd = 1.4f;
            midDmg = 50;
            midJmpHeight = new Vector2(0, -26.6f);
            midEthanolConsumption = 2;
            midPlayerDmg = 0.75f;
            midMaxAthyl = maxAthyl;

            //Set default values for melee
            longFireRate = 0.7f;
            longMoveSpd = 0.8f;
            longDmg = 200;
            longJmpHeight = new Vector2(0, -23.1f);
            longEthanolConsumption = 10;
            longPlayerDmg = 1.0f;
            longMaxAthyl = maxAthyl;
        }

        public void CloseRange()
        {
            damage = meleeDmg;
            fireRate = meleeFireRate;
            playerSpeed = meleeMoveSpd;
            playerJumpForce = meleeJmpHeight;
            ethanolConsumption = meleeEthanolConsumption;
            playerDmg = meleePlayerDmg;
            maxAthyl = meleeMaxAthyl;
        }

        public void MidRange()
        {
            damage = midDmg;
            fireRate = midFireRate;
            playerSpeed = midMoveSpd;
            playerJumpForce = midJmpHeight;
            ethanolConsumption = midEthanolConsumption;
            playerDmg = midPlayerDmg;
            maxAthyl = midMaxAthyl;
        }

        public void LongRange()
        {
            damage = longDmg;
            fireRate = longFireRate;
            playerSpeed = longMoveSpd;
            playerJumpForce = longJmpHeight;
            ethanolConsumption = longEthanolConsumption;
            playerDmg = longPlayerDmg;
            maxAthyl = longMaxAthyl;
        }

        #region MeleeStance
        public void increasefireBreath()        //Upgrades fireBreath
        {

        }

        public void increaseAtkSpd()            //Upgrades Attack speed
        {
            meleeFireRate -= (((meleeFireRate/100)*5) * AtkSpdPoint);
        }

        public void increaseAtkDmg()            //Upgrades Attack damage
        {
            meleeDmg += (((meleeDmg / 100) * 10) * AtkDmgPoint);
        }

        public void increaseDodge()             //Upgrades dodge chance
        {

        }
        #endregion

        #region MidRange
        public void increaseFireBurst()        //Upgrades fireburst
        {

        }

        public void increaseAthyl()        //Upgrades Athyl
        {
            midMaxAthyl += (((midMaxAthyl / 100) * 10) * AthylPoint);
        }

        public void increasePasstrough()        //Upgrades Passtrough
        {

        }

        public void increaseFastShot()        //Upgrades fastShot
        {
            midFireRate -= (((midFireRate / 100) * 5) * FastShotPoint);
        }
        #endregion

        #region Longrange
        public void increaseShield()        //Upgrades Shield
        {
            activeSkills.shieldHP += (int)((playerInfo.playerHP / 10) * ShieldPoint);
            activeSkills.shieldDuration += 2 * ShieldPoint;
        }

        public void increaseArmor()        //Upgrades armor
        {
            longPlayerDmg -= (((longPlayerDmg / 100) * 5) * KevlarPoint);
        }

        public void increaseAim()        //Upgrades Aim
        {

        }

        public void increaseShieldCD()        //Upgrades Shield by decreasing the cooldown
        {
            activeSkills.shieldCooldown -= ShieldCDPoint*2;
        }
        #endregion
        
    }
}
