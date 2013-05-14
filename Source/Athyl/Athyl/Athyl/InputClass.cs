using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Xna.Framework.Input;

namespace Athyl
{
    class InputClass
    {
        //keys
        public static Keys rightKey, leftKey, upKey, downKey, jumpKey, shootKey, closeKey, middleKey, longKey, crouchKey;

        private InputClass()
        {
        }

        public static void ReadConfig()
        {
            //get the config file
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            NameValueCollection nvc = (NameValueCollection)ConfigurationManager.GetSection("movement");

            //read all keys
            //movement
            String key = nvc.Get("Up");
            upKey = (Keys)Enum.Parse(typeof(Keys), key, true);
            key = nvc.Get("Left");
            leftKey = (Keys)Enum.Parse(typeof(Keys), key, true);
            key = nvc.Get("Down");
            downKey = (Keys)Enum.Parse(typeof(Keys), key, true);
            key = nvc.Get("Right");
            rightKey = (Keys)Enum.Parse(typeof(Keys), key, true);
            key = nvc.Get("Crouch");
            crouchKey = (Keys)Enum.Parse(typeof(Keys), key, true);

            //shoot/jump
            key = nvc.Get("Shoot");
            shootKey = (Keys)Enum.Parse(typeof(Keys), key, true);
            key = nvc.Get("Jump");
            jumpKey = (Keys)Enum.Parse(typeof(Keys), key, true);

            //stances
            key = nvc.Get("Close");
            closeKey = (Keys)Enum.Parse(typeof(Keys), key, true);
            key = nvc.Get("Middle");
            middleKey = (Keys)Enum.Parse(typeof(Keys), key, true);
            key = nvc.Get("Long");
            longKey = (Keys)Enum.Parse(typeof(Keys), key, true);

        }
    }
}
