using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Phone.Tasks;
using SpacepiXX.Inputs;

namespace SpacepiXX
{
    class HighscoreManager
    {
        #region Members

        LeaderboardManager leaderboardManager;

        public enum ScoreState { Local, OnlineAll, OnlineWeek, OnlineMe };

        private ScoreState scoreState = ScoreState.Local;

        private readonly Rectangle switcherSource = new Rectangle(400, 350, 
                                                                  100, 100);
        private readonly Rectangle switcherRightDestination = new Rectangle(700, 0,
                                                                            100, 100);

        private readonly Rectangle switcherLeftDestination = new Rectangle(0, 0,
                                                                           100, 100);

        private readonly Rectangle browserSource = new Rectangle(400, 700,
                                                                 100, 100);
        private readonly Rectangle browserDestination = new Rectangle(700, 190,
                                                                      100, 100);

        private readonly Rectangle refreshSource = new Rectangle(400, 450,
                                                                 100, 100);
        private readonly Rectangle refreshDestination = new Rectangle(700, 380,
                                                                      100, 100);

        private readonly Rectangle resubmitSource = new Rectangle(300, 700,
                                                                 100, 100);
        private readonly Rectangle resubmitDestination = new Rectangle(700, 380,
                                                                      100, 100);

        private static HighscoreManager highscoreManager;

        private long currentHighScore;

        private List<Highscore> topScores = new List<Highscore>();
        public const int MaxScores = 10;

        public static Texture2D Texture;
        public static SpriteFont Font;
        private readonly Rectangle LocalTitleSource = new Rectangle(0, 450,
                                                                        300, 50);
        private readonly Rectangle OnlineAllTitleSource = new Rectangle(0, 500,
                                                                        300, 50);
        private readonly Rectangle OnlineWeekTitleSource = new Rectangle(0, 550,
                                                                        300, 50);
        private readonly Rectangle OnlineMeTitleSource = new Rectangle(0, 800,
                                                                        300, 50);
        private readonly Vector2 TitlePosition = new Vector2(250.0f, 30.0f);

        private string lastName = "Unknown";

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private WebBrowserTask browser;
        private const string BROWSER_URL = "http://bensaute.cwsurf.de/spacepixx/requestscores.php?Method=TOP100WEB";

        private const string TEXT_ME = "Your best personal online achievements:";
        private const string TEXT_RANK = "Best Rank:";
        private const string TEXT_SCORE = "Best Score:";
        private const string TEXT_LEVEL = "Best Level:";

        public static GameInput GameInput;
        private const string RefreshAction = "Refresh";
        private const string GoLeftAction = "GoLeft";
        private const string GoRightAction = "GoRight";
        private const string BrowserAction = "Browser";
        private const string ResubmitAction = "Resubmit";

        private float switchPageTimer = 0.0f;
        private const float SwitchPageMinTimer = 0.25f;

        #endregion

        #region Constructors

        private HighscoreManager()
        {
            leaderboardManager = LeaderboardManager.GetInstance();

            browser = new WebBrowserTask();
            browser.URL = BROWSER_URL;

            this.LoadHighScore();

            this.loadLastName();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(RefreshAction,
                                           GestureType.Tap,
                                           refreshDestination);
            GameInput.AddTouchGestureInput(GoLeftAction,
                                           GestureType.Tap,
                                           switcherLeftDestination);
            GameInput.AddTouchGestureInput(GoRightAction,
                                           GestureType.Tap,
                                           switcherRightDestination);
            GameInput.AddTouchGestureInput(BrowserAction,
                                           GestureType.Tap,
                                           browserDestination);
            GameInput.AddTouchGestureInput(ResubmitAction,
                                           GestureType.Tap,
                                           resubmitDestination);

            GameInput.AddTouchSlideInput(GoLeftAction,
                                         Input.Direction.Left,
                                         50.0f);
            GameInput.AddTouchSlideInput(GoRightAction,
                                         Input.Direction.Right,
                                         50.0f);
        }

        public static HighscoreManager GetInstance()
        {
            if (highscoreManager == null)
            {
                highscoreManager = new HighscoreManager();
            }

            return highscoreManager;
        }

