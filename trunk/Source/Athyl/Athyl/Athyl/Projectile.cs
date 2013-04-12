using System;
using System.Collections.Generic;
using System.Linq;
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
    class Projectile
    {
        private int projectileVelocity;
        private Vector2 projectileDirection;
        private float damage;
        public enum projectiletype { small, medium, large }

        public Projectile(projectiletype type)
        {
            if (type == projectiletype.small)
            {
                projectileVelocity = 5;
                damage = 10;

                
            }

            else if (type == projectiletype.medium)
            {
                projectileVelocity = 10;
                damage = 20;
            }

            else if (type == projectiletype.large)
            {
                projectileVelocity = 15;
                damage = 30;
            }
        }


    }
}
