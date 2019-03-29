using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状态控制使<see cref="SceneGate"/>组件生效
/// </summary>
[RequireComponent(typeof(SceneGate))]
public class LightingGate : MonoBehaviour, IEventListener
{
    public void StateCheck()
    {
        bool trig = _bin == binAnswer;

        if (gateTrigger != null) { gateTrigger.isTrigger = trig; }

        selfMaterial.SetColor("_AmbientColor", trig ? ambientColor1 : ambientColor0);

        if (showText != null) {
            showText.text = trig ? string.Empty : string.Format("{0} : {1}", _bin, binAnswer);
        }
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        // 没有安全检查
        if (eventType == EVENT_TYPE.STEP_BUTTON)
        {
            var post = (SBPoster)param;
            Debug.Log("Get: "+post.dParam.ToString()); 
            if (post.strParam.Equals(url))
            {
                _bin ^= post.dParam;
                GlobalHub.Instance.Url2Point[url] = _bin;
                StateCheck();
            }
        }
    }

    [Header("设置物体的唯一标识")]
    public string url;

    int _bin = 0;
    int binAnswer = -1;
    [Header("取随机数种子的后几位，没有安全检查")]
    public int binMask = 3;

    Collider gateTrigger;
    Renderer selfRenderer;
    Material selfMaterial;
    Text showText;
    
    public Color ambientColor0, ambientColor1;

    void Start()
    {
        int seed = GlobalHub.Instance.MazeSeed;
        int mask = 1 << binMask - 1;
        binAnswer = seed & mask;

        gateTrigger = GetComponent<Collider>();
        selfRenderer = GetComponent<Renderer>();
        selfMaterial = selfRenderer.material;
        showText = GetComponentInChildren<Text>();

        EventManager.Instance.AddListener(EVENT_TYPE.STEP_BUTTON, this);
        // init
        var u2p = GlobalHub.Instance.Url2Point;
        if (u2p.ContainsKey(url))
        {
            _bin = u2p[url];
            StateCheck();
        }
        else
        {
            showText.text = string.Format("{0} : {1}", _bin, binAnswer);
        }
    }  
}
