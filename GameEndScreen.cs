using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class GameEndScreen : Screen
    {
        public static Texture2D textureDefeat, textureVictory, textureGameOverText, textureDefeatText,
            textureVictoryText;
        Button continueButton, restartLevelButton, quitToMainButton, exitGameButton;
        ScreenText gameEndText, gameEndScenarioText;
        Scoreboard scoreboard;
        string scenario;

        // defeat constructor
        public GameEndScreen(string scen)
        {
            Type = "GameEndScreen";
            scenario = scen;

            gameEndText = new ScreenText(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)), textureGameOverText);
            gameEndText.position.Y -= gameEndText.texture.Height*2.5f;

            gameEndScenarioText = new ScreenText(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)), textureDefeatText);
            gameEndScenarioText.position.Y = gameEndText.position.Y + (gameEndScenarioText.texture.Height*1.2f);

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
        // victory constructor
        public GameEndScreen(string scen, int numSoldiersSurvived)
        {
            Type = "GameEndScreen";
            scenario = scen;

            gameEndText = new ScreenText(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)), textureGameOverText);
            gameEndText.position.Y -= gameEndText.texture.Height*2.5f;

            gameEndScenarioText = new ScreenText(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)), textureVictoryText);
            gameEndScenarioText.position.Y = gameEndText.position.Y + (gameEndScenarioText.texture.Height * 1.2f);

            scoreboard = new Scoreboard(new Vector2((GameManager.screenWidth / 2),
                        (GameManager.screenHeight / 2)),
                        numSoldiersSurvived, 
                        10, // total soldiers possible to save from beginning to end of game
                        Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f))); // not as transparent
            scoreboard.rect.X -= (int)(scoreboard.rect.Width / 2);
            scoreboard.rect.Y -= scoreboard.rect.Height / 5;

            continueButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Continue to Main Menu",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            continueButton.rect.X -= continueButton.rect.Width / 2;
            continueButton.rect.Y += (int)(continueButton.rect.Height * 4f);

            exitGameButton = new Button(new Vector2((GameManager.screenWidth / 2),
                (GameManager.screenHeight / 2)),
                "Exit Game",
                Color.Black, new Color(new Vector4(.8f, .8f, .8f, .9f)));
            exitGameButton.rect.X -= exitGameButton.rect.Width / 2;
            exitGameButton.rect.Y = (int)(continueButton.rect.Y + exitGameButton.rect.Height * 1.2f);
            
        }

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            switch(scenario)
            { 
                case "PlayerDead":
                case "SoldiersDead":
                    if (restartLevelButton.clicked(mouseState))
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
                    break;
                case "Victory":
                    if (continueButton.clicked(mouseState))
                    {
                        Type = "MainMenuScreen";
                    }
                    else if (exitGameButton.clicked(mouseState))
                    {
                        Type = "ExitGame";
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch(scenario)
            { 
                case "PlayerDead":
                case "SoldiersDead":
                    //background
                    spriteBatch.Draw(textureDefeat, // defeat screen
                        destinationRectangle: new Rectangle(0, 0, GameManager.screenWidth, GameManager.screenHeight),
                        scale: new Vector2(1, 1), rotation: 0f, origin: Vector2.Zero,
                        effects: SpriteEffects.None,
                        color: Color.White,
                        layerDepth: .12f); // right behind buttons
                    //text
                    spriteBatch.Draw(textureGameOverText,
                        position: gameEndText.position,
                        scale: new Vector2(1, 1), rotation: 0f,
                        origin: new Vector2(gameEndText.texture.Width / 2, gameEndText.texture.Height / 2),
                        effects: SpriteEffects.None,
                        color: Color.White,
                        layerDepth: 0); 
                    //text
                    spriteBatch.Draw(textureDefeatText,
                        position: gameEndScenarioText.position,
                        scale: new Vector2(1, 1), rotation: 0f,
                        origin: new Vector2(gameEndScenarioText.texture.Width / 2, gameEndScenarioText.texture.Height / 2),
                        effects: SpriteEffects.None,
                        color: Color.White,
                        layerDepth: 0); 
                    restartLevelButton.Draw(spriteBatch);
                    quitToMainButton.Draw(spriteBatch);
                    exitGameButton.Draw(spriteBatch);
                    break;
                case "Victory":
                    //background
                    spriteBatch.Draw(textureVictory, // victory screen
                        destinationRectangle: new Rectangle(0, 0, GameManager.screenWidth, GameManager.screenHeight),
                        scale: new Vector2(1.28f, 1.28f), // scaled by 1.28
                        rotation: 0f, origin: Vector2.Zero,
                        effects: SpriteEffects.None,
                        color: Color.White,
                        layerDepth: .12f); // right behind buttons
                    //text
                    spriteBatch.Draw(textureGameOverText,
                        position: gameEndText.position,
                        scale: new Vector2(1, 1), rotation: 0f,
                        origin: new Vector2(gameEndText.texture.Width / 2, gameEndText.texture.Height / 2),
                        effects: SpriteEffects.None,
                        color: Color.White,
                        layerDepth: 0); 
                    //text
                    spriteBatch.Draw(textureVictoryText,
                        position: gameEndScenarioText.position,
                        scale: new Vector2(1, 1), rotation: 0f,
                        origin: new Vector2(gameEndScenarioText.texture.Width / 2, gameEndScenarioText.texture.Height / 2),
                        effects: SpriteEffects.None,
                        color: Color.White,
                        layerDepth: 0); 
                    scoreboard.Draw(spriteBatch);
                    continueButton.Draw(spriteBatch);
                    exitGameButton.Draw(spriteBatch);
                    break;
            }
        }
    }
}
