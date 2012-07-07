using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Text;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using SpacepiXX.Inputs;
using Microsoft.Advertising.Mobile.Xna;

namespace SpacepiXX
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Spacepixx : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// Advertising stuff
        /// </summary>
        static AdGameComponent adGameComponent;
        static DrawableAd bannerAd;

        private const string HighscoreText = "Personal Highscore!";
        private const string GameOverText = "GAME OVER!";

        private const string KeyboardTitleTextVeryBad = "Are you kidding me???";
        private const string KeyboardTitleTextBad = "Really?!?";
        private const string KeyboardTitleTextVeryLow = "Not bad!";
        private const string KeyboardTitleTextLow = "Well done!";
        private const string KeyboardTitleTextMed = "Congratulation!";
        private const string KeyboardTitleTextHigh = "Nice!";
        private const string KeyboardTitleTextVeryHigh = "Amazing!";
        private const string KeyboardTitleTextUltra = "Awesome!";
        private const string KeyboardTitleTextUltraPlus = "Excellent!";
        private const string KeyboardTitleTextGodlike = "God? Is it you?";

        private const string KeyboardInLocalMessageFormatText = "You are locally ranked {0}/10!\nPlease enter your name...\n[only: A..Z, a..z, 0..9, 12 characters]";
        private const string KeyboardNotInLocalMessageFormatText = "You are not locally ranked!\nPlease enter your name for online submission...\n[only: A..Z, a..z, 0..9, 12 characters]";

        private Random rand = new Random();
        private static readonly string[] GetReadyText = { "Get Ready!", "Rock 'n' Roll!", "Rock on!", "Fight!", "Attack!" };

        private const string ContinueText = "Push to continue...";
        private string VersionText;
        private const string MusicByText = "Music by";
        private const string MusicCreatorText = "Tscho";
        private const string CreatorText = "by B. Sautermeister";

        enum GameStates { TitleScreen, MainMenu, Highscores, Inscructions, Help, Settings, Playing, BossDuell, Paused, PlayerDead, GameOver, Leaderboards, Submittion };
        GameStates gameState = GameStates.TitleScreen;
        GameStates stateBeforePaused;
        Texture2D spriteSheet;
        Texture2D menuSheet;

        StarFieldManager starFieldManager1;
        StarFieldManager starFieldManager2;
        StarFieldManager starFieldManager3;

        AsteroidManager asteroidManager;

        PlayerManager playerManager;

        EnemyManager enemyManager;
        BossManager bossManager;

        CollisionManager collisionManager;

        SpriteFont pericles16;
        SpriteFont pericles18;

        ZoomTextManager zoomTextManager;
        
        private float playerDeathTimer = 0.0f;
        private const float playerDeathDelayTime = 6.0f;
        private const float playerGameOverDelayTime = 5.0f;
        
        private float titleScreenTimer = 0.0f;
        private const float titleScreenDelayTime = 1.0f;

        private Vector2 playerStartLocation = new Vector2(375, 375);
        private Vector2 highscoreLocation = new Vector2(10, 10);

        Hud hud;

        HighscoreManager highscoreManager;
        private bool highscoreMessageShown = false;

        LeaderboardManager leaderboardManager;

        SubmissionManager submissionManager;

        MainMenuManager mainMenuManager;

        private float backButtonTimer = 0.0f;
        private const float backButtonDelayTime = 0.25f;

        LevelManager levelManager;

        InstructionManager instructionManager;

        HelpManager helpManager;

        PowerUpManager powerUpManager;
        Texture2D powerUpSheet;

        SettingsManager settingsManager;

        private bool bossDirectKill = true;
        private const long InitialBossBonusScore = 5000;
        private long bossBonusScore = InitialBossBonusScore;

        GameInput gameInput = new GameInput();
        private const string TitleAction = "Title";
        private const string BackToGameAction = "BackToGame";
        private const string BackToMainAction = "BackToMain";

        private readonly Rectangle cancelSource = new Rectangle(0, 900,
                                                                300, 50);
        private readonly Rectangle cancelDestination = new Rectangle(450, 370,
                                                                     300, 50);

        private readonly Rectangle continueSource = new Rectangle(0, 850,
                                                                  300, 50);
        private readonly Rectangle continueDestination = new Rectangle(50, 370,
                                                                       300, 50);

        public Spacepixx()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            Content.RootDirectory = "Content";

#if DEBUG
            AdGameComponent.Initialize(this, "test_client");
#else
            AdGameComponent.Initialize(this, "3e0d3faa-d78e-4052-b9d3-2cefd90f50f2");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
#endif       

            adGameComponent = AdGameComponent.Current;

            // Frame rate is 60 fps
            TargetElapsedTime = TimeSpan.FromTicks(166667);

            InitializaPhoneServices();
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 800;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;

            // Aplly the gfx changes
            graphics.ApplyChanges();

            TouchPanel.EnabledGestures = GestureType.Tap;

            loadVersion();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a banner ad for the game.
#if DEBUG
            bannerAd = adGameComponent.CreateAd("Image480_80", new Rectangle(160, 0, 480, 80));
