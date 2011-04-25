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
        private int specialShots;

        private Rectangle screenBounds;
        private Texture2D texture;
        private SpriteFont font;

        private Vector2 scoreLocation = new Vector2(5, 5);
        private Vector2 livesLocation = new Vector2(5, 30);
        private Vector2 specialShotsLocation = new Vector2(5, 55);
        private Vector2 hitPointLocation = new Vector2(645, 17);
        private Vector2 overheatLocation = new Vector2(645, 42);
        private Vector2 hitPointTextLocation = new Vector2(600, 10);
        private Vector2 overheatTextLocation = new Vector2(600, 35);
        
        private readonly Rectangle SpecialShotsDestination = new Rectangle(7, 57, 24, 24);
        private readonly Rectangle SpecialShotsSource = new Rectangle(0, 430, 5, 5);

        Vector2 barOverlayStart = new Vector2(0, 350);

        #endregion

        #region Constructors

        private Hud(Rectangle screen, Texture2D texture, SpriteFont font,
                    long score, int lives, float overHeat, float hitPoints, int specialShots)
        {
            this.screenBounds = screen;
            this.texture = texture;
            this.font = font;
            this.score = score;
            this.remainingLives = lives;
            this.overHeat = overHeat;
            this.hitPoints = hitPoints;
            this.specialShots = specialShots;
        }

        #endregion

        #region Methods

        public static Hud GetInstance(Rectangle screen, Texture2D texture, SpriteFont font, long score,
                                      int lives, float overHeat, float hitPoints, int specialShots)
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
                              specialShots);
            }

            return hud;
        }

        public void Update(long score, int lives, float overHeat, float hitPoints, int specialShots)
        {
            this.score = score;
            this.remainingLives = lives;
            this.overHeat = overHeat;
            this.hitPoints = hitPoints;
            this.specialShots = specialShots;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            drawScore(spriteBatch);

            if (remainingLives >= 0)
            {
                drawOverheat(spriteBatch);
                drawHitPoints(spriteBatch);
                drawRemainingLives(spriteBatch);
                drawSpecialShots(spriteBatch);
            }       
        }

        private void drawScore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   string.Format("{0:00000000}", score),
                                   scoreLocation,
                                   Color.White * 0.8f);
        }

        private void drawSpecialShots(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             SpecialShotsDestination,
                             SpecialShotsSource,
                             new Color(75, 75, 255) * 0.8f);

            string remainingSpecialShots = specialShots.ToString();

            spriteBatch.DrawString(font,
                                   remainingSpecialShots,
                                   new Vector2(SpecialShotsDestination.X + (SpecialShotsDestination.Width / 2) - (font.MeasureString(remainingSpecialShots).X / 2),
                                               SpecialShotsDestination.Y + (SpecialShotsDestination.Height / 2) - (font.MeasureString(remainingSpecialShots).Y / 2)),
                                   Color.White * 0.8f);
        }

        private void drawOverheat(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   "OH:",
                                   overheatTextLocation,
                                   Color.White * 0.8f);

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

        private void drawHitPoints(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   "HP:",
                                   hitPointTextLocation,
                                   Color.White * 0.8f);

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

        private void drawRemainingLives(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < remainingLives; x++)
            {
                spriteBatch.Draw(texture,
                                 new Rectangle((int)livesLocation.X + (x * 25),
                                               (int)livesLocation.Y,
                                               25,
                                               25),
                                 new Rectangle(0, 400, 25, 25),
                                 Color.White * 0.8f);              
            }
        }

        #endregion
    }
}
