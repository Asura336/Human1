using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandMaze;

public class BridgeMaze : BaseMaze
{
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
            // 根据当前位置实例化桥，不在当前位置的桥需要回收
            void _DellBridge(KeyValuePair<(int, int), GameObject> pair)
            {
                if (m_active.ContainsKey(pair.Key))
                {
                    DelBridgeObj(pair.Value);
                    m_active.Remove(pair.Key);
                }
            }
            void _BuildBridge(int x, int y)
            {
                int p = dMaze.ToPoint(x, y);
                Vector3 pivot = Point2Pos(x, y) + selfTransform.position;
                InsBridge(p, pivot);  // 检定，实例化
            }

            if (value == (_bridgePointX, _bridgePointY)) { return; }

            foreach (KeyValuePair<(int, int), GameObject> single in m_active)
            {
                if (single.Key.Item1 != dMaze.ToPoint(value.Item1, value.Item2))
                {
                    _dellStack.Push(single);
                }
            }
            while (_dellStack.Count != 0) { _DellBridge(_dellStack.Pop()); }
            _BuildBridge(value.Item1, value.Item2);
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

    IEnumerator CheckPosition(Transform player)
    {
        Vector3 selfPos = selfTransform.position;
        int[] graph = dMaze.Maze;
        Func<int, int, int> toPoint = dMaze.ToPoint;
        float dHeight = dMaze.XCount * cellHeight;
        float dWidth = dMaze.YCount * cellWidth;

        while (true)
        {
            float playerX = player.position.x, playerZ = player.position.z;
            bool inSide = playerX > selfPos.x && playerX < selfPos.x + dHeight &&
                playerZ > selfPos.z && playerZ < selfPos.z + dWidth;
            if (inSide)
            {
                (int x, int y) = Pos2Point(player.position - selfTransform.position);
                BridgePoint = (x, y);
            }
            yield return null;
        }
    }

    /// <summary>
    /// 为一个点实例化四周墙体
    /// </summary>
    /// <param name="p">迷宫点的指针</param>
    /// <param name="pivot">迷宫单元的坐标</param>
    void InsBridge(int p, Vector3 pivot)
    {
        int x = p / mazeHeight, y = p % mazeWidth;
        if (x < 0 || x >= mazeHeight || y < 0 || y >= mazeWidth) { return; }
        int cell = dMaze.Maze[p];
        GameObject p_obj = null;

        Func<int, int, int> toPoint = dMaze.ToPoint;

        // 判断 cell 与周围节点是否连通及 cell 是否是需要开口的出入口
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
