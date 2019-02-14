using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 对话框的显示元素
/// </summary>
public class TalkPlan : UIBehaviour, IEventListener
{
    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.TALK:
                PlayNode((int)param);
                break;
            default:
                return;
        }
    }

    Text selfText;

    protected override void Start()
    {
        selfText = GetComponentInChildren<Text>();
        EventManager.Instance.AddListener(EVENT_TYPE.TALK, this);
        PlayNode(-1);
    }

    /// <summary>
    /// 每一句台词作为一个事件节点播放
    /// </summary>
    void PlayNode(int point)
    {
        if (point < 0)  // 对话结束
        {
            OnNodeClose();
            return;
        }
        gameObject.SetActive(true);
        // 解析节点内容到 UI
        selfText.text = GlobalHub.Instance.point2TalkNode[point].talkStrs;
    }

    /// <summary>
    /// 对话完毕或者中止
    /// </summary>
    void OnNodeClose()
    {
        gameObject.SetActive(false);
        // ADD
    }
}
