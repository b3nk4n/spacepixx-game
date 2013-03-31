using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpacepiXX
{
    class Boss : ILevel
    {
        #region Members

        private Sprite bossSprite;
        private Queue<Vector2> wayPoints = new Queue<Vector2>();
        private Vector2 currentWayPoint = Vector2.Zero;

        private float speed;

        private const int BossRadiusEasy = 30;
        private const int BossRadiusMedium = 40;
        private const int BossRadiusHard = 36;
        private const int BossRadiusSpeeder = 36;
        private const int BossRadiusTank = 36;

        private const float InitialHitPointsEasy = 1750.0f;
        private const float InitialHitPointsMedium = 2000.0f;
        private const float InitialHitPointsHard = 2250.0f;
        private const float InitialHitPointsSpeeder = 1750.0f;
        private const float InitialHitPointsTank = 2500.0f;

        private readonly Rectangle EasySource = new Rectangle(350, 300,
                                                              100, 100);
        private readonly Rectangle MediumSource = new Rectangle(0, 500,
                                                              100, 100);
        private readonly Rectangle HardSource = new Rectangle(600, 500,
                                                                100, 100);
        private readonly Rectangle SpeederSource = new Rectangle(600, 600,
                                                              100, 100);
        private readonly Rectangle TankSource = new Rectangle(0, 600,
                                                              100, 100);
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

        public static PlayerManager Player;

        #endregion

        #region Constructors

        private Boss(Texture2D texture, Vector2 location,
                     float speed, int hitScore, int killScore,
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

            bossSprite = new Sprite(location,
                                     texture,
                                     initialFrame,
                                     Vector2.Zero);

            for (int x = 1; x < frameCount; x++)
            {
                BossSprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
                previousCenter = location;
                currentWayPoint = location;
                BossSprite.CollisionRadius = collisionRadius;
            }

            this.speed = speed;

            this.initialHitScore = hitScore;
            this.hitScore = hitScore;
            this.initialKillScore = killScore;
            this.killScore = killScore;

            this.type = type;
        }

        #endregion

        #region Methods

        public static Boss CreateEasyBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                 location,
                                 50.0f,
                                 75,
                                 10000,
                                 Boss.BossRadiusEasy,
                                 EnemyType.Easy);

            boss.hitPoints = InitialHitPointsEasy;
            boss.MaxHitPoints = InitialHitPointsEasy;

            return boss;
        }

        public static Boss CreateMediumBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                 location,
                                 50.0f,
                                 100,
                                 10000,
                                 Boss.BossRadiusMedium,
                                 EnemyType.Medium);

            boss.hitPoints = InitialHitPointsMedium;
            boss.MaxHitPoints = InitialHitPointsMedium;

            return boss;
        }

        public static Boss CreateHardBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                location,
                                40.0f,
                                125,
                                12500,
                                Boss.BossRadiusHard,
                                EnemyType.Hard);

            boss.hitPoints = InitialHitPointsHard;
            boss.MaxHitPoints = InitialHitPointsHard;

            return boss;
        }

        public static Boss CreateSpeederBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                location,
                                60.0f,
                                100,
                                12500,
                                Boss.BossRadiusSpeeder,
                                EnemyType.Speeder);

            boss.hitPoints = InitialHitPointsSpeeder;
            boss.MaxHitPoints = InitialHitPointsSpeeder;

            return boss;
        }

        public static Boss CreateTankBoss(Texture2D texture, Vector2 location)
        {
            Boss boss = new Boss(texture,
                                location,
                                30.0f,
                                150,
                                15000,
                                Boss.BossRadiusTank,
                                EnemyType.Tank);

            boss.hitPoints = InitialHitPointsTank;
            boss.MaxHitPoints = InitialHitPointsTank;

            return boss;
        }

        public void AddWayPoint(Vector2 wayPoint)
        {
            wayPoints.Enqueue(wayPoint);
        }

        public bool WayPointReached()
        {
            if (Vector2.Distance(BossSprite.Center, currentWayPoint) <
                (float)BossSprite.Source.Width / 2)
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
                Vector2 heading = currentWayPoint - BossSprite.Center;

                if (heading != Vector2.Zero)
                {
                    heading.Normalize();
                }

                heading *= speed;
                BossSprite.Velocity = heading;
                previousCenter = BossSprite.Center;
                BossSprite.Update(gameTime);
                BossSprite.Rotation = (float)Math.Atan2(BossSprite.Center.Y - previousCenter.Y,
                                                         BossSprite.Center.X - previousCenter.X);

                if (WayPointReached())
                {
                    if (wayPoints.Count > 0)
                    {
                        currentWayPoint = wayPoints.Dequeue();
                    }
                }

                float factor = (float)Math.Max((this.hitPoints / this.MaxHitPoints), 0.66f);
                this.BossSprite.TintColor = Color.White * factor;

                this.BossSprite.RotateTo(Player.playerSprite.Center - this.BossSprite.Center);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
            {
                BossSprite.Draw(spriteBatch);
            }
        }

        public void SetLevel(int lvl)
        {
            this.hitScore = initialHitScore + ((lvl - 1) / 5) * (initialHitScore / 2);
            this.killScore = initialKillScore + ((lvl - 1) / 5) * (initialKillScore / 2);

            this.MaxHitPoints += 250 * ((lvl - 1) / 5);
            this.HitPoints += 250 * ((lvl - 1) / 5);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Boss sprite
            bossSprite.Activated(reader);

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
        }

        public void Deactivated(StreamWriter writer)
        {
            // Boss sprite
            bossSprite.Deactivated(writer);

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
        }

        #endregion

        #region Properties

        public Sprite BossSprite
        {
            get
            {
                return this.bossSprite;
            }
        }

        public EnemyType Type
        {
            get
            {
                return this.type;
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
