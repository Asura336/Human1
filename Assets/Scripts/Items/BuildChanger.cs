using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BuildChanger : MonoBehaviour, IPhysicsInteract
{
    [Header("将蓝轴对朝门外/要响应的碰撞方向")]
    public GameObject[] objects;
    public bool[] visibility;
    bool[] initVisi;

    BoxCollider thisCollider;
    Vector3 selfPosition;
    Vector3 selfForward;  // 来自蓝轴指向的碰撞盒面的碰撞事件被相应

    public void StateChange()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(!initVisi[i]);
        }
    }

    public void StateReset()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(initVisi[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        initVisi = new bool[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            bool vi = i < visibility.Length && visibility[i];
            objects[i].SetActive(vi);
            initVisi[i] = vi;
        }

        thisCollider = GetComponent<BoxCollider>();
        thisCollider.isTrigger = true;
        selfPosition = transform.position;
        selfForward = transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        float dot = Vector3.Dot(selfForward, other.transform.position - selfPosition);
        if (other.CompareTag("Player") && dot > 0) { StateChange(); }
    }

    private void OnTriggerExit(Collider other)
    {
        float dot = Vector3.Dot(selfForward, other.transform.position - selfPosition);
        if (other.CompareTag("Player") && dot > 0) { StateReset(); }
    }
}
