using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Devices;

namespace SpacepiXX
{
    static class EffectManager
    {
        #region Members

        public static List<Particle> Effects = new List<Particle>();
        private static Random rand = new Random();
        public static Texture2D Texture;
        public static Rectangle ParticleFrame = new Rectangle(0, 450, 2, 2);
        public static List<Rectangle> ExplosionFrames = new List<Rectangle>();

        #endregion

        #region Methods

        public static void Initialize(Texture2D texture, Rectangle particleFrame,
                                      Rectangle initialExplosionFrame, int explosionFameCount)
        {
            Texture = texture;
            ParticleFrame = particleFrame;
            ExplosionFrames.Clear();

            for (int x = 1; x < explosionFameCount; x++)
            {
                initialExplosionFrame.Offset(initialExplosionFrame.Width, 0);
                ExplosionFrames.Add(initialExplosionFrame);
            }
        }

        public static Vector2 RandomDirection(float scale)
        {
            Vector2 direction;

            do
            {
                direction = new Vector2(rand.Next(0, 101) - 50,
                                        rand.Next(0, 101) - 50);
            } while (direction.Length() == 0);

            direction.Normalize();
            direction *= scale;

            return direction;
        }

        public static void Update(GameTime gameTime)
        {
            for (int x = Effects.Count - 1; x >= 0; --x)
            {
                if (Effects[x].IsActive)
                {
                    Effects[x].Update(gameTime);
                }
                else
                {
                    Effects.RemoveAt(x);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in Effects)
            {
                particle.Draw(spriteBatch);
            }
        }

        public static void AddCustomExplosion(Vector2 location, Vector2 momentum, int minPointCount,
                                              int maxPointCount, int minPieceCount, int maxPieceCount,
                                              float pieceSpeedScale, int duration, Color initialColor,
                                              Color finalColor)
        {
            float explosionMaxSpeed = 30.0f;
            int pointSpeedMin = (int)pieceSpeedScale * 2;
            int pointSpeedMax = (int)pieceSpeedScale * 3;

            Vector2 pieceLocation = location - new Vector2(ExplosionFrames[0].Width / 2,
                                                           ExplosionFrames[0].Height / 2);

            int pieceCount = rand.Next(minPieceCount, maxPointCount + 1);

            for (int x = 0; x < pieceCount; x++)
            {
                Effects.Add(new Particle(pieceLocation,
                                         Texture,
                                         ExplosionFrames[rand.Next(0, ExplosionFrames.Count - 1)],
                                         RandomDirection(pieceSpeedScale) + momentum,
                                         Vector2.Zero,
                                         ((float)rand.NextDouble()) * explosionMaxSpeed,
                                         duration,
                                         initialColor,
                                         finalColor));
            }

            int pointsCount = rand.Next(minPointCount, maxPointCount + 1);

            for (int x = 0; x < maxPointCount; x++)
			{
                Effects.Add(new Particle(location,
                                         Texture,
                                         ParticleFrame,
                                         RandomDirection((float)rand.Next(pointSpeedMin, pointSpeedMax + 1)) + momentum,
                                         Vector2.Zero,
                                         ((float)rand.NextDouble()) * explosionMaxSpeed,
                                         duration,
                                         initialColor,
                                         finalColor));
			}

            SoundManager.PlayExplosion();
        }

        public static void AddExplosion(Vector2 location, Vector2 momentum)
        {
            AddCustomExplosion(location,
                               momentum,
                               10,
                               25,
                               2,
                               4,
                               6.0f,
                               30, 
                               new Color(1.0f, 1.0f, 1.0f),
                               new Color(1.0f, 1.0f, 1.0f) * 0.0f);
                               //new Color(1.0f, 0.3f, 0.0f) * 0.5f,
                               //new Color(1.0f, 0.3f, 0.0f) * 0.0f);
        }

        public static void AddLargeExplosion(Vector2 location, Vector2 momentum)
        {
            AddCustomExplosion(location,
                               momentum,
                               15,
                               20,
                               4,
                               6,
                               30.0f,
                               60,
                               new Color(1.0f, 1.0f, 1.0f),
                               new Color(1.0f, 1.0f, 1.0f) * 0.0f);
                               //new Color(1.0f, 0.3f, 0.0f) * 0.5f,
                               //new Color(1.0f, 0.3f, 0.0f) * 0.0f);
        }

        public static void AddSparksEffect(Vector2 location, Vector2 impectVelocity, Vector2 momentum)
        {
            int particleCount = rand.Next(10, 20);

            for (int x = 0; x < particleCount; x++)
            {
                Effects.Add(new Particle(location - (impectVelocity / 60),
                                         Texture,
                                         ParticleFrame,
                                         RandomDirection((float)rand.Next(10, 20)) + momentum * 0.75f,
                                         Vector2.Zero,
                                         60.0f,
                                         20,
                                         Color.Gray,
                                         Color.Gray * 0.0f));
            }
        }

        public static void AddLargeSparksEffect(Vector2 location, Vector2 impectVelocity, Vector2 momentum, Color tintColor)
        {
            int particleCount = rand.Next(15, 25);

            for (int x = 0; x < particleCount; x++)
            {
                Effects.Add(new Particle(location - (impectVelocity / 60),
                                         Texture,
                                         ParticleFrame,
                                         RandomDirection((float)rand.Next(10, 20)) * 3 + momentum * 0.25f,
                                         Vector2.Zero,
                                         80.0f,
                                         15,
                                         tintColor * 0.75f,
                                         tintColor * 0.0f));
            }
        }

        public static void AddAsteroidExplosion(Vector2 location, Vector2 momentum)
        {
            AddCustomExplosion(location,
                               momentum,
                               25,
                               35,
                               2,
                               4,
                               80.0f,
                               45,
                               Color.DarkGray * 0.5f,
                               Color.DarkGray * 0.0f);
        }

        public static void Reset()
        {
            Effects.Clear();
        }

        #endregion
    }
}
