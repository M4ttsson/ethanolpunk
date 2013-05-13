﻿
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
        public enum GameState { StartMenu, Loading, Playing, Paused, ControlsStartMenu, ControlsPauseMenu, Story, GameOver, LevelUp }

        private Texture2D startButton;
        private Texture2D exitButton;
        private Texture2D pauseButton;
        private Texture2D resumeButton;
        private Texture2D controlsButton;
        private Texture2D storyButton;
        private Texture2D returnButton;
        private Texture2D restartButton;
        private Texture2D mainMenuButton;
        private Texture2D gameOverMainMenuButton;
        private Texture2D skillTreeButton;
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
        private Texture2D progressBar;
        private Texture2D progressBarBorder;
        private Texture2D runningWoman;
        private Texture2D deadWoman;
        private Texture2D gameOver;
        private int colFrame;
        private float TimePerFrame;
        private float TotalElapsed;

        public float totalTime = 0;
        private Texture2D originalResumeButton;
        private Texture2D originalControlsButton;
        private Texture2D originalStartButton;
        private Texture2D originalExitButton;
        private Texture2D originalPauseButton;
        private Texture2D originalStoryButton;
        private Texture2D originalReturnButton;
        private Texture2D originalRestartButton;
        private Texture2D originalMainMenuButton;
        private Texture2D originalSkillTreeButton;
        private Texture2D originalGameOverMainMenuButton;
        private Texture2D skillTreeButton2;
        private Texture2D resumeButton2;
        private Texture2D startButton2;
        private Texture2D exitButton2;
        private Texture2D controlsButton2;
        private Texture2D storyButton2;
        private Texture2D returnButton2;
        private Texture2D mainMenuButton2;
        private Texture2D restartButton2;
        private Texture2D gameOverMainMenuButton2;
        private Texture2D keyboardLayout;
        private Texture2D Combat, Mid, Long, ColorCombat, ColorMid, ColorLong;      //Button texture
        private Texture2D BarBorder, BarBorerBkg, AthylTexture, HealthTexture;      //Hp/Athyl mätare
        private Texture2D ExpBorder, ExpBar, ExpBarBkg;                             //Exp mätare
        private Texture2D SkilltreeCombat, SkilltreeMid, SkilltreeLong;
        

        Vector2 soundSliderPosition;
        Vector2 musicSliderPosition;
        Vector2 startButtonPosition;
        Vector2 exitButtonPositionPauseMenu;
        Vector2 resumeButtonPosition;
        Vector2 pauseButtonPosition;
        Vector2 controlsButtonPositionPauseMenu;
        Vector2 controlsButtonPositionStartMenu;
        Vector2 restartButtonPosition;
        Vector2 mainMenuButtonPosition;
        Vector2 storyButtonPosition;
        Vector2 saveButtonPosition;
        Vector2 loadButtonPosition;
        Vector2 pauseMenuPosition;
        Vector2 loadingScreenPosition;
        Vector2 exitButtonPositionStartMenu;
        Vector2 returnButtonPosition;
        Vector2 skillTreeButtonPosition;
        Vector2 gameOverMainMenuButtonPosition;


        private Thread backGroundThread;
        public bool isLoading = false;
        private static bool runOnce = false;
        private SpriteFont myFont;
        MouseState mouseState;
        MouseState previousMouseState;

        KeyboardState prevKeyboardState;


        private Sounds soundManager;
        public GameState gameState;

        public Menu(Game1 game)
        {

            pauseMenuBackgroundFront = game.Content.Load<Texture2D>("Menu items/PauseMenu");
            pauseMenuBackgroundBack = game.Content.Load<Texture2D>("Menu items/PauseMenuBackground");
            loadingGameButton = game.Content.Load<Texture2D>("Menu items/LoadingGameButton");
            loadingBackground = game.Content.Load<Texture2D>("Menu items/LoadingScreen");
            soundSlider = game.Content.Load<Texture2D>("note");
            musicSlider = game.Content.Load<Texture2D>("note");
            soundManager = new Sounds(game);

            startButton = game.Content.Load<Texture2D>("Menu items/StartButtonHighlight");
            exitButton = game.Content.Load<Texture2D>("Menu items/ExitButtonHighlight");
            pauseButton = game.Content.Load<Texture2D>("Menu items/PauseButton");
            resumeButton = game.Content.Load<Texture2D>("Menu items/ResumeButtonHighlight");
            controlsButton = game.Content.Load<Texture2D>("Menu items/ControlsButtonHighlight");
            storyButton = game.Content.Load<Texture2D>("Menu items/StoryButtonHighlight");
            returnButton = game.Content.Load<Texture2D>("Menu items/ReturnButtonHighlight");
            saveButton = game.Content.Load<Texture2D>("Menu items/SaveButton");
            LoadButon = game.Content.Load<Texture2D>("Menu items/LoadButton");
            skillTreeButton = game.Content.Load<Texture2D>("Menu items/SkillTreeButtonHighlight");
            mainMenuButton = game.Content.Load<Texture2D>("Menu items/MainMenuButtonHighlight");
            gameOverMainMenuButton = game.Content.Load<Texture2D>("Menu items/MainMenuButtonHighlight");
            restartButton = game.Content.Load<Texture2D>("Menu items/RestartButtonHighlight");
            gameOver = game.Content.Load<Texture2D>("Menu items/GameOver");
            progressBar = game.Content.Load<Texture2D>("ProgressBar");
            progressBarBorder = game.Content.Load<Texture2D>("ProgressBarBorder");
            runningWoman = game.Content.Load<Texture2D>("Player/Gilliam");
            deadWoman = game.Content.Load<Texture2D>("Player/die");
            colFrame = 1;

            originalResumeButton = game.Content.Load<Texture2D>("Menu items/ResumeButton");
            originalControlsButton = game.Content.Load<Texture2D>("Menu items/ControlsButton");
            originalStartButton = game.Content.Load<Texture2D>("Menu items/StartButton");
            originalExitButton = game.Content.Load<Texture2D>("Menu items/ExitButton");
            originalStoryButton = game.Content.Load<Texture2D>("Menu items/StoryButton");
            originalReturnButton = game.Content.Load<Texture2D>("Menu items/ReturnButton");
            originalSkillTreeButton = game.Content.Load<Texture2D>("Menu items/SkillTreeButton");
            originalMainMenuButton = game.Content.Load<Texture2D>("Menu items/MainMenuButton");
            originalRestartButton = game.Content.Load<Texture2D>("Menu items/RestartButton");
            originalGameOverMainMenuButton = game.Content.Load<Texture2D>("Menu items/MainMenuButton");
            

            resumeButton2 = game.Content.Load<Texture2D>("Menu items/ResumeButton");
            controlsButton2 = game.Content.Load<Texture2D>("Menu items/ControlsButton");
            startButton2 = game.Content.Load<Texture2D>("Menu items/StartButton");
            exitButton2 = game.Content.Load<Texture2D>("Menu items/ExitButton");
            storyButton2 = game.Content.Load<Texture2D>("Menu items/StoryButton");
            returnButton2 = game.Content.Load<Texture2D>("Menu items/ReturnButton");
            skillTreeButton2 = game.Content.Load<Texture2D>("Menu items/SkillTreeButton");
            mainMenuButton2 = game.Content.Load<Texture2D>("Menu items/MainMenuButton");
            gameOverMainMenuButton2 = game.Content.Load<Texture2D>("Menu items/MainMenuButton");
            restartButton2 = game.Content.Load<Texture2D>("Menu items/RestartButton");
            keyboardLayout = game.Content.Load<Texture2D>("Menu items/ControlMenu");
            myFont = game.Content.Load<SpriteFont>("font");

            ColorCombat = game.Content.Load<Texture2D>("GUI/ColorCombat");
            ColorMid = game.Content.Load<Texture2D>("GUI/ColorMidrange");
            ColorLong = game.Content.Load<Texture2D>("GUI/ColorLong");
            Combat = game.Content.Load<Texture2D>("GUI/NoColorCombat");
            Mid = game.Content.Load<Texture2D>("GUI/NoColorMidrange");
            Long = game.Content.Load<Texture2D>("GUI/NoColorLong");

            BarBorder = game.Content.Load<Texture2D>("GUI/HpAthylBar");
            BarBorerBkg = game.Content.Load<Texture2D>("GUI/HpAthylBarBkg");
            HealthTexture = game.Content.Load<Texture2D>("GUI/HpBar");
            AthylTexture = game.Content.Load<Texture2D>("GUI/AthylBar");

            ExpBar = game.Content.Load<Texture2D>("GUI/ExpBar");
            ExpBarBkg = game.Content.Load<Texture2D>("GUI/ExpBarBKG");
            ExpBorder = game.Content.Load<Texture2D>("GUI/ExpBorder");

            SkilltreeCombat = game.Content.Load<Texture2D>("Menu items/TalentTreeClose");
            SkilltreeMid = game.Content.Load<Texture2D>("Menu items/TalentTreeMiddle");
            SkilltreeLong = game.Content.Load<Texture2D>("Menu items/TalentTreeLong");

            this.TimePerFrame = (float)1 / 1;
            this.TotalElapsed = 0;
        }

        /// <summary>
        /// Positioning buttons and background for the start menu
        /// </summary>
        /// <param name="game"></param>
        public void StartMenu(Game1 game)
        {
            gameState = GameState.StartMenu;
            startButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - startButton.Width), 310);
            controlsButtonPositionStartMenu = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - 140), 375);
            storyButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - storyButton.Width), 410);
            exitButtonPositionStartMenu = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width), 465);
            loadingScreenPosition = new Vector2(550, 335);
        }

        /// <summary>
        /// Not currently in use. Option to have a pause icon which can be mouseclicked.
        /// </summary>
        /// <param name="game"></param>
        public void PauseIcon(Game1 game)
        {
            gameState = GameState.Playing;
            pauseButtonPosition = new Vector2(20, 20);
        }
        
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
            controlsButtonPositionPauseMenu = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - controlsButton.Width / 2), 280);
            mainMenuButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - mainMenuButton.Width / 2), (game.GraphicsDevice.Viewport.Height / 2 - 30));
            exitButtonPositionPauseMenu = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width / 2), (game.GraphicsDevice.Viewport.Height / 2 + 10));
            soundSliderPosition = new Vector2((game.GraphicsDevice.Viewport.Width - 60), 30);
            musicSliderPosition = new Vector2((game.GraphicsDevice.Viewport.Width - 60), 80);


        }

        /// <summary>
        /// Positioning buttons for the options menu
        /// </summary>
        /// <param name="game"></param>
        public void OptionsStartMenu(Game1 game)
        {
            gameState = GameState.ControlsStartMenu;
            returnButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - startButton.Width), 300);
            exitButtonPositionStartMenu = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width), 465);
        }

        public void StoryMenu(Game1 game)
        {
            gameState = GameState.Story;
            returnButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - startButton.Width), 300);
        }

        public void OptionsPauseMenu(Game1 game)
        {
            gameState = GameState.ControlsPauseMenu;
        }

        public void GameOverMenu(Game1 game)
        {
            gameState = GameState.GameOver;
            restartButtonPosition = new Vector2((int)Camera.transform.Translation.X + game.GraphicsDevice.Viewport.Width / 2 - restartButton.Width / 2, (int)Camera.transform.Translation.Y + game.GraphicsDevice.Viewport.Height / 2);
            gameOverMainMenuButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - mainMenuButton.Width / 2), (game.GraphicsDevice.Viewport.Height / 2 + 40));
        }

        public void LevelUpMenu(Game1 game)
        {
            gameState = GameState.LevelUp;
            skillTreeButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2 - skillTreeButton.Width / 2), 280);
        }

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
                if (game.MapLoaded)
                {
                    isLoading = true;
                    gameState = GameState.Playing;
                    game.Restart();

                }
                else
                {

                    isLoading = false;

                    if (!runOnce)
                    {
                        game.loadThread.Start();
                        runOnce = true;
                    }
                }
            }

            if (kbState.IsKeyDown(Keys.Escape) && gameState == GameState.Playing)
            {
                gameState = GameState.Paused;
            }

            else if (kbState.IsKeyDown(Keys.F3))
            {
                gameState = GameState.Playing;
            }


            if(kbState.IsKeyDown(Keys.N) && !prevKeyboardState.IsKeyDown(Keys.N))// && player.NextLevel)
            {
                if (gameState == GameState.Playing)
                {
                    LevelUpMenu(game);
                }
                else
                {
                    gameState = GameState.Playing;
                }
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
            else if (gameState == GameState.ControlsStartMenu)
            {
                OptionsStartMenu(game);
            }
            else if (gameState == GameState.ControlsPauseMenu)
            {
                OptionsPauseMenu(game);
            }
            else if (gameState == GameState.Story)
            {
                StoryMenu(game);
            }
            else if (gameState == GameState.GameOver)
            {
                GameOverMenu(game);
            }
            else if (gameState == GameState.LevelUp)
            {
                LevelUpMenu(game);

                if (player.NextLevel && kbState.IsKeyDown(Keys.D1))
                {
                    player.LevelUp(Player.Stances.CloseRange);
                    gameState = GameState.Playing;
                }
                else if (player.NextLevel && kbState.IsKeyDown(Keys.D2))
                {
                    player.LevelUp(Player.Stances.MidRange);
                    gameState = GameState.Playing;
                }
                else if (player.NextLevel && kbState.IsKeyDown(Keys.D3))
                {
                    player.LevelUp(Player.Stances.LongRange);
                    gameState = GameState.Playing;
                }
            }

            MouseOver(mouseState.X, mouseState.Y, game);

            prevKeyboardState = kbState;
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

            //spriteBatch.Draw(GUI, new Vector2(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y), Color.White);

            UpdateStanceButtons(spriteBatch, player);
            UpdateGuiBars(spriteBatch, player);

            //spriteBatch.DrawString(myFont, "Health: " + (int)player.playerHP, new Vector2(-(int)Camera.transform.Translation.X + 10, -(int)Camera.transform.Translation.Y + 590), Color.DarkRed);
            //spriteBatch.DrawString(myFont, "Ethanol: " + (int)player.playerAthyl, new Vector2(-(int)Camera.transform.Translation.X + 10, -(int)Camera.transform.Translation.Y + 620), Color.LightBlue);
            //spriteBatch.DrawString(myFont,player.playerXP.ToString() + " / " + (int)player.xpRequiredPerLevel, new Vector2(-(int)Camera.transform.Translation.X + 1050, -(int)Camera.transform.Translation.Y + 20), Color.Red);
            spriteBatch.DrawString(myFont, "Exp:", new Vector2(-(int)Camera.transform.Translation.X + 970, -(int)Camera.transform.Translation.Y + 20), Color.Gold);
            spriteBatch.DrawString(myFont, "Level: " + player.playerLevel, new Vector2(-(int)Camera.transform.Translation.X + 830, -(int)Camera.transform.Translation.Y + 20), Color.Gold);
        }

        public void UpdateGuiBars(SpriteBatch spriteBatch, Player player)
        {
            spriteBatch.Draw(BarBorerBkg, new Vector2(-(int)Camera.transform.Translation.X + 32, -(int)Camera.transform.Translation.Y + 15), Color.White);
            spriteBatch.Draw(BarBorerBkg, new Vector2(-(int)Camera.transform.Translation.X + 32, -(int)Camera.transform.Translation.Y + 32), Color.White);
            spriteBatch.Draw(ExpBarBkg, new Vector2(-(int)Camera.transform.Translation.X + 1024, -(int)Camera.transform.Translation.Y + 15), Color.White);

            Rectangle barHp = new Rectangle(-(int)Camera.transform.Translation.X + 34, -(int)Camera.transform.Translation.Y + 17, (int)(player.playerHpPc * 218), HealthTexture.Height);
            Rectangle barAthyl = new Rectangle(-(int)Camera.transform.Translation.X + 34, -(int)Camera.transform.Translation.Y + 34, (int)(player.playerAthylPc * 218), AthylTexture.Height);
            Rectangle barExp = new Rectangle(-(int)Camera.transform.Translation.X + 1026, -(int)Camera.transform.Translation.Y + 17, (int)((player.playerXP * 220) / player.xpRequiredPerLevel), ExpBar.Height);
            spriteBatch.Draw(HealthTexture, barHp, Color.White);
            spriteBatch.Draw(AthylTexture, barAthyl, Color.White);
            spriteBatch.Draw(ExpBar, barExp, Color.White);
            spriteBatch.Draw(BarBorder, new Vector2(-(int)Camera.transform.Translation.X + 32, -(int)Camera.transform.Translation.Y + 15), Color.White);
            spriteBatch.Draw(BarBorder, new Vector2(-(int)Camera.transform.Translation.X + 32, -(int)Camera.transform.Translation.Y + 32), Color.White);
            spriteBatch.Draw(ExpBorder, new Vector2(-(int)Camera.transform.Translation.X + 1024, -(int)Camera.transform.Translation.Y + 15), Color.White);
        }

        public void UpdateStanceButtons(SpriteBatch spriteBatch, Player player)
        {
            if (player.Stance == Player.Stances.CloseRange)
            {
                spriteBatch.Draw(ColorCombat, new Vector2(-(int)Camera.transform.Translation.X + 575, -(int)Camera.transform.Translation.Y + 20), Color.White);
                spriteBatch.Draw(Mid, new Vector2(-(int)Camera.transform.Translation.X + 623, -(int)Camera.transform.Translation.Y + 20), Color.White);
                spriteBatch.Draw(Long, new Vector2(-(int)Camera.transform.Translation.X + 671, -(int)Camera.transform.Translation.Y + 20), Color.White);
            }
            else if (player.Stance == Player.Stances.MidRange)
            {
                spriteBatch.Draw(Combat, new Vector2(-(int)Camera.transform.Translation.X + 575, -(int)Camera.transform.Translation.Y + 20), Color.White);
                spriteBatch.Draw(ColorMid, new Vector2(-(int)Camera.transform.Translation.X + 623, -(int)Camera.transform.Translation.Y + 20), Color.White);
                spriteBatch.Draw(Long, new Vector2(-(int)Camera.transform.Translation.X + 671, -(int)Camera.transform.Translation.Y + 20), Color.White);
            }
            else
            {
                spriteBatch.Draw(Combat, new Vector2(-(int)Camera.transform.Translation.X + 575, -(int)Camera.transform.Translation.Y + 20), Color.White);
                spriteBatch.Draw(Mid, new Vector2(-(int)Camera.transform.Translation.X + 623, -(int)Camera.transform.Translation.Y + 20), Color.White);
                spriteBatch.Draw(ColorLong, new Vector2(-(int)Camera.transform.Translation.X + 671, -(int)Camera.transform.Translation.Y + 20), Color.White);
            }
        }
        

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
                Rectangle optionsButtonRect = new Rectangle((int)controlsButtonPositionStartMenu.X + 20, (int)controlsButtonPositionStartMenu.Y, 120, 20);
                Rectangle storyButtonRect = new Rectangle((int)storyButtonPosition.X + 20, (int)storyButtonPosition.Y + 10, 120, 20);
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
                    gameState = GameState.ControlsStartMenu;
                }
                else if (mouseClickRect.Intersects(storyButtonRect))
                {
                    gameState = GameState.Story;
                }
            }

            //checking the pausemenu
            if (gameState == GameState.Paused)
            {
                Rectangle resumeButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width  /2 - resumeButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 105, 120, 20);
                Rectangle optionsButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - controlsButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 60, 160, 20);
                Rectangle mainMenuButtonRect = new Rectangle((game.GraphicsDevice.Viewport.Width / 2 - mainMenuButton.Width), (game.GraphicsDevice.Viewport.Height / 2 - 20), 190, 20);
                Rectangle exitbuttonRect2 = new Rectangle((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width), (game.GraphicsDevice.Viewport.Height / 2 + 20), 120, 20);
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
                else if (mouseClickRect.Intersects(optionsButtonRect))
                {
                    gameState = GameState.ControlsPauseMenu;
                }
                else if (mouseClickRect.Intersects(mainMenuButtonRect))
                {
                    gameState = GameState.StartMenu;
                }
                else if (mouseClickRect.Intersects(musicRect))
                {

                }

                else if (mouseClickRect.Intersects(soundRect))
                {
                }
                
            }

            //checking the options menu during start menu
            if (gameState == GameState.ControlsStartMenu)
            {
                Rectangle returnButtonRect = new Rectangle((int)returnButtonPosition.X + 20, (int)returnButtonPosition.Y -180, 120, 20);
                Rectangle exitbuttonRect2 = new Rectangle((int)exitButtonPositionStartMenu.X + 20, (int)exitButtonPositionStartMenu.Y, 120, 20);

                if (mouseClickRect.Intersects(returnButtonRect))
                {
                    gameState = GameState.StartMenu;
                }

                else if (mouseClickRect.Intersects(exitbuttonRect2))
                {
                    game.Exit();
                }
            }

            //checking the options menu during pause game
            if (gameState == GameState.ControlsPauseMenu)
            {
                Rectangle returnButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - returnButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 240, 120, 20);
                Rectangle exitbuttonRect2 = new Rectangle((game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width), (game.GraphicsDevice.Viewport.Height / 2 - 20), 120, 20);

                if (mouseClickRect.Intersects(returnButtonRect))
                {
                    gameState = GameState.Paused;
                }

                else if (mouseClickRect.Intersects(exitbuttonRect2))
                {
                    game.Exit();
                }
            }

            //checking the story menu
            if (gameState == GameState.Story)
            {
                Rectangle returnButtonRect = new Rectangle((int)returnButtonPosition.X + 20, (int)returnButtonPosition.Y -180, 120, 20);
                if (mouseClickRect.Intersects(returnButtonRect))
                {
                    gameState = GameState.StartMenu;
                }
            }

            //checking the level up menu
            if (gameState == GameState.LevelUp)
            {
                Rectangle skillTreeButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - skillTreeButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 60, 160, 20);
                if (mouseClickRect.Intersects(skillTreeButtonRect))
                {
                    gameState = GameState.Paused;
                }
            }
            if (gameState == GameState.GameOver)
            {
                Rectangle restartButtonRect = new Rectangle((game.GraphicsDevice.Viewport.Width / 2 - restartButton.Width / 2 + 10), (game.GraphicsDevice.Viewport.Height / 2 + 15), 140, 20);
                Rectangle gameOverMainMenuButtonRect = new Rectangle((game.GraphicsDevice.Viewport.Width / 2 - mainMenuButton.Width / 2), (game.GraphicsDevice.Viewport.Height / 2 + 60), 190, 20);
                if (mouseClickRect.Intersects(restartButtonRect))
                {                    
                    game.Restart();
                    gameState = GameState.Playing;
                }
                else if (mouseClickRect.Intersects(gameOverMainMenuButtonRect))
                {
                    gameState = GameState.StartMenu;
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
            Rectangle optionsButtonRect1 = new Rectangle((int)controlsButtonPositionStartMenu.X + 20, (int)controlsButtonPositionStartMenu.Y, 160, 20);
            Rectangle optionsButtonRect2 = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - controlsButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 65, 160, 20);
            Rectangle exitButtonRect1 = new Rectangle((int)exitButtonPositionStartMenu.X + 20, (int)exitButtonPositionStartMenu.Y - 5, 120, 20);
            Rectangle exitButtonRect2 = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - exitButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 + 20, 120, 20);
            Rectangle mainMenuButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - mainMenuButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 20, 190, 20);
            Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X + 20, (int)startButtonPosition.Y + 28, 120, 30);
            Rectangle storyButtonRect = new Rectangle((int)storyButtonPosition.X + 20, (int)storyButtonPosition.Y + 10, 120, 30);
            Rectangle pausedReturnButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - returnButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 240, 120, 20);
            Rectangle startReturnButtonRect = new Rectangle((int)returnButtonPosition.X + 10, (int)returnButtonPosition.Y -180, 120, 30);
            Rectangle skillTreeButtonRect = new Rectangle((int)game.GraphicsDevice.Viewport.Width / 2 - skillTreeButton.Width, (int)game.GraphicsDevice.Viewport.Height / 2 - 65, 160, 20);
            Rectangle restartButtonRect = new Rectangle((game.GraphicsDevice.Viewport.Width / 2 - restartButton.Width / 2 + 20), (game.GraphicsDevice.Viewport.Height / 2 + 15), 140, 20);
            Rectangle gameOverMainMenuButtonRect = new Rectangle((game.GraphicsDevice.Viewport.Width / 2 - mainMenuButton.Width / 2), (game.GraphicsDevice.Viewport.Height / 2 + 60), 190, 20);

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
                originalControlsButton = controlsButton;
            }
            else if (mouseClickRect.Intersects(optionsButtonRect2) && gameState == GameState.Paused)
            {
                originalControlsButton = controlsButton;
            }
            else
            {
                originalControlsButton = controlsButton2;
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
            else if (mouseClickRect.Intersects(exitButtonRect2) && gameState == GameState.ControlsPauseMenu)
            {
                originalExitButton = exitButton;
            }
            else if (mouseClickRect.Intersects(exitButtonRect1) && gameState == GameState.ControlsStartMenu)
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
            if (mouseClickRect.Intersects(pausedReturnButtonRect) && gameState == GameState.ControlsPauseMenu)
            {
                originalReturnButton = returnButton;
            }
            else if (mouseClickRect.Intersects(startReturnButtonRect) && gameState == GameState.ControlsStartMenu)
            {
                originalReturnButton = returnButton;
            }
            else if (mouseClickRect.Intersects(startReturnButtonRect) && gameState == GameState.Story)
            {
                originalReturnButton = returnButton;
            }
            else
            {
                originalReturnButton = returnButton2;
            }
            if (mouseClickRect.Intersects(skillTreeButtonRect) && gameState == GameState.LevelUp)
            {
                originalSkillTreeButton = skillTreeButton;
            }
            else
            {
                originalSkillTreeButton = skillTreeButton2;
            }
            if (mouseClickRect.Intersects(mainMenuButtonRect) && gameState == GameState.Paused)
            {
                originalMainMenuButton = mainMenuButton;
            }
            else if(mouseClickRect.Intersects(gameOverMainMenuButtonRect) && gameState == GameState.GameOver)
            {
                originalMainMenuButton = mainMenuButton;
            }
            else
            {
                originalMainMenuButton = mainMenuButton2;
            }
            if(mouseClickRect.Intersects(restartButtonRect) && gameState == GameState.GameOver)
            {
                originalRestartButton = restartButton;
            }
            else
            {
                originalRestartButton = restartButton2;
            }

        }
                

            

        /// <summary>
        /// Drawing the graphics for the menus for the different game states.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="game"></param>
        public void Draw(SpriteBatch spriteBatch, Game1 game, Player player)
        {

            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(loadingBackground, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                //spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                //spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle(-(int)Camera.transform.Translation.X + 425, -(int)Camera.transform.Translation.Y + 250, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(originalStartButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - startButton.Width / 2, -(int)Camera.transform.Translation.Y + 310, startButton.Width, startButton.Height), Color.White);
                spriteBatch.Draw(originalControlsButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - controlsButton.Width / 2, -(int)Camera.transform.Translation.Y + 360, controlsButton.Width, controlsButton.Height), Color.White);
                spriteBatch.Draw(originalStoryButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - storyButton.Width / 2, -(int)Camera.transform.Translation.Y + 395, storyButton.Width, storyButton.Height), Color.White);
                spriteBatch.Draw(originalExitButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - exitButton.Width / 2, -(int)Camera.transform.Translation.Y + 435, exitButton.Width, exitButton.Height), Color.White);

            }

            if (gameState == GameState.Loading)
            {

                spriteBatch.Draw(loadingBackground, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(loadingGameButton, new Rectangle(-(int)Camera.transform.Translation.X + 480, -(int)Camera.transform.Translation.Y + 300, loadingGameButton.Width, loadingGameButton.Height), Color.CornflowerBlue);
                spriteBatch.End();
                spriteBatch.Begin();
                //Progressbar in loading menu
                Rectangle bar = new Rectangle(-(int)Camera.transform.Translation.X + 425, -(int)Camera.transform.Translation.Y - 400, (int)((Map.progress / Map.done) * 400), 40);
                Rectangle border = new Rectangle(-(int)Camera.transform.Translation.X + 425, -(int)Camera.transform.Translation.Y - 400, 400, 40);
                spriteBatch.Draw(progressBarBorder, border, Color.White);
                spriteBatch.Draw(progressBar, bar, Color.White);

                //The animation of the running woman in loading menu
                int FrameWidth = runningWoman.Width / 7;
                int FrameHeight = runningWoman.Height / 2;
                Rectangle sourcerect = new Rectangle(FrameWidth * colFrame, FrameHeight * 0,
                   FrameWidth, FrameHeight);
                spriteBatch.Draw(runningWoman, new Vector2(425, -(int)Camera.transform.Translation.Y - 500), sourcerect, Color.White,
                    0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 1.0f);

                TotalElapsed += 0.2f;
                if (TotalElapsed > TimePerFrame)
                {
                    colFrame++;
                    if (colFrame == 7)
                        colFrame = 1;
                    TotalElapsed -= TimePerFrame;
                }
            }

            if (gameState == GameState.Paused)
            {
                spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                //spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle(-(int)Camera.transform.Translation.X + 405, -(int)Camera.transform.Translation.Y + 150, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(originalResumeButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - resumeButton.Width / 2, -(int)Camera.transform.Translation.Y + 230, resumeButton.Width, resumeButton.Height), Color.White);
                spriteBatch.Draw(originalControlsButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - controlsButton.Width / 2, -(int)Camera.transform.Translation.Y + 280, controlsButton.Width, controlsButton.Height), Color.White);
                spriteBatch.Draw(originalMainMenuButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - mainMenuButton.Width / 2, -(int)Camera.transform.Translation.Y + 315, mainMenuButton.Width, mainMenuButton.Height), Color.White);
                spriteBatch.Draw(originalExitButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - exitButton.Width / 2, -(int)Camera.transform.Translation.Y + 355, exitButton.Width, exitButton.Height), Color.White);
                //spriteBatch.Draw(musicSlider, new Rectangle(-(int)Camera.transform.Translation.X + 1230, -(int)Camera.transform.Translation.Y + 10, musicSlider.Width, musicSlider.Height), Color.White);
                //spriteBatch.Draw(soundSlider, new Rectangle(-(int)Camera.transform.Translation.X + 1230, -(int)Camera.transform.Translation.Y + 60, soundSlider.Width, soundSlider.Height), Color.White);
            }

            if (gameState == GameState.ControlsStartMenu)
            {
                spriteBatch.Draw(loadingBackground, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(keyboardLayout, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                //spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                //spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle(-(int)Camera.transform.Translation.X + 425, -(int)Camera.transform.Translation.Y + 250, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(originalReturnButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - returnButton.Width / 2, -(int)Camera.transform.Translation.Y + 100, returnButton.Width, returnButton.Height), Color.White);
                //spriteBatch.Draw(originalExitButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - exitButton.Width / 2, -(int)Camera.transform.Translation.Y + 435, exitButton.Width, exitButton.Height), Color.White);
            }
            if (gameState == GameState.ControlsPauseMenu)
            {
                spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(keyboardLayout, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                //spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle(-(int)Camera.transform.Translation.X + 405, -(int)Camera.transform.Translation.Y + 150, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(originalReturnButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - returnButton.Width / 2, -(int)Camera.transform.Translation.Y + 100, returnButton.Width, returnButton.Height), Color.White);
                //spriteBatch.Draw(originalExitButton, new Rectangle(-(int)Camera.transform.Translation.X + 580 - exitButton.Width / 2, -(int)Camera.transform.Translation.Y + 315, exitButton.Width, exitButton.Height), Color.White);
            }
            if(gameState == GameState.Story)
            {
                spriteBatch.Draw(loadingBackground, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                //spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                //spriteBatch.Draw(pauseMenuBackgroundFront, new Rectangle(-(int)Camera.transform.Translation.X + 425, -(int)Camera.transform.Translation.Y + 250, pauseMenuBackgroundFront.Width, pauseMenuBackgroundFront.Height), Color.White);
                spriteBatch.Draw(originalReturnButton, new Rectangle(-(int)Camera.transform.Translation.X + 600 - returnButton.Width / 2, -(int)Camera.transform.Translation.Y + 100, returnButton.Width, returnButton.Height), Color.White);
                spriteBatch.Draw(game.Content.Load<Texture2D>("Menu items/Story"), new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y + 20, 1280, 720), Color.White);
            }
            if (gameState == GameState.GameOver)
            {
                spriteBatch.Draw(loadingBackground, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(gameOver, new Rectangle(-(int)Camera.transform.Translation.X + 640 - gameOver.Width / 2, -(int)Camera.transform.Translation.Y + 220, (int)420, (int)100), Color.White);
                spriteBatch.Draw(originalRestartButton, new Rectangle(-(int)Camera.transform.Translation.X + 640 - restartButton.Width / 2, -(int)Camera.transform.Translation.Y + 360, restartButton.Width, restartButton.Height), Color.White);
                spriteBatch.Draw(originalMainMenuButton, new Rectangle(-(int)Camera.transform.Translation.X + 640 - mainMenuButton.Width / 2, -(int)Camera.transform.Translation.Y + 400, mainMenuButton.Width, mainMenuButton.Height), Color.White);
                
                //The dead woman
                int FrameWidth = deadWoman.Width / 3;
                int FrameHeight = deadWoman.Height / 2;
                Rectangle sourcerect = new Rectangle(FrameWidth * 2, FrameHeight * 0,
                   FrameWidth, FrameHeight);
                spriteBatch.Draw(deadWoman, new Vector2(-(int)Camera.transform.Translation.X + 540, -(int)Camera.transform.Translation.Y + 163), sourcerect, Color.White,
                    0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 1.0f);

                
                
            }
            if (gameState == GameState.LevelUp)
            {
                spriteBatch.Draw(pauseMenuBackgroundBack, new Rectangle(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y, (int)1280, (int)720), Color.White);
                spriteBatch.Draw(SkilltreeCombat, new Rectangle(-(int)Camera.transform.Translation.X + 250, -(int)Camera.transform.Translation.Y + 200, (int)SkilltreeCombat.Width, (int)SkilltreeCombat.Height), Color.White);
                spriteBatch.Draw(SkilltreeMid, new Rectangle(-(int)Camera.transform.Translation.X + 550, -(int)Camera.transform.Translation.Y + 200, (int)SkilltreeMid.Width, (int)SkilltreeMid.Height), Color.White);
                spriteBatch.Draw(SkilltreeLong, new Rectangle(-(int)Camera.transform.Translation.X + 850, -(int)Camera.transform.Translation.Y + 200, (int)SkilltreeLong.Width, (int)SkilltreeLong.Height), Color.White);
                spriteBatch.DrawString(myFont, "Close", new Vector2(-(int)Camera.transform.Translation.X + 280, -(int)Camera.transform.Translation.Y + 160), Color.Green, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(myFont, "Middle", new Vector2(-(int)Camera.transform.Translation.X + 580, -(int)Camera.transform.Translation.Y + 160), Color.Red, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(myFont, "Range", new Vector2(-(int)Camera.transform.Translation.X + 880, -(int)Camera.transform.Translation.Y + 160), Color.Blue, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

                if(player.NextLevel)
                    spriteBatch.DrawString(myFont, "Upgrade\n Press 1 for close combat\n Press 2 for middle combat\n Press 3 for range combat", new Vector2(-(int)Camera.transform.Translation.X + 550, -(int)Camera.transform.Translation.Y + 560), Color.White);
            }
        }
    }
}