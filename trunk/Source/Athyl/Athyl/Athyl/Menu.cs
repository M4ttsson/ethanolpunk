
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
        public bool toggleTextures { get; set; }


        public Button(Texture2D normal, Texture2D highlight, bool toggleTextures)
        {
            this.button = normal;
            this.buttonHl = highlight;
            this.toggleTextures = toggleTextures;
            this.rectangle = Rectangle.Empty;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 viewPortPos)
        {
            position = new Vector2(position.X - (button.Width / 2), position.Y);
            rectangle = new Rectangle((int)(position.X - viewPortPos.X), (int)(position.Y - viewPortPos.Y), (int)button.Width, (int)this.button.Height);

            if (toggleTextures)
                spriteBatch.Draw(buttonHl, position, Color.White);
            else
                spriteBatch.Draw(button, position, Color.White);
        }
    }

    class HooverText
    {
        private SpriteFont font;
        private string text;
        public bool visible;

        public HooverText(Game1 game, string text, bool visible)
        {
            this.font = game.Content.Load<SpriteFont>("font");
            this.text = text;
            this.visible = visible;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            if(visible)
                spriteBatch.DrawString(font, text, pos, Color.White,0,Vector2.Zero,0.75f,SpriteEffects.None,1);
        }
    }

    class Menu
    {
        #region Properties
        public enum GameState { StartMenu, Loading, Playing, Paused, ControlsMenu, Story, GameOver, Skilltree }

        private Button start, exit, control, story, back, resume, mainMenu, restart;  //De olika text knapparna
        private Button Shield, ShieldCD, Kevlar, Aim;                                //Long Skilltree knappar;
        private Button Fireburst, MoreAthyl, Passtrough, FastShot;                  //Mid Skilltree knappar;
        private Button AtkDmg, AtkSpd, FireBreath, Dodge;                          //close Skilltree knappar;

        private HooverText fireBreathText, AtkDmgText, AtkSpdText, DodgeText;          //Hoovertext för de olika skillsen i close
        private HooverText fireBurstText, AthylText, PasstroughText, FastShotText;      //Hoovertext för de olika skillsen i midd
        private HooverText ShieldText, KelvarText, AimText, ShieldCDText;               //Hoovertext för de olika skillsen i long

        private Texture2D menuBackground, pauseBackground, keyboardLayout, AthylLogo; 
        private Texture2D storyText, loadingText, gameOverText, PauseText, closeText, rangeText, middleText;

        private Texture2D deadWoman, progressBar, progressBarBorder, runningWoman;

        private Texture2D Combat, Mid, Long, ColorCombat, ColorMid, ColorLong;      //Button texture
        private Texture2D BarBorder, BarBorerBkg, AthylTexture, HealthTexture;      //Hp/Athyl mätare
        private Texture2D ExpBorder, ExpBar, ExpBarBkg;                             //Exp mätare
        private Texture2D SkilltreeCombat, SkilltreeMid, SkilltreeLong;             //Stance ikoner
        private Texture2D SkilltreeBorder, SkilltreeGray, SkilltreePipes;

        private Vector2 cameraPos, viewPortPos, viewPortDim;                  //Olika positioner man kan använda för att få en standard.
        private Vector2 pos0, pos1, pos2, pos3, pos4, pos5, pos6, pos7, pos8, pos9 = Vector2.Zero;

        private int CloseColorIncrease, MidColorIncrease, LongColorIncrease;
        private Rectangle SkillInfoRect;
        private int colFrame;
        private float TimePerFrame;
        private float TotalElapsed;
        public float totalTime = 0;

        public bool isLoading = false;
        public bool paused = false;
        private static bool runOnce = false;
        private SpriteFont myFont;
        private MouseState mouseState;
        private MouseState previousMouseState;

        public GameState gameState;
        public KeyboardState kbState, prevKdState; 
        #endregion
        #region Constructor
        public Menu(Game1 game)
        {
            viewPortDim = new Vector2(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);

            keyboardLayout = game.Content.Load<Texture2D>("Menu items/ControlMenu");
            storyText = game.Content.Load<Texture2D>("Menu items/story");
            loadingText = game.Content.Load<Texture2D>("Menu items/LoadingGameButton");
            gameOverText = game.Content.Load<Texture2D>("Menu items/GameOver");
            PauseText = game.Content.Load<Texture2D>("Menu items/Pause");
            menuBackground = game.Content.Load<Texture2D>("Menu items/LoadingScreen");
            pauseBackground = game.Content.Load<Texture2D>("Menu items/PauseMenuBackground");
            AthylLogo = game.Content.Load<Texture2D>("Menu items/Logotyp");

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
            Kevlar = new Button(game.Content.Load<Texture2D>("Menu items/KevlarIcon"), game.Content.Load<Texture2D>("Menu items/KevlarIconGray"), true);
            Aim = new Button(game.Content.Load<Texture2D>("Menu items/AimIcon"), game.Content.Load<Texture2D>("Menu items/AimIconGray"), true);

            Fireburst = new Button(game.Content.Load<Texture2D>("Menu items/FBIcon"), game.Content.Load<Texture2D>("Menu items/FBIconGray"), true);
            MoreAthyl = new Button(game.Content.Load<Texture2D>("Menu items/AthylIcon"), game.Content.Load<Texture2D>("Menu items/AthylIconGray"), true);
            Passtrough = new Button(game.Content.Load<Texture2D>("Menu items/PenetrationIcon"), game.Content.Load<Texture2D>("Menu items/PenetrationIconGray"), true);
            FastShot = new Button(game.Content.Load<Texture2D>("Menu items/FireSPDIcon"), game.Content.Load<Texture2D>("Menu items/FireSPDIconGray"), true);

            AtkDmg = new Button(game.Content.Load<Texture2D>("Menu items/YUIcon"), game.Content.Load<Texture2D>("Menu items/YUIconGray"), true);
            AtkSpd = new Button(game.Content.Load<Texture2D>("Menu items/FastFisting"), game.Content.Load<Texture2D>("Menu items/FastFistingGray"), true);
            FireBreath = new Button(game.Content.Load<Texture2D>("Menu items/Fireburst"), game.Content.Load<Texture2D>("Menu items/FireburstGray"), true);
            Dodge = new Button(game.Content.Load<Texture2D>("Menu items/HornedMan"), game.Content.Load<Texture2D>("Menu items/HornedManGray"), true);

            ShieldText = new HooverText(game, "\nShield: Places a shield in front of Gilliam, protecting her from attacks.\nUse with " + InputClass.useKey.ToString() + "-button.", false);
            ShieldCDText = new HooverText(game, "\nReduce cooldown: Reduces the cooldown of the shield.", false);
            KelvarText = new HooverText(game, "\nArmor: Reduces the damage taken from enemy attacks.\nOBS! Skill is under construction!", false);
            AimText = new HooverText(game, "\nLaser Sight: Gives you a better aim with a laser.\nOBS! Skill is under construction!", false);

            fireBurstText = new HooverText(game, "\nBurst: Fires a rapid burst of bullets. Gives increased damage.\nUse with " + InputClass.useKey.ToString() + "-button.", false);
            AthylText = new HooverText(game, "\nGive me Athyl: Increases your max Athyl reserves.", false);
            PasstroughText = new HooverText(game, "\nPiercing Bullets: Gives a chance for bullets to penetrate through enemies.\nOBS! Skill is under construction!", false);
            FastShotText = new HooverText(game, "\nRapid fire: Increases the rate of fire.", false);

            AtkDmgText = new HooverText(game, "\nBalcon Punsch: Fisting deals more damage.", false);
            AtkSpdText = new HooverText(game, "\nFast Fisting: Fist faster!", false);
            fireBreathText = new HooverText(game, "\nFlamethrower: Burn your enemies in flames!\nUse with " + InputClass.useKey.ToString() + "-button.", false);
            DodgeText = new HooverText(game, "\nMatrix Style: Gives a chance for Gilliam to avoid bullets.\nOBS! Skill is under construction!", false);

            this.TimePerFrame = (float)1 / 1;           //Update animations
            this.TotalElapsed = 0;
            this.colFrame = 1;

            progressBar = game.Content.Load<Texture2D>("ProgressBar");
            progressBarBorder = game.Content.Load<Texture2D>("ProgressBarBorder");
            runningWoman = game.Content.Load<Texture2D>("Player/Gilliam");
            deadWoman = game.Content.Load<Texture2D>("Player/die");

            myFont = game.Content.Load<SpriteFont>("font");
        } 
        #endregion
        #region UpdatesAndSkillTreeApperance
        /// <summary>
        /// UpdateMenu used to handle different states. Pausing the game and keeping track on if the mouse is clicked on buttons.
        /// </summary>
        /// <param name="gametime"></param>
        /// <param name="game"></param>
        public void UpdateMenu(GameTime gametime, Game1 game, Player player)
        {
            mouseState = Mouse.GetState();
            kbState = Keyboard.GetState();
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
                gameState = GameState.Skilltree;
                paused = true;
            }
            else if (kbState.IsKeyDown(Keys.N) && !prevKdState.IsKeyDown(Keys.N) && gameState == GameState.Skilltree)
            {
                gameState = GameState.Playing;
                paused = false;
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

            UpdateColorSkilltree(player);

            MouseOver(game, player);
            previousMouseState = mouseState;
            prevKdState = kbState;
        }

        /// <summary>
        /// Calculates the standards position for buttons and such.
        /// </summary>
        /// <param name="game"></param>
        private void setButtonPositions(Game1 game)
        {
            cameraPos = new Vector2(-(int)Camera.transform.Translation.X, -(int)Camera.transform.Translation.Y);
            viewPortPos = new Vector2(game.GraphicsDevice.Viewport.X + cameraPos.X, game.GraphicsDevice.Viewport.Y + cameraPos.Y);
            pos0 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 50);
            pos1 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 100);
            pos2 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 150);
            pos3 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 200);
            pos4 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 250);
            pos5 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 300);
            pos6 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 350);
            pos7 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 400);
            pos8 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 450);
            pos9 = cameraPos + new Vector2(game.graphics.PreferredBackBufferWidth / 2, 500);
        }

        /// <summary>
        /// Uppdaterar så att skillträdets färger stämmer med poängen.
        /// </summary>
        /// <param name="player"></param>
        public void UpdateColorSkilltree(Player player)
        {
            CloseColorIncrease = 3;  //Dessa ändrar färgen på själva skillsbordets bakgrund
            MidColorIncrease = 3;
            LongColorIncrease = 3;

            ifFullIncreaseColor(player);
            ifPointsIncreaseColor(player);
        }

        /// <summary>
        /// Ger de skillsen som har poäng färg.
        /// </summary>
        /// <param name="player"></param>
        public void ifFullIncreaseColor(Player player)
        {
            if (player.skillTree.firebreathPoint > 0)
                FireBreath.toggleTextures = false;
            else
                FireBreath.toggleTextures = true;
            if (player.skillTree.AtkDmgPoint > 0)
                AtkDmg.toggleTextures = false;
            else
                AtkDmg.toggleTextures = true;
            if (player.skillTree.AtkSpdPoint > 0)
                AtkSpd.toggleTextures = false;
            else
                AtkSpd.toggleTextures = true;
            if (player.skillTree.DodgePoint > 0)
                Dodge.toggleTextures = false;
            else
                Dodge.toggleTextures = true;

            if (player.skillTree.FireBurstPoint > 0)
                Fireburst.toggleTextures = false;
            else
                Fireburst.toggleTextures = true;
            if (player.skillTree.AthylPoint > 0)
                MoreAthyl.toggleTextures = false;
            else
                MoreAthyl.toggleTextures = true;
            if (player.skillTree.PassthroughPoint > 0)
                Passtrough.toggleTextures = false;
            else
                Passtrough.toggleTextures = true;
            if (player.skillTree.FastShotPoint > 0)
                FastShot.toggleTextures = false;
            else
                FastShot.toggleTextures = true;

            if (player.skillTree.ShieldPoint > 0)
                Shield.toggleTextures = false;
            else
                Shield.toggleTextures = true;
            if (player.skillTree.KevlarPoint > 0)
                Kevlar.toggleTextures = false;
            else
                Kevlar.toggleTextures = true;
            if (player.skillTree.AimPoint > 0)
                Aim.toggleTextures = false;
            else
                Aim.toggleTextures = true;
            if (player.skillTree.ShieldCDPoint > 0)
                ShieldCD.toggleTextures = false;
            else
                ShieldCD.toggleTextures = true;
        } 

        /// <summary>
        /// Om tillgängliga poäng, ge färg till de skills man får lägga på.
        /// </summary>
        /// <param name="player"></param>
        public void ifPointsIncreaseColor(Player player)
        {
            if (player.skillPoints >= 1)     //Ska längre fram öka färgen på skillträdet.
            {
                FireBreath.toggleTextures = false;
                Fireburst.toggleTextures = false;
                Shield.toggleTextures = false;
                if (player.skillTree.firebreathPoint > 0)
                {
                    AtkDmg.toggleTextures = false;
                    AtkSpd.toggleTextures = false;
                    if (player.skillTree.AtkDmgPoint == 5 || player.skillTree.AtkSpdPoint == 5)
                    {
                        Dodge.toggleTextures = false;
                    }
                }
                if (player.skillTree.FireBurstPoint > 0)
                {
                    MoreAthyl.toggleTextures = false;
                    Passtrough.toggleTextures = false;
                    if (player.skillTree.AthylPoint == 5 || player.skillTree.PassthroughPoint == 5)
                    {
                        FastShot.toggleTextures = false;
                    }
                }
                if (player.skillTree.ShieldPoint > 0)
                {
                    Kevlar.toggleTextures = false;
                    Aim.toggleTextures = false;
                    if (player.skillTree.KevlarPoint == 5 || player.skillTree.AimPoint == 5)
                    {
                        ShieldCD.toggleTextures = false;
                    }
                }
            }
        }
        #endregion
        #region GUIUpdateAndDraw
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
        #endregion
        #region MouseClickAndHighlight
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

        /// <summary>
        /// Bestämmer vad som ska hända när man klickar i skillträdet.
        /// </summary>
        /// <param name="player"></param>
        public void skillTreeClick(Player player)
        {
            if (player.skillPoints > 0)
            {
                if (FireBreath.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.firebreathPoint < 5)
                {
                    player.skillTree.firebreathPoint++;
                    player.skillTree.increasefireBreath();
                    player.skillTree.CloseRange();
                    player.skillPoints--;
                }
                else if (Fireburst.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.FireBurstPoint < 5)
                {
                    player.skillTree.FireBurstPoint++;
                    player.skillTree.increaseFireBurst();
                    player.skillTree.MidRange();
                    player.skillPoints--;
                }
                else if (Shield.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.ShieldPoint < 5)
                {
                    player.skillTree.ShieldPoint++;
                    player.skillTree.increaseShield();
                    player.skillTree.LongRange();
                    player.skillPoints--;
                }
                else if (AtkDmg.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.AtkDmgPoint < 5 && player.skillTree.firebreathPoint > 0)
                {
                    player.skillTree.AtkDmgPoint++;
                    player.skillTree.increaseAtkDmg();
                    player.skillTree.CloseRange();
                    player.skillPoints--;
                }
                else if (MoreAthyl.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.AthylPoint < 5 && player.skillTree.FireBurstPoint > 0)
                {
                    player.skillTree.AthylPoint++;
                    player.skillTree.increaseAthyl();
                    player.skillTree.MidRange();
                    player.skillPoints--;
                }
                else if (Kevlar.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.KevlarPoint < 5 && player.skillTree.ShieldPoint > 0)
                {
                    //player.skillTree.KevlarPoint++;
                    //player.skillTree.increaseArmor();
                    //player.skillTree.LongRange();
                    //player.skillPoints--;
                }
                else if (AtkSpd.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.AtkSpdPoint < 5 && player.skillTree.firebreathPoint > 0)
                {
                    player.skillTree.AtkSpdPoint++;
                    player.skillTree.increaseAtkSpd();
                    player.skillTree.CloseRange();
                    player.skillPoints--;
                }
                else if (Passtrough.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.PassthroughPoint < 5 && player.skillTree.FireBurstPoint > 0)
                {
                    //player.skillTree.PassthroughPoint++;
                    //player.skillTree.increasePasstrough();
                    //player.skillTree.MidRange();
                    //player.skillPoints--;
                }
                else if (Aim.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.AimPoint < 5 && player.skillTree.ShieldPoint > 0)
                {
                    //player.skillTree.AimPoint++;
                    //player.skillTree.increaseAim();
                    //player.skillTree.LongRange();
                    //player.skillPoints--;
                }
                else if (Dodge.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.DodgePoint < 5 && (player.skillTree.AtkDmgPoint == 5 || player.skillTree.AtkSpdPoint == 5))
                {
                    //player.skillTree.DodgePoint++;
                    //player.skillTree.increaseDodge();
                    //player.skillTree.CloseRange();
                    //player.skillPoints--;
                }
                else if (FastShot.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.FastShotPoint < 5 && (player.skillTree.AthylPoint == 5 || player.skillTree.PassthroughPoint == 5))
                {
                    player.skillTree.FastShotPoint++;
                    player.skillTree.increaseFastShot();
                    player.skillTree.MidRange();
                    player.skillPoints--;
                }
                else if (ShieldCD.rectangle.Contains(mouseState.X, mouseState.Y) && player.skillTree.ShieldCDPoint < 5 && (player.skillTree.KevlarPoint == 5 || player.skillTree.AimPoint == 5))
                {
                    player.skillTree.ShieldCDPoint++;
                    player.skillTree.increaseShieldCD();
                    player.skillTree.LongRange();
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
                start.toggleTextures = true;
            }
            else
            {
                start.toggleTextures = false;
            }
            if (control.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                control.toggleTextures = true;
            }
            else
            {
                control.toggleTextures = false;
            }
            if (story.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                story.toggleTextures = true;
            }
            else
            {
                story.toggleTextures = false;
            }
            if (exit.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                exit.toggleTextures = true;
            }
            else
            {
                exit.toggleTextures = false;
            }
            if (back.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                back.toggleTextures = true;
            }
            else
            {
                back.toggleTextures = false;
            }
            if (resume.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                resume.toggleTextures = true;
            }
            else
            {
                resume.toggleTextures = false;
            }
            if (restart.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                restart.toggleTextures = true;
            }
            else
            {
                restart.toggleTextures = false;
            }
            if (mainMenu.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                mainMenu.toggleTextures = true;
            }
            else
            {
                mainMenu.toggleTextures = false;
            }

            if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                MouseClicked(game, player);
            }

            IfMouseOverSkills();
        }

        public void IfMouseOverSkills()
        {
            fireBreathText.visible = false;
            AtkDmgText.visible = false;
            AtkSpdText.visible = false;
            DodgeText.visible = false;
            fireBurstText.visible = false;
            AthylText.visible = false;
            PasstroughText.visible = false;
            FastShotText.visible = false;
            ShieldText.visible = false;
            KelvarText.visible = false;
            AimText.visible = false;
            ShieldCDText.visible = false;

            if (FireBreath.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                fireBreathText.visible = true;
            }
            if (AtkDmg.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                AtkDmgText.visible = true;
            }
            if (AtkSpd.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                AtkSpdText.visible = true;
            }
            if (Dodge.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                DodgeText.visible = true;
            }

            if (Fireburst.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                fireBurstText.visible = true;
            }
            if (MoreAthyl.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                AthylText.visible = true;
            }
            if (Passtrough.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                PasstroughText.visible = true;
            }
            if (FastShot.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                FastShotText.visible = true;
            }

            if (Shield.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                ShieldText.visible = true;
            }
            if (Kevlar.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                KelvarText.visible = true;
            }
            if (Aim.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                AimText.visible = true;
            }
            if (ShieldCD.rectangle.Contains(mouseState.X, mouseState.Y))
            {
                ShieldCDText.visible = true;
            }
        }
        #endregion
        #region Draw
        /// <summary>
        /// Drawing the graphics for the menus for the different game states.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="game"></param>
        public void Draw(SpriteBatch spriteBatch, Game1 game, Player player)
        {
            start.Draw(spriteBatch, viewPortPos - new Vector2(100, 100), viewPortPos);
            control.Draw(spriteBatch, viewPortPos - new Vector2(100, 100), viewPortPos);
            story.Draw(spriteBatch, viewPortPos - new Vector2(100, 100), viewPortPos);
            exit.Draw(spriteBatch, viewPortPos - new Vector2(100, 100), viewPortPos);
            resume.Draw(spriteBatch, viewPortPos - new Vector2(100, 100), viewPortPos);
            restart.Draw(spriteBatch, viewPortPos - new Vector2(100, 100), viewPortPos);
            mainMenu.Draw(spriteBatch, viewPortPos - new Vector2(100, 100), viewPortPos);
            back.Draw(spriteBatch, viewPortPos - new Vector2(100, 100), viewPortPos);

            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(menuBackground, cameraPos, Color.White);
                spriteBatch.Draw(AthylLogo, pos1 - new Vector2(AthylLogo.Width/2, 0), Color.White);
                //spriteBatch.DrawString(myFont, "ATHYL\nLOGOTYP", pos0 - new Vector2(200, 0), Color.White, 0, Vector2.Zero, new Vector2(4, 4), SpriteEffects.None, 1);  //måste ändras
                if (paused)
                {
                    start.Draw(spriteBatch, pos5, viewPortPos);
                    control.Draw(spriteBatch, pos6, viewPortPos);
                    story.Draw(spriteBatch, pos7, viewPortPos);
                    exit.Draw(spriteBatch, pos8, viewPortPos);
                }
                else
                {
                    start.Draw(spriteBatch, pos5, viewPortPos);
                    control.Draw(spriteBatch, pos6, viewPortPos);
                    story.Draw(spriteBatch, pos7, viewPortPos);
                    exit.Draw(spriteBatch, pos8, viewPortPos);
                }
            }

            else if (gameState == GameState.Loading)
            {
                spriteBatch.Draw(menuBackground, cameraPos, Color.White);
                spriteBatch.Draw(loadingText, pos5 - new Vector2(loadingText.Width / 2, 0), Color.White);

                //Progressbar in loading menu
                Rectangle border = new Rectangle((int)(cameraPos.X + viewPortDim.X / 2 - 200), (int)(cameraPos.Y + viewPortDim.Y / 2), 400, 40);
                Rectangle bar = new Rectangle(border.X, border.Y, (int)((Map.progress / Map.done) * border.Width), border.Height);
                spriteBatch.Draw(progressBar, bar, Color.White);
                spriteBatch.Draw(progressBarBorder, border, Color.White);

                //The animation of the running woman in loading menu
                int FrameWidth = runningWoman.Width / 7;
                int FrameHeight = runningWoman.Height / 2;
                Rectangle sourcerect = new Rectangle(FrameWidth * colFrame, FrameHeight * 0,
                   FrameWidth, FrameHeight);
                spriteBatch.Draw(runningWoman, pos5 - new Vector2(loadingText.Width, runningWoman.Height / 4), sourcerect, Color.White,
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
                spriteBatch.Draw(pauseBackground, cameraPos, Color.White);
                spriteBatch.Draw(PauseText, pos2 - new Vector2(PauseText.Width / 2, 0), Color.White);
                mainMenu.Draw(spriteBatch, pos4, viewPortPos);
                resume.Draw(spriteBatch, pos5, viewPortPos);
                restart.Draw(spriteBatch, pos6, viewPortPos);
                control.Draw(spriteBatch, pos7, viewPortPos);
                story.Draw(spriteBatch, pos8, viewPortPos);
                exit.Draw(spriteBatch, pos9, viewPortPos);
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
                spriteBatch.Draw(gameOverText, pos2 - new Vector2(gameOverText.Width / 2, 0), Color.White);
                restart.Draw(spriteBatch, pos6, viewPortPos);
                mainMenu.Draw(spriteBatch, pos7, viewPortPos);

                //The dead woman
                int FrameWidth = deadWoman.Width / 3;
                int FrameHeight = deadWoman.Height / 2;
                Rectangle sourcerect = new Rectangle(FrameWidth * 2, FrameHeight * 0,
                   FrameWidth, FrameHeight);
                spriteBatch.Draw(deadWoman, pos4 - new Vector2(FrameWidth / 2, FrameHeight / 4), sourcerect, Color.White,
                    0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 1.0f);
            }

            else if (gameState == GameState.Skilltree)
            {
                spriteBatch.Draw(pauseBackground, cameraPos, Color.White);

                SkillInfoRect = new Rectangle(0, 0, (int)viewPortDim.X / 2, (int)viewPortDim.Y / 8);
                spriteBatch.Draw(pauseBackground, pos8 + new Vector2(-viewPortDim.X / 4, viewPortDim.Y / 7), SkillInfoRect, Color.DarkRed);

                fireBreathText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));
                AtkDmgText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));
                AtkSpdText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));
                DodgeText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));

                fireBurstText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));
                AthylText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));
                PasstroughText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));
                FastShotText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));

                ShieldText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));
                KelvarText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));
                AimText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));
                ShieldCDText.Draw(spriteBatch, pos8 + new Vector2(-viewPortDim.X / 5, viewPortDim.Y / 7));

                spriteBatch.Draw(SkilltreeGray, new Rectangle((int)cameraPos.X + 258, (int)cameraPos.Y + 208, (int)SkilltreeGray.Width, (int)SkilltreeGray.Height), Color.White);
                spriteBatch.Draw(SkilltreeGray, new Rectangle((int)cameraPos.X + 558, (int)cameraPos.Y + 208, (int)SkilltreeGray.Width, (int)SkilltreeGray.Height), Color.White);
                spriteBatch.Draw(SkilltreeGray, new Rectangle((int)cameraPos.X + 858, (int)cameraPos.Y + 208, (int)SkilltreeGray.Width, (int)SkilltreeGray.Height), Color.White);
                spriteBatch.Draw(SkilltreeCombat, new Rectangle((int)cameraPos.X + 258, (int)cameraPos.Y + 208, (int)SkilltreeCombat.Width, (int)(SkilltreeCombat.Height * 0.34) * CloseColorIncrease), Color.White);
                spriteBatch.Draw(SkilltreeMid, new Rectangle((int)cameraPos.X + 558, (int)cameraPos.Y + 208, (int)SkilltreeMid.Width, (int)(SkilltreeCombat.Height * 0.34) * MidColorIncrease), Color.White);
                spriteBatch.Draw(SkilltreeLong, new Rectangle((int)cameraPos.X + 858, (int)cameraPos.Y + 208, (int)SkilltreeLong.Width, (int)(SkilltreeCombat.Height * 0.34) * LongColorIncrease), Color.White);
                spriteBatch.Draw(SkilltreeBorder, new Rectangle((int)cameraPos.X + 250, (int)cameraPos.Y + 200, (int)SkilltreeBorder.Width, (int)SkilltreeBorder.Height), Color.White);
                spriteBatch.Draw(SkilltreeBorder, new Rectangle((int)cameraPos.X + 550, (int)cameraPos.Y + 200, (int)SkilltreeBorder.Width, (int)SkilltreeBorder.Height), Color.White);
                spriteBatch.Draw(SkilltreeBorder, new Rectangle((int)cameraPos.X + 850, (int)cameraPos.Y + 200, (int)SkilltreeBorder.Width, (int)SkilltreeBorder.Height), Color.White);
                spriteBatch.Draw(SkilltreePipes, new Rectangle((int)cameraPos.X + 290, (int)cameraPos.Y + 265, (int)SkilltreePipes.Width, (int)SkilltreePipes.Height), Color.White);
                spriteBatch.Draw(SkilltreePipes, new Rectangle((int)cameraPos.X + 590, (int)cameraPos.Y + 265, (int)SkilltreePipes.Width, (int)SkilltreePipes.Height), Color.White);
                spriteBatch.Draw(SkilltreePipes, new Rectangle((int)cameraPos.X + 890, (int)cameraPos.Y + 265, (int)SkilltreePipes.Width, (int)SkilltreePipes.Height), Color.White);

                spriteBatch.Draw(closeText, cameraPos + new Vector2(285, 185), Color.White);
                spriteBatch.Draw(middleText, cameraPos + new Vector2(575, 190), Color.White);
                spriteBatch.Draw(rangeText, cameraPos + new Vector2(885, 188), Color.White);

                FireBreath.Draw(spriteBatch, cameraPos + new Vector2(335, 250), viewPortPos);
                AtkDmg.Draw(spriteBatch, cameraPos + new Vector2(295, 350), viewPortPos);
                AtkSpd.Draw(spriteBatch, cameraPos + new Vector2(380, 350), viewPortPos);
                Dodge.Draw(spriteBatch, cameraPos + new Vector2(335, 450), viewPortPos);

                Fireburst.Draw(spriteBatch, cameraPos + new Vector2(635, 250), viewPortPos);
                MoreAthyl.Draw(spriteBatch, cameraPos + new Vector2(595, 350), viewPortPos);
                Passtrough.Draw(spriteBatch, cameraPos + new Vector2(680, 350), viewPortPos);
                FastShot.Draw(spriteBatch, cameraPos + new Vector2(635, 450), viewPortPos);

                Shield.Draw(spriteBatch, cameraPos + new Vector2(935, 250), viewPortPos);
                Kevlar.Draw(spriteBatch, cameraPos + new Vector2(895, 350), viewPortPos);
                Aim.Draw(spriteBatch, cameraPos + new Vector2(980, 350), viewPortPos);
                ShieldCD.Draw(spriteBatch, cameraPos + new Vector2(935, 450), viewPortPos);

                //Skriver ut antalet poäng tillgängliga
                spriteBatch.DrawString(myFont, "Points: " + player.skillPoints, pos1 - new Vector2(50, 0), Color.Gold);

                //Skriver ut antalet poäng lagda på skills i close trädet.
                spriteBatch.DrawString(myFont, "" + player.skillTree.firebreathPoint + "/5", cameraPos + new Vector2(320, 285), Color.Gold);
                spriteBatch.DrawString(myFont, "" + player.skillTree.AtkDmgPoint + "/5", cameraPos + new Vector2(280, 385), Color.Gold);
                spriteBatch.DrawString(myFont, "" + player.skillTree.AtkSpdPoint + "/5", cameraPos + new Vector2(365, 385), Color.Gold);
                spriteBatch.DrawString(myFont, "" + player.skillTree.DodgePoint + "/5", cameraPos + new Vector2(320, 485), Color.Gold);

                //Skriver ut antalet poäng lagda på skills i mid trädet.
                spriteBatch.DrawString(myFont, "" + player.skillTree.FireBurstPoint + "/5", cameraPos + new Vector2(620, 285), Color.Gold);
                spriteBatch.DrawString(myFont, "" + player.skillTree.AthylPoint + "/5", cameraPos + new Vector2(580, 385), Color.Gold);
                spriteBatch.DrawString(myFont, "" + player.skillTree.PassthroughPoint + "/5", cameraPos + new Vector2(665, 385), Color.Gold);
                spriteBatch.DrawString(myFont, "" + player.skillTree.FastShotPoint + "/5", cameraPos + new Vector2(620, 485), Color.Gold);

                //Skriver ut antalet poäng lagda på skills i long trädet.
                spriteBatch.DrawString(myFont, "" + player.skillTree.ShieldPoint + "/5", cameraPos + new Vector2(920, 285), Color.Gold);
                spriteBatch.DrawString(myFont, "" + player.skillTree.KevlarPoint + "/5", cameraPos + new Vector2(880, 385), Color.Gold);
                spriteBatch.DrawString(myFont, "" + player.skillTree.AimPoint + "/5", cameraPos + new Vector2(965, 385), Color.Gold);
                spriteBatch.DrawString(myFont, "" + player.skillTree.ShieldCDPoint + "/5", cameraPos + new Vector2(920, 485), Color.Gold);
            }
        }
    } 
        #endregion
}