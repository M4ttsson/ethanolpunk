using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Athyl
{
    class AnimatedSprite
    {

        Texture2D spriteTexture;
        Rectangle sourceRectangle;
        Vector2 position;
        Vector2 origin;

        float timer = 0f;
        float interval = 200f;
        int currentFrame = 0;
        int spriteWidth = 80;
        int spriteHeight = 160;
        int spriteSpeed = 2;


        public Vector2 getSet_position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 getSet_origin
        {
            get { return origin; }
            set { origin = value; }
        }
        public Texture2D getSet_spriteTexture
        {
            get { return spriteTexture; }
            set { spriteTexture = value; }
        }
        public Rectangle getSet_sourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }


        public AnimatedSprite(Texture2D texture, int currentFrame, int spriteWidth, int spriteHeight)
        {
            this.spriteTexture = texture;
            this.currentFrame = currentFrame;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
        }

        KeyboardState currentKBState;
        KeyboardState previousKBState;

        public void handleSpriteMovement(GameTime gameTime)
        {
            previousKBState = currentKBState;
            currentKBState = Keyboard.GetState();

            sourceRectangle = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);

            if (currentKBState.GetPressedKeys().Length == 0)
            {
                if (currentFrame > 1 && currentFrame < 10)
                {
                    currentFrame = 0;
                }

            }
        }

        public void AnimateLeft(GameTime gameTime)
        {
            if (currentKBState != previousKBState)
            {
                currentFrame = 11;
            }

            timer += (float)gameTime.ElapsedGameTime.Milliseconds;

            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame > 10)
                {
                    currentFrame = 0;
                }
                timer = 0f;
            }
        }

        public void AnimateRight(GameTime gameTime)
        {
            if (currentKBState != previousKBState)
            {
                currentFrame = 12;
            }

            timer += (float)gameTime.ElapsedGameTime.Milliseconds;

            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame > 22)
                {
                    currentFrame = 13;
                }
                timer = 0f;
            }
        }




    }
}
