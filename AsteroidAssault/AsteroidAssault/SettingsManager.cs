using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO.IsolatedStorage;
using System.IO;
using SpacepiXX.Inputs;

namespace SpacepiXX
{
    class SettingsManager
    {
        #region Members

        private static SettingsManager settingsManager;

        private static Texture2D texture;
        private static SpriteFont font;
        private readonly Rectangle SettingsTitleSource = new Rectangle(0, 400,
                                                                       300, 50);
        private readonly Vector2 TitlePosition = new Vector2(250.0f, 100.0f);

        public enum SoundValues {Off, VeryLow, Low, Med, High, VeryHigh};
        public enum VibrationValues { On, Off };
        public enum NeutralPositionValues { Angle0, Angle15, Angle30, Angle45 };

        private const string MUSIC_TITLE = "Music: ";
        private SoundValues musicValue = SoundValues.Med;
        private readonly int musicPositionY = 180;
        private readonly Rectangle musicDestination = new Rectangle(250, 175,
                                                                    300, 50);

        private const string SFX_TITLE = "SFX: ";
        private SoundValues sfxValue = SoundValues.Med;
        private readonly int sfxPositionY = 250;
        private readonly Rectangle sfxDestination = new Rectangle(250, 245,
                                                                  300, 50);

        private const string VIBRATION_TITLE = "Vibration: ";
        private VibrationValues vibrationValue = VibrationValues.On;
        private readonly int vibrationPositionY = 320;
        private readonly Rectangle vibrationDestination = new Rectangle(250, 315,
                                                                        300, 50);

        private const string NEUTRAL_POSITION_TITLE = "Neutral Position: ";
        private NeutralPositionValues neutralPositionValue = NeutralPositionValues.Angle30;
        private readonly int neutralPositionY = 390;
        private readonly Rectangle neutralPositionDestination = new Rectangle(250, 385,
                                                                              300, 50);

        private static Rectangle screenBounds;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        public static GameInput GameInput;
        private const string MusicAction = "Music";
        private const string SfxAction = "SFX";
        private const string VibrationAction = "Vibration";
        private const string NeutralPositionAction = "NeutralPos";

        private const string ON = "ON";
        private const string OFF = "OFF";
        private const string VERY_LOW = "VERY LOW";
        private const string LOW = "LOW";
        private const string MEDIUM = "MEDIUM";
        private const string HIGH = "HIGH";
        private const string VERY_HIGH = "VERY HIGH";
        private const string LEFT = "LEFT";
        private const string RIGHT = "RIGHT";

        private const string ANGLE0 = "0   ";
        private const string ANGLE15 = "15   ";
        private const string ANGLE30 = "30   ";
        private const string ANGLE45 = "45   ";

        #endregion

        #region Constructors

        private SettingsManager()
        {
            this.Load();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(MusicAction,
                                           GestureType.Tap,
                                           musicDestination);
            GameInput.AddTouchGestureInput(SfxAction,
                                           GestureType.Tap,
                                           sfxDestination);
            GameInput.AddTouchGestureInput(VibrationAction,
                                           GestureType.Tap,
                                           vibrationDestination);
            GameInput.AddTouchGestureInput(NeutralPositionAction,
                                           GestureType.Tap,
                                           neutralPositionDestination);
        }

        public void Initialize(Texture2D tex, SpriteFont f, Rectangle screen)
        {
            texture = tex;
            font = f;
            screenBounds = screen;
        }

        public static SettingsManager GetInstance()
        {
            if (settingsManager == null)
            {
                settingsManager = new SettingsManager();
            }

            return settingsManager;
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
                             SettingsTitleSource,
                             Color.White * opacity);

            drawMusic(spriteBatch);
            drawSfx(spriteBatch);
            drawVibration(spriteBatch);
            drawNeutralPosition(spriteBatch);
        }

        private void handleTouchInputs()
        {
            // Music
            if (GameInput.IsPressed(MusicAction))
            {
                toggleMusic();
            }
            // Sfx
            if (GameInput.IsPressed(SfxAction))
            {
                toggleSfx();
            }
            // Vibration
            if (GameInput.IsPressed(VibrationAction))
            {
                toggleVibration();
            }
            // NeutralPosition
            if (GameInput.IsPressed(NeutralPositionAction))
            {
                toggleNeutralPosition();
            }
        }

