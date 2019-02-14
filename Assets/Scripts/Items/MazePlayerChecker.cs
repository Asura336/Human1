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

    private void Start()
    {
        selfTransform = transform;
        maze = GetComponent<Maze>();
        dMaze = maze.dMaze;
        var li = LevelManager.Instance;
        if (li == null) { return; }
        StartCoroutine(CheckPosition(li.PlayerTrans));  // 检查玩家的位置
    }

    /*
     * 每隔指定时间间隔检查玩家的位置，提示迷宫路径。
     * TODO: 提示方式待改为使用声音
     */
    readonly int infinity = Maze.infinity;
    readonly float cellHeight = Maze.cellHeight;
    readonly float cellWidth = Maze.cellWidth;
    delegate int DelToPoint(int x, int y);
    delegate (int, int) DelPos2Point(Vector3 v);
    readonly WaitForSeconds checkWait = new WaitForSeconds(0.75f);
    readonly string[] dirs = new string[] { "up", "right", "down", "left", "dead way" };
    IEnumerator CheckPosition(Transform player)
    {
        Vector3 selfPos = selfTransform.position;
        int[] graph = dMaze.Maze;
        int[] DistGraph = maze.DistGraph;
        DelToPoint toPoint = dMaze.ToPoint;
        DelPos2Point Pos2Point = Maze.Pos2Point;
        float mazeHeight = dMaze.XCount * cellHeight;
        float mazeWidth = dMaze.YCount * cellWidth;
        while (true)
        {
            float playerX = player.position.x, playerZ = player.position.z;
            bool inSide = playerX > selfPos.x && playerX < selfPos.x + mazeHeight &&
                playerZ > selfPos.z && playerZ < selfPos.z + mazeWidth;
            if (inSide)
            {
                (int x, int y) = Pos2Point(player.position - selfTransform.position);

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
                Debug.Log(dirs[minDisDir] + " " + DistGraph[toPoint(x, y)]);
            }
            yield return checkWait;
        }
    }
}
