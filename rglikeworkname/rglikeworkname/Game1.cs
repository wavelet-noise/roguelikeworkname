using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mork;
using System;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Parser;

namespace jarg
{
    public class Game1 : Game
    {
        private BlockDataBase bdb_;
        private FloorDataBase fdb_;
        private MonsterDataBase mdb_;
        private SchemesDataBase sdb_;
        private ItemDataBase idb_;

        private GraphicsDeviceManager graphics_;
        private Vector2 camera_;
        private GameLevel currentFloor_;
        private SpriteFont font1_;
        private KeyboardState ks_;
        private Vector3 lastPos_;
        private LineBatch lineBatch_;
        private KeyboardState lks_;
        private MouseState lms_;
        private MonsterSystem monsterSystem_;
        private MouseState ms_;
        private Vector2 pivotpoint_;
        private Player player_;
        private SpriteBatch spriteBatch_;

        //private Manager manager_;

        private GamePhase gamePhase_ = GamePhase.testgame;

        enum GamePhase
        {
            testgame
        }

        public Game1()
        {
            graphics_ = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Log.Init();

            rglikeworknamelib.Settings.Resolution = new Vector2(1024, 768);
            graphics_.IsFullScreen = false;
            graphics_.PreferredBackBufferHeight = (int)rglikeworknamelib.Settings.Resolution.Y;
            graphics_.PreferredBackBufferWidth = (int)rglikeworknamelib.Settings.Resolution.X;
            graphics_.SynchronizeWithVerticalRetrace = false;
            
            IsFixedTimeStep = true;
            IsMouseVisible = true;
            graphics_.ApplyChanges();


            base.Initialize();
        }

