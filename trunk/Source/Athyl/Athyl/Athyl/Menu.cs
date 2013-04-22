
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
        public enum GameState { StartMenu, Loading, Playing, Paused, Options, Story }

        private Texture2D startButton;
        private Texture2D exitButton;
        private Texture2D pauseButton;
        private Texture2D resumeButton;
        private Texture2D settingsButton;
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


        Vector2 soundSliderPosition;
        Vector2 musicSliderPosition;
        Vector2 startButtonPosition;
        Vector2 exitButtonPosition;
        Vector2 resumeButtonPosition;
        Vector2 pauseButtonPosition;
        Vector2 settingsButtonPosition;
        Vector2 optionsButtonPosition;
        Vector2 storyButtonPosition;
        Vector2 saveButtonPosition;
        Vector2 loadButtonPosition;
        Vector2 pauseMenuPosition;
        Vector2 loadingScreenPosition;



        private Thread backGroundThread;
        public bool isLoading = false;
        private static bool runOnce = false;
        MouseState mouseState;
        MouseState previousMouseState;


        private Sounds soundManager;
        public GameState gameState;

        public Menu(Game1 game)
        {
            startButton = game.Content.Load<Texture2D>("StartButton");
            exitButton = game.Content.Load<Texture2D>("ExitButton");
            pauseButton = game.Content.Load<Texture2D>("PauseButton");
            resumeButton = game.Content.Load<Texture2D>("ResumeButton");
            settingsButton = game.Content.Load<Texture2D>("OptionsButton");
            optionsButton = game.Content.Load<Texture2D>("OptionsButton");
            storyButton = game.Content.Load<Texture2D>("StoryButton");
            saveButton = game.Content.Load<Texture2D>("SaveButton");
            LoadButon = game.Content.Load<Texture2D>("LoadButton");
            pauseMenuBackgroundFront = game.Content.Load<Texture2D>("PauseMenu");
            pauseMenuBackgroundBack = game.Content.Load<Texture2D>("PauseMenuBackground");
            loadingGameButton = game.Content.Load<Texture2D>("LoadingGameButton");
            loadingBackground = game.Content.Load<Texture2D>("LoadingBackgroundAlt");
            soundSlider = game.Content.Load<Texture2D>("note");
            musicSlider = game.Content.Load<Texture2D>("note");
            soundManager = new Sounds(game);
        }

        public void StartMenu(Game1 game)
        {
            gameState = GameState.StartMenu;
            startButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - startButton.Width), 300);
            optionsButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - 140), 355);
            storyButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - storyButton.Width), 410);
            exitButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width), 465);
            loadingScreenPosition = new Vector2(550, 335);
        }

        public void PauseIcon(Game1 game)
        {
            gameState = GameState.Playing;
            pauseButtonPosition = new Vector2(20, 20);
        }
        
        public void PauseMenu(Game1 game)
        {
            gameState = GameState.Paused;
            pauseMenuPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - pauseMenuBackgroundFront.Width / 2), 160);
            //Rectangle resumeButtonRect = new Rectangle((game.GraphicsDevice.Viewport.Width / 2 - resumeButton.Width / 2), (game.GraphicsDevice.Viewport.Height / 2 - 130), 120, 20);
            resumeButtonPosition = new Vector2((int)Camera.transform.Translation.X + game.GraphicsDevice.Viewport.Width / 2 - resumeButton.Width / 2, (int)Camera.transform.Translation.Y + game.GraphicsDevice.Viewport.Height / 2 - 130);
            saveButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 330);
            loadButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 360);
            settingsButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - settingsButton.Width / 2), 280);
            exitButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width / 2), (game.GraphicsDevice.Viewport.Height / 2 - 30));
            soundSliderPosition = new Vector2((game.GraphicsDevice.Viewport.Width - 60), 30);
            musicSliderPosition = new Vector2((game.GraphicsDevice.Viewport.Width - 60), 80);


        }
        public void OptionsMenu(Game1 game)
        {
            gameState = GameState.Options;
            resumeButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - resumeButton.Width), 230);
        }

        public void LoadGame()
        {
            Thread.Sleep(6000);

            gameState = GameState.Playing;
            isLoading = true;

        }

        public void UpdateMenu(GameTime gametime, Game1 game)
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


        }
        private void DrawPlayerInfo(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Player player, SpriteFont myFont)
        {
            spriteBatch.DrawString(myFont, "Health:" + player.playerHP.ToString(), new Vector2(20, graphicsDevice.Viewport.Height - 100), Color.DarkRed);
            spriteBatch.DrawString(myFont, "Ethanol:" + player.playerAthyl.ToString(), new Vector2(20, graphicsDevice.Viewport.Height - 70), Color.MidnightBlue);
            spriteBatch.DrawString(myFont, "Exp:" + player.playerXP.ToString(), new Vector2(20, graphicsDevice.Viewport.Height - 130), Color.Green);
            spriteBatch.DrawString(myFont, "Level:" + player.playerLevel.ToString(), new Vector2(20, graphicsDevice.Viewport.Height - 160), Color.Wheat);
        }
        void MouseClicked(int x, int y, Game1 game)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            //checking the startmenu
            if (gameState == GameState.StartMenu)
            {
                Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y + 28, 120, 20);
                Rectangle optionsButtonRect = new Rectangle((int)optionsButtonPosition.X, (int)optionsButtonPosition.Y, 120, 20);
                Rectangle storyButtonRect = new Rectangle((int)storyButtonPosition.X, (int)storyButtonPosition.Y, 120, 20);
                Rectangle exitButtonRect1 = new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y + 23, 120, 20);

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
                Rectangle settingsButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width  /2 - optionsButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 60, 160, 20);
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
                Rectangle resumeButtonRect = new Rectangle((int)resumeButtonPosition.X, (int)resumeButtonPosition.Y, 120, 60);
                //gameState = GameState.Paused;
                if (mouseClickRect.Intersects(resumeButtonRect))
                {
                    gameState = GameState.Playing;
                }
            }
        }

        
        public void Draw(SpriteBatch spriteBatch, Game1 game)
        {

            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle(-(int)Camera.transform.Translation.X + 405, -(int)Camera.transform.Translation.Y + 250, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(startButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - startButton.Width / 2, -(int)Camera.transform.Translation.Y + 310, startButton.Width, startButton.Height), Color.White);
                spriteBatch.Draw(optionsButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - optionsButton.Width / 2, -(int)Camera.transform.Translation.Y + 350, optionsButton.Width, optionsButton.Height), Color.White);
                spriteBatch.Draw(storyButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - storyButton.Width / 2, -(int)Camera.transform.Translation.Y + 395, storyButton.Width, storyButton.Height), Color.White);
                spriteBatch.Draw(exitButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - exitButton.Width / 2, -(int)Camera.transform.Translation.Y + 435, exitButton.Width, exitButton.Height), Color.White);

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
                spriteBatch.Draw(resumeButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - resumeButton.Width / 2, -(int)Camera.transform.Translation.Y + 230, resumeButton.Width, resumeButton.Height), Color.White);
                spriteBatch.Draw(optionsButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - optionsButton.Width / 2, -(int)Camera.transform.Translation.Y + 270, settingsButton.Width, settingsButton.Height), Color.White);
                spriteBatch.Draw(exitButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - exitButton.Width / 2, -(int)Camera.transform.Translation.Y + 315, exitButton.Width, exitButton.Height), Color.White);
                spriteBatch.Draw(musicSlider, new Rectangle(-(int)Camera.transform.Translation.X + 1230, -(int)Camera.transform.Translation.Y + 10, musicSlider.Width, musicSlider.Height), Color.White);
                spriteBatch.Draw(soundSlider, new Rectangle(-(int)Camera.transform.Translation.X + 1230, -(int)Camera.transform.Translation.Y + 60, soundSlider.Width, soundSlider.Height), Color.White);
            }

            if (gameState == GameState.Options)
            {
                spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(0, 0, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(resumeButton, new Rectangle((int)resumeButtonPosition.X, (int)resumeButtonPosition.Y, resumeButton.Width, resumeButton.Height), Color.White);
            }


        }



    }
}