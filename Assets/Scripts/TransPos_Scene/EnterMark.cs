using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 人物进入当前场景后自动与该组件对其位置和朝向,
/// 开启自动对齐覆盖 <see cref="LevelManager.ChangeScene(string, Vector3, Vector3)"/> 的效果
/// </summary>
public class EnterMark : MonoBehaviour
{
    [Header("_自动对齐位置_")]
    public bool autoPos = false;
    [Header("_自动对齐朝向_")]
    public bool autoEuler = false;

    Transform selfTransform;

    private void Start()
    {
        selfTransform = transform;
        var li = LevelManager.Instance;
        Transform playerTrans = li.PlayerTrans;
        if (playerTrans == null) { Debug.LogWarning("该处玩家引用为空" + gameObject); }
        if (autoPos) { playerTrans.position = selfTransform.position; }
        if (autoEuler) { playerTrans.rotation = selfTransform.rotation; }
        li.p_camera.SetAlphaBack();
        li.p_camera.SetPosForce();
    }
}
