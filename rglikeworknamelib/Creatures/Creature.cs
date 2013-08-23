using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Creatures {
    public class Creature {
        public string ID;

        private Vector2 lastpos_;

        public bool IsWarmCloth() {
            return true;
        }

        public float hunger_ = 100;
        public float maxHunger_ = 100;

        public float thirst_ = 100;
        public float maxThirst_ = 100;

        public float heat_ = 36;
        public float maxHeat_ = 36;

        private Vector2 position_;
        public Vector2 Position  {
            get { return position_; }
            set {
                lastpos_ = position_; 
                position_ = value;
            }
        }

        public void SetPositionInBlocks(int x, int y)
        {
            position_ = new Vector2((x + 0.5f) * 32, (y + 0.5f) * 32);
        }

        /// <summary>
        /// Returns creature position in game blocks
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPositionInBlocks() {
            var po = Position;
            po.X = po.X < 0 ? po.X / 32 - 1 : po.X / 32;
            po.Y = po.Y < 0 ? po.Y / 32 - 1 : po.Y / 32;
            return po;
        }

        public Vector2 LastPos
        {
            get { return lastpos_; }
            set { lastpos_ = value; }
        }

        public void Update(GameTime gt, MapSector ms) {
            
            if (hunger_ > 0) {
                hunger_ -= (float) gt.ElapsedGameTime.TotalDays*60f;
            }  else {
                int a = IsWarmCloth() ? 10 : 1;
                heat_ -= heat_ * (float)gt.ElapsedGameTime.TotalMinutes / 10 / a;
                heat_ += GlobalWorldLogic.Temperature * (float)gt.ElapsedGameTime.TotalMinutes / 10 / a;
            }
        }


        public Vector2 ShootPoint { get; set; }

        public Vector2 Velocity;

        public void Draw(SpriteBatch spriteBatch, Vector2 camera, MapSector ms) {
            var a = GetPositionInBlocks();

            spriteBatch.Draw(Atlases.CreatureAtlas[MonsterDataBase.Data[ID].MTex], Position - camera, ms.GetBlock((int)a.X, (int)a.Y).Lightness);
        }
    }
}