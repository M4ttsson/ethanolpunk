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
        public enum GameState { StartMenu, Loading, Playing ,Paused }

        private Texture2D startButton;
        private Texture2D exitButton;
        private Texture2D pauseButton;
        private Texture2D resumeButton;
        private Texture2D settingsButton;
        private Texture2D saveButton;
        private Texture2D LoadButon;
        private Texture2D loadingScreen;


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

        GameState gameState;

        public Menu(Game1 game)
        {
            startButton = game.Content.Load<Texture2D>("StartButton");
            exitButton = game.Content.Load<Texture2D>("ExitButon");
            pauseButton = game.Content.Load<Texture2D>("PauseButton");
            resumeButton = game.Content.Load<Texture2D>("ResumeButton");
            settingsButton = game.Content.Load<Texture2D>("SeetingsButton");
            saveButton = game.Content.Load<Texture2D>("SaveButton");
            LoadButon = game.Content.Load<Texture2D>("LoadButton");
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
            resumeButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 300);
            saveButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 330);
            loadButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 360);
            settingsButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2), 390);
        }

        public void LoadGame()
        {
            //Load the game images into the content pipeline

            
        }
        public void Update(GameTime gametime)
        {
        }

        public void Draw(SpriteBatch draw)
        {
        }

  
            
    }
}
