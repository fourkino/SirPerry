using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SirPerry_CCFRS
{
    class SpriteManager
    {
        public Texture2D texture;
        public int rows, columns;
        int currRow, currCol;
        int currentFrame, totalFrames;
        int width, height;
        Rectangle sourceRect, destRect;
        float totalElapsed, timePerFrame;
        float layerDepth;
        float playerPosY;

        public SpriteManager(Texture2D t, int r, int c)
        {
            texture = t;
            rows = r;
            columns = c;
            currentFrame = 0;
            totalFrames = rows * columns;
            timePerFrame = 1 / 5f; // 5 fps
            width = texture.Width / columns;
            height = texture.Height / rows;
        }

        public void Update(GameTime gameTime)
        {
            

            totalElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (totalElapsed >= timePerFrame)
            {
                currentFrame++;
                if (currentFrame >= totalFrames)
                {
                    currentFrame = 0;
                }
                totalElapsed -= timePerFrame;
            }
            layerDepth = 0f;
            currRow = (int)((float)currentFrame / (float)columns);
        }

        // apply player sprite direction based on input
        public void applyDirection(float elapsed, int dir) // give elapsed time to determine update rate
        {
            switch(dir)
            {
                case 1: // down
                    currRow = 0;
                    break;
                case 2: // left
                    currRow = 1;
                    break;
                case 3: // right
                    currRow = 2;
                    break;
                case 4: // up
                    currRow = 3;
                    break;
                default: // default is down
                    currRow = 0;
                    break;
            }
            totalElapsed += elapsed;
            if (totalElapsed > timePerFrame)
            {
                currentFrame++;
                if (currentFrame == 3)
                {
                    currentFrame = 0;
                }
                totalElapsed -= timePerFrame;
            }
            layerDepth = (.9f + (0f - .9f) * (playerPosY - 0) / (GameManager.screenHeight - 0));
        }

        public void Draw(SpriteBatch spriteBatch, Actor actor)
        {
            //currRow = (int)((float)currentFrame/(float)columns); // decide current row in update, depending on player input
            currCol = currentFrame % columns;

            sourceRect = new Rectangle(width * currCol, height * currRow, width, height);
            destRect = new Rectangle((int)actor.position.X, (int)actor.position.Y, width, height);

            spriteBatch.Draw(texture, null, destRect, sourceRect,
                origin: new Vector2(actor.texture.Width / columns / 2, actor.texture.Height / rows / 2),
                scale: new Vector2(.5f, .5f),
                layerDepth: layerDepth);

            playerPosY = actor.position.Y;
        }
    }
}
