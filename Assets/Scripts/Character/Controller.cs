using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家角色的控制器
/// 接收按键指令控制玩家类中行为的表达
/// </summary>
[RequireComponent(typeof(BaseAct))]
public class Controller : MonoBehaviour
{
    PlayerAct player;
    SimpleCameraFreeLook p_sCamera;
    Transform p_cameraTrans;

    public bool isControlable = true;

    private void Start()
    {
        player = GetComponent<PlayerAct>();
        p_sCamera = FindObjectOfType<SimpleCameraFreeLook>();
        if (p_sCamera == null) { Debug.LogErrorFormat(this, "Camera missing."); }
        p_cameraTrans = p_sCamera.transform;
    }

    /// <summary>
    /// 接受输入，控制转向和播放移动动画
    /// </summary>
    void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        var r = Input.GetAxis("Run");
        p_sCamera.HorizontalHandle = -Input.GetAxis("Mouse X");
        p_sCamera.VerticalHandle = Input.GetAxis("Mouse Y");

        if (!isControlable) { return; }

        player.IsMove = h != 0 || v != 0;
        player.MoveOrder = Mathf.Max(0.5f, r);

        Vector3 newForward = p_cameraTrans.forward * v + p_cameraTrans.right * h;
        newForward.y = 0;
        if (h != 0 || v != 0) { player.Rotate(newForward.normalized); }

        if ((Input.GetMouseButtonUp(0)||Input.GetKeyUp(KeyCode.E))
            && !enteractLock
            && GlobalHub.Instance.p_enteract != null)
        {
            EnteractLock(0.25f);
            GlobalHub.Instance.p_enteract.ActDo();
        }
    }

    void PauseControl(float waitTime)
    {
        StartCoroutine(_PauseControl(waitTime));
    }
    IEnumerator _PauseControl(float waitTime)
    {
        isControlable = false;
        yield return new WaitForSeconds(waitTime);
        isControlable = true;
    }

    bool enteractLock = false;
    void EnteractLock(float waitTime)
    {
        StartCoroutine(_EnteractLock(waitTime));
    }
    IEnumerator _EnteractLock(float waitTime)
    {
        enteractLock = true;
        yield return new WaitForSeconds(waitTime);
        enteractLock = false;
    }
}
