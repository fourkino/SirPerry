using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Audio;

namespace SirPerry_CCFRS
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    public class GameManager : Game
    {
        #region Var Declarations
        // Managers
        GraphicsDeviceManager graphics;
        BackgroundManager bgManager;
        ObstacleManager obstacleManager;
        SpriteManager playerSM;
        SpriteManager flagSM;
        public static SoundManager soundManager = new SoundManager();

        // Classes
        SpriteBatch spriteBatch;
        KeyboardState keyState;
        KeyboardState previousKeyState;
        MouseState mouseState;
        MouseState previousMouseState;
        Formation formation;
        Player player;
        Background background1, background2;
        GUI gui;
        Screen currentScreen;
        Flag flag;
        FinishLine finishLine;

        // Vars - grouped by type
        public static int screenHeight, screenWidth;
        //int numSoldiersSurvived = 10; // amount survived each level...
        //int soldierLevelTotal = 10; // out of total that entered the level
        int numSoldiersEntered, numSoldiersExited = 10;

        public const float defaultWorldScrollSpeed = 80f; // units per second
        public static float worldScrollSpeed;
        float delta;
        float finishLineTimeMax, finishLineTimer, levelClearTimeMax, levelClearTimer, gameOverTimeMax, gameOverTimer;
        float announcerWaitTimer, announcerWaitTimeMax;
        float preStartTimer, preStartTimeMax;
        Vector4 readyTextAlpha, goTextAlpha, deltaVector;

        bool titleScreenShowed = false; // set to true indefinitely when shown on launch
        bool restarted = false;
        bool retried = false;
        bool playerIsScary;
        public static bool inPlay;
        bool levelFinishSpawned, levelCleared;
        bool flagActive;
        bool fromScreen = true;
        bool announcerWaitOver = false;
        bool soundLoaded = false;
        bool inPreStartPhase = false;
        bool readyTextActive, goTextActive;
        
        string currentLevelBG = "sprites/ground_field"; // start on field, but allow var to update when needed
        string currentLevelPF = "sprites/pitfall";

        Texture2D flagTexture;
        public Texture2D playerTex, soldierTex;
        Texture2D readyText, goText;
        
        List<Background> bgList;
        
        public static Difficulty difficulty;
        #endregion

        public GameManager()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width / 2;
            //graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height / 2;
            graphics.PreferredBackBufferWidth = 1024; // 1280 x 720 is original, test background is 1024 x 768
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;
            
            // position window correctly
            this.Window.Position = new Point(100, 100);

            // fixed time step is ON, for fixed fps
            //TargetElapsedTime = TimeSpan.FromTicks((long)10000000/((long)(60)));
        }

        protected override void Initialize()
        {
            #region Soldier behavior weight info
            /* SOLDIER WEIGHTS
             * higher = more influence on behavior.
             * separation: 1.0 is good for maintaining personal space.
             * alignment: higher than cohesion for more movement "forward".
             * cohesion: higher than alignment for slower, tighter group.
             * 
             * TESTING: 
             * align @ 1.5 and coh @ 1.3 for reasonable paced, but loosely packed group.
             * align @ 1 and coh @ 1.2 or 1.3 for tighter, slow group. (closer to game idea needs)
             * playerFear close to or matching cohesion to split up group, or cohesion higher for a group that won't split
             */
            #endregion

            Soldier.separationWeight = .9f;
            Soldier.alignmentWeight = 1.0f;
            Soldier.cohesionWeight = 1.1f;
            Soldier.playerFearFactor = 1.3f;

            playerIsScary = true;
            worldScrollSpeed = defaultWorldScrollSpeed;
            inPlay = false;
            finishLineTimeMax = 10f; // CONTROLS LEVEL LENGTH (in seconds)
            finishLineTimer = 0;
            levelClearTimeMax = 1f; // Time between finish line cross and screen transition (in seconds)
            levelClearTimer = 0;
            gameOverTimeMax = 1.5f; // Time between death states and screen transition (in seconds)
            gameOverTimer = 0;
            levelFinishSpawned = false;
            levelCleared = false;
            flagActive = false;
            announcerWaitTimeMax = 1.5f;
            announcerWaitTimer = 0;
            preStartTimeMax = 4.1f;
            preStartTimer = 0;
            readyTextActive = false;
            goTextActive = false;
            readyTextAlpha = Vector4.Zero;
            goTextAlpha = Vector4.Zero;
            deltaVector = Vector4.Zero;

            // create each background, put in list, give to manager
            Background.height = graphics.PreferredBackBufferHeight;
            background1 = new Background(Vector2.Zero);
            background2 = new Background(new Vector2(0, background1.position.Y + Background.height));
            bgList = new List<Background>();
            bgList.Add(background1);
            bgList.Add(background2);
            bgManager = new BackgroundManager(bgList, 
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 
                worldScrollSpeed);
            gui = new GUI();
            formation = new Formation();

            // state controls
            //System.Diagnostics.Debug.WriteLine(".");
            //System.Diagnostics.Debug.WriteLine(".");
            //System.Diagnostics.Debug.WriteLine(".");
            //System.Diagnostics.Debug.WriteLine("CONTROLS : (A)lignment, (S)eparation, (C)ohesion, (N)eighborhood Radius, Separation (R)adius + UP/DOWN to influence AI behavior. RMB to spawn AI. LMB to move player. P to toggle Fear." );
            //System.Diagnostics.Debug.WriteLine("CONTROLS : RMB to spawn AI. WASD to move player.");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            Background.texture = this.Content.Load<Texture2D>(currentLevelBG);
            Pitfall.texture = this.Content.Load<Texture2D>(currentLevelPF);

            Soldier.texture = this.Content.Load<Texture2D>("sprites/soldier");
            playerTex = this.Content.Load<Texture2D>("sprites/knight_gold");
            Barricade.texture = this.Content.Load<Texture2D>("sprites/barricade");
            FinishLine.texture = this.Content.Load<Texture2D>("sprites/finish_line");
            Boulder.texture = this.Content.Load<Texture2D>("sprites/boulder");
            Boulder.explosionTexture = this.Content.Load<Texture2D>("sprites/boulder_explosion");
            Boulder.shadowTexture = this.Content.Load<Texture2D>("sprites/boulder_shadow");

            gui.containerTexture = this.Content.Load<Texture2D>("sprites/containerGUI_alpha");
            gui.heartTexture = this.Content.Load<Texture2D>("sprites/heartGUI");
            gui.textBackground = this.Content.Load<Texture2D>("sprites/dividerGUI");
            gui.dividerTexture = this.Content.Load<Texture2D>("sprites/dividerGUI_alpha");
            gui.soldierTexture = this.Content.Load<Texture2D>("sprites/soldierGUI");
            gui.font = this.Content.Load<SpriteFont>("sprites/fontGUI");

            Button.font = this.Content.Load<SpriteFont>("sprites/fontButton");
            Button.texture = this.Content.Load<Texture2D>("sprites/containerGUI");
            Scoreboard.font = this.Content.Load<SpriteFont>("sprites/fontButton");
            Scoreboard.texture = this.Content.Load<Texture2D>("sprites/containerGUI");
            Scoreboard.starsTexture = this.Content.Load<Texture2D>("sprites/stars");
            TitleScreen.texture = this.Content.Load<Texture2D>("sprites/title_screen");
            TitleScreen.textTexture = this.Content.Load<Texture2D>("sprites/text_titlescreen");
            MainMenuScreen.texture = this.Content.Load<Texture2D>("sprites/mainmenu_screen");
            MainMenuScreen.textTexture = this.Content.Load<Texture2D>("sprites/text_mainmenu");
            LevelEndScreen.texture = this.Content.Load<Texture2D>("sprites/levelend_screen");
            LevelEndScreen.textureLevelEndText = this.Content.Load<Texture2D>("sprites/text_levelclear");
            GameEndScreen.textureDefeat = this.Content.Load<Texture2D>("sprites/gameend_defeat_screen"); // defeat
            GameEndScreen.textureVictory = this.Content.Load<Texture2D>("sprites/gameend_victory_screen"); // victory
            GameEndScreen.textureGameOverText = this.Content.Load<Texture2D>("sprites/text_gameover");
            GameEndScreen.textureDefeatText = this.Content.Load<Texture2D>("sprites/text_defeat");
            GameEndScreen.textureVictoryText = this.Content.Load<Texture2D>("sprites/text_victory");
            PauseScreen.backgroundTexture = Scoreboard.texture;
            PauseScreen.textTexture = this.Content.Load<Texture2D>("sprites/text_paused");
            readyText = this.Content.Load<Texture2D>("sprites/text_ready");
            goText = this.Content.Load<Texture2D>("sprites/text_go");

            flagTexture = this.Content.Load<Texture2D>("sprites/flagwave");

            if (!soundLoaded)
            {
                soundManager.LoadContent(this.Content);
                soundLoaded = true;
            }

            // reseting vars here that depend on LoadContent running first
            player = new Player(new Vector2(500, 0), playerTex);
            playerSM = new SpriteManager(player.texture, 4, 3);
            player.dashBarTexture = this.Content.Load<Texture2D>("sprites/dashbar");

            flag = new Flag(new Vector2(775, 74), flagTexture); // placed at flag pole  (775, 74)
            flagSM = new SpriteManager(flag.texture, 7, 4);

            // spawn soldiers that survived
            if (restarted)
            {
                numSoldiersEntered = 10; // reset if restarted game
            }
            for (int i = 0; i < numSoldiersEntered; i++)
                formation.add((screenWidth / 3f) + (i*35), 30);

            // start from title screen, if not shown already
            if (!titleScreenShowed)
            {
                currentScreen = new TitleScreen();
                //currentScreen = new GameEndScreen("Victory", 10); 
            }
            obstacleManager = new ObstacleManager();
        }

        protected override void UnloadContent()
        {
            soundManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            keyState = Keyboard.GetState();

            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            deltaVector = new Vector4(delta, delta, delta, delta);
            
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            #region mini game state
            switch (currentScreen.Type)
            {
                case "TitleScreen":
                    if (mouseState.LeftButton == ButtonState.Pressed) // when LMB is pressed on titleScreen, update changes screen to main menu
                    {
                        soundManager.menu_clickInstance.Play();
                        currentScreen.Update(gameTime);
                    }
                    if (!titleScreenShowed)
                    {
                        soundManager.createAllInstances(); // to avoid exeption if quitting from main menu
                        soundManager.bgm_titleInstance.Play();
                    }
                    titleScreenShowed = true;
                    break;
                case "MainMenuScreen":
                    if (currentScreen.GetType() != typeof(MainMenuScreen))
                    {
                        currentScreen = new MainMenuScreen();
                        // if we didn't come from title screen, reset sounds and play main song
                        if (soundManager.bgm_titleInstance.State != SoundState.Playing)
                        {
                            soundManager.UnloadContent();
                            soundManager.createScreenInstances();
                            soundManager.bgm_titleInstance.Play();
                        }
                    }
                    if(!restarted) // restart game once
                    {
                        restarted = true;
                        restartGame();
                        numSoldiersEntered = 10; // needs reset on new game, but not each level
                    } 
                    if (mouseState.LeftButton == ButtonState.Pressed 
                        && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        soundManager.menu_clickInstance.Play();
                        currentScreen.Update(gameTime, mouseState);
                    }
                    #region level select debugging control
                    /*
                    if (keyState.IsKeyDown(Keys.D1))
                    {
                        currentLevelBG = "ground_field";
                        currentLevelPF = "pitfall";
                        Background.texture = this.Content.Load<Texture2D>(currentLevelBG);
                        Pitfall.texture = this.Content.Load<Texture2D>(currentLevelPF);
                    }
                    if (keyState.IsKeyDown(Keys.D2))
                    {
                        currentLevelBG = "ground_mountain";
                        currentLevelPF = "pitfall_mountain";
                        Background.texture = this.Content.Load<Texture2D>(currentLevelBG);
                        Pitfall.texture = this.Content.Load<Texture2D>(currentLevelPF);
                    }
                    if (keyState.IsKeyDown(Keys.D3))
                    {
                        currentLevelBG = "ground_castle";
                        currentLevelPF = "pitfall_castle";
                        Background.texture = this.Content.Load<Texture2D>(currentLevelBG);
                        Pitfall.texture = this.Content.Load<Texture2D>(currentLevelPF);
                    }*/
                    #endregion
                    break;
                case "GameScreen":
                    if (currentScreen.GetType() != typeof(GameScreen))
                    {
                        currentScreen = new GameScreen();
                        if (fromScreen || retried) // if coming from non-in-game screen
                        {
                            if (soundManager.bgm_inGameInstance.State == SoundState.Playing)
                            {
                                soundManager.bgm_inGameInstance.Stop();
                                //soundManager.bgm_inGameInstance.Dispose();
                            }
                            soundManager.UnloadContent();
                            soundManager.createInGameInstances();
                            inPreStartPhase = true;
                            restarted = false; // allow restart again
                            retried = false; // allow retry again
                        }
                        if(!inPreStartPhase) // if resuming from pause
                        {
                            inPlay = true;
                        }
                    }
                    break;
                case "PauseScreen":
                    if (currentScreen.GetType() != typeof(PauseScreen))
                    {
                        currentScreen = new PauseScreen();
                        fromScreen = false;
                        inPlay = false;
                    }
                    
                    if (keyState.IsKeyDown(Keys.P) && previousKeyState.IsKeyUp(Keys.P) ||
                            keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
                    {
                        soundManager.menu_pauseOffInstance.Play();
                        currentScreen.Update(gameTime, mouseState);
                    }
                    else if (mouseState.LeftButton == ButtonState.Pressed 
                        && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        soundManager.menu_clickInstance.Play();
                        currentScreen.Update(gameTime, mouseState);
                    }
                    break;
                case "GameEndScreen":
                    if (currentScreen.GetType() != typeof(GameEndScreen))
                    {
                        soundManager.UnloadContent();
                        soundManager.createScreenInstances();
                        soundManager.bgm_gameEndDefeatInstance.Volume = .6f;
                        soundManager.crowd_cheerInstance.Volume = .6f;
                        if (player.dead)
                        {
                            currentScreen = new GameEndScreen("PlayerDead");
                            soundManager.bgm_gameEndDefeatInstance.Play();
                        }
                        else if (Formation.soldiers.Count <= 0)
                        {
                            currentScreen = new GameEndScreen("SoldiersDead");
                            soundManager.bgm_gameEndDefeatInstance.Play();
                        }
                        else
                        {
                            currentScreen = new GameEndScreen("Victory", numSoldiersExited);
                            flagActive = true;
                            soundManager.bgm_gameEndVictoryInstance.Volume = .7f;
                            soundManager.bgm_gameEndVictoryInstance.Play();
                            soundManager.crowd_cheerInstance.Play();
                        }
                        fromScreen = true;
                        announcerWaitOver = false;
                        inPlay = false;
                    }
                    // play announcer voice!
                    if (player.dead)
                    {
                        if (!announcerWaitOver)
                        {
                            soundManager.ann_gameOverInstance.Play();
                            announcerWaitTimer += delta;
                        }
                        if (announcerWaitTimer >= announcerWaitTimeMax)
                        {
                            if(!announcerWaitOver) soundManager.ann_continueInstance.Play(); // after 1 second, "Continue?"
                            announcerWaitOver = true;
                        }
                    }
                    else if (Formation.soldiers.Count <= 0)
                    {
                        if (!announcerWaitOver)
                        {
                            soundManager.ann_gameOverInstance.Play();
                            announcerWaitTimer += delta;
                        }
                        if (announcerWaitTimer >= announcerWaitTimeMax)
                        {
                            if (!announcerWaitOver) soundManager.ann_continueInstance.Play(); // after 1 second, "Continue?"
                            announcerWaitOver = true;
                        }
                    }
                    else
                    {
                        if (!announcerWaitOver)
                        {
                            soundManager.ann_congratsInstance.Play();
                            announcerWaitTimer += delta;
                        }
                        if (announcerWaitTimer >= announcerWaitTimeMax)
                        {
                            if (!announcerWaitOver) soundManager.ann_youWinInstance.Play(); // after 1 second, "Continue?"
                            announcerWaitOver = true;
                        }
                        if (soundManager.crowd_cheerInstance.State != SoundState.Playing
                        && soundManager.crowd_chattingInstance.State != SoundState.Playing)
                        {
                            soundManager.crowd_chattingInstance.Volume = .1f;
                            soundManager.crowd_chattingInstance.Play();
                        }
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed 
                        && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        soundManager.menu_clickInstance.Play();
                        currentScreen.Update(gameTime, mouseState);
                    }
                    if(flagActive)
                        flagSM.Update(gameTime);
                    break;
                case "LevelEndScreen":
                    if (currentScreen.GetType() != typeof(LevelEndScreen))
                    {
                        // create level end screen and keep track of how many survived each level
                        numSoldiersExited = Formation.soldiers.Count;
                        currentScreen = new LevelEndScreen(numSoldiersExited, numSoldiersEntered);
                        soundManager.cleanUpInGameSounds();
                        soundManager.createScreenInstances();
                        soundManager.bgm_levelEndInstance.Volume = .6f;
                        soundManager.bgm_levelEndInstance.Play();
                        // announcer says "Perfect!" if num survived equals num entered level
                        if (numSoldiersExited == numSoldiersEntered)
                        {
                            GameManager.soundManager.ann_perfectInstance.Play();
                        }
                        fromScreen = true;
                        inPlay = false;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed 
                        && previousMouseState.LeftButton == ButtonState.Released)
                    {
                        soundManager.menu_clickInstance.Play();
                        currentScreen.Update(gameTime, mouseState);
                    }
                    break;

                case "RestartLevel":
                    restartLevel();
                    break;
                case "NextLevel":
                    // this is entering amount for next level
                    numSoldiersEntered = numSoldiersExited;
                    nextLevel();
                    break;
                case "ExitGame":
                    Exit();
                    break;
            }
            #endregion
            if(inPreStartPhase)
            {
                preStartTimer += delta;

                // do the prestart stuff, moving down and no margins and godmode
                Player.godMode = true;
                Soldier.godMode = true;
                Player.marginEnabled = false;
                player.moveDown(delta);
                formation.moveDown();
                bgManager.Update(gameTime);
                gui.Update(gameTime);
                formation.run(gameTime, player);
                player.Update(gameTime);
                playerSM.applyDirection(delta, 1);

                if (soundManager.bgm_inGameInstance.State != SoundState.Playing)
                    soundManager.crowd_marchingInstance.Play(); // possible instance reference error
                // timing of announcer voices
                if(preStartTimer >= 1.5f
                    && preStartTimer < 1.6f)
                {
                    soundManager.ann_readyInstance.Play(); // "Ready?" 
                    readyTextActive = true;
                }
                // deactivate text
                if(preStartTimer >= 2.5f)
                {
                    readyTextActive = false;
                }
                if(preStartTimer >= 3.5f
                    && preStartTimer < 3.6f)
                {
                    soundManager.ann_goInstance.Play(); // "Go!" after "ready" ends
                    goTextActive = true;
                }
                // deactivate text
                if (preStartTimer >= 4f)
                {
                    goTextActive = false;
                }
                /*
                if(preStartTimer > 2.5f
                    && preStartTimer < 2.6f)
                {
                    //soundManager.getBattleCryInstance().Play(); // at the end of "go"
                }*/
                if(preStartTimer >= 3.8f
                    && preStartTimer < 3.9f)
                {
                    soundManager.crowd_chargeInstance.Play(); // right after battlecry
                }
                
                if(preStartTimer >= preStartTimeMax)
                {
                    inPreStartPhase = false;
                    inPlay = true;
                    Player.godMode = false;
                    Soldier.godMode = false;
                    Player.marginEnabled = true;
                    // set world speed after prestart intro has finished, and update bgManager at this time
                    if (difficulty == Difficulty.Easy)
                        worldScrollSpeed = defaultWorldScrollSpeed; // reset to default difficulty
                    if(difficulty == Difficulty.Medium)
                        worldScrollSpeed = defaultWorldScrollSpeed * 1.5f;
                    if(difficulty == Difficulty.Hard)
                        worldScrollSpeed = defaultWorldScrollSpeed * 2f;
                    bgManager.setScrollSpeed(worldScrollSpeed);

                    if (soundManager.bgm_inGameInstance.State != SoundState.Playing)
                    {
                        soundManager.bgm_inGameInstance.Volume = .5f;
                        soundManager.bgm_inGameInstance.Play();
                        soundManager.crowd_marchingInstance.Stop();
                    }
                }
            }
            if (inPlay) // false until prestart phase timer ends
            {
                // fade out crowd charge sound as play begins
                if(soundManager.crowd_chargeInstance.Volume >= delta * .2f)
                    soundManager.crowd_chargeInstance.Volume -= (delta * .2f);
                if (soundManager.crowd_chargeInstance.Volume == 0) // if silent, stop
                    soundManager.crowd_chargeInstance.Stop();

                if(!levelFinishSpawned)
                    finishLineTimer += delta;

                if (finishLineTimer >= finishLineTimeMax)
                {
                    levelFinishSpawned = true;
                    //System.Console.WriteLine("TIME!");
                    finishLine = new FinishLine(new Vector2(Soldier.margin - 3, screenHeight),
                        FinishLine.texture);
                    finishLineTimer = 0;
                }

                // Pause with p or escape while in game
                if (keyState.IsKeyDown(Keys.P) && previousKeyState.IsKeyUp(Keys.P) ||
                        keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
                {
                    soundManager.menu_pauseOnInstance.Play();
                    currentScreen.Update(gameTime);
                }
                /*
                if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                {
                    //player.position = new Vector2(mouseState.X, mouseState.Y);
                    //obstacleManager.add(new Vector2(mouseState.X, mouseState.Y));
                }
                
                if (mouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released)
                {
                    // use formation add function
                    formation.add(mouseState.X, mouseState.Y);
                }
                */

                #region Player Is Scary toggle (Commented out)
                /*
            if (keyState.IsKeyDown(Keys.P) && previousKeyState.IsKeyUp(Keys.P)) 
            {
                playerIsScary = !playerIsScary;
                System.Diagnostics.Debug.WriteLine("Player is Scary = " + playerIsScary);
            }*/
                #endregion
                #region AI CONTROLS (Commented out)
                /*
            if (keyState.IsKeyDown(Keys.N) && keyState.IsKeyDown(Keys.Up)) // neighborhood radius control UP
            {
                Soldier.neighborhoodRadius += 1f;
                System.Diagnostics.Debug.WriteLine("Neighborhood Radius = "+ Soldier.neighborhoodRadius);
            }
            if (keyState.IsKeyDown(Keys.N) && keyState.IsKeyDown(Keys.Down)) // neighborhood radius control DOWN
            {
                Soldier.neighborhoodRadius -= 1f;
                System.Diagnostics.Debug.WriteLine("Neighborhood Radius = "+Soldier.neighborhoodRadius);
            }

            if (keyState.IsKeyDown(Keys.R) && keyState.IsKeyDown(Keys.Up)) // separation radius control UP
            {
                Soldier.desiredSeparation += 1f;
                System.Diagnostics.Debug.WriteLine("Separation Radius = "+Soldier.desiredSeparation);
            }
            if (keyState.IsKeyDown(Keys.R) && keyState.IsKeyDown(Keys.Down)) // separation radius control DOWN
            {
                Soldier.desiredSeparation -= 1f;
                System.Diagnostics.Debug.WriteLine("Separation Radius = "+Soldier.desiredSeparation);
            }

            if (keyState.IsKeyDown(Keys.A) && keyState.IsKeyDown(Keys.Up)) // alignment control UP
            {
                Soldier.alignmentWeight += .1f;
                System.Diagnostics.Debug.WriteLine("Alignment Weight = "+Soldier.alignmentWeight);
            }
            if (keyState.IsKeyDown(Keys.A) && keyState.IsKeyDown(Keys.Down)) // alignment control DOWN
            {
                Soldier.alignmentWeight -= .1f;
                System.Diagnostics.Debug.WriteLine("Alignment Weight = " + Soldier.alignmentWeight);
            }

            if (keyState.IsKeyDown(Keys.S) && keyState.IsKeyDown(Keys.Up)) // separation control UP
            {
                Soldier.separationWeight += .1f;
                System.Diagnostics.Debug.WriteLine("Separation Weight = " + Soldier.separationWeight);
            }
            if (keyState.IsKeyDown(Keys.S) && keyState.IsKeyDown(Keys.Down)) // separation control DOWN
            {
                Soldier.separationWeight -= .1f;
                System.Diagnostics.Debug.WriteLine("Separation Weight = " + Soldier.separationWeight);
            }

            if (keyState.IsKeyDown(Keys.C) && keyState.IsKeyDown(Keys.Up)) // cohesion control UP
            {
                Soldier.cohesionWeight += .1f;
                System.Diagnostics.Debug.WriteLine("Cohesion Weight = " + Soldier.cohesionWeight);
            }
            if (keyState.IsKeyDown(Keys.C) && keyState.IsKeyDown(Keys.Down)) // cohesion control DOWN
            {
                Soldier.cohesionWeight -= .1f;
                System.Diagnostics.Debug.WriteLine("Cohesion Weight = " + Soldier.cohesionWeight);
            }
            */
                #endregion

                #region WASD PLAYER CONTROLS
                if (!levelCleared && !player.dead)
                {
                    if (keyState.IsKeyDown(Keys.W))
                    {
                        player.moveUp(delta);
                        playerSM.applyDirection(delta, 4);
                    }
                    if (keyState.IsKeyDown(Keys.A))
                    {
                        player.moveLeft(delta);
                        playerSM.applyDirection(delta, 2);
                    }
                    if (keyState.IsKeyDown(Keys.D))
                    {
                        player.moveRight(delta);
                        playerSM.applyDirection(delta, 3);
                    }
                    if (keyState.IsKeyDown(Keys.S))
                    {
                        player.moveDown(delta);
                        playerSM.applyDirection(delta, 1);
                    }
                    if(keyState.IsKeyDown(Keys.Space) 
                        && previousKeyState.IsKeyUp(Keys.Space))
                    {
                        player.dash();
                    }
                }
                else // level clear, so player is moving down, therefore animate
                    playerSM.applyDirection(delta, 1);
                if (keyState.IsKeyDown(Keys.G) && previousKeyState.IsKeyUp(Keys.G))
                {
                    player.resetHealth();
                    Player.godMode = !Player.godMode;
                    Soldier.godMode = !Soldier.godMode;
                    System.Console.WriteLine("GOD MODE: " + Player.godMode);
                }
                #endregion
                
                formation.run(gameTime, player);
                obstacleManager.Update(gameTime, player, levelFinishSpawned);
                player.Update(gameTime);
                bgManager.Update(gameTime);
                gui.Update(gameTime);

                // check for game over - defeat
                if(player.dead || Formation.soldiers.Count <= 0)
                {
                    gameOverTimer += delta;
                    if (gameOverTimer >= gameOverTimeMax)
                    {
                        currentScreen.Type = "GameEndScreen";
                        gameOverTimer = 0;
                    }
                }
                if (levelFinishSpawned)
                {
                    finishLine.Update(gameTime);
                    levelCleared = finishLine.cleared(player, Formation.soldiers);
                    // diable soldiers bottom margin restraint to allow to cross line asap
                    Soldier.bottomMarginEnabled = false;
                }
                if (levelCleared)
                {
                    // level is clear, so enable god mode and disable bottom margin to move off screen
                    Player.godMode = true;
                    Soldier.godMode = true;
                    Player.marginEnabled = false;
                    player.moveDown(delta);
                    formation.moveDown();

                    // buffer time between level clear and level end screen
                    levelClearTimer += delta;
                    if(levelClearTimer >= levelClearTimeMax)
                    {
                        levelCleared = false; // done with the level cleared loop
                        // switch to level end screen
                        currentScreen.Type = "LevelEndScreen";
                        levelClearTimer = 0;
                    }
                }
            }
            previousMouseState = mouseState;
            previousKeyState = keyState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if(currentLevelBG == "sprites/ground_field")
                GraphicsDevice.Clear(Color.DarkGoldenrod); // matches level majority color when tearing
            else
                GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            // if on game screen or paused, draw in game stuff, otherwise draw whatever screen is current
            switch(currentScreen.Type)
            {
                case "GameScreen":
                case "PauseScreen":
                    bgManager.Draw(spriteBatch);
                    if (levelFinishSpawned)
                        finishLine.Draw(spriteBatch);
                    obstacleManager.Draw(spriteBatch);
                    formation.Draw(spriteBatch);
                    player.Draw(spriteBatch); // draws player-based UI
                    playerSM.Draw(spriteBatch, player);
                    gui.Draw(spriteBatch);
                    currentScreen.Draw(spriteBatch);

                    if (readyTextActive || readyTextAlpha.W >= 0) // alpha will begin at 0 here, then fade till 0
                    {
                        spriteBatch.Draw(readyText, position: new Vector2(screenWidth / 2, screenHeight / 2),
                            origin: new Vector2(readyText.Width / 2, readyText.Height / 2),
                            color: new Color(
                                readyTextActive ? readyTextAlpha += (deltaVector * 2f) : readyTextAlpha -= (deltaVector * 2f))); // alpha up over time if active, down if inactive
                    }
                    if (goTextActive || goTextAlpha.W >= 0)
                    {
                        spriteBatch.Draw(goText, position: new Vector2(screenWidth / 2, screenHeight / 2),
                            origin: new Vector2(goText.Width / 2, goText.Height / 2),
                            color: new Color(
                                goTextActive ? goTextAlpha += (deltaVector * 3f) : goTextAlpha -= (deltaVector * 3f))); // alpha up over time if active, down if inactive
                    }
                    break;
                default:
                    currentScreen.Draw(spriteBatch);
                    if(flagActive) flagSM.Draw(spriteBatch, flag);
                    break;
                
            }
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        protected void restartGame()
        {
            currentLevelBG = "sprites/ground_field";
            currentLevelPF = "sprites/pitfall";
            Background.texture = this.Content.Load<Texture2D>(currentLevelBG);
            Pitfall.texture = this.Content.Load<Texture2D>(currentLevelPF);
            Initialize();
            fromScreen = true;
        }

        protected void restartLevel()
        {
            Background.texture = this.Content.Load<Texture2D>(currentLevelBG);
            Pitfall.texture = this.Content.Load<Texture2D>(currentLevelPF);
            Initialize();
            currentScreen.Type = "GameScreen";
            retried = true;
        }

        protected void nextLevel()
        {
            switch(currentLevelBG)
            {
                case "sprites/ground_field":
                    currentLevelBG = "sprites/ground_mountain";
                    currentLevelPF = "sprites/pitfall_mountain";
                    // world scroll speed handled at end of prestart phase
                    difficulty = Difficulty.Medium;
                    restartLevel(); // using these new values
                    break;
                case "sprites/ground_mountain":
                    currentLevelBG = "sprites/ground_castle";
                    currentLevelPF = "sprites/pitfall_castle";
                    difficulty = Difficulty.Hard;
                    restartLevel(); // using these new values
                    break;
                case "sprites/ground_castle":
                    // game over - victory, do not auto restart
                    currentScreen.Type = "GameEndScreen";
                    difficulty = Difficulty.Easy;
                    break;
            }
        }
    }
}
