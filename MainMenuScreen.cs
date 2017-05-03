using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class MainMenuScreen : Screen
    {
        Button startButton, quitButton;
        ScreenText mainMenuText;
        public static Texture2D texture, textTexture;

        public MainMenuScreen()
        {
            Type = "MainMenuScreen";

            mainMenuText = new ScreenText(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)), textTexture);
            mainMenuText.position.Y -= mainMenuText.texture.Height;

            startButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Start Game",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            startButton.rect.X -= startButton.rect.Width / 2;
            startButton.rect.Y += startButton.rect.Height;

            quitButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Exit Game",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            quitButton.rect.X -= startButton.rect.Width / 2;
            quitButton.rect.Y = (int)(startButton.rect.Y + (startButton.rect.Height * 1.2f));
        }

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            if(startButton.clicked(mouseState))
            {
                Type = "GameScreen";
            }
            if(quitButton.clicked(mouseState))
            {
                Type = "ExitGame";
            }
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
                position: mainMenuText.position,
                scale: new Vector2(1, 1), rotation: 0f,
                origin: new Vector2(mainMenuText.texture.Width / 2, mainMenuText.texture.Height / 2),
                effects: SpriteEffects.None,
                color: Color.White,
                layerDepth: 0); 
            startButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }
    }
}
