using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpacepiXX
{
    class ZoomText
    {
        #region Membsers

        public string text;
        public Color drawColor;
        public int displayCounter;
        private int maxDisplayCount = 250;
        private float lastScaleAmount = 0.0f;
        private float scaleRate = 0.05f;

        #endregion

        #region Constructors

        public ZoomText(string text, Color fontColor,
                        int maxDisplayCount, float scaleRate)
        {
            this.text = text;
            this.drawColor = fontColor;
            this.displayCounter = 0;
            this.maxDisplayCount = maxDisplayCount;
            this.scaleRate = scaleRate;
        }

        #endregion

        #region Methods

        public void Update()
        {
            lastScaleAmount += scaleRate;
            displayCounter++;
        }

        #endregion

        #region Properties

        public float Scale
        {
            get
            {
                return scaleRate * displayCounter;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return displayCounter > maxDisplayCount;
            }
        }

        public float Progress
        {
            get
            {
                return (float)displayCounter / maxDisplayCount;
            }
        }

        #endregion
    }
}
