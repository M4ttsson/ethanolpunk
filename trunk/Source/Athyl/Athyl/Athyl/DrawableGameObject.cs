using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    class DrawableGameObject
    {
        public Body body;
        public Texture2D texture;

        //converts to units or pixels before getting or setting
        public Vector2 Position
        {
            get { return ConvertUnits.ToDisplayUnits(body.Position); }
            set { body.Position = ConvertUnits.ToSimUnits(value); }
        }

        private Vector2 size;
        public Vector2 Size
        {
            get { return ConvertUnits.ToDisplayUnits(size); }
            set { size = ConvertUnits.ToSimUnits(value); }
        }

        public DrawableGameObject(World world, Texture2D texture, Vector2 size, float mass)
        {
            this.Size = size;
            this.texture = texture;
            

            body = BodyFactory.CreateRectangle(world, Size.X, Size.Y, 1);
            body.BodyType = BodyType.Dynamic;

            body.Mass = mass;
        }

        /// <summary>
        /// Draw the object to screen
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to draw with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle destination = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

            spriteBatch.Draw(texture, destination, null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), SpriteEffects.None, 0);
        }

    }
}
