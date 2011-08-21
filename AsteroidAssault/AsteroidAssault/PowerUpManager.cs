using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpacepiXX
{
    class PowerUpManager
    {
        #region Members

        private List<PowerUp> powerUps = new List<PowerUp>();
        private Texture2D texture;

        private const int SPAWN_CHANCE = 10; // 10%

        private int lastPowerUpNumber = -1;

        private Random rand = new Random();

        #endregion

        #region Constructors

        public PowerUpManager(Texture2D texture)
        {
            this.texture = texture;
        }

        #endregion

        #region Methods

        public void ProbablySpawnPowerUp(Vector2 location)
        {
            int spawnChance = rand.Next(100);
            
            if (spawnChance >= SPAWN_CHANCE)
                return;

            int rnd = rand.Next(59);

            // check, that the new powerup to drop is not equal
            // to the last one
            if (rnd == lastPowerUpNumber)
                return;
            else
                lastPowerUpNumber = rnd;

            PowerUp.PowerUpType type = PowerUp.PowerUpType.SpecialShot;
            Rectangle initialFrame = new Rectangle(0, 0, 25, 25);

            switch(rnd)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    type = PowerUp.PowerUpType.Health25;
                    initialFrame = new Rectangle(0, 0, 25, 25);
                    break;

                case 4:
                case 5:
                    type = PowerUp.PowerUpType.Health50;
                    initialFrame = new Rectangle(0, 25, 25, 25);
                    break;

                case 6:
                    type = PowerUp.PowerUpType.Health100;
                    initialFrame = new Rectangle(0, 50, 25, 25);
                    break;

                case 7:
                    type = PowerUp.PowerUpType.Life;
                    initialFrame = new Rectangle(0, 75, 25, 25);
                    break;

                case 8:
                case 9:
                    type = PowerUp.PowerUpType.SpecialShot;
                    initialFrame = new Rectangle(0, 100, 25, 25);
                    break;

                case 10:
                    type = PowerUp.PowerUpType.KillAll;
                    initialFrame = new Rectangle(0, 125, 25, 25);
                    break;

                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    type = PowerUp.PowerUpType.LowBonusScore;
                    initialFrame = new Rectangle(0, 150, 25, 25);
                    break;

                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                    type = PowerUp.PowerUpType.MediumBonusScore;
                    initialFrame = new Rectangle(0, 175, 25, 25);
                    break;

                case 26:
                case 27:
                case 28:
                case 29:
                    type = PowerUp.PowerUpType.HighBonusScore;
                    initialFrame = new Rectangle(0, 200, 25, 25);
                    break;

                case 30:
                case 31:
                case 32:
                    type = PowerUp.PowerUpType.CoolWater;
                    initialFrame = new Rectangle(0, 225, 25, 25);
                    break;

                case 33:
                case 34:
                    type = PowerUp.PowerUpType.BonusRockets;
                    initialFrame = new Rectangle(0, 250, 25, 25);
                    break;

                case 35:
                case 36:
                case 37:
                    type = PowerUp.PowerUpType.Shield;
                    initialFrame = new Rectangle(0, 275, 25, 25);
                    break;

                case 38:
                case 39:
                case 40:
                    type = PowerUp.PowerUpType.ScoreMultiLow;
                    initialFrame = new Rectangle(0, 300, 25, 25);
                    break;

                case 41:
                case 42:
                    type = PowerUp.PowerUpType.ScoreMultiMedium;
                    initialFrame = new Rectangle(0, 325, 25, 25);
                    break;

                case 43:
                    type = PowerUp.PowerUpType.ScoreMultiHigh;
                    initialFrame = new Rectangle(0, 350, 25, 25);
                    break;

                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                    type = PowerUp.PowerUpType.Random;
                    initialFrame = new Rectangle(0, 375, 25, 25);
                    break;

                case 49:
                case 50:
                case 51:
                    type = PowerUp.PowerUpType.OverHeat;
                    initialFrame = new Rectangle(0, 400, 25, 25);
                    break;

                case 52:
                case 53:
                    type = PowerUp.PowerUpType.AntiScoreMulti;
                    initialFrame = new Rectangle(0, 425, 25, 25);
                    break;

                case 54:
                case 55:
                    type = PowerUp.PowerUpType.OutOfControl;
                    initialFrame = new Rectangle(0, 450, 25, 25);
                    break;

                case 56:
                case 57:
                case 58:
                    type = PowerUp.PowerUpType.Slow;
                    initialFrame = new Rectangle(0, 475, 25, 25);
                    break;
            }

            PowerUp p = new PowerUp(texture,
                                    location - new Vector2(12.5f, 12.5f),
                                    initialFrame,
                                    1,
                                    type);

            powerUps.Add(p);
        }

        public PowerUp.PowerUpType GetPowerUpNotRandom()
        {
            int rnd = rand.Next(19);
            PowerUp.PowerUpType type = PowerUp.PowerUpType.ScoreMultiLow;

            switch (rnd)
            {
                case 0:
                    type =  PowerUp.PowerUpType.Health25;
                    break;

                case 1:
                    type = PowerUp.PowerUpType.Health50;
                    break;

                case 2:
                    type = PowerUp.PowerUpType.Health100;
                    break;

                case 3:
                    type = PowerUp.PowerUpType.ScoreMultiLow;
                    break;

                case 4:
                    type = PowerUp.PowerUpType.ScoreMultiMedium;
                    break;

                case 5:
                    type = PowerUp.PowerUpType.ScoreMultiHigh;
                    break;

                case 6:
                    type = PowerUp.PowerUpType.CoolWater;
                    break;

                case 7:
                    type = PowerUp.PowerUpType.Life;
                    break;

                case 8:
                    type = PowerUp.PowerUpType.KillAll;
                    break;

                case 9:
                    type = PowerUp.PowerUpType.LowBonusScore;
                    break;

                case 10:
                    type = PowerUp.PowerUpType.MediumBonusScore;
                    break;

                case 11:
                    type = PowerUp.PowerUpType.HighBonusScore;
                    break;

                case 12:
                    type = PowerUp.PowerUpType.OutOfControl;
                    break;

                case 13:
                    type = PowerUp.PowerUpType.OverHeat;
                    break;

                case 14:
                    type = PowerUp.PowerUpType.SpecialShot;
                    break;

                case 15:
                    type = PowerUp.PowerUpType.AntiScoreMulti;
                    break;

                case 16:
                    type = PowerUp.PowerUpType.Slow;
                    break;

                case 17:
                    type = PowerUp.PowerUpType.BonusRockets;
                    break;

                case 18:
                    type = PowerUp.PowerUpType.Shield;
                    break;
            }

            return type;
        }

        public void Reset()
        {
            this.PowerUps.Clear();
            this.lastPowerUpNumber = -1;
        }

        public void Update(GameTime gameTime)
        {
            for (int x = powerUps.Count - 1; x >= 0; --x)
            {
                powerUps[x].Update(gameTime);

                if (!powerUps[x].IsActive)
                {
                    powerUps.RemoveAt(x);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var powerUp in powerUps)
            {
                powerUp.Draw(spriteBatch);
            }
        }

        private Rectangle getInitialFrameByType(PowerUp.PowerUpType type)
        {
            switch (type)
            {
                case PowerUp.PowerUpType.Health25:
                    return new Rectangle(0, 0, 25, 25);
                case PowerUp.PowerUpType.Health50:
                    return new Rectangle(0, 25, 25, 25);
                case PowerUp.PowerUpType.Health100:
                    return new Rectangle(0, 50, 25, 25);
                case PowerUp.PowerUpType.CoolWater:
                    return new Rectangle(0, 225, 25, 25);
                case PowerUp.PowerUpType.Life:
                    return new Rectangle(0, 75, 25, 25);
                case PowerUp.PowerUpType.SpecialShot:
                    return new Rectangle(0, 100, 25, 25);
                case PowerUp.PowerUpType.KillAll:
                    return new Rectangle(0, 125, 25, 25);
                case PowerUp.PowerUpType.LowBonusScore:
                    return new Rectangle(0, 150, 25, 25);
                case PowerUp.PowerUpType.MediumBonusScore:
                    return new Rectangle(0, 175, 25, 25);
                case PowerUp.PowerUpType.HighBonusScore:
                    return new Rectangle(0, 200, 25, 25);
                case PowerUp.PowerUpType.ScoreMultiLow:
                    return new Rectangle(0, 300, 25, 25);
                case PowerUp.PowerUpType.ScoreMultiMedium:
                    return new Rectangle(0, 325, 25, 25);
                case PowerUp.PowerUpType.ScoreMultiHigh:
                    return new Rectangle(0, 350, 25, 25);
                case PowerUp.PowerUpType.BonusRockets:
                    return new Rectangle(0, 250, 25, 25);
                case PowerUp.PowerUpType.Shield:
                    return new Rectangle(0, 275, 25, 25);
                case PowerUp.PowerUpType.AntiScoreMulti:
                    return new Rectangle(0, 425, 25, 25);
                case PowerUp.PowerUpType.OutOfControl:
                    return new Rectangle(0, 450, 25, 25);
                case PowerUp.PowerUpType.Slow:
                    return new Rectangle(0, 475, 25, 25);
                case PowerUp.PowerUpType.OverHeat:
                    return new Rectangle(0, 400, 25, 25);
                case PowerUp.PowerUpType.Random:
                    return new Rectangle(0, 375, 25, 25);
                default:
                    return new Rectangle(0, 0, 25, 25);
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Powerups
            int powerUpsCount = Int32.Parse(reader.ReadLine());
            
            powerUps.Clear();

            for (int i = 0; i < powerUpsCount; ++i)
            {
                PowerUp.PowerUpType type = PowerUp.PowerUpType.Random;
                type = (PowerUp.PowerUpType)Enum.Parse(type.GetType(), reader.ReadLine(), false);
                PowerUp p = new PowerUp(texture,
                                    Vector2.Zero,
                                    getInitialFrameByType(type),
                                    1,
                                    type);
                p.Activated(reader);
                powerUps.Add(p);
            }

            this.lastPowerUpNumber = Int32.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Powerups
            writer.WriteLine(powerUps.Count);

            for (int i = 0; i < powerUps.Count; ++i)
            {
                writer.WriteLine(powerUps[i].Type);
                powerUps[i].Deactivated(writer);
            }

            writer.WriteLine(lastPowerUpNumber);
        }

        #endregion

        #region Properties

        public List<PowerUp> PowerUps
        {
            get
            {
                return powerUps;
            }
        }

        #endregion
    }
}
