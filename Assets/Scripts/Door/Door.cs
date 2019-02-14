using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 与按钮等联动的门
/// </summary>
[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour, IPhysicsInteract
{
    protected Animator selfAnimator;

    static readonly int m_IsDoorOpen = Animator.StringToHash("IsDoorOpen");
    static readonly int m_DoorDo = Animator.StringToHash("DoorDo");

    bool doorIsOpen = false;
    bool DoorIsOpen
    {
        get
        {
            return doorIsOpen;
        }
        set
        {
            doorIsOpen = value;
            selfAnimator.SetBool(m_IsDoorOpen, value);
        }
    }

    virtual protected void Start()
    {
        selfAnimator = GetComponent<Animator>();
    }

    public void StateChange()
    {
        DoorIsOpen = !DoorIsOpen;
        selfAnimator.SetTrigger(m_DoorDo);
    }

    public void OpenDoor()
    {
        if (DoorIsOpen) { return; }
        StateChange();
    }

    public void CloseDoor()
    {
        if (!DoorIsOpen) { return; }
        StateChange();
    }

    public void StateReset()
    {
        // 按钮弹开后不回弹
    }
}
