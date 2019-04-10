using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 欢迎界面中的说明文本框
/// </summary>
public class WelcomeNote : UIBehaviour, IEventListener
{
    void IEventListener.OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        if (eventType == EVENT_TYPE.WELCOME_UI)
        {
            selfText.text = param as string;
        }
    }

    Text selfText;

    protected override void Start()
    {
        selfText = GetComponent<Text>();
        selfText.text = string.Empty;
        EventManager.Instance.AddListener(EVENT_TYPE.WELCOME_UI, this);
    }
}
