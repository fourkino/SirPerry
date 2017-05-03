using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SirPerry_CCFRS
{
    class Actor
    {
        public Vector2 position;
        public Texture2D texture;
        public Rectangle rect;

        public Actor(Vector2 pos, Texture2D tex)
        {
            position = pos;
            texture = tex;
        }
        public Actor(Vector2 pos)
        {
            position = pos;
        }
    }
}
