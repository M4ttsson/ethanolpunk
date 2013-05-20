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

using NLog;

namespace Athyl
{
    class Bullet
    {
        public DrawableGameObject Obj;
        public float LifeTime;
        public float WasFired;
        public float Damage;

        private Game1 game;
        private World world;
        private Player player;
        

        public Bullet()
        {

        }

        public Bullet(DrawableGameObject Obj, float Damage, Game1 game, World world, float LifeTime)
        {
            this.Obj = Obj;
            this.Damage = Damage;
            this.game = game;
            this.world = world;
            this.WasFired = Game1.runTime;
            this.LifeTime = LifeTime;
        }

        ~Bullet()
        {
            world.RemoveBody(Obj.body);
        }

    }
}
