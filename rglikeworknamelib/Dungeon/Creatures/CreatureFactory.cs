using System;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Creatures
{
    public class CreatureFactory {
        private static readonly Logger logger = LogManager.GetLogger("ItemFactory");
        public static Creature GetInstance(string id, Vector2 pos = default(Vector2))
        {
            if (!CreatureDataBase.Data.ContainsKey(id))
            {
                logger.Error(string.Format("Missing CreatureData id={0}!!!", id));
                return null;
            }
            CreatureData creatureData = CreatureDataBase.Data[id];
            var a = new Creature();
            a.Id = id;
            a.MTex = creatureData.MTex;
            a.Position = pos;
            a.Hp = new Stat(creatureData.Hp, creatureData.Hp);

            if (a.Data.BehaviorScript != null)
            {
                a.behaviorScript = CreatureDataBase.ScriptExecute;
            }

            //if (a.Data.BehaviorScript == "bs_zombie")
            //{
            //    a.behaviorScript = CreatureDataBase.Bs_zombie;
            //}

            //if (a.Data.BehaviorScript == "bs_wander")
            //{
            //    a.behaviorScript = CreatureDataBase.Bs_wander;
            //}

            //if (a.Data.BehaviorScript == "bs_rabbit")
            //{
            //    a.behaviorScript = CreatureDataBase.Bs_rabbit;
            //}

            //if (a.Data.BehaviorScript == "bs_dog")
            //{
            //    a.behaviorScript = CreatureDataBase.Bs_dog;
            //}

            return a;
        }
    }
}