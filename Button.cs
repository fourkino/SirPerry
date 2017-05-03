using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class Button
    {
        //public bool clicked = false;
        public string Text { get; set; }
        public static SpriteFont font;
        Vector2 textMiddlePoint;

        public Color TextColor { get; set; }
        public Color BackgroundColor { get; set; }
        public static Texture2D texture;

        public Rectangle rect;
        public Vector2 Position { get; set; }

        public Button(Vector2 pos, string text, Color tcolor, Color bgcolor)
        {
            Text = text; // set text of button
            Position = pos;
            rect = new Rectangle((int)Position.X, (int)Position.Y, 275, 50);
            TextColor = tcolor;
            BackgroundColor = bgcolor;
        }

        public bool clicked(MouseState mouseState)
        {
            // if mouse click is inside rect, clicked = true
            return rect.Contains(new Vector2(mouseState.X, mouseState.Y));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            textMiddlePoint = font.MeasureString(Text)/2;
            spriteBatch.Draw(texture, destinationRectangle: rect, scale: new Vector2(1f, 1),
                color: BackgroundColor, layerDepth: .1f);

            spriteBatch.DrawString(spriteFont: font, text: Text,
                position: new Vector2(rect.Center.X, rect.Center.Y),
                scale: new Vector2(1.5f,1.5f), color: TextColor, rotation: 0f,
                origin: textMiddlePoint, effects: SpriteEffects.None, layerDepth: 0f);
        }
    }
}
