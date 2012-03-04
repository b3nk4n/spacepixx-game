using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.IsolatedStorage;

namespace SpacepiXX
{
    class InstructionManager
    {
        #region Members

        private float progressTimer = 0.0f;

        public enum InstructionStates { 
            Welcome,                       
            Movement,                           
            SingleShot, 
            DoubleShot, 
            TripleShot,
            SideShot,
            SpecialShot, 
            Rocket, 
            HitPoints, 
            Shield, 
            ScoreMulti, 
            Overheat, 
            BewareAsteroid, 
            KillEnemies, 
            PowerUps, 
            KillBoss,
            BossBonus,
            GoodLuck, 
            ReturnWithBackButton};

        private InstructionStates state = InstructionStates.Welcome;

        private const float WelcomeLimit = 2.0f;
        private const float MovementLimit = 5.0f;
        private const float SingleShotLimit = 9.0f;
        private const float DoubleShotLimit = 13.0f;
        private const float TripleShotLimit = 17.0f;
        private const float SideShotLimit = 21.0f;
        private const float SpecielShotLimit = 25.0f;
        private const float RocketLimit = 29.0f;
        private const float HitPointsLimit = 32.0f;
        private const float ShieldLimit = 35.0f;
        private const float ScoreMultiLimit = 38.0f;
        private const float OverheatLimit = 41.0f;
        private const float BewareAsteroidsLimit = 44.0f;
        private const float KillEnemiesLimit = 47.0f;
        private const float PowerUpsLimit = 50.0f;
        private const float KillBossLimit = 55.0f;
        private const float BossBonusLimit = 60.0f;

        private SpriteFont font;

        private Texture2D texture;

        private Rectangle screenBounds;

        private Rectangle areaSource = new Rectangle(660, 60, 380, 150);
        private Rectangle sideAreaSource = new Rectangle(950, 210, 250, 140);
        private Rectangle arrowRightSource = new Rectangle(100, 460, 40, 20);

        private Rectangle singleShotDestination = new Rectangle(10, 320, 380, 150);
        private Rectangle doubleShotDestination = new Rectangle(410, 320, 380, 150);
        private Rectangle sideShotLeftDestination = new Rectangle(10, 170, 250, 140);
        private Rectangle sideShotRightDestination = new Rectangle(540, 170, 250, 150);
        private Rectangle specialShotDestination = new Rectangle(410, 10, 380, 150);
        private Rectangle rocketDestination = new Rectangle(10, 10, 380, 150);
        private Rectangle hitPointsDestination = new Rectangle(575, 5, 40, 20);
        private Rectangle shieldDestination = new Rectangle(575, 25, 40, 20);
        private Rectangle overheatDestination = new Rectangle(575, 45, 40, 20);
        private Rectangle scoreMultiDestination = new Rectangle(575, 65, 40, 20);

        private Color areaTint = Color.Red * 0.5f;
        private Color arrowTint = Color.Red * 0.8f;

        private AsteroidManager asteroidManager;

        private EnemyManager enemyManager;

        private PlayerManager playerManager;

        private PowerUpManager powerUpManager;

        private BossManager bossManager;

        private readonly string WelcomeText = "Welcome to SpacepiXX!";
        private readonly string MovementText = "Move your spaceship by tilting your phone";
        private readonly string SingleShotText = "Press here for single shot...";
        private readonly string DoubleShotText = "and here for double shot...";
        private readonly string TripleShotText = "Press both together for triple shot";
        private readonly string SideShotText = "Flick for side shot!";
        private readonly string SpecialShotText = "Press here to fire your Super-Laser";
        private readonly string RocketText = "And here to fire your C.A.R.L.I-Rocket";
        private readonly string HitPointsText = "The HUD display your current hit points...";
        private readonly string ShieldText = "your shield level...";
        private readonly string ScoreMultiText = "and your score multiplier";
        private readonly string OverheatText = "Avoid overheating!";
        private readonly string BewareAsteroidsText = "Beware flying asteroids!";
        private readonly string KillEnemiesText = "Kill enemies to score!";
        private readonly string PowerUpsText = "Gather powerups ... but avoid the red ones";
        private readonly string KillBossText = "Defeat the BOSS to reach the next level";
        private readonly string BossBonusText = "Kill the BOSS at the first try results in bonus score!";
        private readonly string GoodLuckText = "Good luck commander!";
        private readonly string ReturnWithBackButtonText = "Press BACK to return...";

