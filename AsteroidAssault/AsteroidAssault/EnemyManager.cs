using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpacepiXX
{
    class EnemyManager : ILevel
    {
        #region Members

        private Texture2D texture;

        public List<Enemy> Enemies = new List<Enemy>();

        public ShotManager EnemyShotManager;
        private PlayerManager playerManager;

        public const int SOFT_ROCKET_EXPLOSION_RADIUS = 100;
        public const float ROCKET_POWER_AT_CENTER = 200.0f;

        public const int DAMAGE_ROCKET_MIN = 75;
        public const int DAMAGE_ROCKET_MAX = 100;
        public const int DAMAGE_LASER_MIN = 33;
        public const int DAMAGE_LASER_MAX = 66;

        public int MinShipsPerWave = 4;
        public int MaxShipsPerWave = 8;
        private float nextWaveTimer = 0.0f;
        private const float initialNextWaveMinTimer = 5.0f;
        private float nextWaveMinTimer = 5.0f;
        private float shipSpawnTimer = 0.0f;
        private float shipSpawnWaitTimer = 0.5f;

        private List<List<Vector2>> pathWayPoints = new List<List<Vector2>>();

        private Dictionary<int, WaveInfo> waveSpawns = new Dictionary<int, WaveInfo>();

        public bool IsActive = false;

        private Random rand = new Random();

        private int currentLevel;

        private readonly Rectangle screen = new Rectangle(0, 0, 800, 480);

        #endregion

        #region Constructors

        public EnemyManager(Texture2D texture, PlayerManager playerManager,
                            Rectangle screenBounds)
        {
            this.texture = texture;
            this.playerManager = playerManager;

            EnemyShotManager = new ShotManager(texture,
                                               new Rectangle(100, 430, 15, 5),
                                               4,
                                               2,
                                               150.0f,
                                               screenBounds);

            setUpWayPoints();

            this.currentLevel = 1;
        }

        #endregion

        #region Methods

        private void setUpWayPoints()
        {
            List<Vector2> path0 = new List<Vector2>();
            path0.Add(new Vector2(850, 100));
            path0.Add(new Vector2(-50, 100));
            pathWayPoints.Add(path0);
            waveSpawns.Add(0, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path1 = new List<Vector2>();
            path1.Add(new Vector2(-50, 150));
            path1.Add(new Vector2(850, 150));
            pathWayPoints.Add(path1);
            waveSpawns.Add(1, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path2 = new List<Vector2>();
            path2.Add(new Vector2(850, 200));
            path2.Add(new Vector2(-50, 200));
            pathWayPoints.Add(path2);
            waveSpawns.Add(2, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path3 = new List<Vector2>();
            path3.Add(new Vector2(-50, 250));
            path3.Add(new Vector2(850, 250));
            pathWayPoints.Add(path3);
            waveSpawns.Add(3, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path4 = new List<Vector2>();
            path4.Add(new Vector2(850, 300));
            path4.Add(new Vector2(-50, 300));
            pathWayPoints.Add(path4);
            waveSpawns.Add(4, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path5 = new List<Vector2>();
            path5.Add(new Vector2(-50, 400));
            path5.Add(new Vector2(300, 70));
            path5.Add(new Vector2(500, 70));
            path5.Add(new Vector2(850, 400));
            pathWayPoints.Add(path5);
            waveSpawns.Add(5, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path6 = new List<Vector2>();
            path6.Add(new Vector2(350, -50));
            path6.Add(new Vector2(350, 275));
            path6.Add(new Vector2(300, 325));
            path6.Add(new Vector2(-50, 325));
            pathWayPoints.Add(path6);
            waveSpawns.Add(6, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path7 = new List<Vector2>();
            path7.Add(new Vector2(450, -50));
            path7.Add(new Vector2(450, 275));
            path7.Add(new Vector2(500, 325));
            path7.Add(new Vector2(850, 325));
            pathWayPoints.Add(path7);
            waveSpawns.Add(7, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path8 = new List<Vector2>();
            path8.Add(new Vector2(200, -50));
            path8.Add(new Vector2(200, 530));
            pathWayPoints.Add(path8);
            waveSpawns.Add(8, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path9 = new List<Vector2>();
            path9.Add(new Vector2(400, -50));
            path9.Add(new Vector2(400, 530));
            pathWayPoints.Add(path9);
            waveSpawns.Add(9, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path10 = new List<Vector2>();
            path10.Add(new Vector2(600, -50));
            path10.Add(new Vector2(600, 530));
            pathWayPoints.Add(path10);
            waveSpawns.Add(10, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path11 = new List<Vector2>();
            path11.Add(new Vector2(100, -50));
            path11.Add(new Vector2(100, 325));
            path11.Add(new Vector2(150, 375));
            path11.Add(new Vector2(250, 375));
            path11.Add(new Vector2(300, 325));
            path11.Add(new Vector2(300, -50));
            pathWayPoints.Add(path11);
            waveSpawns.Add(11, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path12 = new List<Vector2>();
            path12.Add(new Vector2(700, -50));
            path12.Add(new Vector2(700, 325));
            path12.Add(new Vector2(650, 375));
            path12.Add(new Vector2(550, 375));
            path12.Add(new Vector2(500, 325));
            path12.Add(new Vector2(500, -50));
            pathWayPoints.Add(path12);
            waveSpawns.Add(12, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path13 = new List<Vector2>();
            path13.Add(new Vector2(-50, 50));
            path13.Add(new Vector2(350, 275));
            path13.Add(new Vector2(450, 275));
            path13.Add(new Vector2(850, 50));
            pathWayPoints.Add(path13);
            waveSpawns.Add(13, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path14 = new List<Vector2>();
            path14.Add(new Vector2(850, 100));
            path14.Add(new Vector2(450, 325));
            path14.Add(new Vector2(350, 325));
            path14.Add(new Vector2(-50, 100));
            pathWayPoints.Add(path14);
            waveSpawns.Add(14, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path15 = new List<Vector2>();
            path15.Add(new Vector2(-50, 150));
            path15.Add(new Vector2(350, 375));
            path15.Add(new Vector2(450, 375));
            path15.Add(new Vector2(850, 150));
            pathWayPoints.Add(path15);
            waveSpawns.Add(15, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path16 = new List<Vector2>();
            path16.Add(new Vector2(650, -50));
            path16.Add(new Vector2(650, 100));
            path16.Add(new Vector2(600, 200));
            path16.Add(new Vector2(550, 250));
            path16.Add(new Vector2(450, 300));
            path16.Add(new Vector2(350, 300));
            path16.Add(new Vector2(250, 250));
            path16.Add(new Vector2(200, 200));
            path16.Add(new Vector2(150, 100));
            path16.Add(new Vector2(150, -50));
            pathWayPoints.Add(path16);
            waveSpawns.Add(16, new WaveInfo(0, Enemy.EnemyType.Easy));

            //Version 1.1:
            List<Vector2> path17 = new List<Vector2>();
            path17.Add(new Vector2(-50, 150));
            path17.Add(new Vector2(725, 150));
            path17.Add(new Vector2(750, 175));
            path17.Add(new Vector2(750, 250));
            path17.Add(new Vector2(725, 275));
            path17.Add(new Vector2(-50, 275));
            pathWayPoints.Add(path17);
            waveSpawns.Add(17, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path18 = new List<Vector2>();
            path18.Add(new Vector2(850, 200));
            path18.Add(new Vector2(75, 200));
            path18.Add(new Vector2(50, 225));
            path18.Add(new Vector2(50, 300));
            path18.Add(new Vector2(75, 325));
            path18.Add(new Vector2(850, 325));
            pathWayPoints.Add(path18);
            waveSpawns.Add(18, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path19 = new List<Vector2>();
            path19.Add(new Vector2(850, 250));
            path19.Add(new Vector2(75, 250));
            path19.Add(new Vector2(50, 225));
            path19.Add(new Vector2(50, 150));
            path19.Add(new Vector2(75, 125));
            path19.Add(new Vector2(850, 125));
            pathWayPoints.Add(path19);
            waveSpawns.Add(19, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path20 = new List<Vector2>();
            path20.Add(new Vector2(-50, 25));
            path20.Add(new Vector2(725, 25));
            path20.Add(new Vector2(750, 50));
            path20.Add(new Vector2(750, 150));
            path20.Add(new Vector2(725, 175));
            path20.Add(new Vector2(-50, 175));
            pathWayPoints.Add(path20);
            waveSpawns.Add(20, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path21 = new List<Vector2>();
            path21.Add(new Vector2(-50, 100));
            path21.Add(new Vector2(850, 350));
            pathWayPoints.Add(path21);
            waveSpawns.Add(21, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path22 = new List<Vector2>();
            path22.Add(new Vector2(850, 100));
            path22.Add(new Vector2(-50, 350));
            pathWayPoints.Add(path22);
            waveSpawns.Add(22, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path23 = new List<Vector2>();
            path23.Add(new Vector2(-100, 50));
            path23.Add(new Vector2(150, 50));
            path23.Add(new Vector2(200, 75));
            path23.Add(new Vector2(200, 125));
            path23.Add(new Vector2(150, 150));
            path23.Add(new Vector2(150, 175));
            path23.Add(new Vector2(200, 200));
            path23.Add(new Vector2(600, 200));
            path23.Add(new Vector2(850, 480));
            pathWayPoints.Add(path23);
            waveSpawns.Add(23, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path24 = new List<Vector2>();
            path24.Add(new Vector2(850, 50));
            path24.Add(new Vector2(650, 50));
            path24.Add(new Vector2(600, 75));
            path24.Add(new Vector2(600, 125));
            path24.Add(new Vector2(650, 150));
            path24.Add(new Vector2(650, 175));
            path24.Add(new Vector2(600, 200));
            path24.Add(new Vector2(200, 200));
            path24.Add(new Vector2(-50, 480));
            pathWayPoints.Add(path24);
            waveSpawns.Add(24, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path25 = new List<Vector2>();
            path25.Add(new Vector2(600, -100));
            path25.Add(new Vector2(600, 250));
            path25.Add(new Vector2(580, 275));
            path25.Add(new Vector2(500, 250));
            path25.Add(new Vector2(500, 200));
            path25.Add(new Vector2(450, 175));
            path25.Add(new Vector2(400, 150));
            path25.Add(new Vector2(-100, 150));
            pathWayPoints.Add(path25);
            waveSpawns.Add(25, new WaveInfo(0, Enemy.EnemyType.Easy));

            List<Vector2> path26 = new List<Vector2>();
            path26.Add(new Vector2(-50, 400));
            path26.Add(new Vector2(300, 70));
            path26.Add(new Vector2(500, 70));
            path26.Add(new Vector2(850, 400));
            pathWayPoints.Add(path26);
            waveSpawns.Add(26, new WaveInfo(0, Enemy.EnemyType.Easy));

        }

        public void SpawnEnemy(int path, Enemy.EnemyType type)
        {
            Enemy newEnemy;

            switch (type)
            {
                case Enemy.EnemyType.Medium:
                    newEnemy = Enemy.CreateMediumEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;

                case Enemy.EnemyType.Hard:
                    newEnemy = Enemy.CreateHardEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;

                case Enemy.EnemyType.Speeder:
                    newEnemy = Enemy.CreateSpeederEnemy(texture,
                                                        pathWayPoints[path][0]);
                    break;

                case Enemy.EnemyType.Tank:
                    newEnemy = Enemy.CreateTankEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;

                default:
                    newEnemy = Enemy.CreateEasyEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;
            }

            newEnemy.SetLevel(currentLevel);

            for (int x = 0; x < pathWayPoints[path].Count; x++)
            {
                newEnemy.AddWayPoint(pathWayPoints[path][x]);
            }

            Enemies.Add(newEnemy);
        }

        public void SpawnWave(int waveType)
        {
            int spawns = rand.Next(MinShipsPerWave, MaxShipsPerWave + 1);

            Enemy.EnemyType type;
            int rnd = rand.Next(0, 13);

            switch (rnd)
            {
                case 0:
                case 1:
                case 2:
                    type = Enemy.EnemyType.Medium;
                    break;

                case 3:
                case 4:
                    type = Enemy.EnemyType.Hard;
                    break;

                case 5:
                case 6:
                    type = Enemy.EnemyType.Speeder;
                    break;

                case 7:
                    type = Enemy.EnemyType.Tank;
                    break;

                default:
                    type = Enemy.EnemyType.Easy;
                    break;
            }

            waveSpawns[waveType] = new WaveInfo(spawns, type);
        }

        private void updateWaveSpawns(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            shipSpawnTimer += elapsed;

            if (shipSpawnTimer >= shipSpawnWaitTimer)
            {
                for (int x = waveSpawns.Count - 1; x >= 0; --x)
                {
                    if (waveSpawns[x].SpawnsCount > 0)
                    {
                        waveSpawns[x].DecrementSpawns();

                        SpawnEnemy(x, waveSpawns[x].Type);
                    }
                }

                shipSpawnTimer = 0.0f;
            }

            nextWaveTimer += elapsed;

            if (nextWaveTimer > nextWaveMinTimer)
            {
                int rnd1 = rand.Next(0, pathWayPoints.Count);
                int rnd2 = rand.Next(0, pathWayPoints.Count);

                int rndSecondSpawn = rand.Next(0, 10); // 10%
                
                SpawnWave(rnd1);

                if (rndSecondSpawn == 0 && rnd1 != rnd2)
                    SpawnWave(rnd2);

                nextWaveTimer = 0.0f;
            }
        }

        public void Update(GameTime gameTime)
        {
            EnemyShotManager.Update(gameTime);

            for (int x = Enemies.Count - 1; x >= 0; --x)
            {
                Enemies[x].Update(gameTime);

                if (!Enemies[x].IsActive())
                {
                    Enemies.RemoveAt(x);
                }
                else
                {
                    if ((float)rand.Next(0, 1000) / 10 <= Enemies[x].ShotChance &&
                        !playerManager.IsDestroyed &&
                         screen.Contains((int)Enemies[x].EnemySprite.Center.X,
                                         (int)Enemies[x].EnemySprite.Center.Y))
                    {
                        Vector2 fireLocation = Enemies[x].EnemySprite.Location;
                        fireLocation += Enemies[x].GunOffset;

                        Vector2 shotDirection = (playerManager.playerSprite.Center - fireLocation);

                        shotDirection.Normalize();

                        if (Enemies[x].Type == Enemy.EnemyType.Tank &&
                            (float)rand.Next(0, 7) == 0) // 14.2%
                        {
                            EnemyShotManager.FireRocket(fireLocation,
                                                             shotDirection,
                                                             false,
                                                             Color.White,
                                                             true);
                        }
                        else
                        {
                            EnemyShotManager.FireShot(fireLocation,
                                                      shotDirection,
                                                      false,
                                                      new Color(1.0f, 0.1f, 0.1f),
                                                      true);
                        }
                    }
                }
            }

            if (this.IsActive)
            {
                updateWaveSpawns(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyShotManager.Draw(spriteBatch);

            foreach (var enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        public void Reset()
        {
            this.Enemies.Clear();
            this.EnemyShotManager.Shots.Clear();

            this.nextWaveTimer = 0.0f;
            this.shipSpawnTimer = 0.0f;

            this.nextWaveMinTimer = initialNextWaveMinTimer;

            for (int i = 0; i < waveSpawns.Count; i++)
            {
                waveSpawns[i] = new WaveInfo(0, Enemy.EnemyType.Easy);
            }

            this.IsActive = true;
        }

        public void SetLevel(int lvl)
        {
            this.currentLevel = lvl;

            // Version 1.0
            //float tmp = initialNextWaveMinTimer - (lvl - 1) * 0.075f;

            // Version 1.1 (old)
            //float tmp = initialNextWaveMinTimer - (float)Math.Sqrt(((lvl - 1) * 0.15));

            // Version 1.1
            float tmp = (int)(initialNextWaveMinTimer - (float)Math.Sqrt(lvl - 1) * 0.075f + 0.015 * (lvl - 1)); // 5 - WURZEL(A2-1) / 2 * 0,15 - 0,015 * (A2 - 1)

            this.nextWaveMinTimer = Math.Max(tmp, 1.0f);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Enemies
            int enemiesCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < enemiesCount; ++i)
            {
                Enemy.EnemyType type = Enemy.EnemyType.Easy;
                Enemy e;

                type = (Enemy.EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

                switch (type)
                {
                    case Enemy.EnemyType.Easy:
                        e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                    case Enemy.EnemyType.Medium:
                        e = Enemy.CreateMediumEnemy(texture, Vector2.Zero);
                        break;
                    case Enemy.EnemyType.Hard:
                        e = Enemy.CreateHardEnemy(texture, Vector2.Zero);
                        break;
                    case Enemy.EnemyType.Speeder:
                        e = Enemy.CreateSpeederEnemy(texture, Vector2.Zero);
                        break;
                    case Enemy.EnemyType.Tank:
                        e = Enemy.CreateTankEnemy(texture, Vector2.Zero);
                        break;
                    default:
                        e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                }
                e.Activated(reader);

                Enemies.Add(e);
            }

            EnemyShotManager.Activated(reader);

            this.MinShipsPerWave = Int32.Parse(reader.ReadLine());
            this.MaxShipsPerWave = Int32.Parse(reader.ReadLine());

            this.nextWaveTimer = Single.Parse(reader.ReadLine());
            this.nextWaveMinTimer = Single.Parse(reader.ReadLine());
            this.shipSpawnTimer = Single.Parse(reader.ReadLine());
            this.shipSpawnWaitTimer = Single.Parse(reader.ReadLine());

            // Wave spawns
            int waveSpawnsCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < waveSpawnsCount; ++i)
            {
                int idx = Int32.Parse(reader.ReadLine());
                WaveInfo waveInfo = new WaveInfo();
                waveInfo.Activated(reader);
                waveSpawns[idx] = waveInfo;
            }

            this.IsActive = Boolean.Parse(reader.ReadLine());

            this.currentLevel = Int32.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            //Enemies
            writer.WriteLine(Enemies.Count);

            for (int i = 0; i < Enemies.Count; ++i)
            {
                writer.WriteLine(Enemies[i].Type);
                Enemies[i].Deactivated(writer);
            }

            EnemyShotManager.Deactivated(writer);

            writer.WriteLine(MinShipsPerWave);
            writer.WriteLine(MaxShipsPerWave);

            writer.WriteLine(nextWaveTimer);
            writer.WriteLine(nextWaveMinTimer);
            writer.WriteLine(shipSpawnTimer);
            writer.WriteLine(shipSpawnWaitTimer);

            // Wave spawns
            writer.WriteLine(waveSpawns.Count);

            foreach (var waveSpawn in waveSpawns)
            {
                writer.WriteLine(waveSpawn.Key);
                waveSpawn.Value.Deactivated(writer);
            }

            writer.WriteLine(IsActive);

            writer.WriteLine(currentLevel);
        }

        #endregion
    }
}
