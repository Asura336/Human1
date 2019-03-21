using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="GameObject"/>对象的池
/// </summary>
public class SimObjPool
{
    /// <summary>
    /// 池成员预制体
    /// </summary>
    public GameObject Prefab { get; protected set; }
    private List<GameObject> poolCells;
    private Queue<GameObject> collectCells;
    private Dictionary<GameObject, bool> isActive = new Dictionary<GameObject, bool>();

    /// <summary>
    /// 在活动的个体数目
    /// </summary>
    public int ActiveCount { get { return poolCells.Count - collectCells.Count; } }
    /// <summary>
    /// 对象池可用个体数目不大于此值时对象池扩增
    /// </summary>
    public int waterLine = 0;
    /// <summary>
    /// 对象池的扩增倍数
    /// </summary>
    public float developMutiper = 2;

    /// <summary>
    /// 构造对象池
    /// </summary>
    /// <param name="t">对象池成员预制体</param>
    /// <param name="capacity">指定初始容量</param>
    public SimObjPool(GameObject prefab, int capacity = 8)
    {
        int _capacity = capacity < 8 ? 8 : capacity;
        poolCells = new List<GameObject> { Capacity = _capacity };
        collectCells = new Queue<GameObject>();
        Prefab = prefab;

        IncreaseTo(_capacity);
    }

    /// <summary>
    /// 扩增对象池
    /// </summary>
    /// <param name="newLen">新的对象池容量</param>
    protected virtual void IncreaseTo(int newLen)
    {
        for (int i = 0; i < newLen - poolCells.Count; i++)
        {
            // 构造
            GameObject cell = Object.Instantiate(Prefab, Vector3.zero, Quaternion.identity, null);
            poolCells.Add(cell);
            isActive.Add(cell, true);
            CollectCell(cell);
        }
    }

    /// <summary>
    /// 将指定个体收回对象池
    /// </summary>
    /// <param name="cell">收回对象池的个体</param>
    public virtual void CollectCell(GameObject cell)
    {
        if (!isActive.ContainsKey(cell) || !isActive[cell]) { return; }
        isActive[cell] = false;
        collectCells.Enqueue(cell);
    }

    /// <summary>
    /// 从对象池中拿出个体
    /// </summary>
    /// <returns>对象池中的成员</returns>
    public virtual GameObject PopCell()
    {
        if (collectCells.Count <= waterLine)
        {
            IncreaseTo((int)(poolCells.Count * developMutiper));
        }
        var reserve = collectCells.Dequeue();
        isActive[reserve] = true;
        return reserve;
    }
}
