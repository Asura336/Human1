using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandMaze;

[RequireComponent(typeof(BaseMaze))]
public class ColumnBuilder : MonoBehaviour
{
    BaseMaze maze;
    Transform selfTransform;

    public GameObject columnPrefab;

    private void Start()
    {
        selfTransform = transform;
        maze = GetComponent<BaseMaze>();

        StartCoroutine(BuildColumnAsync());
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
                    Instantiate(columnPrefab, pivot, Quaternion.identity, selfTransform);
                }
                yield return null;
            }
        }
    }
}
