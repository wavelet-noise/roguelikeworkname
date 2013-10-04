using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Generation;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class MapSector {
        /// <summary>
        /// Sector ox size
        /// </summary>
        public const int Rx = 16;

        /// <summary>
        /// Sectro oy size
        /// </summary>
        public const int Ry = 16;

        public bool ready;

        public GameLevel Parent;
        private BackgroundWorker bw;

        public int SectorOffsetX, SectorOffsetY;

        internal List<IBlock> Blocks;
        internal Floor[] Floors;
        internal List<ICreature> creatures;
        internal List<Particle> decals;
        internal SectorBiom biom;

        internal List<Light> lights; 

        public MapSector(int sectorOffsetX, int sectorOffsetY) {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;

            Blocks = new List<IBlock>(Rx * Ry);
            Floors = new Floor[Rx * Ry];
            creatures = new List<ICreature>();

            int i = Rx * Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks[i] = new Block();
            }
        }

        public MapSector(GameLevel parent, int sectorOffsetX, int sectorOffsetY)
        {
            SectorOffsetX = sectorOffsetX;
            SectorOffsetY = sectorOffsetY;
            Parent = parent;

            Blocks = new List<IBlock>(Rx * Ry);
            Floors = new Floor[Rx * Ry];
            creatures = new List<ICreature>();
            decals = new List<Particle>();

            int i = Rx * Ry;
            while (i-- != 0) {
                Floors[i] = new Floor();
                Blocks.Add(new Block());
            }
            lights = new List<Light>();
        }

        /// <summary>
        /// Map from just serialized data
        /// </summary>
        /// <param name="bdb"></param>
        /// <param name="fdb"></param>
        /// <param name="sdb"></param>
        /// <param name="sectorOffsetX"></param>
        /// <param name="sectorOffsetY"></param>
        /// <param name="blocksArray"></param>
        /// <param name="floorsArray"></param>
        public MapSector(GameLevel parent, object sectorOffsetX, object sectorOffsetY, object blocksArray, object floorsArray, object obiom, object creat, object decal)
        {
            SectorOffsetX = (int)sectorOffsetX;
            SectorOffsetY = (int)sectorOffsetY;
            Parent = parent;

            Blocks = blocksArray as List<IBlock>;
            Floors = floorsArray as Floor[];
            biom = (SectorBiom)obiom;
            creatures = (List<ICreature>)creat;
            decals = (List<Particle>)decal;
            lights = new List<Light>();
            ResetLightingSources();

            foreach (var block in Blocks) {
                block.Source = BlockData.GetSource(block.MTex);
            }
            foreach (var floor in Floors) {
                floor.Source = FloorData.GetSource(floor.MTex);
            }
        }

        public List<StorageBlock> GetStorageBlocks() {
            List<StorageBlock> list = new List<StorageBlock>();
            for (int i = 0; i < Blocks.Count; i++) {
                IBlock a = Blocks[i];
                if (a.Id != "0" && a.Data.Prototype == typeof (StorageBlock)) {
                    list.Add(a as StorageBlock);
                }
            }
            return list;
        }

        public void ResetLightingSources() {
            lights.Clear();
            int i = 0;
            for (int index = 0; index < Blocks.Count; index++) {
                var block = Blocks[index];
                if (block is ILightSource) {
                    lights.Add(GetLights(block, i/Ry*32 + SectorOffsetX*Rx*32, i%Ry*32 + SectorOffsetY*Ry*32));
                }
                i++;
            }
        }

        private Light GetLights(IBlock block, int x, int y) {
            var a1 = block as ILightSource;
            var t = new Light {
                                  Color = a1.LightColor, LightRadius = a1.LightRange, Power = a1.LightPower, Position = new Vector3(x+16, y, 1+32)
                              };
            return t;
        }

        public void ExploreAllSector()
        {
            foreach (var b in Blocks) {
                b.Explored = true;
                b.Lightness = Color.White;
            }
        }

        /// <summary>
        /// Main sector generation proc
        /// </summary>
        /// <param name="mapseed">���������� ��� �����</param>
        public void Rebuild(int mapseed) {
            Action<int> a = AsyncGeneration;
            a(mapseed);
        }

        /// <summary>
        /// ###---### Main generation proc ###---###
        /// </summary>
        /// <param name="mapseed">Global map seed</param>
        private void AsyncGeneration(int mapseed) {
            int s = (int) (MapGenerators.Noise2D(SectorOffsetX, SectorOffsetY)*int.MaxValue);
            Random rand = new Random(s);

            biom = GetBiom(SectorOffsetX, SectorOffsetY, Parent);

            MapGenerators.FillTest1(this, "1");
            MapGenerators.ClearBlocks(this);
            MapGenerators.FloorPerlin(this);

            switch (biom) {
                case SectorBiom.Bushland:
                    int next = rand.Next(1, 3);
                    for (int i = 0; i < next; i++ ) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "bbochka");
                    break;

                case SectorBiom.Forest:
                    var aa = rand.Next(5, 10);
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    break;

                case SectorBiom.WildForest:
                    aa = rand.Next(20, 40);
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    break;

                case SectorBiom.SuperWildForest:
                    aa = rand.Next(60, 80);
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "17");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustsmall");
                    for (int i = 0; i < aa; i++) SetBlock(rand.Next(0, Rx - 1), rand.Next(0, Ry - 1), "kustbig");
                    break;

                case SectorBiom.House:
                    MapGenerators.PlaceRandomSchemeByType(this, SchemesType.house, rand.Next(0, Rx - 1),
                                                          rand.Next(0, Ry - 1), rand);
                    break;

                case SectorBiom.RoadHevt:
                case SectorBiom.RoadCross:
                case SectorBiom.RoadHor:
                    MapGenerators.GenerateRoad(this, biom);
                    break;
            }

            var sb = GetStorageBlocks();

            foreach (var block in sb) {
                int next = rand.Next(0, 3);
                for (int i = 0; i < next; i++) {
                    block.StoredItems.Add(new Item.Item(ItemDataBase.Data.ElementAt(rand.Next(0, ItemDataBase.Data.Count)).Key, rand.Next(1, 2)));
                }
            }
            var rnd1 = rand.Next(0, 5);
            var rnd2 = rand.Next(0, 3);
            for (int i = 1; i< rnd1; i++ ) {
                Spawn("zombie1", rand);
            }
            for (int i = 1; i < rnd2; i++)
            {
                Spawn("hdzombie", rand);
            }

            ResetLightingSources();

            Parent.generated++;
            ready = true;
        }

        public static SectorBiom GetBiom(int offX, int offY, GameLevel gl) {
            SectorBiom biom;
            int s = (int)(MapGenerators.Noise2D(offX, offY) * int.MaxValue);
            Random rand = new Random(s);
            var most = MapGenerators.GetMost(offX, offY);

            biom = (SectorBiom)rand.Next(0, Enum.GetNames(typeof(SectorBiom)).Length - 3);

            if (most == "1")
            {
                if (biom == SectorBiom.Forest || biom == SectorBiom.WildForest || biom == SectorBiom.SuperWildForest)
                {
                    biom = SectorBiom.Field;
                }
            }

            if(gl.RoadSectors.Contains(new Tuple<int, int>(offX, offY))) {
                biom = SectorBiom.RoadCross;
            }

            return biom;
        }

        public void Spawn(string i, Random rnd) {
            Spawn(i, rnd.Next(0, Rx), rnd.Next(0, Ry));
        }

        public void Spawn(string i, int x, int y) {
            var n = (ICreature) Activator.CreateInstance(MonsterDataBase.Data[i].Prototype);
            n.Position = new Vector2(x*32, y*32);
            n.Id = i;
            creatures.Add(n);
        }

        public IBlock GetBlock(int x, int y)
        {
            return Blocks[x * Ry + y];
        }

        public IBlock GetBlock(int a)
        {
            return Blocks[a];
        }

        public void SetFloor(int x, int y, string id)
        {
            Floors[x * Ry + y].Id = id;
            Floors[x * Ry + y].Source = FloorDataBase.Data[id].RandomMtexFromAlters(ref Floors[x * Ry + y].MTex);
        }

        public string GetId(int x, int y)
        {
            return Blocks[x * Ry + y].Id;
        }

        public string GetId(int a)
        {
            return Blocks[a].Id;
        }

        /// <summary>
        /// Base standart block setter
        /// </summary>
        /// <param name="oneDimCoord"></param>
        /// <param name="id"></param>
        public void SetBlock(int oneDimCoord, string id) {
            Blocks[oneDimCoord] = (IBlock)Activator.CreateInstance(BlockDataBase.Data[id].Prototype);
            Blocks[oneDimCoord].Id = id;
            ((Block)Blocks[oneDimCoord]).data = BlockDataBase.Data[id];
            string mTex ="";
            Blocks[oneDimCoord].Source = BlockDataBase.Data[id].RandomMtexFromAlters(ref mTex);
            Blocks[oneDimCoord].MTex = mTex;
        }

        /// <summary>
        /// Base advansed block setter
        /// </summary>
        /// <param name="oneDimCoord"></param>
        /// <param name="id"></param>
        public void SetBlock(int oneDimCoord, IBlock bl)
        {
            Blocks[oneDimCoord] = bl;
            ((Block)Blocks[oneDimCoord]).data = BlockDataBase.Data[bl.Id];
            string mTex ="";
            Blocks[oneDimCoord].Source = bl.Data.RandomMtexFromAlters(ref mTex);
            Blocks[oneDimCoord].MTex = mTex;
        }

        public void SetBlock(int posX, int posY, string id)
        {
            SetBlock(posX * Ry + posY, id);
        }

        public void SetBlock(Vector2 pos, string id)
        {
            SetBlock((int)pos.X, (int)pos.Y, id);
        }

        public void OpenCloseDoor(int x, int y)
        {
            if (Blocks[x * Rx + y].Data.SmartAction == SmartAction.ActionOpenClose)
            {
                SetBlock(x, y, Blocks[x * Ry + y].Data.AfterDeathId);
            }
        }

        public void CreateAllMapFromArray(string[] arr)
        {
            for (int i = 0; i < Rx; i++)
            {
                for (int j = 0; j < Ry; j++)
                {
                    SetBlock(i, j, arr[i]);
                }
            }
        }

        public void AddDecal(Particle particle) {
            decals.Add(particle);
            if(decals.Count > 256) {
                decals.RemoveAt(0);
            }
        }
    }
}