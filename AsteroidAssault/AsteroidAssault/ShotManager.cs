using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class ShotManager
    {
        #region Members

        public List<Sprite> Shots = new List<Sprite>();
        private Rectangle screenBounds;

        private static Texture2D Texture;
        private static Rectangle InitialFrame;
        private static int FrameCount;
        private float shotSpeed;
        private static int CollisionRadius;

        #endregion

        #region Constructors

        public ShotManager(Texture2D texture, Rectangle initialFrame, int frameCount,
                           int collisionRadius, float speed, Rectangle screenBounds)
        {
            ShotManager.Texture = texture;
            ShotManager.InitialFrame = initialFrame;
            ShotManager.FrameCount = frameCount;
            ShotManager.CollisionRadius = collisionRadius;
            this.shotSpeed = speed;
            this.screenBounds = screenBounds;
        }

        #endregion

        #region Methods

        public void FireShot(Vector2 location, Vector2 velocity, bool playerFired, Color tintColor, bool sound)
        {
            Sprite newShot = new Sprite(location,
                                        ShotManager.Texture,
                                        ShotManager.InitialFrame,
                                        velocity);

            newShot.TintColor = tintColor;
            newShot.Velocity *= shotSpeed;

            for (int x = 0; x < ShotManager.FrameCount; x++)
            {
                newShot.AddFrame(new Rectangle(ShotManager.InitialFrame.X + (x * ShotManager.InitialFrame.Width),
                                               ShotManager.InitialFrame.Y,
                                               ShotManager.InitialFrame.Width,
                                               ShotManager.InitialFrame.Height));
            }
            newShot.CollisionRadius = ShotManager.CollisionRadius;
            Shots.Add(newShot);

            if (sound)
            {
                if (playerFired)
                {
                    SoundManager.PlayPlayerShot();
                }
                else
                {
                    SoundManager.PlayEnemyShot();
                }
            }
        }

        //public void FireShot(Vector2 location, Vector2 velocity, bool playerFired)
        //{
        //    FireShot(location, velocity, playerFired, Color.White);
        //}

        public void Update(GameTime gameTime)
        {
            for (int x = Shots.Count - 1; x >= 0; --x)
            {
                Shots[x].Update(gameTime);
                if (!screenBounds.Intersects(Shots[x].Destination))
                {
                    Shots.RemoveAt(x);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var shot in this.Shots)
	        {
                shot.Draw(spriteBatch);
	        }
        }

        #endregion
    }
}
