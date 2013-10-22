﻿using System;
using System.Collections.Generic;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Generation.Names {
    public enum NameType {
        City,
        Male,
        Fermale,
        Unisex
    }

    public class NameClass {
        public string Name;
        public NameType NameType;
    }

    public class NameDataBase {
        public static List<NameClass> data = new List<NameClass>();


        public NameDataBase() {
            List<object> temp = ParsersCore.UniversalParseDirectory(Settings.GetNamesDataDirectory(),
                                                                    UniversalParser.NoIdParser<NameClass>);

            foreach (object o in temp) {
                data.Add((NameClass) o);
            }
        }

        public static string GetRandom(Random rnd) {
            return data[rnd.Next(0, data.Count + 1)].Name;
        }
    }
}