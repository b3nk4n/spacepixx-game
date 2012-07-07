using System;
using Microsoft.Xna.Framework;

namespace SpacepiXX
{
    class CollisionManager
    {
        #region Members

        private AsteroidManager asteroidManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private BossManager bossManager;
        private PowerUpManager powerUpManager;
        private Vector2 offScreen = new Vector2(-500, -500);
        private Vector2 shotToAsteroidImpact = new Vector2(0, -20);

        private const string INFO_HEALTH25 = "+25% Repair-Kit";
        private const string INFO_HEALTH50 = "+50% Repair-Kit";
        private const string INFO_HEALTH100 = "+100% Repair-Kit";
        private const string INFO_COOLINGWATER = "Cooling Water";
        private const string INFO_EXTRASHIP = "Extra Spaceship!";
        private const string INFO_SUPERLASER = "+1 Super-Laser";
        private const string INFO_ROCKETS = "+2 C.A.R.L.I-Rockets";
        private const string INFO_NUKE = "!!! N U K E !!!";
        private const string INFO_BONUSSCORE_LOW = "+1000";
        private const string INFO_BONUSSCORE_MEDIUM = "+2500";
        private const string INFO_BONUSSCORE_HIGH = "+5000";
        private const string INFO_SCOREMULTI_LOW = "+50% Score-Multiplier";
        private const string INFO_SCOREMULTI_MEDIUM = "+75% Score-Multiplier";
        private const string INFO_SCOREMULTI_HIGH = "+100% Score-Multiplier";
        private const string INFO_SCOREMULTI_LOST = "Score-Multiplier Lost!";
        private const string INFO_OUT_OF_CONTROL = "We get out of control!";
        private const string INFO_SLOW = "Hyper Eninge damaged!";
        private const string INFO_OVERHEAT = "Overheating Problem!";
        private const string INFO_SHIELDS = "Activated Shields";
        private const string INFO_OVERDRIVE = "Overdrive!";
        private const string INFO_UNDERDRIVE = "Underdrive!";
        private const string INFO_FRIENDLY = "Almost friendly!";
        private const string INFO_ANGRY = "Rage!";

        Random rand = new Random();

        #endregion

        #region Constructors

        public CollisionManager(AsteroidManager asteroidManager, PlayerManager playerManager,
                                EnemyManager enemyManager, BossManager bossManager,
                                PowerUpManager powerUpManager)
        {
            this.asteroidManager = asteroidManager;
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
            this.bossManager = bossManager;
            this.powerUpManager = powerUpManager;
        }

        #endregion

        #region Methods

        private void checkShotToEnemyCollisions()
        {
            foreach (var enemy in enemyManager.Enemies)
            {
                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                foreach (var shot in playerManager.PlayerShotManager.Shots)
                {
                    if (shot.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.HitPoints -= playerManager.ShotPower;

                        location = shot.Location;
                        velocity = shot.Velocity;

                        shot.Location = offScreen;
                        
                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }

                if (location != Vector2.Zero)
                {
                    if (enemy.IsDestroyed)
                    {
                        playerManager.IncreasePlayerScore(enemy.KillScore, true);

                        EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                        enemy.EnemySprite.Velocity / 10);

                        powerUpManager.ProbablySpawnPowerUp(enemy.EnemySprite.Center);

                        playerManager.IncreaseScoreMulti(enemy.InitialKillScore, true);
                    }
                    else
                    {
                        playerManager.IncreasePlayerScore(enemy.HitScore, true);

                        EffectManager.AddLargeSparksEffect(location,
                                                      velocity,
                                                      -velocity,
                                                      Color.Orange);

                        playerManager.IncreaseScoreMulti(enemy.InitialHitScore, false);
                    }
                }
            }
        }

