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
    class AI
    {

        Body enemyBody;

        public AI()
        {

        }
        /*
        //kass prototyp på move-towards AI.

        Vector2 right = new Vector2(2, 0);
        Vector2 left = new Vector2(-2, 0);

        public void towardsPlayer()
        {
            if (enemyBody.Position.X < playerBody.Position.X)
            {
                enemyBody.Position += right;
            }

            else if (enemyBody.Position.X > playerBody.Position.X)
            {
                enemyBody.Position += left;
            }

        }

        public void attackPlayer()
        {

        }

        public void UpdateEnemy()
        {
            towardsPlayer();
            attackPlayer();
        }

        */
    }
}
