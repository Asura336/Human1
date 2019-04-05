using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandMaze;

public class BridgeMaze : BaseMaze
{
    /// <summary>
    /// 键表示为 (迷宫节点下标, 通路方向) 的元组，值为桥的<see cref="GameObject"/>实体
    /// </summary>
    Dictionary<(int, int), GameObject> m_active = new Dictionary<(int, int), GameObject>();
    Stack<KeyValuePair<(int, int), GameObject>> _dellStack =
        new Stack<KeyValuePair<(int, int), GameObject>>();
    SimObjPool objPool;

    int _bridgePointX = -1, _bridgePointY = -1;
    (int,int)BridgePoint
    {
        get { return (_bridgePointX, _bridgePointY); }
        set
        {
            if (value == (_bridgePointX, _bridgePointY)) { return; }
            // 根据当前位置实例化桥，不在当前位置的桥需要回收
            foreach (KeyValuePair<(int, int), GameObject> single in m_active)
            {
                if (single.Key.Item1 != dMaze.ToPoint(value.Item1, value.Item2))
                {
                    _dellStack.Push(single);
                }
            }   
            while (_dellStack.Count != 0)
            {
                var pair = _dellStack.Pop();
                if (m_active.ContainsKey(pair.Key))
                {
                    DelBridgeObj(pair.Value);
                    m_active.Remove(pair.Key);
                }
            }

            // 检定，实例化
            InsBridge(
                dMaze.ToPoint(value.Item1, value.Item2),
                Point2Pos(value.Item1, value.Item2) + selfTransform.position
            );

            (_bridgePointX, _bridgePointY) = value;
        }
    }

    protected override void Start()
    {
        base.Start();
        objPool = new SimObjPool(wallPrefab);
        var li = LevelManager.Instance;
        if (li != null) { StartCoroutine(CheckPosition(li.PlayerTrans)); }
    }

    GameObject NewBridge(Vector3 position)
    {
        var o = objPool.PopCell();
        o.transform.position = position;
        return o;
    }

    void DelBridgeObj(GameObject obj)
    {
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        objPool.CollectCell(obj);
    }

    readonly WaitForSeconds checkPosWait = new WaitForSeconds(0.25f);
    IEnumerator CheckPosition(Transform player)
    {
        Vector3 selfPos = selfTransform.position;
        int[] graph = dMaze.Maze;
        Func<int, int, int> toPoint = dMaze.ToPoint;
        float dHeight = dMaze.XCount * cellHeight;
        float dWidth = dMaze.YCount * cellWidth;

        while (true)
        {
            float playerX = player.position.x;
            float playerZ = player.position.z;
            bool inSide = playerX > selfPos.x && playerX < selfPos.x + dHeight &&
                playerZ > selfPos.z && playerZ < selfPos.z + dWidth;
            if (inSide)
            {
                (int x, int y) = Pos2Point(player.position - selfTransform.position);
                BridgePoint = (x, y);
            }
            yield return checkPosWait;
        }
    }

    /// <summary>
    /// 在指定位置实例化桥模型
    /// </summary>
    /// <param name="p">迷宫节点下标</param>
    /// <param name="pivot">节点对应的世界坐标</param>
    void InsBridge(int p, Vector3 pivot)
    {
        int x = p / mazeHeight, y = p % mazeWidth;
        if (x < 0 || x >= mazeHeight || y < 0 || y >= mazeWidth) { return; }
        int cell = dMaze.Maze[p];
        GameObject p_obj = null;

        Func<int, int, int> toPoint = dMaze.ToPoint;

        // 判断 cell 与周围节点是否连通及 cell 是否是空穴，每个节点计算4次
        if ((cell & DMaze.up) != 0 && x != 0 &&
            !(dMaze.Hole[p] && dMaze.Hole[toPoint(x - 1, y)]))
        {
            (int, int) key = (cell, DMaze.up);
            if (!m_active.ContainsKey(key))
            {
                p_obj = NewBridge(pivot);
                p_obj.transform.eulerAngles = new Vector3(0, -90, 0);
                m_active.Add(key, p_obj);
            }
        }
        if ((cell & DMaze.right) != 0 && y != mazeWidth - 1 &&
            !(dMaze.Hole[p] && dMaze.Hole[toPoint(x, y + 1)]))
        {
            (int, int) key = (cell, DMaze.right);
            if (!m_active.ContainsKey(key))
            {
                p_obj = NewBridge(pivot);
                m_active.Add(key, p_obj);
            }
        }
        if ((cell & DMaze.down) != 0 && x != mazeHeight - 1 &&
            !(dMaze.Hole[p] && dMaze.Hole[toPoint(x + 1, y)]))
        {
            (int, int) key = (cell, DMaze.down);
            if (!m_active.ContainsKey(key))
            {
                p_obj = NewBridge(pivot);
                p_obj.transform.eulerAngles = new Vector3(0, 90, 0);
                m_active.Add(key, p_obj);
            }
        }
        if ((cell & DMaze.left) != 0 && y != 0 &&
            !(dMaze.Hole[p] && dMaze.Hole[toPoint(x, y - 1)]))
        {
            (int, int) key = (cell, DMaze.left);
            if (!m_active.ContainsKey(key))
            {
                p_obj = NewBridge(pivot);
                p_obj.transform.eulerAngles = new Vector3(0, 180, 0);
                m_active.Add(key, p_obj);
            }
        }
    }
}
