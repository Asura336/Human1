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
    public Material p_playerMaterial;  // 在类外赋值

    COLOR_TYPE _playerColorType = COLOR_TYPE.NULL;
    /// <summary>
    /// 玩家颜色的枚举，改变枚举即变换颜色
    /// </summary>
    public COLOR_TYPE PlayerColorType
    {
        get { return _playerColorType; }
        set
        {
            if (_playerColorType != value)
            {
                _playerColorType = value;
                Url2Point["Player"] = (int)value;
                p_playerMaterial.color = colorTypes[value];
            }
        }
    }

    public int GlobalKeyFlag
    {
        get { return Url2Point["BKeyFlag"]; }
        set
        {
            if (Url2Point.ContainsKey("BKeyFlag"))
            { Url2Point["BKeyFlag"] = value; }
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
        {COLOR_TYPE.GREEN, Color.green},
        {COLOR_TYPE.BLUE, Color.blue}
    };

    /// <summary>
    /// 保存声音文件的容器，成员下标与 <see cref="SOUND"/> 枚举对应
    /// 初始化在 <see cref="GameInit"/>
    /// </summary>
    public AudioClip[] SoundClips { get; private set; }

    public static readonly Vector3 initPlayerPos = new Vector3(1.5f, 0, -2.5f);
    public static readonly Vector3 initPlayerForward = new Vector3(0, 0, 1);
    public const string initPlayerScene = "Level0";
    public const string savePath = @"D:\Human1Save";
    public const string saveFileName = @"Save.svsc";


    /// <summary>
    /// 事件指针指向台词节点
    /// </summary>
    public Dictionary<int, TalkNode> Point2TextNode { get; private set; }
    public Dictionary<string, UiTextNode> UiTexts { get; private set; }

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
        string nodeText = Resources.Load<TextAsset>("Text/point2TextNode").text;
        TalkNode[] nodeArray = JsonTool.ToJsonArray<TalkNode>(nodeText);
        Point2TextNode = new Dictionary<int, TalkNode>();
        foreach (var node in nodeArray) { Point2TextNode[node.id] = node; }

        string uiTextRaw = Resources.Load<TextAsset>("Text/uiTexts").text;
        UiTextNode[] uiTextArray = JsonTool.ToJsonArray<UiTextNode>(uiTextRaw);
        UiTexts = new Dictionary<string, UiTextNode>();
        foreach (var text in uiTextArray) { UiTexts.Add(text.key, text); }

        SoundClips = new AudioClip[]    
        {
            Resources.Load<AudioClip>("Sound/EnteractNull"),  // 0
            Resources.Load<AudioClip>("Sound/EnteractSuccess"),  // 1
            Resources.Load<AudioClip>("Sound/EnteractFail"),  // 2
            Resources.Load<AudioClip>("Sound/Land"),  // 3
            Resources.Load<AudioClip>("Sound/StepButtonAudio"),  // 4
            Resources.Load<AudioClip>("Sound/MazePointOut"),  // 5
        };

        // 固定随机数种子
        var r = new System.Random();
        MazeSeed = r.Next();

        // TODO:
        // 从序列化文件建立玩家信息和有唯一标识的物件 Point 预设值
        //CreateInitSaveFile();  // 调试用，建立存档系统后此函数与类似函数应在外部调用
        ReadSaveFile();
    }

    /// <summary>
    /// 外部调用的存档命令
    /// </summary>
    public void OnGameSave()
    {
        // TODO:
        // 序列化文件存档的动作
        if (LevelManager.Instance != null)
        {
            var player = LevelManager.Instance.PlayerTrans;
            PlayerPos = player.position;
            PlayerForward = player.forward;
        }
        SerializeTool.ToFile(
            new FormatSaveFile
            {
                scene = PlayerScene,
                pos = PlayerPos,
                euler = PlayerForward,
                cache = Url2Point
            },
            savePath, saveFileName);
    }

    /// <summary>
    /// 写入初始化游戏进度存档
    /// </summary>
    public void CreateInitSaveFile()
    {
        PlayerPos = initPlayerPos;
        PlayerForward = initPlayerForward;
        PlayerScene = initPlayerScene;
        Url2Point = new Dictionary<string, int>()
        {
            {"Player", (int)COLOR_TYPE.NULL},
            {"BKeyFlag", 0}
        };

        FormatSaveFile saveFile = new FormatSaveFile
        {
            scene = PlayerScene,
            pos = PlayerPos,
            euler = PlayerForward,
            cache = Url2Point
        };
        SerializeTool.ToFile(saveFile, savePath, saveFileName);
    }

    /// <summary>
    /// 保存游戏进度，没有安全校验
    /// </summary>
    public void ReadSaveFile()
    {
        var obj = SerializeTool.ToObj(savePath + @"\" + saveFileName);
        PlayerPos = obj.pos;
        PlayerForward = obj.euler;
        PlayerScene = obj.scene;
        Url2Point = obj.cache;
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
    GREEN,
    BLUE
}

/// <summary>
/// 对话信息节点
/// </summary>
[Serializable]
public class TalkNode
{
    public int id;
    public int next;
    public string talkStrs;
    public string nodeCall = "";

    public override string ToString()
    {
        return string.Format("\"id\" : {0}, \"next\" : {1}, \"talkStrs\" : {2}, \"nodeCall\" : {3}",
            id, next, talkStrs, nodeCall);
    }
}

/// <summary>
/// UI 使用的文字内容
/// </summary>
[Serializable]
public class UiTextNode
{
    public string key;
    public string valueChs;

    public string GetValue(string language = "chs")
    {
        if (language.Equals("chs"))
        {
            return valueChs;
        }
        return valueChs;
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

