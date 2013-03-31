using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacepiXX.Inputs;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Phone.Tasks;

namespace SpacepiXX
{
    class HelpManager
    {
        #region Members

        private Texture2D texture;
        private SpriteFont font;
        private readonly Rectangle HelpTitleSource = new Rectangle(0, 350,
                                                                   300, 50);
        private readonly Vector2 TitlePosition = new Vector2(250.0f, 100.0f);

        private static readonly string[] Content = {"If you have any further questions,",
                                            "ideas or problems with SpacepiXX,",
                                            "please do not hesitate to contact us."};

        private const string Email = "apps@bsautermeister.de";
        private const string EmailSubject = "SpacepiXX - Support";
        private const string Blog = "bsautermeister.de";

        private readonly Rectangle screenBounds;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private WebBrowserTask browser;
        private const string BROWSER_URL = "http://bsautermeister.de";

        private readonly Rectangle EmailDestination = new Rectangle(250,330,
                                                                    300,50);
        private readonly Rectangle BlogDestination = new Rectangle(250, 380,
                                                                    300, 50);

        public static GameInput GameInput;
        private const string EmailAction = "Email";
        private const string BlogAction = "Blog";

        #endregion

        #region Constructors

        public HelpManager(Texture2D tex, SpriteFont font, Rectangle screenBounds)
        {
            this.browser = new WebBrowserTask();
            this.browser.Uri = new Uri(BROWSER_URL);

            this.texture = tex;
            this.font = font;
            this.screenBounds = screenBounds;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(EmailAction,
                                           GestureType.Tap,
                                           EmailDestination);
            GameInput.AddTouchGestureInput(BlogAction,
                                           GestureType.Tap,
                                           BlogDestination);
        }

        private void handleTouchInputs()
        {
            // Email
            if (GameInput.IsPressed(EmailAction))
            {
                EmailComposeTask emailTask = new EmailComposeTask();
                emailTask.To = Email;
                emailTask.Subject = EmailSubject;
                emailTask.Show();
            }
            // Blog
            if (GameInput.IsPressed(BlogAction))
            {
                browser.Show();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             TitlePosition,
                             HelpTitleSource,
                             Color.White * opacity);

            for (int i = 0; i < Content.Length; ++i)
            {
                spriteBatch.DrawString(font,
                       Content[i],
                       new Vector2((screenBounds.Width - font.MeasureString(Content[i]).X) / 2,
                                   190 + (i * 35)),
                       Color.Red * opacity);
            }

            spriteBatch.DrawString(font,
                       Email,
                       new Vector2((screenBounds.Width - font.MeasureString(Email).X) / 2,
                                   340),
                       Color.Red * opacity);

            spriteBatch.DrawString(font,
                       Blog,
                       new Vector2((screenBounds.Width - font.MeasureString(Blog).X) / 2,
                                   390),
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
