﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mork;
using NLog;
using rglikeworknamelib;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dialogs;
using rglikeworknamelib.Dungeon;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Generation.Names;
using rglikeworknamelib.Parser;
using rglikeworknamelib.Window;
using Button = rglikeworknamelib.Window.Button;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using EventLog = rglikeworknamelib.EventLog;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Label = rglikeworknamelib.Window.Label;
using ProgressBar = rglikeworknamelib.Window.ProgressBar;
using Settings = rglikeworknamelib.Settings;

namespace jarg
{
    public partial class Game1
    {
        #region Window Designer
        #region Windows Vars

        private Window WindowStats;
        private ProgressBar StatsHunger;
        private ProgressBar StatsJajda;
        private ProgressBar StatsHeat;
        private Button CloseAllTestButton;
        private ListContainer contaiter1;

        private Window WindowMinimap;
        private Image ImageMinimap;

        private Window WindowSettings;
        private Label LabelHudColor;
        private Button ButtonHudColor1;
        private Button ButtonHudColor2;
        private Button ButtonHudColor3;
        private Button ButtonHudColor4;
        private Button ButtonHudColor5;
        private Label LabelTimeType;
        private Button Button12h, Button24h;

        private Window WindowIngameMenu;
        private Label LabelIngameMenu1;
        private Button ButtonIngameMenuSettings;
        private Button ButtonIngameExit;

        private Window WindowMainMenu;
        private Label LabelMainMenu;
        private Button ButtonNewGame;
        private Button ButtonSettings;
        private RunningLabel RunningMotd;
        private Label LabelControls;
        private Button ButtonOpenGit;

        private Window WindowCaracterCration;
        private Button ButtonCaracterConfirm;
        private Button ButtonCaracterCancel;

        private Window WindowPickup;

        private Window WindowInventory;
        private ListContainer ContainerInventoryItems;
        private LabelFixed InventoryMoreInfo;
        private Button InventorySortAll;
        private Button InventorySortMedicine;
        private Button InventorySortFood;
        private Button IntentoryEquip;

        private Window WindowContainer;
        private ListContainer ContainerContainer;
        private LabelFixed LabelContainer;
        private Button ButtonContainerTakeAll;

        private Window WindowEventLog;
        private ListContainer ContainerEventLog;

        private Window WindowIngameHint;
        private Label LabelIngameHint;

        private Window WindowGlobal;
        private Image ImageGlobal;

        private Window WindowCaracter;
        private DoubleLabel LabelCaracterHat;
        private DoubleLabel LabelCaracterGlaces;
        private DoubleLabel LabelCaracterHelmet;
        private DoubleLabel LabelCaracterChest;
        private DoubleLabel LabelCaracterShirt;
        private DoubleLabel LabelCaracterPants;
        private DoubleLabel LabelCaracterGloves;
        private DoubleLabel LabelCaracterBoots;
        private DoubleLabel LabelCaracterGun;
        private DoubleLabel LabelCaracterMeele;
        private DoubleLabel LabelCaracterAmmo;
        private DoubleLabel LabelCaracterBag;
        private DoubleLabel LabelCaracterHp;

        private Window InfoWindow;
        private DoubleLabel InfoWindowLabel;

        private Window WindowStatist;
        private ListContainer ListStatist;
        #endregion

