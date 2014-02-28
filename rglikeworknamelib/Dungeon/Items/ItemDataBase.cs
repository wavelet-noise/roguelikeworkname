using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using jarg;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Items
{
    public sealed class ItemDataBase
    {
        private static readonly Logger logger = LogManager.GetLogger("ItemDataBase");

        private static Logger logger_ = LogManager.GetCurrentClassLogger();

        public Dictionary<string, ItemData> Data;

        public static List<ItemCraftData> Craft;
        private static ItemDataBase instance_;
        public Dictionary<string,ItemAction> ItemScripts;

        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        private ItemDataBase()
        {
            //texatlas = texatlas_;
            Data = UniversalParser.JsonDictionaryDataLoader<ItemData>(Settings.GetItemDataDirectory());
            ItemScripts = new Dictionary<string, ItemAction>();
            Craft = UniversalParser.JsonListDataLoader<ItemCraftData>(Settings.GetCraftsDirectory());

            ItemScripts.Add("nothing", new ItemAction(Nothing, "������"));
            ItemScripts.Add("opencan", new ItemAction(OpenCan, "������� �����"));
            ItemScripts.Add("dissass", new ItemAction(Disass, "��������� �����"));
            ItemScripts.Add("turnradio", new ItemAction(RadioOnOff, "�������� �����"));
            ItemScripts.Add("openbottle", new ItemAction(OpenBottle, "������� �������"));
            ItemScripts.Add("destroycloth", new ItemAction(DestroyCloth, "�������� �� ������"));
            ItemScripts.Add("smoke", new ItemAction(Smoke, "�������� ��������"));
        }

        private void Nothing(Player arg1, Item arg2) {
            
        }

        /// <summary>
        ///     ������ ��������� ��������� ��������� ��������
        /// </summary>
        public static ItemDataBase Instance
        {
            get { return instance_ ?? (instance_ = new ItemDataBase()); }
        }

        public string GetItemDescription(Item i)
        {
            return i.Data.Description;
        }

        public static void ScriptExecute(GameTime gt, MapSector ms_, Player hero, Creature target)
        {
            //ItemScripts[target.Data.BehaviorScript].BehaviorScript(gt, ms_, hero, target, Settings.rnd);
        }

        public List<KeyValuePair<string, ItemData>> GetBySpawnGroup(string group)
        {
            return Data.Where(x => x.Value.SpawnGroup == group).ToList();
        }

        public static void StackSimilar(ref List<Item> items)
        {
            var a = new List<Item>();

            foreach (Item item in items)
            {
                Item it = a.FirstOrDefault(x => x.Id == item.Id && item.Doses == 0);
                if (it != null)
                {
                    it.Count += item.Count;
                }
                else
                {
                    a.Add(item);
                }
            }

            items = a;
        }

        public string GetItemFullDescription(Item i)
        {
            ItemData item = Data[i.Id];
            var sb = new StringBuilder();
            sb.Append(item.Name);
            if (item.Description != null)
            {
                sb.Append(Environment.NewLine + item.Description);
            }
            sb.Append(Environment.NewLine + string.Format("{0} �", item.Weight));
            sb.Append(Environment.NewLine + string.Format("{0} �����", item.Volume));
            if (Settings.DebugInfo)
            {
                sb.Append(Environment.NewLine + string.Format("id {0} uid {1}", i.Id, 0));
            }

            if (item.AfteruseId != null)
            {
                sb.Append(Environment.NewLine + string.Format("��������� {0}", Data[item.AfteruseId].Name));
            }

            if (item.Buff != null)
            {
                sb.Append(Environment.NewLine + Environment.NewLine + string.Format("������� :"));

                foreach (string buff in item.Buff)
                {
                    sb.Append(Environment.NewLine + string.Format("{0}", BuffDataBase.Data[buff].Name));
                }
            }
            if (item.Ammo != null)
            {
                sb.Append(Environment.NewLine + Environment.NewLine + string.Format("������ :"));

                sb.Append(Environment.NewLine + Data[item.Ammo].Name);
            }
            //switch (item.SortType) {
            //    case ItemType.Medicine:
            //        sb.Append(item.)
            //        break;
            //}
            return sb.ToString();
        }

        #region ItemScripts

        public void OpenCan(Player p, Item target)
        {
            if (p.Inventory.ContainsId("knife") || p.Inventory.ContainsId("otvertka"))
            {
                p.Inventory.TryRemoveItem(target.Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance(Data[target.Id].AfteruseId, 1));
            }
            else
            {
                EventLog.Add("����� ��������� ����� ��� ����� ��� ��� ��������", Color.Yellow,
                    LogEntityType.NoAmmoWeapon);
            }
        }

        private void Disass(Player p, Item target)
        {
            if (p.Inventory.ContainsId("otvertka"))
            {
                p.Inventory.TryRemoveItem(target.Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance("chipset", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("batery", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("smallvint", Settings.rnd.Next(5) + 10));

                EventLog.Add(string.Format("�� ������� ���������� {0}", target.Data.Name), Color.Yellow,
                    LogEntityType.NoAmmoWeapon);
            }
            else
            {
                EventLog.Add("����� ��������� ����������� ��� ����� ��������", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }

        private void RadioOnOff(Player p, Item target)
        {
            EventLog.Add("����� ����������", Color.White, LogEntityType.Default);
        }

        public void OpenBottle(Player p, Item target)
        {
            p.Inventory.TryRemoveItem(target.Id, 1);
            p.Inventory.AddItem(ItemFactory.GetInstance(Data[target.Id].AfteruseId, 1));
        }

        public void DestroyCloth(Player p, Item target)
        {
            int weight = target.Data.Weight;
            if (weight > 10000)
            {
                EventLog.Add("������� ������� �������", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            if (weight < 100)
            {
                EventLog.Add("������� ������� ���������", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            p.Inventory.TryRemoveItem(target.Id, 1);
            int smallparts = 0;
            int bigparts = 0;
            Random rnd = Settings.rnd;
            while (weight >= 50)
            {
                int part = rnd.Next(2);
                switch (part)
                {
                    case 0:
                        weight -= 50;
                        p.Inventory.AddItem(ItemFactory.GetInstance("brcloth", 1));
                        smallparts++;
                        break;
                    case 1:
                        weight -= 100;
                        p.Inventory.AddItem(ItemFactory.GetInstance("partcloth", 1));
                        bigparts++;
                        break;
                }
            }
            string mess = string.Format("�� ������� ��������� {0} ��", target.Data.Name);
            if (bigparts > 0)
            {
                mess += string.Format(" {0} {1}", bigparts, Data["brcloth"].Name);
            }
            if (smallparts > 0)
            {
                mess += string.Format(" {0} {1}", smallparts, Data["partcloth"].Name);
            }
            EventLog.Add(mess, Color.Yellow, LogEntityType.NoAmmoWeapon);
        }

        public void Smoke(Player p, Item target)
        {
            if (p.Inventory.ContainsId("lighter1") || p.Inventory.ContainsId("lighter2"))
            {
                EventLog.Add(
                    string.Format("�� �������� �������� "),
                    GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                foreach (IBuff buff in target.Buffs)
                {
                    buff.ApplyToTarget(p);
                }

                AchievementDataBase.Stat["sigause"].Count++;

                target.Doses--;
                if (target.Doses <= 0)
                {
                    p.Inventory.RemoveItem(target);
                }
            }
            else
            {
                EventLog.Add("����� ������ ��� ����� ���������", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }

        #endregion
    }
}