using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SirPerry_CCFRS
{
    class BackgroundManager
    {
        List<Background> backgrounds;
        int screenWidth, screenHeight;
        float scrollSpeed;

        public BackgroundManager(List<Background> bgs, int screenW, int screenH, float wsSpeed)
        {
            backgrounds = bgs;
            screenWidth = screenW;
            screenHeight = screenH;
            scrollSpeed = wsSpeed;
        }

        // update checks for bg position relative to screen and moves world continuously up
        public void Update(GameTime gameTime)
        {
            foreach(Background bg in backgrounds)
            {
                bg.position.Y -= scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; //scroll upwards

                // check if bg needs repositioning
                if(bg.position.Y + Background.height < 0)
                {
                    bg.position.Y += (Background.height * 2);
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Background bg in backgrounds)
            {
                spriteBatch.Draw(Background.texture, bg.position, layerDepth: 1f);
            }
        }
        public void setScrollSpeed(float speed)
        {
            scrollSpeed = speed;
        }
    }
}
