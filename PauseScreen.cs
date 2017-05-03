using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class PauseScreen : Screen
    {
        Button resumeButton, restartLevelButton, quitToMainButton, exitGameButton;
        ScreenText pauseText;
        public static Texture2D backgroundTexture, textTexture;

        public PauseScreen()
        {
            Type = "PauseScreen";

            pauseText = new ScreenText(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)), textTexture);
            pauseText.position.Y -= pauseText.texture.Height*2;

            resumeButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Resume Game",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            resumeButton.rect.X -= resumeButton.rect.Width / 2;
            resumeButton.rect.Y -= (int)(resumeButton.rect.Height * 1.2f);

            restartLevelButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Retry Level",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            restartLevelButton.rect.X -= restartLevelButton.rect.Width / 2;

            quitToMainButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Quit to Main Menu",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            quitToMainButton.rect.X -= quitToMainButton.rect.Width / 2;
            quitToMainButton.rect.Y += (int)(quitToMainButton.rect.Height * 1.2f);

            exitGameButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Exit Game",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            exitGameButton.rect.X -= exitGameButton.rect.Width / 2;
            exitGameButton.rect.Y += (int)(exitGameButton.rect.Height * 2.4f);
        }

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            if (resumeButton.clicked(mouseState))
            {
                GameManager.soundManager.menu_pauseOffInstance.Play();
                Type = "GameScreen";
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.P) || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Type = "GameScreen";
            }
            else if (restartLevelButton.clicked(mouseState))
            {
                Type = "RestartLevel";
            }
            else if (quitToMainButton.clicked(mouseState))
            {
                Type = "MainMenuScreen";
            }
            else if (exitGameButton.clicked(mouseState))
            {
                Type = "ExitGame";
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw half-transparent rect for background.
            spriteBatch.Draw(backgroundTexture,
                destinationRectangle: new Rectangle(0,0,GameManager.screenWidth,GameManager.screenHeight),
                scale: new Vector2(1,1), rotation: 0f, origin: Vector2.Zero,
                effects: SpriteEffects.None,
                color: new Color(new Vector4(0, 0, 0, .5f)),
                layerDepth: .12f); // right behind buttons
            //text
            spriteBatch.Draw(textTexture,
                position: pauseText.position,
                scale: new Vector2(1, 1), rotation: 0f,
                origin: new Vector2(pauseText.texture.Width / 2, pauseText.texture.Height / 2),
                effects: SpriteEffects.None,
                color: Color.White,
                layerDepth: 0); 
            resumeButton.Draw(spriteBatch);
            restartLevelButton.Draw(spriteBatch);
            quitToMainButton.Draw(spriteBatch);
            exitGameButton.Draw(spriteBatch);
        }
    }
}
