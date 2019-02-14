using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NPC 使用的控制器
/// </summary>
[RequireComponent(typeof(Collider))]
public class NPCController : MonoBehaviour
{
    Transform selfTransform;
    BaseAct act;
    NavMeshAgent agent;

    public Transform[] points;
    int p_point = 0;

    /// <summary>
    /// 看向玩家或其他指定目标时为真
    /// </summary>
    bool focusing = false;
    Vector3 focusDis;
    Transform focusTarget;

    // Start is called before the first frame update
    void Start()
    {
        selfTransform = transform;
        act = GetComponent<BaseAct>();
        agent = GetComponent<NavMeshAgent>();

        agent.autoBraking = false;
        GotoNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        // 持续看向玩家并停止当前移动
        if (focusing)
        {
            focusDis = focusTarget.position;
            act.Rotate(focusDis - selfTransform.position);
            agent.isStopped = true;
        }
        else
        {
            // ADD
            agent.isStopped = false;
        }

        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            GotoNextPoint();
        }

        act.IsMove = (agent.remainingDistance > agent.stoppingDistance &&
            !focusing);
        act.MoveOrder = agent.speed;
    }

    void GotoNextPoint()
    {
        if (points.Length == 0) { return; }

        agent.destination = points[p_point].position;
        p_point = (p_point + 1) % points.Length;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            focusing = true;
            focusTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            focusing = false;
        }
    }
}
