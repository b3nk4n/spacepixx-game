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

        private const string SETTINGS_FILE = "settings2.txt";

        private static SettingsManager settingsManager;

        private static Texture2D texture;
        private static SpriteFont font;
        private readonly Rectangle SettingsTitleSource = new Rectangle(0, 400,
                                                                       300, 50);
        private readonly Vector2 TitlePosition = new Vector2(250.0f, 100.0f);

        public enum SoundValues {Off, VeryLow, Low, Med, High, VeryHigh};
        public enum VibrationValues { On, Off };
        public enum NeutralPositionValues { Angle0, Angle10, Angle20, Angle30, Angle40, Angle50, Angle60, Unsupported };

        private const string MUSIC_TITLE = "Music: ";
        private SoundValues musicValue = SoundValues.Med;
        private readonly int musicPositionY = 200;
        private readonly Rectangle musicDestination = new Rectangle(250, 195,
                                                                    300, 50);

        private const string SFX_TITLE = "SFX: ";
        private SoundValues sfxValue = SoundValues.High;
        private readonly int sfxPositionY = 270;
        private readonly Rectangle sfxDestination = new Rectangle(250, 265,
                                                                  300, 50);

        private const string VIBRATION_TITLE = "Vibration: ";
        private VibrationValues vibrationValue = VibrationValues.On;
        private readonly int vibrationPositionY = 340;
        private readonly Rectangle vibrationDestination = new Rectangle(250, 335,
                                                                        300, 50);

        private NeutralPositionValues neutralPositionValue = NeutralPositionValues.Angle20;

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

        private const string ON = "ON";
        private const string OFF = "OFF";
        private const string VERY_LOW = "VERY LOW";
        private const string LOW = "LOW";
        private const string MEDIUM = "MEDIUM";
        private const string HIGH = "HIGH";
        private const string VERY_HIGH = "VERY HIGH";
        private const string LEFT = "LEFT";
        private const string RIGHT = "RIGHT";

        private const int TextPositonX = 250;
        private const int ValuePositionX = 550;

        private bool isInvalidated = false;

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
            isInvalidated = true;
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
            isInvalidated = true;
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
            isInvalidated = true;
            if (vibrationValue == VibrationValues.On)
                VibrationManager.Vibrate(0.2f);
        }

        public void SetNeutralPosition(NeutralPositionValues value)
        {
            this.neutralPositionValue = value;
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
                                   new Vector2(TextPositonX,
                                               musicPositionY),
                                   Color.Red * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
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
                                   new Vector2(TextPositonX,
                                               sfxPositionY),
                                   Color.Red * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
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
                                   new Vector2(TextPositonX,
                                               vibrationPositionY),
                                   Color.Red * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
                                               vibrationPositionY),
                                   Color.Red * opacity);
        }

        #endregion

        #region Load/Save

        public void Save()
        {
            if (!isInvalidated)
                return;

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(SETTINGS_FILE, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(this.musicValue);
                        sw.WriteLine(this.sfxValue);
                        sw.WriteLine(this.vibrationValue);

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
                bool hasExisted = isf.FileExists(SETTINGS_FILE);

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(SETTINGS_FILE, FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    isInvalidated = false;

                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            this.musicValue = (SoundValues)Enum.Parse(musicValue.GetType(), sr.ReadLine(), true);
                            this.sfxValue = (SoundValues)Enum.Parse(sfxValue.GetType(), sr.ReadLine(), true);
                            this.vibrationValue = (VibrationValues)Enum.Parse(vibrationValue.GetType(), sr.ReadLine(), true);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(this.musicValue);
                            sw.WriteLine(this.sfxValue);
                            sw.WriteLine(this.vibrationValue);

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
            return GetNeutralPositionValue(settingsManager.neutralPositionValue);
        }

        private float GetNeutralPositionValue(NeutralPositionValues value)
        {
            switch (value)
            {
                case NeutralPositionValues.Angle0:
                    return 0.0f;

                case NeutralPositionValues.Angle10:
                    return (float)Math.PI * 10.0f / 180.0f;

                case NeutralPositionValues.Angle20:
                    return (float)Math.PI * 20.0f / 180.0f;

                case NeutralPositionValues.Angle30:
                    return (float)Math.PI * 30.0f / 180.0f;

                case NeutralPositionValues.Angle40:
                    return (float)Math.PI * 40.0f / 180.0f;

                case NeutralPositionValues.Angle50:
                    return (float)Math.PI * 50.0f / 180.0f;

                case NeutralPositionValues.Angle60:
                    return (float)Math.PI * 60.0f / 180.0f;

                default:
                    return 0.0f;
            }
        }

        public float GetNeutralPositionRadianValue(float angle)
        {
            return (float)Math.PI * angle / 180.0f;
        }

        public int GetNeutralPositionIndex()
        {
            switch (settingsManager.neutralPositionValue)
            {
                case NeutralPositionValues.Angle0:
                    return 0;

                case NeutralPositionValues.Angle10:
                    return 1;

                case NeutralPositionValues.Angle20:
                    return 2;

                case NeutralPositionValues.Angle30:
                    return 3;

                case NeutralPositionValues.Angle40:
                    return 4;

                case NeutralPositionValues.Angle50:
                    return 5;

                case NeutralPositionValues.Angle60:
                    return 6;

                default:
                    return -1;
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            opacity = Single.Parse(reader.ReadLine());
            isInvalidated = Boolean.Parse(reader.ReadLine());
            neutralPositionValue = (NeutralPositionValues)Enum.Parse(neutralPositionValue.GetType(), reader.ReadLine(), false);
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(opacity);
            writer.WriteLine(isInvalidated);
            writer.WriteLine(neutralPositionValue);
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
