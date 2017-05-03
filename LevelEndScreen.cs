using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class LevelEndScreen : Screen
    {
        Button continueButton, restartLevelButton, quitToMainButton, exitGameButton;
        Scoreboard scoreboard;
        ScreenText levelEndText;
        public static Texture2D texture, textureLevelEndText;

        public LevelEndScreen(int numSoldiersSurvived, int numSoldiersTotal)
        {
            Type = "LevelEndScreen";

            levelEndText = new ScreenText(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)), textureLevelEndText);
            levelEndText.position.Y -= levelEndText.texture.Height*3;

            scoreboard = new Scoreboard(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                numSoldiersSurvived, // num soldiers that survived this level... 
                numSoldiersTotal, // out of total soldiers entered level with
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f))); // not as transparent
            scoreboard.rect.X -= (int)(scoreboard.rect.Width /2);
            scoreboard.rect.Y -= (int)(scoreboard.rect.Height *.9f);

            continueButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Continue",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            continueButton.rect.X -= continueButton.rect.Width / 2;
            continueButton.rect.Y += (int)(continueButton.rect.Height * 1.3f);

            restartLevelButton = new Button(new Vector2((GameManager.screenWidth / 2),
                    (GameManager.screenHeight / 2)),
                    "Retry Level",
                    Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            restartLevelButton.rect.X -= restartLevelButton.rect.Width / 2;
            restartLevelButton.rect.Y = (int)(continueButton.rect.Y + (restartLevelButton.rect.Height * 1.2f));

            quitToMainButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Quit to Main Menu",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            quitToMainButton.rect.X -= quitToMainButton.rect.Width / 2;
            quitToMainButton.rect.Y = (int)(restartLevelButton.rect.Y + quitToMainButton.rect.Height * 1.2f);

            exitGameButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Exit Game",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            exitGameButton.rect.X -= exitGameButton.rect.Width / 2;
            exitGameButton.rect.Y = (int)(quitToMainButton.rect.Y + exitGameButton.rect.Height*1.2f);
        }

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            if (continueButton.clicked(mouseState))
            {
                Type = "NextLevel";
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
            //background
            spriteBatch.Draw(texture,
                destinationRectangle: new Rectangle(0, 0, GameManager.screenWidth, GameManager.screenHeight),
                scale: new Vector2(1, 1), rotation: 0f, origin: Vector2.Zero,
                effects: SpriteEffects.None,
                color: Color.White,
                layerDepth: .12f); // right behind buttons
            //text
            spriteBatch.Draw(textureLevelEndText,
                position: levelEndText.position,
                scale: new Vector2(1, 1), rotation: 0f,
                origin: new Vector2(levelEndText.texture.Width / 2, levelEndText.texture.Height / 2),
                effects: SpriteEffects.None,
                color: Color.White,
                layerDepth: 0); 
            scoreboard.Draw(spriteBatch);
            continueButton.Draw(spriteBatch);
            restartLevelButton.Draw(spriteBatch);
            quitToMainButton.Draw(spriteBatch);
            exitGameButton.Draw(spriteBatch);
        }
    }
}
