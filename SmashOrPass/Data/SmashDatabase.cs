﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using Newtonsoft.Json;

namespace SmashOrPass.Data
{
    public static class SmashDatabase
    {
        public static Dictionary<ulong, UserEntry> Data { get; private set; }

        private static Timer timer = new Timer(20000);
        private static string currentDir;

        public static void Init()
        {
            currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SoPDB.json";
            Console.WriteLine("[Info][SmashDB] DB save path: " + currentDir);

            if (File.Exists(currentDir))
            {
                Console.WriteLine("[Info][SmashDB] Restoring data from db file");
                Data = JsonConvert.DeserializeObject<Dictionary<ulong, UserEntry>>(
                    File.ReadAllText(currentDir));
            }
            else
            {
                File.WriteAllText(currentDir, string.Empty);
                Data = new Dictionary<ulong, UserEntry>();
            }

            timer.Elapsed += SaveToFile;
            timer.Start();
            Console.WriteLine("[Info][SmashDB] Initialized");
        }

        public static void AddEntry(UserEntry entry)
        {
            if (Data.ContainsKey(entry.Id))
            {
                Data[entry.Id] = entry;
            }
            else
            {
                Data.Add(entry.Id, entry);
            }

            Console.WriteLine($"[Info][SmashDB] Added user {entry.Id}");
        }

        public static UserEntry GetEntry(ulong id)
        {
            return Data.ContainsKey(id) ? Data[id] : null;
        }

        public static bool HasUser(ulong id)
        {
            return Data.ContainsKey(id);
        }

        public static void UpdateEntry(UserEntry entry)
        {
            Data[entry.Id] = entry;
            Console.WriteLine($"[Info][SmashDB] Updated user: {entry.Id} (S: {entry.Smashes}, P: {entry.Passes})");
        }

        public static void RemoveEntry(ulong id)
        {
            if (Data.ContainsKey(id))
            {
                Data.Remove(id);
            }
        }

        private static void SaveToFile(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (Data)
            {
                string json = JsonConvert.SerializeObject(Data);

                if (json != File.ReadAllText(currentDir))
                {
                    File.WriteAllText(currentDir, json);
                    Console.WriteLine("[Info][SmashDB] Saved db");
                }
            }
        }
    }
}
