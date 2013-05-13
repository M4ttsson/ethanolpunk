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
        //public Body crouchBody { get; private set; }
        public Texture2D texture;
        private Vector2 origin;

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

        public int Id
        {
            get;
            set;
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
            //float density = mass / (ConvertUnits.ToSimUnits(size.X * size.Y));
            body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(size.X), ConvertUnits.ToSimUnits(size.Y), 1, userdata);
            body.BodyType = BodyType.Dynamic;
            //body.Mass = mass;

            /* if (userdata.ToString() == "player")
             {
                 crouchBody = BodyFactory.CreateRectangle(world, 50 * CoordinateHelper.pixelToUnit, 40 * CoordinateHelper.pixelToUnit, 1, userdata);
                 crouchBody.BodyType = BodyType.Dynamic;
                 crouchBody.Enabled = false;
             }
             */

            this.Size = size;
            this.texture = texture;

        }



        /// <summary>
        /// Creates a new rectangular object
        /// </summary>
        /// <param name="world"></param>
        /// <param name="texture"></param>
        /// <param name="size"></param>
        /// <param name="mass"></param>
        /// <param name="userdata"></param>
        /// <param name="id">Makes it easy to find the right object</param> 
        public DrawableGameObject(World world, Texture2D texture, Vector2 size, float mass, string userdata, int id)
        {
            //float density = mass / (ConvertUnits.ToSimUnits(size.X * size.Y));
            body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(size.X), ConvertUnits.ToSimUnits(size.Y), 1, userdata);
            body.BodyType = BodyType.Dynamic;
            //body.Mass = mass;

            /* if (userdata.ToString() == "player")
             {
                 crouchBody = BodyFactory.CreateRectangle(world, 50 * CoordinateHelper.pixelToUnit, 40 * CoordinateHelper.pixelToUnit, 1, userdata);
                 crouchBody.BodyType = BodyType.Dynamic;
                 crouchBody.Enabled = false;
             }
             */
            this.Id = id;
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
            //body.Mass = mass;

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
            //float density = mass / (ConvertUnits.ToSimUnits(size.X * size.Y));
            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(diameter / 2.0f), 1, userdata);
            body.BodyType = BodyType.Dynamic;
            //body.Mass = mass;

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

            spriteBatch.Draw(texture, destination, null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), SpriteEffects.None, 0);
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
            spriteBatch.Draw(texture, destination, null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), SpriteEffects.None, 0);
        }
    }
}
