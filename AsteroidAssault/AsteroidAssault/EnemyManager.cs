using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class EnemyManager : ILevel
    {
        #region Members

        private Texture2D texture;

        public List<Enemy> Enemies = new List<Enemy>();

        public ShotManager EnemyShotManager;
        private PlayerManager playerManager;

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

        private float shotPower = 75.0f;

        #endregion

        #region Constructors

        public EnemyManager(Texture2D texture, PlayerManager playerManager,
                            Rectangle screenBounds)
        {
            this.texture = texture;
            this.playerManager = playerManager;

            EnemyShotManager = new ShotManager(texture,
                                               new Rectangle(0, 430, 5, 5),
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
            //List<Vector2> path2 = new List<Vector2>();
            //path2.Add(new Vector2(-100, 50));
            //path2.Add(new Vector2(150, 50));
            //path2.Add(new Vector2(200, 75));
            //path2.Add(new Vector2(200, 125));
            //path2.Add(new Vector2(150, 150));
            //path2.Add(new Vector2(150, 175));
            //path2.Add(new Vector2(200, 200));
            //path2.Add(new Vector2(600, 200));
            //path2.Add(new Vector2(850, 480));
            //pathWayPoints.Add(path2);
            //waveSpawns.Add(2, new WaveInfo(0, Enemy.EnemyType.Easy));

            //List<Vector2> path3 = new List<Vector2>();
            //path3.Add(new Vector2(600, -100));
            //path3.Add(new Vector2(600, 250));
            //path3.Add(new Vector2(580, 275));
            //path3.Add(new Vector2(500, 250));
            //path3.Add(new Vector2(500, 200));
            //path3.Add(new Vector2(450, 175));
            //path3.Add(new Vector2(400, 150));
            //path3.Add(new Vector2(-100, 150));
            //pathWayPoints.Add(path3);
            //waveSpawns.Add(3, new WaveInfo(0, Enemy.EnemyType.Easy));

            //List<Vector2> path4 = new List<Vector2>();
            //path4.Add(new Vector2(-50, 400));
            //path4.Add(new Vector2(300, 70));
            //path4.Add(new Vector2(500, 70));
            //path4.Add(new Vector2(850, 400));
            //pathWayPoints.Add(path4);
            //waveSpawns.Add(4, new WaveInfo(0, Enemy.EnemyType.Easy));

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
                SpawnWave(rand.Next(0, pathWayPoints.Count));
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
                        !playerManager.IsDestroyed)
                    {
                        Vector2 fireLocation = Enemies[x].EnemySprite.Location;
                        fireLocation += Enemies[x].GunOffset;

                        Vector2 shotDirection = (playerManager.playerSprite.Center - fireLocation);

                        shotDirection.Normalize();

                        EnemyShotManager.FireShot(fireLocation,
                                                  shotDirection,
                                                  false,
                                                  new Color(1.0f, 0.1f, 0.1f),
                                                  true);
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

            float tmp = initialNextWaveMinTimer - (lvl - 1) * 0.075f;

            this.nextWaveMinTimer = Math.Max(tmp, 1.0f);
        }

        #endregion

        #region Properties

        public float ShotPower
        {
            get
            {
                return this.shotPower;
            }
        }

        #endregion
    }
}
