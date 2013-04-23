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
using FarseerPhysics.Common.PolygonManipulation;


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
        Matrix projectionMatrix;
        SpriteFont myFont;

        Camera camera;


        static Map map;
        DrawableGameObject floor;
        DrawableGameObject wallright;
        DrawableGameObject wallleft;
        DrawableGameObject box;
        Texture2D texture;
        Texture2D skyTexture;
        Texture2D progressBar;
        Texture2D progressBarBorder;
        Menu menu;
        Sounds sound;
        Skilltree skilltree;
        Projectile projectile;
        Thread listenPauseThread;

        public Thread loadThread;
        private bool paused = false;
        private Texture2D playerTexture, enemyTexture;

        List<Rectangle> spawnpoints = new List<Rectangle>(10);

        System.Timers.Timer timer;
        public static int runTime = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
                {
                    PreferredBackBufferHeight = 720,
                    PreferredBackBufferWidth = 1280,
                    IsFullScreen = false
                };
            Content.RootDirectory = "Content";

            loadThread = new Thread(Load);
            loadThread.IsBackground = true;

            CreateSpawns();
        }

        private void CreateSpawns()
        {
            spawnpoints.Add(new Rectangle(825, 921, 50, 100));
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
            menu.gameState = Menu.GameState.StartMenu;
            IsMouseVisible = true;
            projectile = new Projectile(this);

            skilltree = new Skilltree();
            myFont = Content.Load<SpriteFont>("font");

            listenPauseThread = new Thread(ListenPause);
            listenPauseThread.IsBackground = true;
            listenPauseThread.Start();
            IsFixedTimeStep = false;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            camera = new Camera(GraphicsDevice.Viewport);
            
           
            base.Initialize();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            runTime++;
        }

        /// <summary>
        /// Load all performance heavy things
        /// </summary>
        public void Load()
        {
            map = new Map(world, this);
            menu.gameState = Menu.GameState.Playing;
            menu.isLoading = true;
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
                if (keyboardState.IsKeyDown(Keys.R))
                {
                    Restart();
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

            //map = new Map(world, this);

            debugView = new DebugViewXNA(world);
            debugView.LoadContent(GraphicsDevice, Content);
            //texture = Content.Load<Texture2D>("testat");
            
            //sound = new Sounds(this);

            //sound.Play("castlevagina");

            //progressbar
            progressBar = Content.Load<Texture2D>("ProgressBar");
            progressBarBorder = Content.Load<Texture2D>("ProgressBarBorder");

            //music.Stop();

            enemyTexture = Content.Load<Texture2D>("RunningDummyEnemy");
            playerTexture = Content.Load<Texture2D>("TestGubbar");

            skyTexture = Content.Load<Texture2D>("Background");
            player = new Player(world, playerTexture, new Vector2(42, 90), 100, new Vector2(30, 1300), this, "player");

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
            if (contact.FixtureA.UserData.ToString() == "player")
            {
                player.numSideContacts++;
            }
            if (contact.FixtureB.UserData.ToString() == "player")
            {
                player.numSideContacts++;
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
            if (contact.FixtureA.UserData.ToString() == "player")
            {
                player.numSideContacts--;
            }
            if (contact.FixtureB.UserData.ToString() == "player")
            {
                player.numSideContacts--;
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


                    player.playerXP += 3;

                }
            }
            damageList.Clear();
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Restart the game
        /// </summary>
        private void Restart()
        {
            foreach (AI ai in theAI)
            {
                world.RemoveBody(ai.wheel.body);
                world.RemoveBody(ai.torso.body);
            }
            theAI.Clear();

            world.RemoveBody(player.torso.body);
            world.RemoveBody(player.wheel.body);
            player = null;


            player = new Player(world, playerTexture, new Vector2(42, 90), 100, new Vector2(30, 1300), this, "player");

            runTime = 0;

            camera = new Camera(graphics.GraphicsDevice.Viewport);
            camera.UpdateCamera(player);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (player != null && player.Dead)
            {
                keyboardState = Keyboard.GetState();
                player.UpdatePlayer();
            }
            else if (player != null)
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

                    Input();

                    //SpawnEnemies();

                    if (runTime == 2 && theAI.Count < 0)
                    {
                        theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), new Vector2(300, 300), 100, 20, this));
                        for (int i = 0; i < theAI.Count; i++)
                        {
                            for (int j = 0; j < theAI.Count; j++)
                            {
                                theAI[i].torso.body.IgnoreCollisionWith(theAI[j].torso.body);
                                theAI[i].wheel.body.IgnoreCollisionWith(theAI[j].wheel.body);
                                theAI[i].torso.body.IgnoreCollisionWith(theAI[j].wheel.body);
                                theAI[i].wheel.body.IgnoreCollisionWith(theAI[j].torso.body);
                            }
                        }


                    }
                    //sound.UpdateSound(gameTime);

                    foreach (AI ai in theAI)
                    {
                        ai.UpdateEnemy(player, world);
                    }
                    player.UpdatePlayer();
                    DamageAI();

                    camera.UpdateCamera(player);

                    world.Step(0.033333f);
                }
            }

            base.Update(gameTime);
        }

        private void SpawnEnemies()
        {
            if (spawnpoints[0].Contains(new Point((int)player.torso.Position.X, (int)player.torso.Position.Y)) && theAI.Count <= 2)
            {
                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), new Vector2(25, 550), 100, 20, this));
                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), new Vector2(835, 300), 100, 20, this));
            }
        }

        /// <summary>
        /// Handle all input
        /// </summary>
        private void Input()
        {
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
            {
                player.Jump();
            }

            if (keyboardState.IsKeyDown(Keys.Down) && prevKeyboardState != keyboardState)
            {
                player.Crouching = false;
            }
            else if (keyboardState.IsKeyUp(Keys.Down) && prevKeyboardState != keyboardState)
            {
                player.Crouching = false;
            }

            if (keyboardState.IsKeyDown(Keys.Z))
            {
                player.useWeapon(world);
            }


            if (keyboardState.IsKeyDown(Keys.Left))
            {
                player.Move(Player.Movement.Left);
                if(keyboardState.IsKeyDown(Keys.Up)){
                    player.direction = Player.Direction.Upleft;
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {
                    player.direction = Player.Direction.Downleft;
                }
                else
                    player.direction = Player.Direction.Left;
            }

            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                player.Move(Player.Movement.Right);
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    player.direction = Player.Direction.Upright;
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {
                    player.direction = Player.Direction.Downright;
                }
                else
                    player.direction = Player.Direction.Right;
            }           
           
            else if (keyboardState.IsKeyDown(Keys.Up))
            {

                player.direction = Player.Direction.Up;
                player.Move(Player.Movement.Stop);
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {

                player.direction = Player.Direction.Down;
                player.Move(Player.Movement.Stop);
            }

            //Logik för att kunna skjuta diagonalt när man står still, men det funkar dåligt
            /*else if(keyboardState.IsKeyDown(Keys.X)){
                if(player.Direction == 0){
                    player.Direction = 4;
                    if(keyboardState.IsKeyDown(Keys.Down)){
                        player.Direction = 6;
                    }
                    else if(keyboardState.IsKeyDown(Keys.Up)){
                        player.Direction = 4;
                    }
                }
                else if(player.Direction == 1){
                    player.Direction = 5;
                    if(keyboardState.IsKeyDown(Keys.Down)){
                        player.Direction = 7;
                    }
                    else if(keyboardState.IsKeyDown(Keys.Up)){
                        player.Direction = 5;
                    }
                }

            }*/

            else
            {
                player.Move(Player.Movement.Stop);
                //Vänder riktningen åt rätt håll ifall man har skjutit diagonalt, upp eller ner
                if (player.lastDirection)
                    player.direction = Player.Direction.Left;
                else
                    player.direction = Player.Direction.Right;


            } 

            if (keyboardState.IsKeyDown(Keys.M) && prevKeyboardState.IsKeyDown(Keys.M))
            {
                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), new Vector2(20, 1300), 100, 20, this));

                for (int i = 0; i < theAI.Count; i++)
                {
                    for (int j = 0; j < theAI.Count; j++)
                    {
                        theAI[i].torso.body.IgnoreCollisionWith(theAI[j].torso.body);
                        theAI[i].wheel.body.IgnoreCollisionWith(theAI[j].wheel.body);
                        theAI[i].torso.body.IgnoreCollisionWith(theAI[j].wheel.body);
                        theAI[i].wheel.body.IgnoreCollisionWith(theAI[j].torso.body);
                    }
                }
            }

            prevKeyboardState = keyboardState;

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.transform);
            spriteBatch.Draw(skyTexture, new Vector2(-Camera.transform.Translation.X, 0), Color.Wheat);
            if (map != null)
                map.Draw(spriteBatch);

            if (player != null)
            {
                player.Draw(spriteBatch);

                projectile.Draw(spriteBatch, player.torso.Position);
            }

            foreach (AI ai in theAI)
                ai.Draw(spriteBatch);

            if(player != null)
                menu.DrawPlayerInfo(spriteBatch, GraphicsDevice, player, myFont);
            
            menu.Draw(spriteBatch, this);

            if (!menu.isLoading && menu.gameState != Menu.GameState.StartMenu)
            {
                Rectangle bar = new Rectangle(425, GraphicsDevice.Viewport.Height - 200, (int)((Map.progress / Map.done) * 400), 40);
                Rectangle border = new Rectangle(425, GraphicsDevice.Viewport.Height - 200, 400, 40);
                spriteBatch.Draw(progressBarBorder, border, Color.White);
                spriteBatch.Draw(progressBar, bar, Color.White);
                //spriteBatch.DrawString(myFont, "Loading", new Vector2(500, GraphicsDevice.Viewport.Height - 240), Color.DarkRed);
            }

            //Writes out Game Over when the player dies
            if (player.Dead == true)
            {
                spriteBatch.DrawString(myFont, "Game Over", new Vector2(-(int)Camera.transform.Translation.X + 590, -(int)Camera.transform.Translation.Y + 360), Color.DarkRed);
            }

            //!!!!
            //!!!!
            //DON'T PUT ANY DRAWING STUFF AFTER THIS!!
            //Writes the players HP & Ethanol reserves into the game. 

            //!!!!
            //!!!!
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}