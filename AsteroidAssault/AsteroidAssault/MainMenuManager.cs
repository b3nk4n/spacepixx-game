using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace SpacepiXX
{
    class MainMenuManager
    {
        #region Members

        public enum MenuItems { None, Start, Highscores, Instructions, Help };

        private MenuItems lastPressedMenuItem = MenuItems.None;

        private Texture2D texture;

        private Rectangle spacepixxSource = new Rectangle(0, 0,
                                                          500, 100);
        private Rectangle spacepixxDestination = new Rectangle(150, 50,
                                                               500, 100);

        private Rectangle startSource = new Rectangle(0, 200,
                                                      300, 50);
        private Rectangle startDestination = new Rectangle(250, 175,
                                                           300, 50);

        private Rectangle highscoresSource = new Rectangle(0, 250,
                                                           300, 50);
        private Rectangle highscoresDestination = new Rectangle(250, 250,
                                                                300, 50);

        private Rectangle instructionsSource = new Rectangle(0, 300,
                                                             300, 50);
        private Rectangle instructionsDestination = new Rectangle(250, 325,
                                                                  300, 50);

        private Rectangle helpSource = new Rectangle(0, 350,
                                                     300, 50);
        private Rectangle helpDestination = new Rectangle(250, 400,
                                                          300, 50);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

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

            spriteBatch.Draw(texture,
                             instructionsDestination,
                             instructionsSource,
                             Color.Red * opacity);

            spriteBatch.Draw(texture,
                             helpDestination,
                             helpSource,
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
