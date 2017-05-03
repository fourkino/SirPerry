using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class TitleScreen : Screen
    {
        Button startButton;
        ScreenText titleText;
        public static Texture2D texture, textTexture;

        public TitleScreen()
        {
            Type = "TitleScreen";

            titleText = new ScreenText(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)), textTexture);
            titleText.position.Y -= titleText.texture.Height / 5;

            startButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Press LMB to continue",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            startButton.rect.X -= startButton.rect.Width / 2;
            startButton.rect.Y += startButton.rect.Height * 3;
        }

        public override void Update(GameTime gameTime)
        {
            Type = "MainMenuScreen";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //background
            spriteBatch.Draw(texture,
                destinationRectangle: new Rectangle(0, 0, GameManager.screenWidth, GameManager.screenHeight),
                scale: new Vector2(1, 1), rotation: 0f, origin: Vector2.Zero,
                effects: SpriteEffects.None,
                color: Color.White,
                layerDepth: .12f); // right behind buttons
            //text
            spriteBatch.Draw(textTexture,
                position: titleText.position,
                scale: new Vector2(.85f, .85f), rotation: 0f,
                origin: new Vector2(titleText.texture.Width/2, titleText.texture.Height/2),
                effects: SpriteEffects.None,
                color: Color.White,
                layerDepth: 0); 
            startButton.Draw(spriteBatch);
        }
    }
}
