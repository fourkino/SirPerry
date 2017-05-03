using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    abstract public class Screen
    {

        public string Type;

        public virtual void Update(GameTime gameTime) { }
        public virtual void Update(GameTime gameTime, MouseState mouseState) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
