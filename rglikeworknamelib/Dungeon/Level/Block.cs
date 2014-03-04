using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Block {
        private string id_;

        public object ScriptTag;

        public List<Item> StoredItems = new List<Item>();

        public string Id {
            get { return id_; }
            set {
                id_ = value;
                Data = Registry.Instance.Blocks[value];
            }
        }

        public BlockData Data {
            get { return data_; }
            private set { data_ = value; }
        }

        public Color Lightness { get; set; }

        public Vector2 Source { get; private set; }

        public bool IsVisible()
        {
            return Lightness == Color.White;
        }

        private string mTex_;
        [NonSerialized]
        private BlockData data_;

        public string MTex
        {
            get { return mTex_; }
            set
            {
                Source = Atlases.GetSource(value);
                mTex_ = value;
            }
        }

        public virtual void Update(TimeSpan ts, MapSector ms, Player p)
        {
        }

        public static string GetSmartActionName(SmartAction smartAction)
        {
            switch (smartAction)
            {
                case SmartAction.ActionOpenContainer:
                    return "��������� ����������";
                case SmartAction.ActionOpenClose:
                    return "�������/�������";
                default:
                    return "���������";
            }
        }
    }
}