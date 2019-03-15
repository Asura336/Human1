using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 参与模拟视线的射线检测，挂在被注视时改变可见性的物体上
/// </summary>
[RequireComponent(typeof(Collider))]
public class VisibilityChanger : MonoBehaviour
{
    // need "VcRaycastField" layer
    public bool initVisibility = true;
    MeshRenderer[] childMeshes;
    Collider[] childColliders;

    WaitForSeconds y_wait;
    [SerializeField]float resetWait = 0;

    bool _isTrig = false;
    public bool IsTrig
    {
        get
        {
            return _isTrig;
        }
        set
        {
            _isTrig = value;
            if (_isTrig) { SetChildVisibility(!initVisibility); }
            else if (resetWait > 0) { StartCoroutine(ResetVisibilityAsync()); }
            else { SetChildVisibility(initVisibility); }
        }
    }

    private void Start()
    {
        childMeshes = GetComponentsInChildren<MeshRenderer>();
        var c = from cell in GetComponentsInChildren<Collider>()
                where !cell.isTrigger
                select cell;
        childColliders = c.ToArray();

        y_wait = new WaitForSeconds(resetWait);
        SetChildVisibility(initVisibility);
    }

    void SetChildVisibility(bool value)
    {
        foreach (var coll in childColliders) { coll.enabled = value; }
        foreach (var mesh in childMeshes) { mesh.enabled = value; }
    }

    IEnumerator ResetVisibilityAsync()
    {
        yield return y_wait;
        SetChildVisibility(initVisibility);
    }
}
