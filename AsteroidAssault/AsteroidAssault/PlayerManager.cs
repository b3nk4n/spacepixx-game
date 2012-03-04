using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using System.IO;
using SpacepiXX.Inputs;
using Microsoft.Phone.Applications.Common;

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
        
        // *** Before version 2.0 ***
        //public const float ROCKET_POWER_AT_CENTER = 500.0f;
        public const float ROCKET_POWER_AT_CENTER = 300.0f;

        private float hitPoints = 100.0f;
        public const float MaxHitPoints = 100.0f;
        const float HealRate = 0.5f; // Version 1.0:  1.5    Version 1.4: 1.0

        private Vector2 gunOffset = new Vector2(19, 5);
        private float shotTimer = 0.0f;
        private float sideShotTimer = 0.0f;
        private float rocketTimer = 0.0f;
        private float specialShotTimer = 0.0f;
        private float minShotTimer = 0.15f;
        private float minDoubleShotTimer = 0.25f;
        private float minTripleShotTimer = 0.35f;
        private float minSideShotTimer = 0.30f;
        private float minSpecialShotTimer = 0.5f;
        private float minCarliRocketTimer = 0.5f;
        
        private const int PlayerRadius = 20;
        public ShotManager PlayerShotManager;

        private float overheat = 0.0f;

        // Version 2.4
        //private const float OverheatSingleShot = 0.08025f;
        //private const float OverheatDoubleShot = 0.1295f;
        //private const float OverheatTripleShot = 0.1795f;
        //private const float OverheatSideShot = 0.18f;
        //private const float OverheatSpecialShot = 0.25f;
        //private const float OverheatCarliRocket = 0.05f;

        // Version 2.5 (had to be adjusted because of the 60 fps change)
        private const float OverheatSingleShot = 0.0725f;
        private const float OverheatDoubleShot = 0.12225f;
        private const float OverheatTripleShot = 0.172f;
        private const float OverheatSideShot = 0.18f;
        private const float OverheatSpecialShot = 0.25f;
        private const float OverheatCarliRocket = 0.05f;

        public const float OVERHEAT_MAX = 1.0f;
        public const float OVERHEAT_MIN = 0.0f;
        private const float CoolDownRate = 0.45f; // Version 1.0:  0.25   Before version 2.0: 0.266
        private const float OverheatKillRateMax = 0.075f;

        Vector3 currentAccValue = Vector3.Zero;

        Rectangle leftSideScreen;
        Rectangle rightSideScreen;
        Rectangle middleScreen;
        Rectangle upperRightScreen;
        Rectangle upperLeftScreen;

        public const float MIN_SCORE_MULTI = 1.0f;
        private float scoreMulti = MIN_SCORE_MULTI;
        public const float MAX_SCORE_MULTI = 3.0f;
        private const float MULTI_DECREASE_RATE = 0.09f; // before version 2.0: 0.85

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

        GameInput gameInput;
        private const string ActionLeft = "Left";
        private const string ActionRight = "Right";
        private const string ActionUpperLeft = "UpperLeft";
        private const string ActionUpperRight = "UpperRight";
        private const string ActionSlideLeft = "SlideLeft";
        private const string ActionSlideRight = "SlideRight";

        // Over/Underdrive
        private float overdriveTimer = 0.0f;
        public const float OVERDRIVE_DURATION = 20.0f;
        private float overdriveFactor = OVERDRIVE_NORMAL_FACTOR;
        public const float OVERDRIVE_FACTOR = 1.75f;
        public const float OVERDRIVE_NORMAL_FACTOR = 1.0f;
        public const float UNDERDRIVE_FACTOR = 0.70f;

        #endregion

        #region Constructors

        public PlayerManager(Texture2D texture, Rectangle initialFrame,
                             int frameCount, Rectangle screenBounds,
                             Vector2 startLocation, GameInput input)
        {
            this.playerSprite = new Sprite(new Vector2(500, 500),
                                           texture,
                                           initialFrame,
                                           Vector2.Zero);

            this.PlayerShotManager = new ShotManager(texture,
                                                     new Rectangle(100, 430, 15, 5),
                                                     4,
                                                     2,
                                                     350.0f,
                                                     screenBounds);

            this.playerAreaLimit = new Rectangle(0,
                                                 screenBounds.Height / 5,
                                                 screenBounds.Width,
                                                 4 * screenBounds.Height / 5);

            for (int x = 0; x < frameCount; x++)
            {
                this.playerSprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                         initialFrame.Y,
                                                         initialFrame.Width,
                                                         initialFrame.Height));
            }

            playerSprite.CollisionRadius = PlayerRadius;

            AccelerometerHelper.Instance.ReadingChanged += new EventHandler<AccelerometerHelperReadingEventArgs>(OnAccelerometerHelperReadingChanged);
            AccelerometerHelper.Instance.Active = true;

            leftSideScreen = new Rectangle(0,
                                           2 * screenBounds.Height / 3,
                                           screenBounds.Width / 2,
                                           screenBounds.Height / 3);

            rightSideScreen = new Rectangle(screenBounds.Width / 2,
                                           2 * screenBounds.Height / 3,
                                           screenBounds.Width / 2,
                                           screenBounds.Height / 3);

            middleScreen = new Rectangle(0,
                                              screenBounds.Height / 3,
                                              screenBounds.Width,
                                              screenBounds.Height / 3);

            upperLeftScreen = new Rectangle(0, 0,
                                            screenBounds.Width / 2,
                                            screenBounds.Height / 4);

            upperRightScreen = new Rectangle(screenBounds.Width / 2,
                                             0,
                                             screenBounds.Width / 2,
                                             screenBounds.Height / 4);

            this.startLocation = startLocation;

            this.shieldSprite = new Sprite(Vector2.Zero,
                                           texture,
                                           initialShieldFrame,
                                           Vector2.Zero);

            gameInput = input;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            gameInput.AddTouchTapInput(ActionLeft,
                                       leftSideScreen,
                                       false);
            gameInput.AddTouchTapInput(ActionRight,
                                       rightSideScreen,
                                       false);

            gameInput.AddTouchTapInput(ActionUpperLeft,
                                       upperLeftScreen,
                                       true);
            gameInput.AddTouchTapInput(ActionUpperRight,
                                       upperRightScreen,
                                       true);

            gameInput.AddTouchSlideInput(ActionSlideLeft,
                                         Input.Direction.Left,
                                         25.0f);
            gameInput.AddTouchSlideInput(ActionSlideRight,
                                         Input.Direction.Right,
                                         25.0f);
        }

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

            this.isSlow = false;
            this.slowTimer = 0.0f;

            this.overdriveFactor = OVERDRIVE_NORMAL_FACTOR;
            this.overdriveTimer = 0.0f;
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
                    shotTimer = minShotTimer / overdriveFactor;
                }
                else
                {
                    shotTimer = minShotTimer * 1.5f / overdriveFactor;
                }
            }
        }

        private void fireDoubleShot()
        {
            if (shotTimer <= 0.0f)
            {
                overheat += OverheatDoubleShot;

                if (overheat < OVERHEAT_MAX)
                {
                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset + new Vector2(13, 0),
                                                    new Vector2(0, -1),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);
                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, 0),
                                                    new Vector2(0, -1),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);
                    shotTimer = minDoubleShotTimer / overdriveFactor;
                }
                else
                {
                    shotTimer = minDoubleShotTimer * 1.5f / overdriveFactor;
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

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(-13, 0),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(87.5f)), -(float)Math.Sin(MathHelper.ToRadians(87.5f))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset - new Vector2(13, 0),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(92.5f)), -(float)Math.Sin(MathHelper.ToRadians(92.5f))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    false);

                    shotTimer = minTripleShotTimer / overdriveFactor;
                }
                else
                {
                    shotTimer = minTripleShotTimer * 2.0f / overdriveFactor;
                }
            }
        }

        private void fireSideLeftShot()
        {
            if (sideShotTimer <= 0.0f)
            {
                overheat += (OverheatSideShot);

                if (overheat < OVERHEAT_MAX)
                {
                    this.PlayerShotManager.FireShot(this.playerSprite.Center + new Vector2(-20, 5),
                                                    new Vector2(-1, 0),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Center + new Vector2(-20, 5),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(187.5f)), -(float)Math.Sin(MathHelper.ToRadians(187.5f))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Center + new Vector2(-20, 5),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(172.5f)), -(float)Math.Sin(MathHelper.ToRadians(172.5f))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                    sideShotTimer = minSideShotTimer / overdriveFactor;
                }
                else
                {
                    sideShotTimer = minSideShotTimer * 2.0f / overdriveFactor;
                }
            }
        }

        private void fireSideRightShot()
        {
            if (sideShotTimer <= 0.0f)
            {
                overheat += (OverheatSideShot);

                if (overheat < OVERHEAT_MAX)
                {
                    this.PlayerShotManager.FireShot(this.playerSprite.Center + new Vector2(20, 5),
                                                    new Vector2(1, 0),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                    this.PlayerShotManager.FireShot(this.playerSprite.Center + new Vector2(20, 5),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(7.5f)), -(float)Math.Sin(MathHelper.ToRadians(7.5f))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    false);

                    this.PlayerShotManager.FireShot(this.playerSprite.Center + new Vector2(20, 5),
                                                    new Vector2((float)Math.Cos(MathHelper.ToRadians(-7.5f)), -(float)Math.Sin(MathHelper.ToRadians(-7.5f))),
                                                    true,
                                                    new Color(0, 255, 33),
                                                    true);

                    sideShotTimer = minSideShotTimer / overdriveFactor;
                }
                else
                {
                    sideShotTimer = minSideShotTimer * 2.0f / overdriveFactor;
                }
            }
        }

        private void fireSpecialShot()
        {
            if (specialShotTimer <= 0.0f &&
                SpecialShotsRemaining > 0 &&
                overheat < (OVERHEAT_MAX - OverheatSpecialShot))
            {
                overheat += (OverheatSideShot);
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

                    specialShotTimer = minSpecialShotTimer / overdriveFactor;
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

                    rocketTimer = minCarliRocketTimer / overdriveFactor;
                }
            }
        }

        private void HandleTouchInput(TouchCollection touches)
        {
                if (touches.Count == 1)
                {
                    if (gameInput.IsPressed(ActionSlideLeft) && middleScreen.Contains(gameInput.CurrentTouchPoint(ActionSlideLeft)))
                    {
                        shotTimer = minShotTimer;
                        fireSideLeftShot();
                    }
                    if (gameInput.IsPressed(ActionSlideRight) && middleScreen.Contains(gameInput.CurrentTouchPoint(ActionSlideRight)))
                    {
                        shotTimer = minShotTimer;
                        fireSideRightShot();
                    }

                    if (gameInput.IsPressed(ActionLeft))
                    {
                        fireShot();
                    }
                    if (gameInput.IsPressed(ActionRight))
                    {
                        fireDoubleShot();
                    }
                    if (gameInput.IsPressed(ActionUpperLeft))
                    {
                        fireCarliRocket();
                    }
                    if (gameInput.IsPressed(ActionUpperRight))
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

            // ***** new in version 1.2 to 2.4 - 30fps *****
            //currentAccValue.X = currentAccValue.X + (float)Math.Sin(settings.GetNeutralPosition());

            //currentAccValue.Y = MathHelper.Clamp(currentAccValue.Y, -0.4f, 0.4f);
            //currentAccValue.X = MathHelper.Clamp(currentAccValue.X, -0.4f, 0.4f);

            //if (!IsOutOfControl)
            //    playerSprite.Velocity = new Vector2(-currentAccValue.Y * 6,
            //                                        -currentAccValue.X * 3);
            //else
            //    playerSprite.Velocity = new Vector2(currentAccValue.Y * 6,
            //                                        -currentAccValue.X * 3);

            //if (playerSprite.Velocity.Length() < 0.1f)
            //{
            //    playerSprite.Velocity = Vector2.Zero;
            //}

            // ******* Version 2.5 - 60 fps ********
            Vector3 current = currentAccValue;
            current.X = current.X + (float)Math.Sin(settings.GetNeutralPosition());

            current.Y = MathHelper.Clamp(current.Y, -0.4f, 0.4f);
            current.X = MathHelper.Clamp(current.X, -0.4f, 0.4f);

            if (!IsOutOfControl)
                playerSprite.Velocity = new Vector2(-current.Y * 6,
                                                    -current.X * 3);
            else
                playerSprite.Velocity = new Vector2(current.Y * 6,
                                                    -current.X * 3);

            if (playerSprite.Velocity.Length() < 0.15f)
            {
                playerSprite.Velocity = Vector2.Zero;
            }
        }

        private void HandleKeyboardInput(KeyboardState state)
        {
            #if DEBUG

            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1))
            {
                fireShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D2))
            {
                fireDoubleShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D3))
            {
                fireTripleShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                fireSideLeftShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                fireSideRightShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D4))
            {
                fireSpecialShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D5))
            {
                fireCarliRocket();
            }

            Vector2 velo = Vector2.Zero;

            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                velo += new Vector2(-1.0f, 0.0f);
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                velo += new Vector2(1.0f, 0.0f);
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                velo += new Vector2(0.0f, -1.0f);
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                velo += new Vector2(0.0f, 1.0f);
            }

            if (velo != Vector2.Zero)
                velo.Normalize();

            playerSprite.Velocity = velo;

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
                shotTimer -= elapsed;
                sideShotTimer -= elapsed;
                specialShotTimer -= elapsed;
                rocketTimer -= elapsed;

                HandleTouchInput(TouchPanel.GetState());
                HandleKeyboardInput(Keyboard.GetState());

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

                if (Overheat > 0.95f)
                    Overheat -= CoolDownRate * elapsed * 0.5f * overdriveFactor;
                else if (Overheat > 0.9f)
                    Overheat -= CoolDownRate * elapsed * 0.6f * overdriveFactor;
                else if (Overheat > 0.85f)
                    Overheat -= CoolDownRate * elapsed * 0.7f * overdriveFactor;
                else if (Overheat > 0.80f)
                    Overheat -= CoolDownRate * elapsed * 0.8f * overdriveFactor;
                else if (Overheat > 0.75f)
                    Overheat -= CoolDownRate * elapsed * 0.9f * overdriveFactor;
                else
                    Overheat -= CoolDownRate * elapsed * overdriveFactor;

                if (HitPoints != 0.0f)
                {
                    this.IncreaseHitPoints(HealRate * elapsed);
                }

                if (this.overheat > 0.80f)
                {
                    this.DecreaseHitPoints(OverheatKillRateMax);
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

                // Overdrive/Underdrive
                if (IsOverUnderdrive)
                {
                    overdriveTimer -= elapsed;

                    if (overdriveTimer <= 0.0f)
                    {
                        overdriveFactor = OVERDRIVE_NORMAL_FACTOR;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);

            if (!IsDestroyed)
            {
                if (IsOverdrive)
                {
                    playerSprite.TintColor = new Color(0.5f, 1.0f, 0.5f);
                }
                else if (isOutOfControl || IsUnderdrive)
                {
                    playerSprite.TintColor = new Color(1.0f, 0.5f, 0.5f);
                }
                
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

        public void StartOverdrive()
        {
            this.overdriveTimer = OVERDRIVE_DURATION;
            this.overdriveFactor = OVERDRIVE_FACTOR;
        }

        public void StartUnderdrive()
        {
            this.overdriveTimer = OVERDRIVE_DURATION;
            this.overdriveFactor = UNDERDRIVE_FACTOR;
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
                ScoreMulti += score / 4000.0f;
            else
                ScoreMulti += score / 4500.0f;
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
            this.sideShotTimer = Single.Parse(reader.ReadLine());
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

            this.overdriveTimer = Single.Parse(reader.ReadLine());
            this.overdriveFactor = Single.Parse(reader.ReadLine());
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
            writer.WriteLine(this.sideShotTimer);
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

            writer.WriteLine(this.overdriveTimer);
            writer.WriteLine(this.overdriveFactor);
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
                this.overheat = MathHelper.Clamp(value, OVERHEAT_MIN, OVERHEAT_MAX);
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

        public bool IsOverUnderdrive
        {
            get
            {
                return overdriveFactor != OVERDRIVE_NORMAL_FACTOR;
            }
        }

        public bool IsOverdrive
        {
            get
            {
                return overdriveFactor == OVERDRIVE_FACTOR;
            }
        }

        public bool IsUnderdrive
        {
            get
            {
                return overdriveFactor == UNDERDRIVE_FACTOR;
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

        private void OnAccelerometerHelperReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            currentAccValue = new Vector3((float)e.OptimalyFilteredAcceleration.X,
                                          (float)e.OptimalyFilteredAcceleration.Y,
                                          (float)e.OptimalyFilteredAcceleration.Z);
        }

        #endregion
    }
}