        private void CreateWindows(Texture2D wp, SpriteFont sf, WindowSystem ws)
        {
            Random rnd = new Random();

            WindowStats = new Window(new Rectangle(50, 50, 400, 400), "Stats", true, wp, sf, ws) { Visible = false };
            StatsHeat = new ProgressBar(new Rectangle(50, 50, 100, 20), "", wp, sf, WindowStats);
            StatsJajda = new ProgressBar(new Rectangle(50, 50 + 30, 100, 20), "", wp, sf, WindowStats);
            StatsHunger = new ProgressBar(new Rectangle(50, 50 + 30 * 2, 100, 20), "", wp, sf, WindowStats);
            CloseAllTestButton = new Button(new Vector2(10, 100), "Close all", wp, sf, WindowStats);
            CloseAllTestButton.onPressed += CloseAllTestButton_onPressed;
            contaiter1 = new ListContainer(new Rectangle(200, 200, 100, 200), wp, sf, WindowStats);
            for (int i = 1; i < 20; i++)
                contaiter1.AddItem(new Button(Vector2.Zero, rnd.Next(1, 1000).ToString(), wp, sf, WindowStats));

            WindowMinimap = new Window(new Rectangle((int)Settings.Resolution.X - 180, 10, 128 + 20, 128 + 40), "minimap", true, wp, sf, ws) { Closable = false, hides = true };
            ImageMinimap = new Image(new Vector2(10, 10), new Texture2D(GraphicsDevice, 88, 88), Color.White, WindowMinimap);

            WindowSettings =
                new Window(new Vector2(Settings.Resolution.X, Settings.Resolution.Y),
                    "Settings", true, wp, sf, ws) { Visible = false, Moveable = false };
            LabelHudColor = new Label(new Vector2(10, 10), "HUD color", wp, sf, WindowSettings);
            ButtonHudColor1 = new Button(new Vector2(10 + 50 + 40 * 1, 10), "1", wp, sf, WindowSettings);
            ButtonHudColor1.onPressed += ButtonHudColor1_onPressed;
            ButtonHudColor2 = new Button(new Vector2(10 + 50 + 40 * 2, 10), "2", wp, sf, WindowSettings);
            ButtonHudColor2.onPressed += ButtonHudColor2_onPressed;
            ButtonHudColor3 = new Button(new Vector2(10 + 50 + 40 * 3, 10), "3", wp, sf, WindowSettings);
            ButtonHudColor3.onPressed += ButtonHudColor3_onPressed;
            ButtonHudColor4 = new Button(new Vector2(10 + 50 + 40 * 4, 10), "4", wp, sf, WindowSettings);
            ButtonHudColor4.onPressed += ButtonHudColor4_onPressed;
            ButtonHudColor5 = new Button(new Vector2(10 + 50 + 40 * 5, 10), "5", wp, sf, WindowSettings);
            ButtonHudColor5.onPressed += ButtonHudColor5_onPressed;
            LabelHudColor = new Label(new Vector2(10, 10 + 40 * 1), "Time format", wp, sf, WindowSettings);
            Button12h = new Button(new Vector2(10 + 50 + 40 * 2, 10 + 40 * 1), "12h", wp, sf, WindowSettings);
            Button12h.onPressed += Button12h_onPressed;
            Button24h = new Button(new Vector2(10 + 50 + 40 * 3, 10 + 40 * 1), "24h", wp, sf, WindowSettings);
            Button24h.onPressed += Button24h_onPressed;

            WindowIngameMenu = new Window(new Vector2(300, 400), "Pause", true, wp, sf, ws) { Visible = false };
            ButtonIngameMenuSettings = new Button(new Vector2(20, 100), "Settings", wp, sf, WindowIngameMenu);
            ButtonIngameMenuSettings.onPressed += ButtonIngameMenuSettings_onPressed;
            ButtonIngameExit = new Button(new Vector2(20, 100 + 30 * 3), "Exit game", wp, sf, WindowIngameMenu);
            ButtonIngameExit.onPressed += new EventHandler(ButtonIngameExit_onPressed);

            WindowMainMenu = new Window(new Vector2(Settings.Resolution.X, Settings.Resolution.Y), "MAIN MENU",
                                        false, wp, sf, ws) { NoBorder = true, Moveable = false };
            LabelMainMenu = new Label(new Vector2(10, 10), @"     __                      
    |__|____ _______  ____  
    |  \__  \\_  __ \/ ___\ 
    |  |/ __ \|  | \/ /_/  >
/\__|  (____  /__|  \___  / 
\______|    \/     /_____/", wp, sf, WindowMainMenu);
            LabelMainVer = new Label(new Vector2(10, LabelMainMenu.Height + 10), Version.GetShort(), wp, sf, Color.Gray, WindowMainMenu);
            WindowMainMenu.CenterComponentHor(LabelMainMenu);
            WindowMainMenu.CenterComponentHor(LabelMainVer);
            ButtonNewGame = new Button(new Vector2(10, 120 + 40 * 1), "New game", wp, sf, WindowMainMenu);
            ButtonNewGame.onPressed += ButtonNewGame_onPressed;
            WindowMainMenu.CenterComponentHor(ButtonNewGame);

            ButtonSettings = new Button(new Vector2(10, 100 + 40 * 5), "Settings", wp, sf, WindowMainMenu);
            WindowMainMenu.CenterComponentHor(ButtonSettings);
            ButtonSettings.onPressed += ButtonIngameMenuSettings_onPressed;
            RunningMotd = new RunningLabel(new Vector2(10, Settings.Resolution.Y / 2 - 50), "Jarg now in early development. It's tottaly free and opensource. Please send your suggestions to ishellstrike@gmail.com or github.com/ishellstrike/roguelikeworkname/issues.", 50, wp, sf, WindowMainMenu);
            WindowMainMenu.CenterComponentHor(RunningMotd);
            LabelControls = new Label(new Vector2(10, Settings.Resolution.Y / 2 + 10), "I-inventory C-caracter page L-event log M-map WASD-moving LMB-shooting F1-debug info" + Environment.NewLine +
                                                                                     "O-statistic P-achievements", wp, sf, WindowMainMenu);
            WindowMainMenu.CenterComponentHor(LabelControls);
            ButtonOpenGit = new Button(new Vector2(10, Settings.Resolution.Y / 2 - 20), "Open in browser", wp, sf, WindowMainMenu);
            ButtonOpenGit.onPressed += ButtonOpenGit_onPressed;
            WindowMainMenu.CenterComponentHor(ButtonOpenGit);

            WindowCaracterCration = new Window(new Vector2(Settings.Resolution.X, Settings.Resolution.Y), "CARACTER CREATION",
                                        false, wp, sf, ws) { NoBorder = true, Moveable = false, Visible = false };
            ButtonCaracterConfirm = new Button(new Vector2(Settings.Resolution.X / 4 * 2, Settings.Resolution.Y / 2 - 20), "Continue", wp, sf, WindowCaracterCration);
            ButtonCaracterConfirm.onPressed += ButtonCaracterConfirm_onPressed;
            ButtonCaracterCancel = new Button(new Vector2(0, Settings.Resolution.Y / 2 - 20), "Cancel", wp, sf, WindowCaracterCration);
            ButtonCaracterCancel.onPressed += ButtonCaracterCancel_onPressed;

            WindowInventory = new Window(new Vector2(Settings.Resolution.X / 2, Settings.Resolution.Y - Settings.Resolution.Y / 10), "Inventory", true, wp, sf, ws) { Visible = false };
            ContainerInventoryItems = new ListContainer(new Rectangle(10, 10, WindowInventory.Locate.Width / 2, WindowInventory.Locate.Height - 40), wp, sf, WindowInventory);
            InventoryMoreInfo = new LabelFixed(new Vector2(WindowInventory.Locate.Width - 200, 40), "", 20, wp, sf, WindowInventory);
            InventorySortAll = new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200), "All", wp, sf, WindowInventory);
            InventorySortAll.onPressed += InventorySortAll_onPressed;
            InventorySortMedicine = new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30), "Medicine", wp, sf, WindowInventory);
            InventorySortMedicine.onPressed += InventorySortMedicine_onPressed;
            InventorySortFood = new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30 * 2), "Food", wp, sf, WindowInventory);
            InventorySortFood.onPressed += InventorySortFood_onPressed;
            IntentoryEquip = new Button(new Vector2(WindowInventory.Locate.Width - 100, WindowInventory.Locate.Height - 200 + 30 * 2), "Equip", wp, sf, WindowInventory);
            IntentoryEquip.onPressed += new EventHandler(IntentoryEquip_onPressed);

            WindowContainer = new Window(new Vector2(Settings.Resolution.X / 2, Settings.Resolution.Y - Settings.Resolution.Y / 10), "Container", true, wp, sf, ws) { Visible = false };
            WindowContainer.SetPosition(new Vector2(Settings.Resolution.X / 2, 0));
            ContainerContainer = new ListContainer(new Rectangle(10, 10, WindowInventory.Locate.Width / 2, WindowInventory.Locate.Height - 40), wp, sf, WindowContainer);
            LabelContainer = new LabelFixed(new Vector2(WindowInventory.Locate.Width - 200, 40), "", 20, wp, sf, WindowContainer);
            ButtonContainerTakeAll = new Button(new Vector2(WindowInventory.Locate.Width - 200, WindowInventory.Locate.Height - 200 + 30 * 2), "Take All (R)", wp, sf, WindowContainer);
            ButtonContainerTakeAll.onPressed += ButtonContainerTakeAll_onPressed;

            WindowEventLog = new Window(new Rectangle(3, 570, (int)Settings.Resolution.X / 3, (int)Settings.Resolution.Y / 4), "Log", true, wp, sf, ws_) { Visible = false, Closable = false, hides = true };
            ContainerEventLog = new ListContainer(new Rectangle(0, 0, (int)Settings.Resolution.X / 3, (int)Settings.Resolution.Y / 4 - 20), wp, sf, WindowEventLog);
            EventLog.onLogUpdate += EventLog_onLogUpdate;

            WindowIngameHint = new Window(new Vector2(50, 50), "HINT", false, wp, sf, ws) { NoBorder = true };
            LabelIngameHint = new Label(new Vector2(10, 3), "a-ha", wp, sf, WindowIngameHint);

            WindowGlobal = new Window(new Vector2(Settings.Resolution.X - 100, Settings.Resolution.Y - 50), "Map", true, wp, sf, ws) { Visible = false };
            ImageGlobal = new Image(new Vector2(10, 10), new Texture2D(GraphicsDevice, 10, 10), Color.White, WindowGlobal);

            int ii = 0;
            WindowCaracter = new Window(new Vector2(Settings.Resolution.X / 2, Settings.Resolution.Y - Settings.Resolution.Y / 10), "Caracter info", true, wp, sf, ws) { Visible = false };
            LabelCaracterHat = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Hat : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterGlaces = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Glaces : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterHelmet = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Helmet : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterChest = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Chest Armor : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterShirt = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Shirt : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterPants = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Pants : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterGloves = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Gloves : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterBoots = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Boots : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterGun = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Ranged Weapon : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterMeele = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Meele Weapon : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterAmmo = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Ammo : ", wp, sf, WindowCaracter); ii++;
            LabelCaracterBag = new DoubleLabel(new Vector2(10, 10 + 15 * ii), "Bag : ", wp, sf, WindowCaracter);
            ii = 0;
            LabelCaracterHp = new DoubleLabel(new Vector2(10 + 300, 10 + 15 * ii), "HP : ", wp, sf, WindowCaracter);

            InfoWindow = new Window(new Vector2(200, 100), "Info", true, wp, sf, ws) { Visible = false };
            InfoWindowLabel = new DoubleLabel(new Vector2(20, 20), "some info", wp, sf, InfoWindow);

            WindowStatist = new Window(new Vector2(Settings.Resolution.X / 3, Settings.Resolution.Y / 3), "Statistic", true, wp, sf, ws) { Visible = false };
            ListStatist = new ListContainer(new Rectangle(0, 0, (int)Settings.Resolution.X / 3, (int)Settings.Resolution.Y / 3 - 20), wp, sf, WindowStatist);
        }

        void ShowInfoWindow(string s1, string s2)
        {
            InfoWindowLabel.Text = s1;
            InfoWindowLabel.Text2 = s2;
            // InfoWindow.CenterComponentHor(InfoWindowLabel);
            InfoWindow.Visible = true;
            InfoWindow.OnTop();
        }

        void HideInfoWindow()
        {
            InfoWindow.Visible = false;
        }

        void IntentoryEquip_onPressed(object sender, EventArgs e)
        {
            inventory_.UseItem(selectedItem, player_);
            UpdateInventoryContainer();
            selectedItem = null;
            InventoryMoreInfo.Text = "";
        }

        void ButtonIngameExit_onPressed(object sender, EventArgs e)
        {
            currentFloor_.SaveAll(this);
        }

        void ButtonContainerTakeAll_onPressed(object sender, EventArgs e)
        {
            inventory_.AddItemRange(inContainer_);
            inContainer_.Clear();
            inventory_.StackSimilar();
            UpdateContainerContainer(inContainer_);
            UpdateInventoryContainer();
        }

        void Button24h_onPressed(object sender, EventArgs e)
        {
            Settings.IsAMDM = false;
        }

        void Button12h_onPressed(object sender, EventArgs e)
        {
            Settings.IsAMDM = true;
        }

        void EventLog_onLogUpdate(object sender, EventArgs e)
        {
            ContainerEventLog.Clear();
            int i = 0;
            foreach (var ss in EventLog.log)
            {
                ContainerEventLog.AddItem(new LabelFixed(Vector2.Zero, ss.message, whitepixel_, font1_, ss.col, 35, ContainerEventLog));
                i++;
            }
            ContainerEventLog.ScrollBottom();
        }

        void InventorySortFood_onPressed(object sender, EventArgs e)
        {
            nowSort_ = ItemType.Food;
            UpdateInventoryContainer();
        }

        void InventorySortMedicine_onPressed(object sender, EventArgs e)
        {
            nowSort_ = ItemType.Medicine;
            UpdateInventoryContainer();
        }

        void InventorySortAll_onPressed(object sender, EventArgs e)
        {
            nowSort_ = ItemType.Nothing;
            UpdateInventoryContainer();
        }

        private ItemType nowSort_ = ItemType.Nothing;
        private List<Item> inInv_ = new List<Item>();
        void UpdateInventoryContainer()
        {
            var a = inventory_.FilterByType(nowSort_);
            inInv_ = a;

            ContainerInventoryItems.Clear();

            int cou = 0;
            foreach (var item in a)
            {
                var i = new LabelFixed(Vector2.Zero, string.Format("{0} x{1}", ItemDataBase.Data[item.Id].Name, item.Count), 22, whitepixel_, font1_, ContainerInventoryItems);
                i.Tag = cou;
                i.onPressed += PressInInventory;
                cou++;
                ContainerInventoryItems.AddItem(i);
            }
        }

        private ItemType nowSortContainer_ = ItemType.Nothing;
        private List<Item> inContainer_ = new List<Item>();
        private Vector2 containerOn = new Vector2();
        void UpdateContainerContainer(List<Item> a)
        {
            inContainer_ = a;

            ContainerContainer.Clear();

            int cou = 0;
            foreach (var item in a)
            {
                var i = new LabelFixed(Vector2.Zero, string.Format("{0} x{1}", ItemDataBase.Data[item.Id].Name, item.Count), 22, whitepixel_, font1_, ContainerContainer);
                i.Tag = cou;
                i.onPressed += PressInContainer;
                cou++;
                ContainerContainer.AddItem(i);
            }
        }

        private Item selectedItem;
        void PressInInventory(object sender, EventArgs e)
        {
            var a = (int)(sender as Label).Tag;
            selectedItem = inInv_[a];

            if (!doubleclick)
            {
                InventoryMoreInfo.Text = ItemDataBase.GetItemFullDescription(inInv_[a]);
            }
            else
            {
                IntentoryEquip_onPressed(null, null);
            }
        }

        private Item ContainerSelected;
        void PressInContainer(object sender, EventArgs e)
        {
            var a = (int)(sender as Label).Tag;
            if (inInv_.Count > a)
            {
                ContainerSelected = inContainer_[a];
                LabelContainer.Text = ItemDataBase.GetItemFullDescription(ContainerSelected);
                if (doubleclick)
                {
                    if (inContainer_.Contains(ContainerSelected))
                    {
                        inventory_.AddItem(ContainerSelected);
                        inContainer_.Remove(ContainerSelected);
                        inventory_.StackSimilar();
                        UpdateInventoryContainer();
                        UpdateContainerContainer(inContainer_);
                    }
                }
            }
        }

        void ButtonCaracterCancel_onPressed(object sender, EventArgs e)
        {
            WindowMainMenu.Visible = true;
            WindowCaracterCration.Visible = false;
        }

        void ButtonCaracterConfirm_onPressed(object sender, EventArgs e)
        {
            WindowCaracterCration.Visible = false;
            DrawAction = GameDraw;
            UpdateAction = GameUpdate;
        }

        void ButtonNewGame_onPressed(object sender, EventArgs e)
        {
            WindowMainMenu.Visible = false;
            WindowCaracterCration.Visible = true;
        }

        void ButtonOpenGit_onPressed(object sender, EventArgs e)
        {
            Process.Start("https://github.com/ishellstrike/roguelikeworkname/issues");
        }

        void ButtonIngameMenuSettings_onPressed(object sender, EventArgs e)
        {
            WindowSettings.Visible = true;
            WindowSettings.OnTop();
        }

        private void ButtonHudColor3_onPressed(object sender, EventArgs e)
        {
            Settings.HudСolor = Color.DarkGray;
        }

        private void ButtonHudColor5_onPressed(object sender, EventArgs e)
        {
            Settings.HudСolor = Color.LightGreen;
        }

        private void ButtonHudColor4_onPressed(object sender, EventArgs e)
        {
            Settings.HudСolor = Color.DarkOrange;
        }

        private void ButtonHudColor2_onPressed(object sender, EventArgs e)
        {
            Settings.HudСolor = Color.LightGray;
        }

        void ButtonHudColor1_onPressed(object sender, EventArgs e)
        {
            Settings.HudСolor = Color.White;
        }

        void CloseAllTestButton_onPressed(object sender, EventArgs e)
        {
            player_.Hunger.Current--;
        }

        private TimeSpan SecondTimespan;
        private void WindowsUpdate(GameTime gt)
        {

            if (WindowStats.Visible)
            {
                StatsHeat.Max = (int)player_.Heat.Max;
                StatsHeat.Progress = (int)player_.Heat.Current;

                StatsJajda.Max = (int)player_.Thirst.Max;
                StatsJajda.Progress = (int)player_.Thirst.Current;

                StatsHunger.Max = (int)player_.Hunger.Max;
                StatsHunger.Progress = (int)player_.Heat.Current;
            }

            if (currentFloor_ != null && WindowMinimap.Visible)
            {
                ImageMinimap.image = currentFloor_.GetMinimap();
            }

            if (WindowCaracter.Visible)
            {
                LabelCaracterHp.Text2 = string.Format("{0}/{1}", player_.Hp.Current, player_.Hp.Max);
            }

            if (SecondTimespan.TotalSeconds >= 1)
            {
                ListStatist.Clear();
                foreach (var statist in Achievements.Stat)
                {
                    if (statist.Value.Count != 0)
                    {
                        ListStatist.AddItem(new Label(Vector2.Zero, statist.Value.Name + ": " + statist.Value.Count,
                                                      whitepixel_, font1_, ListStatist));
                    }
                }
            }
        }

        private void UpdateCaracterWindowItems(object sender, EventArgs eventArgs)
        {
            LabelCaracterGun.Text2 = player_.ItemGun != null ? ItemDataBase.Data[player_.ItemGun.Id].Name : "";
            LabelCaracterHat.Text2 = player_.ItemHat != null ? ItemDataBase.Data[player_.ItemHat.Id].Name : "";
            LabelCaracterAmmo.Text2 = player_.ItemAmmo != null
                                          ? ItemDataBase.Data[player_.ItemAmmo.Id].Name + " x" +
                                            player_.ItemAmmo.Count
                                          : "";
            LabelCaracterPants.Text2 = player_.ItemPants != null
                                           ? ItemDataBase.Data[player_.ItemPants.Id].Name
                                           : "";
            LabelCaracterChest.Text2 = player_.ItemChest != null
                                           ? ItemDataBase.Data[player_.ItemChest.Id].Name
                                           : "";
            LabelCaracterBag.Text2 = player_.ItemBag != null ? ItemDataBase.Data[player_.ItemBag.Id].Name : "";
            LabelCaracterGlaces.Text2 = player_.ItemGlaces != null
                                            ? ItemDataBase.Data[player_.ItemGlaces.Id].Name
                                            : "";
            LabelCaracterHelmet.Text2 = player_.ItemHelmet != null
                                            ? ItemDataBase.Data[player_.ItemHelmet.Id].Name
                                            : "";
            LabelCaracterShirt.Text2 = player_.ItemShirt != null
                                           ? ItemDataBase.Data[player_.ItemShirt.Id].Name
                                           : "";
            LabelCaracterGloves.Text2 = player_.ItemGloves != null
                                            ? ItemDataBase.Data[player_.ItemGloves.Id].Name
                                            : "";
            LabelCaracterBoots.Text2 = player_.ItemBoots != null
                                           ? ItemDataBase.Data[player_.ItemBoots.Id].Name
                                           : "";
            LabelCaracterMeele.Text2 = player_.ItemMeele != null
                                           ? ItemDataBase.Data[player_.ItemMeele.Id].Name
                                           : "";
        }

        #endregion
    }
}