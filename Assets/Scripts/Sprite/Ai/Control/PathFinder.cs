﻿using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Path.AI
{
    public struct PathResult
    {
        public Vector2Int[] path;
        public bool success;

        public PathResult(Vector2Int[] path, bool success)
        {
            this.path = path;
            this.success = success;
        }
    }


    public class PathFinder
    {

        public static PathResult GetPath(MapManage manage, Vector2Int startPosition, Vector2Int endPosition)
        {
            if (manage.width< startPosition.x||manage.height<startPosition.y||manage.width < endPosition.x || manage.height < endPosition.y)
            {
                return new PathResult() {success=false };
            }
            MapTile start = manage.map[startPosition.x, startPosition.y];
            MapTile end = manage.map[endPosition.x,endPosition.y];
            bool success = false;
            Vector2Int[] path = new Vector2Int[0];

            if (!start.isWall && !end.isWall)
            {
               // Queue<MapTile> openSet = new Queue<MapTile>();
                SimplePriorityQueue<MapTile> openSet = new SimplePriorityQueue<MapTile>();
                HashSet<MapTile> closedSet = new HashSet<MapTile>();

                 openSet.Enqueue(start, start.CostF);
               // openSet.Enqueue(start);
                while (openSet.Count > 0)
                {
                    MapTile current = openSet.Dequeue();
                    if (current == end)
                    {
                        success = true;
                        break;
                    }
                    closedSet.Add(current);
                    // for (int i = 0; i < 8; i++)//按次序取出8个角的格子
                    // {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            MapTile neighbour = manage.map[current.posX + (i -1), current.posY - (j + 1)];
                            if (neighbour == null || neighbour.isWall || closedSet.Contains(neighbour))
                            {
                                continue;
                            }
                            float neighbourCost = current.costG + Distance(new Vector2Int(current.posX, current.posY), new Vector2Int(neighbour.posX, neighbour.posY)) + neighbour.costH;
                            if (neighbourCost > neighbour.costG || !openSet.Contains(neighbour))
                            {
                                neighbour.costG = neighbourCost;
                                neighbour.costH = Distance(new Vector2Int(neighbour.posX,neighbour.posY), new Vector2Int(end.posX, end.posY));
                               // neighbour.parent = current;
                                if (!openSet.Contains(neighbour))
                                {
                                     openSet.Enqueue(neighbour, neighbour.CostF);
                                   // openSet.Enqueue(neighbour);
                                }
                                else
                                {
                                     openSet.UpdatePriority(neighbour, neighbour.CostF);
                                }
                            }
                        }
                    }

                }
            }
            //  }

            if (success)
            {
                path = PathFinder.CalcPath(start, end);
                success = path.Length > 0;
            }
            return new PathResult(path, success);
        }

        public static Vector2Int[] CalcPath(MapTile start, MapTile end)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            MapTile current = end;
            while (current != start)
            {
                path.Add(new Vector2Int(current.posX,current.posY));
               // current = current.parent;
            }
            Vector2Int[] result = path.ToArray();
            System.Array.Reverse(result);
            return result;
        }

        public static float Distance(Vector2Int a, Vector2Int b)
        {
            if (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1)
            {
                return 1f;
            }

            if (Mathf.Abs(a.x - b.x) == 1 &&
                Mathf.Abs(a.y - b.y) == 1)
            {
                return 1.41121356237f;
            }

            return Mathf.Sqrt(Mathf.Pow((float)a.x - (float)b.x, 2) +Mathf.Pow((float)a.y - (float)b.y, 2));
        }
    }
}
