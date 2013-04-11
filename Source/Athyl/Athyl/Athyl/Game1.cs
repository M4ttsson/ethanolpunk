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
using System.Diagnostics;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace Athyl
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DebugViewXNA debugView;
        World world;
        List<AI> theAI = new List<AI>();
        Player player;
        KeyboardState prevKeyboardState;

        Weapons weapon;
        Vector2 weaponPosition;
        Texture2D weaponTexture;
        string currentTextureString = "Sniper";
        //tests
        Map map;
        DrawableGameObject floor;
        DrawableGameObject box;
        Texture2D texture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
                {

                      PreferredBackBufferHeight = 720,
                      PreferredBackBufferWidth = 1280,
                      IsFullScreen = false
                 };
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //create a world with normal gravity
            world = new World(new Vector2(0, 9.82f));

           // map = new Map(world, Content.Load<Texture2D>("middleground"));


            //map = new Map(world, Content.Load<Texture2D>("middleground"));



            debugView = new DebugViewXNA(world);
            debugView.LoadContent(GraphicsDevice, Content);
            texture = Content.Load<Texture2D>("testat");
            weaponTexture = Content.Load<Texture2D>(currentTextureString);

            floor = new DrawableGameObject(world, Content.Load<Texture2D>("testat"), new Vector2(GraphicsDevice.Viewport.Width, 100.0f), 1000, "ground");
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 50);
            floor.body.BodyType = BodyType.Static;

            weapon = new Weapons(0, Content.Load<Texture2D>(currentTextureString), new Vector2(50, 50));

            //player = new Player(world, Content.Load<Texture2D>("megaman3"), new Vector2(42, 56), 100, 20, new Vector2(430, 0));
            player = new Player(world, Content.Load<Texture2D>("RunningDummy"), new Vector2(55, 120), 100, 20, new Vector2(430, 0));


           // world.ContactManager.OnBroadphaseCollision += OnBroadPhaseCollision;
            //world.ContactManager.EndContact += OnBroadPhaseCollision;
            
        }

        public void OnBroadPhaseCollision(Contact contact)
        {

        }
        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            weaponPosition.X = player.torso.Position.X;
            weaponPosition.Y = player.torso.Position.Y;

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            

            KeyboardState keyboardState = Keyboard.GetState();

            /*Vector2 targetPosition;
            float minFrac = float.MaxValue;
            const float l = 11.0f;
            Vector2 point1 = player.torso.Position;
            Vector2 d = new Vector2(l * (float)Math.Cos(MathHelper.ToRadians(90)), l * (float)Math.Sin(MathHelper.ToRadians(90)));
            Vector2 point2 = point1 + d;

            Vector2 point = Vector2.Zero, normal = Vector2.Zero;

            
            world.RayCast((f, p, n, fr) =>
            {
                    if (fr < minFrac)
                    {
                        minFrac = fr;
                        targetPosition = p;
                    }
                    return fr;
            }, point1, point2);*/

            if (keyboardState.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
            {
                player.Jump();
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                player.Move(Player.Movement.Left);
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                player.Move(Player.Movement.Right);
            }
            else
            {
                player.Move(Player.Movement.Stop);
            }

            if (gameTime.TotalGameTime.TotalSeconds > 0.9f && gameTime.TotalGameTime.TotalSeconds < 0.95f)
            {
                theAI.Add(new AI(world, Content.Load<Texture2D>("RunningDummyEnemy"), new Vector2(55, 120), 100, 20));

                //Debug.WriteLine(theAI[0].enemyBody.Position.X + " Ai\n");

                //Debug.WriteLine(player.torso.Position.X + " Player\n");
                //theAI.Add(new AI(world, Content.Load<Texture2D>("megaman3"), new Vector2(42, 56), 100, 20));
            }


            foreach (AI ai in theAI)
            {
                ai.UpdateEnemy(player);
            }

            #region Textures
            //Vapenladdning
            
            if (keyboardState.IsKeyDown(Keys.D1))
            {
                weapon.weaponId = 0;
                currentTextureString = "Sniper";
            }

            else if (keyboardState.IsKeyDown(Keys.D2))
            {
                weapon.weaponId = 1;
                currentTextureString = "AK47";
            }

            else if (keyboardState.IsKeyDown(Keys.D3))
            {
                weapon.weaponId = 2;
                currentTextureString = "M4A1";

            }
            weaponTexture = Content.Load<Texture2D>(currentTextureString);

            #endregion


            //Debug.WriteLine(box.Position);
            prevKeyboardState = keyboardState;
            world.Step(0.033333f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // spriteBatch.Begin();
            // box.Draw(spriteBatch);
            // spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
          
            player.Draw(spriteBatch);
            spriteBatch.Draw(weaponTexture, new Vector2(player.torso.Position.X - 18,player.torso.Position.Y - 10), Color.White); 
            foreach (AI ai in theAI)
                ai.Draw(spriteBatch);

            floor.Draw(spriteBatch);
            //map.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}