        private void checkShotToBossCollisions()
        {
            foreach (var boss in bossManager.Bosses)
            {
                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                foreach (var shot in playerManager.PlayerShotManager.Shots)
                {
                    if (shot.IsCircleColliding(boss.BossSprite.Center,
                                               boss.BossSprite.CollisionRadius) &&
                        !boss.IsDestroyed)
                    {
                        boss.HitPoints -= playerManager.ShotPower;

                        location = shot.Location;
                        velocity = shot.Velocity;

                        shot.Location = offScreen;
                        
                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }    
                }

                if (location != Vector2.Zero)
                {
                    if (boss.IsDestroyed)
                    {
                        playerManager.IncreasePlayerScore(boss.KillScore, true);

                        EffectManager.AddBossExplosion(boss.BossSprite.Center,
                                                        boss.BossSprite.Velocity / 10);

                        powerUpManager.ProbablySpawnPowerUp(boss.BossSprite.Center);

                        playerManager.IncreaseScoreMulti(boss.InitialKillScore / 5, true);
                    }
                    else
                    {
                        playerManager.IncreasePlayerScore(boss.HitScore, true);

                        EffectManager.AddLargeSparksEffect(location,
                                                           velocity,
                                                           -velocity,
                                                           Color.Orange);

                        playerManager.IncreaseScoreMulti(boss.InitialHitScore, false);
                    }
                }
            }
        }

