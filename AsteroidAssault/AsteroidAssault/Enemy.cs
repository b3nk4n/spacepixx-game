using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class Enemy : ILevel
    {
        #region Members

        private Sprite enemySprite;
        private Vector2 gunOffset = new Vector2(25, 25);
        private Queue<Vector2> wayPoints = new Queue<Vector2>();
        private Vector2 currentWayPoint = Vector2.Zero;
        private float speed;

        private const int EnemyRadiusEasy = 12;
        private const int EnemyRadiusMedium = 20;
        private const int EnemyRadiusHard = 18;
        private const int EnemyRadiusSpeeder = 18;
        private const int EnemyRadiusTank = 18;

        private const float InitialHitPointsEasy = 25.0f;
        private const float InitialHitPointsMedium = 80.0f;
        private const float InitialHitPointsHard = 125.0f;
        private const float InitialHitPointsSpeeder = 75.0f;
        private const float InitialHitPointsTank = 210.0f;
        

        private Vector2 previousLocation = Vector2.Zero;

        private float hitPoints;
        public float MaxHitPoints;

        public enum EnemyType { Easy, Medium, Hard, Speeder, Tank };

        private readonly int initialHitScore;
        private int hitScore;
        private readonly int initialKillScore;
        private int killScore;

        private readonly float initialShotChance;
        private float shotChance;

        #endregion

        #region Constructors

        private Enemy(Texture2D texture, Vector2 location, Rectangle initialFrame,
                     int frameCount, int hitScore, int killScore, float shotChance,
                     int collisionRadius)
        {
            enemySprite = new Sprite(location,
                                     texture,
                                     initialFrame,
                                     Vector2.Zero);

            for (int x = 1; x < frameCount; x++)
            {
                EnemySprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
                previousLocation = location;
                currentWayPoint = location;
                EnemySprite.CollisionRadius = collisionRadius;
            }

            this.initialHitScore = hitScore;
            this.hitScore = hitScore;
            this.initialKillScore = killScore;
            this.killScore = killScore;

            this.initialShotChance = shotChance;
            this.shotChance = shotChance;
        }

        #endregion

        #region Methods

        public static Enemy CreateEasyEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                    location,
                                    new Rectangle(0, 200,
                                                  50, 50),
                                    4,
                                    50,
                                    100,
                                    0.15f,
                                    Enemy.EnemyRadiusEasy);

            enemy.speed = 150.0f;
            enemy.previousLocation = Vector2.Zero;

            enemy.hitPoints = InitialHitPointsEasy;
            enemy.MaxHitPoints = InitialHitPointsEasy;

            return enemy;
        }

        public static Enemy CreateMediumEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                new Rectangle(0, 250,
                                              50, 50),
                                6,
                                75,
                                150,
                                0.25f,
                                Enemy.EnemyRadiusMedium);

            enemy.speed = 125.0f;
            enemy.previousLocation = Vector2.Zero;

            enemy.hitPoints = InitialHitPointsMedium;
            enemy.MaxHitPoints = InitialHitPointsMedium;

            return enemy;
        }

        public static Enemy CreateHardEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                new Rectangle(350, 200,
                                              50, 50),
                                6,
                                100,
                                200,
                                0.3f,
                                Enemy.EnemyRadiusHard);

            enemy.speed = 100.0f;
            enemy.previousLocation = Vector2.Zero;

            enemy.hitPoints = InitialHitPointsHard;
            enemy.MaxHitPoints = InitialHitPointsHard;

            return enemy;
        }

        public static Enemy CreateSpeederEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                new Rectangle(350, 250,
                                              50, 50),
                                6,
                                100,
                                200,
                                0.25f,
                                Enemy.EnemyRadiusSpeeder);

            enemy.speed = 175.0f;
            enemy.previousLocation = Vector2.Zero;

            enemy.hitPoints = InitialHitPointsSpeeder;
            enemy.MaxHitPoints = InitialHitPointsSpeeder;

            return enemy;
        }

        public static Enemy CreateTankEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                new Rectangle(350, 300,
                                              50, 50),
                                6,
                                50,
                                300,
                                0.35f,
                                Enemy.EnemyRadiusTank);

            enemy.speed = 100.0f;
            enemy.previousLocation = Vector2.Zero;

            enemy.hitPoints = InitialHitPointsTank;
            enemy.MaxHitPoints = InitialHitPointsTank;

            return enemy;
        }

        public void AddWayPoint(Vector2 wayPoint)
        {
            wayPoints.Enqueue(wayPoint);
        }

        public bool WayPointReached()
        {
            if (Vector2.Distance(EnemySprite.Location, currentWayPoint) <
                (float)EnemySprite.Source.Width / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsActive()
        {
            if (IsDestroyed)
            {
                return false;
            }

            if (wayPoints.Count > 0)
            {
                return true;
            }

            if (WayPointReached())
            {
                return false;
            }

            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (IsActive())
            {
                Vector2 heading = currentWayPoint - EnemySprite.Location;

                if (heading != Vector2.Zero)
                {
                    heading.Normalize();
                }

                heading *= speed;
                EnemySprite.Velocity = heading;
                previousLocation = EnemySprite.Location;
                EnemySprite.Update(gameTime);
                EnemySprite.Rotation = (float)Math.Atan2(EnemySprite.Location.Y - previousLocation.Y,
                                                         EnemySprite.Location.X - previousLocation.X);

                if (WayPointReached())
                {
                    if (wayPoints.Count > 0)
                    {
                        currentWayPoint = wayPoints.Dequeue();
                    }
                }

                float factor = (float)Math.Max((this.hitPoints / this.MaxHitPoints), 0.66f);
                this.EnemySprite.TintColor = Color.White * factor;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
            {
                EnemySprite.Draw(spriteBatch);
            }
        }

        public void SetLevel(int lvl)
        {
            this.hitScore = initialHitScore + (lvl - 1) * (initialHitScore / 10);
            this.killScore = initialKillScore + (lvl - 1) * (initialKillScore / 10);

            this.shotChance = initialShotChance + (lvl - 1) * 0.01f;

            this.MaxHitPoints += (lvl - 1);
            this.HitPoints += (lvl - 1); 
        }

        #endregion

        #region Properties

        public Sprite EnemySprite
        {
            get
            {
                return this.enemySprite;
            }
        }

        public Vector2 GunOffset
        {
            get
            {
                return this.gunOffset;
            }
        }

        public int HitScore
        {
            get
            {
                return this.hitScore;
            }
        }

        public int KillScore
        {
            get
            {
                return this.killScore;
            }
        }

        public float ShotChance
        {
            get
            {
                return this.shotChance;
            }
        }

        public float HitPoints
        {
            get
            {
                return this.hitPoints;
            }
            set
            {
                this.hitPoints = MathHelper.Clamp(value, 0.0f, MaxHitPoints);
            }
        }

        public bool IsDestroyed
        {
            get
            {
                return this.hitPoints <= 0.0f;
            }
        }

        #endregion
    }
}
