using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

namespace SpacepiXX
{
    public static class SoundManager
    {
        #region Members

        private static SettingsManager settings;

        private static List<SoundEffect> explosions = new List<SoundEffect>();
        private static int explosionsCount = 4;

        private static SoundEffect playerShot;
        private static SoundEffect enemyShot;

        private static SoundEffect coinSound;
        private static SoundEffect repairSound;
        private static SoundEffect coolWaterSound;
        private static SoundEffect extraShipSound;
        private static SoundEffect extraSpeciaShotSound;
        private static SoundEffect nukeExplosionSound;
        private static SoundEffect rocketSound;
        private static SoundEffect scoreMultiSound;
        private static SoundEffect antiScoreMultiSound;
        private static SoundEffect bonusRocketSound;
        private static SoundEffect slowSound;
        private static SoundEffect outOfControlSound;
        private static SoundEffect overheatSound;
        private static SoundEffect enemyRocketSound;
        private static SoundEffect shieldSound;

        private static SoundEffect bossEasySound;
        private static SoundEffect bossMediumSound;
        private static SoundEffect bossHardSound;
        private static SoundEffect bossSpeederSound;
        private static SoundEffect bossTankSound;

        private static List<SoundEffect> hitSounds = new List<SoundEffect>();
        private static int hitSoundsCount = 6;

        private static List<SoundEffect> asteroidHitSounds = new List<SoundEffect>();
        private static int asteroidHitSoundsCount = 5;

        private static Random rand = new Random();

        private static Song backgroundSound;
        //private static SoundEffectInstance backgroundSound;

        #endregion

        #region Methods

        public static void Initialize(ContentManager content)
        {
            try
            {
                settings = SettingsManager.GetInstance();

                playerShot = content.Load<SoundEffect>(@"Sounds\Shot1");
                enemyShot = content.Load<SoundEffect>(@"Sounds\Shot2");

                repairSound = content.Load<SoundEffect>(@"Sounds\Repair");
                coinSound = content.Load<SoundEffect>(@"Sounds\Coin");
                coolWaterSound = content.Load<SoundEffect>(@"Sounds\CoolWater");
                extraShipSound = content.Load<SoundEffect>(@"Sounds\Life");
                extraSpeciaShotSound = content.Load<SoundEffect>(@"Sounds\SpecialShot");
                nukeExplosionSound = content.Load<SoundEffect>(@"Sounds\Nuke");
                rocketSound = content.Load<SoundEffect>(@"Sounds\Rocket");
                scoreMultiSound = content.Load<SoundEffect>(@"Sounds\ScoreMulti");
                antiScoreMultiSound = content.Load<SoundEffect>(@"Sounds\AntiScoreMulti");
                bonusRocketSound = content.Load<SoundEffect>(@"Sounds\BonusRockets");
                slowSound = content.Load<SoundEffect>(@"Sounds\Slow");
                outOfControlSound = content.Load<SoundEffect>(@"Sounds\OutOfControl");
                overheatSound = content.Load<SoundEffect>(@"Sounds\Overheat");
                enemyRocketSound = content.Load<SoundEffect>(@"Sounds\EnemyRocket");
                shieldSound = content.Load<SoundEffect>(@"Sounds\Shield");

                bossEasySound = content.Load<SoundEffect>(@"Sounds\boss_easy");
                bossMediumSound = content.Load<SoundEffect>(@"Sounds\boss_medium");
                bossHardSound = content.Load<SoundEffect>(@"Sounds\boss_hard");
                bossSpeederSound = content.Load<SoundEffect>(@"Sounds\boss_speeder");
                bossTankSound = content.Load<SoundEffect>(@"Sounds\boss_tank");

                for (int x = 1; x <= explosionsCount; x++)
                {
                    explosions.Add(content.Load<SoundEffect>(@"Sounds\Explosion"
                                                             + x.ToString()));
                }

                for (int x = 1; x <= hitSoundsCount; x++)
                {
                    hitSounds.Add(content.Load<SoundEffect>(@"Sounds\Hit"
                                                             + x.ToString()));
                }

                for (int x = 1; x <= asteroidHitSoundsCount; x++)
                {
                    asteroidHitSounds.Add(content.Load<SoundEffect>(@"Sounds\AsteroidHit"
                                                             + x.ToString()));
                }

                backgroundSound = content.Load<Song>(@"Sounds\GameSound");
            }
            catch
            {
                Debug.WriteLine("SoundManager: Content not found.");
            }
        }

        public static void PlayExplosion()
        {
            try
            {
                SoundEffectInstance s = explosions[rand.Next(0, explosionsCount)].CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayPlayerShot()
        {
            try
            {
                SoundEffectInstance s = playerShot.CreateInstance();
                s.Volume = 0.75f * settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play player shot failed.");
            }
        }

        public static void PlayEnemyShot()
        {
            try
            {
                SoundEffectInstance s = enemyShot.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayCoinSound()
        {
            try
            {
                SoundEffectInstance s = coinSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayRepairSound()
        {
            try
            {
                SoundEffectInstance s = repairSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayCoolWaterSound()
        {
            try
            {
                SoundEffectInstance s = coolWaterSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayExtraShipSound()
        {
            try
            {
                SoundEffectInstance s = extraShipSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayExtraSpecialShotSound()
        {
            try
            {
                SoundEffectInstance s = extraSpeciaShotSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayHitSound()
        {
            try
            {
                SoundEffectInstance s = hitSounds[rand.Next(0, hitSoundsCount)].CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayAsteroidHitSound()
        {
            try
            {
                SoundEffectInstance s = asteroidHitSounds[rand.Next(0, asteroidHitSoundsCount)].CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayNukeExplosionSound()
        {
            try
            {
                SoundEffectInstance s = nukeExplosionSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayRocketSound()
        {
            try
            {
                SoundEffectInstance s = rocketSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayScoreMultiSound()
        {
            try
            {
                SoundEffectInstance s = scoreMultiSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayAntiScoreMultiSound()
        {
            try
            {
                SoundEffectInstance s = antiScoreMultiSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayBonusRocketSound()
        {
            try
            {
                SoundEffectInstance s = bonusRocketSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayOutOfControlSound()
        {
            try
            {
                SoundEffectInstance s = outOfControlSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlaySlowSound()
        {
            try
            {
                SoundEffectInstance s = slowSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayOverheatSound()
        {
            try
            {
                SoundEffectInstance s = overheatSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayEnemyRocketSound()
        {
            try
            {
                SoundEffectInstance s = enemyRocketSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayShieldSound()
        {
            try
            {
                SoundEffectInstance s = shieldSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayBossEasySound()
        {
            try
            {
                SoundEffectInstance s = bossEasySound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayBossMediumSound()
        {
            try
            {
                SoundEffectInstance s = bossMediumSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayBossHardSound()
        {
            try
            {
                SoundEffectInstance s = bossHardSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayBossSpeederSound()
        {
            try
            {
                SoundEffectInstance s = bossSpeederSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayBossTankSound()
        {
            try
            {
                SoundEffectInstance s = bossTankSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play boss sound failed.");
            }
        }

        public static void PlayBackgroundSound()
        {
            try
            {
                if (MediaPlayer.GameHasControl)
                {
                    MediaPlayer.Play(backgroundSound);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = settings.GetMusicValue();
                }
            }
            catch (UnauthorizedAccessException)
            {
                // play no music...
            }
            catch (InvalidOperationException)
            {
                // play no music (because of Zune on PC)
            }
        }

        public static void RefreshMusicVolume()
        {
            float val = settings.GetMusicValue();
            MediaPlayer.Volume = val;
        }

        #endregion
    }
}
