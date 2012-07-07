using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpacepiXX
{
    /// <summary>
    /// Manages the enemies.
    /// </summary>
    class EnemyManager : ILevel
    {
        #region Members

        private Texture2D texture;

        public List<Enemy> Enemies = new List<Enemy>(64);

        public ShotManager EnemyShotManager;
        private PlayerManager playerManager;

        public const int SOFT_ROCKET_EXPLOSION_RADIUS = 100;
        public const float ROCKET_POWER_AT_CENTER = 200.0f;

        public const int DAMAGE_ROCKET_MIN = 75;
        public const int DAMAGE_ROCKET_MAX = 100;
        public const int DAMAGE_LASER_MIN = 40;
        public const int DAMAGE_LASER_MAX = 60;

        public int MinShipsPerWave = 4;
        public int MaxShipsPerWave = 8;
        private float nextWaveTimer = 0.0f;
        public const float InitialNextWaveMinTimer = 5.0f;
        private float nextWaveMinTimer = 5.0f;
        private float shipSpawnTimer = 0.0f;
        private float shipSpawnWaitTimer = 0.5f;

        private List<List<Vector2>> pathWayPoints = new List<List<Vector2>>(32);

        private Dictionary<int, WaveInfo> waveSpawns = new Dictionary<int, WaveInfo>(16);

        public bool IsActive = false;

        private Random rand = new Random();

        private int currentLevel;

        private readonly Rectangle screen = new Rectangle(0, 0, 800, 480);

        // Angry/Friendly
        private float sentimentTimer = 0.0f;
        public const float SENTIMENT_DURATION = 20.0f;
        private float sentimentFactor = SENTIMENT_NORMAL_FACTOR;
        public const float FRIENDLY_FACTOR = 3.0f;
        public const float SENTIMENT_NORMAL_FACTOR = 1.0f;
        public const float ANGRY_FACTOR = 0.5f;

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
            waveSpawns.Add(0, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path1 = new List<Vector2>();
            path1.Add(new Vector2(-50, 150));
            path1.Add(new Vector2(850, 150));
            pathWayPoints.Add(path1);
            waveSpawns.Add(1, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path2 = new List<Vector2>();
            path2.Add(new Vector2(850, 200));
            path2.Add(new Vector2(-50, 200));
            pathWayPoints.Add(path2);
            waveSpawns.Add(2, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path3 = new List<Vector2>();
            path3.Add(new Vector2(-50, 250));
            path3.Add(new Vector2(850, 250));
            pathWayPoints.Add(path3);
            waveSpawns.Add(3, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path4 = new List<Vector2>();
            path4.Add(new Vector2(850, 300));
            path4.Add(new Vector2(-50, 300));
            pathWayPoints.Add(path4);
            waveSpawns.Add(4, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path5 = new List<Vector2>();
            path5.Add(new Vector2(-50, 400));
            path5.Add(new Vector2(300, 70));
            path5.Add(new Vector2(500, 70));
            path5.Add(new Vector2(850, 400));
            pathWayPoints.Add(path5);
            waveSpawns.Add(5, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path6 = new List<Vector2>();
            path6.Add(new Vector2(350, -50));
            path6.Add(new Vector2(350, 275));
            path6.Add(new Vector2(300, 325));
            path6.Add(new Vector2(-50, 325));
            pathWayPoints.Add(path6);
            waveSpawns.Add(6, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path7 = new List<Vector2>();
            path7.Add(new Vector2(450, -50));
            path7.Add(new Vector2(450, 275));
            path7.Add(new Vector2(500, 325));
            path7.Add(new Vector2(850, 325));
            pathWayPoints.Add(path7);
            waveSpawns.Add(7, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path8 = new List<Vector2>();
            path8.Add(new Vector2(200, -50));
            path8.Add(new Vector2(200, 530));
            pathWayPoints.Add(path8);
            waveSpawns.Add(8, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path9 = new List<Vector2>();
            path9.Add(new Vector2(400, -50));
            path9.Add(new Vector2(400, 530));
            pathWayPoints.Add(path9);
            waveSpawns.Add(9, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path10 = new List<Vector2>();
            path10.Add(new Vector2(600, -50));
            path10.Add(new Vector2(600, 530));
            pathWayPoints.Add(path10);
            waveSpawns.Add(10, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path11 = new List<Vector2>();
            path11.Add(new Vector2(100, -50));
            path11.Add(new Vector2(100, 325));
            path11.Add(new Vector2(150, 375));
            path11.Add(new Vector2(250, 375));
            path11.Add(new Vector2(300, 325));
            path11.Add(new Vector2(300, -50));
            pathWayPoints.Add(path11);
            waveSpawns.Add(11, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path12 = new List<Vector2>();
            path12.Add(new Vector2(700, -50));
            path12.Add(new Vector2(700, 325));
            path12.Add(new Vector2(650, 375));
            path12.Add(new Vector2(550, 375));
            path12.Add(new Vector2(500, 325));
            path12.Add(new Vector2(500, -50));
            pathWayPoints.Add(path12);
            waveSpawns.Add(12, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path13 = new List<Vector2>();
            path13.Add(new Vector2(-50, 50));
            path13.Add(new Vector2(350, 275));
            path13.Add(new Vector2(450, 275));
            path13.Add(new Vector2(850, 50));
            pathWayPoints.Add(path13);
            waveSpawns.Add(13, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path14 = new List<Vector2>();
            path14.Add(new Vector2(850, 100));
            path14.Add(new Vector2(450, 325));
            path14.Add(new Vector2(350, 325));
            path14.Add(new Vector2(-50, 100));
            pathWayPoints.Add(path14);
            waveSpawns.Add(14, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path15 = new List<Vector2>();
            path15.Add(new Vector2(-50, 150));
            path15.Add(new Vector2(350, 375));
            path15.Add(new Vector2(450, 375));
            path15.Add(new Vector2(850, 150));
            pathWayPoints.Add(path15);
            waveSpawns.Add(15, new WaveInfo(0, EnemyType.Easy));

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
            waveSpawns.Add(16, new WaveInfo(0, EnemyType.Easy));

            //Version 1.1:
            List<Vector2> path17 = new List<Vector2>();
            path17.Add(new Vector2(-50, 150));
            path17.Add(new Vector2(725, 150));
            path17.Add(new Vector2(750, 175));
            path17.Add(new Vector2(750, 250));
            path17.Add(new Vector2(725, 275));
            path17.Add(new Vector2(-50, 275));
            pathWayPoints.Add(path17);
            waveSpawns.Add(17, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path18 = new List<Vector2>();
            path18.Add(new Vector2(850, 200));
            path18.Add(new Vector2(75, 200));
            path18.Add(new Vector2(50, 225));
            path18.Add(new Vector2(50, 300));
            path18.Add(new Vector2(75, 325));
            path18.Add(new Vector2(850, 325));
            pathWayPoints.Add(path18);
            waveSpawns.Add(18, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path19 = new List<Vector2>();
            path19.Add(new Vector2(850, 250));
            path19.Add(new Vector2(75, 250));
            path19.Add(new Vector2(50, 225));
            path19.Add(new Vector2(50, 150));
            path19.Add(new Vector2(75, 125));
            path19.Add(new Vector2(850, 125));
            pathWayPoints.Add(path19);
            waveSpawns.Add(19, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path20 = new List<Vector2>();
            path20.Add(new Vector2(-50, 25));
            path20.Add(new Vector2(725, 25));
            path20.Add(new Vector2(750, 50));
            path20.Add(new Vector2(750, 150));
            path20.Add(new Vector2(725, 175));
            path20.Add(new Vector2(-50, 175));
            pathWayPoints.Add(path20);
            waveSpawns.Add(20, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path21 = new List<Vector2>();
            path21.Add(new Vector2(-50, 100));
            path21.Add(new Vector2(850, 350));
            pathWayPoints.Add(path21);
            waveSpawns.Add(21, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path22 = new List<Vector2>();
            path22.Add(new Vector2(850, 100));
            path22.Add(new Vector2(-50, 350));
            pathWayPoints.Add(path22);
            waveSpawns.Add(22, new WaveInfo(0, EnemyType.Easy));

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
            waveSpawns.Add(23, new WaveInfo(0, EnemyType.Easy));

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
            waveSpawns.Add(24, new WaveInfo(0, EnemyType.Easy));

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
            waveSpawns.Add(25, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path26 = new List<Vector2>();
            path26.Add(new Vector2(-50, 400));
            path26.Add(new Vector2(300, 70));
            path26.Add(new Vector2(500, 70));
            path26.Add(new Vector2(850, 400));
            pathWayPoints.Add(path26);
            waveSpawns.Add(26, new WaveInfo(0, EnemyType.Easy));

            // Added in Version 2.5
            List<Vector2> path27 = new List<Vector2>();
            path27.Add(new Vector2(300, -50));
            path27.Add(new Vector2(300, 50));
            path27.Add(new Vector2(275, 100));
            path27.Add(new Vector2(225, 175));
            path27.Add(new Vector2(200, 200));
            path27.Add(new Vector2(200, 250));
            path27.Add(new Vector2(225, 275));
            path27.Add(new Vector2(325, 300));
            path27.Add(new Vector2(475, 300));
            path27.Add(new Vector2(575, 275));
            path27.Add(new Vector2(600, 250));
            path27.Add(new Vector2(600, 200));
            path27.Add(new Vector2(575, 175));
            path27.Add(new Vector2(525, 100));
            path27.Add(new Vector2(500, 50));
            path27.Add(new Vector2(500, -50));
            pathWayPoints.Add(path27);
            waveSpawns.Add(27, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path28 = new List<Vector2>();
            path28.Add(new Vector2(250, -50));
            path28.Add(new Vector2(250, 50));
            path28.Add(new Vector2(225, 100));
            path28.Add(new Vector2(175, 175));
            path28.Add(new Vector2(150, 200));
            path28.Add(new Vector2(150, 300));
            path28.Add(new Vector2(175, 325));
            path28.Add(new Vector2(275, 350));
            path28.Add(new Vector2(850, 350));
            pathWayPoints.Add(path28);
            waveSpawns.Add(28, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path29 = new List<Vector2>();
            path29.Add(new Vector2(550, -50));
            path29.Add(new Vector2(550, 50));
            path29.Add(new Vector2(575, 100));
            path29.Add(new Vector2(625, 175));
            path29.Add(new Vector2(650, 200));
            path29.Add(new Vector2(650, 300));
            path29.Add(new Vector2(625, 325));
            path29.Add(new Vector2(525, 350));
            path29.Add(new Vector2(-50, 350));
            pathWayPoints.Add(path29);
            waveSpawns.Add(29, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path30 = new List<Vector2>();
            path30.Add(new Vector2(500, -50));
            path30.Add(new Vector2(500, 50));
            path30.Add(new Vector2(525, 100));
            path30.Add(new Vector2(575, 175));
            path30.Add(new Vector2(600, 200));
            path30.Add(new Vector2(600, 250));
            path30.Add(new Vector2(575, 275));
            path30.Add(new Vector2(475, 300));
            path30.Add(new Vector2(325, 300));
            path30.Add(new Vector2(225, 275));
            path30.Add(new Vector2(200, 250));
            path30.Add(new Vector2(200, 200));
            path30.Add(new Vector2(225, 175));
            path30.Add(new Vector2(275, 100));
            path30.Add(new Vector2(300, 50));
            path30.Add(new Vector2(300, -50));
            pathWayPoints.Add(path30);
            waveSpawns.Add(30, new WaveInfo(0, EnemyType.Easy));

            // added in version 2.7:
            List<Vector2> path31 = new List<Vector2>();
            path31.Add(new Vector2(-50, -50));
            path31.Add(new Vector2(850, 300));
            pathWayPoints.Add(path31);
            waveSpawns.Add(31, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path32 = new List<Vector2>();
            path32.Add(new Vector2(850, -50));
            path32.Add(new Vector2(-50, 300));
            pathWayPoints.Add(path32);
            waveSpawns.Add(32, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path33 = new List<Vector2>();
            path33.Add(new Vector2(-50, 0));
            path33.Add(new Vector2(850, 350));
            pathWayPoints.Add(path33);
            waveSpawns.Add(33, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path34 = new List<Vector2>();
            path34.Add(new Vector2(850, 0));
            path34.Add(new Vector2(-50, 350));
            pathWayPoints.Add(path34);
            waveSpawns.Add(34, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path35 = new List<Vector2>();
            path35.Add(new Vector2(450, -50));
            path35.Add(new Vector2(500, 25));
            path35.Add(new Vector2(600, 75));
            path35.Add(new Vector2(650, 125));
            path35.Add(new Vector2(675, 150));
            path35.Add(new Vector2(700, 180));
            path35.Add(new Vector2(700, 200));
            path35.Add(new Vector2(675, 225));
            path35.Add(new Vector2(650, 250));
            path35.Add(new Vector2(550, 300));
            path35.Add(new Vector2(400, 350));
            path35.Add(new Vector2(-50, 350));
            pathWayPoints.Add(path35);
            waveSpawns.Add(35, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path36 = new List<Vector2>();
            path36.Add(new Vector2(450, -50));
            path36.Add(new Vector2(500, 25));
            path36.Add(new Vector2(600, 75));
            path36.Add(new Vector2(650, 125));
            path36.Add(new Vector2(675, 150));
            path36.Add(new Vector2(700, 180));
            path36.Add(new Vector2(700, 200));
            path36.Add(new Vector2(675, 225));
            path36.Add(new Vector2(650, 250));
            path36.Add(new Vector2(550, 300));
            path36.Add(new Vector2(400, 350));
            path36.Add(new Vector2(300, 325));
            path36.Add(new Vector2(200, 275));
            path36.Add(new Vector2(100, 200));
            path36.Add(new Vector2(-50, 100));
            pathWayPoints.Add(path36);
            waveSpawns.Add(36, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path37 = new List<Vector2>();
            path37.Add(new Vector2(350, -50));
            path37.Add(new Vector2(300, 25));
            path37.Add(new Vector2(200, 75));
            path37.Add(new Vector2(150, 125));
            path37.Add(new Vector2(125, 150));
            path37.Add(new Vector2(100, 180));
            path37.Add(new Vector2(100, 200));
            path37.Add(new Vector2(125, 225));
            path37.Add(new Vector2(150, 250));
            path37.Add(new Vector2(250, 300));
            path37.Add(new Vector2(400, 350));
            path37.Add(new Vector2(850, 350));
            pathWayPoints.Add(path37);
            waveSpawns.Add(37, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path38 = new List<Vector2>();
            path38.Add(new Vector2(350, -50));
            path38.Add(new Vector2(300, 25));
            path38.Add(new Vector2(200, 75));
            path38.Add(new Vector2(150, 125));
            path38.Add(new Vector2(125, 150));
            path38.Add(new Vector2(100, 180));
            path38.Add(new Vector2(100, 200));
            path38.Add(new Vector2(125, 225));
            path38.Add(new Vector2(150, 250));
            path38.Add(new Vector2(250, 300));
            path38.Add(new Vector2(400, 350));
            path38.Add(new Vector2(500, 325));
            path38.Add(new Vector2(600, 275));
            path38.Add(new Vector2(700, 200));
            path38.Add(new Vector2(850, 100));
            pathWayPoints.Add(path38);
            waveSpawns.Add(38, new WaveInfo(0, EnemyType.Easy));

            // Added in version 2.10
            List<Vector2> path39 = new List<Vector2>();
            path39.Add(new Vector2(350, -50));
            path39.Add(new Vector2(350, 250));
            path39.Add(new Vector2(325, 300));
            path39.Add(new Vector2(275, 350));
            path39.Add(new Vector2(225, 375));
            path39.Add(new Vector2(200, 375));
            path39.Add(new Vector2(150, 350));
            path39.Add(new Vector2(100, 300));
            path39.Add(new Vector2(75,  250));
            path39.Add(new Vector2(75, 225));
            path39.Add(new Vector2(100, 175));
            path39.Add(new Vector2(150, 125));
            path39.Add(new Vector2(200, 100));
            path39.Add(new Vector2(600, 100));
            path39.Add(new Vector2(650, 125));
            path39.Add(new Vector2(700, 175));
            path39.Add(new Vector2(725, 225));
            path39.Add(new Vector2(725, 250));
            path39.Add(new Vector2(700, 300));
            path39.Add(new Vector2(650, 350));
            path39.Add(new Vector2(600, 375));
            path39.Add(new Vector2(575, 375));
            path39.Add(new Vector2(525, 350));
            path39.Add(new Vector2(475, 300));
            path39.Add(new Vector2(450, 250));
            path39.Add(new Vector2(450, -50));
            pathWayPoints.Add(path39);
            waveSpawns.Add(39, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path40 = new List<Vector2>();
            path40.Add(new Vector2(500, -50));
            path40.Add(new Vector2(500, 300));
            path40.Add(new Vector2(525, 350));
            path40.Add(new Vector2(575, 400));
            path40.Add(new Vector2(625, 425));
            path40.Add(new Vector2(650, 425));
            path40.Add(new Vector2(700, 400));
            path40.Add(new Vector2(750, 350));
            path40.Add(new Vector2(775, 300));
            path40.Add(new Vector2(775, 275));
            path40.Add(new Vector2(750, 225));
            path40.Add(new Vector2(700, 175));
            path40.Add(new Vector2(650, 150));
            path40.Add(new Vector2(150, 150));
            path40.Add(new Vector2(100, 175));
            path40.Add(new Vector2(50, 225));
            path40.Add(new Vector2(25, 275));
            path40.Add(new Vector2(25, 300));
            path40.Add(new Vector2(50, 350));
            path40.Add(new Vector2(100, 400));
            path40.Add(new Vector2(150, 425));
            path40.Add(new Vector2(175, 425));
            path40.Add(new Vector2(225, 400));
            path40.Add(new Vector2(275, 350));
            path40.Add(new Vector2(300, 300));
            path40.Add(new Vector2(300, -50));
            pathWayPoints.Add(path40);
            waveSpawns.Add(40, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path41 = new List<Vector2>();
            path41.Add(new Vector2(850, 175));
            path41.Add(new Vector2(150, 175));
            path41.Add(new Vector2(100, 200));
            path41.Add(new Vector2(50, 250));
            path41.Add(new Vector2(25, 300));
            path41.Add(new Vector2(25, 325));
            path41.Add(new Vector2(50, 375));
            path41.Add(new Vector2(100, 425));
            path41.Add(new Vector2(150, 450));
            path41.Add(new Vector2(475, 450));
            path41.Add(new Vector2(525, 425));
            path41.Add(new Vector2(575, 375));
            path41.Add(new Vector2(600, 325));
            path41.Add(new Vector2(600, -50));
            pathWayPoints.Add(path41);
            waveSpawns.Add(41, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path42 = new List<Vector2>();
            path42.Add(new Vector2(850, 125));
            path42.Add(new Vector2(150, 125));
            path42.Add(new Vector2(100, 150));
            path42.Add(new Vector2(50, 200));
            path42.Add(new Vector2(25, 250));
            path42.Add(new Vector2(25, 275));
            path42.Add(new Vector2(50, 325));
            path42.Add(new Vector2(100, 375));
            path42.Add(new Vector2(150, 400));
            path42.Add(new Vector2(600, 400));
            path42.Add(new Vector2(650, 375));
            path42.Add(new Vector2(700, 325));
            path42.Add(new Vector2(725, 275));
            path42.Add(new Vector2(725, -50));
            pathWayPoints.Add(path42);
            waveSpawns.Add(42, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path43 = new List<Vector2>();
            path43.Add(new Vector2(-50, 175));
            path43.Add(new Vector2(650, 175));
            path43.Add(new Vector2(700, 200));
            path43.Add(new Vector2(750, 250));
            path43.Add(new Vector2(775, 300));
            path43.Add(new Vector2(775, 325));
            path43.Add(new Vector2(750, 375));
            path43.Add(new Vector2(700, 425));
            path43.Add(new Vector2(650, 450));
            path43.Add(new Vector2(325, 450));
            path43.Add(new Vector2(275, 425));
            path43.Add(new Vector2(225, 375));
            path43.Add(new Vector2(200, 325));
            path43.Add(new Vector2(200, -50));
            pathWayPoints.Add(path43);
            waveSpawns.Add(43, new WaveInfo(0, EnemyType.Easy));

            List<Vector2> path44 = new List<Vector2>();
            path44.Add(new Vector2(-50, 125));
            path44.Add(new Vector2(650, 125));
            path44.Add(new Vector2(700, 150));
            path44.Add(new Vector2(750, 200));
            path44.Add(new Vector2(775, 250));
            path44.Add(new Vector2(775, 275));
            path44.Add(new Vector2(750, 325));
            path44.Add(new Vector2(700, 375));
            path44.Add(new Vector2(650, 400));
            path44.Add(new Vector2(200, 400));
            path44.Add(new Vector2(150, 375));
            path44.Add(new Vector2(100, 325));
            path44.Add(new Vector2(75, 275));
            path44.Add(new Vector2(75, -50));
            pathWayPoints.Add(path44);
            waveSpawns.Add(44, new WaveInfo(0, EnemyType.Easy));
        }

        public void SpawnEnemy(int path, EnemyType type)
        {
            Enemy newEnemy;

            switch (type)
            {
                case EnemyType.Medium:
                    newEnemy = Enemy.CreateMediumEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;

                case EnemyType.Hard:
                    newEnemy = Enemy.CreateHardEnemy(texture,
                                                     pathWayPoints[path][0]);
                    break;

                case EnemyType.Speeder:
                    newEnemy = Enemy.CreateSpeederEnemy(texture,
                                                        pathWayPoints[path][0]);
                    break;

                case EnemyType.Tank:
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

            EnemyType type;
            int rnd = rand.Next(0, 12);

            switch (rnd)
            {
                case 0:
                case 1:
                case 2:
                    type = EnemyType.Medium;
                    break;

                case 3:
                case 4:
                    type = EnemyType.Hard;
                    break;

                case 5:
                case 6:
                    type = EnemyType.Speeder;
                    break;

                case 7:
                    type = EnemyType.Tank;
                    break;

                default:
                    type = EnemyType.Easy;
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

                int rndSecondSpawn = rand.Next(0, 100 / (currentLevel));
                // Lvl conforms to probability!

                SpawnWave(rnd1);

                if (rndSecondSpawn == 0 && rnd1 != rnd2)
                    SpawnWave(rnd2);

                nextWaveTimer = 0.0f;
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                    float rndShot = (float)rand.Next(0, ((int)(2000 * sentimentFactor))) / 10;

                    if (rndShot <= Enemies[x].ShotChance &&
                        !playerManager.IsDestroyed &&
                         screen.Contains((int)Enemies[x].EnemySprite.Center.X,
                                         (int)Enemies[x].EnemySprite.Center.Y))
                    {
                        Vector2 fireLocation = Enemies[x].EnemySprite.Location;
                        fireLocation += Enemies[x].GunOffset;

                        //Vector2 shotDirection = (playerManager.playerSprite.Center - fireLocation);
                        Vector2 shotDirection = ((playerManager.playerSprite.Center + playerManager.playerSprite.Velocity / (2.0f + (float)rand.NextDouble() * 8.0f)) - fireLocation);

                        shotDirection.Normalize();

                        if (Enemies[x].Type == EnemyType.Tank &&
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

            // Angry/friendly
            if (IsAngryOrFriendly)
            {
                sentimentTimer -= elapsed;

                if (sentimentTimer <= 0.0f)
                {
                    sentimentFactor = SENTIMENT_NORMAL_FACTOR;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyShotManager.Draw(spriteBatch);

            foreach (var enemy in Enemies)
            {
                Color tint = enemy.EnemySprite.TintColor;

                if (IsAngry)
                {
                    enemy.EnemySprite.TintColor = new Color(255, 128, 128, tint.A);
                }
                else if (IsFriendly)
                {
                    enemy.EnemySprite.TintColor = new Color(128, 255, 128, tint.A);
                }

                enemy.Draw(spriteBatch);

                enemy.EnemySprite.TintColor = tint;
            }
        }

        public void Reset()
        {
            this.Enemies.Clear();
            this.EnemyShotManager.Shots.Clear();

            this.nextWaveTimer = 0.0f;
            this.shipSpawnTimer = 0.0f;

            this.nextWaveMinTimer = InitialNextWaveMinTimer;

            for (int i = 0; i < waveSpawns.Count; i++)
            {
                waveSpawns[i] = new WaveInfo(0, EnemyType.Easy);
            }

            this.IsActive = true;

            this.sentimentFactor = SENTIMENT_NORMAL_FACTOR;
            this.sentimentTimer = 0.0f;
        }

        public void SetLevel(int lvl)
        {
            this.currentLevel = lvl;

            // Version 1.0
            //float tmp = initialNextWaveMinTimer - (lvl - 1) * 0.075f;

            // Version 1.1
            //float tmp = (int)(InitialNextWaveMinTimer - (float)Math.Sqrt(lvl - 1) * 0.075f + 0.015 * (lvl - 1)); // 5 - WURZEL(A2-1) / 2 * 0,15 - 0,015 * (A2 - 1)

            // Version 2.0
            float tmp = (int)(InitialNextWaveMinTimer - (float)Math.Sqrt(lvl - 1) * 0.075f + 0.02 * (lvl - 1)); // 5 - WURZEL(A2-1) / 2 * 0,15 - 0,02 * (A2 - 1)

            this.nextWaveMinTimer = Math.Max(tmp, 1.0f);
        }

        public void StartFriendly()
        {
            this.sentimentTimer = SENTIMENT_DURATION;
            this.sentimentFactor = FRIENDLY_FACTOR;
        }

        public void StartAngry()
        {
            this.sentimentTimer = SENTIMENT_DURATION;
            this.sentimentFactor = ANGRY_FACTOR;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Enemies
            int enemiesCount = Int32.Parse(reader.ReadLine());

            for (int i = 0; i < enemiesCount; ++i)
            {
                EnemyType type = EnemyType.Easy;
                Enemy e;

                type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

                switch (type)
                {
                    case EnemyType.Easy:
                        e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Medium:
                        e = Enemy.CreateMediumEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Hard:
                        e = Enemy.CreateHardEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Speeder:
                        e = Enemy.CreateSpeederEnemy(texture, Vector2.Zero);
                        break;
                    case EnemyType.Tank:
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

            // Angry/Friendly
            this.sentimentTimer = Single.Parse(reader.ReadLine());
            this.sentimentFactor = Single.Parse(reader.ReadLine());
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

            // Angry/friendly
            writer.WriteLine(this.sentimentTimer);
            writer.WriteLine(this.sentimentFactor);
        }

        #endregion

        #region Properties

        public bool IsAngryOrFriendly
        {
            get
            {
                return sentimentFactor != SENTIMENT_NORMAL_FACTOR;
            }
        }

        public bool IsFriendly
        {
            get
            {
                return sentimentFactor == FRIENDLY_FACTOR;
            }
        }

        public bool IsAngry
        {
            get
            {
                return sentimentFactor == ANGRY_FACTOR;
            }
        }

        #endregion
    }
}
