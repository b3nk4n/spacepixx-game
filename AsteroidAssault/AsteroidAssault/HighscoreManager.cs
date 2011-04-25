using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class HighscoreManager
    {
        #region Members

        private static HighscoreManager highscoreManager;

        private long currentHighScore;

        private List<Highscore> topScores = new List<Highscore>();
        public const int MaxScores = 10;

        public static Texture2D Texture;
        public static SpriteFont Font;
        private readonly Rectangle HighscoreTitleDestination = new Rectangle(0, 250,
                                                                             300, 50);
        private readonly Vector2 TitlePosition = new Vector2(250.0f, 40.0f);

        private string lastName = "Unknown";

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        #endregion

        #region Constructors

        private HighscoreManager()
        {
            this.LoadHighScore();
        }

        #endregion

        #region Methods

        public static HighscoreManager GetInstance()
        {
            if (highscoreManager == null)
            {
                highscoreManager = new HighscoreManager();
            }

            return highscoreManager;
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                             TitlePosition,
                             HighscoreTitleDestination,
                             Color.White * opacity);

            for (int i = 0; i < MaxScores; i++)
            {
                string scoreText;
                string nameText;
                string levelText;

                if (topScores[i].Score > 0)
                {
                    Highscore h = new Highscore(topScores[i].Name, topScores[i].Score, topScores[i].Level);

                    scoreText = h.ScoreText;
                    nameText = h.Name;
                    levelText = h.LevelText;
                }
                else
                {
                    scoreText = ". . . . . . . . . ";
                    nameText = ". . . . . . . . . . . . . . . . . . . . . ";
                    levelText = ". . . ";
                }

                spriteBatch.DrawString(Font,
                       string.Format("{0:d}.", i + 1),
                       new Vector2(100, 100 + (i * 35)),
                       Color.Red * opacity);

                spriteBatch.DrawString(Font,
                                       nameText,
                                       new Vector2(180, 100 + (i * 35)),
                                       Color.Red * opacity);

                spriteBatch.DrawString(Font,
                                       scoreText,
                                       new Vector2(500, 100 + (i * 35)),
                                       Color.Red * opacity);

                spriteBatch.DrawString(Font,
                                       levelText,
                                       new Vector2(670, 100 + (i * 35)),
                                       Color.Red * opacity);
            }
        }


        /// <summary>
        /// Saves the current highscore to a text file.
        /// </summary>
        public void SaveHighScore(string name, long score, int level)
        {
            if(this.IsInScoreboard(score))
            {
                Highscore newScore = new Highscore(name, score, level);

                topScores.Add(newScore);
                this.sortScoreList();
                this.trimScoreList();

                this.lastName = name;

                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("highscore.txt", FileMode.Create, isf))
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            //sw.Write(score.ToString(System.Globalization.CultureInfo.InvariantCulture));

                            for (int i = 0; i < MaxScores; i++)
                            {
                                sw.WriteLine(topScores[i]);
                            }

                            sw.Flush();
                            sw.Close();
                        }
                    }
                }

                this.currentHighScore = maxScore();
            } 
        }

        /// <summary>
        /// Loads the high score from a text file.
        /// </summary>
        private void LoadHighScore()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(@"highscore.txt");

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(@"highscore.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            //this.currentHighScore = Int32.Parse(sr.ReadToEnd(), System.Globalization.CultureInfo.InvariantCulture);

                            for (int i = 0; i < MaxScores; i++)
                            {
                                topScores.Add(new Highscore(sr.ReadLine()));
                            }

                            this.sortScoreList();
                            this.currentHighScore = this.maxScore();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            //this.currentHighScore = 0;
                            //sw.WriteLine(0);

                            for (int i = 0; i < MaxScores; i++)
                            {
                                Highscore newScore = new Highscore();
                                topScores.Add(newScore);
                                sw.WriteLine(newScore);
                            }

                            this.currentHighScore = 0;
                        }
                    }  
                }
            }
        }

        private void sortScoreList()
        {
            for (int i = 0; i < topScores.Count; i++)
            {
                for (int j = 0; j < topScores.Count - 1; j++)
                {
                    if (topScores[j].Score < topScores[j + 1].Score)
                    {
                        Highscore tmp = topScores[j];
                        topScores[j] = topScores[j + 1];
                        topScores[j + 1] = tmp;
                    }
                }
            }
        }

        private void trimScoreList()
        {
            while (topScores.Count > MaxScores)
            {
                topScores.RemoveAt(topScores.Count - 1);
            }
        }

        private long maxScore()
        {
            long max = 0;

            for (int i = 0; i < topScores.Count; i++)
            {
                max = Math.Max(max, topScores[i].Score);
            }

            return max;
        }

        /// <summary>
        /// Checks wheather the score reaches top 5.
        /// </summary>
        /// <param name="score">The score to check</param>
        /// <returns>True if the player is under the top 5.</returns>
        public bool IsInScoreboard(long score)
        {
            return score > topScores[MaxScores - 1].Score;
        }

        #endregion

        #region Properties

        public long CurrentHighscore
        {
            get
            {
                return this.currentHighScore;
            }
        }

        public string LastName
        {
            get
            {
                return this.lastName;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;

                if (isActive == false)
                {
                    this.opacity = OpacityMin;
                }
            }
        }

        #endregion
    }
}
