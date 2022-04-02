using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class Sprite
    {
        #region Members

        public Texture2D Texture;

        protected List<Rectangle> frames = new List<Rectangle>();
        private int frameWidth;
        private int frameHeight;
        private int currentFrame;
        private float frameTime = 0.1f;
        private float timeForCurrentFrame = 0.0f;

        private Color tintColor = Color.White;
        private float rotation = 0.0f;

        public int CollisionRadius = 0;
        public int BoundingXPadding = 0;
        public int BoundingYPadding = 0;

        protected Vector2 location = Vector2.Zero;
        protected Vector2 velocity = Vector2.Zero;

        #endregion

        #region Constructors

        public Sprite(Vector2 location, Texture2D texture,
                      Rectangle initialFrame, Vector2 velocity)
        {
            this.location = location;
            this.Texture = texture;
            this.velocity = velocity;

            this.frames.Add(initialFrame);
            this.frameWidth = initialFrame.Width;
            this.frameHeight = initialFrame.Height;
        }

        #endregion

        #region Methods

        public bool isBoxColliding(Rectangle otherBox)
        {
            return this.BoundingBoxRect.Intersects(otherBox);
        }

        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            if (Vector2.Distance(this.Center, otherCenter) < this.CollisionRadius + otherRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddFrame(Rectangle frameRect)
        {
            this.frames.Add(frameRect);
        }

        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.timeForCurrentFrame += elapsed;

            if (this.timeForCurrentFrame >= this.FrameTime)
            {
                this.currentFrame = (++this.currentFrame) % this.frames.Count;
                this.timeForCurrentFrame = this.timeForCurrentFrame - this.FrameTime;
            }

            location += (this.Velocity * elapsed);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                             Center,
                             Source,
                             TintColor,
                             Rotation,
                             new Vector2(frameWidth / 2, frameHeight / 2),
                             1.0f,
                             SpriteEffects.None,
                             0.0f);
        }

        #endregion

        #region Properties

        public Vector2 Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return this.velocity;
            }
            set
            {
                this.velocity = value;
            }
        }

        public Color TintColor
        {
            get
            {
                return this.tintColor;
            }
            set
            {
                this.tintColor = value;
            }
        }

        public float Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.rotation = value;
            }
        }

        public int Frame
        {
            get
            {
                return this.currentFrame;
            }
            set
            {
                this.currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1);
            }
        }

        public float FrameTime
        {
            get
            {
                return this.frameTime;
            }
            set
            {
                this.frameTime = MathHelper.Max(0, value);
            }
        }

        public Rectangle Source
        {
            get
            {
                return this.frames[currentFrame];
            }
        }

        public Rectangle Destination
        {
            get
            {
                return new Rectangle((int)location.X,
                                     (int)location.Y,
                                     frameWidth,
                                     frameHeight);
            }
        }

        public Vector2 Center
        {
            get
            {
                return location + new Vector2(frameWidth / 2,
                                              frameHeight / 2);
            }
        }

        public Rectangle BoundingBoxRect
        {
            get
            {
                return new Rectangle((int)location.X + BoundingXPadding,
                                     (int)location.Y + BoundingYPadding,
                                     frameWidth - (2 * BoundingXPadding),
                                     frameHeight - (2 * BoundingYPadding));
            }
        }

        #endregion
    }
}
