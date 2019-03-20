using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandMaze;

[RequireComponent(typeof(Maze))]
public class MazePlayerChecker : MonoBehaviour
{
    Maze maze;
    DMaze dMaze;
    Transform selfTransform;

    public SoundPoint pointOut;

    private void Start()
    {
        selfTransform = transform;
        maze = GetComponent<Maze>();
        dMaze = maze.dMaze;

        if (pointOut == null) { pointOut = GetComponentInChildren<SoundPoint>(); }
        var li = LevelManager.Instance;
        var gi = GlobalHub.Instance;
        if ((gi.GlobalKeyFlag & (1 << (int)COLOR_TYPE.RED)) == 0)
        {
            StartCoroutine(CheckPosition(li.PlayerTrans));  // 检查玩家的位置
        }
    }

    void PointOut(Vector3 original, Vector3 dir)
    {
        // 传入玩家位置和位置增量
        pointOut.Position = original + dir;
        // 声音提示加在这里
        EventManager.Instance.PostNotification(EVENT_TYPE.AUDIO, this, SOUND.POINT_OUT);
    }

    /*
     * 每隔指定时间间隔检查玩家的位置，提示迷宫路径。
     */
    readonly int infinity = BaseMaze.infinity;
    readonly WaitForSeconds checkWait = new WaitForSeconds(0.75f);
    readonly Vector3[] m_dirs =
        new Vector3[] { Vector3.left, Vector3.forward, Vector3.right, Vector3.back, Vector3.up, };
    IEnumerator CheckPosition(Transform player)
    {
        float cellHeight = maze.cellHeight;
        float cellWidth = maze.cellWidth;

        Vector3 selfPos = selfTransform.position;
        int[] graph = dMaze.Maze;
        int[] DistGraph = maze.DistGraph;
        Func<int, int, int> toPoint = dMaze.ToPoint;
        Func<Vector3, (int, int)> pos2Point = maze.Pos2Point;
        float mazeHeight = dMaze.XCount * cellHeight;
        float mazeWidth = dMaze.YCount * cellWidth;
        while (true)
        {
            float playerX = player.position.x, playerZ = player.position.z;
            bool inSide = playerX > selfPos.x && playerX < selfPos.x + mazeHeight &&
                playerZ > selfPos.z && playerZ < selfPos.z + mazeWidth;
            if (inSide)
            {
                (int x, int y) = pos2Point(player.position - selfTransform.position);

                if (dMaze.Hole[toPoint(x, y)])
                {
                    // 寻路结束
                    yield break;
                }

                int minDis = infinity;
                int minDisDir = 4;
                if ((graph[toPoint(x, y)] & DMaze.up) != 0 && 
                    DistGraph[toPoint(x - 1, y)] < minDis)
                {
                    minDis = DistGraph[toPoint(x - 1, y)];
                    minDisDir = 0;
                }
                if ((graph[toPoint(x, y)] & DMaze.right) != 0 && 
                    DistGraph[toPoint(x, y + 1)] < minDis)
                {
                    minDis = DistGraph[toPoint(x, y + 1)];
                    minDisDir = 1;
                }
                if ((graph[toPoint(x, y)] & DMaze.down) != 0 && 
                    DistGraph[toPoint(x + 1, y)] < minDis)
                {
                    minDis = DistGraph[toPoint(x + 1, y)];
                    minDisDir = 2;
                }
                if ((graph[toPoint(x, y)] & DMaze.left) != 0 && 
                    DistGraph[toPoint(x, y - 1)] < minDis)
                {
                    minDis = DistGraph[toPoint(x, y - 1)];
                    minDisDir = 3;
                }
                PointOut(player.position, m_dirs[minDisDir] * 4);
            }
            yield return checkWait;
        }
    }
}
