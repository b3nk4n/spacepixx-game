using Microsoft.Phone.Applications.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using SpacepiXX.Inputs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SpacepiXX
{
    class PhonePositionManager
    {
        #region Members

        private readonly Rectangle cancelSource = new Rectangle(0, 750,
                                                                300, 50);
        private readonly Rectangle cancelDestination = new Rectangle(450, 370,
                                                                     300, 50);

        private readonly Rectangle goSource = new Rectangle(0, 1150,
                                                            240, 50);
        private readonly Rectangle goDestination = new Rectangle(50, 370,
                                                                    300, 50);

        private static PhonePositionManager phonePositionManager;

        public static Texture2D Texture;
        public static SpriteFont Font;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private bool cancelClicked = false;
        private bool startClicked = false;

        private readonly string TEXT_PHONEPOSITION = "Hold your phone to the desired position:";

        public static GameInput GameInput;
        private const string CancelAction = "CancelStart";
        private const string GoAction = "GoGame";

        private readonly Rectangle[] PhoneSource = {new Rectangle(500, 0, 300, 200),
                                                    new Rectangle(800, 0, 300, 200),
                                                    new Rectangle(500, 200, 300, 200),
                                                     new Rectangle(800, 200, 300, 200),
                                                    new Rectangle(500, 400, 300, 200),
                                                   new Rectangle(800, 400, 300, 200),
                                                   new Rectangle(500, 600, 300, 200)};
        private readonly Rectangle PhoneUnknownSource = new Rectangle(800, 600, 300, 200);

        private readonly Rectangle PhoneSelectedDestination = new Rectangle(250, 170, 300, 200);

        private readonly Rectangle PhoneBehind1Destination = new Rectangle(295, 140, 210, 140);
        private readonly Rectangle PhoneBehind2Destination = new Rectangle(313, 120, 174, 116);
        private readonly Rectangle PhoneBehind3Destination = new Rectangle(331, 100, 138, 92);
        private readonly Rectangle PhoneBehind4Destination = new Rectangle(349, 80, 102, 68);

        private readonly SettingsManager settingsManager;

        private bool canStart;

        #endregion

        #region Constructors

        private PhonePositionManager()
        {
            settingsManager = SettingsManager.GetInstance();

            AccelerometerHelper.Instance.ReadingChanged += new EventHandler<AccelerometerHelperReadingEventArgs>(OnAccelerometerHelperReadingChanged);
            AccelerometerHelper.Instance.Active = true;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(CancelAction,
                                           GestureType.Tap,
                                           cancelDestination);

            GameInput.AddTouchGestureInput(GoAction,
                                           GestureType.Tap,
                                           goDestination);
            GameInput.AddTouchGestureInput(GoAction,
                                           GestureType.Tap,
                                           PhoneSelectedDestination);
        }

        public static PhonePositionManager GetInstance()
        {
            if (phonePositionManager == null)
            {
                phonePositionManager = new PhonePositionManager();
            }

            return phonePositionManager;
        }

        private void handleTouchInputs()
        {
            // Cancel
            if (GameInput.IsPressed(CancelAction))
            {
                cancelClicked = true;
            }

            // Start
            if (canStart && GameInput.IsPressed(GoAction))
            {
                startClicked = true;
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isActive)
            {
                if (settingsManager.GetNeutralPositionIndex() >= 0)
                {
                    canStart = true;
                }
                else
                {
                    canStart = false;
                }

                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;

                handleTouchInputs();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                                 cancelDestination,
                                 cancelSource,
                                 Color.Red * opacity);

            Color startColor;

            if (canStart)
            {
                startColor = Color.Red * opacity;
            }
            else
            {
                startColor = Color.Red * opacity * 0.5f;
            }

            spriteBatch.Draw(Texture,
                                 goDestination,
                                 goSource,
                                 startColor);

            spriteBatch.DrawString(Font,
                                TEXT_PHONEPOSITION,
                                new Vector2(400 - Font.MeasureString(TEXT_PHONEPOSITION).X / 2,
                                            50),
                                Color.Red * opacity);

            // Phones:
            int phoneIndex = settingsManager.GetNeutralPositionIndex();
            int rightIndex = phoneIndex + 1;
            int right2Index = phoneIndex + 2;
            int right3Index = phoneIndex + 3;
            int right4Index = phoneIndex + 4;

            if (phoneIndex >= 0 && phoneIndex < PhoneSource.Length)
            {
                if (right4Index >= 0 && right4Index < PhoneSource.Length)
                    spriteBatch.Draw(
                        Texture,
                        PhoneBehind4Destination,
                        PhoneSource[right4Index],
                        Color.Red * 0.1f);

                if (right3Index >= 0 && right3Index < PhoneSource.Length)
                    spriteBatch.Draw(
                        Texture,
                        PhoneBehind3Destination,
                        PhoneSource[right3Index],
                        Color.Red * 0.2f);

                if (right2Index >= 0 && right2Index < PhoneSource.Length)
                    spriteBatch.Draw(
                        Texture,
                        PhoneBehind2Destination,
                        PhoneSource[right2Index],
                        Color.Red * 0.3f);

                if (rightIndex >= 0 && rightIndex < PhoneSource.Length)
                    spriteBatch.Draw(
                        Texture,
                        PhoneBehind1Destination,
                        PhoneSource[rightIndex],
                        Color.Red * 0.4f);

                spriteBatch.Draw(
                    Texture,
                    PhoneSelectedDestination,
                    PhoneSource[phoneIndex],
                    Color.Red);
            }
            else
            {
                spriteBatch.Draw(
                    Texture,
                    PhoneSelectedDestination,
                    PhoneUnknownSource,
                    Color.Red);
            }
        }

        private void OnAccelerometerHelperReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            if (isActive)
            {
                Vector3 currentAccValue = new Vector3((float)e.AverageAcceleration.X,
                                          (float)e.AverageAcceleration.Y,
                                          (float)e.AverageAcceleration.Z);

                if (currentAccValue.Z > 0.001f || Math.Abs(currentAccValue.Y) > 0.5f)
                {
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Unsupported);
                    return;
                }

                float val = -(float)Math.Asin(currentAccValue.X);

                if (val >= settingsManager.GetNeutralPositionRadianValue(-10.0f) && val < settingsManager.GetNeutralPositionRadianValue(5.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle0);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(5.0f) && val < settingsManager.GetNeutralPositionRadianValue(15.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle10);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(15.0f) && val < settingsManager.GetNeutralPositionRadianValue(25.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle20);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(25.0f) && val < settingsManager.GetNeutralPositionRadianValue(35.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle30);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(35.0f) && val < settingsManager.GetNeutralPositionRadianValue(45.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle40);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(45.0f) && val < settingsManager.GetNeutralPositionRadianValue(55.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle50);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(55.0f) && val < settingsManager.GetNeutralPositionRadianValue(70.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle60);
                else
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Unsupported);
            }

        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
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
                    this.startClicked = false;
                    this.cancelClicked = false;
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

        public bool StartClicked
        {
            get
            {
                return this.startClicked;
            }
        }

        #endregion
    }
}
