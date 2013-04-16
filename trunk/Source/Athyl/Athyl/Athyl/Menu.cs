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
        public enum GameState { StartMenu, Loading, Playing, Paused }

        private Texture2D startButton;
        private Texture2D exitButton;
        private Texture2D pauseButton;
        private Texture2D resumeButton;
        private Texture2D settingsButton;
        private Texture2D saveButton;
        private Texture2D LoadButon;
        private Texture2D loadingScreen;
        private Texture2D pauseMenuBackgroundFront;
        private Texture2D pauseMenuBackgroundBack;


        Vector2 startButtonPosition;
        Vector2 exitButtonPosition;
        Vector2 resumeButtonPosition;
        Vector2 pauseButtonPosition;
        Vector2 settingsButtonPosition;
        Vector2 saveButtonPosition;
        Vector2 loadButtonPosition;
        Vector2 pauseMenuPosition;
        Vector2 loadingScreenPosition;

        private Thread backGroundThread;
        private bool isLoading = false;
        MouseState mouseState;
        MouseState previousMouseState;

        public GameState gameState;

        public Menu(Game1 game)
        {
            startButton = game.Content.Load<Texture2D>("StartButton");
            exitButton = game.Content.Load<Texture2D>("ExitButton");
            pauseButton = game.Content.Load<Texture2D>("PauseButton");
            resumeButton = game.Content.Load<Texture2D>("ResumeButton");
            settingsButton = game.Content.Load<Texture2D>("SettingsButton");
            saveButton = game.Content.Load<Texture2D>("SaveButton");
            LoadButon = game.Content.Load<Texture2D>("LoadButton");
            pauseMenuBackgroundFront = game.Content.Load<Texture2D>("PauseMenu");
            pauseMenuBackgroundBack = game.Content.Load<Texture2D>("PauseMenuBackground");
            loadingScreen = game.Content.Load<Texture2D>("LoadingGameButton");
        }

        public void StartMenu(Game1 game)
        {
            gameState = GameState.StartMenu;
            startButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - startButton.Width), 300);
            exitButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width), 355);
            loadingScreenPosition = new Vector2(800, 500); 
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
            resumeButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - resumeButton.Width / 2), 230);
            saveButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 330);
            loadButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 360);
            settingsButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - settingsButton.Width / 2), 280);
            exitButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width / 2), 330);

        }

        public void LoadGame()
        {
            //Load the game images into the content pipeline
            Thread.Sleep(3000);

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

            if (gameState == GameState.Playing && isLoading)
            {
                LoadGame();
                isLoading = false;
            }
       

            if (gameState == GameState.Loading && !isLoading)
            {
                backGroundThread = new Thread(LoadGame);
                isLoading = true;

                backGroundThread.Start();
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


        }

        void MouseClicked(int x, int y, Game1 game)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            //checking the startmenu
            if (gameState == GameState.StartMenu)
            {
                Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, 120, 60);
                Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X, (int)startButtonPosition.Y, 120, 60);

                if (mouseClickRect.Intersects(startButtonRect))
                {
                    gameState = GameState.Loading;
                    isLoading = true;
                }
                else if (mouseClickRect.Intersects(exitButtonRect))
                {
                    game.Exit();
                }
            }

            //checking the pausemenu
            if (gameState == GameState.Paused)
            {
                Rectangle resumeButtonRect = new Rectangle((int)resumeButtonPosition.X, (int)resumeButtonPosition.Y, 120, 60);
                Rectangle settingsButtonRect = new Rectangle((int)settingsButtonPosition.X, (int)settingsButtonPosition.Y, 160, 60);
                Rectangle exitbuttonRect = new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, 120, 60);

                if (mouseClickRect.Intersects(resumeButtonRect))
                {
                    gameState = GameState.Playing;
                    
                }
                else if (mouseClickRect.Intersects(exitbuttonRect))
                {
                    game.Exit();
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(startButton, new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, startButton.Width, startButton.Height), Color.White);
                spriteBatch.Draw(exitButton, new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, exitButton.Width, exitButton.Height), Color.White);

            }

            if (gameState == GameState.Loading)
            {
                spriteBatch.Draw(loadingScreen, new Rectangle((int)loadingScreenPosition.X, (int)loadingScreenPosition.Y, loadingScreen.Width, loadingScreen.Height), Color.CornflowerBlue);
            }

            if (gameState == GameState.Paused)
            {
                spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(0, 0, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle((int)pauseMenuPosition.X, (int)pauseMenuPosition.Y, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(resumeButton, new Rectangle((int)resumeButtonPosition.X, (int)resumeButtonPosition.Y, resumeButton.Width, resumeButton.Height), Color.White);
                spriteBatch.Draw(settingsButton, new Rectangle((int)settingsButtonPosition.X, (int)settingsButtonPosition.Y, settingsButton.Width, settingsButton.Height), Color.White);
                spriteBatch.Draw(exitButton, new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, exitButton.Width, exitButton.Height), Color.White);
            }
            //spriteBatch.Draw(settingsButton, new Rectangle((int)settingsButtonPosition.X, (int)settingsButtonPosition.Y, settingsButton.Width, settingsButton.Height), Color.White);


        }



    }
}
