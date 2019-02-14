using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="InteractObject"/> 的策略之一，对话台词组件
/// </summary>
public class TalkActor : IEnteract
{
    public ENTERACT_TYPE EnteractType { get; set; }
    public int Point { get; set; }
    public string Url { get; set; }

    // 唤醒全局互动物体引用
    public void ActAwake()
    {
        GlobalHub.Instance.p_enteract = this;
    }

    // 全局互动物体引用置空，事件指针归位
    public void ActClose()
    {
        GlobalHub.Instance.p_enteract = null;
        currentPoint = Point;
        EventManager.Instance.PostNotification(EVENT_TYPE.TALK, parent, -1);
        EventManager.Instance.PostNotification(EVENT_TYPE.ENTERACT_UI, parent, EnteractType);
    }

    public int ActDo()
    {
        var ei = EventManager.Instance;
        var p2T = GlobalHub.Instance.point2TalkNode;

        // 播放事件节点
        ei.PostNotification(EVENT_TYPE.TALK, parent, currentPoint);

        // 测试用的事件跳转
        if (p2T.ContainsKey(currentPoint) && !p2T[currentPoint].nodeCall.Equals(""))
        {
            ei.PostNotification(EVENT_TYPE.NODE_CALL, parent, p2T[currentPoint].nodeCall);
        }

        // 指针指向下一个事件
        currentPoint = p2T.ContainsKey(currentPoint) ? p2T[currentPoint].next : Point;
        return 0;
    }

    public Component parent;
    /// <summary>
    /// 活动的事件指针
    /// </summary>
    public int currentPoint;

    public TalkActor(Component parent)
    {
        this.parent = parent;
        EnteractType = ENTERACT_TYPE.TLK;
    }
}
