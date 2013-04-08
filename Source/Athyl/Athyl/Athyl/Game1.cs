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
using FarseerPhysics.Factories;
using FarseerPhysics.DebugViews;

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
        AI theAI;
        Player player;
        KeyboardState prevKeyboardState;

        //tests
        DrawableGameObject floor;
        DrawableGameObject box;
        Texture2D texture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
                {
                      PreferredBackBufferHeight = 800,
                      PreferredBackBufferWidth = 800,
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
            

            debugView = new DebugViewXNA(world);
            debugView.LoadContent(GraphicsDevice, Content);
            texture = Content.Load<Texture2D>("testat");

            floor = new DrawableGameObject(world, Content.Load<Texture2D>("testat"), new Vector2(GraphicsDevice.Viewport.Width, 100.0f), 1000);
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 50);
            floor.body.BodyType = BodyType.Static;

            theAI = new AI(world, Content.Load<Texture2D>("megaman"), new Vector2(42, 56), 100, 20, new Vector2(600, 0));
            player = new Player(world, Content.Load<Texture2D>("megaman"), new Vector2(42, 56), 100, 20, new Vector2(430, 0));
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
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            KeyboardState keyboardState = Keyboard.GetState();

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


            theAI.UpdateEnemy(player);


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
            theAI.Draw(spriteBatch);


            floor.Draw(spriteBatch);
            spriteBatch.End();

           // spriteBatch.Begin();
           //// floor.Draw(spriteBatch);
           // spriteBatch.End();
            //var projection = Matrix.CreateOrthographicOffCenter(
            // 0f,
            // ConvertUnits.ToSimUnits(graphics.GraphicsDevice.Viewport.Width),
            // ConvertUnits.ToSimUnits(graphics.GraphicsDevice.Viewport.Height), 0f, 0f,
            // 1f);
            //debugView.RenderDebugData(ref projection);
     

            base.Draw(gameTime);
        }
    }
}
