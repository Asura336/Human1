using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 踩踏板的行为
/// 人物或者物体压在上面之后会按下改变状态
/// 改变状态关联其他物体
/// </summary>
[RequireComponent(typeof(Animator))]
public class StepButton : MonoBehaviour, IPhysicsInteract, IEventListener
{
    public Transform follower;  // 与踩踏板关联的物体
    IPhysicsInteract i_follower;

    Animator selfAnimator;

    static readonly int m_IsTriggered = Animator.StringToHash("IsTriggered");
    static readonly int m_ButtonDo = Animator.StringToHash("ButtonDo");

    AudioSource selfAudioSource;

    bool isTriggered = false;
    bool IsTriggered
    {
        get
        {
            return isTriggered;
        }
        set
        {
            isTriggered = value;
            selfAnimator.SetBool(m_IsTriggered, value);
        }
    }

    private void Start()
    {
        selfAnimator = GetComponent<Animator>();
        
        i_follower = follower.GetComponent<IPhysicsInteract>();

        selfAudioSource = GetComponent<AudioSource>();
        EventManager.Instance.AddListener(EVENT_TYPE.AUDIO, this);
    }

    public void StateChange()
    {
        if (IsTriggered) { return; }
        IsTriggered = true;
        selfAnimator.SetTrigger(m_ButtonDo);
    }

    public void StateReset()
    {
        if (!IsTriggered) { return; }
        IsTriggered = false;
        selfAnimator.SetTrigger(m_ButtonDo); 
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        var clips = GlobalHub.Instance.SoundClips;
        if (eventType == EVENT_TYPE.AUDIO &&
            ReferenceEquals(sender, this) &&  // 只响应来自自身的消息
            param.Equals(SOUND.STEP_BUTTON))  // 限定踩踏板音效
        {
            selfAudioSource.PlayOneShot(clips[(int)param]);
        }
    }

    #region 帧动画事件调用函数

    public void AnimeEventOnStateChange()
    {
        // 增加消息发送
        var ei = EventManager.Instance;
        ei.PostNotification(EVENT_TYPE.AUDIO, this, SOUND.STEP_BUTTON);
        if (i_follower != null) { i_follower.StateChange(); }
    }

    public void AnimeEventOnStateReset()
    {
        // 增加消息发送
        if (i_follower != null) { i_follower.StateReset(); }
    }

    #endregion
}
