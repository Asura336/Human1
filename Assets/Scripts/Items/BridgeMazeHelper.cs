using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandMaze;

[RequireComponent(typeof(BaseMaze))]
public class BridgeMazeHelper : MonoBehaviour
{
    BaseMaze maze;
    Transform selfTransform;

    int mazeHeight, mazeWidth;
    int centerX, centerY;

    public GameObject columnPrefab;
    public Transform keyTransform;
    public int keyPosXMin = 1, keyPosXMax = 5;
    public int keyPosZMin = 3, keyPosZMax = 6;

    private void Start()
    {
        selfTransform = transform;
        maze = GetComponent<BaseMaze>();
        mazeHeight = maze.mazeHeight;
        mazeWidth = maze.mazeWidth;

        SetCenterPoint();
        //centerX = mazeHeight / 2;
        //centerY = mazeWidth / 2;
        //StartCoroutine(BuildColumnAsync());
    }

    void SetCenterPoint()
    {
        var rand = new System.Random(GlobalHub.Instance.MazeSeed);

        centerX = rand.Next(keyPosXMin, keyPosXMax);
        centerY = rand.Next(keyPosZMin, keyPosZMax);

        keyTransform.position = selfTransform.position + maze.Point2Pos(centerX, centerY);
    }

    float SetScale(int px, int py)
    {
        float minH = 5, maxH = 50;
        int manhattan = Mathf.Abs(px - centerX) + Mathf.Abs(py - centerY);
        int maxManhattan = Mathf.Max(
                centerX + centerY, 
                centerX + Mathf.Abs(mazeWidth - centerY),
                Mathf.Abs(mazeHeight - centerX) + centerY,
                Mathf.Abs(mazeHeight - centerX) + Mathf.Abs(mazeWidth - centerY)
            );
        float lerp = 1 - (float)manhattan / maxManhattan;
        return Mathf.Lerp(minH, maxH, lerp);
    }

    IEnumerator BuildColumnAsync()
    {
        int mazeHeight = maze.mazeHeight;
        int mazeWidth = maze.mazeWidth;
        Func<int, int, Vector3> point2Pos = maze.Point2Pos;
        DMaze dMaze = maze.dMaze;

        for (int i = 0; i < mazeHeight; i++)
        {
            for (int j = 0; j < mazeWidth; j++)
            {
                int p = dMaze.ToPoint(i, j);
                if (!maze.useHole || !dMaze.Hole[p])
                {
                    Vector3 pivot = point2Pos(i, j) + selfTransform.position;
                    var column = Instantiate(columnPrefab, pivot, Quaternion.identity, selfTransform);
                    var scale = column.transform.localScale;
                    scale.y = SetScale(i, j);
                    column.transform.localScale = scale;
                }
                yield return null;
            }
        }
    }
}
