using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinder
{
    private static float removeFromOpenChrono = 0f;
    private static float foreachNeighborsChrono = 0f;
    private static float clonePathChrono = 0f;
    private static float extendPathChrono = 0f;
    private static float searchInOpenListChrono = 0f;
    private static float searchInCloseListChrono = 0f;
    private static float searchInsertionChrono = 0f;
    private static float insertToOpenListChrono = 0f;
    private static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();

    #region Public Methods
    /// <summary>
    /// Search a path with Dijkstra algorithm
    /// </summary>
    /// <param name="_Start">Tile to start from</param>
    /// <param name="_End">Tile to reach</param>
    /// <returns>the shortest path from start tile to end tile if it exists, null otherwise</returns>
    public static Path SearchDijkstraPathFromTo(GameTile _Start, GameTile _End, out ChronoInfos _infos)
    {
        searchInOpenListChrono = 0f;
        foreachNeighborsChrono = 0f;
        clonePathChrono = 0f;
        searchInsertionChrono = 0f;
        searchInCloseListChrono = 0f;
        extendPathChrono = 0f;

        if (!_Start || !_End)
        {
            _infos = ChronoInfos.InfosNone;
            return null;
        }

        Path firstPath = new Path(_Start);

        List<Path> openList = new List<Path> { firstPath };
        //List<Path> closeList = new List<Path>();
        Dictionary<GameTile, int> closePaths = new Dictionary<GameTile, int>();
        int shortest = -1;

        chrono.Restart();
        while (openList.Count > 0)
        {
            // get the path at index 0 to extend. The open list is assumed as always sorted by path weight
            Path pathToExtend = openList[0];

            // if last step is the end, then return this path
            if (pathToExtend.LastStep == _End)
            {
                _infos = StopChrono(_Start, _End);
                return pathToExtend;
            }

            int weightToExtend = pathToExtend.Weight;
            GameTile stepToExtend = pathToExtend.LastStep;

            foreachNeighborsChrono -= chrono.ElapsedMilliseconds;
            // loop through all neighbors from step to extend
            for (int i = 0; i < stepToExtend.Neighbors.Count; i++)
            {
                GameTile neighbor = stepToExtend.Neighbors[i];

                // if inaccessible neighbor, skip iteration
                if (!neighbor.IsAccessible)
                    continue;

                int weightExtended = weightToExtend + neighbor.Weight;

                // predicate to find a shortest or same length path to reach the same tile
                System.Predicate<Path> predShortest = (Path p) =>
                {
                    return p.LastStep == neighbor && p.Weight <= weightExtended;
                };

                bool shortestExists = false;
                /**
                // search in open list if a shortest path already reached the same tile
                searchInOpenList -= chrono.ElapsedMilliseconds;
                bool shortestExists = openList.Exists(predShortest);
                searchInOpenList += chrono.ElapsedMilliseconds;

                if (!shortestExists)
                {
                    // if no path found in open list, search also in the close list
                    searchInCloseList -= chrono.ElapsedMilliseconds;
                    //shortestExists = closeList.Exists(predShortest);
                    shortestExists = closePaths.TryGetValue(neighbor, out shortest);
                    shortestExists = shortestExists && shortest <= weightExtended;
                    searchInCloseList += chrono.ElapsedMilliseconds;
                }
                /**/

                // if no path found in any list, search index of first path longer than extended path
                if (!shortestExists)
                {
                    System.Predicate<Path> predInsertion = (Path p) =>
                    {
                        return p.Weight > weightExtended;
                    };

                    extendPathChrono -= chrono.ElapsedMilliseconds;
                    clonePathChrono -= chrono.ElapsedMilliseconds;
                    Path pathExtended = new Path(pathToExtend);
                    clonePathChrono += chrono.ElapsedMilliseconds;

                    pathExtended.AddStep(neighbor);
                    extendPathChrono += chrono.ElapsedMilliseconds;

                    searchInsertionChrono -= chrono.ElapsedMilliseconds;
                    int indexInsertion = openList.FindIndex(predInsertion);
                    searchInsertionChrono += chrono.ElapsedMilliseconds;

                    // if no longer path found, add the extended path at end
                    if (indexInsertion == -1)
                    {
                        openList.Add(pathExtended);
                    }
                    else // otherwise, insert path at right index
                    {
                        openList.Insert(indexInsertion, pathExtended);
                    }
                }

            }
            foreachNeighborsChrono += chrono.ElapsedMilliseconds;

            //closeList.Add(pathToExtend);
            closePaths.Add(stepToExtend, pathToExtend.Weight);

            openList.RemoveAt(0);
        }
        _infos = StopChrono(_Start, _End);
        return null;
    }

    /// <summary>
    /// Search a path with AStar algorithm
    /// </summary>
    /// <param name="_Start">Tile to start from</param>
    /// <param name="_End">Tile to reach</param>
    /// <returns>the shortest path from start tile to end tile if it exists, null otherwise</returns>
    public static Path AstarWiki(GameTile _Start, GameTile _End, out ChronoInfos _infos)
    {
        if (!_Start || !_End)
        {
            _infos = ChronoInfos.InfosNone;
            return null;
        }

        chrono.Restart();
        HashSet<GameTile> closeHashSet = new HashSet<GameTile>();
        PathHeuristic firstPath = new PathHeuristic(_Start, 0);
        List<PathHeuristic> openList = new List<PathHeuristic>() { firstPath };

        PathHeuristic pathToExtend = null;
        GameTile stepToExtend = null;
        GameTile neighbor = null;
        int heuristic = 0;
        PathHeuristic extendedPath = null;
        int indexInsertion = 0;
        PathHeuristicComparer phComparer = new PathHeuristicComparer(false);
        while (openList.Count > 0)
        {
            pathToExtend = openList.Last();
            openList.RemoveAt(openList.Count - 1);
            stepToExtend = pathToExtend.LastStep;
            if (stepToExtend == _End)
            {
                _infos = StopChrono(_Start, _End);
                return pathToExtend;
            }

            foreachNeighborsChrono -= chrono.ElapsedMilliseconds;
            for (int i = 0; i < stepToExtend.Neighbors.Count; i++)
            {
                neighbor = stepToExtend.Neighbors[i];
                if (neighbor.IsAccessible)
                {
                    heuristic = GetManhattanDistance(neighbor, _End);

                    searchInCloseListChrono -= chrono.ElapsedMilliseconds;
                    bool canExtend = !closeHashSet.Contains(neighbor);
                    searchInCloseListChrono += chrono.ElapsedMilliseconds;

                    searchInOpenListChrono -= chrono.ElapsedMilliseconds;
                    canExtend = canExtend && !openList.Exists(p => p.LastStep == neighbor && p.WeightWithHeuristic <= pathToExtend.Weight + neighbor.Weight + heuristic);
                    searchInOpenListChrono += chrono.ElapsedMilliseconds;

                    //if (!(closeHashSet.Contains(neighbor) || openList.Exists(p => p.LastStep == neighbor && p.WeightWithHeuristic <= pathToExtend.Weight + neighbor.Weight + heuristic)))
                    if (canExtend)
                    {
                        clonePathChrono -= chrono.ElapsedMilliseconds;
                        extendedPath = new PathHeuristic(pathToExtend);
                        clonePathChrono += chrono.ElapsedMilliseconds;

                        extendPathChrono -= chrono.ElapsedMilliseconds;
                        extendedPath.AddStep(neighbor, heuristic);
                        extendPathChrono += chrono.ElapsedMilliseconds;

                        searchInsertionChrono -= chrono.ElapsedMilliseconds;
                        indexInsertion = openList.BinarySearch(extendedPath, phComparer);
                        if (indexInsertion < 0)
                        {
                            indexInsertion = ~indexInsertion;
                        }
                        searchInsertionChrono += chrono.ElapsedMilliseconds;

                        insertToOpenListChrono -= chrono.ElapsedMilliseconds;
                        openList.Insert(indexInsertion, extendedPath);
                        insertToOpenListChrono += chrono.ElapsedMilliseconds;
                    }
                }
            }
            foreachNeighborsChrono += chrono.ElapsedMilliseconds;

            closeHashSet.Add(stepToExtend);
        }

        _infos = StopChrono(_Start, _End);
        return null;
    }

    public static Path AStarCustomBasic(GameTile _Start, GameTile _End, out ChronoInfos _infos)
    {
        removeFromOpenChrono = 0f;
        foreachNeighborsChrono = 0f;
        clonePathChrono = 0f;
        extendPathChrono = 0f;
        searchInOpenListChrono = 0f;
        searchInCloseListChrono = 0f;
        searchInsertionChrono = 0f;
        insertToOpenListChrono = 0f;

        chrono.Restart();
        if (!_Start || !_End)
        {
            _infos = ChronoInfos.InfosNone;
            return null;
        }

        List<PathHeuristic> openList = new List<PathHeuristic>(50) { new PathHeuristic(_Start, GetManhattanDistance(_Start, _End)) };
        //List<PathHeuristic> closeList = new List<PathHeuristic>(50);
        HashSet<GameTile> closeHashSet = new HashSet<GameTile>();

        PathHeuristic pathToExtend = null;
        GameTile stepToExtend = null;
        GameTile neighbor = null;
        int weightExtended = 0;
        int heuristic;
        bool shortestExist = false;
        System.Predicate<PathHeuristic> shortestPredicate = (PathHeuristic path) =>
        {
            return path.LastStep == neighbor && path.WeightWithHeuristic <= weightExtended;
        };
        PathHeuristic extendedPath = null;
        int indexInsertion = -1;
        PathHeuristicComparer phComparer = new PathHeuristicComparer(true);

        while(openList.Count > 0)
        {
            pathToExtend = openList.First(); // keep open list sorted by ascending

            removeFromOpenChrono -= chrono.ElapsedMilliseconds;
            openList.RemoveAt(0);
            removeFromOpenChrono += chrono.ElapsedMilliseconds;

            stepToExtend = pathToExtend.LastStep;
            if (stepToExtend == _End)
            {
                _infos = StopChrono(_Start, _End);
                return pathToExtend;
            }

            //closeList.Add(pathToExtend);
            closeHashSet.Add(stepToExtend);

            foreachNeighborsChrono -= chrono.ElapsedMilliseconds;
            for(int i = 0; i < stepToExtend.Neighbors.Count; i++)
            {
                neighbor = stepToExtend.Neighbors[i];
                if (!neighbor.IsAccessible)
                    continue;

                heuristic = GetManhattanDistance(neighbor, _End);
                weightExtended = pathToExtend.Weight + neighbor.Weight + heuristic;

                searchInCloseListChrono -= chrono.ElapsedMilliseconds;
                //shortestExist = closeList.Exists(shortestPredicate);
                shortestExist = closeHashSet.Contains(neighbor);
                searchInCloseListChrono += chrono.ElapsedMilliseconds;

                searchInOpenListChrono -= chrono.ElapsedMilliseconds;
                shortestExist = shortestExist || openList.Exists(shortestPredicate);
                searchInOpenListChrono += chrono.ElapsedMilliseconds;

                if (shortestExist)
                    continue;

                clonePathChrono -= chrono.ElapsedMilliseconds;
                extendedPath = new PathHeuristic(pathToExtend);
                clonePathChrono += chrono.ElapsedMilliseconds;

                extendPathChrono -= chrono.ElapsedMilliseconds;
                extendedPath.AddStep(neighbor, heuristic);
                extendPathChrono += chrono.ElapsedMilliseconds;

                searchInsertionChrono -= chrono.ElapsedMilliseconds;
                indexInsertion = openList.BinarySearch(extendedPath, phComparer);
                if (indexInsertion < 0)
                {
                    indexInsertion = ~indexInsertion;
                }
                searchInsertionChrono += chrono.ElapsedMilliseconds;

                insertToOpenListChrono -= chrono.ElapsedMilliseconds;
                openList.Insert(indexInsertion, extendedPath);
                insertToOpenListChrono += chrono.ElapsedMilliseconds;
            }
            foreachNeighborsChrono += chrono.ElapsedMilliseconds;
        }

        _infos = StopChrono(_Start, _End);
        return null;
    }

    public static Path SearchHPAFromTo(GameTile _Start, GameTile _End, out ChronoInfos _infos)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Private Methods
    private static int GetManhattanDistance(GameTile _TileA, GameTile _TileB)
    {
        if (!_TileA || !_TileB)
            return -1;
        return Mathf.Abs(_TileA.GridPosition.x - _TileB.GridPosition.x) + Mathf.Abs(_TileA.GridPosition.y - _TileB.GridPosition.y);
    }

    private static ChronoInfos StopChrono(GameTile _Start, GameTile _End)
    {
        chrono.Stop();
        string log = $"from [{_Start.GridPosition.y}, {_Start.GridPosition.x}] to [{_End.GridPosition.y}, {_End.GridPosition.x}]\n";
        log += $"elasped time : {chrono.ElapsedMilliseconds} ms\n";
        log += $"visit all neighbors : {foreachNeighborsChrono} ms\n";
        log += $"clone path to extend : {clonePathChrono} ms\n";
        log += $"extend path : {extendPathChrono} ms\n";
        log += $"search in open list : {searchInOpenListChrono} ms\n";
        log += $"search in close list : {searchInCloseListChrono} ms\n";
        log += $"search index in open list to insert : {searchInsertionChrono} ms\n";
        log += $"insert to open list : {insertToOpenListChrono} ms\n";
        Debug.Log(log);

        ChronoInfos infos = new ChronoInfos();
        infos.ElapsedTime = chrono.ElapsedMilliseconds;
        infos.ForeachNeighborsChrono = foreachNeighborsChrono;
        infos.ClonePathChrono = clonePathChrono;
        infos.ExtendPathChrono = extendPathChrono;
        infos.SearchInOpenListChrono = searchInOpenListChrono;
        infos.SearchInCloseListChrono = searchInCloseListChrono;
        infos.SearchInsertionChrono = searchInsertionChrono;
        infos.InsertToOpenListChrono = insertToOpenListChrono;
        return infos;
    }
    #endregion

    #region Enumerations
    public enum PathfindingMode
    {
        PM_None,
        PM_Dijkstra,
        PM_AStar,
        PM_HPA
    }
    #endregion
}
