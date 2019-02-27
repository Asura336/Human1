﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 挂在 3C 和 UI 元素的根节点上；
/// 游戏进行中作为全局单例，整个游戏应只初始化一次；
/// 管理场景转换；
/// </summary>
public class LevelManager : MonoBehaviour, IEventListener
{
    private static LevelManager instance = null;
    public static LevelManager Instance
    {
        get { return instance; }
        set { }
    }

    public Transform PlayerTrans { get; set; }

    [HideInInspector] public PlayerAct p_player;
    [HideInInspector] public Controller p_controller;
    [HideInInspector] public SimpleCameraFreeLook p_camera;

    /// <summary>
    /// 保险机制，防止切换场景时玩家落到地面以下
    /// </summary>
    Vector3 _cachePlayerPos;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.FALL_OUT_RANGE, this);
        EventManager.Instance.AddListener(EVENT_TYPE.GET_KEY, this);

        SceneManager.sceneLoaded += OnSceneLoaded;

        p_player = GetComponentInChildren<PlayerAct>();
        p_controller = GetComponentInChildren<Controller>();
        p_camera = GetComponentInChildren<SimpleCameraFreeLook>();
        PlayerTrans = p_player.transform;

        var gi = GlobalHub.Instance;        
        ChangeScene(gi.PlayerScene, gi.PlayerPos, gi.PlayerForward);
    }

    private void OnDestroy()
    {
        // 需要丰富或者修改实现
        GlobalHub.Instance.OnGameSave();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
#if UNITY_EDITOR
        Debug.Log(string.Format("OnSceneLoaded: {0}; mode: {1}", scene.name, mode));
#endif
        EventManager.Instance.RemoveRedundancies();
        p_player.useGravity = true;
        PlayerTrans.position = _cachePlayerPos;

        // 让引擎卸载不使用的资源
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    #region 切换场景函数，由场景中的触发器组件调用


    /// <summary>
    /// 转换到下一个场景
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
        p_player.useGravity = false;
        _cachePlayerPos = PlayerTrans.position;
    }

    readonly WaitForSeconds changeSceneWait = new WaitForSeconds(1);
    IEnumerator _ChangeScene(string sceneName)
    {
        yield return changeSceneWait;
        ChangeScene(sceneName);
    }

    /// <summary>
    /// 转换到下一个场景
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    /// <param name="pos">目标场景下玩家的新位置，表示为世界坐标</param>
    public void ChangeScene(string sceneName, Vector3 pos)
    {
        SceneManager.LoadSceneAsync(sceneName);
        p_player.useGravity = false;
        PlayerTrans.position = pos;
        _cachePlayerPos = pos;
        p_camera.SetAlphaBack();
        p_camera.SetPosForce();
    }

    /// <summary>
    /// 转换到下一个场景
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    /// <param name="pos">目标场景下玩家的新位置，表示为世界坐标</param>
    /// <param name="forward">目标场景下玩家的新朝向，表示为方向</param>
    public void ChangeScene(string sceneName, Vector3 pos, Vector3 forward)
    {
        SceneManager.LoadSceneAsync(sceneName);
        p_player.useGravity = false;
        PlayerTrans.position = pos;
        _cachePlayerPos = pos;
        forward.y = 0;
        p_player.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        p_camera.SetAlphaBack();
        p_camera.SetPosForce();
    }

    #endregion

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.GET_KEY:
                GlobalHub.Instance.Url2Point["BKeyFlag"] |= 1 << (int)param;
                switch ((COLOR_TYPE)param)
                {
                    case COLOR_TYPE.RED:  // red
                        StartCoroutine(_ChangeScene("Level_white_0"));
                        break;
                    default:
                        Debug.LogError("Key's value out of range.", this);
                        return;
                }
                break;
            case EVENT_TYPE.FALL_OUT_RANGE:
                ChangeScene(GlobalHub.initPlayerScene,
                            GlobalHub.initPlayerPos, 
                            GlobalHub.initPlayerForward);
                break;
            default: return;
        }
    }
}
