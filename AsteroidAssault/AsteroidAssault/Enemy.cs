using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpacepiXX
{
    class Enemy : ILevel
    {
        #region Members

        private Sprite enemySprite;
        private Vector2 gunOffset = new Vector2(25, 25);
        private Queue<Vector2> wayPoints = new Queue<Vector2>(32);
        private Vector2 currentWayPoint = Vector2.Zero;

        private float speed;

        private const int EnemyRadiusEasy = 14;
        private const int EnemyRadiusMedium = 20;
        private const int EnemyRadiusHard = 18;
        private const int EnemyRadiusSpeeder = 18;
        private const int EnemyRadiusTank = 18;

        private const float InitialHitPointsEasy = 25.0f;
        private const float InitialHitPointsMedium = 80.0f;
        private const float InitialHitPointsHard = 125.0f;
        private const float InitialHitPointsSpeeder = 75.0f;
        private const float InitialHitPointsTank = 210.0f;

        private readonly Rectangle EasySource = new Rectangle(0, 200,
                                                              50, 50);
        private readonly Rectangle MediumSource = new Rectangle(0, 250,
                                                                50, 50);
        private readonly Rectangle HardSource = new Rectangle(350, 200,
                                                              50, 50);
        private readonly Rectangle SpeederSource = new Rectangle(350, 250,
                                                                 50, 50);
        private readonly Rectangle TankSource = new Rectangle(350, 150,
                                                              50, 50);

        private const int EasyFrameCount = 4;
        private const int MediumFrameCount = 6;
        private const int HardFrameCount = 6;
        private const int SpeederFrameCount = 6;
        private const int TankFrameCount = 6;

        private Vector2 previousCenter = Vector2.Zero;

        private float hitPoints;
        public float MaxHitPoints;

        private EnemyType type;

        private readonly int initialHitScore;
        private int hitScore;
        private readonly int initialKillScore;
        private int killScore;

        private readonly float initialShotChance;
        private float shotChance;

        #endregion

        #region Constructors

        private Enemy(Texture2D texture, Vector2 location,
                     float speed, int hitScore, int killScore, float shotChance,
                     int collisionRadius, EnemyType type)
        {
            Rectangle initialFrame = new Rectangle();
            int frameCount = 0;

            switch (type)
            {
                case EnemyType.Easy:
                    initialFrame = EasySource;
                    frameCount = EasyFrameCount;
                    break;
                case EnemyType.Medium:
                    initialFrame = MediumSource;
                    frameCount = MediumFrameCount;
                    break;
                case EnemyType.Hard:
                    initialFrame = HardSource;
                    frameCount = HardFrameCount;
                    break;
                case EnemyType.Speeder:
                    initialFrame = SpeederSource;
                    frameCount = SpeederFrameCount;
                    break;
                case EnemyType.Tank:
                    initialFrame = TankSource;
                    frameCount = TankFrameCount;
                    break;
            }

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
                previousCenter = location;
                currentWayPoint = location;
                EnemySprite.CollisionRadius = collisionRadius;
            }

            this.speed = speed;

            this.initialHitScore = hitScore;
            this.hitScore = hitScore;
            this.initialKillScore = killScore;
            this.killScore = killScore;

            this.initialShotChance = shotChance;
            this.shotChance = shotChance;

            this.type = type;
        }

        #endregion

        #region Methods

        public static Enemy CreateEasyEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                    location,
                                    150.0f,
                                    50,
                                    150,
                                    0.15f,
                                    Enemy.EnemyRadiusEasy,
                                    EnemyType.Easy);

            enemy.hitPoints = InitialHitPointsEasy;
            enemy.MaxHitPoints = InitialHitPointsEasy;

            return enemy;
        }

        public static Enemy CreateMediumEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                125.0f,
                                75,
                                200,
                                0.25f,
                                Enemy.EnemyRadiusMedium,
                                EnemyType.Medium);

            enemy.hitPoints = InitialHitPointsMedium;
            enemy.MaxHitPoints = InitialHitPointsMedium;

            return enemy;
        }

        public static Enemy CreateHardEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                100.0f,
                                100,
                                300,
                                0.3f,
                                Enemy.EnemyRadiusHard,
                                EnemyType.Hard);

            enemy.hitPoints = InitialHitPointsHard;
            enemy.MaxHitPoints = InitialHitPointsHard;

            return enemy;
        }

        public static Enemy CreateSpeederEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                175.0f,
                                100,
                                300,
                                0.25f,
                                Enemy.EnemyRadiusSpeeder,
                                EnemyType.Speeder);

            enemy.hitPoints = InitialHitPointsSpeeder;
            enemy.MaxHitPoints = InitialHitPointsSpeeder;

            return enemy;
        }

        public static Enemy CreateTankEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                100.0f,
                                50,
                                500,
                                0.35f,
                                Enemy.EnemyRadiusTank,
                                EnemyType.Tank);

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
            if (Vector2.Distance(EnemySprite.Center, currentWayPoint) <
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
                Vector2 heading = currentWayPoint - EnemySprite.Center;

                if (heading != Vector2.Zero)
                {
                    heading.Normalize();
                }

                heading *= speed;
                EnemySprite.Velocity = heading;
                previousCenter = EnemySprite.Center;
                EnemySprite.Update(gameTime);
                EnemySprite.Rotation = (float)Math.Atan2(EnemySprite.Center.Y - previousCenter.Y,
                                                         EnemySprite.Center.X - previousCenter.X);

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

            //this.shotChance = initialShotChance + (lvl - 1) * 0.0075f; // Version 1.0: init + lvl * 0.01f
            this.shotChance = initialShotChance + (float)Math.Sqrt(lvl - 1) * 0.02f * initialShotChance * (lvl - 1) * 0.002f;

            this.MaxHitPoints += (lvl - 1);
            this.HitPoints += (lvl - 1);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Enemy sprite
            enemySprite.Activated(reader);

            //Waypoints
            int waypointsCount = Int32.Parse(reader.ReadLine());
            wayPoints.Clear();
            for (int i = 0; i < waypointsCount; ++i)
            {
                Vector2 v = new Vector2(Single.Parse(reader.ReadLine()),
                                        Single.Parse(reader.ReadLine()));
                wayPoints.Enqueue(v);
            }

            this.currentWayPoint = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));

            this.speed = Single.Parse(reader.ReadLine());

            this.previousCenter = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

            this.hitPoints = Single.Parse(reader.ReadLine());
            this.MaxHitPoints = Single.Parse(reader.ReadLine());

            this.type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

            this.hitScore = Int32.Parse(reader.ReadLine());

            this.killScore = Int32.Parse(reader.ReadLine());

            this.shotChance = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Enemy sprite
            enemySprite.Deactivated(writer);

            // Waypoints
            int wayPointsCount = wayPoints.Count;
            writer.WriteLine(wayPointsCount);
            
            for (int i = 0; i < wayPointsCount; ++i)
            {
                Vector2 wayPoint = wayPoints.Dequeue();
                writer.WriteLine(wayPoint.X);
                writer.WriteLine(wayPoint.Y);
            }

            writer.WriteLine(currentWayPoint.X);
            writer.WriteLine(currentWayPoint.Y);

            writer.WriteLine(speed);

            writer.WriteLine(previousCenter.X);
            writer.WriteLine(previousCenter.Y);

            writer.WriteLine(hitPoints);
            writer.WriteLine(MaxHitPoints);

            writer.WriteLine(type);

            writer.WriteLine(hitScore);

            writer.WriteLine(killScore);

            writer.WriteLine(shotChance);
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

        public EnemyType Type
        {
            get
            {
                return this.type;
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

        public int InitialHitScore
        {
            get
            {
                return this.initialHitScore;
            }
        }

        public int KillScore
        {
            get
            {
                return this.killScore;
            }
        }

        public int InitialKillScore
        {
            get
            {
                return this.initialKillScore;
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
