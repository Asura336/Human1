using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 位于游戏全局的单例
/// 存储游戏事件开关，需要广播的实体引用等
/// </summary>
public class GlobalHub
{
    #region 单例标记和构造函数

    private static GlobalHub _instance = null;
    public static GlobalHub Instance
    {
        get
        {
            if (_instance == null) { _instance = new GlobalHub(); }
            return _instance;
        }
    }

    private GlobalHub()
    {
        // 初始化操作
        GameInit();
    }

    #endregion

    /// <summary>
    /// 全局管理器记录的在激活的互动物件的引用
    /// </summary>
    public IEnteract p_enteract;  // 在类外赋值
    /// <summary>
    /// 玩家的材质引用
    /// </summary>
    public Renderer p_playerRenderer;  // 在类外赋值

    COLOR_TYPE _playerColorType = COLOR_TYPE.NULL;
    /// <summary>
    /// 玩家颜色的枚举，改变枚举即变换颜色
    /// </summary>
    public COLOR_TYPE PlayerColorType
    {
        get { return _playerColorType; }
        set
        {
            if (p_playerRenderer != null)
            {
                _playerColorType = value;
                p_playerRenderer.material.color = colorTypes[value];
            }
        }
    }

    /// <summary>
    /// 颜色枚举量与颜色的映射关系
    /// </summary>
    public readonly Dictionary<COLOR_TYPE, Color> colorTypes = new Dictionary<COLOR_TYPE, Color>
    {
        {COLOR_TYPE.NULL, Color.white},
        {COLOR_TYPE.RED, Color.red},
        {COLOR_TYPE.YELLOW, Color.yellow},
        {COLOR_TYPE.BLUE, Color.blue},
        {COLOR_TYPE.GREEN, Color.green}
    };

    public static readonly Vector3 initPlayerPos = new Vector3(1.5f, 0, -2.5f);
    public static readonly Vector3 initPlayerForward = new Vector3(0, 0, 1);
    public const string initPlayerScene = "Level0";

    /// <summary>
    /// 事件指针指向台词节点
    /// </summary>
    public Dictionary<int, TalkNode> point2TalkNode;

    /// <summary>
    /// 拥有唯一标识的物体，存放一份状态信息的缓存
    /// </summary>
    public Dictionary<string, int> Url2Point { get; set; }

    public Vector3 PlayerPos { get; set; }
    public Vector3 PlayerForward { get; set; }
    public string PlayerScene { get; set; }
    public int MazeSeed { get; private set; } = 0;

    /// <summary>
    /// 初始化操作，包括读取存档的步骤
    /// </summary>
    void GameInit()
    {
        // 读入静态信息
        string nodeText = Resources.Load<TextAsset>("Text/point2TalkNode").text;
        TalkNode[] nodeArray = JsonTool.ToJsonArray<TalkNode>(nodeText);
        point2TalkNode = new Dictionary<int, TalkNode>();
        foreach (var node in nodeArray) { point2TalkNode[node.id] = node; }

        // 固定随机数种子
        var r = new System.Random();
        MazeSeed = r.Next();

        // TODO:
        // 从序列化文件建立玩家信息和有唯一标识的物件 Point 预设值
        PlayerColorType = COLOR_TYPE.NULL;
        PlayerPos = initPlayerPos;
        PlayerForward = initPlayerForward;
        PlayerScene = initPlayerScene;
        Url2Point = new Dictionary<string, int>();
    }

    public void OnGameSave()
    {
        // TODO:
        // 序列化文件存档的动作
    }
}

/// <summary>
/// 颜色种类的枚举，映射方式定义于 <see cref="GlobalHub"/>
/// </summary>
public enum COLOR_TYPE
{
    NULL,
    RED,
    YELLOW,
    BLUE,
    GREEN
}

[Serializable]
public class TalkNode
{
    public int id;
    public int next;
    public string talkStrs;
    public string nodeCall = "";

    public override string ToString()
    {
        return String.Format("\"id\" : {0}, \"next\" : {1}, \"talkStrs\" : {2}, \"nodeCall\" : {3}",
            id, next, talkStrs, nodeCall);
    }
}

// https://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/#post-2585129
public static class JsonTool
{
    /// <summary>
    /// 从 Json 文件字符串返回对象数组
    /// </summary>
    /// <typeparam name="T">反序列化类型</typeparam>
    /// <param name="jsonStr">Json 文件内容，可以为 Json 数组</param>
    /// <returns></returns>
    public static T[] ToJsonArray<T>(string jsonStr)
    {
        string newJson = "{ \"array\": " + jsonStr + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    /// <summary>
    /// 反序列化使用的包装器
    /// </summary>
    /// <typeparam name="T">反序列化类型</typeparam>
    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

