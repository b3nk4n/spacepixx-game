using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace SpacepiXX
{
    public static class SoundManager
    {
        #region Members

        private static List<SoundEffect> explosions = new List<SoundEffect>();
        private static int explosionsCount = 4;
        private static SoundEffect playerShot;
        private static SoundEffect enemyShot;

        private static Random rand = new Random();

        #endregion

        #region Methods

        public static void Initialize(ContentManager content)
        {
            try
            {
                playerShot = content.Load<SoundEffect>(@"Sounds\Shot1");
                enemyShot = content.Load<SoundEffect>(@"Sounds\Shot2");

                for (int x = 1; x <= explosionsCount; x++)
                {
                    explosions.Add(content.Load<SoundEffect>(@"Sounds\Explosion"
                                                             + x.ToString()));
                }
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
                explosions[rand.Next(0, explosionsCount)].Play();
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
                playerShot.Play();
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
                enemyShot.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        #endregion
    }
}
