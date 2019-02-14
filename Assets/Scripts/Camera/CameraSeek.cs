using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 参与模拟视线的射线检测，挂在相机上
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraSeek : MonoBehaviour
{
    Camera thisCamera;
    Vector2 screenCenter;

    RaycastHit hitCache;

    [Range(0.1f, 100)]
    public float raycastDis = 10;
    public LayerMask seekLayer;

    [Range(0, 10)]
    public float trigTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        thisCamera = GetComponent<Camera>();
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void FixedUpdate()
    {
        SeekRaycast();
    }

    const int hitCapacity = 8;
    RaycastHit[] hits = new RaycastHit[hitCapacity];
    VisibilityChanger[] vcs = new VisibilityChanger[hitCapacity];
    int vcsCount = 0;
    Stack<VisibilityChanger> actVcs = new Stack<VisibilityChanger>();  // 被注视的物件
    Stack<VisibilityChanger> delVcs = new Stack<VisibilityChanger>();  // 待重置状态的物件
    Dictionary<VisibilityChanger, float> seekTimes = new Dictionary<VisibilityChanger, float>();
    void SeekRaycast()
    {
        vcsCount = 0;
        Ray ray = thisCamera.ScreenPointToRay(screenCenter);
        int hitsCount = Physics.RaycastNonAlloc(ray, hits, raycastDis, seekLayer);
        if (hitsCount != 0)  // 加入新扫描的物件
        {
            for (int i = 0; i < hitsCount; i++)
            {
                var vc = hits[i].transform.GetComponent<VisibilityChanger>();
                if (vc != null)
                {
                    vcs[vcsCount++] = vc;
                    if (!seekTimes.ContainsKey(vc)) { seekTimes.Add(vc, 0); }
                }
            }
        }

        foreach (var cell in seekTimes)
        {
            bool cellInVcs = false;
            for (int i = 0; i < vcsCount; i++)
            {
                if (ReferenceEquals(cell.Key, vcs[i]))
                {
                    cellInVcs = true;
                    break;
                }
            }
            if (cellInVcs) { actVcs.Push(cell.Key); }
            else { delVcs.Push(cell.Key); }
        }

        while (actVcs.Count > 0)
        {
            var thing = actVcs.Pop();
            seekTimes[thing] += Time.fixedDeltaTime;
            if (seekTimes[thing] > trigTime) { thing.IsTrig = true; }
        }
        
        while (delVcs.Count > 0)
        {
            var thing = delVcs.Pop();
            thing.IsTrig = false;
            seekTimes.Remove(thing);
        }
    }
}
