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
        private Vector2 offScreen = new Vector2(-500, -500);
        private Vector2 shotToAsteroidImpact = new Vector2(0, -20);

        #endregion

        #region Constructors

        public CollisionManager(AsteroidManager asteroidManager, PlayerManager playerManager,
                                EnemyManager enemyManager)
        {
            this.asteroidManager = asteroidManager;
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
        }

        #endregion

        #region Methods

        private void checkShotToEnemyCollisions()
        {
            foreach (var shot in playerManager.PlayerShotManager.Shots)
            {
                foreach (var enemy in enemyManager.Enemies)
                {
                    if (shot.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius))
                    {
                        enemy.HitPoints -= playerManager.ShotPower;

                        if (enemy.IsDestroyed)
                        {
                            playerManager.PlayerScore += enemy.KillScore;

                            EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                            enemy.EnemySprite.Velocity / 10);
                        }
                        else
                        {
                            playerManager.PlayerScore += enemy.HitScore;

                            EffectManager.AddLargeSparksEffect(shot.Location,
                                                          shot.Velocity,
                                                          -shot.Velocity,
                                                          Color.Orange);
                        }
                        
                        shot.Location = offScreen;
                    }
                }
            }
        }

        private void checkShotToAsteroidCollisions()
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
                                                      asteroid.Velocity);
                        shot.Location = offScreen;
                        asteroid.Velocity += shotToAsteroidImpact;
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
                    playerManager.HitPoints -= enemyManager.ShotPower;

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrateController.Default.Start(TimeSpan.FromSeconds(0.5));
                    }
                    else
                    {
                        EffectManager.AddExplosion(playerManager.playerSprite.Center,
                                                   playerManager.playerSprite.Velocity / 10);

                        VibrateController.Default.Start(TimeSpan.FromSeconds(0.2));
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
                    enemy.HitPoints -= 100.0f;
                    EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                    enemy.EnemySprite.Velocity / 10);

                    playerManager.HitPoints -= 100.0f;
                    EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                    playerManager.playerSprite.Velocity / 10);

                    VibrateController.Default.Start(TimeSpan.FromSeconds(0.5));
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

                    playerManager.HitPoints -= asteroidManager.CrashPower;

                    if (playerManager.IsDestroyed)
                    {
                        EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                        playerManager.playerSprite.Velocity / 10);

                        VibrateController.Default.Start(TimeSpan.FromSeconds(0.5));
                    }
                    else
                    {
                        EffectManager.AddExplosion(playerManager.playerSprite.Center,
                                                   playerManager.playerSprite.Velocity / 10);

                        VibrateController.Default.Start(TimeSpan.FromSeconds(0.2));
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

                        enemy.HitPoints -= asteroidManager.CrashPower;

                        EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                        enemy.EnemySprite.Velocity / 10);
                        
                        asteroid.Location = offScreen;
                    }
                }
            }
        }

        public void Update()
        {
            checkShotToAsteroidCollisions();
            checkShotToEnemyCollisions();
            checkAsteroidToEnemiesCollisions();

            if (!playerManager.IsDestroyed)
            {
                checkShotToPlayerCollisions();
                checkAsteroidToPlayerCollisions();
                checkEnemyToPlayerCollisions();
            }
        }

        #endregion
    }
}
