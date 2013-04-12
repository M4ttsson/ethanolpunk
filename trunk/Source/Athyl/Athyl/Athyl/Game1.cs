﻿using System;
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
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DebugViewXNA debugView;
        World world;
        List<AI> theAI = new List<AI>();
        Player player;
        KeyboardState prevKeyboardState;


        Weapons weapon;

        //tests
        Map map;
        DrawableGameObject floor;
        DrawableGameObject wallright;
        DrawableGameObject wallleft;
        DrawableGameObject box;
        Texture2D texture;
        Texture2D skyTexture;
        Menu startButtonPosition;
        Menu exitButtonPosition;
        Sounds music;

        private bool paused = false;
        

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

            weapon = new Weapons();
            IsMouseVisible = true;
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

              
            
            map = new Map(world, Content.Load<Texture2D>("middleground"));

            debugView = new DebugViewXNA(world);
            debugView.LoadContent(GraphicsDevice, Content);
            texture = Content.Load<Texture2D>("testat");
            //weaponTexture = Content.Load<Texture2D>(currentTextureString);
            
            music = new Sounds(this);
            music.Play("Title");
            //music.Stop();

            /*floor = new DrawableGameObject(world, Content.Load<Texture2D>("testat"), new Vector2(GraphicsDevice.Viewport.Width, 100.0f), 1000, "ground");
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 50);
            floor.body.BodyType = BodyType.Static;*/
            weapon = new Weapons(world, this, weapon.weaponId, weapon.weaponTexture);


           /* wallleft = new DrawableGameObject(world, Content.Load<Texture2D>("testat"), new Vector2(100.0f, 720), 1000, "wall");
            wallleft.Position = new Vector2(0, GraphicsDevice.Viewport.Height / 2.0f); 
            wallleft.body.BodyType = BodyType.Static;*/

           /* wallright = new DrawableGameObject(world, Content.Load<Texture2D>("testat"), new Vector2(100.0f, 720), 1000, "wall");
            wallright.Position = new Vector2(GraphicsDevice.Viewport.Width - 50, GraphicsDevice.Viewport.Height / 2.0f);
            wallright.body.BodyType = BodyType.Static;*/

            //weapon = new Weapons(0, Content.Load<Texture2D>(currentTextureString), new Vector2(50, 50));

            //player = new Player(world, Content.Load<Texture2D>("megaman3"), new Vector2(42, 56), 100, 20, new Vector2(430, 0));
            player = new Player(world, Content.Load<Texture2D>("RunningDummy"), new Vector2(55, 120), 100, 20, new Vector2(600, 600));
            skyTexture = Content.Load<Texture2D>("Sky");

            //foot contacts
            world.ContactManager.BeginContact += BeginContact;
            world.ContactManager.EndContact += EndContact;

        }

        private bool BeginContact(Contact contact)
        {

                if (contact.FixtureA.UserData.ToString() == "wheel")
                {
                    player.numFootContacts++;
                }
                if (contact.FixtureB.UserData.ToString() == "wheel")
                {
                    player.numFootContacts++;
                }

            return true;
        }

        private void EndContact(Contact contact)
        {
            if (contact.FixtureA.UserData.ToString() == "wheel")
            {
                player.numFootContacts--;
            }
            if (contact.FixtureB.UserData.ToString() == "wheel")
            {
                player.numFootContacts--;
            }
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
            //check if player foot is touching the ground
            if (player.numFootContacts < 1)
            {
                player.OnGround = false;
            }
            else
            {
                player.OnGround = true;
            }

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

            if (gameTime.TotalGameTime.TotalSeconds > 0.9f && gameTime.TotalGameTime.TotalSeconds < 1.2f && theAI.Count == 0)

            {
                theAI.Add(new AI(world, Content.Load<Texture2D>("RunningDummyEnemy"), new Vector2(55, 120), 100, 20));
            }
             

            weapon.UpdateWeapon(gameTime, this, player);

            foreach (AI ai in theAI)
            {
                ai.UpdateEnemy(player);
            }

            music.UpdateSound(gameTime);

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


            weapon.weaponPosition.X = player.torso.Position.X-55/2;
            weapon.weaponPosition.Y = player.torso.Position.Y;
            // spriteBatch.Begin();
            // box.Draw(spriteBatch);
            // spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(skyTexture, new Vector2(0, 0), Color.Wheat);
            player.Draw(spriteBatch);
            weapon.DrawWeapon(spriteBatch);
            //spriteBatch.Draw(weaponTexture, new Vector2(player.torso.Position.X - 18,player.torso.Position.Y - 10), Color.White); 
            foreach (AI ai in theAI)
                ai.Draw(spriteBatch);


            //floor.Draw(spriteBatch);
            /*wallleft.Draw(spriteBatch);
            wallright.Draw(spriteBatch);*/
            map.Draw(spriteBatch);
            //Debug.WriteLine(player.torso.Position);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}