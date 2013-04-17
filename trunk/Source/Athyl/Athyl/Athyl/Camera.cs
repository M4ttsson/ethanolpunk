using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Athyl
{
    class Camera
    {
        public Matrix transform;
        private Vector3 moveDirection;
        private int x, y, z;
        private Viewport view;

        public Camera(Viewport view)
        {
            this.view = view;
            this.transform = Matrix.Identity;
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public void UpdateCamera(GameTime gameTime, Player player)
        {

            if (player.torso.Position.X < (-transform.Translation.X + 300))
                x = (int)-player.torso.Position.X + 300;
            if (player.torso.Position.X > (-transform.Translation.X + 1280 - 300))
                x = (int)-player.torso.Position.X + 1280 - 300;
            if (player.torso.Position.Y < (-transform.Translation.Y + 200))
                y = (int)-player.torso.Position.Y + 200;
            if (player.torso.Position.Y > (-transform.Translation.Y + 720 - 200))
                y = (int)-player.torso.Position.Y + 720 - 200;
        }
    }
}
