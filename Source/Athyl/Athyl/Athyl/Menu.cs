
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.Threading;

namespace Athyl
{
    class Menu
    {
        #region Properties
        public enum GameState { StartMenu, Loading, Playing, Paused, Options, Story }

        private Texture2D startButton;
        private Texture2D exitButton;
        private Texture2D pauseButton;
        private Texture2D resumeButton;
        private Texture2D optionsButton;
        private Texture2D storyButton;
        private Texture2D saveButton;
        private Texture2D LoadButon;
        private Texture2D loadingGameButton;
        private Texture2D pauseMenuBackgroundFront;
        private Texture2D pauseMenuBackgroundBack;
        private Texture2D loadingBackground;
        private Texture2D soundSlider;
        private Texture2D musicSlider;
        private Texture2D soundSliderBackground;
        private Texture2D musicSliderBackground;

        public float totalTime = 0;
        private Texture2D originalResumeButton;
        private Texture2D originalOptionsButton;
        private Texture2D originalStartButton;
        private Texture2D originalExitButton;
        private Texture2D originalPauseButton;
        private Texture2D originalStoryButton;
        private Texture2D resumeButton2;
        private Texture2D startButton2;
        private Texture2D exitButton2;
        private Texture2D optionsButton2;
        private Texture2D storyButton2;
        

        Vector2 soundSliderPosition;
        Vector2 musicSliderPosition;
        Vector2 startButtonPosition;
        Vector2 exitButtonPositionPauseMenu;
        Vector2 resumeButtonPosition;
        Vector2 pauseButtonPosition;
        Vector2 optionsButtonPositionPauseMenu;
        Vector2 optionsButtonPositionStartMenu;
        Vector2 storyButtonPosition;
        Vector2 saveButtonPosition;
        Vector2 loadButtonPosition;
        Vector2 pauseMenuPosition;
        Vector2 loadingScreenPosition;
        Vector2 exitButtonPositionStartMenu;



        private Thread backGroundThread;
        public bool isLoading = false;
        private static bool runOnce = false;
        MouseState mouseState;
        MouseState previousMouseState;


