using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UI 提示戳
/// </summary>
public class CheckPanel : UIBehaviour, IEventListener
{
    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch(eventType)
        {
            case EVENT_TYPE.ENTERACT_UI:
                SetActive((ENTERACT_TYPE)param);
                break;
            default: return;
        }
    }

    Text selfText;

    protected override void Start()
    {
        selfText = GetComponentInChildren<Text>();

        EventManager.Instance.AddListener(EVENT_TYPE.ENTERACT_UI, this);
        SetActive();
    }

    /// <summary>
    /// 设置 UI 节点活动性
    /// </summary>
    /// <param name="eType"></param> 枚举量作为判断方式的依据
    void SetActive(ENTERACT_TYPE eType = ENTERACT_TYPE.NULL)
    {
        GlobalHub gi = GlobalHub.Instance;
        switch (eType)
        {
            case ENTERACT_TYPE.CLR_EXC:
                selfText.text = "交换颜色";
                bool enteractAwake = (gi.p_enteract != null && gi.p_playerRenderer != null &&
                    gi.p_enteract.Point != (int)gi.PlayerColorType);
                gameObject.SetActive(enteractAwake);
                break;
            case ENTERACT_TYPE.CLR_SPR:
                selfText.text = "触碰色彩源";
                enteractAwake = (gi.p_enteract != null && gi.p_playerRenderer != null &&
                    gi.PlayerColorType == COLOR_TYPE.NULL);
                gameObject.SetActive(enteractAwake);
                break;
            case ENTERACT_TYPE.TLK:
                selfText.text = "对话";
                enteractAwake = gi.p_enteract != null;
                gameObject.SetActive(enteractAwake);
                break;
            default:
                gameObject.SetActive(false);
                break;
        }
    }
}
