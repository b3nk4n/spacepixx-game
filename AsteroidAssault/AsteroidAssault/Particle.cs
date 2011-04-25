using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class Particle : Sprite
    {
        #region Members

        private Vector2 acceleration;
        private float maxSpeed;
        private int initialDuration;
        private int remainingDuration;
        private Color initialColor;
        private Color finalColor;
        
        #endregion

        #region Constructors

        public Particle(Vector2 location, Texture2D texture, Rectangle initialFrame,
                        Vector2 velocity, Vector2 acceleration, float maxSpeed,
                        int duration, Color initialColor, Color finalColor)
            : base(location, texture, initialFrame, velocity)
        {
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.initialDuration = duration;
            this.remainingDuration = duration;
            this.initialColor = initialColor;
            this.finalColor = finalColor;
        }

        #endregion

        #region Methods

        public override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                velocity += acceleration;

                if (velocity.Length() > maxSpeed)
                {
                    velocity.Normalize();
                    velocity *= maxSpeed;
                }

                TintColor = Color.Lerp(initialColor,
                                       finalColor,
                                       this.DurationProgress);
                remainingDuration--;

                base.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch);
            }
            
        }

        #endregion

        #region Properties

        public int ElapsedDuration
        {
            get
            {
                return initialDuration - remainingDuration;
            }
        }

        public float DurationProgress
        {
            get
            {
                return (float)ElapsedDuration / (float)initialDuration;
            }
        }

        public bool IsActive
        {
            get
            {
                return remainingDuration > 0;
            }
        }

        #endregion
    }
}
