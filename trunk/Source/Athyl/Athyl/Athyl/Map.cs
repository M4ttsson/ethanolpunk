using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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
    class Map
    {
        #region Properties
        public static float progress;
        public static float done;
        public static float BoundsX { get; private set; }
        public static float BoundsY { get; private set; }
        private World world;
        private Game1 game;
        private Color[] colors;
        private Color[,] colors2D;
        private List<DrawableGameObject> body = new List<DrawableGameObject>();
        private List<Texture2D> middleTexture = new List<Texture2D>();
        private List<Vector2> middlePosition = new List<Vector2>();
        private int middleTileNr = 1;
        private Random rand;
        private List<Texture2D> lvl1 = new List<Texture2D>();
        private List<Texture2D> lvl2 = new List<Texture2D>();
        private List<Texture2D> lvl3 = new List<Texture2D>();
        private Texture2D buttonTexture;
        private DrawableGameObject b;

        public int currentLevel;
        public DrawableGameObject button;
        #endregion
        #region Constructor
        public Map(World world, Game1 game)
        {
            this.world = world;
            this.game = game;
            
            currentLevel = 1;  //Vilken nivå? Ändra mellan 1-3 för att byta utseende på banan.

            rand = new Random();

            progress = 1;

            InializeMap();
        }
        #endregion
        #region Initialization
        public void InializeMap()
        {   
            ReadingTilesTextureToList();

            ReadMapLayout(game.Content.Load<Texture2D>("Lvl" + 2 + "Tiles/Map"));
            buttonTexture = game.Content.Load<Texture2D>("buttons/button");

            DrawTilesOnPlace();
        }

        private void CreateDrawableGameObject(int x, int y, int tileNumber, Vector2 BodySize, bool Collidable)
        {
            if (currentLevel == 1)                     //Sets the borders between the different levels in our game.
                b = new DrawableGameObject(world, lvl1[tileNumber], BodySize, 0, "ground");
            else if (currentLevel == 2)
                b = new DrawableGameObject(world, lvl2[tileNumber], BodySize, 0, "ground");
            else if (currentLevel == 3)
                b = new DrawableGameObject(world, lvl3[tileNumber], BodySize, 0, "ground");

            b.Position = new Vector2((x - 1) * 32 + 16, (y-1) * 32 + 16);
            b.body.BodyType = BodyType.Static;
            b.body.Awake = false;
            if (!Collidable)
            {
                for (int i = 0; i < b.body.FixtureList.Count; i++)
                {
                    b.body.DestroyFixture(b.body.FixtureList[i]);
                }
            }
            SetCollisionCategories(b, tileNumber);
            body.Add(b);
            randomizeMiddleTexture(x, y, BodySize);
        }

        private void randomizeMiddleTexture(int x, int y, Vector2 BodySize)
        {
            Texture2D t = game.Content.Load<Texture2D>("Lvl" + currentLevel + "Tiles/Middle" + middleTileNr);
            Vector2 p = new Vector2((x - 1) * 32, (y - 1) * 32);

            middleTexture.Add(t);
            middlePosition.Add(p);
        }

        private void SetCollisionCategories(DrawableGameObject b, int tileNumber)
        {
            if (tileNumber == 0)
                b.body.CollisionCategories = Category.Cat5;         //LeftUpperCorner
            else if (tileNumber == 1)
                b.body.CollisionCategories = Category.Cat5;         //Ground
            else if (tileNumber == 2)
                b.body.CollisionCategories = Category.Cat5;         //RightUpperCorner
            else if (tileNumber == 3)
                b.body.CollisionCategories = Category.Cat6;         //LeftWall
            else if (tileNumber == 4)
                b.body.CollisionCategories = Category.Cat6;         //Middle
            else if (tileNumber == 5)
                b.body.CollisionCategories = Category.Cat6;         //RightWall
            else if (tileNumber == 6)
                b.body.CollisionCategories = Category.Cat7;         //LeftDownCorner
            else if (tileNumber == 7)
                b.body.CollisionCategories = Category.Cat7;         //Ceiling
            else if (tileNumber == 8)
                b.body.CollisionCategories = Category.Cat7;         //RightDownCorner
        }
        #endregion
        #region Readings
        private void ReadingTilesTextureToList()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    lvl1.Add(game.Content.Load<Texture2D>("Lvl1Tiles/Lvl1" + (i + 1) + "x" + (j + 1)));
                    lvl2.Add(game.Content.Load<Texture2D>("Lvl2Tiles/Lvl2" + (i + 1) + "x" + (j + 1)));
                    lvl3.Add(game.Content.Load<Texture2D>("Lvl3Tiles/Lvl3" + (i + 1) + "x" + (j + 1)));
                }
            }
        }

        public void reloadMapTexture()
        {
            foreach (DrawableGameObject b in body)
            {
                world.RemoveBody(b.body);   
            }
            body.Clear();
            middleTexture.Clear();
            middlePosition.Clear();
            DrawTilesOnPlace();
        }

        /// <summary>
        /// Reads a color coded map from a texture 2d.
        /// </summary>
        /// <param name="map">Texture to read from</param>
        private void ReadMapLayout(Texture2D map)
        {
            //loading bar length
            done = map.Height * map.Width;

            //map bounds
            BoundsX = map.Width * 32;
            BoundsY = map.Height * 32;

            //This Reads the map layout
            colors = new Color[map.Width * map.Height];
            map.GetData<Color>(colors);

            colors2D = new Color[map.Width, map.Height];

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    colors2D[x, y] = colors[x + y * map.Width];
                }
            }
        }
        #endregion
        #region Draw
        private void DrawTilesOnPlace()
        {
            Vector2 bodySize = new Vector2(33f, 33f);

            for (int y = 0; y < 70; y++)
            {
                middleTileNr++;

                if (middleTileNr > 9)
                    middleTileNr = 1;

                for (int x = 0; x < 322; x++)
                {
                    if (colors2D[x, y] == new Color(255, 0, 0))             //Red  LeftUpperCorner
                    {
                        CreateDrawableGameObject(x, y, 0, bodySize, true);
                    }
                    else if (colors2D[x, y] == new Color(255, 255, 0))           //Yellow    Ground
                    {
                        CreateDrawableGameObject(x, y, 1, bodySize, true);
                    }
                    else if (colors2D[x, y] == new Color(0, 255, 0))             //Green     RightUpperCorner
                    {
                        CreateDrawableGameObject(x, y, 2, bodySize, true);
                    }
                    else if (colors2D[x, y] == new Color(0, 0, 255))             //DarkBlue  LeftWall
                    {
                        CreateDrawableGameObject(x, y, 3, bodySize, true);
                    }
                    else if (colors2D[x, y] == new Color(0, 0, 0))               //Black     Middle
                    {
                        CreateDrawableGameObject(x, y, 4, bodySize, false);
                    }
                    else if (colors2D[x, y] == new Color(0, 246, 255))           //LightBlue     RightWall
                    {
                        CreateDrawableGameObject(x, y, 5, bodySize, true);
                    }
                    else if (colors2D[x, y] == new Color(96, 57, 19))            //Brown     LeftDownCorner
                    {
                        CreateDrawableGameObject(x, y, 6, bodySize, true);
                    }
                    else if (colors2D[x, y] == new Color(83, 71, 65))            //Gray      Ceiling
                    {
                        CreateDrawableGameObject(x, y, 7, bodySize, true);
                    }
                    else if (colors2D[x, y] == new Color(77, 0, 68))             //Purple    RightDownCorner
                    {
                        CreateDrawableGameObject(x, y, 8, bodySize, true);
                    }
                    else if (colors2D[x, y] == new Color(121, 0, 0))                //Goal Tiles;
                    {
                        b = new DrawableGameObject(world, lvl3[0], bodySize, 0, "goal");
                        b.Position = new Vector2((x - 1) * 32 + 16, (y - 1) * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        b.body.Awake = false;
                        b.body.CollisionCategories = Category.Cat13;
                    }
                    else if (colors2D[x, y] == new Color(150, 150, 150))         //Quest Button
                    {
                        button = new DrawableGameObject(world, buttonTexture, 0, "button");
                        button.Position = new Vector2((x - 1) * 32 + 16, (y - 1) * 32 + 16);
                        button.body.BodyType = BodyType.Static;
                        b.body.Awake = false;
                        body.Add(button);
                    }
                    else if (colors2D[x, y] == new Color(255, 102, 0))           //Invicible walls
                    {
                        b = new DrawableGameObject(world, lvl3[0], bodySize, 0, "ground");
                        b.Position = new Vector2((x - 1) * 32 + 16, (y - 1) * 32 + 16);
                        b.body.BodyType = BodyType.Static;
                        b.body.Awake = false;
                        b.body.CollisionCategories = Category.Cat12;
                    }

                    progress++;
                }
                progress++;
            }
            RandomizeList();
        }

        public void RandomizeList()
        {
            int n = middlePosition.Count;

            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                Vector2 value = middlePosition[k];
                middlePosition[k] = middlePosition[n];
                middlePosition[n] = value;
            }
        }

        public void Draw(SpriteBatch spriteBatch)   
        {
            for (int i = 0; i < middlePosition.Count; i++)
            {
                spriteBatch.Draw(middleTexture[i], middlePosition[i], Color.White);
            }

            foreach (DrawableGameObject b in body)
            {
                b.Draw(spriteBatch);
            }
        }
        #endregion
    }
}
