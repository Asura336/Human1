using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandMaze;

public class BaseMaze : MonoBehaviour
{
    protected Transform selfTransform;
    public DMaze dMaze;
    public GameObject wallPrefab;

    [Header("迷宫单元尺寸")]
    public float cellHeight = 4;
    public float cellWidth = 4;
    public float HalfCellHeight { get { return cellHeight / 2; } }
    public float HalfCellWidth { get { return cellWidth / 2; } }

    [Header("迷宫长和宽，表示为单元数量")]
    public int mazeHeight = 8;
    public int mazeWidth = 8;
    public int seed = 0;  // 随机数种子，每次游戏打开时不同

    [Header("入口（如果在边缘）是否开启及入口坐标")]
    public bool enterOpen = false;
    public int enterPoint_x = 0, enterPoint_y = 4;  // 入口坐标
    [Header("出口（如果在边缘）是否开启及出口坐标")]
    public bool exitOpen = false;
    public int exitPoint_x = 8, exitPoint_y = 4;  // 出口坐标
    protected int p_enter, p_exit;  // 入口和出口指针

    [Header("迷宫内空洞坐标及尺寸，按序号分别设置")]
    public bool useHole = false;
    public int[] holeXs;
    public int[] holeYs;
    public int[] holeDXs;
    public int[] holeDYs;

    [Header("迷宫终点坐标")]
    public int endX = 8, endY = 8;

    protected int[] _distGraph;
    public int[] DistGraph { get { return _distGraph; } }

    public const int infinity = int.MaxValue / 2;

    protected virtual void Awake()
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
    }

    protected virtual void Start()
    {
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
}
