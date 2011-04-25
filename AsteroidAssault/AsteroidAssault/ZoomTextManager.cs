using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacepiXX
{
    class ZoomTextManager
    {
        #region Members

        private Queue<ZoomText> zoomTexts = new Queue<ZoomText>();
        private Vector2 location;
        private SpriteFont font;

        #endregion

        #region Constructors

        public ZoomTextManager(Vector2 location, SpriteFont font)
        {
            this.location = location;
            this.font = font;
        }

        #endregion

        #region Methods

        public void ShowText(string text)
        {
            zoomTexts.Enqueue(new ZoomText(text,
                                           Color.Red,
                                           60,
                                           0.10f));
        }

        public void Update()
        {
            if (zoomTexts.Count > 0)
            {
                zoomTexts.First().Update();

                if (zoomTexts.First().IsCompleted)
                {
                    zoomTexts.Dequeue();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var zoom in zoomTexts)
            {
                spriteBatch.DrawString(font,
                                       zoom.text,
                                       location,
                                       zoom.drawColor * (float)(1.0f - Math.Pow(zoom.Progress, 4.0f)),
                                       0.0f,
                                       new Vector2(font.MeasureString(zoom.text).X / 2,
                                                   font.MeasureString(zoom.text).Y / 2),
                                       zoom.Scale,
                                       SpriteEffects.None,
                                       0.0f);
            }
        }

        public void Reset()
        {
            this.zoomTexts.Clear();
        }

        #endregion
    }
}
