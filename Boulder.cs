using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class Boulder : Obstacle
    {
        public static Texture2D texture, explosionTexture, shadowTexture;
        float fallSpeed, fallSpeedDefault;
        float fallTimer, fallTimeMax, fallTimeMaxDefault;
        public bool hitGround, doneExploding;
        Vector2 shadowPosition, shadowScale;
        int currentFrame, totalFrames;
        float totalElapsed, timePerFrame;
        float collisionTimer, collisionTimeMax;
        bool smashed;

        public Boulder(Vector2 pos, Texture2D tex)
            : base(pos, tex)
        {
            // fallspeed and fallTimeMax change with difficulty
            // fallspeed easy: 400, med: 600, hard: 2x default?
            // fallTimeMax easy: 2 seconds, med: 1.5, hard: 1
            fallSpeedDefault = 400f;
            fallTimeMaxDefault = 2f;
            switch(GameManager.difficulty)
            {
                case Difficulty.Easy:
                    fallSpeed = fallSpeedDefault;
                    fallTimeMax = fallTimeMaxDefault;
                    break;
                case Difficulty.Medium:
                    fallSpeed = fallSpeedDefault * 1.5f;
                    fallTimeMax = fallTimeMaxDefault * .75f;
                    break;
                case Difficulty.Hard:
                    fallSpeed = fallSpeedDefault * 2f;
                    fallTimeMax = fallTimeMaxDefault * .5f;
                    break;
            }
            fallTimer = 0;
            hitGround = false;
            doneExploding = false;
            currentFrame = 0;
            totalFrames = 2 * 8; // rows * columns
            totalElapsed = 0f;
            timePerFrame = 1 / 10f; // 10 fps
            shadowScale = Vector2.Zero;
            collisionTimer = 0;
            collisionTimeMax = .2f; // .2 seconds
            smashed = false;

            shadowPosition = new Vector2(pos.X, pos.Y);
            //+ (fallSpeed * fallTimeMax)
            position = new Vector2(pos.X, (shadowPosition.Y - shadowTexture.Height / 2) - (fallSpeed * fallTimeMax));
            rect = new Rectangle((int)shadowPosition.X, (int)shadowPosition.Y,
                shadowTexture.Width, shadowTexture.Height); // size of shadow
            texWidth = tex.Width;
        }
        public void fall(GameTime gameTime)
        {
            position.Y += fallSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameManager.soundManager.rock_fallInstance.Play();
        }
        public override void Update(GameTime gameTime)
        {
            position.Y -= GameManager.worldScrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            shadowPosition.Y -= GameManager.worldScrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(!hitGround)
                fall(gameTime);
            // update collision rect without affecting width or height
            rect.X = (int)position.X - shadowTexture.Width/2;
            rect.Y = (int)position.Y;
            // map(value, istart, istop, ostart, ostop)
            // { return ostart + (ostop - ostart) * ((value - istart) / (istop -istart)); }
            layerDepth = .9f + (0f - .9f) * ((position.Y - 0) / (GameManager.screenHeight - 0));
            shadowScale.X = 0f + (1f - 0f) * ((fallTimer - 0f) / (fallTimeMax - 0f));
            shadowScale.Y = shadowScale.X;

            // timer till collision
            fallTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(fallTimer >= fallTimeMax)
            {
                // collide with ground
                hitGround = true;
                fallTimer = 0;
                if(!smashed) GameManager.soundManager.rock_smashInstance.Play();
                smashed = true;
            }
            if (hitGround)
            {
                totalElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (totalElapsed >= timePerFrame)
                {
                    currentFrame++;
                    if (currentFrame >= totalFrames)
                    {
                        doneExploding = true;
                    }
                    totalElapsed -= timePerFrame;
                }
                collisionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!hitGround)
            {
                spriteBatch.Draw(texture, position,
                    origin: new Vector2(texture.Width / 2, texture.Height/2), layerDepth: .1f);
                spriteBatch.Draw(shadowTexture, shadowPosition,
                    origin: new Vector2(shadowTexture.Width / 2, shadowTexture.Height/2),
                    scale: shadowScale,
                    layerDepth: .11f);
            }
            if(hitGround)
            {
                spriteBatch.Draw(explosionTexture, null,
                    sourceRectangle: new Rectangle((explosionTexture.Width / 8) * (currentFrame % 8), ((explosionTexture.Height / 2) * ((int)((float)currentFrame / (float)8))),
                        explosionTexture.Width / 8, explosionTexture.Height / 2),
                    destinationRectangle: new Rectangle((int)position.X + texture.Width / 2, (int)position.Y + texture.Height / 2,
                        texture.Width, texture.Height),
                    origin: new Vector2(explosionTexture.Width/8, explosionTexture.Height/2),
                    layerDepth: .1f);

                //spriteBatch.Draw(texture, rect, Color.White); // draw collision rect
            }
        }
        public override void checkCollision(Player player, List<Soldier> soldiers)
        {
            if (hitGround && collisionTimer < collisionTimeMax)
            {
                if (rect.Contains(player.position + new Vector2(0, 30))) // corrected for height of player sprite (evaluates at feet, so player can walk along bottom edge)
                {
                    player.takeDamage();
                }
                foreach (Soldier s in soldiers)
                {
                    if (rect.Contains(s.position + new Vector2(0, 30)))
                    {
                        s.dead = true;
                        GameManager.soundManager.getSoldierDeathInstance().Play();
                    }
                }
            }
        }
    }
}
