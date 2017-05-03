using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class GUI
    {
        public Texture2D heartTexture;
        public Texture2D containerTexture;
        public Texture2D soldierTexture;
        public Texture2D dividerTexture;
        public Texture2D textBackground;
        public SpriteFont font;
        Vector2 textMiddlePoint;

        Vector2 containerPosition, heartOffset, dividerOffset, soldierOffset, fontOffset;
        int heartRows, heartCurrRow; // only one column
        Rectangle heartSourceRect; // no dest rect; drew to position in order to rescale

        Color textColor;
        Color noBlack;
        Color yesWhite;

        public GUI()
        {
            containerPosition = new Vector2(5, 5);
            heartOffset = new Vector2(containerPosition.X + 10,15);
            dividerOffset = new Vector2(heartOffset.X + 74, -5);
            soldierOffset = new Vector2(dividerOffset.X + 25, 10);
            fontOffset = new Vector2(soldierOffset.X + 57, 28);

            heartRows = 4;

            noBlack = Color.FromNonPremultiplied(new Vector4(0, 0, 0, 0));
            yesWhite = Color.FromNonPremultiplied(new Vector4(1, 1, 1, 1f));
            textColor = yesWhite;
        }

        public void Update(GameTime gameTime)
        {
            // draw row is decided by player health
            switch(Player.health)
            {
                case 3:
                    heartCurrRow = 0; 
                    break;
                case 2:
                    heartCurrRow = 1; 
                    break;
                case 1:
                    heartCurrRow = 2; 
                    break;
                default:
                    heartCurrRow = 3; 
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            heartSourceRect = new Rectangle(0, (heartTexture.Height / heartRows) * heartCurrRow, heartTexture.Width, heartTexture.Height / heartRows );

            textMiddlePoint = font.MeasureString(Formation.soldiers.Count.ToString()) / 2;

            spriteBatch.Draw(containerTexture, containerPosition,
                scale: new Vector2(2.05f, .5f), color: Color.White, layerDepth: .3f);

            spriteBatch.Draw(heartTexture, containerPosition + heartOffset, null, heartSourceRect,
                scale: new Vector2(.15f, .15f), color: Color.White, layerDepth: 0f);

            spriteBatch.Draw(dividerTexture, heartOffset + dividerOffset,
                scale: new Vector2(.15f, .3f), color: Color.White , layerDepth: .1f);

            spriteBatch.Draw(soldierTexture, containerPosition + soldierOffset,
                scale: new Vector2(.5f, .5f), color: Color.White);

            spriteBatch.Draw(textBackground, containerPosition + new Vector2(soldierOffset.X + 37, 12.5f),
                scale: new Vector2(1.8f, .2f), color: Color.White, layerDepth: .2f);

            spriteBatch.DrawString(font, Formation.soldiers.Count.ToString(), containerPosition + fontOffset,
                scale: new Vector2(1.5f,1.5f), color: textColor, rotation: 0f, origin: textMiddlePoint, effects: SpriteEffects.None, layerDepth: .1f);
        }
    }
}
