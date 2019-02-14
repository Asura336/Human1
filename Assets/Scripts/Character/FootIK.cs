using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 对使用角色控制器和动画的人物，描述足部 IK 功能的表达
/// 从动画状态机中提取左右脚的 IK 权重并影响动作效果
/// </summary>
public class FootIK : MonoBehaviour
{
    public CharacterController cc;
    public Animator animator;
    public LayerMask raycastLayer = 1 << 10;  // IK 适应的图层
    
    [SerializeField] private bool isUseIK = true;  // 用于测试的 IK 效果开关
    private float rightFootWeight = 0f;  // 右脚的 IK 权重 
    private float leftFootWeight = 0f;  // 左脚的 IK 权重
    private Vector3 rightFootPos;  // 右脚的位置
    private Vector3 leftFootPos;  // 左脚的位置
    private Quaternion rightFootRot;  // 右脚的角度
    private Quaternion leftFootRot;  // 左脚的角度
    [SerializeField] private float offset = 0.1f;  // 脚部着地点的偏移量
    [SerializeField] private float rayRange = 1f;  // 射线检测的射线长度
    private bool isRightFootIK = false;
    private bool isLeftFootIK = false;

    /// <summary>
    /// 处理站立时的姿势
    /// </summary>
    bool IsCharacterIdle
    {
        get
        {
            return !animator.GetBool("IsMove");
        }
    }  // 人物在站立状态的判断，具体实现可调整
    [SerializeField] private bool isChangeColPos = true;  // 是否改变碰撞盒的中心位置
    private Vector3 defaultCenter;  // 碰撞盒的初始中心点
    [SerializeField] private float smoothing = 2f;  // 碰撞盒位置调节的速度

    void Start()
    {
        defaultCenter = cc.center;
    }

    void OnAnimatorIK()
    {
        // 右脚射线检测画线调试
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.RightFoot), -transform.up * rayRange, Color.red);
        // 右脚部分的射线检测
        var ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot), -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayRange, raycastLayer))
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
        Debug.DrawRay(animator.GetIKPosition(AvatarIKGoal.LeftFoot), -transform.up * rayRange, Color.red);
        // 左脚部分的射线检测
        ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot), -transform.up);
        if (Physics.Raycast(ray, out hit, rayRange, raycastLayer))
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
            rightFootWeight = animator.GetFloat("RightFootWeight");
            leftFootWeight = animator.GetFloat("LeftFootWeight");
            if (isRightFootIK)
            {
                // 设置右脚的 IK 权重
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                // 设置右脚的位置和角度
                animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos + new Vector3(0f, offset, 0f));
                animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRot);
            }
            if (isLeftFootIK)
            {
                // 设置左脚的 IK 权重
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                // 设置左脚的位置和角度
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos + new Vector3(0f, offset, 0f));
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
                cc.center = Vector3.Lerp(
                                                cc.center, 
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
