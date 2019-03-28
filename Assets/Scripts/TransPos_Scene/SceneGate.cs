using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 切换场景所需的触发器
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class SceneGate : MonoBehaviour
{
    public string nextScene;
    public bool changePos = true;
    public Vector3 nextPos;
    public bool changeEuler = false;
    public Vector3 nextForward;

    Collider selfCollider;

    private void Start()
    {
        selfCollider = GetComponent<Collider>();
#if UNITY_EDITOR
        if (!selfCollider.isTrigger)
        {
            Debug.LogWarning(
                string.Format("碰撞器{0}未设置为触发器，检查是否必须", gameObject.name));
        }
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        LevelManager lm = LevelManager.Instance;
        if (other.CompareTag("Player"))
        {
            if (changePos)
            {
                if (changeEuler) { lm.ChangeScene(nextScene, nextPos, nextForward); }
                else { lm.ChangeScene(nextScene, nextPos); }
            }
            else { lm.ChangeScene(nextScene); }
        }
    }
}