        private void checkRocketToEnemyCollisions()
        {
            foreach (var rocket in playerManager.PlayerShotManager.Rockets)
            {
                foreach (var enemy in enemyManager.Enemies)
                {
                    if (rocket.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.HitPoints = 0;

                        playerManager.IncreasePlayerScore(enemy.KillScore, true);

                        EffectManager.AddRocketExplosion(enemy.EnemySprite.Center,
                                                         enemy.EnemySprite.Velocity / 10);

                        powerUpManager.ProbablySpawnPowerUp(enemy.EnemySprite.Center);

                        playerManager.IncreaseScoreMulti(enemy.InitialKillScore, true);

                        foreach (var otherEnemy in enemyManager.Enemies)
                        {
                            if (enemy != otherEnemy)
                            {
                                float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                                if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                    !otherEnemy.IsDestroyed)
                                {
                                    float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                    float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                    otherEnemy.HitPoints -= damage;

                                    if (otherEnemy.IsDestroyed)
                                    {
                                        playerManager.IncreasePlayerScore(otherEnemy.KillScore, true);

                                        EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                        otherEnemy.EnemySprite.Velocity / 10);

                                        powerUpManager.ProbablySpawnPowerUp(otherEnemy.EnemySprite.Center);

                                        playerManager.IncreaseScoreMulti(otherEnemy.InitialKillScore, true);
                                    }
                                    else
                                    {
                                        playerManager.IncreasePlayerScore(otherEnemy.HitScore, true);

                                        playerManager.IncreaseScoreMulti(otherEnemy.InitialHitScore, true);
                                    }
                                }
                            }
                        }

                        rocket.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkRocketToBossCollisions()
        {
            foreach (var rocket in playerManager.PlayerShotManager.Rockets)
            {
                foreach (var boss in bossManager.Bosses)
                {
                    if (rocket.IsCircleColliding(boss.BossSprite.Center,
                                               boss.BossSprite.CollisionRadius) &&
                        !boss.IsDestroyed)
                    {
                        boss.HitPoints -= PlayerManager.ROCKET_POWER_AT_CENTER;

                        if (boss.IsDestroyed)
                        {
                            playerManager.IncreasePlayerScore(boss.KillScore, true);

                            powerUpManager.ProbablySpawnPowerUp(boss.BossSprite.Center);

                            playerManager.IncreaseScoreMulti(boss.InitialKillScore / 5, true);

                            EffectManager.AddBossExplosion(boss.BossSprite.Center,
                                                           boss.BossSprite.Velocity / 10);
                        }
                        else
                        {
                            playerManager.IncreasePlayerScore(boss.HitScore * 6, true);

                            playerManager.IncreaseScoreMulti(boss.InitialHitScore, true);

                            EffectManager.AddRocketExplosion(rocket.Center,
                                                             boss.BossSprite.Velocity / 10);
                        }

                        foreach (var otherBoss in bossManager.Bosses)
                        {
                            if (boss != otherBoss)
                            {
                                float distance = Math.Abs((rocket.Center - otherBoss.BossSprite.Center).Length());

                                if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                    !otherBoss.IsDestroyed)
                                {
                                    float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                    float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                    otherBoss.HitPoints -= damage;

                                    if (otherBoss.IsDestroyed)
                                    {
                                        playerManager.IncreasePlayerScore(otherBoss.KillScore, true);

                                        EffectManager.AddBossExplosion(otherBoss.BossSprite.Center,
                                                                       otherBoss.BossSprite.Velocity / 10);

                                        powerUpManager.ProbablySpawnPowerUp(otherBoss.BossSprite.Center);

                                        playerManager.IncreaseScoreMulti(otherBoss.InitialKillScore, true);
                                    }
                                    else
                                    {
                                        playerManager.IncreasePlayerScore(otherBoss.HitScore, true);

                                        playerManager.IncreaseScoreMulti(otherBoss.InitialHitScore, true);
                                    }
                                }
                            }
                        }

                        rocket.Location = offScreen;
                    }
                }
            }
        }

        private void checkRocketToAsteroidCollisions()
        {
            foreach (var rocket in playerManager.PlayerShotManager.Rockets)
            {
                foreach (var asteroid in asteroidManager.Asteroids)
                {
                    if (rocket.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddRocketExplosion(asteroid.Center,
                                                         asteroid.Velocity / 10);

                        foreach (var otherEnemy in enemyManager.Enemies)
                        {
                            float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                            if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                !otherEnemy.IsDestroyed)
                            {
                                float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                otherEnemy.HitPoints -= damage;

                                if (otherEnemy.IsDestroyed)
                                {
                                    playerManager.IncreasePlayerScore(otherEnemy.KillScore, true);

                                    EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                    otherEnemy.EnemySprite.Velocity / 10);

                                    powerUpManager.ProbablySpawnPowerUp(otherEnemy.EnemySprite.Center);

                                    playerManager.IncreaseScoreMulti(otherEnemy.InitialKillScore, true);
                                }
                                else
                                {
                                    playerManager.IncreasePlayerScore(otherEnemy.HitScore, true);

                                    playerManager.IncreaseScoreMulti(otherEnemy.InitialHitScore, true);
                                }
                            }
                            
                        }

                        foreach (var otherBoss in bossManager.Bosses)
                        {
                            float distance = Math.Abs((rocket.Center - otherBoss.BossSprite.Center).Length());

                            if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                                !otherBoss.IsDestroyed)
                            {
                                float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                                otherBoss.HitPoints -= damage;

                                if (otherBoss.IsDestroyed)
                                {
                                    playerManager.IncreasePlayerScore(otherBoss.KillScore, true);

                                    EffectManager.AddLargeExplosion(otherBoss.BossSprite.Center,
                                                                    otherBoss.BossSprite.Velocity / 10);

                                    powerUpManager.ProbablySpawnPowerUp(otherBoss.BossSprite.Center);

                                    playerManager.IncreaseScoreMulti(otherBoss.InitialKillScore, true);
                                }
                                else
                                {
                                    playerManager.IncreasePlayerScore(otherBoss.HitScore, true);

                                    playerManager.IncreaseScoreMulti(otherBoss.InitialHitScore, true);
                                }
                            }

                        }

                        rocket.Location = offScreen;
                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkEnemyRocketToPlayerCollisions()
        {
            foreach (var rocket in enemyManager.EnemyShotManager.Rockets)
            {
                if (rocket.IsCircleColliding(playerManager.playerSprite.Center,
                                             playerManager.playerSprite.CollisionRadius) &&
                    !playerManager.IsDestroyed)
                {
                    playerManager.DecreaseHitPoints(rand.Next(EnemyManager.DAMAGE_LASER_MIN,
                                                              EnemyManager.DAMAGE_LASER_MAX + 1));

                    EffectManager.AddRocketExplosion(playerManager.playerSprite.Center,
                                                     playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.3f);

                    foreach (var otherEnemy in enemyManager.Enemies)
                    {
                        float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                        if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                            !otherEnemy.IsDestroyed)
                        {
                            float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                            float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                            otherEnemy.HitPoints -= damage;

                            if (otherEnemy.IsDestroyed)
                            {
                                EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                otherEnemy.EnemySprite.Velocity / 10);
                            }
                        }
                    }

                    rocket.Location = offScreen;
                }
            }
        }

