using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandMaze;

public class Maze : BaseMaze
{
    [Header("是否有外框")]
    public bool outEdge = false;

    [Header("入口（如果在边缘）是否开启")]
    public bool enterOpen = false;
    [Header("出口（如果在边缘）是否开启及出口坐标")]
    public bool exitOpen = false;

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < _distGraph.Length; i++) { _distGraph[i] = infinity; }
        dMaze.FindPathUnAlloc(endX, endY, ref _distGraph);

        p_enter = dMaze.ToPoint(enterPoint_x, enterPoint_y);
        p_exit = dMaze.ToPoint(exitPoint_x, exitPoint_y);
        
        StartCoroutine(BuildWallAsync());
    }

    IEnumerator BuildWallAsync()
    {
        for (int i = 0; i < mazeHeight; i++)
        {
            for (int j = 0; j < mazeWidth; j++)
            {
                int p = dMaze.ToPoint(i, j);
                Vector3 pivot = Point2Pos(i, j) + selfTransform.position;
                InsWall(p, pivot);  // 检定，实例化
                yield return null;
            }
        }
    }

    /// <summary>
    /// 为一个点实例化四周墙体
    /// </summary>
    /// <param name="p">迷宫点的指针</param>
    /// <param name="pivot">迷宫单元的坐标</param>
    void InsWall(int p, Vector3 pivot)
    {
        int x = p / mazeWidth, y = p % mazeWidth;
        int cell = dMaze.Maze[p];
        GameObject p_obj = null;

        bool onEdge(int point)
        {
            return outEdge && (!enterOpen || point != p_enter) && (!exitOpen || point != p_exit);
        }

        // 判断 cell 与周围节点是否连通及 cell 是否是需要开口的出入口
        if ((cell & DMaze.up) == 0 && (x != 0 || onEdge(p)))
        {
            p_obj = Instantiate(wallPrefab, pivot, Quaternion.identity, selfTransform);
            p_obj.transform.eulerAngles = new Vector3(0, -90, 0);
        }
        if ((cell & DMaze.right) == 0 && (y != mazeWidth - 1 || onEdge(p)))
        {
            p_obj = Instantiate(wallPrefab, pivot, Quaternion.identity, selfTransform);
        }
        if ((cell & DMaze.down) == 0 && (x != mazeHeight - 1 || onEdge(p)))
        {
            p_obj = Instantiate(wallPrefab, pivot, Quaternion.identity, selfTransform);
            p_obj.transform.eulerAngles = new Vector3(0, 90, 0);
        }
        if ((cell & DMaze.left) == 0 && (y != 0 || onEdge(p)))
        {
            p_obj = Instantiate(wallPrefab, pivot, Quaternion.identity, selfTransform);
            p_obj.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
