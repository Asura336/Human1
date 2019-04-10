using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 欢迎界面中的按钮
/// </summary>
public class WelcomeButton : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    public Type buttonType;
    Text selfText;

    protected override void Start()
    {
        selfText = GetComponentInChildren<Text>();
        selfText.text = getKey(buttonType);
        string getKey(Type type)
        {
            string key = string.Empty;
            switch (type)
            {
                case Type.NEWGAME:
                    key = "newGame";
                    break;
                case Type.LOADGAME:
                    key = "loadGame";
                    break;
                case Type.QUITGAME:
                    key = "quitGame";
                    break;
                default: break;
            }
            return GlobalHub.Instance.UiTexts[key].GetValue();
        }
    }

    /// <summary>
    /// 按钮回调函数，在编辑器中绑定
    /// </summary>
    public void OnButtonClick()
    {
        var gi = GlobalHub.Instance;
        var ei = EventManager.Instance;
        switch (buttonType)
        {
            case Type.NEWGAME:
                gi.CreateInitSaveFile();
                SceneManager.LoadScene("3C_and_UI");
                break;
            case Type.LOADGAME:
                bool exist = gi.SaveFileExist();
                bool intact = gi.SaveFileIntact();
                if (exist && intact)
                {
                    gi.ReadSaveFile();
                    SceneManager.LoadScene("3C_and_UI");
                }
                else
                {
                    ei.PostNotification(EVENT_TYPE.WELCOME_UI, this,
                            exist ?
                            gi.UiTexts["saveInvalid"].GetValue() :
                            gi.UiTexts["saveNotExist"].GetValue());
                }
                break;
            case Type.QUITGAME:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
            default: return;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.WELCOME_UI, this, Type2Str(buttonType));
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.WELCOME_UI, this, string.Empty);
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.WELCOME_UI, this, Type2Str(buttonType));
    }

    string Type2Str(Type type)
    {
        string key = string.Empty;
        switch (type)
        {
            case Type.NEWGAME:
                key = "newGameNote";
                break;
            case Type.LOADGAME:
                key = "loadGameNote";
                break;
            case Type.QUITGAME:
                key = "quitGameNote";
                break;
            default: return string.Empty;
        }
        return GlobalHub.Instance.UiTexts[key].GetValue();
    }

    // 按钮状态枚举
    public enum Type
    {
        NEWGAME,
        LOADGAME,
        QUITGAME
    }
}
