﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;

namespace DropListEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Clear();

            new ItemDataBase();

            foreach (var itemData in ItemDataBase.Data) {
                listBox1.Items.Add(string.Format("id {0} -- {1}", itemData.Key, itemData.Value.Name));
            }

            new BlockDataBase();

            foreach (var itemData in BlockDataBase.Storages) {
                listBox2.Items.Add(string.Format("id {0} -- {1}", itemData.Key, itemData.Value.Name));
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if(ItemDataBase.SpawnLists.ContainsKey(BlockDataBase.Storages.ElementAt(listBox2.SelectedIndex).Key)) {
                foreach (var list in ItemDataBase.SpawnLists[BlockDataBase.Storages.ElementAt(listBox2.SelectedIndex).Key]) {
                    sb.Append("x");
                    sb.Append(list.Repeat);
                    sb.Append(" :: ");
                    sb.Append(string.Join(", ", list.Ids));
                    sb.Append("\n       ");
                    sb.Append(list.MinCount);
                    sb.Append("-");
                    sb.Append(list.MaxCount);
                    sb.Append(" : ");
                    sb.Append(list.Prob);
                    sb.Append("%");
                    sb.AppendLine();
                }
            }
            label1.Text = sb.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                var a = new DataContractJsonSerializer(typeof(Dictionary<string, ItemData>));
            Dictionary<string, ItemData> ibj;
            using (var stream = new StreamReader("1.json")) {
                ibj = (Dictionary<string, ItemData>) a.ReadObject(stream.BaseStream);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var a = new DataContractJsonSerializer(typeof(Dictionary<string, ItemData>));
            Dictionary<string, ItemData> ibj;
            using (var stream = new StreamWriter("2.json"))
            {
                a.WriteObject(stream.BaseStream,ItemDataBase.Data);
            }
        }
    }
}
