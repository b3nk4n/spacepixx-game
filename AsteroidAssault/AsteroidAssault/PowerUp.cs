using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpacepiXX
{
    class PowerUp
    {
        #region Members

        private Sprite powerUpSprite;

        private const float SPEED = 100.0f;
        private const int RADIUS = 10;

        public enum PowerUpType { Health25, Health50, Health100, CoolWater, Life, SpecialShot, KillAll, LowBonusScore, MediumBonusScore, HighBonusScore, ScoreMultiLow, ScoreMultiMedium, ScoreMultiHigh, BonusRockets, Shield,
                                  AntiScoreMulti, OutOfControl, Slow, OverHeat,
                                  Random};

        private PowerUpType type;

        private bool isActive = true;

        #endregion

        #region Constructor

        public PowerUp(Texture2D texture, Vector2 location, Rectangle initialFrame,
                       int frameCount, PowerUpType type)
        {
            powerUpSprite = new Sprite(location, texture, initialFrame, new Vector2(0, 1) * SPEED);
            powerUpSprite.CollisionRadius = RADIUS;

            this.type = type;
        }

        #endregion

        public void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                powerUpSprite.Update(gameTime);

                if (!IsInScreen)
                {
                    IsActive = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                powerUpSprite.Draw(spriteBatch);
            }
        }

        #region Methods

        public bool isCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            return this.powerUpSprite.IsCircleColliding(otherCenter, otherRadius);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            //powerUpSprite.Location = new Vector2(Single.Parse(reader.ReadLine()),
            //                                     Single.Parse(reader.ReadLine()));
            //powerUpSprite.Rotation = Single.Parse(reader.ReadLine());
            //powerUpSprite.Velocity = new Vector2(Single.Parse(reader.ReadLine()),
            //                                     Single.Parse(reader.ReadLine()));
            powerUpSprite.Activated(reader);

            this.type = (PowerUpType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

            this.isActive = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Powerup sprite
            //writer.WriteLine(powerUpSprite.Location.X);
            //writer.WriteLine(powerUpSprite.Location.Y);
            //writer.WriteLine(powerUpSprite.Rotation);
            //writer.WriteLine(powerUpSprite.Velocity.X);
            //writer.WriteLine(powerUpSprite.Velocity.Y);
            powerUpSprite.Deactivated(writer);

            writer.WriteLine(type);

            writer.WriteLine(isActive);
        }

        #endregion

        #region Properties

        public PowerUpType Type
        {
            get
            {
                return type;
            }
            set
            {
                this.type = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                this.isActive = value;
            }
        }

        public bool IsInScreen
        {
            get
            {
                return powerUpSprite.Location.Y < 480;
            }
        }

        #endregion
    }
}
