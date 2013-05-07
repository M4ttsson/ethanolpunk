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
        #region Properties
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public World world;
        List<AI> theAI = new List<AI>();
        public List<Damage> damageList = new List<Damage>();
        private List<AI> removedAIList = new List<AI>();
        Player player;
        public static KeyboardState prevKeyboardState;
        static KeyboardState keyboardState;
        Matrix projectionMatrix;
        SpriteFont myFont;
        //Quests quest;
        Camera camera;
        List<Drops> drops = new List<Drops>();
        private List<Drops> removedDropsList = new List<Drops>();
        static Map map;

        Texture2D skyTexture;
        //Texture2D progressBar;
        //Texture2D progressBarBorder;
        Menu menu;
        Sounds sound;
        Skilltree skilltree;
        Projectile projectile;
        Thread listenPauseThread;

           private bool timedXPisApplied = false;
        public Thread loadThread;
        private Texture2D playerTexture, enemyTexture;
        private float tempTime;
        List<Spawn> spawnpoints = new List<Spawn>(10);
        private int timedBonusXP;
        System.Timers.Timer timer;
        public static float runTime = 0;
        int i = 0;
        //private DrawableGameObject button;
        #endregion

        #region Constructor
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

            //create spawnpoints
            CreateSpawns();
        }
        #endregion

        #region InitializationRestartAndLoad
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            menu = new Menu(this);
            menu.gameState = Menu.GameState.StartMenu;
            IsMouseVisible = true;
            projectile = new Projectile(this);

            //skilltree = new Skilltree(player);
            myFont = Content.Load<SpriteFont>("font");

            //start a thread that listens for exit (debug)
            listenPauseThread = new Thread(ListenPause);
            listenPauseThread.IsBackground = true;
            listenPauseThread.Start();
            //quest = new Quests(world, this);
            IsFixedTimeStep = false;
            timer = new System.Timers.Timer(10);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            camera = new Camera(GraphicsDevice.Viewport);

            base.Initialize();
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

            //zero gravity
            //world = new World(Vector2.Zero);

            //sound = new Sounds(this);

            //sound.Play("Music/song1");
            // MediaPlayer.IsRepeating = true;
            //progressbar
            //progressBar = Content.Load<Texture2D>("ProgressBar");
            //progressBarBorder = Content.Load<Texture2D>("ProgressBarBorder");

            //music.Stop();

            enemyTexture = Content.Load<Texture2D>("Player/SoldierWalk");
            playerTexture = Content.Load<Texture2D>("Player/Gilliam");

            skyTexture = Content.Load<Texture2D>("Menu items/Background");

            //button = new DrawableGameObject(world, Content.Load<Texture2D>("buttons/button"), 0, "button");
            //button.Position = new Vector2(2743, 1390); 
            Restart();

            //foot contacts
            world.ContactManager.BeginContact += BeginContact;
            world.ContactManager.EndContact += EndContact;

       
            timer.Start();
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

            if (player != null)
            {
                world.RemoveBody(player.torso.body);
                world.RemoveBody(player.wheel.body);
            }
            player = null;

            //player = new Player(world, playerTexture, new Vector2(42, 90), 100, new Vector2(8385, 1000), this, "player");
            player = new Player(world, playerTexture, new Vector2(60, 88), 100, new Vector2(60, 1300), this, "player");

            //reset spawnpoints
            foreach (Spawn sp in spawnpoints)
            {
                if (sp.Visited)
                    sp.Visited = false;
            }

            if (drops.Count > 0)
            {
                foreach (Drops d in drops)
                {
                    world.RemoveBody(d.hpBox.body);
                    world.RemoveBody(d.ethanolBox.body);
                    
                }
            }

            drops.Clear();


            menu.totalTime = 0f;

            runTime = 0;

            camera = new Camera(graphics.GraphicsDevice.Viewport);
            camera.UpdateCamera(player);
