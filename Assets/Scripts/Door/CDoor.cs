using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用事件驱动控制的门
/// 颜色枚举与 <see cref="answer"/> 数字相同时门开启
/// </summary>
[RequireComponent(typeof(Animator))]
public class CDoor : Door, IEventListener
{
    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.COLOR_ACT:
                if (ReferenceEquals(sender, enteract))
                {
                    if (answer.Equals(param)) { OpenDoor(); }
                    else { CloseDoor(); }
                }
                break;
            default:
                return;
        }
    }

    [Header("响应来自该对象的颜色变化")]
    public Component p_cEnteract = null;
    IEnteract enteract;
    [Header("触发条件，颜色或者事件指针")]
    public int answer;

    override protected void Start()
    {
        base.Start();


        if (p_cEnteract == null) { p_cEnteract = this; }
        enteract = p_cEnteract.GetComponentInChildren<IEnteract>();
        if (enteract == null)
        {
            Debug.LogErrorFormat(this, "控制器引用为空");
            return;
        }
        EventManager.Instance.AddListener(EVENT_TYPE.COLOR_ACT, this);
    }
}
