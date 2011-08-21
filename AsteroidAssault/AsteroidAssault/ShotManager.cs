using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

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

        public List<Sprite> Rockets = new List<Sprite>();
        private static Rectangle InitialRocketFrame;
        private static int RocketFrameCount = 5;
        private float rocketSpeed = 100.0f;
        private float softRocketSpeed = 50.0f;
        private static int RocketCollisionRadius = 7;

        private const float ROCKET_SMOKE_DELAY = 0.025f;
        private float rocketSmokeTimer = ROCKET_SMOKE_DELAY;

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

            ShotManager.InitialRocketFrame = new Rectangle(100, 440, 24, 8);
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
            newShot.RotateTo(velocity);

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

        //public void FireCarliRocket(Vector2 location, Vector2 velocity, bool playerFired, Color tintColor, bool sound)
        //{
        //    Sprite newShot = new Sprite(location,
        //                                ShotManager.Texture,
        //                                ShotManager.InitialRocketFrame,
        //                                velocity);

        //    newShot.TintColor = tintColor;

        //    if (newShot.Velocity != Vector2.Zero)
        //        newShot.Velocity.Normalize();

        //    newShot.Velocity *= rocketSpeed;
        //    newShot.RotateTo(velocity);

        //    for (int x = 0; x < ShotManager.RocketFrameCount; x++)
        //    {
        //        newShot.AddFrame(new Rectangle(ShotManager.InitialRocketFrame.X + (x * ShotManager.InitialRocketFrame.Width),
        //                                       ShotManager.InitialRocketFrame.Y,
        //                                       ShotManager.InitialRocketFrame.Width,
        //                                       ShotManager.InitialRocketFrame.Height));
        //    }
        //    newShot.CollisionRadius = ShotManager.RocketCollisionRadius;
        //    Rockets.Add(newShot);

        //    if (sound)
        //    {
        //        SoundManager.PlayRocketSound();
        //    }
        //}

        //public void FireSoftRocket(Vector2 location, Vector2 velocity, bool playerFired, Color tintColor, bool sound)
        //{
        //    Sprite newShot = new Sprite(location,
        //                                ShotManager.Texture,
        //                                ShotManager.InitialRocketFrame,
        //                                velocity);

        //    newShot.TintColor = tintColor;

        //    if (newShot.Velocity != Vector2.Zero)
        //        newShot.Velocity.Normalize();

        //    newShot.Velocity *= softRocketSpeed;
        //    newShot.RotateTo(velocity);

        //    for (int x = 0; x < ShotManager.RocketFrameCount; x++)
        //    {
        //        newShot.AddFrame(new Rectangle(ShotManager.InitialRocketFrame.X + (x * ShotManager.InitialRocketFrame.Width),
        //                                       ShotManager.InitialRocketFrame.Y,
        //                                       ShotManager.InitialRocketFrame.Width,
        //                                       ShotManager.InitialRocketFrame.Height));
        //    }
        //    newShot.CollisionRadius = ShotManager.RocketCollisionRadius;
        //    Rockets.Add(newShot);

        //    if (sound)
        //    {
        //        SoundManager.PlayEnemyRocketSound();
        //    }
        //}

        public void FireRocket(Vector2 location, Vector2 direction, bool playerFired, Color tintColor, bool sound)
        {
            Sprite newShot = new Sprite(location,
                                        ShotManager.Texture,
                                        ShotManager.InitialRocketFrame,
                                        direction);

            newShot.TintColor = tintColor;

            if (newShot.Velocity != Vector2.Zero)
                newShot.Velocity.Normalize();

            if (playerFired)
                newShot.Velocity *= rocketSpeed;
            else
                newShot.Velocity *= softRocketSpeed;

            newShot.RotateTo(direction);

            for (int x = 0; x < ShotManager.RocketFrameCount; x++)
            {
                newShot.AddFrame(new Rectangle(ShotManager.InitialRocketFrame.X + (x * ShotManager.InitialRocketFrame.Width),
                                               ShotManager.InitialRocketFrame.Y,
                                               ShotManager.InitialRocketFrame.Width,
                                               ShotManager.InitialRocketFrame.Height));
            }
            newShot.CollisionRadius = ShotManager.RocketCollisionRadius;
            Rockets.Add(newShot);

            if (sound)
            {
                if (playerFired)
                    SoundManager.PlayRocketSound();
                else
                    SoundManager.PlayEnemyRocketSound();
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int x = Shots.Count - 1; x >= 0; --x)
            {
                Shots[x].Update(gameTime);
                if (!screenBounds.Intersects(Shots[x].Destination))
                {
                    Shots.RemoveAt(x);
                }
            }

            for (int x = Rockets.Count - 1; x >= 0; --x)
            {
                Rockets[x].Update(gameTime);
                Rockets[x].Velocity *= 1.0f + (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f;

                if (!screenBounds.Intersects(Rockets[x].Destination))
                {
                    Rockets.RemoveAt(x);
                }
            }

            rocketSmokeTimer -= elapsed;

            if (rocketSmokeTimer <= 0.0f)
            {
                foreach (var rocket in Rockets)
                {
                    Vector2 offset = -rocket.Velocity;
                    offset.Normalize();
                    offset *= 10;

                    EffectManager.AddSparksEffect(rocket.Center + offset,
                                                  rocket.Velocity, Vector2.Zero,
                                                  Color.Orange * 0.4f,
                                                  false);
                }

                rocketSmokeTimer = ROCKET_SMOKE_DELAY;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var shot in this.Shots)
	        {
                shot.Draw(spriteBatch);
	        }

            foreach (var rocket in this.Rockets)
            {
                rocket.Draw(spriteBatch);
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Shots
            int shotsCount = Int32.Parse(reader.ReadLine());

            Shots.Clear();

            for (int i = 0; i < shotsCount; ++i)
            {
                Vector2 location = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));
                Vector2 direction = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

                if (direction != Vector2.Zero)
                    direction.Normalize();

                FireShot(location,
                         direction,
                         true,
                         new Color(Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine())),
                         false);
            }

            // Rockets
            int rocketsCount = Int32.Parse(reader.ReadLine());

            Rockets.Clear();

            for (int i = 0; i < rocketsCount; ++i)
            {
                bool playerFired = false;
                Vector2 location = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));
                Vector2 direction = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

                if (direction != Vector2.Zero)
                    direction.Normalize();

                if (direction.X == 0.0f)
                    playerFired = true;

                FireRocket(location,
                           direction,
                           playerFired,
                           new Color(Int32.Parse(reader.ReadLine()),
                                     Int32.Parse(reader.ReadLine()),
                                     Int32.Parse(reader.ReadLine()),
                                     Int32.Parse(reader.ReadLine())),
                           false);
            }

            this.rocketSmokeTimer = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Shots
            writer.WriteLine(Shots.Count);

            for (int i = 0; i < Shots.Count; ++i)
            {
                writer.WriteLine(Shots[i].Location.X);
                writer.WriteLine(Shots[i].Location.Y);
                writer.WriteLine(Shots[i].Velocity.X);
                writer.WriteLine(Shots[i].Velocity.Y);
                writer.WriteLine((int)Shots[i].TintColor.R);
                writer.WriteLine((int)Shots[i].TintColor.G);
                writer.WriteLine((int)Shots[i].TintColor.B);
                writer.WriteLine((int)Shots[i].TintColor.A);
            }

            // Rockets
            writer.WriteLine(Rockets.Count);

            for (int i = 0; i < Rockets.Count; ++i)
            {
                writer.WriteLine(Rockets[i].Location.X);
                writer.WriteLine(Rockets[i].Location.Y);
                writer.WriteLine(Rockets[i].Velocity.X);
                writer.WriteLine(Rockets[i].Velocity.Y);
                writer.WriteLine((int)Rockets[i].TintColor.R);
                writer.WriteLine((int)Rockets[i].TintColor.G);
                writer.WriteLine((int)Rockets[i].TintColor.B);
                writer.WriteLine((int)Rockets[i].TintColor.A);
            }

            writer.WriteLine(rocketSmokeTimer);
        }

        #endregion
    }
}
