﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    public class SchemesDataBase {
        public static List<Schemes> Data;
        public static List<Schemes> Houses;
        public static List<Schemes> Storages;
        public static List<Schemes> NormalCity; 

        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        public SchemesDataBase() {
            Data = new List<Schemes>();
            List<Schemes> a = ParsersCore.ParseDirectory(Directory.GetCurrentDirectory() + @"\Content\Data\Schemes\",
                                                         ChemesParser.Parser);
            foreach (Schemes pair in a) {
                Data.Add(pair);
            }

            Houses = Data.Where(x => x.type == SchemeSortType.House).ToList();
            Storages = Data.Where(x => x.type == SchemeSortType.Storage).ToList();
            NormalCity =
                Data.Where(
                    x => x.type == SchemeSortType.House || x.type == SchemeSortType.Shop || x.type == SchemeSortType.Hospital || x.type == SchemeSortType.WearShop).
                    ToList();
        }
    }
}