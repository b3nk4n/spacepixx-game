using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class Hud
    {
        #region Members

        private static Hud hud;

        private long score;
        private int remainingLives;
        private float overHeat;
        private float hitPoints;
        private float shieldPoints;
        private int specialShots;
        private int rockets;
        private float scoreMulti;
        private int level;

        private Rectangle screenBounds;
        private Texture2D texture;
        private SpriteFont font;

        private Vector2 scoreLocation = new Vector2(5, 5);
        private Vector2 livesLocation = new Vector2(5, 35);
        private Vector2 hitPointLocation = new Vector2(645, 10);
        private Vector2 shieldPointLocation = new Vector2(645, 30);
        private Vector2 overheatLocation = new Vector2(645, 50);
        private Vector2 scoreMultiLocation = new Vector2(645, 70);

        private readonly Rectangle hitPointSymbolSoruce = new Rectangle(0, 320, 14, 14);
        private readonly Rectangle shieldPointSymbolSoruce = new Rectangle(14, 320, 14, 14);
        private readonly Rectangle overheatSymbolSoruce = new Rectangle(28, 320, 14, 14);
        private readonly Rectangle scoreMultiSymbolSoruce = new Rectangle(42, 320, 14, 14);

        private Vector2 hitPointSymbolLocation = new Vector2(625, 8);
        private Vector2 shieldPointSymbolLocation = new Vector2(625, 28);
        private Vector2 overheatSymbolLocation = new Vector2(625, 48);
        private Vector2 scoreMultiSymbolLocation = new Vector2(625, 68);
        
        private readonly Rectangle SpecialShotsDestination = new Rectangle(75, 57, 24, 24);
        private readonly Rectangle SpecialShotsSource = new Rectangle(900, 350, 24, 24);

        private readonly Rectangle RocketsDestination = new Rectangle(7, 57, 24, 24);
        private readonly Rectangle RocketsSource = new Rectangle(925, 350, 24, 24);

        Vector2 barOverlayStart = new Vector2(0, 350);

        #endregion

        #region Constructors

        private Hud(Rectangle screen, Texture2D texture, SpriteFont font,
                    long score, int lives, float overHeat, float hitPoints, float shieldPoints,
                    int rockets, int specialShots, float scoreMulti, int level)
        {
            this.screenBounds = screen;
            this.texture = texture;
            this.font = font;
            this.score = score;
            this.remainingLives = lives;
            this.overHeat = overHeat;
            this.hitPoints = hitPoints;
            this.shieldPoints = shieldPoints;
            this.specialShots = specialShots;
            this.rockets = rockets;
            this.scoreMulti = scoreMulti;
            this.level = level;
        }

        #endregion

        #region Methods

        public static Hud GetInstance(Rectangle screen, Texture2D texture, SpriteFont font, long score,
                                      int lives, float overHeat, float hitPoints, float shieldPoints,
                                      int specialShots, int rockets, float scoreMulti, int level)
        {
            if (hud == null)
            {
                hud = new Hud(screen,
                              texture,
                              font,
                              score,
                              lives,
                              overHeat,
                              hitPoints,
                              shieldPoints,
                              specialShots,
                              rockets,
                              scoreMulti,
                              level);
            }

            return hud;
        }

        public void Update(long score, int lives, float overHeat, float hitPoints, float shieldPoints,
                           int specialShots, int rockets, float scoreMulti, int level)
        {
            this.score = score;
            this.remainingLives = lives;
            this.overHeat = overHeat;
            this.hitPoints = hitPoints;
            this.shieldPoints = shieldPoints;
            this.specialShots = specialShots;
            this.rockets = rockets;
            this.scoreMulti = scoreMulti;
            this.level = level;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            drawScore(spriteBatch);

            if (remainingLives >= 0)
            {
                drawOverheat(spriteBatch);
                drawScoreMulti(spriteBatch);
                drawHitPoints(spriteBatch);
                drawShieldPoints(spriteBatch);
                drawRemainingLives(spriteBatch);
                drawSpecialShots(spriteBatch);
                drawRockets(spriteBatch);
                drawLevel(spriteBatch);
            }       
        }

        private void drawScore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   string.Format("{0:00000000000}", score),
                                   scoreLocation,
                                   Color.White * 0.8f);
        }

        private void drawSpecialShots(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             SpecialShotsDestination,
                             SpecialShotsSource,
                             Color.White * 0.8f);

            string remainingSpecialShots = specialShots.ToString();

            spriteBatch.DrawString(font,
                                   remainingSpecialShots,
                                   new Vector2(SpecialShotsDestination.X + 30,
                                               SpecialShotsDestination.Y + (SpecialShotsDestination.Height / 2) - (font.MeasureString(remainingSpecialShots).Y / 2)),
                                   Color.White * 0.8f);
        }

        private void drawRockets(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             RocketsDestination,
                             RocketsSource,
                             Color.White * 0.8f); //new Color(75, 75, 255) * 0.8f);

            string remainingRockets = rockets.ToString();

            spriteBatch.DrawString(font,
                                   remainingRockets,
                                   new Vector2(RocketsDestination.X + 30,
                                               RocketsDestination.Y + (RocketsDestination.Height / 2) - (font.MeasureString(remainingRockets).Y / 2)),
                                   Color.White * 0.8f);
        }

        private void drawOverheat(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             overheatSymbolLocation,
                             overheatSymbolSoruce,
                             Color.Red * 0.8f);

            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)overheatLocation.X,
                        (int)overheatLocation.Y,
                        150,
                        10),
                    new Rectangle(
                        (int)barOverlayStart.X,
                        (int)barOverlayStart.Y,
                        150,
                        10),
                    Color.Red * 0.2f);

            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)overheatLocation.X,
                        (int)overheatLocation.Y,
                        (int)(150 * overHeat),
                        10),
                    new Rectangle(
                        (int)barOverlayStart.X,
                        (int)barOverlayStart.Y,
                        (int)(150 * overHeat),
                        10),
                    Color.Red * 0.5f);
        }

        private void drawScoreMulti(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             scoreMultiSymbolLocation,
                             scoreMultiSymbolSoruce,
                             Color.Yellow * 0.8f);

            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)scoreMultiLocation.X,
                        (int)scoreMultiLocation.Y,
                        150,
                        10),
                    new Rectangle(
                        (int)barOverlayStart.X,
                        (int)barOverlayStart.Y,
                        150,
                        10),
                    Color.Yellow * 0.2f);

            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)scoreMultiLocation.X,
                        (int)scoreMultiLocation.Y,
                        (int)(150 * (scoreMulti - PlayerManager.MIN_SCORE_MULTI) / (PlayerManager.MAX_SCORE_MULTI - PlayerManager.MIN_SCORE_MULTI)),
                        10),
                    new Rectangle(
                        (int)barOverlayStart.X,
                        (int)barOverlayStart.Y,
                        (int)(150 * (scoreMulti - PlayerManager.MIN_SCORE_MULTI) / (PlayerManager.MAX_SCORE_MULTI - PlayerManager.MIN_SCORE_MULTI)),
                        10),
                    Color.Yellow * 0.5f);
        }

        private void drawHitPoints(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             hitPointSymbolLocation,
                             hitPointSymbolSoruce,
                             Color.Green * 0.8f);

            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)hitPointLocation.X,
                        (int)hitPointLocation.Y,
                        150,
                        10),
                    new Rectangle(
                        (int)barOverlayStart.X,
                        (int)barOverlayStart.Y,
                        150,
                        10),
                    Color.Green * 0.2f);

            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)hitPointLocation.X,
                        (int)hitPointLocation.Y,
                        (int)(1.5f * hitPoints),
                        10),
                    new Rectangle(
                        (int)barOverlayStart.X,
                        (int)barOverlayStart.Y,
                        (int)(1.5f * hitPoints),
                        10),
                    Color.Green * 0.5f);
        }

        private void drawShieldPoints(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             shieldPointSymbolLocation,
                             shieldPointSymbolSoruce,
                             Color.Blue * 0.8f);

            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)shieldPointLocation.X,
                        (int)shieldPointLocation.Y,
                        150,
                        10),
                    new Rectangle(
                        (int)barOverlayStart.X,
                        (int)barOverlayStart.Y,
                        150,
                        10),
                    Color.Blue * 0.2f);

            spriteBatch.Draw(texture,
                    new Rectangle(
                        (int)shieldPointLocation.X,
                        (int)shieldPointLocation.Y,
                        (int)(1.5f * shieldPoints),
                        10),
                    new Rectangle(
                        (int)barOverlayStart.X,
                        (int)barOverlayStart.Y,
                        (int)(1.5f * shieldPoints),
                        10),
                    Color.Blue * 0.5f);
        }

        private void drawRemainingLives(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < PlayerManager.MAX_PLAYER_LIVES; x++)
            {
                Color c;

                if (x < remainingLives)
                    c = Color.White * 0.8f;
                else
                    c = Color.White * 0.2f;

                spriteBatch.Draw(texture,
                                    new Rectangle((int)livesLocation.X + (x * 25),
                                                (int)livesLocation.Y,
                                                25,
                                                25),
                                    new Rectangle(0, 400, 25, 25),
                                    c);              
            }
        }

        private void drawLevel(SpriteBatch spriteBatch)
        {
            string lvlText = "Level: " + level;
            spriteBatch.DrawString(font,
                                   lvlText,
                                   new Vector2(screenBounds.Width / 2 - (font.MeasureString(lvlText).Y / 2),
                                               5),
                                   Color.White * 0.8f);
        }

        #endregion
    }
}