        private void checkBossRocketToPlayerCollisions()
        {
            foreach (var rocket in bossManager.BossShotManager.Rockets)
            {
                if (rocket.IsCircleColliding(playerManager.playerSprite.Center,
                                             playerManager.playerSprite.CollisionRadius) &&
                    !playerManager.IsDestroyed)
                {
                    playerManager.DecreaseHitPoints(rand.Next(BossManager.DAMAGE_LASER_MIN,
                                                              BossManager.DAMAGE_LASER_MAX + 1));

                    EffectManager.AddRocketExplosion(playerManager.playerSprite.Center,
                                                     playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.3f);

                    foreach (var otherBoss in bossManager.Bosses)
                    {
                        float distance = Math.Abs((rocket.Center - otherBoss.BossSprite.Center).Length());

                        if (distance < PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS &&
                            !otherBoss.IsDestroyed)
                        {
                            float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance);

                            float damage = PlayerManager.ROCKET_POWER_AT_CENTER * (distAmount / PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS);

                            otherBoss.HitPoints -= damage;

                            if (otherBoss.IsDestroyed)
                            {
                                EffectManager.AddLargeExplosion(otherBoss.BossSprite.Center,
                                                                otherBoss.BossSprite.Velocity / 10);
                            }
                        }
                    }

                    rocket.Location = offScreen;
                }
            }
        }

