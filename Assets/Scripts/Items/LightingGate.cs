using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态控制使<see cref="SceneGate"/>组件生效
/// </summary>
[RequireComponent(typeof(SceneGate))]
public class LightingGate : MonoBehaviour, IPhysicsInteract, IEventListener
{
    public void StateChange()
    {
        bool trig = _bin == binAnswer;
        gateTrigger.isTrigger = trig;
        selfMaterial.SetColor("_AmbientColor", trig ? ambientColor1 : ambientColor0);
        GlobalHub.Instance.Url2Point[url + "_state"] = trig ? 1 : 0;
    }

    public void StateReset()
    {
        
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        if (eventType == EVENT_TYPE.AUDIO && param.Equals(SOUND.STEP_BUTTON))
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (ReferenceEquals(buttons[i], sender))
                {
                    _bin ^= 1 << i;
                    GlobalHub.Instance.Url2Point[url] = _bin;
                    Debug.Log(string.Format("OnEvent: {0}, {1}", _bin, binAnswer));
                    return;
                }
            }
        }
    }

    [Header("设置物体的唯一标识")]
    public string url;

    int _bin = 0;
    int binAnswer = -1;
    [Header("设定此值与 buttons 的长度一致，没有安全检查")]
    public int binMask = 3;

    Collider gateTrigger;
    Renderer selfRenderer;
    Material selfMaterial;
    public Color ambientColor0, ambientColor1;
    public StepButton[] buttons;

    void Start()
    {
        int seed = GlobalHub.Instance.MazeSeed;
        int mask = 1 << binMask - 1;
        binAnswer = seed & mask;

        gateTrigger = GetComponent<Collider>();
        selfRenderer = GetComponent<Renderer>();
        selfMaterial = selfRenderer.material;

        EventManager.Instance.AddListener(EVENT_TYPE.AUDIO, this);
        // init
        var u2p = GlobalHub.Instance.Url2Point;
        if (u2p.ContainsKey(url))
        {
            _bin = u2p[url];
            StateChange();
        }
    }  
}
