using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandMaze;

public class TextCubeFactory : MonoBehaviour
{
    Maze maze;
    DMaze dMaze;
    Transform selfTransform;
    public TextCube prefab;

    void Start()
    {
        maze = GetComponent<Maze>();
        dMaze = maze.dMaze;
        selfTransform = transform;

        StartCoroutine(OnBuild(maze.DistGraph));
    }

    IEnumerator OnBuild(int[] disGraph)
    {
        int mazeHeight = maze.mazeHeight;
        int mazeWidth = maze.mazeWidth;
        for (int i = 0; i < mazeHeight; i++)
        {
            for (int j = 0; j < mazeWidth; j++)
            {
                int p = dMaze.ToPoint(i, j);
                //Vector3 pivot = Maze.Point2Pos(i, j) + selfTransform.position;
                Vector3 pivot = maze.Point2Pos(i, j) + selfTransform.position;
                var fresh = Instantiate(prefab.gameObject, pivot, Quaternion.identity, null);
                var cube = fresh.GetComponent<TextCube>();
                cube.posX = i;
                cube.posY = j;
                cube.Text = disGraph[dMaze.ToPoint(i, j)].ToString();
                yield return null;
            }
        }
    }
}
