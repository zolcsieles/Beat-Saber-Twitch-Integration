﻿using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace TwitchIntegrationPlugin.Serializables
{
    [Serializable]
    public class Config
    {
        public bool ModOnly;
        public bool SubOnly;
        public int ViewerLimit;
        public int SubLimit;
        public bool ContinueQueue;
        public bool Randomize;
        public int RandomizeLimit;

        public Config(bool modonly, bool subonly, int viewerlimit, int sublimit, bool continuequeue, bool randomize, int randomizelimit)
        {
            ModOnly = modonly;
            SubOnly = subonly;
            ViewerLimit = viewerlimit;
            SubLimit = sublimit;
            ContinueQueue = continuequeue;
            Randomize = randomize;
            RandomizeLimit = randomizelimit;
        }

        public void SaveJson()
        {
            using (FileStream fs = new FileStream("UserData/TwitchIntegrationConfig.json", FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(JsonUtility.ToJson(this, true));
                fs.Write(buffer, 0, buffer.Length);
            }
        }

        public Config LoadFromJson()
        {
            if (File.Exists("UserData/TwitchIntegrationConfig.json"))
            {
                using (FileStream fs = new FileStream("UserData/TwitchIntegrationConfig.json", FileMode.Open, FileAccess.Read))
                {
                    byte[] loadBytes = new byte[fs.Length];
                    fs.Read(loadBytes, 0, (int)fs.Length);
                    Config tempConfig = JsonUtility.FromJson<Config>(Encoding.UTF8.GetString(loadBytes));

                    return tempConfig;
                }
            }

            CreateDefaultConfig();
            return null;
        }

        public static void CreateDefaultConfig()
        {
            new Config(false, false, 3, 5, true, false, 0).SaveJson();
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}