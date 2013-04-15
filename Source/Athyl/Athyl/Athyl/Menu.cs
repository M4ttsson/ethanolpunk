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
        private Texture2D pauseMenuBackground;


        Vector2 startButtonPosition;
        Vector2 exitButtonPosition;
        Vector2 resumeButtonPosition;
        Vector2 pauseButtonPosition;
        Vector2 settingsButtonPosition;
        Vector2 saveButtonPosition;
        Vector2 loadButtonPosition;

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
            pauseMenuBackground = game.Content.Load<Texture2D>("PauseMenuBackground");
        }

        public void StartMenu(Game1 game)
        {
            gameState = GameState.StartMenu;
            startButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 300);
            exitButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 355);
        }

        public void PauseIcon(Game1 game)
        {
            gameState = GameState.Playing;
            pauseButtonPosition = new Vector2(20, 20);
        }
        public void PauseMenu(Game1 game)
        {
            gameState = GameState.Paused;
            resumeButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - resumeButton.Width / 2), 230);
            saveButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 330);
            loadButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 360);
            settingsButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - settingsButton.Width / 2), 280);
            exitButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width / 2), 330);

        }

        public void LoadGame()
        {
            //Load the game images into the content pipeline


        }
        public void UpdateMenu(GameTime gametime, Game1 game)
        {

            KeyboardState kbState = Keyboard.GetState();


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

        public void Draw(SpriteBatch spriteBatch)
        {

           

            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(startButton, new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, startButton.Width, startButton.Height), Color.White);
            }
            //spriteBatch.Draw(exitButton, new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, exitButton.Width, exitButton.Height), Color.White);

            //spriteBatch.Draw(pauseButton, new Rectangle((int)pauseButtonPosition.X, (int)pauseButtonPosition.Y, pauseButton.Width, pauseButton.Height), Color.White);

            if (gameState == GameState.Paused)
            {
                spriteBatch.Draw(pauseMenuBackground, new Rectangle(0, 0, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(resumeButton, new Rectangle((int)resumeButtonPosition.X, (int)resumeButtonPosition.Y, resumeButton.Width, resumeButton.Height), Color.White);
                spriteBatch.Draw(settingsButton, new Rectangle((int)settingsButtonPosition.X, (int)settingsButtonPosition.Y, settingsButton.Width, settingsButton.Height), Color.White);
                spriteBatch.Draw(exitButton, new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, exitButton.Width, exitButton.Height), Color.White);
            }
            //spriteBatch.Draw(settingsButton, new Rectangle((int)settingsButtonPosition.X, (int)settingsButtonPosition.Y, settingsButton.Width, settingsButton.Height), Color.White);


        }



    }
}
