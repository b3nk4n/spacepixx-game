using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class InstructionManager
    {
        #region Members

        private float progressTimer = 0.0f;

        public enum InstructionStates { Welcome, Movement, SingleShot, DoubleShot, TripleShot, SpecialShot, Overheat, BewareAsteroid, KillEnemies, GoodLuck, ReturnWithBackButton};

        private InstructionStates state = InstructionStates.Welcome;

        private const float WelcomeLimit = 3.0f;
        private const float MovementLimit = 8.0f;
        private const float SingleShotLimit = 11.0f;
        private const float DoubleShotLimit = 14.0f;
        private const float TripleShotLimit = 17.0f;
        private const float SpecielShotLimit = 20.0f;
        private const float OverheatLimit = 25.0f;
        private const float BewareAsteroidsLimit = 30.0f;
        private const float KillEnemiesLimit = 35.0f;

        private SpriteFont font;

        private Texture2D texture;

        private Rectangle screenBounds;

        private Rectangle areaSource = new Rectangle(0, 350, 50, 50);

        private Rectangle singleShotDestination = new Rectangle(10, 260, 380, 210);
        private Rectangle doubleShotDestination = new Rectangle(410, 260, 380, 210);
        private Rectangle specialShotDestination = new Rectangle(10, 10, 780, 150);
        private Rectangle overheatDestination = new Rectangle(590, 35, 210, 25);

        private Color areaTint = new Color(0.2f, 0.0f, 0.0f, 0.01f);

        private AsteroidManager asteroidManager;

        private EnemyManager enemyManager;

        private PlayerManager playerManager;

        private readonly string WelcomeText = "Welcome to SpacepiXX!";
        private readonly string MovementText = "Move your ship using the accelerometer";
        private readonly string SingleShotText = "Press here for single shot...";
        private readonly string DoubleShotText = "Press here for double shot...";
        private readonly string TripleShotText = "And both for triple shot";
        private readonly string SpecialShotText = "Press here to fire your special gun!";
        private readonly string OverheatText = "Avoid Overheating!";
        private readonly string BewareAsteroidsText = "Beware flying asteroids!";
        private readonly string KillEnemiesText = "Kill enemies to score!";
        private readonly string GoodLuckText = "Good luck commander!";
        private readonly string ReturnWithBackButtonText = "Return to main menu by pressing the Back-Button";

        #endregion

        #region Constructors

        public InstructionManager(Texture2D texture, SpriteFont font, Rectangle screenBounds,
                                  AsteroidManager asteroidManager, PlayerManager playerManager,
                                  EnemyManager enemyManager)
        {
            this.texture = texture;
            this.font = font;
            this.screenBounds = screenBounds;

            this.asteroidManager = asteroidManager;
            this.asteroidManager.Reset();

            this.enemyManager = enemyManager;
            this.enemyManager.Reset();

            this.playerManager = playerManager;
            this.playerManager.Reset();
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

                enemyManager.Update(gameTime);
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
            else if (progressTimer < SpecielShotLimit)
            {
                this.state = InstructionStates.SpecialShot;
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
            }
            else
            {
                this.state = InstructionStates.GoodLuck;

                asteroidManager.Update(gameTime);
                enemyManager.Update(gameTime);
            }

            playerManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch(this.state)
            {
                case InstructionStates.Welcome:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawString(font,
                                           WelcomeText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(WelcomeText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(WelcomeText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.Movement:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawString(font,
                                           MovementText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(MovementText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(MovementText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.SingleShot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     singleShotDestination,
                                     areaSource,
                                     areaTint);
                    spriteBatch.DrawString(font,
                                           SingleShotText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(SingleShotText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(SingleShotText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.DoubleShot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     doubleShotDestination,
                                     areaSource,
                                     areaTint);
                    spriteBatch.DrawString(font,
                                           DoubleShotText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(DoubleShotText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(DoubleShotText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.TripleShot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     singleShotDestination,
                                     areaSource,
                                     areaTint);
                    spriteBatch.Draw(texture,
                                     doubleShotDestination,
                                     areaSource,
                                     areaTint);
                    spriteBatch.DrawString(font,
                                           TripleShotText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(TripleShotText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(TripleShotText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.SpecialShot:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     specialShotDestination,
                                     areaSource,
                                     areaTint);
                    spriteBatch.DrawString(font,
                                           SpecialShotText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(SpecialShotText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(SpecialShotText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.Overheat:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     overheatDestination,
                                     areaSource,
                                     areaTint);
                    spriteBatch.DrawString(font,
                                           OverheatText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(OverheatText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(OverheatText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.BewareAsteroid:
                    asteroidManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);
                    
                    spriteBatch.DrawString(font,
                                           BewareAsteroidsText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(BewareAsteroidsText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(BewareAsteroidsText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.KillEnemies:
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawString(font,
                                           KillEnemiesText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(KillEnemiesText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(KillEnemiesText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.GoodLuck:
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    spriteBatch.DrawString(font,
                                           GoodLuckText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(GoodLuckText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(GoodLuckText).Y / 2),
                                           Color.Red);
                    break;

                case InstructionStates.ReturnWithBackButton:
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);

                    spriteBatch.DrawString(font,
                                           ReturnWithBackButtonText,
                                           new Vector2(screenBounds.Width / 2 - font.MeasureString(ReturnWithBackButtonText).X / 2,
                                                       screenBounds.Height / 2 - font.MeasureString(ReturnWithBackButtonText).Y / 2),
                                           Color.Red);
                    break;
            }
        }

        public void Reset()
        {
            this.progressTimer = 0.0f;
        }

        #endregion
    }
}
