using System;
using System.Text;

namespace SpacepiXX
{
    class Highscore
    {
        private string name;
        private long score;
        private int level;
        public const int MaxNameLength = 12;

        public Highscore()
            : this("Unknown", 0, 1)
        {
        }

        public Highscore(string csvText)
        {
            string[] s = csvText.Split(',');

            if (s.Length != 3)
                throw new FormatException("CSV highscore not in the right format.");

            this.Name = s[0];
            this.score = Int64.Parse(s[1]);
            this.level = Int32.Parse(s[2]);
        }

        public Highscore(string name, long score, int level)
        {
            this.Name = name;
            this.score = score;
            this.level = level;
        }

        public string Name
        {
            private set
            {
                string tmp = value.Replace(',',' ').Trim();

                for (int i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i] < 32 || tmp[i] > 126)
                    {
                        tmp = tmp.Replace(tmp[i], ' '); ;
                    }
                }

                this.name = tmp.Substring(0, Math.Min(tmp.Length, MaxNameLength));
            }
            get
            {
                return this.name;
            }
        }

        public long Score
        {
            get
            {
                return this.score;
            }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
        }

        public override string ToString()
        {
            return new StringBuilder().Append(this.name)
                                      .Append(',')
                                      .Append(this.score)
                                      .Append(',')
                                      .Append(this.level)
                                      .ToString();
        }

        public string ScoreText
        {
            get
            {
                return string.Format("{0:00000000000}", score);
            }
        }

        public string LevelText
        {
            get
            {
                return string.Format("{0:00}", level);
            }
        }
    }
}
