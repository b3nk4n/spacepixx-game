using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace SpacepiXX
{
    class PlayerManager : ILevel
    {
        #region Members

        public Sprite playerSprite;
        public const float PLAYER_SPEED = 225.0f;
        private Rectangle playerAreaLimit;

        private Vector2 startLocation;

        private long playerScore = 0;

        private const int PLAYER_STARTING_LIVES = 2;
        public const int MAX_PLAYER_LIVES = 5;
        private int livesRemaining = PLAYER_STARTING_LIVES;
        public int SpecialShotsRemaining = 5;

        public int CarliRocketsRemaining = 10;
        public const int CARLI_ROCKET_EXPLOSION_RADIUS = 150;

        private float shotPower = 50.0f;
        public const float ROCKET_POWER_AT_CENTER = 500.0f;

        private float hitPoints = 100.0f;
        public const float MaxHitPoints = 100.0f;
        const float HealRate = 1.0f; // Version 1.0:  1.5

        private Vector2 gunOffset = new Vector2(19, 5);
        private float shotTimer = 0.0f;
        private float rocketTimer = 0.0f;
        private float specialShotTimer = 0.0f;
        private float minShotTimer = 0.15f;
        private float minDoubleShotTimer = 0.25f;
        private float minTripleShotTimer = 0.35f;
        private float minSpecialShotTimer = 1.0f;
        private float minCarliRocketTimer = 0.5f;
        
        private const int PlayerRadius = 20;
        public ShotManager PlayerShotManager;

        private float overheat = 0.0f;
        private const float OverheatSingleShot = 0.045f;
        private const float OverheatDoubleShot = 0.09f;
        private const float OverheatTripleShot = 0.135f;
        private const float OverheatSpecialShot = 0.25f;
        private const float OverheatCarliRocket = 0.05f;
        public const float OVERHEAT_MAX = 1.0f;
        public const float OVERHEAT_MIN = 0.0f;
        private const float CoolDownRate = 0.266f; // Version 1.0:  0.25
        private const float OverheatKillRateMax = 0.1f;

        Accelerometer accelerometer = new Accelerometer();
        Vector3 currentAccValue = Vector3.Zero;

        Rectangle leftSideScreen;
        Rectangle rightSideScreen;
        Rectangle upperRightScreen;
        Rectangle upperLeftScreen;

        public const float MIN_SCORE_MULTI = 1.0f;
        private float scoreMulti = MIN_SCORE_MULTI;
        public const float MAX_SCORE_MULTI = 3.0f;
        private const float MULTI_DECREASE_RATE = 0.085f;

        // OutOfControl
        private float outOfControlTimer = 0.0f;
        public const float OUT_OF_CONTROL_DURATION = 5.0f;
        private bool isOutOfControl = false;

        // Slow
        private float slowTimer = 0.0f;
        public const float SLOW_DURATION = 10.0f;
        private bool isSlow = false;
        public const float SLOW_SPEED = 100.0f;

        // Shield
        private float shieldPoints = 0.0f;
        private const float SHIELD_POINTS_MAX = 100.0f;
        private const float SHIELD_DECREASE_RATE = 2.5f; // 40 sec

        private Sprite shieldSprite;
        private readonly Rectangle initialShieldFrame = new Rectangle(250, 400, 70, 70);

        SettingsManager settings = SettingsManager.GetInstance();

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
                                                     new Rectangle(100, 430, 15, 5),
                                                     4,
                                                     2,
                                                     300.0f,
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

            upperLeftScreen = new Rectangle(0, 0,
                                            screenBounds.Width / 2,
                                            screenBounds.Height / 3);

            upperRightScreen = new Rectangle(screenBounds.Width / 2,
                                             0,
                                             screenBounds.Width / 2,
                                             screenBounds.Height / 3);

            this.startLocation = startLocation;

            this.shieldSprite = new Sprite(Vector2.Zero,
                                           texture,
                                           initialShieldFrame,
                                           Vector2.Zero);

            //for (int x = 1; x < 12; x++)
            //{
            //    this.shieldSprite.AddFrame(new Rectangle(initialShieldFrame.X + (x * initialShieldFrame.Width),
            //                                             initialShieldFrame.Y,
            //                                             initialShieldFrame.Width,
            //                                             initialShieldFrame.Height));
            //}
        }

        #endregion

        #region Methods

        public void Reset()
        {
            this.PlayerShotManager.Shots.Clear();
            this.PlayerShotManager.Rockets.Clear();

            this.playerSprite.Location = startLocation;

            this.hitPoints = MaxHitPoints;
            this.shieldPoints = 0.0f;
            this.Overheat = PlayerManager.OVERHEAT_MIN;
            this.ScoreMulti = MIN_SCORE_MULTI;

            this.isOutOfControl = false;
            this.outOfControlTimer = 0.0f;
        }

        public void ResetSpecialWeapons()
        {
            this.CarliRocketsRemaining = 5;
            this.SpecialShotsRemaining = 3;
        }

        public void ResetRemainingLives()
        {
            this.LivesRemaining = PLAYER_STARTING_LIVES;
        }

        private void fireShot()
        {
            if (shotTimer <= 0.0f)
            {
                overheat += OverheatSingleShot;

                if (overheat < OVERHEAT_MAX)
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

                if (overheat < OVERHEAT_MAX)
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

                if (overheat < OVERHEAT_MAX)
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
            if (specialShotTimer <= 0.0f &&
                SpecialShotsRemaining > 0 &&
                overheat < (OVERHEAT_MAX - OverheatSpecialShot))
            {
                overheat += (OverheatTripleShot);
                SpecialShotsRemaining--;

                if (overheat < OVERHEAT_MAX)
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

                    specialShotTimer = minSpecialShotTimer;
                }
            }
        }

        private void fireCarliRocket()
        {
            if (rocketTimer <= 0.0f &&
                CarliRocketsRemaining > 0 &&
                overheat < (OVERHEAT_MAX - OverheatCarliRocket))
            {
                overheat += (OverheatCarliRocket);

                if (overheat < OVERHEAT_MAX)
                {
                    CarliRocketsRemaining--;

                    this.PlayerShotManager.FireRocket(this.playerSprite.Location + gunOffset,
                                                           new Vector2(0, -1),
                                                           true,
                                                           Color.White,
                                                           true);

                    rocketTimer = minCarliRocketTimer;
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
            else if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D5))
            {
                fireCarliRocket();
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

            //if (touches.Count == 1)
            //{
            //    TouchLocation touch = touches[0];
            //    if (leftSideScreen.Contains(new Point((int)touch.Position.X, (int)touch.Position.Y)))
            //    {
            //        fireShot();
            //    }
            //    else if (rightSideScreen.Contains(new Point((int)touch.Position.X, (int)touch.Position.Y)))
            //    {
            //        fireDoubleShot();
            //    }
            //    else if (upperLeftScreen.Contains(new Point((int)touch.Position.X, (int)touch.Position.Y)))
            //    {
            //        fireCarliRocket();
            //    }
            //    else if (upperRightScreen.Contains(new Point((int)touch.Position.X, (int)touch.Position.Y)))
            //    {
            //        fireSpecialShot();
            //    }

            //}
            //else if (touches.Count == 2)
            //{
            //    fireTripleShot();
            //}

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
                else if (upperLeftScreen.Contains(new Point((int)touch.Position.X, (int)touch.Position.Y)))
                {
                    fireCarliRocket();
                }
                else if (upperRightScreen.Contains(new Point((int)touch.Position.X, (int)touch.Position.Y)))
                {
                    fireSpecialShot();
                }

            }
            else if (touches.Count == 2)
            {
                if ((leftSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                    rightSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                    (leftSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                    rightSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                {
                    fireTripleShot();
                }
                else if ((leftSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                         upperLeftScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                        (leftSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                         upperLeftScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                {
                    fireShot();
                    fireCarliRocket();
                }
                else if ((rightSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                         upperLeftScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                        (rightSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                         upperLeftScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                {
                    fireDoubleShot();
                    fireCarliRocket();
                }
                else if ((leftSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                         upperRightScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                        (leftSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                         upperRightScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                {
                    fireSpecialShot();
                }
                else if ((rightSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                         upperRightScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                        (rightSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                         upperRightScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                {
                    fireSpecialShot();
                }
            }


            // ***** up to version 1.1 *****
            // Movement (X = left/right, Y = up/down)
            //currentAccValue.Y = MathHelper.Clamp(currentAccValue.Y, -0.4f, 0.4f);
            //currentAccValue.X = MathHelper.Clamp(currentAccValue.X, -0.4f, 0.4f);

            //if (!IsOutOfControl)
            //    playerSprite.Velocity = new Vector2(-currentAccValue.Y * 6,
            //                                        -currentAccValue.X * 3 - 0.6f);
            //else
            //    playerSprite.Velocity = new Vector2(currentAccValue.Y * 6,
            //                                        -currentAccValue.X * 3 - 0.6f);

            // ***** new in version 1.2 *****
            currentAccValue.X = currentAccValue.X + (float)Math.Sin(settings.GetNeutralPosition());

            currentAccValue.Y = MathHelper.Clamp(currentAccValue.Y, -0.4f, 0.4f);
            currentAccValue.X = MathHelper.Clamp(currentAccValue.X, -0.4f, 0.4f);

            if (!IsOutOfControl)
                playerSprite.Velocity = new Vector2(-currentAccValue.Y * 6,
                                                    -currentAccValue.X * 3);
            else
                playerSprite.Velocity = new Vector2(currentAccValue.Y * 6,
                                                    -currentAccValue.X * 3);

            if (playerSprite.Velocity.Length() < 0.1f)
            {
                playerSprite.Velocity = Vector2.Zero;
            }

            #endif
        }

        private void HandleGamepadInput(GamePadState gamePadState)
        {
            #if DEBUG
            playerSprite.Velocity +=
                new Vector2(
                    gamePadState.ThumbSticks.Left.X,
                    -gamePadState.ThumbSticks.Left.Y);

            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                fireShot();
            }

            if (gamePadState.Buttons.X == ButtonState.Pressed)
            {
                fireDoubleShot();
            }

            if (gamePadState.Buttons.B == ButtonState.Pressed)
            {
                fireTripleShot();
            }

            if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
            {
                fireSpecialShot();
            }

            if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                fireCarliRocket();
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
                specialShotTimer -= elapsed;
                rocketTimer -= elapsed;

                HandleTouchInput(TouchPanel.GetState());
                HandleGamepadInput(GamePad.GetState(PlayerIndex.One));

                if (playerSprite.Velocity.Length() != 0.0f)
                {
                    playerSprite.Velocity.Normalize();
                }
                
                if (!IsSlow)
                    playerSprite.Velocity *= PLAYER_SPEED;
                else
                    playerSprite.Velocity *= SLOW_SPEED;

                playerSprite.Update(gameTime);
                adaptMovementLimits();

                Overheat -= CoolDownRate * elapsed;

                if (HitPoints != 0.0f)
                {
                    this.IncreaseHitPoints(HealRate * elapsed);
                }

                if (this.overheat > 0.75f)
                {
                    this.DecreaseHitPoints(OverheatKillRateMax * this.overheat);
                    this.hitPoints = Math.Max(1.0f, this.hitPoints);
                }

                float factor = (float)Math.Max((this.hitPoints / 100.0f), 0.75f);
                this.playerSprite.TintColor = new Color(factor, factor, factor);

                // Multi
                ScoreMulti -= MULTI_DECREASE_RATE * elapsed;

                // Out of control
                if (IsOutOfControl)
                {
                    outOfControlTimer -= elapsed;

                    if (outOfControlTimer <= 0.0f)
                    {
                        isOutOfControl = false;
                    }
                }

                // Slow
                if (IsSlow)
                {
                    slowTimer -= elapsed;

                    if (slowTimer <= 0.0f)
                    {
                        isSlow = false;
                    }
                }

                // Shield
                if (IsShieldActive)
                {
                    shieldPoints -= elapsed * SHIELD_DECREASE_RATE;
                    shieldPoints = Math.Max(0, this.shieldPoints);

                    shieldSprite.Update(gameTime);
                    shieldSprite.Rotation += (float)Math.PI / 25;
                    shieldSprite.Rotation = (float)Math.Max(shieldSprite.Rotation, Math.PI * 2);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);

            if (!IsDestroyed)
            {
                playerSprite.Draw(spriteBatch);

                if (IsShieldActive)
                {
                    shieldSprite.TintColor = Color.Blue * (0.4f + 0.15f * shieldPoints / SHIELD_POINTS_MAX);
                    shieldSprite.Location = playerSprite.Location - new Vector2(10, 10);
                    shieldSprite.Draw(spriteBatch);
                }
            }
        }

        public void SetLevel(int lvl)
        {
            if (lvl != 1)
            {
                this.SpecialShotsRemaining += 1;
                this.CarliRocketsRemaining += 2;
            }
        }

        public void StartOutOfControl()
        {
            this.outOfControlTimer = OUT_OF_CONTROL_DURATION;
            this.isOutOfControl = true;
        }

        public void StartSlow()
        {
            this.slowTimer = SLOW_DURATION;
            this.isSlow = true;
        }

        public void ActivateShield()
        {
            this.shieldPoints = SHIELD_POINTS_MAX;
        }

        public void IncreasePlayerScore(long score, bool multi)
        {
            if (multi)
                this.playerScore += (long)(score * this.scoreMulti);
            else
                this.playerScore += score;
        }

        public void SetHitPoints(float hp)
        {
            this.hitPoints = MathHelper.Clamp(hp, 0.0f, MaxHitPoints);
        }

        public void IncreaseHitPoints(float hp)
        {
            if (hp < 0)
                throw new ArgumentException("Negative values are not allowed.");

            this.hitPoints += hp;
            this.hitPoints = MathHelper.Clamp(hitPoints, 0.0f, MaxHitPoints);
        }

        public void DecreaseHitPoints(float hp)
        {
            if (hp < 0)
                throw new ArgumentException("Positive values are not allowed.");

            float diff = Math.Max(0, hp - this.shieldPoints);

            this.shieldPoints -= hp;
            this.shieldPoints = Math.Max(0, shieldPoints);

            this.hitPoints -= diff;
            this.hitPoints = MathHelper.Clamp(hitPoints, 0.0f, MaxHitPoints);
        }

        public void IncreaseScoreMulti(long score, bool kill)
        {
            if (kill)
                ScoreMulti += score / 3000.0f;
            else
                ScoreMulti += score / 3500.0f;
        }

        public void ResetPlayerScore()
        {
            this.playerScore = 0;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            //Player sprite
            //this.playerSprite.Location = new Vector2(Single.Parse(reader.ReadLine()),
            //                                         Single.Parse(reader.ReadLine()));
            //this.playerSprite.Rotation = Single.Parse(reader.ReadLine());
            //this.playerSprite.Velocity = new Vector2(Single.Parse(reader.ReadLine()),
            //                                         Single.Parse(reader.ReadLine()));
            playerSprite.Activated(reader);

            this.playerScore = Int64.Parse(reader.ReadLine());

            this.livesRemaining = Int32.Parse(reader.ReadLine());
            this.SpecialShotsRemaining = Int32.Parse(reader.ReadLine());
            this.CarliRocketsRemaining = Int32.Parse(reader.ReadLine());

            this.shotPower = Single.Parse(reader.ReadLine());
            this.hitPoints = Single.Parse(reader.ReadLine());

            this.shotTimer = Single.Parse(reader.ReadLine());
            this.rocketTimer = Single.Parse(reader.ReadLine());
            this.specialShotTimer = Single.Parse(reader.ReadLine());

            PlayerShotManager.Activated(reader);

            this.overheat = Single.Parse(reader.ReadLine());

            this.scoreMulti = Single.Parse(reader.ReadLine());

            this.outOfControlTimer = Single.Parse(reader.ReadLine());
            this.isOutOfControl = Boolean.Parse(reader.ReadLine());

            this.slowTimer = Single.Parse(reader.ReadLine());
            this.isSlow = Boolean.Parse(reader.ReadLine());

            this.shieldPoints = Single.Parse(reader.ReadLine());

            //Shield sprite
            this.shieldSprite.Location = new Vector2(Single.Parse(reader.ReadLine()),
                                                     Single.Parse(reader.ReadLine()));
            this.shieldSprite.Rotation = Single.Parse(reader.ReadLine());
            this.shieldSprite.Velocity = new Vector2(Single.Parse(reader.ReadLine()),
                                                     Single.Parse(reader.ReadLine()));
        }

        public void Deactivated(StreamWriter writer)
        {
            // Player sprite
            //writer.WriteLine(playerSprite.Location.X);
            //writer.WriteLine(playerSprite.Location.Y);
            //writer.WriteLine(playerSprite.Rotation);
            //writer.WriteLine(playerSprite.Velocity.X);
            //writer.WriteLine(playerSprite.Velocity.Y);
            playerSprite.Deactivated(writer);

            writer.WriteLine(this.playerScore);

            writer.WriteLine(this.livesRemaining);
            writer.WriteLine(this.SpecialShotsRemaining);
            writer.WriteLine(this.CarliRocketsRemaining);

            writer.WriteLine(this.shotPower);
            writer.WriteLine(this.hitPoints);

            writer.WriteLine(this.shotTimer);
            writer.WriteLine(this.rocketTimer);
            writer.WriteLine(this.specialShotTimer);

            PlayerShotManager.Deactivated(writer);

            writer.WriteLine(this.overheat);

            writer.WriteLine(this.scoreMulti);

            writer.WriteLine(this.outOfControlTimer);
            writer.WriteLine(this.isOutOfControl);

            writer.WriteLine(this.slowTimer);
            writer.WriteLine(this.isSlow);

            writer.WriteLine(this.shieldPoints);

            // Shield sprite
            writer.WriteLine(shieldSprite.Location.X);
            writer.WriteLine(shieldSprite.Location.Y);
            writer.WriteLine(shieldSprite.Rotation);
            writer.WriteLine(shieldSprite.Velocity.X);
            writer.WriteLine(shieldSprite.Velocity.Y);
        }

        #endregion

        #region Properties

        public int LivesRemaining
        {
            get
            {
                return this.livesRemaining;
            }
            set
            {
                this.livesRemaining = (int)MathHelper.Clamp(value, -1, MAX_PLAYER_LIVES);
            }
        }

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
            //set
            //{
            //    this.hitPoints = MathHelper.Clamp(value, 0.0f, MaxHitPoints);
            //}
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

        public long PlayerScore
        {
            get
            {
                return this.playerScore;
            }
        }

        public float ScoreMulti
        {
            get
            {
                return this.scoreMulti;
            }
            set
            {
                this.scoreMulti = MathHelper.Clamp(value, MIN_SCORE_MULTI, MAX_SCORE_MULTI);
            }
        }

        public bool IsOutOfControl
        {
            get
            {
                return this.isOutOfControl;
            }
        }

        public bool IsSlow
        {
            get
            {
                return this.isSlow;
            }
        }

        public float ShieldPoints
        {
            get
            {
                return this.shieldPoints;
            }
        }

        public bool IsShieldActive
        {
            get
            {
                return this.shieldPoints > 0.0f;
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