        private Sounds soundManager;
        public GameState gameState;
        #endregion
        #region ContructorAndLoad
        public Menu(Game1 game)
        {

            pauseMenuBackgroundFront = game.Content.Load<Texture2D>("Menu items/PauseMenu");
            pauseMenuBackgroundBack = game.Content.Load<Texture2D>("Menu items/PauseMenuBackground");
            loadingGameButton = game.Content.Load<Texture2D>("Menu items/LoadingGameButton");
            loadingBackground = game.Content.Load<Texture2D>("Menu items/LoadingBackgroundAlt");
            soundSlider = game.Content.Load<Texture2D>("note");
            musicSlider = game.Content.Load<Texture2D>("note");
            soundManager = new Sounds(game);

            startButton = game.Content.Load<Texture2D>("Menu items/StartButtonHighlight");
            exitButton = game.Content.Load<Texture2D>("Menu items/ExitButtonHighlight");
            pauseButton = game.Content.Load<Texture2D>("Menu items/PauseButton");
            resumeButton = game.Content.Load<Texture2D>("Menu items/ResumeButtonHighlight");
            optionsButton = game.Content.Load<Texture2D>("Menu items/OptionsButtonHighlight");
            storyButton = game.Content.Load<Texture2D>("Menu items/StoryButtonHighlight");
            saveButton = game.Content.Load<Texture2D>("Menu items/SaveButton");
            LoadButon = game.Content.Load<Texture2D>("Menu items/LoadButton");

            originalResumeButton = game.Content.Load<Texture2D>("Menu items/ResumeButton");
            originalOptionsButton = game.Content.Load<Texture2D>("Menu items/OptionsButton");
            originalStartButton = game.Content.Load<Texture2D>("Menu items/StartButton");
            originalExitButton = game.Content.Load<Texture2D>("Menu items/ExitButton");
            originalStoryButton = game.Content.Load<Texture2D>("Menu items/StoryButton");

            resumeButton2 = game.Content.Load<Texture2D>("Menu items/ResumeButton");
            optionsButton2 = game.Content.Load<Texture2D>("Menu items/OptionsButton");
            startButton2 = game.Content.Load<Texture2D>("Menu items/StartButton");
            exitButton2 = game.Content.Load<Texture2D>("Menu items/ExitButton");
            storyButton2 = game.Content.Load<Texture2D>("Menu items/StoryButton");
        }
        /// <summary>
        /// Starting the game after that the thread has been inactive for 6 sec.
        /// </summary>
        public void LoadGame()
        {
            Thread.Sleep(6000);

            gameState = GameState.Playing;
            isLoading = true;

        }
        #endregion
        #region Unused
        /// <summary>
        /// Not currently in use. Option to have a pause icon which can be mouseclicked.
        /// </summary>
        /// <param name="game"></param>
        public void PauseIcon(Game1 game)
        {
            gameState = GameState.Playing;
            pauseButtonPosition = new Vector2(20, 20);
        }
        #endregion
        #region Buttonpositioning
        /// <summary>
        /// Positioning buttons and backgrounds for the pause menu
        /// </summary>
        /// <param name="game"></param>
        public void PauseMenu(Game1 game)
        {
            gameState = GameState.Paused;
            pauseMenuPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - pauseMenuBackgroundFront.Width / 2), 160);
            resumeButtonPosition = new Vector2((int)Camera.transform.Translation.X + game.GraphicsDevice.Viewport.Width / 2 - resumeButton.Width / 2, (int)Camera.transform.Translation.Y + game.GraphicsDevice.Viewport.Height / 2 - 130);
            saveButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 330);
            loadButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 360);
            optionsButtonPositionPauseMenu = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - optionsButton.Width / 2), 280);
            exitButtonPositionPauseMenu = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width / 2), (game.GraphicsDevice.Viewport.Height / 2 - 30));
            soundSliderPosition = new Vector2((game.GraphicsDevice.Viewport.Width - 60), 30);
            musicSliderPosition = new Vector2((game.GraphicsDevice.Viewport.Width - 60), 80);


        }

        /// <summary>
        /// Positioning buttons for the options menu
        /// </summary>
        /// <param name="game"></param>
        public void OptionsMenu(Game1 game)
        {
            gameState = GameState.Options;
            resumeButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - resumeButton.Width), 230);
        }

        /// <summary>
        /// Positioning buttons and background for the start menu
        /// </summary>
        /// <param name="game"></param>
        public void StartMenu(Game1 game)
        {
            gameState = GameState.StartMenu;
            startButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - startButton.Width), 300);
            optionsButtonPositionStartMenu = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - 140), 355);
            storyButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - storyButton.Width), 410);
            exitButtonPositionStartMenu = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width), 465);
            loadingScreenPosition = new Vector2(550, 335);
        }
        #endregion
        #region MouseActions
        /// <summary>
        /// MouseClicked is function used to create rectangles for the buttons that when intersecting with the mouse pointer and clicked will trigger a game state. Used for the menus.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="game"></param>
        void MouseClicked(int x, int y, Game1 game)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            //checking the startmenu
            if (gameState == GameState.StartMenu)
            {
                Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X + 20, (int)startButtonPosition.Y + 28, 120, 20);
                Rectangle optionsButtonRect = new Rectangle((int)optionsButtonPositionStartMenu.X + 20, (int)optionsButtonPositionStartMenu.Y, 120, 20);
                Rectangle storyButtonRect = new Rectangle((int)storyButtonPosition.X + 20, (int)storyButtonPosition.Y, 120, 20);
                Rectangle exitButtonRect1 = new Rectangle((int)exitButtonPositionStartMenu.X + 20, (int)exitButtonPositionStartMenu.Y, 120, 20);

                if (mouseClickRect.Intersects(startButtonRect))
                {
                    gameState = GameState.Loading;

                    isLoading = true;
                }
                else if (mouseClickRect.Intersects(exitButtonRect1))
                {
                    game.Exit();
                }
                else if (mouseClickRect.Intersects(optionsButtonRect))
                {

                }
            }

            //checking the pausemenu
            if (gameState == GameState.Paused)
            {
                Rectangle resumeButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width  /2 - resumeButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 105, 120, 20);
                Rectangle optionsButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width  /2 - optionsButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 60, 160, 20);
                Rectangle exitbuttonRect2 = new Rectangle((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width), (game.GraphicsDevice.Viewport.Height / 2 - 20), 120, 20);
                Rectangle musicRect = new Rectangle((int)musicSliderPosition.X, (int)musicSliderPosition.Y, 40, 40);
                Rectangle soundRect = new Rectangle((int)soundSliderPosition.X, (int)soundSliderPosition.Y, 40, 40);
                

                if (mouseClickRect.Intersects(resumeButtonRect))
                {
                    gameState = GameState.Playing;
                }
                else if (mouseClickRect.Intersects(exitbuttonRect2))
                {
                    game.Exit();
                }

                else if (mouseClickRect.Intersects(musicRect))
                {

                }

                else if (mouseClickRect.Intersects(soundRect))
                {
                }
                
            }

            //checking the options menu
            if (gameState == GameState.Options)
            {
                Rectangle resumeButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - resumeButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 105, 120, 20);

                //gameState = GameState.Paused;
                if (mouseClickRect.Intersects(resumeButtonRect))
                {
                    gameState = GameState.Playing;
                }
            }
        }

        /// <summary>
        /// Function that Highlights a button when the mouse pointer intesects with button.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="game"></param>
        public void MouseOver(int x, int y, Game1 game)
        {
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);
            Rectangle resumeButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - resumeButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 105, 120, 20);
            Rectangle optionsButtonRect1 = new Rectangle((int)optionsButtonPositionStartMenu.X + 20, (int)optionsButtonPositionStartMenu.Y + 20, 160, 20);
            Rectangle optionsButtonRect2 = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - optionsButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 65, 160, 20);
            Rectangle exitButtonRect1 = new Rectangle((int)exitButtonPositionStartMenu.X + 20, (int)exitButtonPositionStartMenu.Y - 5, 120, 20);
            Rectangle exitButtonRect2 = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 20, 120, 20);
            Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X + 20, (int)startButtonPosition.Y + 28, 120, 30);
            Rectangle storyButtonRect = new Rectangle((int)storyButtonPosition.X + 20, (int)storyButtonPosition.Y, 120, 30);


            if (mouseClickRect.Intersects(resumeButtonRect))
            {
                originalResumeButton = resumeButton;
            }
            else
            {
                originalResumeButton = resumeButton2;
            }
            if (mouseClickRect.Intersects(optionsButtonRect1) && gameState == GameState.StartMenu)
            {
                originalOptionsButton = optionsButton;
            }
            else if (mouseClickRect.Intersects(optionsButtonRect2) && gameState == GameState.Paused)
            {
                originalOptionsButton = optionsButton;
            }
            else
            {
                originalOptionsButton = optionsButton2;
            }
            if (mouseClickRect.Intersects(startButtonRect))
            {
                originalStartButton = startButton;
            }
            else
            {
                originalStartButton = startButton2;
            }
            if (mouseClickRect.Intersects(exitButtonRect1) && gameState == GameState.StartMenu)
            {
                originalExitButton = exitButton;
            }
            else if (mouseClickRect.Intersects(exitButtonRect2) && gameState == GameState.Paused)
            {
                originalExitButton = exitButton;
            }
            else
            {
                originalExitButton = exitButton2;
            }
            if (mouseClickRect.Intersects(storyButtonRect))
            {
                originalStoryButton = storyButton;
            }
            else
            {
                originalStoryButton = storyButton2;
            }

        }
        #endregion
        #region DrawAndUpdate

        /// <summary>
        /// UpdateMenu used to handle different states. Pausing the game and keeping track on if the mouse is clicked on buttons.
        /// </summary>
        /// <param name="gametime"></param>
        /// <param name="game"></param>
        public void UpdateMenu(GameTime gametime, Game1 game, Player player)
        {

            KeyboardState kbState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                MouseClicked(mouseState.X, mouseState.Y, game);
            }
            previousMouseState = mouseState;

            if (gameState == GameState.Loading)
            {
                isLoading = false;
            }

            if (gameState == GameState.Loading && !isLoading)
            {
                // backGroundThread = new Thread(game.Load);
                //this.backGroundThread.IsBackground = true;
                isLoading = false;


                // backGroundThread.Start();


                if (!runOnce)
                {
                    game.loadThread.Start();
                    runOnce = true;
                }
            }

            if (kbState.IsKeyDown(Keys.F2))
            {
                gameState = GameState.Paused;
            }

            else if (kbState.IsKeyDown(Keys.F3))
            {
                gameState = GameState.Playing;
            }

            if (gameState == GameState.Playing)
            {
                PauseIcon(game);

            }
            else if (gameState == GameState.Paused)
            {
                PauseMenu(game);

            }

            else if (gameState == GameState.StartMenu)
            {
                StartMenu(game);
            }
            else if (gameState == GameState.Options)
            {
                OptionsMenu(game);
            }

            MouseOver(mouseState.X, mouseState.Y, game);

        }

        /// <summary>
        /// Drawing the UI
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="player"></param>
        /// <param name="myFont"></param>
        public void DrawPlayerInfo(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Player player, SpriteFont myFont, GameTime gameTime)
        {
            if (!player.Dead)
            {
                totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            spriteBatch.DrawString(myFont, "Health:" + player.playerHP.ToString(), new Vector2(-(int)Camera.transform.Translation.X + 10, -(int)Camera.transform.Translation.Y + 600), Color.DarkRed);
            spriteBatch.DrawString(myFont, "Ethanol:" + player.playerAthyl.ToString(), new Vector2(-(int)Camera.transform.Translation.X + 10, -(int)Camera.transform.Translation.Y + 630), Color.MidnightBlue);
            spriteBatch.DrawString(myFont, "Exp:" + player.playerXP.ToString(), new Vector2(-(int)Camera.transform.Translation.X + 10, -(int)Camera.transform.Translation.Y + 660), Color.Green);
            spriteBatch.DrawString(myFont, "Level:" + player.playerLevel.ToString(), new Vector2(-(int)Camera.transform.Translation.X + 10, -(int)Camera.transform.Translation.Y + 690), Color.Wheat);

            spriteBatch.DrawString(myFont, "Time:" + totalTime.ToString("0"), new Vector2(-(int)Camera.transform.Translation.X + 10, -(int)Camera.transform.Translation.Y + 570), Color.Violet);
        }
        /// <summary>
        /// Drawing the graphics for the menus for the different game states.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="game"></param>
        public void Draw(SpriteBatch spriteBatch, Game1 game)
        {

            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle(-(int)Camera.transform.Translation.X + 425, -(int)Camera.transform.Translation.Y + 250, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(originalStartButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - startButton.Width / 2, -(int)Camera.transform.Translation.Y + 310, startButton.Width, startButton.Height), Color.White);
                spriteBatch.Draw(originalOptionsButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - optionsButton.Width / 2, -(int)Camera.transform.Translation.Y + 350, optionsButton.Width, optionsButton.Height), Color.White);
                spriteBatch.Draw(originalStoryButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - storyButton.Width / 2, -(int)Camera.transform.Translation.Y + 395, storyButton.Width, storyButton.Height), Color.White);
                spriteBatch.Draw(originalExitButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - exitButton.Width / 2, -(int)Camera.transform.Translation.Y + 435, exitButton.Width, exitButton.Height), Color.White);

            }

            if (gameState == GameState.Loading)
            {
                spriteBatch.Draw(loadingBackground, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(loadingGameButton, new Rectangle(-(int)Camera.transform.Translation.X + 550, -(int)Camera.transform.Translation.Y + 330, loadingGameButton.Width, loadingGameButton.Height), Color.CornflowerBlue);
            }

            if (gameState == GameState.Paused)
            {
                spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle(-(int)Camera.transform.Translation.X + 405, -(int)Camera.transform.Translation.Y + 150, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(originalResumeButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - resumeButton.Width / 2, -(int)Camera.transform.Translation.Y + 230, resumeButton.Width, resumeButton.Height), Color.White);
                spriteBatch.Draw(originalOptionsButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - optionsButton.Width / 2, -(int)Camera.transform.Translation.Y + 270, optionsButton.Width, optionsButton.Height), Color.White);
                spriteBatch.Draw(originalExitButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - exitButton.Width / 2, -(int)Camera.transform.Translation.Y + 315, exitButton.Width, exitButton.Height), Color.White);
                //spriteBatch.Draw(musicSlider, new Rectangle(-(int)Camera.transform.Translation.X + 1230, -(int)Camera.transform.Translation.Y + 10, musicSlider.Width, musicSlider.Height), Color.White);
                //spriteBatch.Draw(soundSlider, new Rectangle(-(int)Camera.transform.Translation.X + 1230, -(int)Camera.transform.Translation.Y + 60, soundSlider.Width, soundSlider.Height), Color.White);
            }

            if (gameState == GameState.Options)
            {
                spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle(-(int)Camera.transform.Translation.X + 405, -(int)Camera.transform.Translation.Y + 150, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(originalResumeButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - resumeButton.Width / 2, -(int)Camera.transform.Translation.Y + 230, resumeButton.Width, resumeButton.Height), Color.White);
            }
        }
        #endregion
    }
}