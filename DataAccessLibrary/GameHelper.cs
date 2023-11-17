﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace DataAccessLibrary
{
    public class GameInfo
    {
        /// <summary>
        /// 游戏名（非进程名，但在游戏名未知的情况下先使用进程名替代）
        /// </summary>
        public string GameName { get; set; }

        /// <summary>
        /// 游戏文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 游戏ID
        /// </summary>
        public Guid GameID { get; set; }

        /// <summary>
        /// 翻译模式,1=hook 2=ocr
        /// </summary>
        public int TransMode { get; set; }

        /// <summary>
        /// 源语言代码，同翻译API中语言代码
        /// </summary>
        public string SrcLang { get; set; }

        /// <summary>
        /// 目标语言代码，同翻译API中语言代码
        /// </summary>
        public string DstLang { get; set; }

        /// <summary>
        /// 去重方法，仅在hook模式有效
        /// </summary>
        public string RepairFunc { get; set; }

        /// <summary>
        /// 去重方法所需参数1，仅在hook模式有效
        /// </summary>
        public string RepairParamA { get; set; }

        /// <summary>
        /// 去重方法所需参数2，仅在hook模式有效
        /// </summary>
        public string RepairParamB { get; set; }

        /// <summary>
        /// 特殊码值，仅在hook模式有效
        /// </summary>
        public string HookCode { get; set; }

        /// <summary>
        /// 用户自定的特殊码值，如果用户这一项不是自定义的，那么应该为'NULL'，仅在hook模式有效，注意下次开启游戏时这里就需要注入一下
        /// </summary>
        public string HookCodeCustom { get; set; }

        /// <summary>
        /// 检查是否是64位应用程序
        /// </summary>
        public bool Isx64 { get; set; }

        /// <summary>
        /// 包含hook地址信息的本软件特有的MisakaHookCode
        /// </summary>
        public string MisakaHookCode { get; set; }
    }

    public static class GameHelper
    {
        //游戏信息文件夹
        public static readonly DirectoryInfo directory = Directory.CreateDirectory($"{Environment.CurrentDirectory}\\data\\games\\");

        /// <summary>
        /// 根据id获得游戏信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static GameInfo GetGameById(Guid? id)
        {
            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                string jsonString = File.ReadAllText(fileInfo.FullName);
                GameInfo gameInfo = JsonSerializer.Deserialize<GameInfo>(jsonString)!;
                if (gameInfo.GameID == id)
                {
                    return gameInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// 得到一个游戏的GameInfo
        /// 如果游戏已经存在，则直接返回GameInfo，否则追加新游戏路径并返回新GameInfo
        /// </summary>
        public static GameInfo GetGameByPath(string gamePath)
        {
            (bool existed, GameInfo gameInfo) = GameExists(gamePath);
            if (existed && gameInfo != null)
            {
                return gameInfo;
            }
            else
            {
                return AddGame(gamePath);
            }
        }

        private static (bool, GameInfo) GameExists(string gamePath)
        {
            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                string jsonString = File.ReadAllText(fileInfo.FullName);
                GameInfo gameInfo = JsonSerializer.Deserialize<GameInfo>(jsonString)!;
                if (gameInfo.FilePath == gamePath)
                {
                    return (true, gameInfo);
                }
            }
            return (false, null);
        }

        private static GameInfo AddGame(string gamePath)
        {
            string name = Path.GetFileNameWithoutExtension(gamePath);
            Guid id = Guid.NewGuid();
            GameInfo gameInfo = new GameInfo()
            {
                GameID = id,
                GameName = name,
                FilePath = gamePath,
            };
            SaveGameInfo(gameInfo);
            return gameInfo;
        }


        /// <summary>
        /// 得到游戏库中所有有效游戏的信息
        /// </summary>
        public static List<GameInfo> GetAllCompletedGames()
        {
            List<GameInfo> list = new List<GameInfo>();
            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                string jsonString = File.ReadAllText(fileInfo.FullName);
                GameInfo gameInfo = JsonSerializer.Deserialize<GameInfo>(jsonString)!;
                if (string.IsNullOrEmpty(gameInfo.SrcLang) ||
                    string.IsNullOrEmpty(gameInfo.DstLang) ||
                    string.IsNullOrEmpty(gameInfo.RepairFunc) ||
                    string.IsNullOrEmpty(gameInfo.HookCode)
                    )
                {
                    continue;
                }
                list.Add(gameInfo);
            }
            return list;
        }



        /// <summary>
        /// 删除游戏信息
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static bool DeleteGameByID(Guid gameID)
        {
            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                string jsonString = File.ReadAllText(fileInfo.FullName);
                GameInfo gameInfo = JsonSerializer.Deserialize<GameInfo>(jsonString)!;
                if (gameInfo.GameID == gameID)
                {
                    File.Delete(fileInfo.FullName);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 修改游戏名信息
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool UpdateGameNameByID(Guid gameID, string name)
        {
            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                string jsonString = File.ReadAllText(fileInfo.FullName);
                GameInfo gameInfo = JsonSerializer.Deserialize<GameInfo>(jsonString)!;
                if (gameInfo.GameID == gameID)
                {
                    File.Delete(fileInfo.FullName);
                    gameInfo.GameName = name;
                    SaveGameInfo(gameInfo);
                    return true;
                }
            }
            return false;
        }

        private static JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };
        /// <summary>
        /// 保存GameInfo
        /// </summary>
        /// <param name="gameInfo"></param>
        /// <returns></returns>
        public static void SaveGameInfo(GameInfo gameInfo)
        {
            string fileName = $"{directory.FullName}\\{gameInfo.GameName}.json";
            string jsonString = JsonSerializer.Serialize(gameInfo, options);
            File.WriteAllText(fileName, jsonString);
        }
    }
}