using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 人形角色的基础移动表示
/// </summary>
[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class BaseAct : MonoBehaviour
{
    protected Animator animator;
    protected CharacterController cc;

    public LayerMask raycastLayer;
    public float raycastRange = 1;

    /// <summary>
    /// 表示是否受重力影响并播放落下动画
    /// </summary>
    public bool useGravity = true;
    public Vector3 g = Physics.gravity;
    public float rotatePower = 180;
    Vector3 _velocity = Vector3.zero;

    protected static readonly int m_MoveOrder = Animator.StringToHash("MoveOrder");
    protected static readonly int m_IsRunning = Animator.StringToHash("IsRunning");
    protected static readonly int m_IsMove = Animator.StringToHash("IsMove");
    protected static readonly int m_IsGrounded = Animator.StringToHash("IsGrounded");

    #region 与动画状态机关联的属性

    float _moveOrder = 0;
    public float MoveOrder
    {
        get
        {
            return _moveOrder;
        }
        set
        {
            _moveOrder = value;
            if (animator)
            {
                animator.SetFloat(m_MoveOrder, value);
                animator.SetBool(m_IsRunning, value > 0.6f);
            }
        }
    }

    bool _isMove = false;
    public bool IsMove
    {
        get
        {
            return _isMove;
        }

        set
        {
            _isMove = value;
            if (animator) { animator.SetBool(m_IsMove, value); }
        }
    }

    #endregion

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }

    /// <summary>
    /// 模拟重力加速度效果
    /// </summary>
    protected virtual void Update()
    {
        if (!cc.isGrounded && useGravity) { _velocity += g * Time.deltaTime; }
        cc.Move(_velocity * Time.deltaTime);
    }

    /// <summary>
    /// 双脚射线检测，与角色控制器共同决定播放下落动画
    /// </summary>
    protected virtual void OnAnimatorIK()
    {
        int footOnAir = 2;
        var ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot), -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, raycastLayer)) { footOnAir--; }
        ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot), -transform.up);
        if (Physics.Raycast(ray, out hit, raycastRange, raycastLayer)) { footOnAir--; }

        if (useGravity) { animator.SetBool(m_IsGrounded, footOnAir == 0 || cc.isGrounded); }
        else { animator.SetBool(m_IsGrounded, true); }
    }

    public void Rotate(Vector3 newForward)
    {
        // 削除 y 轴的影响
        newForward.y = 0;
        // 将 faceTo 表示的旋转角度信息与 newForward 矢量对齐
        Quaternion faceTo = Quaternion.LookRotation(newForward);
        // 使转动效果看起来更平滑
        transform.rotation = Quaternion.Slerp(transform.rotation, faceTo, rotatePower * Time.deltaTime);
    }
}
