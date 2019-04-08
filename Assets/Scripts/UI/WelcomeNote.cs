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
        string getValue(WelcomeButton.Type type)
        {
            string key = string.Empty;
            switch (type)
            {
                case WelcomeButton.Type.NEWGAME:
                    key = "newGameNote";
                    break;
                case WelcomeButton.Type.LOADGAME:
                    key = "loadGameNote";
                    break;
                case WelcomeButton.Type.QUITGAME:
                    key = "quitGameNote";
                    break;
                default: break;
            }
            return GlobalHub.Instance.UiTexts[key].GetValue();
        }

        if (eventType == EVENT_TYPE.WELCOME_UI)
        {
            if ((int)param<0)
            {
                selfText.text = string.Empty;
                return;
            }
            selfText.text = getValue((WelcomeButton.Type)param);
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
