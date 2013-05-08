﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Athyl
{
    class Camera
    {
        #region Properties
        public static Matrix transform;
        private Vector3 moveDirection;
        private int x, y, z;
        private Viewport view;
        private bool endOfMapX = false;
        private bool endOfMapY = false;
        #endregion
        #region Constructor
        public Camera(Viewport view)
        {
            this.view = view;
            transform = Matrix.Identity;
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }
        #endregion
        #region Update
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public void UpdateCamera(Player player)
        {
            if (player != null)                     //this decides if your at the end of world or not.
            {
                if (player.torso.Position.X < 600 || player.torso.Position.X > 10240 - 600)
                    endOfMapX = true;
                else
                    endOfMapX = false;

                if (player.torso.Position.Y < 200 || player.torso.Position.Y > 2160 - 200)
                    endOfMapY = true;
                else
                    endOfMapY = false;


                if (endOfMapX)          //this is the cameracalculations if the end of map in x-axis
                {
                    if (transform.Translation.X > 0)
                    {
                        Vector3 temp = transform.Translation;
                        temp.X = 0;
                        transform.Translation = temp;
                    }
                    if (transform.Translation.X < -10240 + view.Width)
                    {
                        Vector3 temp = transform.Translation;
                        temp.X = -10240 + view.Width;
                        transform.Translation = temp;
                    }
                }
                else                           //this is the cameracalculations if not at the end of map in x-axis
                {
                    if (player.torso.Position.X < (-transform.Translation.X + 600))
                        x = (int)-player.torso.Position.X + 600;
                    if (player.torso.Position.X > (-transform.Translation.X + 1280 - 600))
                        x = (int)-player.torso.Position.X + 1280 - 600;
                    moveDirection = new Vector3(x, y, z);
                    transform = Matrix.CreateTranslation(moveDirection);
                }

                if (endOfMapY)                  //this is the cameracalculations if the end of map in y-axis
                {
                    if (transform.Translation.Y > 0)
                    {
                        Vector3 temp = transform.Translation;
                        temp.Y = 0;
                        //temp.Y = -2160 + view.Height;
                        transform.Translation = temp;
                    }
                    if (transform.Translation.Y < -2160 + view.Height)
                    {
                        Vector3 temp = transform.Translation;
                        temp.Y = -2160 + view.Height;
                        //temp.Y = 0;
                        transform.Translation = temp;
                    }
                   
                }
                else                                //this is the cameracalculations if not at the end of map in y-axis
                {
                    if (player.torso.Position.Y < (-transform.Translation.Y + 200))
                        y = (int)-player.torso.Position.Y + 200;
                    if (player.torso.Position.Y > (-transform.Translation.Y + 720 - 200))
                        y = (int)-player.torso.Position.Y + 720 - 200;
                    moveDirection = new Vector3(x, y, z);
                    transform = Matrix.CreateTranslation(moveDirection);
                }
            }
        }
        #endregion
    }
}