using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandMaze;

public class Maze : MonoBehaviour
{
    Transform selfTransform;
    public DMaze dMaze;
    public GameObject wallPrefab;

    [Header("迷宫单元尺寸")]
    public float cellHeight = 4;
    public float cellWidth = 4;
    float HalfCellHeight { get { return cellHeight / 2; } }
    float HalfCellWidth { get { return cellWidth / 2; } }

    [Header("迷宫长和宽，表示为单元数量")]
    public int mazeHeight = 8;
    public int mazeWidth = 8;
    public int seed = 0;  // 随机数种子，每次游戏打开时不同
    [Header("是否有外框")]
    public bool outEdge = false;
    [Header("入口（如果在边缘）是否开启及入口坐标")]
    public bool enterOpen = false;
    public int enterPoint_x = 0, enterPoint_y = 4;  // 入口坐标
    [Header("出口（如果在边缘）是否开启及出口坐标")]
    public bool exitOpen = false;
    public int exitPoint_x = 8, exitPoint_y = 4;  // 出口坐标
    int p_enter, p_exit;  // 入口和出口指针

    [Header("迷宫内空洞坐标及尺寸，按序号分别设置")]
    public bool useHole = false;
    public int[] holeXs;
    public int[] holeYs;
    public int[] holeDXs;
    public int[] holeDYs;

    [Header("迷宫终点坐标")]
    public int endX = 8, endY = 8;

    int[] _distGraph;
    public int[] DistGraph { get { return _distGraph; } }

    public const int infinity = int.MaxValue / 2;

    private void Awake()
    {
        selfTransform = transform;
        seed = GlobalHub.Instance.MazeSeed;
        dMaze = new DMaze(mazeHeight, mazeWidth) { rand = new System.Random(seed) };
        p_enter = dMaze.ToPoint(enterPoint_x, enterPoint_y);
        p_exit = dMaze.ToPoint(exitPoint_x, exitPoint_y);
        if (useHole)
        {
            for (int i = 0; i < holeXs.Length; i++)
            {
                dMaze.SetHole(holeXs[i], holeYs[i], holeDXs[i], holeDYs[i]);
            }
        }
        dMaze.DfsBuild(enterPoint_x, enterPoint_y, useHole);
        if (useHole) { dMaze.BuildHole(); }
        StartCoroutine(BuildWallAsync());

        _distGraph = new int[dMaze.Capacity];
        for (int i = 0; i < _distGraph.Length; i++) { _distGraph[i] = infinity; }
        dMaze.FindPathUnAlloc(endX, endY, ref _distGraph);
    }

    public Vector3 Point2Pos(int x, int z)
    {
        return new Vector3(x * cellHeight + HalfCellHeight, 0, z * cellWidth + HalfCellWidth);
    }

    public (int x, int y) Pos2Point(Vector3 mPos)
    {
        return ((int)(mPos.x / cellHeight), (int)(mPos.z / cellWidth));
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

        Func<int, bool> onEdge = (int point) =>
        {
            return outEdge && (!enterOpen || point != p_enter) && (!exitOpen || point != p_exit);
        };

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
