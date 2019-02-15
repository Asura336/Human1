using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 从入口进入后移动到出口位置，同时改变位置和方向
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class TransPos : MonoBehaviour
{
    public Transform trans_exit;
    SimpleCameraFreeLook simpleCamera;
    BoxCollider selfCollider;
    Transform selfTransform;

    private void Start()
    {
        selfTransform = transform;
        if (trans_exit == null)
        {
            Debug.LogWarning("trans_exit is NULL");
            trans_exit = transform;
        }
        simpleCamera = LevelManager.Instance != null ? 
            LevelManager.Instance.p_camera : FindObjectOfType<SimpleCameraFreeLook>();

        selfCollider = GetComponent<BoxCollider>();
        selfCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var dv = other.transform.position - selfTransform.position;
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.transform.rotation);
            other.transform.position = trans_exit.position + dv;
            //other.transform.rotation = trans_exit.rotation;
            simpleCamera.SetPosForce();
        }
        Debug.Log(other.transform.rotation.ToString() + trans_exit.rotation.ToString());
    }
}
