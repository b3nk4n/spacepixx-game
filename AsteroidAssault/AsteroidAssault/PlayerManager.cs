using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;

namespace SpacepiXX
{
    class PlayerManager : ILevel
    {
        #region Members

        public Sprite playerSprite;
        private float playerSpeed = 250.0f;
        private Rectangle playerAreaLimit;

        private Vector2 startLocation;

        public long PlayerScore = 0;

        public int LivesReamining = 3;
        public int SpecialShotsRemaining = 5;

        private float shotPower = 50.0f;

        private float hitPoints = 100.0f;
        public const float MaxHitPoints = 100.0f;
        const float HealRate = 1.5f;

        private Vector2 gunOffset = new Vector2(25, 10);
        private float shotTimer = 0.0f;
        private float minShotTimer = 0.15f;
        private float minDoubleShotTimer = 0.25f;
        private float minTripleShotTimer = 0.35f;
        private float minSpecialShotTimer = 1.0f;
        
        private const int PlayerRadius = 20;
        public ShotManager PlayerShotManager;

        private float overheat = 0.0f;
        private const float OverheatSingleShot = 0.045f;
        private const float OverheatDoubleShot = 0.09f;
        private const float OverheatTripleShot = 0.135f;
        private const float OverheatSpecialShot = 0.25f;
        private const float OverheatMax = 1.0f;
        private const float CoolDownRate = 0.25f;
        private const float OverheatKillRateMax = 0.1f;

        Accelerometer accelerometer = new Accelerometer();
        Vector3 currentAccValue = Vector3.Zero;

        Rectangle leftSideScreen;
        Rectangle rightSideScreen;
        Rectangle upperScreen;

        #endregion

        #region Constructors

        public PlayerManager(Texture2D texture, Rectangle initialFrame,
                             int frameCount, Rectangle screenBounds,
                             Vector2 startLocation)
        {
            this.playerSprite = new Sprite(new Vector2(500, 500),
                                           texture,
                                           initialFrame,
                                           Vector2.Zero);

            this.PlayerShotManager = new ShotManager(texture,
                                                     new Rectangle(0, 430, 5, 5),
                                                     4,
                                                     2,
                                                     250.0f,
                                                     screenBounds);

            this.playerAreaLimit = new Rectangle(0,
                                                 screenBounds.Height / 3,
                                                 screenBounds.Width,
                                                 2 * screenBounds.Height / 3);

            for (int x = 0; x < frameCount; x++)
            {
                this.playerSprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                         initialFrame.Y,
                                                         initialFrame.Width,
                                                         initialFrame.Height));
            }

            playerSprite.CollisionRadius = PlayerRadius;

            accelerometer.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(accelerometer_ReadingChanged);
            accelerometer.Start();

            leftSideScreen = new Rectangle(0,
                                           screenBounds.Height / 2,
                                           screenBounds.Width / 2,
                                           screenBounds.Height / 2);

            rightSideScreen = new Rectangle(screenBounds.Width / 2,
                                           screenBounds.Height / 2,
                                           screenBounds.Width / 2,
                                           screenBounds.Height / 2);

            upperScreen = new Rectangle(0, 0,
                                        screenBounds.Width,
                                        screenBounds.Height / 3);

