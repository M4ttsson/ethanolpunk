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
        Viewport view;
        Vector2 centre;


        public Camera(Viewport newView)
        {
            view = newView;
        }


        public void UpdateCamera(GameTime gameTime, Player player)
        {
            if (player.torso.Position.Y > 360)
            {
                centre = new Vector2(player.torso.Position.X - 640, 0);
                transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(-centre.X, -centre.Y, 0));
            }

            else if (player.torso.Position.Y < 360)
            {
                centre = new Vector2(player.torso.Position.X - 640, player.torso.Position.Y - 360);
                transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                    Matrix.CreateTranslation(new Vector3(-centre.X, -centre.Y, 0));
            }
        }
    }
}
