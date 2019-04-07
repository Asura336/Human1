﻿using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SerializeTool
{
    /// <summary>
    /// 序列化字段到文件
    /// </summary>
    /// <param name="dataSource">数据源</param>
    /// <param name="file">文件名</param>
    public static void ToFile(FormatSaveFile dataSource,string path, string fileName)
    {
        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
        var file = path + @"\" + fileName;
        using (StreamWriter sw = File.CreateText(file))
        {
            var scene = dataSource.scene;
            var pos = dataSource.pos;
            var euler = dataSource.euler;

            sw.WriteLine($"Scene {scene}");
            sw.WriteLine($"Pos {pos.x} {pos.y} {pos.z}");
            sw.WriteLine($"Euler {euler.x} {euler.y} {euler.z}");

            var cache = dataSource.cache;
            foreach (var pair in cache)
            {
                sw.WriteLine($"Pair {pair.Key} {pair.Value}");
            }
        }
    }

    /// <summary>
    /// 从存档文件到内存，没有安全校验
    /// </summary>
    /// <param name="path">完整的存档文件路径</param>
    /// <returns></returns>
    public static FormatSaveFile ToObj(string path)
    {
        FormatSaveFile reserve = new FormatSaveFile
        {
            cache = new Dictionary<string, int>()
        };
        using (StreamReader sr = File.OpenText(path))
        {
            string s = string.Empty;
            while ((s = sr.ReadLine()) != null)
            {
                var line = s.Split(' ');
                if (line[0].Equals("Scene")) { reserve.scene = line[1]; }
                if (line[0].Equals("Pos"))
                {
                    reserve.pos = new Vector3(
                        float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3]));
                }
                if (line[0].Equals("Euler"))
                {
                    reserve.euler = new Vector3(
                        float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3]));
                }
                if (line[0].Equals("Pair"))
                {
                    reserve.cache.Add(line[1], int.Parse(line[2]));
                }
            }
        }
        return reserve;
    }
}

public struct FormatSaveFile
{
    public string scene;
    public Vector3 pos;
    public Vector3 euler;
    public Dictionary<string, int> cache;
}