        private void toggleMusic()
        {
            switch (musicValue)
            {
                case SoundValues.Off:
                    musicValue = SoundValues.VeryLow;
                    break;
                case SoundValues.VeryLow:
                    musicValue = SoundValues.Low;
                    break;
                case SoundValues.Low:
                    musicValue = SoundValues.Med;
                    break;
                case SoundValues.Med:
                    musicValue = SoundValues.High;
                    break;
                case SoundValues.High:
                    musicValue = SoundValues.VeryHigh;
                    break;
                case SoundValues.VeryHigh:
                    musicValue = SoundValues.Off;
                    break;
            }

            SoundManager.RefreshMusicVolume();
        }

        private void toggleSfx()
        {
            switch (sfxValue)
            {
                case SoundValues.Off:
                    sfxValue = SoundValues.VeryLow;
                    break;
                case SoundValues.VeryLow:
                    sfxValue = SoundValues.Low;
                    break;
                case SoundValues.Low:
                    sfxValue = SoundValues.Med;
                    break;
                case SoundValues.Med:
                    sfxValue = SoundValues.High;
                    break;
                case SoundValues.High:
                    sfxValue = SoundValues.VeryHigh;
                    break;
                case SoundValues.VeryHigh:
                    sfxValue = SoundValues.Off;
                    break;
            }

            if (sfxValue != SoundValues.Off)
                SoundManager.PlayPlayerShot();
        }

        private void toggleVibration()
        {
            switch (vibrationValue)
            {
                case VibrationValues.Off:
                    vibrationValue = VibrationValues.On;
                    break;
                case VibrationValues.On:
                    vibrationValue = VibrationValues.Off;
                    break;
            }

            if (vibrationValue == VibrationValues.On)
                VibrationManager.Vibrate(0.2f);
        }

        private void toggleNeutralPosition()
        {
            switch (neutralPositionValue)
            {
                case NeutralPositionValues.Angle0:
                    neutralPositionValue = NeutralPositionValues.Angle15;
                    break;
                case NeutralPositionValues.Angle15:
                    neutralPositionValue = NeutralPositionValues.Angle30;
                    break;
                case NeutralPositionValues.Angle30:
                    neutralPositionValue = NeutralPositionValues.Angle45;
                    break;
                case NeutralPositionValues.Angle45:
                    neutralPositionValue = NeutralPositionValues.Angle0;
                    break;
                default:
                    break;
            }
        }

        private void drawMusic(SpriteBatch spriteBatch)
        {
            string text;

            switch (musicValue)
            {
                case SoundValues.VeryLow:
                    text = VERY_LOW;
                    break;
                case SoundValues.Low:
                    text = LOW;
                    break;
                case SoundValues.Med:
                    text = MEDIUM;
                    break;
                case SoundValues.High:
                    text = HIGH;
                    break;
                case SoundValues.VeryHigh:
                    text = VERY_HIGH;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   MUSIC_TITLE,
                                   new Vector2(250,
                                               musicPositionY),
                                   Color.Red * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((550 - font.MeasureString(text).X),
                                               musicPositionY),
                                   Color.Red * opacity);
        }

        private void drawSfx(SpriteBatch spriteBatch)
        {
            string text;

            switch (sfxValue)
            {
                case SoundValues.VeryLow:
                    text = VERY_LOW;
                    break;
                case SoundValues.Low:
                    text = LOW;
                    break;
                case SoundValues.Med:
                    text = MEDIUM;
                    break;
                case SoundValues.High:
                    text = HIGH;
                    break;
                case SoundValues.VeryHigh:
                    text = VERY_HIGH;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   SFX_TITLE,
                                   new Vector2(250,
                                               sfxPositionY),
                                   Color.Red * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((550 - font.MeasureString(text).X),
                                               sfxPositionY),
                                   Color.Red * opacity);
        }

