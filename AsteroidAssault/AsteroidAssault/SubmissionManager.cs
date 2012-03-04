using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using SpacepiXX.Inputs;
using SpacepiXX.Extensions;

namespace SpacepiXX
{
    class SubmissionManager
    {
        #region Members

        LeaderboardManager leaderboardManager;

        private readonly Rectangle submitSource = new Rectangle(0, 700, 
                                                                300, 50);
        private readonly Rectangle submitDestination = new Rectangle(50, 370,
                                                                     300, 50);

        private readonly Rectangle cancelSource = new Rectangle(0, 750,
                                                                300, 50);
        private readonly Rectangle cancelDestination = new Rectangle(450, 370,
                                                                     300, 50);

        private readonly Rectangle continueSource = new Rectangle(0, 850,
                                                                  300, 50);
        private readonly Rectangle continueDestination = new Rectangle(250, 370,
                                                                       300, 50);

        private static SubmissionManager submissionManager;

        public const int MaxScores = 10;

        public static Texture2D Texture;
        public static SpriteFont Font;
        private readonly Rectangle TitleSource = new Rectangle(0, 600,
                                                               500, 100);
        private readonly Vector2 TitlePosition = new Vector2(150.0f, 80.0f);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private string name = string.Empty;
        private long score;
        private int level;

        private bool cancelClicked = false;
        private bool continueClicked = false;


        private const string TEXT_SUBMIT = "You have now the ability to submit your score!";
        private const string TEXT_NAME = "Name:";
        private const string TEXT_SCORE = "Score:";
        private const string TEXT_LEVEL = "Level:";

        private enum SubmitState { Submit, Submitted };
        private SubmitState submitState = SubmitState.Submit;

        public static GameInput GameInput;
        private const string SubmitAction = "Submit";
        private const string CancelAction = "Cancel";
        private const string ContinueAction = "Continue";

        #endregion

        #region Constructors

        private SubmissionManager()
        {
            leaderboardManager = LeaderboardManager.GetInstance();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(SubmitAction,
                                           GestureType.Tap,
                                           submitDestination);
            GameInput.AddTouchGestureInput(CancelAction,
                                           GestureType.Tap,
                                           cancelDestination);
            GameInput.AddTouchGestureInput(ContinueAction,
                                           GestureType.Tap,
                                           continueDestination);
        }

        public static SubmissionManager GetInstance()
        {
            if (submissionManager == null)
            {
                submissionManager = new SubmissionManager();
            }

            return submissionManager;
        }

        private void handleTouchInputs()
        {
            // Submit
            if (GameInput.IsPressed(SubmitAction))
            {
                if (submitState == SubmitState.Submit)
                {
                    leaderboardManager.Submit(LeaderboardManager.SUBMIT,
                                              name,
                                              score,
                                              level);
                    submitState = SubmitState.Submitted;
                }
            }
            // Cancel
            if (GameInput.IsPressed(CancelAction))
            {
                if (submitState == SubmitState.Submit)
                {
                    leaderboardManager.StatusText = LeaderboardManager.TEXT_NONE;
                    cancelClicked = true;
                }
            }
            // Cancel
            if (GameInput.IsPressed(ContinueAction))
            {
                if (submitState == SubmitState.Submitted)
                {
                    cancelClicked = true;
                }
            }
        }

        public void SetUp(string name, long score, int level)
        {
            this.name = name;
            this.score = score;
            this.level = level;
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
            if (submitState == SubmitState.Submit)
            {
                spriteBatch.Draw(Texture,
                                 submitDestination,
                                 submitSource,
                                 Color.Red * opacity);

                spriteBatch.Draw(Texture,
                                 cancelDestination,
                                 cancelSource,
                                 Color.Red * opacity);
            }
            else if (submitState == SubmitState.Submitted)
            {
                spriteBatch.Draw(Texture,
                                 continueDestination,
                                 continueSource,
                                 Color.Red * opacity);

                spriteBatch.DrawString(Font,
                                   leaderboardManager.StatusText,
                                   new Vector2(800 / 2 - Font.MeasureString(leaderboardManager.StatusText).X / 2,
                                               440),
                                   Color.Red * opacity);

            }

            spriteBatch.DrawString(Font,
                                   TEXT_SUBMIT,
                                   new Vector2(800 / 2 - Font.MeasureString(TEXT_SUBMIT).X / 2,
                                               180),
                                   Color.Red * opacity);

            // Title:
            spriteBatch.DrawString(Font,
                                   TEXT_NAME,
                                   new Vector2(300,
                                               230),
                                   Color.Red * opacity);

            spriteBatch.DrawString(Font,
                                   TEXT_SCORE,
                                   new Vector2(300,
                                               270),
                                   Color.Red * opacity);

            spriteBatch.DrawString(Font,
                                   TEXT_LEVEL,
                                   new Vector2(300,
                                               310),
                                   Color.Red * opacity);

            // Content:
            spriteBatch.DrawString(Font,
                                   name,
                                   new Vector2(450,
                                               230),
                                   Color.Red * opacity);

            spriteBatch.DrawInt64(Font,
                                  score,
                                  new Vector2(450,
                                              270),
                                  Color.Red * opacity);

            spriteBatch.DrawInt64(Font,
                                  level,
                                  new Vector2(450,
                                              310),
                                  Color.Red * opacity);

            spriteBatch.Draw(Texture,
                             TitlePosition,
                             TitleSource,
                             Color.White * opacity);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
            this.name = reader.ReadLine();
            this.score = Int64.Parse(reader.ReadLine());
            this.level = Int32.Parse(reader.ReadLine());
            this.submitState = (SubmitState)Enum.Parse(submitState.GetType(), reader.ReadLine(), false);
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
            writer.WriteLine(name);
            writer.WriteLine(score);
            writer.WriteLine(level);
            writer.WriteLine(submitState);
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
                    this.continueClicked = false;
                    this.cancelClicked = false;
                    this.submitState = SubmitState.Submit;
                }
            }
        }

        public bool CancelClicked
        {
            get
            {
                return this.cancelClicked;
            }
        }

        public bool ContinueClicked
        {
            get
            {
                return this.continueClicked;
            }
        }

        #endregion
    }
}
