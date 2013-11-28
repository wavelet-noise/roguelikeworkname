using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Creatures {
    public interface ICreature {
        Vector2 Position { get; set; }
        Stat Hp { get; set; }
        bool isDead { get; }
        string Id { get;}
        CreatureData Data { get; }
        Texture2D MTex { get; }
        bool Skipp { get; set; }
        MapSector ms { get; set; }
        List<IBuff> Buffs { get; set; }
        Abilities Abilities { get; set; }
        void Kill(MapSector ms);
        void GiveDamage(float value, DamageType type);
        void Update(GameTime gt, MapSector ms, Player hero);
        void Draw(SpriteBatch spriteBatch, Vector2 camera, MapSector ms);
        Vector2 WorldPosition();
        Vector2 GetWorldPositionInBlocks();
        event EventHandler OnDamageRecieve;
        event EventHandler OnDeath;
        void OnLoad();
    }
}