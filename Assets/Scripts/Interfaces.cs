using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物理层面的互动行为，踩踏板、门等
/// </summary>
public interface IPhysicsInteract
{
    /// <summary>
    /// 状态切换开始
    /// </summary>
    void StateChange();
    /// <summary>
    /// 状态重置
    /// </summary>
    void StateReset();
}

/// <summary>
/// 玩家走近后可按键调查的物品
/// </summary>
public interface IEnteract
{
    /// <summary>
    /// 活动类型，如颜色变换或者可弹出文字显示框
    /// </summary>
    ENTERACT_TYPE EnteractType { get; set; }
    /// <summary>
    /// 表示为颜色枚举或者事件列表指针
    /// </summary>
    int Point { get; set; }
    /// <summary>
    /// 物体的唯一标识
    /// </summary>
    string Url { get; set; }
    void ActAwake();
    int ActDo();
    void ActClose();
}
/// <summary>
/// 可交互物体的类型。变换颜色、弹出台词等
/// </summary>
public enum ENTERACT_TYPE
{
    NULL,
    CLR_EXC,
    CLR_SPR,
    TLK
}

/// <summary>
/// 互动音效的枚举
/// </summary>
public enum SOUND
{
    ENTERACT_NULL,
    ENTERACT_SUCCEED,
    ENTERACT_FAIL,
    STEP
}

/// <summary>
/// UI 节点侦听器
/// </summary>
public interface IEventListener
{
    void OnEvent(EVENT_TYPE eventType, Component sender, object param = null);
}