/*
            if (quest != null)
            {
                world.RemoveBody(quest.boulder.body);
            }

            quest = new Quests(world, this);
            if (map != null)
                map.button.body.OnCollision += quest.InteractWithQuestItems;
 * */
        }
        #endregion

        #region Spawn
        //class for spawnpoints
        public class Spawn
        {
            public int Id { get; set; }
            public bool Visited { get; set; }
            public Rectangle SpawnTriggerRect { get; private set; }
            public Vector2[] SpawnPositions;

            public Spawn(int id, bool visit, Rectangle rect, Vector2[] positions)
            {
                Id = id;
                Visited = visit;
                SpawnTriggerRect = rect;
                SpawnPositions = positions;
            }
        }

        private void CreateSpawns()
        {
            //visited, spawnrectangle and enemyspawn position.
            spawnpoints.Add(new Spawn(1, false, new Rectangle(825, 890, 50, 120), new Vector2[] { new Vector2(40, 550), new Vector2(835, 300) }));
            spawnpoints.Add(new Spawn(2, false, new Rectangle(500, 25, 170, 550), new Vector2[] { new Vector2(1950, 425), new Vector2(2270, 290) }));
            spawnpoints.Add(new Spawn(3, false, new Rectangle(1440, 410, 320, 120), new Vector2[] { new Vector2(1650, 1225) }));
            spawnpoints.Add(new Spawn(4, false, new Rectangle(2235, 1180, 50, 120), new Vector2[] { new Vector2(3025, 1480), new Vector2(3200, 1545) }));
            spawnpoints.Add(new Spawn(5, false, new Rectangle(4205, 1980, 50, 120), new Vector2[] { new Vector2(4880, 2055), new Vector2(5050, 2055), new Vector2(5100, 1832), new Vector2(5050, 1610) }));
            spawnpoints.Add(new Spawn(6, false, new Rectangle(5500, 1480, 50, 450), new Vector2[] { new Vector2(6290, 1800), new Vector2(6630, 1765) }));
            spawnpoints.Add(new Spawn(7, false, new Rectangle(6190, 1550, 50, 330), new Vector2[] { new Vector2(7440, 1833), new Vector2(7825, 1800) }));
            spawnpoints.Add(new Spawn(8, false, new Rectangle(7865, 1480, 50, 240), new Vector2[] { new Vector2(8900, 1350), new Vector2(8900, 1065) }));
            spawnpoints.Add(new Spawn(9, false, new Rectangle(8785, 500, 50, 1000), new Vector2[] { new Vector2(9723, 1320) }));
        }

        private int SpawnEnemies()
        {
            foreach (Spawn sp in spawnpoints)
            {
                if (!sp.Visited)
                {
                    if (sp.SpawnTriggerRect.Contains((int)player.torso.Position.X, (int)player.torso.Position.Y))
                    {
                        switch (sp.Id)
                        {
                            case 1:
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[0], 100, 20, this, AI.Behavior.Patrol, "enemy"));
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[1], 100, 20, this, AI.Behavior.Turret, "enemy"));
                                break;

                            case 2:
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[0], 100, 20, this, AI.Behavior.Patrol, "enemy"));
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[1], 100, 20, this, AI.Behavior.Turret, "enemy"));
                                break;

                            case 3:
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[0], 100, 20, this, AI.Behavior.PatrolDistance, "enemy"));
                                break;

                            case 4:
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[0], 100, 20, this, AI.Behavior.Turret, "enemy"));
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[1], 100, 20, this, AI.Behavior.PatrolDistance, "enemy"));
                                break;

                            case 5:
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[0], 100, 20, this, AI.Behavior.PatrolDistance, "enemy"));
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[1], 100, 20, this, AI.Behavior.PatrolDistance, "enemy"));
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[2], 100, 20, this, AI.Behavior.Turret, "enemy"));
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[3], 100, 20, this, AI.Behavior.Turret, "enemy"));
                                break;

                            case 6:
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[0], 100, 20, this, AI.Behavior.Turret, "enemy"));
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[1], 100, 20, this, AI.Behavior.Turret, "enemy"));
                                break;

                            case 7:
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[0], 100, 20, this, AI.Behavior.Turret, "enemy"));
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[1], 100, 20, this, AI.Behavior.Patrol, "enemy"));
                                break;

                            case 8:
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[0], 100, 20, this, AI.Behavior.Patrol, "enemy"));
                                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), sp.SpawnPositions[1], 100, 20, this, AI.Behavior.PatrolDistance, "enemy"));
                                break;

                            case 9:
                                theAI.Add(new AI(world, enemyTexture, new Vector2(84, 120), sp.SpawnPositions[0], 100, 20, this, AI.Behavior.Boss, "boss"));
                                break;

                            default:
                                foreach (Vector2 pos in sp.SpawnPositions)
                                {
                                    theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), pos, 100, 20, this, AI.Behavior.None, "enemy"));
                                }
                                break;
                        }

                        sp.Visited = true;
                        return 1;
                    }
                }
            }
            return 0;
        }
        #endregion

        #region Input
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
        /// Handle all input
        /// </summary>
        private void Input()
        {
            keyboardState = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            //flying debug!
            /*if(keyboardState.IsKeyDown(Keys.Up))
                player.torso.body.ApplyForce(new Vector2(0, -3.0f));
            if (keyboardState.IsKeyDown(Keys.Down))
                player.torso.body.ApplyForce(new Vector2(0, 3));*/

            if (keyboardState.IsKeyDown(Keys.K) && !prevKeyboardState.IsKeyDown(Keys.K))
            {
                player.Jump();
            }

            if (keyboardState.IsKeyDown(Keys.D1))
            {
                player.Stance = Player.Stances.CloseRange;
            }
            else if (keyboardState.IsKeyDown(Keys.D2))
            {
                player.Stance = Player.Stances.MidRange;
            }
            else if (keyboardState.IsKeyDown(Keys.D3))
            {
                player.Stance = Player.Stances.LongRange;
            }

            if (keyboardState.IsKeyDown(Keys.J))
            {
                player.useWeapon(world);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {

                player.Move(Player.Movement.Left);
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    player.direction = Player.Direction.Upleft;
                }

                else if (keyboardState.IsKeyDown(Keys.S) && !player.Crouching)
                {
                    player.direction = Player.Direction.Downleft;
                }
            }

            else if (keyboardState.IsKeyDown(Keys.D))
            {
                player.Move(Player.Movement.Right);
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    player.direction = Player.Direction.Upright;
                }

                else if (keyboardState.IsKeyDown(Keys.S) && !player.Crouching)
                {
                    player.direction = Player.Direction.Downright;
                }
            }

            else if (keyboardState.IsKeyDown(Keys.W))
            {
                player.direction = Player.Direction.Up;
                player.Move(Player.Movement.Stop);
            }

            else if (keyboardState.IsKeyUp(Keys.LeftControl) && prevKeyboardState.IsKeyDown(Keys.LeftControl))// && prevKeyboardState.IsKeyDown(Keys.Up))
            {
                player.wheel.body.Enabled = true;
                player.Crouching = false;
            }

            else if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                player.wheel.body.Enabled = false;
                player.Crouching = true;
            }
            else if (!player.OnGround)
            {
                player.Move(Player.Movement.Stop);
            }
                


                    /*if(keyboardState.IsKeyDown(Keys.Left)){
                        player.direction = Player.Direction.Left;
                    }
                    else if(keyboardState.IsKeyDown(Keys.Right)){
                        player.direction = Player.Direction.Right;
                    }*/



            


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
                player.Crouching = false;
                //Vänder riktningen åt rätt håll ifall man har skjutit diagonalt, upp eller ner
                if (player.lastDirection)
                    player.direction = Player.Direction.Left;
                else
                    player.direction = Player.Direction.Right;


            }

            

            if (keyboardState.IsKeyDown(Keys.M) && prevKeyboardState.IsKeyDown(Keys.M))
            {

                theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), new Vector2(50, 1300), 100, 20, this, AI.Behavior.Patrol, "enemy"));

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
        #endregion

        #region CollisionAndDamage
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

        private void Damage()
        {
            
            for (int i = 0; i < theAI.Count; i++)
            {
                for (int j = 0; j < damageList.Count; j++)
                {
                    if (theAI[i].torso.body.BodyId == damageList[j].bodyId)
                    {
                        theAI[i].enemyHP -= (int)player.projectile.damage;
                        //(int)damageList[j].bodyId;
                        //Console.WriteLine(theAI[i].enemyHP);
                    }
                }


                ///<summary>
                ///Awards the player more exp depending on how fast he/she completed the level
                ///</summary>
                ///
                if (theAI[i].torso.body.FixtureList[0].UserData.ToString() == "boss" && theAI[i].enemyHP <= 0)
                {


                    player.playerXP += 30;
                    player.playerXP += (timedBonusXP * 50 / (int)menu.totalTime);

                    removedAIList.Add(theAI[i]);

                    if (removedAIList.Contains(theAI[i]))
                    {
                        world.RemoveBody(theAI[i].wheel.body);
                        world.RemoveBody(theAI[i].torso.body);
                        //theAI.RemoveAt(i);
                       
                    }

                }

                else if (theAI[i].enemyHP <= 0 && theAI[i].torso.body.FixtureList[0].UserData.ToString() != "boss")
                {

                    drops.Add(new Drops(this, world, player));

                    drops[drops.Count - 1].DropPickups(theAI[i]);



                    removedAIList.Add(theAI[i]);

                    if (removedAIList.Contains(theAI[i]))
                    {
                        world.RemoveBody(theAI[i].wheel.body);
                        world.RemoveBody(theAI[i].torso.body);
                        theAI.RemoveAt(i);

                    }


                    player.playerXP += 3;
                    timedBonusXP += 3;
                }


            }

            for (int i = 0; i < removedAIList.Count; i++)
            {
                theAI.Remove(removedAIList[i]);
            }

            foreach (var damage in damageList)
            {
                if (player.torso.body.BodyId == damage.bodyId)
                {
                    player.playerHP -= (int)(damage.damage / player.Difficulty);
                }
            }

            damageList.Clear();
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            Console.WriteLine(theAI.Count);

            if (player != null && player.Dead)
            {
                keyboardState = Keyboard.GetState();
                player.UpdatePlayer();

            }
            else if (player != null)
            {

                menu.UpdateMenu(gameTime, this, player);
                if (menu.gameState == Menu.GameState.Paused)
                {
                    timer.Stop();
                    // menu.StartMenu(this);
                }

                else if (menu.gameState == Menu.GameState.LevelUp)
                {
                    timer.Stop();

                }

                else if (menu.gameState == Menu.GameState.Playing)
                {
                    if (!timer.Enabled)
                    {
                        timer.Start();
                    }

                    Input();

                    if (SpawnEnemies() == 1)
                    {
                        for (int i = 0; i < theAI.Count; i++)
                        {
                            for (int j = 0; j < theAI.Count; j++)
                            {
                                theAI[i].torso.body.IgnoreCollisionWith(theAI[j].torso.body);
                                theAI[i].wheel.body.IgnoreCollisionWith(theAI[j].wheel.body);
                                theAI[i].torso.body.IgnoreCollisionWith(theAI[j].wheel.body);
                                theAI[i].wheel.body.IgnoreCollisionWith(theAI[j].torso.body);
                                //theAI[i].wheel.body.IgnoreCollisionWith(drops[j].ethanolBox.body);
                                //theAI[i].wheel.body.IgnoreCollisionWith(drops[j].hpBox.body);
                                //theAI[i].torso.body.IgnoreCollisionWith(drops[j].hpBox.body);
                                //theAI[i].torso.body.IgnoreCollisionWith(drops[j].ethanolBox.body);

                            }
                        }
                    }

                    if (runTime == 2 && theAI.Count < 0)
                    {

                        theAI.Add(new AI(world, enemyTexture, new Vector2(42, 90), new Vector2(300, 300), 100, 20, this, AI.Behavior.Patrol, "enemy"));


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
                        ai.UpdateEnemy(player, world, drops);


                    }

                    player.UpdatePlayer();
                    Damage();

                    camera.UpdateCamera(player);

                    world.Step(0.033333f);
                    removedDropsList.Clear();

                    foreach (var d in drops)
                    {
                        if (d.ethanolDrop)
                        {
                            removedDropsList.Add(d);




                        }

                        else if (d.hpDrop)
                        {
                            removedDropsList.Add(d);

                        }
                    }

                    for (int i = 0; i < removedDropsList.Count; i++)
                    {
                        world.RemoveBody(removedDropsList[i].hpBox.body);
                        world.RemoveBody(removedDropsList[i].ethanolBox.body);
                        drops.Remove(removedDropsList[i]);

                    }


                }
            }
        

            base.Update(gameTime);
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

            foreach (AI ai in theAI)
                ai.Draw(spriteBatch);

            if (player != null && menu.gameState == Menu.GameState.Playing)
            {
                player.Draw(spriteBatch);

                projectile.Draw(spriteBatch, player.torso.Position);
            }

            foreach(Drops d in drops)
            {
               d.Draw(spriteBatch);
                
            }

            

            //quest.DrawQuest(spriteBatch);


            //button.Draw(spriteBatch);
            if (player != null && menu.gameState == Menu.GameState.Playing)
            {
                menu.DrawPlayerInfo(spriteBatch, GraphicsDevice, player, myFont, gameTime);
            }

            menu.Draw(spriteBatch, this);
            //Writes out Game Over when the player dies
            if (player != null && player.Dead == true)
            {
                menu.gameState = Menu.GameState.GameOver;
                //spriteBatch.DrawString(myFont, "Game Over", new Vector2(-(int)Camera.transform.Translation.X + 590, -(int)Camera.transform.Translation.Y + 360), Color.DarkRed);
                //spriteBatch.DrawString(myFont, "Press 'R ' to restart", new Vector2(-(int)Camera.transform.Translation.X + 530, -(int)Camera.transform.Translation.Y + 400), Color.DarkRed);

            }


            spriteBatch.End();
            spriteBatch.Begin();
            
            //if (!menu.isLoading && menu.gameState == Menu.GameState.Loading)
            //{
            //    Rectangle bar = new Rectangle(425, GraphicsDevice.Viewport.Height - 200, (int)((Map.progress / Map.done) * 400), 40);
            //    Rectangle border = new Rectangle(425, GraphicsDevice.Viewport.Height - 200, 400, 40);
            //    spriteBatch.Draw(progressBarBorder, border, Color.White);
            //    spriteBatch.Draw(progressBar, bar, Color.White);
            //}

            



            //Uncomment if you want to check where the spawnpoints are visually
            /*foreach (Spawn sp in spawnpoints)
            {
                spriteBatch.Draw(progressBar, sp.SpawnTriggerRect, Color.White);
            }
            */
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            runTime += 0.1f;
        }
        #endregion
    }
}