        private Texture2D itemSelectTex_;
        private Texture2D transparentPixel_;
        protected override void LoadContent()
        {
            spriteBatch_ = new SpriteBatch(GraphicsDevice);
            lineBatch_ = new LineBatch(GraphicsDevice);

            itemSelectTex_ = Content.Load<Texture2D>(@"Textures/Dungeon/Items/itemselect");
            transparentPixel_ = Content.Load<Texture2D>(@"Textures/transparent_pixel");

            bdb_ = new BlockDataBase();
            fdb_ = new FloorDataBase();
            mdb_ = new MonsterDataBase();
            sdb_ = new SchemesDataBase();
            idb_ = new ItemDataBase(ParsersCore.LoadTexturesInOrder(rglikeworknamelib.Settings.GetItemDataDirectory() + @"/textureloadorder.ord", Content));

            font1_ = Content.Load<SpriteFont>(@"Fonts/Font1");

            var tex = new Collection<Texture2D> {
                                           Content.Load<Texture2D>(@"Textures/transparent_pixel"),
                                           Content.Load<Texture2D>(@"Textures/Units/car")
                                        };
            monsterSystem_ = new MonsterSystem(spriteBatch_, tex);


            currentFloor_ = GameLevel.CreateGameLevel(spriteBatch_, 
                                                      ParsersCore.LoadTexturesInOrder(rglikeworknamelib.Settings.GetFloorDataDirectory() + @"/textureloadorder.ord", Content),
                                                      ParsersCore.LoadTexturesInOrder(rglikeworknamelib.Settings.GetObjectDataDirectory() + @"/textureloadorder.ord", Content), 
                                                      bdb_, 
                                                      fdb_, 
                                                      sdb_,
                                                      512,
                                                      512
                                                     );

            player_ = new Player(spriteBatch_, Content.Load<Texture2D>(@"Textures/Units/car"), font1_) {
            };
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public static bool ErrorExit;
        public float seeAngleDeg = 60;
        public TimeSpan sec = TimeSpan.Zero;
        protected override void Update(GameTime gameTime)
        {
            if (ErrorExit) Exit();

            base.Update(gameTime);

            KeyboardUpdate(gameTime);
            MouseUpdate(gameTime);

            sec += gameTime.ElapsedGameTime;
            if (sec >= TimeSpan.FromSeconds(1)) {
                sec = TimeSpan.Zero;

                currentFloor_.GenerateMinimap(GraphicsDevice);
            }


            lastPos_ = player_.Position;
            if ((int)player_.Position.X / 3 == (int)player_.LastPos.X / 3 && (int)player_.Position.Y / 3 == (int)player_.LastPos.Y / 3) {
                if (seeAngleDeg < 150) {
                    seeAngleDeg += (float)gameTime.ElapsedGameTime.TotalSeconds * 50;
                } else {
                    seeAngleDeg = 150;
                }
            } else {
                if (seeAngleDeg > 60) {
                    seeAngleDeg -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2000;
                } else {
                    seeAngleDeg = 60;
                }
            }
            //currentFloor_.CalcWision(player_, (float)Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X), seeAngleDeg);
            player_.Update(gameTime, currentFloor_, bdb_);

            camera_ = Vector2.Lerp(camera_, pivotpoint_, (float)gameTime.ElapsedGameTime.TotalSeconds * 2);

            if (rglikeworknamelib.Settings.DebugInfo) {
                FrameRateCounter.Update(gameTime);
            }
        }

        private void KeyboardUpdate(GameTime gameTime)
        {
            lks_ = ks_;
            ks_ = Keyboard.GetState();

            if (ks_[Keys.F4] == KeyState.Down && lks_[Keys.F4] == KeyState.Up) {
                rglikeworknamelib.Settings.DebugInfo = !rglikeworknamelib.Settings.DebugInfo;
            }

            if (ks_[Keys.W] == KeyState.Down) {
                player_.Accelerate(new Vector3(0, -10, 0));
            }
            if (ks_[Keys.S] == KeyState.Down) {
                player_.Accelerate(new Vector3(0, 10, 0));
            }
            if (ks_[Keys.A] == KeyState.Down) {
                player_.Accelerate(new Vector3(-10, 0, 0));
            }
            if (ks_[Keys.D] == KeyState.Down) {
                player_.Accelerate(new Vector3(10, 0, 0));
            }

            if (ks_[Keys.G] == KeyState.Down && lks_[Keys.G] == KeyState.Up) {
                currentFloor_.Rebuild();
            }

            if (ks_[Keys.H] == KeyState.Down && lks_[Keys.H] == KeyState.Up) {
                foreach (var b in currentFloor_.blocks_) {
                    b.explored = true;
                    b.lightness = Color.White;
                }
            }

            pivotpoint_ = new Vector2(player_.Position.X - (rglikeworknamelib.Settings.Resolution.X - 200) / 2, player_.Position.Y - rglikeworknamelib.Settings.Resolution.Y / 2);

        }

        private void MouseUpdate(GameTime gameTime)
        {
            lms_ = ms_;
            ms_ = Mouse.GetState();

            int aa = (ms_.X + (int)camera_.X) / 32 * currentFloor_.ry + (ms_.Y + (int)camera_.Y) / 32;
            if (aa >= 0 && currentFloor_.blocks_[aa].id != 0 && currentFloor_.blocks_[aa].explored)
            {
                int nx = (ms_.X + (int)camera_.X) / 32;
                int ny = (ms_.Y + (int)camera_.Y) / 32;
                var a = currentFloor_.blocks_[nx * currentFloor_.ry + ny];
                var b = bdb_.Data[a.id];
                string s = Block.GetSmartActionName(b.smartAction) + " " + b.name;
                if (rglikeworknamelib.Settings.DebugInfo) s += " id" + a.id + " tex" + b.texNo;

                if (ms_.LeftButton == ButtonState.Pressed && lms_.LeftButton == ButtonState.Released && currentFloor_.IsCreatureMeele(nx, ny, player_)) {
                    var undermouseblock = bdb_.Data[a.id];
                    if (undermouseblock.smartAction == SmartAction.ActionOpenContainer) {
                    }

                    if(undermouseblock.smartAction == SmartAction.ActionOpenClose) {
                       currentFloor_.OpenCloseDoor(nx, ny);
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch_.Begin();
            currentFloor_.Draw(gameTime, camera_);
            currentFloor_.Draw2(gameTime, camera_);
            player_.Draw(gameTime, camera_);

            spriteBatch_.Draw(currentFloor_.GetMinimap(), new Rectangle(15,15,128,128),
                 Color.White);
            spriteBatch_.End();
            
            base.Draw(gameTime);

            if (rglikeworknamelib.Settings.DebugInfo) {
                DebugInfoDraw(gameTime);
            }

            lineBatch_.Draw();
            lineBatch_.Clear();
        }

        private void DebugInfoDraw(GameTime gameTime)
        {
            FrameRateCounter.Draw(gameTime, font1_, spriteBatch_, lineBatch_, (int)rglikeworknamelib.Settings.Resolution.X,
                                  (int)rglikeworknamelib.Settings.Resolution.Y);
            spriteBatch_.Begin();
            spriteBatch_.DrawString(font1_, string.Format("{0}\n{1}\n{2}", (float)Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X),
                                                                           (float)Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X) + MathHelper.ToRadians(60),
                                                                           (float)Math.Atan2(ms_.Y - player_.Position.Y + camera_.Y, ms_.X - player_.Position.X + camera_.X) - MathHelper.ToRadians(60)), new Vector2(500, 10), Color.White);

            spriteBatch_.End();
        }
    }
}