using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Devices;

namespace SpacepiXX
{
    class CollisionManager
    {
        #region Members

        private AsteroidManager asteroidManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private PowerUpManager powerUpManager;
        private Vector2 offScreen = new Vector2(-500, -500);
        private Vector2 shotToAsteroidImpact = new Vector2(0, -20);

        Random rand = new Random();

        #endregion

        #region Constructors

        public CollisionManager(AsteroidManager asteroidManager, PlayerManager playerManager,
                                EnemyManager enemyManager, PowerUpManager powerUpManager)
        {
            this.asteroidManager = asteroidManager;
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
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

                        rocket.Location = offScreen;
                        asteroid.Location = offScreen;
                    }
                }
            }
        }

        private void checkRocketToPlayerCollisions()
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

                            if (playerManager.IsDestroyed)
                            {
                                EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                                playerManager.playerSprite.Velocity / 10);
                            }
                        }

                        rocket.Location = offScreen;
                        asteroid.Location = offScreen;
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
                    }
                }
            }
        }

        private void checkShotToPlayerCollisions()
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
                            ZoomTextManager.ShowInfo("+25% Repair-Kit");
                            break;

                        case PowerUp.PowerUpType.Health50:
                            playerManager.IncreaseHitPoints(50.0f);
                            ZoomTextManager.ShowInfo("+50% Repair-Kit");
                            SoundManager.PlayRepairSound();
                            break;

                        case PowerUp.PowerUpType.Health100:
                            playerManager.IncreaseHitPoints(100.0f);
                            ZoomTextManager.ShowInfo("+100% Repair-Kit");
                            SoundManager.PlayRepairSound();
                            break;

                        case PowerUp.PowerUpType.CoolWater:
                            playerManager.Overheat = 0.0f;
                            SoundManager.PlayCoolWaterSound();
                            ZoomTextManager.ShowInfo("Cooling Water");
                            break;

                        case PowerUp.PowerUpType.Life:
                            playerManager.LivesRemaining++;
                            SoundManager.PlayExtraShipSound();
                            ZoomTextManager.ShowInfo("Extra Spaceship!");
                            break;

                        case PowerUp.PowerUpType.SpecialShot:
                            playerManager.SpecialShotsRemaining++;
                            SoundManager.PlayExtraSpecialShotSound();
                            ZoomTextManager.ShowInfo("+1 Super-Laser");
                            break;

                        case PowerUp.PowerUpType.BonusRockets:
                            playerManager.CarliRocketsRemaining += 3;
                            SoundManager.PlayBonusRocketSound();
                            ZoomTextManager.ShowInfo("+2 C.A.R.L.I-Rockets");
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

                            VibrationManager.Vibrate(1.0f);
                            SoundManager.PlayNukeExplosionSound();
                            ZoomTextManager.ShowInfo("!!! N U K E !!!");
                            break;

                        case PowerUp.PowerUpType.LowBonusScore:
                            playerManager.IncreasePlayerScore(1000, false);
                            SoundManager.PlayCoinSound();
                            ZoomTextManager.ShowInfo("+1000");
                            break;

                        case PowerUp.PowerUpType.MediumBonusScore:
                            playerManager.IncreasePlayerScore(2500, false);
                            SoundManager.PlayCoinSound();
                            ZoomTextManager.ShowInfo("+2500");
                            break;

                        case PowerUp.PowerUpType.HighBonusScore:
                            playerManager.IncreasePlayerScore(5000, false);
                            SoundManager.PlayCoinSound();
                            ZoomTextManager.ShowInfo("+5000");
                            break;

                        case PowerUp.PowerUpType.ScoreMultiLow:
                            playerManager.ScoreMulti += 0.5f;
                            SoundManager.PlayScoreMultiSound();
                            ZoomTextManager.ShowInfo("+50% Score-Multiplier");
                            break;

                        case PowerUp.PowerUpType.ScoreMultiMedium:
                            playerManager.ScoreMulti += 0.75f;
                            SoundManager.PlayScoreMultiSound();
                            ZoomTextManager.ShowInfo("+75% Score-Multiplier");
                            break;

                        case PowerUp.PowerUpType.ScoreMultiHigh:
                            playerManager.ScoreMulti += 1.0f;
                            SoundManager.PlayScoreMultiSound();
                            ZoomTextManager.ShowInfo("+100% Score-Multiplier");
                            break;

                        case PowerUp.PowerUpType.AntiScoreMulti:
                            playerManager.ScoreMulti = PlayerManager.MIN_SCORE_MULTI;
                            SoundManager.PlayAntiScoreMultiSound();
                            ZoomTextManager.ShowInfo("Score-Multiplier Lost!");
                            break;

                        case PowerUp.PowerUpType.OverHeat:
                            playerManager.Overheat = PlayerManager.OVERHEAT_MAX;
                            SoundManager.PlayOverheatSound();
                            ZoomTextManager.ShowInfo("Overheating Problem!");
                            break;

                        case PowerUp.PowerUpType.OutOfControl:
                            playerManager.StartOutOfControl();
                            SoundManager.PlayOutOfControlSound();
                            ZoomTextManager.ShowInfo("We get out of control!");
                            break;

                        case PowerUp.PowerUpType.Slow:
                            playerManager.StartSlow();
                            SoundManager.PlaySlowSound();
                            ZoomTextManager.ShowInfo("Hyper Eninge damaged!");
                            break;

                        case PowerUp.PowerUpType.Shield:
                            playerManager.ActivateShield();
                            SoundManager.PlayShieldSound();
                            ZoomTextManager.ShowInfo("Activated Shields");
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
            checkShotToEnemyCollisions();
            checkRocketToEnemyCollisions();
            checkRocketToAsteroidCollisions();
            checkAsteroidToEnemiesCollisions();
            checkEnemyRocketToAsteroidCollisions();
            checkRocketToPlayerCollisions();

            if (!playerManager.IsDestroyed)
            {
                checkShotToPlayerCollisions();
                checkAsteroidToPlayerCollisions();
                checkEnemyToPlayerCollisions();
                checkPowerUpToPlayerCollision();
            }
        }

        #endregion
    }
}
