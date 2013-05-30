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

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace Athyl
{
    class Sounds
    {
        #region Properties
        private Game1 game;
        private Song song;
        private SoundEffect soundFX;
        private bool isPaused = false;
        #endregion
        #region Constructor
        public Sounds(Game1 game)
        {
            this.game = game;


        }
        #endregion
        #region Play
        public void PlaySoundFX(string sound)
        {
            SoundEffect.MasterVolume = 0.1f;
            soundFX = game.Content.Load<SoundEffect>(sound);
            soundFX.Play();
        }
        public void Play(string sound)
        {
            song = game.Content.Load<Song>(sound);
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.02f;
        }
        #endregion
        #region MuteAndVolume
        public void MuteSound()
        {
            if (SoundEffect.MasterVolume > 0)
            {
                SoundEffect.MasterVolume = 0;

            }

            else if (SoundEffect.MasterVolume == 0)
            {
                SoundEffect.MasterVolume = 0.1f;
            }


        }

        public void MuteMusic()
        {
            MediaPlayer.Volume = 0.0f;
        }

        public void adjustVolume()
        {
            KeyboardState kbState = Keyboard.GetState();


            if (kbState.IsKeyDown(Keys.I))
            {
                MediaPlayer.Volume -= 0.01f;
            }

            else if (kbState.IsKeyDown(Keys.O))
            {
                MediaPlayer.Volume += 0.01f;
            }

            if (kbState.IsKeyDown(Keys.L) && isPaused == false)
            {
                MediaPlayer.Pause();
                isPaused = true;
            }


            else if (kbState.IsKeyDown(Keys.L) && isPaused == true)
            {
                MediaPlayer.Resume();
                isPaused = false;
            }

        }
        #endregion
        #region Update
        public void UpdateSound(GameTime gameTime)
        {
            adjustVolume();
        }
        #endregion
    }
}
