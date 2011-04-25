using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    public class HelpManager
    {
        #region Members

        private Texture2D texture;
        private SpriteFont font;
        private readonly Rectangle HelpTitleDestination = new Rectangle(0, 350,
                                                                        300, 50);
        private readonly Vector2 TitlePosition = new Vector2(250.0f, 40.0f);

        private readonly string[] Content = {"If you have any further questions,",
                                            "ideas or problems with SpacepiXX,",
                                            "please do not hesitate to contact us."};

        private readonly string Email = "b.sautermeister@gmx.de";
        private readonly string Blog = "bsautermeister.blogspot.com";

        private readonly Rectangle screenBounds;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        #endregion

        #region Constructors

        public HelpManager(Texture2D tex, SpriteFont font, Rectangle screenBounds)
        {
            this.texture = tex;
            this.font = font;
            this.screenBounds = screenBounds;
        }

        #endregion

        #region Methods

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             TitlePosition,
                             HelpTitleDestination,
                             Color.White * opacity);

            for (int i = 0; i < Content.Length; ++i)
            {
                spriteBatch.DrawString(font,
                       Content[i],
                       new Vector2((screenBounds.Width - font.MeasureString(Content[i]).X) / 2,
                                   150 + (i * 35)),
                       Color.Red * opacity);
            }

            spriteBatch.DrawString(font,
                       Email,
                       new Vector2((screenBounds.Width - font.MeasureString(Email).X) / 2,
                                   325),
                       Color.Red * opacity);

            spriteBatch.DrawString(font,
                       Blog,
                       new Vector2((screenBounds.Width - font.MeasureString(Blog).X) / 2,
                                   360),
                       Color.Red * opacity);
        }

        #endregion

        #region Properties

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;

                if (isActive == false)
                {
                    this.opacity = OpacityMin;
                }
            }
        }

        #endregion
    }
}