        private static bool hasDoneInstructions = false;

        #endregion

        #region Constructors

        public InstructionManager(Texture2D texture, SpriteFont font, Rectangle screenBounds,
                                  AsteroidManager asteroidManager, PlayerManager playerManager,
                                  EnemyManager enemyManager, BossManager bossManager, PowerUpManager powerUpManager)
        {
            this.texture = texture;
            this.font = font;
            this.screenBounds = screenBounds;

            this.asteroidManager = asteroidManager;
            this.asteroidManager.Reset();

            this.enemyManager = enemyManager;
            this.enemyManager.Reset();

            this.bossManager = bossManager;
            this.bossManager.Reset();

            this.playerManager = playerManager;
            this.playerManager.Reset();

            this.powerUpManager = powerUpManager;
            this.powerUpManager.Reset();

            loadHasDoneInstructions();
        }

        #endregion

        #region Methods

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            progressTimer += elapsed;

            if (playerManager.IsDestroyed)
            {
                this.state = InstructionStates.ReturnWithBackButton;

                asteroidManager.Update(gameTime);
                powerUpManager.Update(gameTime);
                enemyManager.Update(gameTime);
                bossManager.Update(gameTime);
                return;
            }
            else if (progressTimer < WelcomeLimit)
            {
                this.state = InstructionStates.Welcome;
            }
            else if (progressTimer < MovementLimit)
            {
                this.state = InstructionStates.Movement;
            }
            else if (progressTimer < SingleShotLimit)
            {
                this.state = InstructionStates.SingleShot;
            }
            else if (progressTimer < DoubleShotLimit)
            {
                this.state = InstructionStates.DoubleShot;
            }
            else if (progressTimer < TripleShotLimit)
            {
                this.state = InstructionStates.TripleShot;
            }
            else if (progressTimer < SideShotLimit)
            {
                this.state = InstructionStates.SideShot;
            }
            else if (progressTimer < SpecielShotLimit)
            {
                this.state = InstructionStates.SpecialShot;
            }
            else if (progressTimer < RocketLimit)
            {
                this.state = InstructionStates.Rocket;
            }
            else if (progressTimer < HitPointsLimit)
            {
                this.state = InstructionStates.HitPoints;
            }
            else if (progressTimer < ShieldLimit)
            {
                this.state = InstructionStates.Shield;
            }
            else if (progressTimer < ScoreMultiLimit)
            {
                this.state = InstructionStates.ScoreMulti;
            }
            else if (progressTimer < OverheatLimit)
            {
                this.state = InstructionStates.Overheat;
            }
            else if (progressTimer < BewareAsteroidsLimit)
            {
                this.state = InstructionStates.BewareAsteroid;

                asteroidManager.Update(gameTime);

                enemyManager.Update(gameTime);
            }
            else if (progressTimer < KillEnemiesLimit)
            {
                this.state = InstructionStates.KillEnemies;

                asteroidManager.Update(gameTime);
                
                enemyManager.Update(gameTime);

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < PowerUpsLimit)
            {
                this.state = InstructionStates.PowerUps;

                asteroidManager.Update(gameTime);

                enemyManager.Update(gameTime);

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < KillBossLimit)
            {
                if (this.state != InstructionStates.KillBoss)
                {
                    bossManager.SpawnRandomBoss();
                }

                this.state = InstructionStates.KillBoss;

                asteroidManager.Update(gameTime);

                enemyManager.Update(gameTime);

                bossManager.Update(gameTime);

                powerUpManager.Update(gameTime);
            }
            else if (progressTimer < BossBonusLimit)
            {
                this.state = InstructionStates.BossBonus;

                asteroidManager.Update(gameTime);

                enemyManager.Update(gameTime);

                bossManager.Update(gameTime);

                powerUpManager.Update(gameTime);
            }
            else
            {
                this.state = InstructionStates.GoodLuck;

                asteroidManager.Update(gameTime);
                enemyManager.Update(gameTime);
                bossManager.Update(gameTime);
                powerUpManager.Update(gameTime);
            }

            playerManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch(this.state)
            {
                case InstructionStates.Welcome:
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, WelcomeText);
                    break;

                case InstructionStates.Movement:
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, MovementText);
                    break;

