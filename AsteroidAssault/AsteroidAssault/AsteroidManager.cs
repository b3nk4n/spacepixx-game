using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class AsteroidManager : ILevel
    {
        #region Members

        private int screenWidth = 800;
        private int screenHeight = 480;
        private int screenPadding = 10;

        private Rectangle initialFrame;
        private int asteroidFrames;
        private Texture2D texture;

        private List<Sprite> asteroids = new List<Sprite>();
        private int minSpeed = 60;
        private int maxSpeed = 120;

        private Random rand = new Random();

        private readonly int initialCount;
        private int count;
        private const int MaxAsteroidsCount = 15;

        private bool isActive = true;

        private float crashPower = 66.0f;

        #endregion

        #region Constructors

        public AsteroidManager(int asteroidCount, Texture2D texture, Rectangle initialFrame,
                               int asteroidFrames, int screenWidth, int screenHeight)
        {
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.asteroidFrames = asteroidFrames;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.initialCount = asteroidCount;
            this.count = asteroidCount;

            for (int x = 0; x < this.count; x++)
            {
                AddAsteroid();
            }
        }

        #endregion

        #region Methods

        public void AddAsteroid()
        {
            Sprite newAsteroid = new Sprite(new Vector2(-500, -500),
                                            texture,
                                            initialFrame,
                                            Vector2.Zero);

            for (int x = 0; x < asteroidFrames; x++)
            {
                newAsteroid.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
            }
            
            newAsteroid.Rotation = MathHelper.ToRadians((float)rand.Next(0, 360));
            newAsteroid.CollisionRadius = 15;
            Asteroids.Add(newAsteroid);
        }

        public void Clear()
        {
            Asteroids.Clear();
        }

        private Vector2 randomLocation()
        {
            Vector2 location = Vector2.Zero;
            bool locationOK = true;
            int tryCount = 0;

            do
            {
                locationOK = true;
                switch (rand.Next(0, 3))
                {
                    case 0:
                        location.X = -initialFrame.Width;
                        location.Y = rand.Next(0, screenHeight);
                        break;

                    case 1:
                        location.X = screenWidth;
                        location.Y = rand.Next(0, screenHeight);
                        break;

                    case 2:
                        location.X = rand.Next(0, screenWidth);
                        location.Y = -initialFrame.Height;
                        break;
                }

                foreach (var asteroid in Asteroids)
                {
                    if (asteroid.isBoxColliding(new Rectangle((int)location.X,
                                                              (int)location.Y,
                                                              initialFrame.Width,
                                                              initialFrame.Height)))
                    {
                        locationOK = false;
                    }
                }

                ++tryCount;

                if (tryCount > 5 && locationOK == false)
                {
                    location = new Vector2(-500, -500);
                    locationOK = true;
                }
            } while (locationOK == false);

            return location;
        }

        private Vector2 randomVelocity()
        {
            Vector2 velocity = new Vector2(rand.Next(0, 101) - 50,
                                           rand.Next(0, 101) - 50);
            velocity.Normalize();
            velocity *= rand.Next(minSpeed, maxSpeed);
            return velocity;
        }

        private bool isOnScreen(Sprite asteroid)
        {
            if (asteroid.Destination.Intersects(new Rectangle(-screenPadding,
                                                              -screenPadding,
                                                              screenWidth + screenPadding,
                                                              screenHeight + screenPadding)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (this.count > Asteroids.Count)
            {
                this.AddAsteroid();
            }
            else if (this.count < Asteroids.Count)
            {
                for (int i = 0; i < Asteroids.Count; i++)
                {
                    if (!isOnScreen(Asteroids[i]))
                    {
                        // Remove just one Asteroid per loop
                        Asteroids.RemoveAt(i);
                        break;
                    }
                }
            }

            foreach (var asteroid in Asteroids)
            {
                asteroid.Update(gameTime);
                if (!isOnScreen(asteroid) && isActive)
                {
                    asteroid.Location = randomLocation();
                    asteroid.Velocity = randomVelocity();
                }
            }

            for (int x = 0; x < Asteroids.Count; x++)
            {
                for (int y = x + 1; y < Asteroids.Count; y++)
                {
                    if (Asteroids[x].IsCircleColliding(Asteroids[y].Center,
                                                       Asteroids[y].CollisionRadius))
                    {
                        BounceAsteroids(Asteroids[x], Asteroids[y]);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var asteroid in Asteroids)
            {
                asteroid.Draw(spriteBatch);
            }
        }

        private void BounceAsteroids(Sprite asteroid1, Sprite asteroid2)
        {
            Vector2 cOfMass = (asteroid1.Velocity + asteroid2.Velocity) / 2;

            Vector2 normal1 = asteroid2.Center - asteroid1.Center;
            normal1.Normalize();
            Vector2 normal2 = asteroid1.Center - asteroid2.Center;
            normal2.Normalize();

            asteroid1.Velocity -= cOfMass;
            asteroid1.Velocity = Vector2.Reflect(asteroid1.Velocity, normal1);
            asteroid1.Velocity += cOfMass;

            asteroid2.Velocity -= cOfMass;
            asteroid2.Velocity = Vector2.Reflect(asteroid2.Velocity, normal2);
            asteroid2.Velocity += cOfMass;
        }

        public void Reset()
        {
            foreach (var asteroid in Asteroids)
            {
                asteroid.Location = new Vector2(-500, -500);
            }
        }

        public void SetLevel(int lvl)
        {
            int newCount = initialCount + (lvl - 1) / 3;
            this.count = Math.Min(newCount, MaxAsteroidsCount);
        }

        #endregion

        #region Properties

        public List<Sprite> Asteroids
        {
            get
            {
                return this.asteroids;
            }
        }

        public bool IsActive
        {
            set
            {
                this.isActive = value;
            }
            get
            {
                return this.isActive;
            }
        }

        public float CrashPower
        {
            get
            {
                return this.crashPower;
            }
        }

        #endregion
    }
}