        private void checkEnemyRocketToAsteroidCollisions()
        {
            foreach (var rocket in enemyManager.EnemyShotManager.Rockets)
            {
                foreach (var asteroid in asteroidManager.Asteroids)
                {
                    if (rocket.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddRocketExplosion(asteroid.Center,
                                                         asteroid.Velocity / 10);


                        foreach (var otherEnemy in enemyManager.Enemies)
                        {
                            float distance = Math.Abs((rocket.Center - otherEnemy.EnemySprite.Center).Length());

                            if (distance < EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS &&
                                !otherEnemy.IsDestroyed)
                            {
                                float distAmount = Math.Max(0, EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = EnemyManager.ROCKET_POWER_AT_CENTER * (distAmount / EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS);

                                otherEnemy.HitPoints -= damage;

                                if (otherEnemy.IsDestroyed)
                                {
                                    EffectManager.AddLargeExplosion(otherEnemy.EnemySprite.Center,
                                                                    otherEnemy.EnemySprite.Velocity / 10);
                                }
                            }
                        }

                        float distance2 = Math.Abs((rocket.Center - playerManager.playerSprite.Center).Length());

                        if (distance2 < EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS &&
                            !playerManager.IsDestroyed)
                        {
                            float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance2);

                            float damage = EnemyManager.ROCKET_POWER_AT_CENTER * (distAmount / EnemyManager.SOFT_ROCKET_EXPLOSION_RADIUS);

                            playerManager.DecreaseHitPoints(damage);

                            VibrationManager.Vibrate(0.2f);

                            if (playerManager.IsDestroyed)
                            {
                                EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                                playerManager.playerSprite.Velocity / 10);
                            }
                        }

                        rocket.Location = offScreen;
                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkBossRocketToAsteroidCollisions()
        {
            foreach (var rocket in bossManager.BossShotManager.Rockets)
            {
                foreach (var asteroid in asteroidManager.Asteroids)
                {
                    if (rocket.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddRocketExplosion(asteroid.Center,
                                                         asteroid.Velocity / 10);


                        foreach (var otherBoss in bossManager.Bosses)
                        {
                            float distance = Math.Abs((rocket.Center - otherBoss.BossSprite.Center).Length());

                            if (distance < BossManager.SOFT_ROCKET_EXPLOSION_RADIUS &&
                                !otherBoss.IsDestroyed)
                            {
                                float distAmount = Math.Max(0, BossManager.SOFT_ROCKET_EXPLOSION_RADIUS - distance);

                                float damage = BossManager.ROCKET_POWER_AT_CENTER * (distAmount / BossManager.SOFT_ROCKET_EXPLOSION_RADIUS);

                                otherBoss.HitPoints -= damage;

                                if (otherBoss.IsDestroyed)
                                {
                                    EffectManager.AddLargeExplosion(otherBoss.BossSprite.Center,
                                                                    otherBoss.BossSprite.Velocity / 10);
                                }
                            }
                        }

                        float distance2 = Math.Abs((rocket.Center - playerManager.playerSprite.Center).Length());

                        if (distance2 < BossManager.SOFT_ROCKET_EXPLOSION_RADIUS &&
                            !playerManager.IsDestroyed)
                        {
                            float distAmount = Math.Max(0, PlayerManager.CARLI_ROCKET_EXPLOSION_RADIUS - distance2);

                            float damage = BossManager.ROCKET_POWER_AT_CENTER * (distAmount / BossManager.SOFT_ROCKET_EXPLOSION_RADIUS);

                            playerManager.DecreaseHitPoints(damage);

                            VibrationManager.Vibrate(0.2f);

                            if (playerManager.IsDestroyed)
                            {
                                EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                                playerManager.playerSprite.Velocity / 10);
                            }
                        }

                        rocket.Location = offScreen;
                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkPlayerShotToAsteroidCollisions()
        {
            foreach (var shot in playerManager.PlayerShotManager.Shots)
            {
                foreach (var asteroid in asteroidManager.Asteroids)
                {
                    if (shot.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddSparksEffect(shot.Location,
                                                      shot.Velocity,
                                                      asteroid.Velocity,
                                                      Color.Gray,
                                                      true);
                        shot.Location = offScreen;
                        Vector2 direction = shot.Velocity;
                        direction.Normalize();
                        direction *= 20;
                        asteroid.Velocity += direction;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkEnemyShotToAsteroidCollisions()
        {
            foreach (var shot in enemyManager.EnemyShotManager.Shots)
            {
                foreach (var asteroid in asteroidManager.Asteroids)
                {
                    if (shot.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddSparksEffect(shot.Location,
                                                      shot.Velocity,
                                                      asteroid.Velocity,
                                                      Color.Gray,
                                                      true);
                        shot.Location = offScreen;
                        Vector2 direction = shot.Velocity;
                        direction.Normalize();
                        direction *= 20;
                        asteroid.Velocity += direction;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkBossShotToAsteroidCollisions()
        {
            foreach (var shot in bossManager.BossShotManager.Shots)
            {
                foreach (var asteroid in asteroidManager.Asteroids)
                {
                    if (shot.IsCircleColliding(asteroid.Center,
                                               asteroid.CollisionRadius))
                    {
                        EffectManager.AddSparksEffect(shot.Location,
                                                      shot.Velocity,
                                                      asteroid.Velocity,
                                                      Color.Gray,
                                                      true);
                        shot.Location = offScreen;
                        Vector2 direction = shot.Velocity;
                        direction.Normalize();
                        direction *= 20;
                        asteroid.Velocity += direction;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkEnemyShotToPlayerCollisions()
        {
            foreach (var shot in enemyManager.EnemyShotManager.Shots)
            {
                if (shot.IsCircleColliding(playerManager.playerSprite.Center,
                                           playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.DecreaseHitPoints(rand.Next(EnemyManager.DAMAGE_LASER_MIN,
                                                              EnemyManager.DAMAGE_LASER_MAX + 1));

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.5f);
                    }
                    else
                    {
                        EffectManager.AddExplosion(playerManager.playerSprite.Center,
                                                   playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.2f);
                    }
                    
                    shot.Location = offScreen;
                }
            }
        }

        private void checkBossShotToPlayerCollisions()
        {
            foreach (var shot in bossManager.BossShotManager.Shots)
            {
                if (shot.IsCircleColliding(playerManager.playerSprite.Center,
                                           playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.DecreaseHitPoints(rand.Next(BossManager.DAMAGE_LASER_MIN,
                                                              BossManager.DAMAGE_LASER_MAX + 1));

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.5f);
                    }
                    else
                    {
                        EffectManager.AddExplosion(playerManager.playerSprite.Center,
                                                   playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.2f);
                    }

                    shot.Location = offScreen;
                }
            }
        }

        private void checkEnemyToPlayerCollisions()
        {
            foreach (var enemy in enemyManager.Enemies)
            {
                if (enemy.EnemySprite.IsCircleColliding(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.CollisionRadius))
                {
                    enemy.HitPoints -= Math.Max(enemy.HitPoints, 99.0f);

                    EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                    enemy.EnemySprite.Velocity / 10);

                    playerManager.DecreaseHitPoints(100f);
                    EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                    playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.5f);
                }
            }
        }

        private void checkBossToPlayerCollisions()
        {
            foreach (var boss in bossManager.Bosses)
            {
                if (boss.BossSprite.IsCircleColliding(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.DecreaseHitPoints(100f);
                    EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                    playerManager.playerSprite.Velocity / 10);

                    VibrationManager.Vibrate(0.5f);
                }
            }
        }

        private void checkAsteroidToPlayerCollisions()
        {
            foreach (var asteroid in asteroidManager.Asteroids)
            {
                if (asteroid.IsCircleColliding(playerManager.playerSprite.Center,
                                               playerManager.playerSprite.CollisionRadius))
                {
                    EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10);

                    playerManager.DecreaseHitPoints(rand.Next(AsteroidManager.CRASH_POWER_MIN,
                                                              AsteroidManager.CRASH_POWER_MAX + 1));

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.5f);
                    }
                    else
                    {
                        EffectManager.AddExplosion(playerManager.playerSprite.Center,
                                                   playerManager.playerSprite.Velocity / 10);

                        VibrationManager.Vibrate(0.2f);
                    }
                    
                    asteroid.Location = offScreen;
                }
            }
        }

        private void checkAsteroidToEnemiesCollisions()
        {
            foreach (var asteroid in asteroidManager.Asteroids)
            {
                foreach (var enemy in enemyManager.Enemies)
                {
                    if (asteroid.IsCircleColliding(enemy.EnemySprite.Center,
                                                   enemy.EnemySprite.CollisionRadius))
                    {
                        EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10);

                        enemy.HitPoints -= rand.Next(AsteroidManager.CRASH_POWER_MIN,
                                                     AsteroidManager.CRASH_POWER_MAX + 1);

                        EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                        enemy.EnemySprite.Velocity / 10);
                        
                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkAsteroidToBossesCollisions()
        {
            foreach (var asteroid in asteroidManager.Asteroids)
            {
                foreach (var boss in bossManager.Bosses)
                {
                    if (asteroid.IsCircleColliding(boss.BossSprite.Center,
                                                   boss.BossSprite.CollisionRadius))
                    {
                        EffectManager.AddAsteroidExplosion(asteroid.Center,
                                                   asteroid.Velocity / 10);

                        boss.HitPoints -= (rand.Next(AsteroidManager.CRASH_POWER_MIN,
                                                     AsteroidManager.CRASH_POWER_MAX + 1) / 5.0f);
                        if (boss.IsDestroyed)
                        {
                            EffectManager.AddBossExplosion(boss.BossSprite.Center,
                                                           boss.BossSprite.Velocity / 10);
                        }

                        asteroid.Location = offScreen;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkPowerUpToPlayerCollision()
        {
            foreach (var powerUp in powerUpManager.PowerUps)
            {
                if (powerUp.isCircleColliding(playerManager.playerSprite.Center, playerManager.playerSprite.CollisionRadius))
                {
                    if (powerUp.Type == PowerUp.PowerUpType.Random)
                        powerUp.Type = powerUpManager.GetPowerUpNotRandom();

                    // Activate power-up
                    switch (powerUp.Type)
                    {
                        case PowerUp.PowerUpType.Health25:
                            playerManager.IncreaseHitPoints(25.0f);
                            SoundManager.PlayRepairSound();
                            ZoomTextManager.ShowInfo(INFO_HEALTH25);
                            break;

                        case PowerUp.PowerUpType.Health50:
                            playerManager.IncreaseHitPoints(50.0f);
                            ZoomTextManager.ShowInfo(INFO_HEALTH50);
                            SoundManager.PlayRepairSound();
                            break;

                        case PowerUp.PowerUpType.Health100:
                            playerManager.IncreaseHitPoints(100.0f);
                            ZoomTextManager.ShowInfo(INFO_HEALTH100);
                            SoundManager.PlayRepairSound();
                            break;

                        case PowerUp.PowerUpType.CoolWater:
                            playerManager.Overheat = 0.0f;
                            SoundManager.PlayCoolWaterSound();
                            ZoomTextManager.ShowInfo(INFO_COOLINGWATER);
                            break;

                        case PowerUp.PowerUpType.Life:
                            playerManager.LivesRemaining++;
                            SoundManager.PlayExtraShipSound();
                            ZoomTextManager.ShowInfo(INFO_EXTRASHIP);
                            break;

                        case PowerUp.PowerUpType.SpecialShot:
                            playerManager.SpecialShotsRemaining++;
                            SoundManager.PlayExtraSpecialShotSound();
                            ZoomTextManager.ShowInfo(INFO_SUPERLASER);
                            break;

                        case PowerUp.PowerUpType.BonusRockets:
                            playerManager.CarliRocketsRemaining += 3;
                            SoundManager.PlayBonusRocketSound();
                            ZoomTextManager.ShowInfo(INFO_ROCKETS);
                            break;

                        case PowerUp.PowerUpType.KillAll:
                            foreach (var enemy in enemyManager.Enemies)
                            {
                                enemy.HitPoints = 0.0f;

                                if (enemy.IsDestroyed)
                                {
                                    playerManager.IncreasePlayerScore(enemy.KillScore, false); // Low score
                                    playerManager.IncreaseScoreMulti(enemy.HitScore, false); // Low score Multi

                                    EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                                    enemy.EnemySprite.Velocity / 10);

                                    // No power-up here...
                                }
                            }

                            foreach (var boss in bossManager.Bosses)
                            {
                                boss.HitPoints /= 2;

                                if (boss.IsDestroyed)
                                {
                                    playerManager.IncreasePlayerScore(boss.KillScore, false); // Low score
                                    playerManager.IncreaseScoreMulti(boss.HitScore, false); // Low score Multi

                                    EffectManager.AddLargeExplosion(boss.BossSprite.Center,
                                                                    boss.BossSprite.Velocity / 10);

                                    // No power-up here...
                                }
                            }

                            VibrationManager.Vibrate(1.0f);
                            SoundManager.PlayNukeExplosionSound();
                            ZoomTextManager.ShowInfo(INFO_NUKE);
                            break;

                        case PowerUp.PowerUpType.LowBonusScore:
                            playerManager.IncreasePlayerScore(1000, false);
                            SoundManager.PlayCoinSound();
                            ZoomTextManager.ShowInfo(INFO_BONUSSCORE_LOW);
                            break;

                        case PowerUp.PowerUpType.MediumBonusScore:
                            playerManager.IncreasePlayerScore(2500, false);
                            SoundManager.PlayCoinSound();
                            ZoomTextManager.ShowInfo(INFO_BONUSSCORE_MEDIUM);
                            break;

                        case PowerUp.PowerUpType.HighBonusScore:
                            playerManager.IncreasePlayerScore(5000, false);
                            SoundManager.PlayCoinSound();
                            ZoomTextManager.ShowInfo(INFO_BONUSSCORE_HIGH);
                            break;

                        case PowerUp.PowerUpType.ScoreMultiLow:
                            playerManager.ScoreMulti += 0.5f;
                            SoundManager.PlayScoreMultiSound();
                            ZoomTextManager.ShowInfo(INFO_SCOREMULTI_LOW);
                            break;

                        case PowerUp.PowerUpType.ScoreMultiMedium:
                            playerManager.ScoreMulti += 0.75f;
                            SoundManager.PlayScoreMultiSound();
                            ZoomTextManager.ShowInfo(INFO_SCOREMULTI_MEDIUM);
                            break;

                        case PowerUp.PowerUpType.ScoreMultiHigh:
                            playerManager.ScoreMulti += 1.0f;
                            SoundManager.PlayScoreMultiSound();
                            ZoomTextManager.ShowInfo(INFO_SCOREMULTI_HIGH);
                            break;

                        case PowerUp.PowerUpType.AntiScoreMulti:
                            playerManager.ScoreMulti = PlayerManager.MIN_SCORE_MULTI;
                            SoundManager.PlayAntiScoreMultiSound();
                            ZoomTextManager.ShowInfo(INFO_SCOREMULTI_LOST);
                            break;

                        case PowerUp.PowerUpType.OverHeat:
                            playerManager.Overheat = PlayerManager.OVERHEAT_MAX;
                            SoundManager.PlayOverheatSound();
                            ZoomTextManager.ShowInfo(INFO_OVERHEAT);
                            break;

                        case PowerUp.PowerUpType.OutOfControl:
                            playerManager.StartOutOfControl();
                            SoundManager.PlayOutOfControlSound();
                            ZoomTextManager.ShowInfo(INFO_OUT_OF_CONTROL);
                            break;

                        case PowerUp.PowerUpType.Slow:
                            playerManager.StartSlow();
                            SoundManager.PlaySlowSound();
                            ZoomTextManager.ShowInfo(INFO_SLOW);
                            break;

                        case PowerUp.PowerUpType.Shield:
                            playerManager.ActivateShield();
                            SoundManager.PlayShieldSound();
                            ZoomTextManager.ShowInfo(INFO_SHIELDS);
                            break;

                        case PowerUp.PowerUpType.Overdrive:
                            playerManager.StartOverdrive();
                            SoundManager.PlayExtraShipSound();
                            ZoomTextManager.ShowInfo(INFO_OVERDRIVE);
                            break;

                        case PowerUp.PowerUpType.Underdrive:
                            playerManager.StartUnderdrive();
                            SoundManager.PlayOverheatSound();
                            ZoomTextManager.ShowInfo(INFO_UNDERDRIVE);
                            break;

                        case PowerUp.PowerUpType.Friendly:
                            enemyManager.StartFriendly();
                            SoundManager.PlayFriendlySound();
                            ZoomTextManager.ShowInfo(INFO_FRIENDLY);
                            break;

                        case PowerUp.PowerUpType.Angry:
                            enemyManager.StartAngry();
                            SoundManager.PlayAngrySound();
                            ZoomTextManager.ShowInfo(INFO_ANGRY);
                            break;
                    }

                    powerUp.IsActive = false;
                }
            }
        }

        public void Update()
        {
            checkPlayerShotToAsteroidCollisions();
            checkEnemyShotToAsteroidCollisions();
            checkBossShotToAsteroidCollisions();
            checkShotToEnemyCollisions();
            checkShotToBossCollisions();
            checkRocketToEnemyCollisions();
            checkRocketToBossCollisions();
            checkRocketToAsteroidCollisions();
            checkAsteroidToEnemiesCollisions();
            checkAsteroidToBossesCollisions();
            checkEnemyRocketToAsteroidCollisions();
            checkBossRocketToAsteroidCollisions();
            checkEnemyRocketToPlayerCollisions();
            checkBossRocketToPlayerCollisions();

            if (!playerManager.IsDestroyed)
            {
                checkEnemyShotToPlayerCollisions();
                checkBossShotToPlayerCollisions();
                checkAsteroidToPlayerCollisions();
                checkEnemyToPlayerCollisions();
                checkBossToPlayerCollisions();
                checkPowerUpToPlayerCollision();
            }
        }

        #endregion
    }
}