        private void handleTouchInputs()
        {
            //if (TouchPanel.IsGestureAvailable)
            //{
            //    GestureSample gs = TouchPanel.ReadGesture();

            //    if (gs.GestureType == GestureType.Tap)
            //    {
            //        // Switcher right
            //        if (switcherRightDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
            //        {
            //            if (scoreState == ScoreState.Local)
            //                scoreState = ScoreState.OnlineAll;
            //            else if (scoreState == ScoreState.OnlineAll)
            //                scoreState = ScoreState.OnlineWeek;
            //            else if (scoreState == ScoreState.OnlineWeek)
            //                scoreState = ScoreState.OnlineMe;
            //            else
            //                scoreState = ScoreState.Local;
            //        }
            //        // Switcher left
            //        if (switcherLeftDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
            //        {
            //            if (scoreState == ScoreState.Local)
            //                scoreState = ScoreState.OnlineMe;
            //            else if (scoreState == ScoreState.OnlineMe)
            //                scoreState = ScoreState.OnlineWeek;
            //            else if (scoreState == ScoreState.OnlineWeek)
            //                scoreState = ScoreState.OnlineAll;
            //            else
            //                scoreState = ScoreState.Local;
            //        }
            //        // Resubmit
            //        if (resubmitDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
            //        {
            //            if (scoreState == ScoreState.Local && topScores.Count > 0 && topScores[0].Score > 0)
            //            {
            //                leaderboardManager.Submit(LeaderboardManager.RESUBMIT,
            //                                          topScores[0].Name,
            //                                          topScores[0].Score,
            //                                          topScores[0].Level);
            //            }
            //        }
            //        // Browser - TOP100
            //        if (browserDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
            //        {
            //            if (scoreState == ScoreState.OnlineAll || scoreState == ScoreState.OnlineWeek || scoreState == ScoreState.OnlineMe)
            //            {
            //                browser.Show();
            //            }
            //        }
            //        // Refresh
            //        if (refreshDestination.Contains((int)gs.Position.X, (int)gs.Position.Y))
            //        {
            //            if (scoreState == ScoreState.OnlineAll || scoreState == ScoreState.OnlineWeek || scoreState == ScoreState.OnlineMe)
            //            {
            //                leaderboardManager.Receive();
            //            }
            //        }
            //    }
            //}

            // Switcher right
            if (GameInput.IsPressed(GoRightAction) && switchPageTimer > SwitchPageMinTimer)
            {
                switchPageTimer = 0.0f;

                if (scoreState == ScoreState.Local)
                    scoreState = ScoreState.OnlineAll;
                else if (scoreState == ScoreState.OnlineAll)
                    scoreState = ScoreState.OnlineWeek;
                else if (scoreState == ScoreState.OnlineWeek)
                    scoreState = ScoreState.OnlineMe;
                else
                    scoreState = ScoreState.Local;
            }
            // Switcher left
            if (GameInput.IsPressed(GoLeftAction) && switchPageTimer > SwitchPageMinTimer)
            {
                switchPageTimer = 0.0f;

                if (scoreState == ScoreState.Local)
                    scoreState = ScoreState.OnlineMe;
                else if (scoreState == ScoreState.OnlineMe)
                    scoreState = ScoreState.OnlineWeek;
                else if (scoreState == ScoreState.OnlineWeek)
                    scoreState = ScoreState.OnlineAll;
                else
                    scoreState = ScoreState.Local;
            }
            // Resubmit
            if (GameInput.IsPressed(ResubmitAction))
            {
                if (scoreState == ScoreState.Local && topScores.Count > 0 && topScores[0].Score > 0)
                {
                    leaderboardManager.Submit(LeaderboardManager.RESUBMIT,
                                              topScores[0].Name,
                                              topScores[0].Score,
                                              topScores[0].Level);
                }
            }
            // Browser - Top100
            if (GameInput.IsPressed(BrowserAction))
            {
                if (scoreState == ScoreState.OnlineAll || scoreState == ScoreState.OnlineWeek || scoreState == ScoreState.OnlineMe)
                {
                    browser.Show();
                }
            }
            // Refresh
            if (GameInput.IsPressed(RefreshAction))
            {
                if (scoreState == ScoreState.OnlineAll || scoreState == ScoreState.OnlineWeek || scoreState == ScoreState.OnlineMe)
                {
                    leaderboardManager.Receive();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isActive)
            {
                switchPageTimer += elapsed;

                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                             switcherRightDestination,
                             switcherSource,
                             Color.Red * opacity);

            spriteBatch.Draw(Texture,
                             switcherLeftDestination,
                             switcherSource,
                             Color.Red * opacity,
                             0.0f,
                             Vector2.Zero,
                             SpriteEffects.FlipHorizontally,
                             0.0f);

            spriteBatch.DrawString(Font,
                                       leaderboardManager.StatusText,
                                       new Vector2(800 / 2 - Font.MeasureString(leaderboardManager.StatusText).X / 2,
                                                   440),
                                       Color.Red * opacity);

            if (scoreState == ScoreState.Local)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 LocalTitleSource,
                                 Color.White * opacity);

                if (topScores.Count > 0 && topScores[0].Score > 0)
                {
                    spriteBatch.Draw(Texture,
                                     resubmitDestination,
                                     resubmitSource,
                                     Color.Red * opacity);
                }

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
                        scoreText = ". . . . . . . . . . . . ";
                        nameText = ". . . . . . . . . . . . . . . . . . . . . ";
                        levelText = ". . . ";
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(50, 100 + (i * 33)),
                           Color.Red * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(130, 100 + (i * 33)),
                                           Color.Red * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(440, 100 + (i * 33)),
                                           Color.Red * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(620, 100 + (i * 33)),
                                           Color.Red * opacity);
                }
            }

            if (scoreState == ScoreState.OnlineAll)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 OnlineAllTitleSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 browserDestination,
                                 browserSource,
                                 Color.Red * opacity);

                spriteBatch.Draw(Texture,
                                 refreshDestination,
                                 refreshSource,
                                 Color.Red * opacity);

                for (int i = 0; i < MaxScores; i++)
                {
                    string scoreText;
                    string nameText;
                    string levelText;

                    if (leaderboardManager.TopScoresAll.Count > i)
                    {
                        Highscore h = new Highscore(leaderboardManager.TopScoresAll[i].Name, leaderboardManager.TopScoresAll[i].Score, leaderboardManager.TopScoresAll[i].Level);

                        scoreText = h.ScoreText;
                        nameText = h.Name;
                        levelText = h.LevelText;
                    }
                    else
                    {
                        scoreText = ". . . . . . . . . . . . ";
                        nameText = ". . . . . . . . . . . . . . . . . . . . . ";
                        levelText = ". . . ";
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(50, 100 + (i * 33)),
                           Color.Red * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(130, 100 + (i * 33)),
                                           Color.Red * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(440, 100 + (i * 33)),
                                           Color.Red * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(620, 100 + (i * 33)),
                                           Color.Red * opacity);
                }
            }

            if (scoreState == ScoreState.OnlineWeek)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 OnlineWeekTitleSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 browserDestination,
                                 browserSource,
                                 Color.Red * opacity);

                spriteBatch.Draw(Texture,
                                 refreshDestination,
                                 refreshSource,
                                 Color.Red * opacity);

                for (int i = 0; i < MaxScores; i++)
                {
                    string scoreText;
                    string nameText;
                    string levelText;

                    if (leaderboardManager.TopScoresWeek.Count > i)
                    {
                        Highscore h = new Highscore(leaderboardManager.TopScoresWeek[i].Name, leaderboardManager.TopScoresWeek[i].Score, leaderboardManager.TopScoresWeek[i].Level);

                        scoreText = h.ScoreText;
                        nameText = h.Name;
                        levelText = h.LevelText;
                    }
                    else
                    {
                        scoreText = ". . . . . . . . . . . . ";
                        nameText = ". . . . . . . . . . . . . . . . . . . . . ";
                        levelText = ". . . ";
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(50, 100 + (i * 33)),
                           Color.Red * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(130, 100 + (i * 33)),
                                           Color.Red * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(440, 100 + (i * 33)),
                                           Color.Red * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(620, 100 + (i * 33)),
                                           Color.Red * opacity);
                }
            }

            if (scoreState == ScoreState.OnlineMe)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 OnlineMeTitleSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 browserDestination,
                                 browserSource,
                                 Color.Red * opacity);

                spriteBatch.Draw(Texture,
                                 refreshDestination,
                                 refreshSource,
                                 Color.Red * opacity);

                spriteBatch.DrawString(Font,
                                   TEXT_ME,
                                   new Vector2(800 / 2 - Font.MeasureString(TEXT_ME).X / 2,
                                               150),
                                   Color.Red * opacity);

                // Title:
                spriteBatch.DrawString(Font,
                                       TEXT_RANK,
                                       new Vector2(250,
                                                   225),
                                       Color.Red * opacity);

                spriteBatch.DrawString(Font,
                                       TEXT_SCORE,
                                       new Vector2(250,
                                                   275),
                                       Color.Red * opacity);

                spriteBatch.DrawString(Font,
                                       TEXT_LEVEL,
                                       new Vector2(250,
                                                   325),
                                       Color.Red * opacity);

                // Content:
                int topRank = leaderboardManager.TopRankMe;
                long topScore = leaderboardManager.TopScoreMe;
                int topLevel = leaderboardManager.TopLevelMe;
                string topRankText;
                string topScoreText;
                string topLevelText;

                if (topRank == 0)
                    topRankText = ". . . . . . ";
                else
                    topRankText = string.Format("{0:00000}", leaderboardManager.TopRankMe);

                if (topScore == 0)
                    topScoreText = ". . . . . . . . . . . . ";
                else
                    topScoreText = string.Format("{0:00000000000}", leaderboardManager.TopScoreMe);

                if (topLevel == 0)
                    topLevelText = ". . . ";
                else
                    topLevelText = string.Format("{0:00}", leaderboardManager.TopLevelMe);


                spriteBatch.DrawString(Font,
                                       topRankText,
                                       new Vector2(550 - Font.MeasureString(topRankText).X,
                                                   225),
                                       Color.Red * opacity);

                spriteBatch.DrawString(Font,
                                       topScoreText,
                                       new Vector2(550 - Font.MeasureString(topScoreText).X,
                                                   275),
                                       Color.Red * opacity);

                spriteBatch.DrawString(Font,
                                       topLevelText,
                                       new Vector2(550 - Font.MeasureString(topLevelText).X,
                                                   325),
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
                    using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("highscore2.txt", FileMode.Create, isf))
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

            this.saveLastName();
        }

        /// <summary>
        /// Loads the high score from a text file.
        /// </summary>
        private void LoadHighScore()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(@"highscore2.txt");

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(@"highscore2.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
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

                // Delete the old file
                if (isf.FileExists(@"highscore.txt"))
                    isf.DeleteFile(@"highscore.txt");
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
        /// Checks wheather the score reaches top 10.
        /// </summary>
        /// <param name="score">The score to check</param>
        /// <returns>True if the player is under the top 1.</returns>
        public bool IsInScoreboard(long score)
        {
            return score > topScores[MaxScores - 1].Score;
        }

        /// <summary>
        /// Calculates the rank of the new score.
        /// </summary>
        /// <param name="score">The new score</param>
        /// <returns>Returns the calculated rank (-1, if the score is not top 10).</returns>
        public int GetRank(long score)
        {
            if (topScores.Count < 0)
                return 1;

            for (int i = 0; i < topScores.Count; i++)
            {
                if (topScores[i].Score < score)
                    return i + 1;
            }

            return -1;
        }

        private void saveLastName()
        {

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("user.txt", FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(this.LastName);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        private void loadLastName()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(@"user.txt");

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(@"user.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            this.lastName = sr.ReadLine();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(this.lastName);

                            // ... ? 
                        }
                    }
                }
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.currentHighScore = Int64.Parse(reader.ReadLine());
            this.lastName = reader.ReadLine();
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
            this.scoreState = (ScoreState)Enum.Parse(scoreState.GetType(), reader.ReadLine(), false);
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(currentHighScore);
            writer.WriteLine(lastName);
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
            writer.WriteLine(scoreState);
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
