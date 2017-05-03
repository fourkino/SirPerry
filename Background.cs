using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SirPerry_CCFRS
{
    class Background
    {
        public Vector2 position;
        public static Texture2D texture;
        public static int height;

        public Background(Vector2 pos)
        {
            position = pos;
        }
    }
}
