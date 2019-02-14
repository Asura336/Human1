using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 踩踏板按钮的头部，触发动作的感受器
/// </summary>
public class StepButtonBody : MonoBehaviour, IPhysicsInteract
{
    Transform thisTransform;  // 每帧调用的 transform 使用缓存

    StepButton thisButton;
    IPhysicsInteract i_thisButon;

    public float raycastRadius = 0.2f;

    private void Awake()
    {
        thisTransform = transform;
    }

    private void Start()
    {
        thisButton = GetComponentInParent<StepButton>();
        i_thisButon = thisButton.GetComponent<IPhysicsInteract>();
    }

    private void FixedUpdate()
    {
        if (Physics.SphereCast(thisTransform.position, raycastRadius, 
            Vector3.up, out RaycastHit hit, 0.3f))
        {
            StateChange();
        }
        else
        {
            StateReset();
        }
    }

    public void StateChange()
    {
        i_thisButon.StateChange();
    }

    public void StateReset()
    {
        i_thisButon.StateReset();
    }
}
