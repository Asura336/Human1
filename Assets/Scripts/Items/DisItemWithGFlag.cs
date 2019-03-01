using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 检查游戏事件开关开启，使 <see cref="affectItem"/> 所指组件失活
/// </summary>
public class DisItemWithGFlag : MonoBehaviour
{
    int GlobalFlag { get { return GlobalHub.Instance.GlobalKeyFlag; } }
    [Header("要检查的整数 flag 的所有二进制位的十进制表示")]
    public int checkBinFlag = 0;
    public GameObject affectItem;

    private void Start()
    {
        OnFlagCheck();
    }

    void OnFlagCheck()
    {
        if ((GlobalFlag & checkBinFlag) == checkBinFlag)
        {
            affectItem.SetActive(false);
        }
    }
}
