using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class Barricade : Obstacle
    {
        public static Texture2D texture;

        public Barricade(Vector2 pos, Texture2D tex)
            : base(pos, tex)
        {
            rect = new Rectangle((int)pos.X, (int)pos.Y + 20, tex.Width, tex.Height/3);
            texWidth = tex.Width;
        }

        public override void Update(GameTime gameTime)
        {
            position.Y -= GameManager.worldScrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            // update collision rect without affecting width or height
            rect.X = (int)position.X;
            rect.Y = (int)position.Y + 20;
            layerDepth = .9f + (0f - .9f) * ((position.Y - 0) / (GameManager.screenHeight - 0));
        }
    }
}
