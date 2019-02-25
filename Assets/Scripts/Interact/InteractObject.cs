using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景中可以调查互动的物体
/// </summary>
[RequireComponent(typeof(Collider))]
public class InteractObject : MonoBehaviour, IEnteract, IEventListener
{
    #region 接口实现部分

    public ENTERACT_TYPE EnteractType
    {
        get { return setType; }
        set { }
    }

    public int Point
    {
        get { return enteract.Point; }
        set
        {
            enteract.Point = value;
            GlobalHub.Instance.Url2Point[Url] = value;
        }
    }

    public string Url
    {
        get { return enteract.Url; }
        set { enteract.Url = value; }
    }

    public void ActAwake()
    {
        enteract.ActAwake();
    }

    public void ActClose()
    {
        enteract.ActClose();
    }

    public int ActDo()
    {
        int doR = enteract.ActDo();
        if (doR == 0)
        {
            // 成功提示音效
            EventManager.Instance.PostNotification(EVENT_TYPE.ENTERACT_AUDIO, this, "Succeed");
            Debug.Log(Url + " Complete");
        }
        else
        {
            // 失败提示音效
            EventManager.Instance.PostNotification(EVENT_TYPE.ENTERACT_AUDIO, this, "Fail");
            Debug.Log(Url + " Fail");
        }
        return 0;
    }

    #endregion

    IEnteract enteract;

    public ENTERACT_TYPE setType;
    [Header("颜色球设置颜色枚举；对话组件设置台词指针")]
    public int point;
    [Header("设置物件的唯一标识")]
    public string url;

    private void Awake()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.NODE_CALL, this);
    }

    void Start()
    {
        Renderer selfRenderer = GetComponent<Renderer>();

        var u2p = GlobalHub.Instance.Url2Point;
        if (u2p.ContainsKey(url))
        {
            point = u2p[url];  // 从缓存字典提取，清空字典则新加载场景颜色为初始化颜色
        }
        switch (EnteractType)
        {
            case ENTERACT_TYPE.CLR_EXC:
                enteract = new ColorExchanger(this, selfRenderer.material)
                {
                    Url = url,
                    Point = point,
                    wrap = this
                };
                // 场景唤醒时发送一条消息，如开启颜色已经正确的门
                EventManager.Instance.PostNotification(EVENT_TYPE.COLOR_ACT, this, Point);
                break;
            case ENTERACT_TYPE.CLR_SPR:
                enteract = new ColorSpring(this, selfRenderer.material)
                {
                    Url = url,
                    Point = point,
                    wrap = this,
                };
                break;
            case ENTERACT_TYPE.TLK:
                enteract = new TalkActor(this)
                {
                    Point = point,
                    currentPoint = point,
                    Url = url
                };
                break;
            default:
                Debug.LogErrorFormat(this, "交互类型未指定");
                return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActAwake();
            EventManager.Instance.PostNotification(EVENT_TYPE.ENTERACT_UI, this, EnteractType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActClose();
            EventManager.Instance.PostNotification(EVENT_TYPE.ENTERACT_UI, this, EnteractType);
        }
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.NODE_CALL:
                CallParser(param as string, sender);
                break;
            default:
                return;
        }
    }
    /// <summary>
    /// 解释指令并触发事件
    /// </summary>
    /// <param name="call">指令应能被空格分为三段</param>
    void CallParser(string call, Component callSender)
    {
        string[] calls = call.Split(' ');
        if (calls[1].Equals("goto") && int.TryParse(calls[2], out int result))
        {
            if (calls[0].Equals("self") && ReferenceEquals(callSender, this)) { Point = result; }
            else if (calls[0].Equals(Url)) { Point = result; }
        }
    }
}