                case InstructionStates.SingleShot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     singleShotDestination,
                                     areaSource,
                                     areaTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f);
                    drawRedCenteredText(spriteBatch, SingleShotText);
                    break;

                case InstructionStates.DoubleShot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     doubleShotDestination,
                                     areaSource,
                                     areaTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.FlipHorizontally,
                                     0.0f);
                    drawRedCenteredText(spriteBatch, DoubleShotText);
                    break;

                case InstructionStates.TripleShot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     singleShotDestination,
                                     areaSource,
                                     areaTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f);
                    spriteBatch.Draw(texture,
                                     doubleShotDestination,
                                     areaSource,
                                     areaTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.FlipHorizontally,
                                     0.0f);
                    drawRedCenteredText(spriteBatch, TripleShotText);
                    break;

                case InstructionStates.SideShot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     sideShotLeftDestination,
                                     sideAreaSource,
                                     areaTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f);
                    spriteBatch.Draw(texture,
                                     sideShotRightDestination,
                                     sideAreaSource,
                                     areaTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.FlipHorizontally,
                                     0.0f);
                    drawRedCenteredText(spriteBatch, SideShotText);
                    break;

                case InstructionStates.SpecialShot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     specialShotDestination,
                                     areaSource,
                                     areaTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.FlipHorizontally,
                                     0.0f);
                    drawRedCenteredText(spriteBatch, SpecialShotText);
                    break;

                case InstructionStates.Rocket:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     rocketDestination,
                                     areaSource,
                                     areaTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.None,
                                     0.0f);
                    drawRedCenteredText(spriteBatch, RocketText);
                    break;

                case InstructionStates.HitPoints:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     hitPointsDestination,
                                     arrowRightSource,
                                     arrowTint);
                    drawRedCenteredText(spriteBatch, HitPointsText);
                    break;

                case InstructionStates.Shield:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     shieldDestination,
                                     arrowRightSource,
                                     arrowTint);
                    drawRedCenteredText(spriteBatch, ShieldText);
                    break;

                case InstructionStates.ScoreMulti:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     scoreMultiDestination,
                                     arrowRightSource,
                                     arrowTint);
                    drawRedCenteredText(spriteBatch, ScoreMultiText);
                    break;

                case InstructionStates.Overheat:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     overheatDestination,
                                     arrowRightSource,
                                     arrowTint);
                    drawRedCenteredText(spriteBatch, OverheatText);
                    break;

                case InstructionStates.BewareAsteroid:
                    asteroidManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, BewareAsteroidsText);
                    break;

                case InstructionStates.KillEnemies:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, KillEnemiesText);
                    break;

                case InstructionStates.PowerUps:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, PowerUpsText);
                    break;

                case InstructionStates.KillBoss:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    bossManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, KillBossText);
                    break;

                case InstructionStates.BossBonus:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    bossManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, BossBonusText);
                    break;

                case InstructionStates.GoodLuck:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    bossManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, GoodLuckText);
                    break;

                case InstructionStates.ReturnWithBackButton:
                    powerUpManager.Draw(spriteBatch);
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    bossManager.Draw(spriteBatch);

                    drawRedCenteredText(spriteBatch, ReturnWithBackButtonText);
                    break;
            }
        }

        private void drawRedCenteredText(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2(screenBounds.Width / 2 - font.MeasureString(text).X / 2,
                                               screenBounds.Height / 2 - font.MeasureString(text).Y / 2),
                                   Color.Red);
        }

        public void Reset()
        {
            this.progressTimer = 0.0f;
            this.state = InstructionStates.Welcome;
        }

        public void SaveHasDoneInstructions()
        {

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("instructions2.txt", FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(hasDoneInstructions);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        private void loadHasDoneInstructions()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(@"instructions2.txt");

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(@"instructions2.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            hasDoneInstructions = Boolean.Parse(sr.ReadLine());
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(hasDoneInstructions);

                            // ... ? 
                        }
                    }
                }

                // Delete the old file
                if (isf.FileExists(@"instructions.txt"))
                    isf.DeleteFile(@"instructions.txt");
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.progressTimer = Single.Parse(reader.ReadLine());
            hasDoneInstructions = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(progressTimer);
            writer.WriteLine(hasDoneInstructions);
        }

        #endregion

        #region Properties

        public static bool HasDoneInstructions
        {
            get
            {
                return hasDoneInstructions;
            }
            set
            {
                hasDoneInstructions = value;
            }
        }

        #endregion
    }
}