        private void drawVibration(SpriteBatch spriteBatch)
        {
            string text;

            switch (vibrationValue)
            {
                case VibrationValues.On:
                    text = ON;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   VIBRATION_TITLE,
                                   new Vector2(250,
                                               vibrationPositionY),
                                   Color.Red * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((550 - font.MeasureString(text).X),
                                               vibrationPositionY),
                                   Color.Red * opacity);
        }

        private void drawNeutralPosition(SpriteBatch spriteBatch)
        {
            string text;

            switch (neutralPositionValue)
            {
                case NeutralPositionValues.Angle0:
                    text = ANGLE0;
                    break;
                case NeutralPositionValues.Angle15:
                    text = ANGLE15;
                    break;
                case NeutralPositionValues.Angle30:
                    text = ANGLE30;
                    break;
                default:
                    text = ANGLE45;
                    break;
            }

            spriteBatch.DrawString(font,
                                   NEUTRAL_POSITION_TITLE,
                                   new Vector2(250,
                                               neutralPositionY),
                                   Color.Red * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((550 - font.MeasureString(text).X),
                                               neutralPositionY),
                                   Color.Red * opacity);
            spriteBatch.DrawString(font,
                                   "o",
                                   new Vector2((550 - font.MeasureString("o").X),
                                               neutralPositionY - 10),
                                   Color.Red * opacity);
        }

        #endregion

        #region Load/Save

        public void Save()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("settings.txt", FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(this.musicValue);
                        sw.WriteLine(this.sfxValue);
                        sw.WriteLine(this.vibrationValue);
                        sw.WriteLine(this.neutralPositionValue);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        public void Load()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(@"settings.txt");

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(@"settings.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            this.musicValue = (SoundValues)Enum.Parse(musicValue.GetType(), sr.ReadLine(), true);
                            this.sfxValue = (SoundValues)Enum.Parse(sfxValue.GetType(), sr.ReadLine(), true);
                            this.vibrationValue = (VibrationValues)Enum.Parse(vibrationValue.GetType(), sr.ReadLine(), true);
                            this.neutralPositionValue = (NeutralPositionValues)Enum.Parse(neutralPositionValue.GetType(), sr.ReadLine(), true);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(this.musicValue);
                            sw.WriteLine(this.sfxValue);
                            sw.WriteLine(this.vibrationValue);
                            sw.WriteLine(this.neutralPositionValue);

                            // ... ? 
                        }
                    }
                }
            }
        }

        public float GetMusicValue()
        {
            switch (settingsManager.musicValue)
            {
                case SoundValues.Off:
                    return 0.0f;

                case SoundValues.VeryLow:
                    return 0.1f;

                case SoundValues.Low:
                    return 0.2f;

                case SoundValues.Med:
                    return 0.3f;

                case SoundValues.High:
                    return 0.4f;

                case SoundValues.VeryHigh:
                    return 0.5f;

                default:
                    return 0.3f;
            }
        }

        public float GetSfxValue()
        {
            switch (settingsManager.sfxValue)
            {
                case SoundValues.Off:
                    return 0.0f;

                case SoundValues.VeryLow:
                    return 0.2f;

                case SoundValues.Low:
                    return 0.4f;

                case SoundValues.Med:
                    return 0.6f;

                case SoundValues.High:
                    return 0.8f;

                case SoundValues.VeryHigh:
                    return 1.0f;

                default:
                    return 0.6f;
            }
        }

        public bool GetVabrationValue()
        {
            switch (settingsManager.vibrationValue)
            {
                case VibrationValues.On:
                    return true;

                case VibrationValues.Off:
                    return false;

                default:
                    return true;
            }
        }

        public float GetNeutralPosition()
        {
            switch (settingsManager.neutralPositionValue)
            {
                case NeutralPositionValues.Angle0:
                    return 0.0f;

                case NeutralPositionValues.Angle15:
                    return (float)Math.PI * 15.0f / 180.0f;

                case NeutralPositionValues.Angle30:
                    return (float)Math.PI * 30.0f / 180.0f;;

                case NeutralPositionValues.Angle45:
                    return (float)Math.PI * 45.0f / 180.0f;;

                default:
                    return 0.0f;
            }
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
                    Save();
                }
            }
        }

        #endregion
    }
}
