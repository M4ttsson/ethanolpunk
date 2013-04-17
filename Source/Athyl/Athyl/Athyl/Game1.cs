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
using System.Threading;

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
        public SpriteBatch spriteBatch;
        DebugViewXNA debugView;
        public World world;
        List<AI> theAI = new List<AI>();
        public List<Damage> damageList = new List<Damage>();
        private List<AI> removedAIList = new List<AI>();
        Player player;
        static KeyboardState prevKeyboardState;
        static KeyboardState keyboardState;

        SpriteFont myFont;

        //tests
        Map map;
        DrawableGameObject floor;
        DrawableGameObject wallright;
        DrawableGameObject wallleft;
        DrawableGameObject box;
        Texture2D texture;
        Texture2D skyTexture;
        Menu menu;
        Sounds sound;

        Projectile projectile;
        Thread listenPauseThread;

        private bool paused = false;

        System.Timers.Timer timer;
        int runTime = 0;

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
            menu = new Menu(this);
            menu.gameState = Menu.GameState.Playing;
            IsMouseVisible = true;
            projectile = new Projectile(this);


            myFont = Content.Load<SpriteFont>("font");

            listenPauseThread = new Thread(ListenPause);
            listenPauseThread.IsBackground = true;
            listenPauseThread.Start();

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            base.Initialize();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            runTime++;
        }

        //listen for pause
        private void ListenPause()
        {
            while (true)
            {
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    Exit();
                }

                Thread.Sleep(20);
            }
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
            //texture = Content.Load<Texture2D>("testat");
            
            //sound = new Sounds(this);

            //sound.Play("castlevagina");


            //music.Stop();

            //player = new Player(world, Content.Load<Texture2D>("megaman3"), new Vector2(42, 56), 100, 20, new Vector2(430, 0));
            player = new Player(world, Content.Load<Texture2D>("TestGubbar"), new Vector2(55, 120), 100, new Vector2(600, 600), this, "player");
            skyTexture = Content.Load<Texture2D>("Sky");

            //foot contacts
            world.ContactManager.BeginContact += BeginContact;
            world.ContactManager.EndContact += EndContact;

            timer.Start();
        }

        private bool BeginContact(Contact contact)
        {

            if (contact.FixtureA.UserData.ToString() == "playerwheel")
            {
                player.numFootContacts++;
            }
            if (contact.FixtureB.UserData.ToString() == "playerwheel")
            {
                player.numFootContacts++;
            }

            return true;
        }

        private void EndContact(Contact contact)
        {
            if (contact.FixtureA.UserData.ToString() == "playerwheel")
            {
                player.numFootContacts--;
            }
            if (contact.FixtureB.UserData.ToString() == "playerwheel")
            {
                player.numFootContacts--;
            }
        }

        private void DamageAI()
        {
            for (int i = 0; i < theAI.Count; i++)
            {
                for (int j = 0; j < damageList.Count; j++)
                {
                    if (theAI[i].torso.body.BodyId == damageList[j].bodyId)
                    {
                        theAI[i].enemyHP -= (int)projectile.damage;
                        //(int)damageList[j].bodyId;
                        //Console.WriteLine(theAI[i].enemyHP);
                    }
                }

                if (theAI[i].enemyHP <= 0)
                {
                    
                    removedAIList.Add(theAI[i]);

                    if (removedAIList.Contains(theAI[i]))
                    {
                        world.RemoveBody(theAI[i].wheel.body);
                        world.RemoveBody(theAI[i].torso.body);
                        theAI.RemoveAt(i);
                        
                    }

                    player.playerXP += 1;
                }
            }
            damageList.Clear();
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
            
            menu.UpdateMenu(gameTime, this);
            if (menu.gameState == Menu.GameState.Paused)
            {
                timer.Stop();
               // menu.StartMenu(this);
            }

            else if (menu.gameState == Menu.GameState.Playing)
            {
                if (!timer.Enabled)
                {
                    timer.Start();
                }

                //Console.WriteLine(runTime);
                KeyboardState keyboardState = Keyboard.GetState();

                Input();

                if (runTime == 2 &&  theAI.Count < 1)
                {
                    theAI.Add(new AI(world, Content.Load<Texture2D>("RunningDummyEnemy"), new Vector2(55, 120), new Vector2(75, 400), 100, 20, this));
                }

                //sound.UpdateSound(gameTime);

                foreach (AI ai in theAI)
                {
                    ai.UpdateEnemy(player, world);
                }
                player.UpdatePlayer();
                DamageAI();

                world.Step(0.033333f);
   
            }

            base.Update(gameTime);
        }

        private void Input()
        {
            keyboardState = Keyboard.GetState();
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

            if (keyboardState.IsKeyDown(Keys.Z))
            {
                player.useWeapon(world);
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                player.Direction = 3;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                player.Direction = 2;
            }

            if (keyboardState.IsKeyDown(Keys.M) && prevKeyboardState.IsKeyDown(Keys.M))
            {
                theAI.Add(new AI(world, Content.Load<Texture2D>("RunningDummyEnemy"), new Vector2(55, 120), new Vector2(75, 400), 100, 20, this));
            }

            prevKeyboardState = keyboardState;

        }

       



        private void DrawText()
        {
            spriteBatch.DrawString(myFont, "Health:" + player.playerHP.ToString(), new Vector2(20, GraphicsDevice.Viewport.Height - 100), Color.DarkRed);
            spriteBatch.DrawString(myFont, "Ethanol:" +  player.playerAthyl.ToString(), new Vector2(20, GraphicsDevice.Viewport.Height - 70), Color.MidnightBlue);
            spriteBatch.DrawString(myFont, "Exp:" +  player.playerXP.ToString(), new Vector2(20, GraphicsDevice.Viewport.Height - 130), Color.Green);
            spriteBatch.DrawString(myFont, "Level:"+ player.playerLevel.ToString(), new Vector2(20, GraphicsDevice.Viewport.Height - 160), Color.Wheat);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(skyTexture, new Vector2(0, 0), Color.Wheat);
            player.Draw(spriteBatch);

            projectile.Draw(spriteBatch);
            //spriteBatch.Draw(weaponTexture, new Vector2(player.torso.Position.X - 18,player.torso.Position.Y - 10), Color.White); 
            foreach (AI ai in theAI)
                ai.Draw(spriteBatch);



            map.Draw(spriteBatch);
            menu.Draw(spriteBatch);



            //!!!!
            //!!!!
            //DON'T PUT ANY DRAWING STUFF AFTER THIS!!
            //Writes the players HP & Ethanol reserves into the game. 
            DrawText();
            //!!!!
            //!!!!
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}