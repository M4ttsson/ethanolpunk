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
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;

namespace Athyl
{
    class DrawableGameObject
    {
        public Body body { get; private set; }
        public Texture2D texture;
        private Vector2 origin;

        //converts to units or pixels before getting or setting
        public Vector2 Position
        {
            get { return CoordinateHelper.ToScreen(body.Position); }
            set { body.Position = CoordinateHelper.ToWorld(value); }
        }

        private Vector2 size;
        public Vector2 Size
        {
            get { return CoordinateHelper.ToScreen(size); }
            set { size = CoordinateHelper.ToWorld(value); }
        }

        /// <summary>
        /// Create a new rectangular object
        /// </summary>
        /// <param name="world">World to create the object in</param>
        /// <param name="texture">Texture for the object</param>
        /// <param name="size">Object size</param>
        /// <param name="mass">Mass</param>
        /// <param name="userdata">Type of object. For example enemy, player, weapon etc.</param>
        public DrawableGameObject(World world, Texture2D texture, Vector2 size, float mass, string userdata)
        {
            body = BodyFactory.CreateRectangle(world, size.X * CoordinateHelper.pixelToUnit, size.Y * CoordinateHelper.pixelToUnit, 1, userdata);
            body.BodyType = BodyType.Dynamic;
            this.Size = size;
            this.texture = texture;
            
        }

        /// <summary>
        /// Create a new rectangular object
        /// </summary>
        /// <param name="world">World to create the object in</param>
        /// <param name="texture">Texture for the object</param>
        /// <param name="verticeList">List of vertices</param>
        /// <param name="mass">Mass</param>
        /// <param name="userdata">Type of object. For example enemy, player, weapon etc.</param>
        public DrawableGameObject(World world, Texture2D texture, float mass, string userdata)
        {
            List<Vertices> verticeList = CreateCompoundPolygon(texture);
            body = BodyFactory.CreateCompoundPolygon(world, verticeList, 1, userdata);
            body.BodyType = BodyType.Dynamic;
            

            Vector2 size = new Vector2(texture.Width, texture.Height);
            this.Size = size;
            this.texture = texture;

        }

        /// <summary>
        /// Create a new circle object
        /// </summary>
        /// <param name="world">World to create the object in</param>
        /// <param name="texture">Texture for the object</param>
        /// <param name="size">Object size</param>
        /// <param name="mass">Mass</param>
        /// <param name="userdata">Type of object. For example enemy, player, weapon etc.</param>
        public DrawableGameObject(World world, Texture2D texture, float diameter, float mass, string userdata)
        {
            size = new Vector2(diameter, diameter);
            body = BodyFactory.CreateCircle(world, (diameter / 2.0f) * CoordinateHelper.pixelToUnit, 1, userdata);
            body.BodyType = BodyType.Dynamic;

            this.Size = size;
            this.texture = texture;
        }

        private List<Vertices> CreateCompoundPolygon(Texture2D polygonTexture)
        {
            uint[] data = new uint[polygonTexture.Width * polygonTexture.Height];

            polygonTexture.GetData(data);
            Vertices verts = PolygonTools.CreatePolygon(data, polygonTexture.Width, false);

            Vector2 centroid = -verts.GetCentroid();
            verts.Translate(ref centroid);

            origin = -centroid;
            verts = SimplifyTools.ReduceByDistance(verts, 4f);

            List<Vertices> verticeList = BayazitDecomposer.ConvexPartition(verts);

            Vector2 scale = new Vector2(ConvertUnits.ToSimUnits(1));
            foreach (Vertices vertices in verticeList)
            {
                vertices.Scale(ref scale);
            }

            return verticeList;
        }

        /// <summary>
        /// Draw the object to screen
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to draw with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle destination = new Rectangle
             (
                 (int)Position.X,
                 (int)Position.Y,
                 (int)Size.X,
                 (int)Size.Y
             );
            
             spriteBatch.Draw(texture, destination, null, Color.Wheat, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw object to screen and cover other things
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to draw with</param>
        /// <param name="Size">Size to draw</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 Size)
        {
            Rectangle destination = new Rectangle
             (
                 (int)Position.X,
                 (int)Position.Y,
                 (int)Size.X,
                 (int)Size.Y
             );
             spriteBatch.Draw(texture, destination, null, Color.Wheat, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), SpriteEffects.None, 0);
        }
    }
}
