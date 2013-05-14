
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
    class Button
    {
        public Texture2D button, buttonHl;
        public Rectangle rectangle;
        public bool mouseOver { get; set; }

        public Button(Texture2D normal, Texture2D highlight, bool mouseOver)
        {
            this.button = normal;
            this.buttonHl = highlight;
            this.mouseOver = mouseOver;
            this.rectangle = Rectangle.Empty;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 viewPortPos)
        {
            position = new Vector2(position.X - (button.Width / 2), position.Y);

            rectangle = new Rectangle((int)(position.X - viewPortPos.X), (int)(position.Y - viewPortPos.Y), (int)button.Width, (int)this.button.Height);

            if(mouseOver)
                spriteBatch.Draw(buttonHl, position, Color.White);
            else
                spriteBatch.Draw(button, position, Color.White);
        }
    }

    class Menu
    {
        public enum GameState { StartMenu, Loading, Playing, Paused, ControlsMenu, Story, GameOver, LevelUp }
        
        private Button start, exit, control, story, back, resume, mainMenu, restart;  //De olika text knapparna
        private Button Shield, ShieldCD, MoreHP, Aim; //Long Skilltree knappar;
        private Button Fireburst, MoreAthyl, Passtrough, FastShot; //Mid Skilltree knappar;
        private Button AtkDmg, AtkSpd, FireBreath, Dodge; //close Skilltree knappar;

        private Texture2D menuBackground, keyboardLayout, storyText, loadingText, gameOverText, closeText, rangeText, middleText;

        private Texture2D deadWoman, progressBar, progressBarBorder, runningWoman;

        private Texture2D Combat, Mid, Long, ColorCombat, ColorMid, ColorLong;      //Button texture
        private Texture2D BarBorder, BarBorerBkg, AthylTexture, HealthTexture;      //Hp/Athyl mätare
        private Texture2D ExpBorder, ExpBar, ExpBarBkg;                             //Exp mätare
        private Texture2D SkilltreeCombat, SkilltreeMid, SkilltreeLong;             //Stance ikoner
        private Texture2D SkilltreeBorder, SkilltreeGray, SkilltreePipes;

        private Vector2 cameraPos, viewPortPos;                     //Olika positioner man kan använda för att få en standard.
        private Vector2 pos0, pos1, pos2, pos3, pos4, pos5, pos6, pos7, pos8, pos9 = Vector2.Zero;

        private int CloseColorIncrease, MidColorIncrease, LongColorIncrease;

        private int colFrame;
        private float TimePerFrame;
        private float TotalElapsed;
        public float totalTime = 0;

        public bool isLoading = false;
        public bool paused = false;
        private static bool runOnce = false;
        private SpriteFont myFont;
        MouseState mouseState;
        MouseState previousMouseState;

        public GameState gameState;
        public KeyboardState kbState, prevKdState;

        public Menu(Game1 game)
        {
            keyboardLayout = game.Content.Load<Texture2D>("Menu items/ControlMenu");
            storyText = game.Content.Load<Texture2D>("Menu items/story");
            loadingText = game.Content.Load<Texture2D>("Menu items/LoadingGameButton");
            gameOverText = game.Content.Load<Texture2D>("Menu items/GameOver");
            menuBackground = game.Content.Load<Texture2D>("Menu items/LoadingScreen");

            ColorCombat = game.Content.Load<Texture2D>("GUI/ColorCombat");  //Stance ikoner
            ColorMid = game.Content.Load<Texture2D>("GUI/ColorMidrange");
            ColorLong = game.Content.Load<Texture2D>("GUI/ColorLong");
            Combat = game.Content.Load<Texture2D>("GUI/NoColorCombat");
            Mid = game.Content.Load<Texture2D>("GUI/NoColorMidrange");
            Long = game.Content.Load<Texture2D>("GUI/NoColorLong");

            BarBorder = game.Content.Load<Texture2D>("GUI/HpAthylBar");         //Health och Athyl bars
            BarBorerBkg = game.Content.Load<Texture2D>("GUI/HpAthylBarBkg");
            HealthTexture = game.Content.Load<Texture2D>("GUI/HpBar");
            AthylTexture = game.Content.Load<Texture2D>("GUI/AthylBar");

            ExpBar = game.Content.Load<Texture2D>("GUI/ExpBar");            //Expbar
            ExpBarBkg = game.Content.Load<Texture2D>("GUI/ExpBarBKG");
            ExpBorder = game.Content.Load<Texture2D>("GUI/ExpBorder");

            SkilltreeCombat = game.Content.Load<Texture2D>("Menu items/TalentTreeClose");  //Skilltree tables
            SkilltreeMid = game.Content.Load<Texture2D>("Menu items/TalentTreeMiddle");
            SkilltreeLong = game.Content.Load<Texture2D>("Menu items/TalentTreeLong");
            closeText = game.Content.Load<Texture2D>("Menu items/CloseText");
            middleText = game.Content.Load<Texture2D>("Menu items/MiddleText");
            rangeText = game.Content.Load<Texture2D>("Menu items/RangeText");
            SkilltreeBorder = game.Content.Load<Texture2D>("Menu items/TalentTreeBorder");
            SkilltreeGray = game.Content.Load<Texture2D>("Menu items/GrayTalentTree");
            SkilltreePipes = game.Content.Load<Texture2D>("Menu items/TalentTreePipes");
            CloseColorIncrease = 0;
            MidColorIncrease = 0;
            LongColorIncrease = 0;

            start = new Button(game.Content.Load<Texture2D>("Menu items/StartButton"), game.Content.Load<Texture2D>("Menu items/StartButtonHighlight"), false);
            exit = new Button(game.Content.Load<Texture2D>("Menu items/ExitButton"), game.Content.Load<Texture2D>("Menu items/ExitButtonHighlight"), false);
            control = new Button(game.Content.Load<Texture2D>("Menu items/ControlsButton"), game.Content.Load<Texture2D>("Menu items/ControlsButtonHighlight"), false);
            story = new Button(game.Content.Load<Texture2D>("Menu items/StoryButton"), game.Content.Load<Texture2D>("Menu items/StoryButtonHighlight"), false);
            back = new Button(game.Content.Load<Texture2D>("Menu items/ReturnButton"), game.Content.Load<Texture2D>("Menu items/ReturnButtonHighlight"), false);
            resume = new Button(game.Content.Load<Texture2D>("Menu items/ResumeButton"), game.Content.Load<Texture2D>("Menu items/ResumeButtonHighlight"), false);
            mainMenu = new Button(game.Content.Load<Texture2D>("Menu items/MainMenuButton"), game.Content.Load<Texture2D>("Menu items/MainMenuButtonHighlight"), false);
            restart = new Button(game.Content.Load<Texture2D>("Menu items/Restartbutton"), game.Content.Load<Texture2D>("Menu items/RestartbuttonHighlight"), false);

            Shield = new Button(game.Content.Load<Texture2D>("Menu items/ShieldIkon"), game.Content.Load<Texture2D>("Menu items/ShieldIkonGray"), true);
            ShieldCD = new Button(game.Content.Load<Texture2D>("Menu items/ShieldCDIcon"), game.Content.Load<Texture2D>("Menu items/ShieldCDIconGray"), true);
            MoreHP = new Button(game.Content.Load<Texture2D>("Menu items/AidIcon"), game.Content.Load<Texture2D>("Menu items/AidIconGray"), true);
            Aim = new Button(game.Content.Load<Texture2D>("Menu items/AimIcon"), game.Content.Load<Texture2D>("Menu items/AimIconGray"), true);

            Fireburst = new Button(game.Content.Load<Texture2D>("Menu items/FBIcon"), game.Content.Load<Texture2D>("Menu items/FBIconGray"), true);
            MoreAthyl = new Button(game.Content.Load<Texture2D>("Menu items/AthylIcon"), game.Content.Load<Texture2D>("Menu items/AthylIconGray"), true);
            Passtrough = new Button(game.Content.Load<Texture2D>("Menu items/PenetrationIcon"), game.Content.Load<Texture2D>("Menu items/PenetrationIconGray"), true);
            FastShot = new Button(game.Content.Load<Texture2D>("Menu items/FireSPDIcon"), game.Content.Load<Texture2D>("Menu items/FireSPDIconGray"), true);

            AtkDmg = new Button(game.Content.Load<Texture2D>("Menu items/YUIcon"), game.Content.Load<Texture2D>("Menu items/YUIconGray"), true);
            AtkSpd = new Button(game.Content.Load<Texture2D>("Menu items/FastFisting"), game.Content.Load<Texture2D>("Menu items/FastFistingGray"), true);
            FireBreath = new Button(game.Content.Load<Texture2D>("Menu items/Fireburst"), game.Content.Load<Texture2D>("Menu items/FireburstGray"), true);
            Dodge = new Button(game.Content.Load<Texture2D>("Menu items/HornedMan"), game.Content.Load<Texture2D>("Menu items/HornedManGray"), true);


            this.TimePerFrame = (float)1 / 1;           //Update animations
            this.TotalElapsed = 0;
            this.colFrame = 1;
            
            progressBar = game.Content.Load<Texture2D>("ProgressBar");
            progressBarBorder = game.Content.Load<Texture2D>("ProgressBarBorder");
            runningWoman = game.Content.Load<Texture2D>("Player/Gilliam");
            deadWoman = game.Content.Load<Texture2D>("Player/die");

            myFont = game.Content.Load<SpriteFont>("font");
        }

        private void setButtonPositions(Game1 game)
        {
            cameraPos = new Vector2(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y);
            viewPortPos = new Vector2(game.GraphicsDevice.Viewport.X + cameraPos.X, game.GraphicsDevice.Viewport.Y + cameraPos.Y);
            pos0 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 50);
            pos1 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 300);
            pos2 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 350);
            pos3 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 400);
            pos4 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 450);
            pos5 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 500);
            pos6 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 550);
            pos7 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 600);
            pos8 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 630);
            pos9 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 700);
        }

        /// <summary>
        /// UpdateMenu used to handle different states. Pausing the game and keeping track on if the mouse is clicked on buttons.
        /// </summary>
        /// <param name="gametime"></param>
        /// <param name="game"></param>
        public void UpdateMenu(GameTime gametime, Game1 game, Player player)
        {
            mouseState = Mouse.GetState();
            kbState =  Keyboard.GetState();
            setButtonPositions(game);

            if (kbState.IsKeyDown(Keys.Escape) && !prevKdState.IsKeyDown(Keys.Escape) && gameState == GameState.Playing)
            {
                gameState = GameState.Paused;
                paused = true;
            }
            else if (kbState.IsKeyDown(Keys.Escape) && !prevKdState.IsKeyDown(Keys.Escape) && gameState == GameState.Paused)
            {
                gameState = GameState.Playing;
                paused = false;
            }
            if (kbState.IsKeyDown(Keys.N) && !prevKdState.IsKeyDown(Keys.N) && gameState == GameState.Playing)
            {
                gameState = GameState.LevelUp;
                paused = true;
            }
            else if (kbState.IsKeyDown(Keys.N) && !prevKdState.IsKeyDown(Keys.N) && gameState == GameState.LevelUp)
            {
                gameState = GameState.Playing;
                paused = false;
            }

            if (gameState == GameState.LevelUp && player.NextLevel)
            {
                if (kbState.IsKeyDown(InputClass.closeKey) && !prevKdState.IsKeyDown(InputClass.closeKey))
                {
                    player.skillTree.LevelCloseRange();
                    player.skillPoints--;
                }
                else if (kbState.IsKeyDown(InputClass.middleKey) && !prevKdState.IsKeyDown(InputClass.middleKey))
                {
                    player.skillTree.LevelMidRange();
                    player.skillPoints--;
                }
                else if (kbState.IsKeyDown(InputClass.longKey) && !prevKdState.IsKeyDown(InputClass.longKey))
                {
                    player.skillTree.LongRange();
                    player.skillPoints--;
                }

                if (player.skillPoints == 0)
                    player.NextLevel = false;
            }

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
                    paused = false;
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

            UpdateColorTalentree(player);

            MouseOver(game, player);
            previousMouseState = mouseState;
            prevKdState = kbState;
        }

        /// <summary>
        /// Uppdaterar så att skillträdets färger stämmer med poängen.
        /// </summary>
        /// <param name="player"></param>
        public void UpdateColorTalentree(Player player)
        {
            if (player.skillPoints >= 1)     //Ska längre fram öka färgen på skillträdet.
            {
                CloseColorIncrease = 1;
                FireBreath.mouseOver = false;
                MidColorIncrease = 1;
                Fireburst.mouseOver = false;
                LongColorIncrease = 1;
                Shield.mouseOver = false;
                if (player.skillTree.firebreathPoint > 0)
                {
                    CloseColorIncrease = 2;
                    AtkDmg.mouseOver = false;
                    AtkSpd.mouseOver = false;
                }
                if (player.skillTree.FireBurstPoint > 0)
                {
                    MidColorIncrease = 2;
                    MoreAthyl.mouseOver = false;
                    Passtrough.mouseOver = false;
                }
                if (player.skillTree.ShieldPoint > 0)
                {
                    LongColorIncrease = 2;
                    MoreHP.mouseOver = false;
                    Aim.mouseOver = false;
                }

                if (player.skillTree.AtkDmgPoint == 5 || player.skillTree.AtkSpdPoint == 5)
                {
                    CloseColorIncrease = 3;
                    Dodge.mouseOver = false;
                }
                if (player.skillTree.AthylPoint == 5 || player.skillTree.PassthroughPoint == 5)
                {
                    MidColorIncrease = 3;
                    FastShot.mouseOver = false;
                }
                if (player.skillTree.HPPoint == 5 || player.skillTree.AimPoint == 5)
                {
                    LongColorIncrease = 3;
                    ShieldCD.mouseOver = false;
                }
            }
            else
            {
                CloseColorIncrease = 0;
                MidColorIncrease = 0;
                LongColorIncrease = 0;
            }
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
            UpdateStanceButtons(spriteBatch, player);
            UpdateGuiBars(spriteBatch, player);

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
        void MouseClicked(Game1 game, Player player)
        {
            if (start.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                gameState = GameState.Loading;
                isLoading = true;
            }
            else if (exit.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                game.Exit();
            }
            else if (control.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                gameState = GameState.ControlsMenu;
            }
            else if (story.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                gameState = GameState.Story;
            }
            else if (resume.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                gameState = GameState.Playing;
            }

            else if (back.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                if (gameState == GameState.Story && paused)
                    gameState = GameState.Paused;
                else if (gameState == GameState.Story && !paused)
                    gameState = GameState.StartMenu;
                else if (gameState == GameState.ControlsMenu && paused)
                    gameState = GameState.Paused;
                else if (gameState == GameState.ControlsMenu && !paused)
                    gameState = GameState.StartMenu;
            }

            else if (restart.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                game.Restart();
                gameState = GameState.Playing;
            }
            else if (mainMenu.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                gameState = GameState.StartMenu;
                game.Restart();
            }
            else if (exit.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                game.Exit();
            }

            skillTreeClick(player);
        }

        public void skillTreeClick(Player player)
        {
            if (player.skillPoints > 0)
            {
                if (FireBreath.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.firebreathPoint < 5)
                {
                    player.skillTree.firebreathPoint++;
                    player.skillPoints--;
                }
                else if (Fireburst.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.FireBurstPoint < 5)
                {
                    player.skillTree.FireBurstPoint++;
                    player.skillPoints--;
                }
                else if (Shield.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.ShieldPoint < 5)
                {
                    player.skillTree.ShieldPoint++;
                    player.skillPoints--;
                }
                else if (AtkDmg.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.AtkDmgPoint < 5)
                {
                    player.skillTree.AtkDmgPoint++;
                    player.skillPoints--;
                }
                else if (MoreAthyl.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.AthylPoint < 5)
                {
                    player.skillTree.AthylPoint++;
                    player.skillPoints--;
                }
                else if (MoreHP.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.HPPoint < 5)
                {
                    player.skillTree.HPPoint++;
                    player.skillPoints--;
                }
                else if (AtkSpd.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.AtkSpdPoint < 5)
                {
                    player.skillTree.AtkSpdPoint++;
                    player.skillPoints--;
                }
                else if (Passtrough.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.PassthroughPoint < 5)
                {
                    player.skillTree.PassthroughPoint++;
                    player.skillPoints--;
                }
                else if (Aim.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.AimPoint < 5)
                {
                    player.skillTree.AimPoint++;
                    player.skillPoints--;
                }
                else if (Dodge.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.DodgePoint < 5)
                {
                    player.skillTree.DodgePoint++;
                    player.skillPoints--;
                }
                else if (FastShot.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.FastShotPoint < 5)
                {
                    player.skillTree.FastShotPoint++;
                    player.skillPoints--;
                }
                else if (ShieldCD.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.ShieldCDPoint < 5)
                {
                    player.skillTree.ShieldCDPoint++;
                    player.skillPoints--;
                }
            }
        }

        /// <summary>
        /// Function that Highlights a button when the mouse pointer intesects with button.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="game"></param>
        public void MouseOver(Game1 game, Player player)
        {
            if (start.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                start.mouseOver = true;
            }
            else
            {
                start.mouseOver = false;
            }
            if (control.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                control.mouseOver = true;
            }
            else
            {
                control.mouseOver = false;
            }
            if (story.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                story.mouseOver = true;
            }
            else
            {
                story.mouseOver = false;
            }
            if (exit.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                exit.mouseOver = true;
            }
            else
            {
                exit.mouseOver = false;
            }
            if (back.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                back.mouseOver = true;
            }
            else
            {
                back.mouseOver = false;
            }
            if (resume.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                resume.mouseOver = true;
            }
            else
            {
                resume.mouseOver = false;
            }
            if (restart.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                restart.mouseOver = true;
            }
            else
            {
                restart.mouseOver = false;
            }
            if (mainMenu.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                mainMenu.mouseOver = true;
            }
            else
            {
                mainMenu.mouseOver = false;
            }

            if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                MouseClicked(game, player);
            }
        }
                
        /// <summary>
        /// Drawing the graphics for the menus for the different game states.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="game"></param>
        public void Draw(SpriteBatch spriteBatch, Game1 game, Player player)
        {
            start.Draw(spriteBatch, new Vector2(-100,-100), viewPortPos);
            control.Draw(spriteBatch, new Vector2(-100, -100), viewPortPos);
            story.Draw(spriteBatch, new Vector2(-100, -100), viewPortPos);
            exit.Draw(spriteBatch, new Vector2(-100, -100), viewPortPos);
            resume.Draw(spriteBatch, new Vector2(-100, -100), viewPortPos);
            restart.Draw(spriteBatch, new Vector2(-100, -100), viewPortPos);
            mainMenu.Draw(spriteBatch, new Vector2(-100, -100), viewPortPos);
            back.Draw(spriteBatch, new Vector2(-100,-100), viewPortPos);

            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(menuBackground, cameraPos, Color.White);
                start.Draw(spriteBatch, pos1, viewPortPos);
                control.Draw(spriteBatch, pos2, viewPortPos);
                story.Draw(spriteBatch, pos3, viewPortPos);
                exit.Draw(spriteBatch, pos4, viewPortPos);
            }

            else if (gameState == GameState.Loading)
            {
                spriteBatch.Draw(menuBackground, cameraPos, Color.White);
                spriteBatch.Draw(loadingText, pos1, Color.White);
                spriteBatch.End();
                spriteBatch.Begin();

                //Progressbar in loading menu
                Rectangle bar = new Rectangle(-(int)Camera.transform.Translation.X + 425, -(int)Camera.transform.Translation.Y - 400, (int)((Map.progress / Map.done) * 400), 40);
                Rectangle border = new Rectangle(-(int)Camera.transform.Translation.X + 425, -(int)Camera.transform.Translation.Y - 400, 400, 40);
                spriteBatch.Draw(progressBar, bar, Color.White);
                spriteBatch.Draw(progressBarBorder, border, Color.White);

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

            else if (gameState == GameState.Paused)
            {
                resume.Draw(spriteBatch, pos1, viewPortPos);
                restart.Draw(spriteBatch, pos2, viewPortPos);
                control.Draw(spriteBatch, pos3, viewPortPos);
                story.Draw(spriteBatch, pos4, viewPortPos);
                exit.Draw(spriteBatch, pos5, viewPortPos);
            }

            else if (gameState == GameState.ControlsMenu)
            {
                spriteBatch.Draw(menuBackground, cameraPos, Color.White);
                spriteBatch.Draw(keyboardLayout, cameraPos, Color.White);
                back.Draw(spriteBatch, pos0, viewPortPos);
            }

            else if (gameState == GameState.Story)
            {
                spriteBatch.Draw(menuBackground, cameraPos, Color.White);
                spriteBatch.Draw(storyText, cameraPos, Color.White);
                back.Draw(spriteBatch, pos0, viewPortPos);
            }

            else if (gameState == GameState.GameOver)
            {
                spriteBatch.Draw(menuBackground, cameraPos, Color.White);
                spriteBatch.Draw(gameOverText, new Vector2(pos1.X - gameOverText.Width/2, pos1.Y), Color.White);
                restart.Draw(spriteBatch, pos4, viewPortPos);
                mainMenu.Draw(spriteBatch, pos5, viewPortPos);
                //The dead woman
                int FrameWidth = deadWoman.Width / 3;
                int FrameHeight = deadWoman.Height / 2;
                Rectangle sourcerect = new Rectangle(FrameWidth * 2, FrameHeight * 0,
                   FrameWidth, FrameHeight);
                spriteBatch.Draw(deadWoman, new Vector2(-(int)Camera.transform.Translation.X + 600, -(int)Camera.transform.Translation.Y + 163), sourcerect, Color.White,
                    0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 1.0f);
            }

            else if (gameState == GameState.LevelUp)
            {
                spriteBatch.Draw(SkilltreeGray, new Rectangle(-(int)Camera.transform.Translation.X + 258, -(int)Camera.transform.Translation.Y + 208, (int)SkilltreeGray.Width, (int)SkilltreeGray.Height), Color.White);
                spriteBatch.Draw(SkilltreeGray, new Rectangle(-(int)Camera.transform.Translation.X + 558, -(int)Camera.transform.Translation.Y + 208, (int)SkilltreeGray.Width, (int)SkilltreeGray.Height), Color.White);
                spriteBatch.Draw(SkilltreeGray, new Rectangle(-(int)Camera.transform.Translation.X + 858, -(int)Camera.transform.Translation.Y + 208, (int)SkilltreeGray.Width, (int)SkilltreeGray.Height), Color.White);
                spriteBatch.Draw(SkilltreeCombat, new Rectangle(-(int)Camera.transform.Translation.X + 258, -(int)Camera.transform.Translation.Y + 208, (int)SkilltreeCombat.Width, (int)(SkilltreeCombat.Height * 0.34) * CloseColorIncrease), Color.White);
                spriteBatch.Draw(SkilltreeMid, new Rectangle(-(int)Camera.transform.Translation.X + 558, -(int)Camera.transform.Translation.Y + 208, (int)SkilltreeMid.Width, (int)(SkilltreeCombat.Height * 0.34) * MidColorIncrease), Color.White);
                spriteBatch.Draw(SkilltreeLong, new Rectangle(-(int)Camera.transform.Translation.X + 858, -(int)Camera.transform.Translation.Y + 208, (int)SkilltreeLong.Width, (int)(SkilltreeCombat.Height * 0.34) * LongColorIncrease), Color.White);
                spriteBatch.Draw(SkilltreeBorder, new Rectangle(-(int)Camera.transform.Translation.X + 250, -(int)Camera.transform.Translation.Y + 200, (int)SkilltreeBorder.Width, (int)SkilltreeBorder.Height), Color.White);
                spriteBatch.Draw(SkilltreeBorder, new Rectangle(-(int)Camera.transform.Translation.X + 550, -(int)Camera.transform.Translation.Y + 200, (int)SkilltreeBorder.Width, (int)SkilltreeBorder.Height), Color.White);
                spriteBatch.Draw(SkilltreeBorder, new Rectangle(-(int)Camera.transform.Translation.X + 850, -(int)Camera.transform.Translation.Y + 200, (int)SkilltreeBorder.Width, (int)SkilltreeBorder.Height), Color.White);
                spriteBatch.Draw(SkilltreePipes, new Rectangle(-(int)Camera.transform.Translation.X + 290, -(int)Camera.transform.Translation.Y + 265, (int)SkilltreePipes.Width, (int)SkilltreePipes.Height), Color.White);
                spriteBatch.Draw(SkilltreePipes, new Rectangle(-(int)Camera.transform.Translation.X + 590, -(int)Camera.transform.Translation.Y + 265, (int)SkilltreePipes.Width, (int)SkilltreePipes.Height), Color.White);
                spriteBatch.Draw(SkilltreePipes, new Rectangle(-(int)Camera.transform.Translation.X + 890, -(int)Camera.transform.Translation.Y + 265, (int)SkilltreePipes.Width, (int)SkilltreePipes.Height), Color.White);

                FireBreath.Draw(spriteBatch, cameraPos + new Vector2(335,250), viewPortPos);     //Första nivå skillsen
                Fireburst.Draw(spriteBatch, cameraPos + new Vector2(635, 250), viewPortPos);
                Shield.Draw(spriteBatch, cameraPos + new Vector2(935,250), viewPortPos);

                AtkDmg.Draw(spriteBatch, cameraPos + new Vector2(295, 350), viewPortPos);       //Andra vänster nivå skillsen
                MoreAthyl.Draw(spriteBatch, cameraPos + new Vector2(595, 350), viewPortPos);
                MoreHP.Draw(spriteBatch, cameraPos + new Vector2(895, 350), viewPortPos);

                AtkSpd.Draw(spriteBatch, cameraPos + new Vector2(380, 350), viewPortPos);       //Andra höger nivå skillsen
                Passtrough.Draw(spriteBatch, cameraPos + new Vector2(680, 350), viewPortPos);
                Aim.Draw(spriteBatch, cameraPos + new Vector2(980, 350), viewPortPos);

                Dodge.Draw(spriteBatch, cameraPos + new Vector2(335, 450), viewPortPos);        //Sista nivå skillsen
                FastShot.Draw(spriteBatch, cameraPos + new Vector2(635, 450), viewPortPos);
                ShieldCD.Draw(spriteBatch, cameraPos + new Vector2(935, 450), viewPortPos);

                spriteBatch.Draw(closeText, new Vector2(-(int)Camera.transform.Translation.X + 285, -(int)Camera.transform.Translation.Y + 185), Color.White);
                spriteBatch.Draw(middleText, new Vector2(-(int)Camera.transform.Translation.X + 575, -(int)Camera.transform.Translation.Y + 190), Color.White);
                spriteBatch.Draw(rangeText, new Vector2(-(int)Camera.transform.Translation.X + 885, -(int)Camera.transform.Translation.Y + 188), Color.White);
                
                spriteBatch.DrawString(myFont, "Points: " + player.skillPoints, pos0 - new Vector2(50, 0), Color.Gold);

                if(player.NextLevel)
                    spriteBatch.DrawString(myFont, "Upgrade\n Press 1 for close combat\n Press 2 for middle combat\n Press 3 for range combat", new Vector2(-(int)Camera.transform.Translation.X + 550, -(int)Camera.transform.Translation.Y + 560), Color.White);
            }
        }
    }
}