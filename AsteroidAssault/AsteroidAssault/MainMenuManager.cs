using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;

namespace SpacepiXX
{
    class MainMenuManager
    {
        #region Members

        public enum MenuItems { None, Start, Highscores, Instructions, Help, Settings };

        private MenuItems lastPressedMenuItem = MenuItems.None;

        private Texture2D texture;

        private Rectangle spacepixxSource = new Rectangle(0, 0,
                                                          500, 100);
        private Rectangle spacepixxDestination = new Rectangle(150, 20,
                                                               500, 100);

        private Rectangle startSource = new Rectangle(0, 200,
                                                      300, 50);
        private Rectangle startDestination = new Rectangle(250, 130,
                                                           300, 50);

        private Rectangle highscoresSource = new Rectangle(0, 250,
                                                           300, 50);
        private Rectangle highscoresDestination = new Rectangle(250, 270,
                                                                300, 50);

        private Rectangle instructionsSource = new Rectangle(0, 300,
                                                             300, 50);
        private Rectangle instructionsDestination = new Rectangle(250, 200,
                                                                  300, 50);

        private Rectangle helpSource = new Rectangle(0, 350,
                                                     300, 50);
        private Rectangle helpDestination = new Rectangle(250, 410,
                                                          300, 50);

        private Rectangle settingsSource = new Rectangle(0, 400,
                                                     300, 50);
        private Rectangle settingsDestination = new Rectangle(250, 340,
                                                          300, 50);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private float time = 0.0f;

        #endregion

        #region Constructors

        public MainMenuManager(Texture2D spriteSheet)
        {
            this.texture = spriteSheet;
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

            time = (float)gameTime.TotalGameTime.TotalSeconds;

            this.handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             spacepixxDestination,
                             spacepixxSource,
                             Color.Red * opacity);

            spriteBatch.Draw(texture,
                             startDestination,
                             startSource,
                             Color.Red * opacity);

            spriteBatch.Draw(texture,
                             highscoresDestination,
                             highscoresSource,
                             Color.Red * opacity);

            if (InstructionManager.HasDoneInstructions)
            {
                spriteBatch.Draw(texture,
                                 instructionsDestination,
                                 instructionsSource,
                                 Color.Red * opacity);
            }
            else
            {
                spriteBatch.Draw(texture,
                                 instructionsDestination,
                                 instructionsSource,
                                 Color.Red * opacity * (float)Math.Pow(Math.Sin(time), 2.0f));
            }

            spriteBatch.Draw(texture,
                             helpDestination,
                             helpSource,
                             Color.Red * opacity);

            spriteBatch.Draw(texture,
                             settingsDestination,
                             settingsSource,
                             Color.Red * opacity);
        }

        private void handleTouchInputs()
        {
            if (TouchPanel.IsGestureAvailable)
            {
                GestureSample gs = TouchPanel.ReadGesture();

                if (gs.GestureType == GestureType.Tap)
                {
                    // Start
                    if (startDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
                    {
                        this.lastPressedMenuItem = MenuItems.Start;
                    }
                    // Highscores
                    else if (highscoresDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
                    {
                        this.lastPressedMenuItem = MenuItems.Highscores;
                    }
                    // Instructions
                    else if (instructionsDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
                    {
                        this.lastPressedMenuItem = MenuItems.Instructions;
                    }
                    // Help
                    else if (helpDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
                    {
                        this.lastPressedMenuItem = MenuItems.Help;
                    }
                    // Settings
                    else if (settingsDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
                    {
                        this.lastPressedMenuItem = MenuItems.Settings;
                    }
                    else
                    {
                        this.lastPressedMenuItem = MenuItems.None;
                    }
                }
            }
            else
            {
                this.lastPressedMenuItem = MenuItems.None;
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.lastPressedMenuItem = (MenuItems)Enum.Parse(lastPressedMenuItem.GetType(), reader.ReadLine(), false);
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
            this.time = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(lastPressedMenuItem);
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
            writer.WriteLine(time);
        }

        #endregion

        #region Properties

        public MenuItems LastPressedMenuItem
        {
            get
            {
                return this.lastPressedMenuItem;
            }
        }

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
