using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局事件管理器
/// </summary>
public class EventManager
{
    private static EventManager _instance = null;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null) { _instance = new EventManager(); }
            return _instance;
        }
    }
    private Dictionary<EVENT_TYPE, List<IEventListener>> listeners;
    private EventManager()
    {
        listeners = new Dictionary<EVENT_TYPE, List<IEventListener>>();
    }

    /// <summary>
    /// 向管理器注册事件
    /// </summary>
    /// <param name="eventType">事件类型枚举</param>
    /// <param name="listener">监听器的引用</param>
    public void AddListener(EVENT_TYPE eventType, IEventListener listener)
    {
        List<IEventListener> listenerList = null;
        if (listeners.TryGetValue(eventType, out listenerList))
        {
            listenerList.Add(listener);
            return;
        }
        listenerList = new List<IEventListener> { listener };
        listeners[eventType] = listenerList;
    }

    /// <summary>
    /// 发送消息给管理器，由所有监听器接收
    /// </summary>
    /// <param name="eventType">事件种类枚举</param>
    /// <param name="sender">事件的发送方</param>
    /// <param name="param">可选参数</param>
    public void PostNotification(EVENT_TYPE eventType, Component sender, object param = null)
    {
        if (!listeners.TryGetValue(eventType, out List<IEventListener> listenerList)) { return; }
        foreach (var listener in listenerList)
        {
            if (listener != null) { listener.OnEvent(eventType, sender, param); }
        }
    }

    /// <summary>
    /// 从列表移除某类型的事件
    /// </summary>
    /// <param name="eventType">事件类型枚举</param> 
    public void RemoveEvent(EVENT_TYPE eventType)
    {
        listeners.Remove(eventType);
    }

    /// <summary>
    /// 移除事件列表中为空的引用，在场景转换时调用
    /// </summary>
    public void RemoveRedundancies()
    {
        Dictionary<EVENT_TYPE, List<IEventListener>> tmpListeners = 
            new Dictionary<EVENT_TYPE, List<IEventListener>>();
        foreach (var item in listeners)
        {
            for (int i = item.Value.Count - 1; i >= 0; i--)
            {
                if (item.Value[i] == null) { item.Value.RemoveAt(i); }
            }
            if (item.Value.Count != 0) { tmpListeners[item.Key] = item.Value; }
        }
        listeners = tmpListeners;
    }
}

/// <summary>
/// UI 侦听器的事件类型
/// </summary>
public enum EVENT_TYPE
{
    NULL,
    ENTERACT_UI,  // UI 戳显示
    COLOR_ACT,  // 交换颜色的互动
    COLOR_GRADIENT, // 颜色渐变的物件
    TALK,  // 对话的互动
    NODE_CALL,  // 播放台词节点时产生的事件
    URL_GOTO,  // 对应 URL 的物体改变 Point 字段
    AUDIO,  // 互动动作的音效
    GET_KEY,  // 与 ColorSpring 互动
    FALL_OUT_RANGE,  // 角色下落
    ELSE
}