            this.startLocation = startLocation;
        }

        #endregion

        #region Methods

        public void Reset()
        {
            this.PlayerShotManager.Shots.Clear();

            this.playerSprite.Location = startLocation;

            this.HitPoints = MaxHitPoints;
            this.Overheat = 0.0f;
        }

        private void fireShot()
        {
            if (shotTimer <= 0.0f)
            {
                overheat += OverheatSingleShot;

                if (overheat < OverheatMax)
                {
                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2(0, -1),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);
                    shotTimer = minShotTimer;
                }
                else
                {
                    shotTimer = minShotTimer * 1.5f;
                }
            }
        }

        private void fireDoubleShot()
        {
            if (shotTimer <= 0.0f)
            {
                overheat += (OverheatDoubleShot);

                if (overheat < OverheatMax)
                {
                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-15, 0),
                                                    new Vector2(0, -1),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(15, 0),
                                                    new Vector2(0, -1),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                    shotTimer = minDoubleShotTimer;
                }
                else
                {
                    shotTimer = minDoubleShotTimer * 2.0f;
                }
            }
        }

        private void fireTripleShot()
        {
            if (shotTimer <= 0.0f)
            {
                overheat += (OverheatTripleShot);

                if (overheat < OverheatMax)
                {
                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2(0, -1),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(75.0f)), -(float)Math.Sin(MathHelper.ToRadians(75.0f))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(105.0f)), -(float)Math.Sin(MathHelper.ToRadians(105.0f))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    false);

                    

                    shotTimer = minTripleShotTimer;
                }
                else
                {
                    shotTimer = minTripleShotTimer * 2.0f;
                }
            }
        }

        private void fireSpecialShot()
        {
            if (shotTimer <= 0.0f &&
                SpecialShotsRemaining > 0 &&
                overheat < (OverheatMax - OverheatSpecialShot))
            {
                overheat += (OverheatTripleShot);
                SpecialShotsRemaining--;

                if (overheat < OverheatMax)
                {
                    // first wave:
                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2(0, -1),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(85.0f)), -(float)Math.Sin(MathHelper.ToRadians(85.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(95.0f)), -(float)Math.Sin(MathHelper.ToRadians(95.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(80.0f)), -(float)Math.Sin(MathHelper.ToRadians(80.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(100.0f)), -(float)Math.Sin(MathHelper.ToRadians(100.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(75.0f)), -(float)Math.Sin(MathHelper.ToRadians(75.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(105.0f)), -(float)Math.Sin(MathHelper.ToRadians(105.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(70.0f)), -(float)Math.Sin(MathHelper.ToRadians(70.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(110.0f)), -(float)Math.Sin(MathHelper.ToRadians(110.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(65.0f)), -(float)Math.Sin(MathHelper.ToRadians(65.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(115.0f)), -(float)Math.Sin(MathHelper.ToRadians(115.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(60.0f)), -(float)Math.Sin(MathHelper.ToRadians(60.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(120.0f)), -(float)Math.Sin(MathHelper.ToRadians(120.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(55.0f)), -(float)Math.Sin(MathHelper.ToRadians(55.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(125.0f)), -(float)Math.Sin(MathHelper.ToRadians(125.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    // second wave:
                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2(0, -1),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(85.0f)), -(float)Math.Sin(MathHelper.ToRadians(85.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(95.0f)), -(float)Math.Sin(MathHelper.ToRadians(95.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(80.0f)), -(float)Math.Sin(MathHelper.ToRadians(80.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(100.0f)), -(float)Math.Sin(MathHelper.ToRadians(100.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(75.0f)), -(float)Math.Sin(MathHelper.ToRadians(75.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(105.0f)), -(float)Math.Sin(MathHelper.ToRadians(105.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(70.0f)), -(float)Math.Sin(MathHelper.ToRadians(70.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(110.0f)), -(float)Math.Sin(MathHelper.ToRadians(110.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(65.0f)), -(float)Math.Sin(MathHelper.ToRadians(65.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(115.0f)), -(float)Math.Sin(MathHelper.ToRadians(115.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(60.0f)), -(float)Math.Sin(MathHelper.ToRadians(60.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(120.0f)), -(float)Math.Sin(MathHelper.ToRadians(120.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(55.0f)), -(float)Math.Sin(MathHelper.ToRadians(55.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(125.0f)), -(float)Math.Sin(MathHelper.ToRadians(125.0f))),
                                                    true,
                                                    new Color(75, 75, 255),
                                                    false);

                    shotTimer = minSpecialShotTimer;
                }
            }
        }

        private void HandleTouchInput(TouchCollection touches)
        {
            #if DEBUG

            Microsoft.Xna.Framework.Input.KeyboardState state = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1))
            {
                fireShot();
            }
            else if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D2))
            {
                fireDoubleShot();
            }
            else if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D3))
            {
                fireTripleShot();
            }
            else if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D4))
            {
                fireSpecialShot();
            }

            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                playerSprite.Velocity = new Vector2(-1.0f, 0.0f);
            } 
            else if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                playerSprite.Velocity = new Vector2(1.0f, 0.0f);
            }
            else if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                playerSprite.Velocity = new Vector2(0.0f, -1.0f);
            }
            else if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                playerSprite.Velocity = new Vector2(0.0f, 1.0f);
            }

            #else       

            if (touches.Count == 1)
            {
                TouchLocation touch = touches[0];
                if (leftSideScreen.Contains(new Point((int)touch.Position.X, (int)touch.Position.Y)))
                {
                    fireShot();
                }
                else if (rightSideScreen.Contains(new Point((int)touch.Position.X, (int)touch.Position.Y)))
                {
                    fireDoubleShot();
                }
                else if (upperScreen.Contains(new Point((int)touch.Position.X, (int)touch.Position.Y)))
                {
                    fireSpecialShot();
                }

            }
            else if (touches.Count == 2)
            {
                fireTripleShot();
            }


            // Movement (X = left/right, Y = up/down)

            currentAccValue.Y = MathHelper.Clamp(currentAccValue.Y, -0.4f, 0.4f);
            currentAccValue.X = MathHelper.Clamp(currentAccValue.X, -0.4f, 0.4f);

            playerSprite.Velocity = new Vector2(-currentAccValue.Y * 6,
                                                -currentAccValue.X * 3 - 0.6f);


            if (playerSprite.Velocity.Length() < 0.1f)
            {
                playerSprite.Velocity = Vector2.Zero;
            }

            #endif
        }

        private void adaptMovementLimits()
        {
            Vector2 location = playerSprite.Location;

            if (location.X < playerAreaLimit.X)
            {
                location.X = playerAreaLimit.X;
            }

            if (location.X > (playerAreaLimit.Right - playerSprite.Source.Width))
            {
                location.X = (playerAreaLimit.Right - playerSprite.Source.Width);
            }

            if (location.Y < playerAreaLimit.Y)
            {
                location.Y = playerAreaLimit.Y;
            }

            if (location.Y > (playerAreaLimit.Bottom - playerSprite.Source.Height))
            {
                location.Y = (playerAreaLimit.Bottom - playerSprite.Source.Height);
            }

            playerSprite.Location = location;
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            PlayerShotManager.Update(gameTime);

            if (!IsDestroyed)
            {
                playerSprite.Velocity = Vector2.Zero;

                shotTimer -= elapsed;

                HandleTouchInput(TouchPanel.GetState());

                if (playerSprite.Velocity.Length() != 0.0f)
                {
                    playerSprite.Velocity.Normalize();
                }
                
                playerSprite.Velocity *= playerSpeed;

                playerSprite.Update(gameTime);
                adaptMovementLimits();

                Overheat -= CoolDownRate * elapsed;

                if (HitPoints != 0.0f)
                {
                    HitPoints += HealRate * elapsed;
                }

                if (this.overheat > 0.75f)
                {
                    this.HitPoints -= (OverheatKillRateMax * this.overheat);
                    this.hitPoints = Math.Max(1.0f, this.hitPoints);
                }

                float factor = (float)Math.Max((this.hitPoints / 100.0f), 0.66f);
                this.playerSprite.TintColor = new Color(factor, factor, factor);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);

            if (!IsDestroyed)
            {
                playerSprite.Draw(spriteBatch);
            }
        }

        public void SetLevel(int lvl)
        {
            this.SpecialShotsRemaining += 1;
        }

        #endregion

        #region Properties

        public float Overheat
        {
            get
            {
                return this.overheat;
            }
            set
            {
                this.overheat = Math.Max(value, 0.0f);
            }
        }

        public float HitPoints
        {
            get
            {
                return this.hitPoints;
            }
            set
            {
                this.hitPoints = MathHelper.Clamp(value, 0.0f, MaxHitPoints);
            }
        }

        public bool IsDestroyed
        {
            get
            {
                return this.hitPoints <= 0.0f;
            }
        }

        public float ShotPower
        {
            get
            {
                return this.shotPower;
            }
        }

        #endregion

        #region Events

        void accelerometer_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            currentAccValue = new Vector3((float)e.X,
                                          (float)e.Y,
                                          (float)e.Z);
        }

        #endregion
    }
}
