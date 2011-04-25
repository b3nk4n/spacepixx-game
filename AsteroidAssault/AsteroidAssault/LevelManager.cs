using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacepiXX
{
    class LevelManager
    {
        #region Members

        private List<ILevel> components = new List<ILevel>();

        public const int StartLevel = 1;

        private float levelTimer = 0.0f;
        public const float TimeForLevel = 45.0f;

        private int currentLevel;
        private int lastLevel;

        private bool hasChanged = false;

        #endregion

        #region Constructors

        public LevelManager()
        {
            Reset();
        }

        #endregion

        #region Methods

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            levelTimer += elapsed;

            if (levelTimer >= LevelManager.TimeForLevel)
            {
                SetLevelAll(currentLevel + 1);

                levelTimer = 0.0f;
            }
        }

        public void Register(ILevel comp)
        {
            if (!components.Contains(comp))
            {
                components.Add(comp);
            }
        }

        private void SetLevelAll(int lvl)
        {
            this.lastLevel = this.currentLevel;
            this.currentLevel = lvl;

            foreach (var comp in components)
            {
                comp.SetLevel(lvl);
            }

            this.hasChanged = true;
        }

        public void Reset()
        {
            levelTimer = 0.0f;

            SetLevelAll(LevelManager.StartLevel);
        }

        #endregion

        #region Properties

        public int CurrentLevel
        {
            get
            {
                return this.currentLevel;
            }
        }

        public int LastLevel
        {
            get
            {
                return this.lastLevel;
            }
        }

        public bool HasChanged
        {
            get
            {
                bool tmp = this.hasChanged;
                this.hasChanged = false;
                return tmp;
            }
        }

        #endregion
    }
}