#else
            bannerAd = adGameComponent.CreateAd("92277", new Rectangle(160, 0, 480, 80));
#endif       
            bannerAd.BorderEnabled = false;

            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");
            menuSheet = Content.Load<Texture2D>(@"Textures\MenuSheet");
            powerUpSheet = Content.Load<Texture2D>(@"Textures\PowerUpSheet");

            starFieldManager1 = new StarFieldManager(this.GraphicsDevice.Viewport.Width,
                                                    this.GraphicsDevice.Viewport.Height,
                                                    100,
                                                    new Vector2(0, 20.0f),
                                                    spriteSheet,
                                                    new Rectangle(0, 350, 1, 1));
            starFieldManager2 = new StarFieldManager(this.GraphicsDevice.Viewport.Width,
                                                    this.GraphicsDevice.Viewport.Height,
                                                    70,
                                                    new Vector2(0, 40.0f),
                                                    spriteSheet,
                                                    new Rectangle(0, 350, 2, 2));
            starFieldManager3 = new StarFieldManager(this.GraphicsDevice.Viewport.Width,
                                                    this.GraphicsDevice.Viewport.Height,
                                                    30,
                                                    new Vector2(0, 60.0f),
                                                    spriteSheet,
                                                    new Rectangle(0, 350, 3, 3));

            asteroidManager = new AsteroidManager(3,
                                                  spriteSheet,
                                                  new Rectangle(0, 0, 50, 50),
                                                  20,
                                                  this.GraphicsDevice.Viewport.Width,
                                                  this.GraphicsDevice.Viewport.Height);

            playerManager = new PlayerManager(spriteSheet,
                                              new Rectangle(0, 150, 50, 50),
                                              6,
                                              new Rectangle(0, 0,
                                                            this.GraphicsDevice.Viewport.Width,
                                                            this.GraphicsDevice.Viewport.Height),
                                              playerStartLocation,
                                              gameInput);

            enemyManager = new EnemyManager(spriteSheet,
                                            playerManager,
                                            new Rectangle(0, 0,
                                                          this.GraphicsDevice.Viewport.Width,
                                                          this.GraphicsDevice.Viewport.Height));

            bossManager = new BossManager(spriteSheet,
                                          playerManager,
                                          new Rectangle(0, 0,
                                                        this.GraphicsDevice.Viewport.Width,
                                                        this.GraphicsDevice.Viewport.Height));
            Boss.Player = playerManager;

            EffectManager.Initialize(spriteSheet,
                                     new Rectangle(0, 350, 2, 2),
                                     new Rectangle(0, 100, 50, 50),
                                     5);

            powerUpManager = new PowerUpManager(powerUpSheet);

            collisionManager = new CollisionManager(asteroidManager,
                                                    playerManager,
                                                    enemyManager,
                                                    bossManager,
                                                    powerUpManager);

            SoundManager.Initialize(Content);

            pericles16 = Content.Load<SpriteFont>(@"Fonts\Pericles16");
            pericles18 = Content.Load<SpriteFont>(@"Fonts\Pericles18");

            zoomTextManager = new ZoomTextManager(new Vector2(this.GraphicsDevice.Viewport.Width / 2,
                                                              this.GraphicsDevice.Viewport.Height / 2),
                                                              pericles16);

            hud = Hud.GetInstance(GraphicsDevice.Viewport.Bounds,
                                  spriteSheet,
                                  pericles16,
                                  0,
                                  3,
                                  0.0f,
                                  100.0f,
                                  0.0f,
                                  3,
                                  10,
                                  PlayerManager.MIN_SCORE_MULTI,
                                  1,
                                  bossManager);

            highscoreManager = HighscoreManager.GetInstance();
            HighscoreManager.Font = pericles18;
            HighscoreManager.Texture = menuSheet;

            leaderboardManager = LeaderboardManager.GetInstance();
            LeaderboardManager.Font = pericles18;
            LeaderboardManager.Texture = menuSheet;
            HighscoreManager.GameInput = gameInput;

            submissionManager = SubmissionManager.GetInstance();
            SubmissionManager.Font = pericles18;
            SubmissionManager.Texture = menuSheet;
            SubmissionManager.GameInput = gameInput;

            mainMenuManager = new MainMenuManager(menuSheet, gameInput);

            levelManager = new LevelManager();
            levelManager.Register(asteroidManager);
            levelManager.Register(enemyManager);
            levelManager.Register(bossManager);
            levelManager.Register(playerManager);

            instructionManager = new InstructionManager(spriteSheet,
                                                        pericles18,
                                                        new Rectangle(0, 0,
                                                                      GraphicsDevice.Viewport.Width,
                                                                      GraphicsDevice.Viewport.Height),
                                                        asteroidManager,
                                                        playerManager,
                                                        enemyManager,
                                                        bossManager,
                                                        powerUpManager);

            helpManager = new HelpManager(menuSheet, pericles18, new Rectangle(0, 0,
                                                                               GraphicsDevice.Viewport.Width,
                                                                               GraphicsDevice.Viewport.Height));
            HelpManager.GameInput = gameInput;
            SoundManager.PlayBackgroundSound();


            settingsManager = SettingsManager.GetInstance();
            settingsManager.Initialize(menuSheet, pericles18, new Rectangle(0, 0,
                                                                            GraphicsDevice.Viewport.Width,
                                                                            GraphicsDevice.Viewport.Height));
            SettingsManager.GameInput = gameInput;

            setupInputs();
        }

        private void setupInputs()
        {
            gameInput.AddTouchGestureInput(TitleAction, GestureType.Tap, new Rectangle(0, 0,
                                                                                   800, 480));
            gameInput.AddTouchGestureInput(BackToGameAction, GestureType.Tap, continueDestination);
            gameInput.AddTouchGestureInput(BackToMainAction, GestureType.Tap, cancelDestination);
            mainMenuManager.SetupInputs();
            playerManager.SetupInputs();
            submissionManager.SetupInputs();
            highscoreManager.SetupInputs();
            settingsManager.SetupInputs();
            helpManager.SetupInputs();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void InitializaPhoneServices()
        {
            PhoneApplicationService.Current.Activated += new EventHandler<ActivatedEventArgs>(GameActivated);
            PhoneApplicationService.Current.Deactivated += new EventHandler<DeactivatedEventArgs>(GameDeactivated);
            PhoneApplicationService.Current.Closing += new EventHandler<ClosingEventArgs>(GameClosing);
            PhoneApplicationService.Current.Launching += new EventHandler<LaunchingEventArgs>(GameLaunching);
        }

        /// <summary>
        /// Occurs when the game class (and application) deactivated and tombstoned.
        /// Saves state of: Spacepixx, AsteroidManager
        /// Does not save: Starfield-Manager (not necessary)
        /// </summary>
        void GameDeactivated(object sender, DeactivatedEventArgs e)
        {
            // Save to Isolated Storage file
            using (IsolatedStorageFile isolatedStorageFile
                = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // If user choose to save, create a new file
                using (IsolatedStorageFileStream fileStream
                    = isolatedStorageFile.CreateFile("state.dat"))
                {
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        // Write date to the file
                        writer.WriteLine(this.gameState);
                        writer.WriteLine(this.stateBeforePaused);
                        writer.WriteLine(this.playerDeathTimer);
                        writer.WriteLine(this.titleScreenTimer);
                        writer.WriteLine(this.highscoreMessageShown);
                        writer.WriteLine(this.backButtonTimer);

                        writer.WriteLine(bossBonusScore);
                        writer.WriteLine(bossDirectKill);

                        asteroidManager.Deactivated(writer);

                        playerManager.Deactivated(writer);

                        enemyManager.Deactivated(writer);

                        bossManager.Deactivated(writer);

                        EffectManager.Deactivated(writer);

                        powerUpManager.Deactivated(writer);

                        zoomTextManager.Deactivated(writer);

                        levelManager.Deactivated(writer);

                        instructionManager.Deactivated(writer);

                        mainMenuManager.Deactivated(writer);

                        highscoreManager.Deactivated(writer);

                        submissionManager.Deactivated(writer);

                        writer.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Pauses the game when a call is incoming.
        /// Attention: Also called for GUID !!!
        /// </summary>
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);

            if (gameState == GameStates.Playing
                || gameState == GameStates.PlayerDead
                || gameState == GameStates.Inscructions
                || gameState == GameStates.BossDuell)
            {
                stateBeforePaused = gameState;
                gameState = GameStates.Paused;
            }
        }

        void GameLaunching(object sender, LaunchingEventArgs e)
        {
            tryLoadGame();

            if (gameState == GameStates.MainMenu || gameState == GameStates.Help ||
                gameState == GameStates.Highscores || gameState == GameStates.Settings ||
                gameState == GameStates.Submittion)
            {
                gameState = GameStates.TitleScreen;
            }
        }

        ///// <summary>
        ///// Occurs when the game class (and application) activated during return from tombstoned state
        ///// </summary>
        void GameActivated(object sender, ActivatedEventArgs e)
        {
            tryLoadGame();
        }

        void GameClosing(object sender, ClosingEventArgs e)
        {
            instructionManager.SaveHasDoneInstructions();

            // delete saved active-game on real exit
            using (IsolatedStorageFile isolatedStorageFile
                    = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isolatedStorageFile.FileExists("state.dat"))
                {
                    isolatedStorageFile.DeleteFile("state.dat");
                }
            }
        }

        private void tryLoadGame()
        {
            try
            {
                using (IsolatedStorageFile isolatedStorageFile
                    = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isolatedStorageFile.FileExists("state.dat"))
                    {
                        //If user choose to save, create a new file
                        using (IsolatedStorageFileStream fileStream
                            = isolatedStorageFile.OpenFile("state.dat", FileMode.Open))
                        {
                            using (StreamReader reader = new StreamReader(fileStream))
                            {
                                this.gameState = (GameStates)Enum.Parse(gameState.GetType(), reader.ReadLine(), true);
                                this.stateBeforePaused = (GameStates)Enum.Parse(stateBeforePaused.GetType(), reader.ReadLine(), true);

                                if (gameState == GameStates.Playing)
                                {
                                    gameState = GameStates.Paused;
                                    stateBeforePaused = GameStates.Playing;
                                }

                                if (gameState == GameStates.PlayerDead)
                                {
                                    gameState = GameStates.Paused;
                                    stateBeforePaused = GameStates.PlayerDead;
                                }

                                this.playerDeathTimer = (float)Single.Parse(reader.ReadLine());
                                this.titleScreenTimer = (float)Single.Parse(reader.ReadLine());
                                this.highscoreMessageShown = (bool)Boolean.Parse(reader.ReadLine());
                                this.backButtonTimer = (float)Single.Parse(reader.ReadLine());

                                this.bossBonusScore = Int64.Parse(reader.ReadLine());
                                this.bossDirectKill = Boolean.Parse(reader.ReadLine());

                                asteroidManager.Activated(reader);

                                playerManager.Activated(reader);

                                enemyManager.Activated(reader);

                                bossManager.Activated(reader);

                                EffectManager.Activated(reader);

                                powerUpManager.Activated(reader);

                                zoomTextManager.Activated(reader);

                                levelManager.Activated(reader);

                                instructionManager.Activated(reader);

                                mainMenuManager.Activated(reader);

                                highscoreManager.Activated(reader);

                                submissionManager.Activated(reader);

                                reader.Close();
                            }
                        }

                        isolatedStorageFile.DeleteFile("state.dat");
                    }
                }
            }
            catch (FormatException)
            {
                // catch end restore in case of incompatible active/deactivate dat-files
                this.resetGame();
                this.gameState = GameStates.TitleScreen;
            }

            GC.Collect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Advertisment stuff
            if (gameState == GameStates.Help ||
                gameState == GameStates.MainMenu ||
                gameState == GameStates.Paused ||
                gameState == GameStates.Settings ||
                gameState == GameStates.TitleScreen ||
                gameState == GameStates.Submittion)
            {
                adGameComponent.Update(gameTime);
            }

            SoundManager.Update(gameTime);

            gameInput.BeginUpdate();

            bool backButtonPressed = false;

            backButtonTimer += elapsed;

            if (backButtonTimer >= backButtonDelayTime)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    backButtonPressed = true;
                    backButtonTimer = 0.0f;
                }
            }

            switch (gameState)
            {
                case GameStates.TitleScreen:

                    titleScreenTimer += elapsed;

                    updateBackground(gameTime);

                    if (titleScreenTimer >= titleScreenDelayTime)
                    {
                        if (gameInput.IsPressed(TitleAction))
                        {
                            gameState = GameStates.MainMenu;
                        }
                    }

                    if (backButtonPressed)
                        this.Exit();

                    break;

                case GameStates.MainMenu:

                    updateBackground(gameTime);

                    mainMenuManager.IsActive = true;
                    mainMenuManager.Update(gameTime);

                    switch(mainMenuManager.LastPressedMenuItem)
                    {
                        case MainMenuManager.MenuItems.Start:
                            resetGame();
                            hud.Update(playerManager.PlayerScore,
                                       playerManager.LivesRemaining,
                                       playerManager.Overheat,
                                       playerManager.HitPoints,
                                       playerManager.ShieldPoints,
                                       playerManager.SpecialShotsRemaining,
                                       playerManager.CarliRocketsRemaining,
                                       playerManager.ScoreMulti,
                                       levelManager.CurrentLevel);
                            gameState = GameStates.Playing;
                            break;

                        case MainMenuManager.MenuItems.Highscores:
                            gameState = GameStates.Highscores;
                            break;

                        case MainMenuManager.MenuItems.Instructions:
                            resetGame();
                            instructionManager.Reset();
                            hud.Update(playerManager.PlayerScore,
                                       playerManager.LivesRemaining,
                                       playerManager.Overheat,
                                       playerManager.HitPoints,
                                       playerManager.ShieldPoints,
                                       playerManager.SpecialShotsRemaining,
                                       playerManager.CarliRocketsRemaining,
                                       playerManager.ScoreMulti,
                                       levelManager.CurrentLevel);
                            gameState = GameStates.Inscructions;
                            break;

                        case MainMenuManager.MenuItems.Help:
                            gameState = GameStates.Help;
                            break;

                        case MainMenuManager.MenuItems.Settings:
                            gameState = GameStates.Settings;
                            break;

                        case MainMenuManager.MenuItems.None:
                            // do nothing
                            break;
                    }

                    if (gameState != GameStates.MainMenu)
                        mainMenuManager.IsActive = false;

                    if (backButtonPressed)
                        this.Exit();

                    break;

                case GameStates.Highscores:

                    updateBackground(gameTime);

                    highscoreManager.IsActive = true;
                    highscoreManager.Update(gameTime);

                    if (backButtonPressed)
                    {
                        highscoreManager.IsActive = false;
                        gameState = GameStates.MainMenu;
                    }

                    break;

                case GameStates.Submittion:

                    updateBackground(gameTime);

                    submissionManager.IsActive = true;
                    submissionManager.Update(gameTime);

                    if (submissionManager.CancelClicked || backButtonPressed)
                    {
                        submissionManager.IsActive = false;
                        gameState = GameStates.MainMenu;
                    }
                    else if (submissionManager.RetryClicked)
                    {
                        submissionManager.IsActive = false;
                        resetGame();
                        hud.Update(playerManager.PlayerScore,
                                   playerManager.LivesRemaining,
                                   playerManager.Overheat,
                                   playerManager.HitPoints,
                                   playerManager.ShieldPoints,
                                   playerManager.SpecialShotsRemaining,
                                   playerManager.CarliRocketsRemaining,
                                   playerManager.ScoreMulti,
                                   levelManager.CurrentLevel);
                        gameState = GameStates.Playing;
                    }

                    break;

                case GameStates.Inscructions:

                    starFieldManager1.Update(gameTime);
                    starFieldManager2.Update(gameTime);
                    starFieldManager3.Update(gameTime);

                    instructionManager.Update(gameTime);
                    collisionManager.Update();
                    EffectManager.Update(gameTime);
                    hud.Update(playerManager.PlayerScore,
                               playerManager.LivesRemaining, 
                               playerManager.Overheat, 
                               playerManager.HitPoints,
                               playerManager.ShieldPoints,
                               playerManager.SpecialShotsRemaining,
                               playerManager.CarliRocketsRemaining,
                               playerManager.ScoreMulti,
                               levelManager.CurrentLevel);

                    if (backButtonPressed)
                    {
                        InstructionManager.HasDoneInstructions = true;
                        gameState = GameStates.MainMenu;
                    }

                    break;

                case GameStates.Help:

                    updateBackground(gameTime);

                    helpManager.IsActive = true;
                    helpManager.Update(gameTime);

                    if (backButtonPressed)
                    {
                        helpManager.IsActive = false;
                        gameState = GameStates.MainMenu;
                    }

                    break;

                case GameStates.Settings:

                    updateBackground(gameTime);

                    settingsManager.IsActive = true;
                    settingsManager.Update(gameTime);

                    if (backButtonPressed)
                    {
                        settingsManager.IsActive = false;
                        gameState = GameStates.MainMenu;
                    }

                    break;

                case GameStates.Playing:

                    updateBackground(gameTime);

                    levelManager.Update(gameTime);

                    playerManager.Update(gameTime);

                    enemyManager.Update(gameTime);
                    enemyManager.IsActive = true;

                    bossManager.Update(gameTime);

                    EffectManager.Update(gameTime);

                    collisionManager.Update();

                    powerUpManager.Update(gameTime);

                    zoomTextManager.Update();

                    hud.Update(playerManager.PlayerScore,
                               playerManager.LivesRemaining,
                               playerManager.Overheat,
                               playerManager.HitPoints,
                               playerManager.ShieldPoints,
                               playerManager.SpecialShotsRemaining,
                               playerManager.CarliRocketsRemaining,
                               playerManager.ScoreMulti,
                               levelManager.CurrentLevel);

                    if (levelManager.HasChanged)
                    {
                        enemyManager.IsActive = false;

                        if (enemyManager.Enemies.Count == 0)
                        {
                            bossManager.SpawnRandomBoss();
                            gameState = GameStates.BossDuell;
                        }
                    }

                    if (playerManager.PlayerScore > highscoreManager.CurrentHighscore &&
                        highscoreManager.CurrentHighscore != 0 &&
                       !highscoreMessageShown)
                    {
                        zoomTextManager.ShowText(HighscoreText);
                        highscoreMessageShown = true;
                    }

                    if (playerManager.IsDestroyed)
                    {
                        playerDeathTimer = 0.0f;
                        enemyManager.IsActive = false;
                        bossManager.IsActive = false;
                        playerManager.LivesRemaining--;

                        if (playerManager.LivesRemaining < 0)
                        {
                            levelManager.ResetLevelTimer();
                            gameState = GameStates.GameOver;
                            zoomTextManager.ShowText(GameOverText);
                        }
                        else
                        {
                            levelManager.ResetLevelTimer();
                            gameState = GameStates.PlayerDead;
                        }
                    }

                    if (backButtonPressed)
                    {
                        stateBeforePaused = GameStates.Playing;
                        gameState = GameStates.Paused;
                    }

                    break;

                case GameStates.BossDuell:

                    updateBackground(gameTime);

                    levelManager.Update(gameTime);

                    playerManager.Update(gameTime);

                    enemyManager.Update(gameTime);
                    //enemyManager.IsActive = false;

                    bossManager.Update(gameTime);

                    EffectManager.Update(gameTime);

                    collisionManager.Update();

                    powerUpManager.Update(gameTime);

                    zoomTextManager.Update();

                    hud.Update(playerManager.PlayerScore,
                               playerManager.LivesRemaining,
                               playerManager.Overheat,
                               playerManager.HitPoints,
                               playerManager.ShieldPoints,
                               playerManager.SpecialShotsRemaining,
                               playerManager.CarliRocketsRemaining,
                               playerManager.ScoreMulti,
                               levelManager.CurrentLevel);

                    if (bossManager.Bosses.Count == 0 && levelManager.HasChanged)
                    {
                        if (bossManager.BossWasKilled)
                        {
                            bossManager.BossWasKilled = false;

                            if (bossDirectKill)
                            {
                                zoomTextManager.ShowText("+" + bossBonusScore + " Bonus");
                                playerManager.IncreasePlayerScore(bossBonusScore, false);
                                bossBonusScore += InitialBossBonusScore;
                            }
                            else
                            {
                                bossBonusScore = InitialBossBonusScore;
                            }
                            levelManager.GoToNextLevel();
                            zoomTextManager.ShowText("Level " + levelManager.CurrentLevel);

                            bossDirectKill = true;
                        }
                        else
                        {
                            bossDirectKill = false;

                            levelManager.ResetLevelTimer();
                        }

                        gameState = GameStates.Playing;
                    }

                    if (playerManager.PlayerScore > highscoreManager.CurrentHighscore &&
                        highscoreManager.CurrentHighscore != 0 &&
                        !highscoreMessageShown)
                    {
                        zoomTextManager.ShowText(HighscoreText);
                        highscoreMessageShown = true;
                    }

                    if (playerManager.IsDestroyed)
                    {
                        playerDeathTimer = 0.0f;
                        enemyManager.IsActive = false;
                        bossManager.IsActive = false;
                        playerManager.LivesRemaining--;

                        if (playerManager.LivesRemaining < 0)
                        {
                            gameState = GameStates.GameOver;
                            zoomTextManager.ShowText(GameOverText);
                        }
                        else
                        {
                            gameState = GameStates.PlayerDead;
                            bossDirectKill = false;
                        }
                        
                        levelManager.ResetLevelTimer();
                    }

                    if (backButtonPressed)
                    {
                        stateBeforePaused = GameStates.BossDuell;
                        gameState = GameStates.Paused;
                    }

                    break;

                case GameStates.Paused:

                    if (gameInput.IsPressed(BackToGameAction) || backButtonPressed)
                    {
                        gameState = stateBeforePaused;
                    }

                    if (gameInput.IsPressed(BackToMainAction))
                    {
                        if (!Guide.IsVisible && playerManager.PlayerScore > 0)
                        {
                            showGuid();
                        }

                        if (playerManager.PlayerScore > 0)
                        {
                            gameState = GameStates.Submittion;
                            submissionManager.SetUp(highscoreManager.LastName, playerManager.PlayerScore, levelManager.CurrentLevel);
                        }
                        else
                        {
                            gameState = GameStates.MainMenu;
                        }
                    }

                    break;

                case GameStates.PlayerDead:

                    playerDeathTimer += elapsed;

                    updateBackground(gameTime);
                    asteroidManager.Update(gameTime);
                    asteroidManager.IsActive = false;
                    starFieldManager1.SpeedFactor = 3.0f;
                    starFieldManager2.SpeedFactor = 3.0f;
                    starFieldManager3.SpeedFactor = 3.0f;

                    playerManager.PlayerShotManager.Update(gameTime);
                    playerManager.PlayerShotManager.Update(gameTime);

                    powerUpManager.Update(gameTime);

                    enemyManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    bossManager.Update(gameTime);
                    bossManager.Update(gameTime);
                    EffectManager.Update(gameTime);
                    EffectManager.Update(gameTime);
                    collisionManager.Update();
                    zoomTextManager.Update();
                    hud.Update(playerManager.PlayerScore,
                               playerManager.LivesRemaining,
                               playerManager.Overheat,
                               playerManager.HitPoints,
                               playerManager.ShieldPoints,
                               playerManager.SpecialShotsRemaining,
                               playerManager.CarliRocketsRemaining,
                               playerManager.ScoreMulti,
                               levelManager.CurrentLevel);

                    if (playerDeathTimer >= playerDeathDelayTime)
                    {
                        starFieldManager1.SpeedFactor = 1.0f;
                        starFieldManager2.SpeedFactor = 1.0f;
                        starFieldManager3.SpeedFactor = 1.0f;
                        asteroidManager.IsActive = true;
                        resetRound();
                        gameState = GameStates.Playing;
                    }

                    if (backButtonPressed)
                    {
                        starFieldManager1.SpeedFactor = 1.0f;
                        starFieldManager2.SpeedFactor = 1.0f;
                        starFieldManager3.SpeedFactor = 1.0f;
                        asteroidManager.IsActive = true;
                        stateBeforePaused = GameStates.PlayerDead;
                        gameState = GameStates.Paused;
                    }

                    break;

                case GameStates.GameOver:

                    playerDeathTimer += elapsed;

                    updateBackground(gameTime);

                    playerManager.PlayerShotManager.Update(gameTime);
                    powerUpManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    bossManager.Update(gameTime);
                    EffectManager.Update(gameTime);
                    collisionManager.Update();
                    zoomTextManager.Update();
                    hud.Update(playerManager.PlayerScore,
                               playerManager.LivesRemaining,
                               playerManager.Overheat,
                               playerManager.HitPoints,
                               playerManager.ShieldPoints,
                               playerManager.SpecialShotsRemaining,
                               playerManager.CarliRocketsRemaining,
                               playerManager.ScoreMulti,
                               levelManager.CurrentLevel);

                    if (playerDeathTimer >= playerGameOverDelayTime)
                    {
                        if (!Guide.IsVisible && playerManager.PlayerScore > 0)
                        {
                            showGuid();
                        }
                        else
                        {
                            playerDeathTimer = 0.0f;
                            titleScreenTimer = 0.0f;

                            if (playerManager.PlayerScore > 0)
                            {
                                gameState = GameStates.Submittion;
                                submissionManager.SetUp(highscoreManager.LastName, playerManager.PlayerScore, levelManager.CurrentLevel);
                            }
                            else
                            {
                                gameState = GameStates.MainMenu;
                            }
                        }
                    }

                    if (backButtonPressed)
                    {
                        stateBeforePaused = GameStates.GameOver;
                        gameState = GameStates.Paused;
                    }

                    break;
            }

            // Remove gesture queue
            //while (TouchPanel.IsGestureAvailable)
            //{
            //    TouchPanel.ReadGesture();
            //}

            // Reset Back-Button flag
            backButtonPressed = false;

            gameInput.EndUpdate();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (gameState == GameStates.TitleScreen)
            {
                drawBackground(spriteBatch);
                
                spriteBatch.Draw(menuSheet,
                                 new Vector2(150.0f, 150.0f),
                                 new Rectangle(0, 0,
                                               500,
                                               100),
                                 Color.White);


                spriteBatch.DrawString(pericles18,
                                       ContinueText,
                                       new Vector2(this.GraphicsDevice.Viewport.Width / 2 - pericles18.MeasureString(ContinueText).X / 2,
                                                   275),
                                       Color.Red * (0.25f + (float)(Math.Pow(Math.Sin(gameTime.TotalGameTime.TotalSeconds), 2.0f)) * 0.75f));

                spriteBatch.DrawString(pericles16,
                                       MusicByText,
                                       new Vector2(this.GraphicsDevice.Viewport.Width / 2 - pericles18.MeasureString(MusicByText).X / 2,
                                                   410),
                                       Color.Red);
                spriteBatch.DrawString(pericles16,
                                       MusicCreatorText,
                                       new Vector2(this.GraphicsDevice.Viewport.Width / 2 - pericles18.MeasureString(MusicCreatorText).X / 2,
                                                   435),
                                       Color.Red);

                spriteBatch.DrawString(pericles16,
                                       VersionText,
                                       new Vector2(this.GraphicsDevice.Viewport.Width - (pericles16.MeasureString(VersionText).X + 15),
                                                   this.GraphicsDevice.Viewport.Height - (pericles16.MeasureString(VersionText).Y + 10)),
                                       Color.Red);

                spriteBatch.DrawString(pericles16,
                                       CreatorText,
                                       new Vector2(15,
                                                   this.GraphicsDevice.Viewport.Height - (pericles16.MeasureString(CreatorText).Y + 10)),
                                       Color.Red);
            }

            if (gameState == GameStates.MainMenu)
            {
                drawBackground(spriteBatch);
                
                mainMenuManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.Highscores)
            {
                drawBackground(spriteBatch);

                highscoreManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.Submittion)
            {
                drawBackground(spriteBatch);

                submissionManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.Inscructions)
            {
                starFieldManager1.Draw(spriteBatch);
                starFieldManager2.Draw(spriteBatch);
                starFieldManager3.Draw(spriteBatch);
 
                instructionManager.Draw(spriteBatch);
                
                EffectManager.Draw(spriteBatch);
                
                hud.Draw(spriteBatch);
            }

            if (gameState == GameStates.Help)
            {
                drawBackground(spriteBatch);

                helpManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.Settings)
            {
                drawBackground(spriteBatch);

                settingsManager.Draw(spriteBatch);
            }

            if (gameState == GameStates.Paused)
            {
                drawBackground(spriteBatch);

                powerUpManager.Draw(spriteBatch);

                playerManager.Draw(spriteBatch);

                enemyManager.Draw(spriteBatch);

                bossManager.Draw(spriteBatch);

                EffectManager.Draw(spriteBatch);

                // Pause title

                spriteBatch.Draw(spriteSheet,
                                 new Rectangle(0, 0, 800, 480),
                                 new Rectangle(0, 350, 1, 1),
                                 new Color(0, 0, 0, 150));

                spriteBatch.Draw(menuSheet,
                                 new Vector2(150.0f, 150.0f),
                                 new Rectangle(0, 100,
                                               500,
                                               100),
                                 Color.White * (0.25f + (float)(Math.Pow(Math.Sin(gameTime.TotalGameTime.TotalSeconds), 2.0f)) * 0.75f));

                spriteBatch.Draw(menuSheet,
                                 cancelDestination,
                                 cancelSource,
                                 Color.Red);

                spriteBatch.Draw(menuSheet,
                                 continueDestination,
                                 continueSource,
                                 Color.Red);
            }

            if (gameState == GameStates.Playing ||
                gameState == GameStates.PlayerDead ||
                gameState == GameStates.GameOver ||
                gameState == GameStates.BossDuell)
            {
                drawBackground(spriteBatch);

                powerUpManager.Draw(spriteBatch);

                playerManager.Draw(spriteBatch);

                enemyManager.Draw(spriteBatch);

                bossManager.Draw(spriteBatch);

                EffectManager.Draw(spriteBatch);

                zoomTextManager.Draw(spriteBatch);

                hud.Draw(spriteBatch);
            }

            // Advertisment stuff
            if (gameState == GameStates.Help ||
                gameState == GameStates.MainMenu ||
                gameState == GameStates.Paused ||
                gameState == GameStates.Settings ||
                gameState == GameStates.TitleScreen ||
                gameState == GameStates.Submittion)
            {
                adGameComponent.Draw(gameTime);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Helper method to reduce update redundace.
        /// </summary>
        private void updateBackground(GameTime gameTime)
        {
            starFieldManager1.Update(gameTime);
            starFieldManager2.Update(gameTime);
            starFieldManager3.Update(gameTime);

            asteroidManager.Update(gameTime);
        }

        /// <summary>
        /// Helper method to reduce draw redundace.
        /// </summary>
        private void drawBackground(SpriteBatch spriteBatch)
        {
            starFieldManager1.Draw(spriteBatch);
            starFieldManager2.Draw(spriteBatch);
            starFieldManager3.Draw(spriteBatch);

            asteroidManager.Draw(spriteBatch);
        }

        private void resetRound()
        {
            asteroidManager.Reset();
            enemyManager.Reset();
            bossManager.Reset();
            playerManager.Reset();
            EffectManager.Reset();
            powerUpManager.Reset();

            zoomTextManager.Reset();
            zoomTextManager.ShowText(GetReadyText[rand.Next(GetReadyText.Length)]);
        }

        private void resetGame()
        {
            resetRound();

            levelManager.Reset();

            playerManager.ResetPlayerScore();
            playerManager.ResetRemainingLives();
            playerManager.ResetSpecialWeapons();

            bossDirectKill = true;
            bossBonusScore = InitialBossBonusScore;

            GC.Collect();
        }

        private void keyboardCallback(IAsyncResult result)
        {
            string name = Guide.EndShowKeyboardInput(result);

            name = Highscore.CheckedName(name);

            if (!string.IsNullOrEmpty(name))
            {
                submissionManager.SetUp(name,
                                        playerManager.PlayerScore,
                                        levelManager.CurrentLevel);

                
                highscoreManager.SaveHighScore(name,
                                               playerManager.PlayerScore,
                                               levelManager.CurrentLevel);
                
            }
            else
            {
                gameState = GameStates.MainMenu;
            }
            
            highscoreMessageShown = false;
        }

        /// <summary>
        /// Loads the current version from assembly.
        /// </summary>
        private void loadVersion()
        {
            System.Reflection.AssemblyName an = new System.Reflection.AssemblyName(System.Reflection.Assembly
                                                                                   .GetExecutingAssembly()
                                                                                   .FullName);
            this.VersionText = new StringBuilder().Append("v ")
                                                  .Append(an.Version.Major)
                                                  .Append('.')
                                                  .Append(an.Version.Minor)
                                                  .ToString();
        }

        /// <summary>
        /// Displays the GUID for name input.
        /// </summary>
        private void showGuid()
        {
            int rank = highscoreManager.GetRank(playerManager.PlayerScore);

            string title = KeyboardTitleTextVeryBad;

            if (playerManager.PlayerScore > 10000)
                title = KeyboardTitleTextBad;

            if (playerManager.PlayerScore > 50000)
                title = KeyboardTitleTextVeryLow;

            if (playerManager.PlayerScore > 100000)
                title = KeyboardTitleTextLow;

            if (playerManager.PlayerScore > 250000)
                title = KeyboardTitleTextMed;

            if (playerManager.PlayerScore > 500000)
                title = KeyboardTitleTextHigh;

            if (playerManager.PlayerScore > 1000000)
                title = KeyboardTitleTextVeryHigh;

            if (playerManager.PlayerScore > 2500000)
                title = KeyboardTitleTextUltra;

            if (playerManager.PlayerScore > 5000000)
                title = KeyboardTitleTextUltraPlus;

            if (playerManager.PlayerScore > 10000000)
                title = KeyboardTitleTextGodlike;

            string text;

            if (highscoreManager.IsInScoreboard(playerManager.PlayerScore))
            {
                text = string.Format(KeyboardInLocalMessageFormatText, rank);
            }
            else
            {
                text = KeyboardNotInLocalMessageFormatText;
            }

            Guide.BeginShowKeyboardInput(PlayerIndex.One,
                                         title,
                                         text,
                                         highscoreManager.LastName,
                                         keyboardCallback,
                                         null);
        }
    }
}
