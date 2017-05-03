using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class Obstacle : Actor
    {

        public Texture2D texture;
        public float layerDepth;
        public Rectangle rect;
        public static int texWidth;

        public Obstacle(Vector2 pos, Texture2D tex)
            : base(pos)
        {
            texture = tex;
        }

        public virtual void Update(GameTime gameTime)
        {
            position.Y -= GameManager.worldScrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //layerDepth = .9f + (0f - .9f) * ((position.Y - 0) / (GameManager.screenHeight - 0));
            // update collision rect without affecting width or height
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, layerDepth: layerDepth);
        }

        public virtual void checkCollision(Player player, List<Soldier> soldiers)
        {
        }
    }
}
