﻿using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Floor {
        private string id_;
        public string Id
        {
            get { return id_; }
            set
            {
                id_ = value;
                data_ = Registry.Instance.Floors[value];
            }
        }

        internal string mTex_;
        public string MTex
        {
            get { return mTex_; }
            set
            {
                source_ = Atlases.GetSource(value);
                mTex_ = value;
            }
        }
        [NonSerialized]
        private FloorData data_;
        public FloorData Data
        {
            get { return data_; }
        }
        [NonSerialized]
        private Vector2 source_;
        public Vector2 Source {
            get { return source_; }
        }
    }
}