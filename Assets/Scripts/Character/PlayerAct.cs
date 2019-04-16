using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家角色表示
/// </summary>
[RequireComponent(typeof(Animator), typeof(CharacterController))]
public sealed class PlayerAct : BaseAct, IEventListener
{
    static readonly int m_LeftFootWeight = Animator.StringToHash("LeftFootWeight");
    static readonly int m_RightFootWeight = Animator.StringToHash("RightFootWeight");

    #region 足部 IK 用到的字段

    [SerializeField] private bool isUseIK = true;  // 用于测试的 IK 效果开关
    private float rightFootWeight = 0f;  // 右脚的 IK 权重 
    private float leftFootWeight = 0f;  // 左脚的 IK 权重
    private Vector3 rightFootPos;  // 右脚的位置
    private Vector3 leftFootPos;  // 左脚的位置
    private Quaternion rightFootRot;  // 右脚的角度
    private Quaternion leftFootRot;  // 左脚的角度
    [SerializeField] private float offset = 0.1f;  // 脚部着地点的偏移量
    Vector3 footOffset;  // set footOffset = Vector3.up * offset
    private bool isRightFootIK = false;
    private bool isLeftFootIK = false;
    [SerializeField] private bool isChangeColPos = true;  // 是否改变碰撞盒的中心位置
    private Vector3 defaultCenter;  // 碰撞盒的初始中心点
    [SerializeField] private float smoothing = 2f;  // 碰撞盒位置调节的速度

    /// <summary>
    /// 处理站立时的姿势
    /// </summary>
    bool IsCharacterIdle
    {
        get
        {
            return !animator.GetBool(m_IsMove);
        }
    }  // 人物在站立状态的判断，具体实现可调整

    #endregion

    AudioSource selfAudioSource2D;

    protected override void Start()
    {
        base.Start();
        defaultCenter = cc.center;
        footOffset = Vector3.up * offset;

        var gi = GlobalHub.Instance;
        Renderer m_renderer = GetComponentInChildren<Renderer>();
        gi.p_playerMaterial = m_renderer.material;
        gi.PlayerColorType = (COLOR_TYPE)gi.Url2Point["Player"];

        selfAudioSource2D = GetComponent<AudioSource>();
        EventManager.Instance.AddListener(EVENT_TYPE.AUDIO, this);
    }

    protected override void Update()
    {
        base.Update();
        if (selfTransform.position.y < -100)
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.FALL_OUT_RANGE, this);
        }
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        var clips = GlobalHub.Instance.SoundClips;
        bool is2dClip = param.Equals(SOUND.ENTERACT_NULL) ||
            param.Equals(SOUND.ENTERACT_SUCCEED) ||
            param.Equals(SOUND.ENTERACT_FAIL) ||
            param.Equals(SOUND.LAND);
        if (eventType == EVENT_TYPE.AUDIO && is2dClip)
        {
            selfAudioSource2D.PlayOneShot(clips[(int)param]);
        }
    }

    void AnimeEventOnLand()
    {
        var ei = EventManager.Instance;
        ei.PostNotification(EVENT_TYPE.AUDIO, this, SOUND.LAND);
    }

    protected override void OnAnimatorIK()
    {
        base.OnAnimatorIK();

        // 右脚射线检测画线调试
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.RightFoot), -transform.up * raycastRange, Color.red);
        // 右脚部分的射线检测
        var ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot), -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, raycastLayer))
        {
            rightFootPos = hit.point;
            rightFootRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            isRightFootIK = true;
        }
        else
        {
            isRightFootIK = false;
        }

        // 左脚射线检测画线调试
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.LeftFoot), -transform.up * raycastRange, Color.red);
        // 左脚部分的射线检测
        ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot), -transform.up);
        if (Physics.Raycast(ray, out hit, raycastRange, raycastLayer))
        {
            leftFootPos = hit.point;
            leftFootRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            isLeftFootIK = true;
        }
        else
        {
            isLeftFootIK = false;
        }

        if (isUseIK)
        {
            // 从动画状态获取权重值
            rightFootWeight = animator.GetFloat(m_RightFootWeight);
            leftFootWeight = animator.GetFloat(m_LeftFootWeight);
            if (isRightFootIK)
            {
                // 设置右脚的 IK 权重
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                // 设置右脚的位置和角度
                animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos + footOffset);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRot);
            }
            if (isLeftFootIK)
            {
                // 设置左脚的 IK 权重
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                // 设置左脚的位置和角度
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos + footOffset);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRot);
            }
        }

        if (isChangeColPos)
        {
            if (IsCharacterIdle) // 人物站立时调整碰撞盒位置
            {
                // 左右脚 y 方向距离的一半
                float distance = Mathf.Abs(rightFootPos.y - leftFootPos.y) / 2;
                // 调节碰撞盒中心位置
                cc.center = Vector3.Lerp(cc.center,
                    new Vector3(0f, defaultCenter.y + distance, 0f),
                    smoothing * Time.deltaTime);
            }
            else
            {
                cc.center = defaultCenter;
            }
        }
    }
}
