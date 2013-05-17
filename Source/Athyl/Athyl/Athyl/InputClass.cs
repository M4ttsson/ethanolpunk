using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Xna.Framework.Input;
using NLog;


namespace Athyl
{
    class InputClass
    {
        //keys
        public static Keys rightKey, leftKey, upKey, downKey, jumpKey, shootKey, closeKey, middleKey, longKey, crouchKey, useKey;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private InputClass()
        {
        }

        public static bool ReadConfig()
        {
            try
            {
                  //read all keys
                //movement
                upKey = Properties.Settings.Default.Up;
                leftKey = Properties.Settings.Default.Left;
                downKey = Properties.Settings.Default.Down;
                rightKey = Properties.Settings.Default.Right;
                crouchKey = Properties.Settings.Default.Crouch;

                //shoot/jump
                shootKey = Properties.Settings.Default.Shoot;
                jumpKey = Properties.Settings.Default.Jump;

                //stances
                closeKey = Properties.Settings.Default.Close;
                middleKey = Properties.Settings.Default.Middle;
                longKey = Properties.Settings.Default.Long;

                useKey = Properties.Settings.Default.Use;

                return false;
            }
            catch (Exception ex)
            {
                logger.Fatal("InputClass " + ex.Message + "  " + ex.TargetSite);
                return true;
            }

        }
    }
}
