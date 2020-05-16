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
    private static float searchInCloseListChrono = 0f;
    private static float searchInsertionChrono = 0f;
    private static float insertToOpenListChrono = 0f;
    private static float createSolutionChrono = 0f;
    private static System.Diagnostics.Stopwatch chrono = new System.Diagnostics.Stopwatch();

    #region Public Methods
    public static PathfindingMode ConvertToPathMode(string _Input)
    {
        _Input = "PM_" + _Input;
        string[] names = System.Enum.GetNames(typeof(PathfindingMode));
        PathfindingMode[] values = (PathfindingMode[]) System.Enum.GetValues(typeof(PathfindingMode));
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == _Input)
                return values[i];
        }
        return PathfindingMode.PM_None;
    }

    /// <summary>
    /// Search a path with Dijkstra algorithm.
    /// </summary>
    /// <param name="_Start">Tile to start from</param>
    /// <param name="_End">Tile to reach</param>
    /// <param name="_ChronoInfos">Struct with times value to evaluate performance</param>
    /// <param name="_MaxSeconds">Max time allowed to search a solution. If value is negative, no time limit applied</param>
    /// <returns>the shortest path from start tile to end tile if it exists, null otherwise</returns>
    public static Path SearchDijkstraPathFromTo(Tile _Start, Tile _End, out ChronoInfos _ChronoInfos, float _TimeLimit)
    {
        StartChrono();

        if (_Start == null || !_Start.IsAccessible || _End == null || !_End.IsAccessible)
        {
            _ChronoInfos = ChronoInfos.InfosNone;
            return null;
        }

        List<Path> openList = new List<Path> { new Path(_Start) };
        HashSet<Tile> closeHashSet = new HashSet<Tile>();
        // Use a comparer to sort the open list by descending order on path weight.
        // This way, the shortest path in open list is at end
        //PathComparer pComparer = new PathComparer(false);
        CustomComparer<Path> pComparer = new CustomComparer<Path>(false);

        while (openList.Count > 0)
        {
            // Stop search if the chrono reached the time limit.
            // It avoids to get infinite loop or too long to find a solution. Mainly usefull to debug
            if (_TimeLimit > 0 && chrono.Elapsed.TotalSeconds > _TimeLimit)
            {
                _ChronoInfos = StopChrono(_Start, _End, false);
                return null;
            }

            // Take the last path (which is the shortest as we sort the open list by descending weight)
            Path pathToExtend = openList.Last();

            // If last tile is the end, then return this path
            if (pathToExtend.LastStep == _End)
            {
                _ChronoInfos = StopChrono(_Start, _End, true);
                return pathToExtend;
            }

            // Remove the current path from open list
            removeFromOpenChrono -= chrono.ElapsedMilliseconds;
            openList.RemoveAt(openList.Count - 1);
            removeFromOpenChrono += chrono.ElapsedMilliseconds;

            // If the last tile was already reach, skip the whole process on the current path
            Tile tileToExtend = pathToExtend.LastStep;
            if (closeHashSet.Contains(tileToExtend))
                continue;

            // Add the last tile to the close list (HashSet)
            closeHashSet.Add(tileToExtend);

            foreachNeighborsChrono -= chrono.ElapsedMilliseconds;
            // loop through all neighbors from step to extend
            for (int i = 0; i < tileToExtend.Neighbors.Count; i++)
            {
                Tile neighbor = tileToExtend.Neighbors[i];

                // if neighbor is inaccessible (wall) , skip iteration
                if (!neighbor.IsAccessible)
                    continue;

                // search in the close list if the neighbor was already reached
                searchInCloseListChrono -= chrono.ElapsedMilliseconds;
                bool shortestExists = closeHashSet.Contains(neighbor);
                searchInCloseListChrono += chrono.ElapsedMilliseconds;

                if (shortestExists)
                    continue;

                // Clone the current path to extend
                clonePathChrono -= chrono.ElapsedMilliseconds;
                Path extendedPath = new Path(pathToExtend);
                clonePathChrono += chrono.ElapsedMilliseconds;

                // Extend path with the current valid neighbor
                extendPathChrono -= chrono.ElapsedMilliseconds;
                extendedPath.AddStep(neighbor);
                extendPathChrono += chrono.ElapsedMilliseconds;

                // Search in open list where to insert the extended path
                searchInsertionChrono -= chrono.ElapsedMilliseconds;
                int indexInsertion = openList.BinarySearch(extendedPath, pComparer);
                if (indexInsertion < 0)
                {
                    indexInsertion = ~indexInsertion;
                }
                searchInsertionChrono += chrono.ElapsedMilliseconds;

                // Insert the extended path in open list
                insertToOpenListChrono -= chrono.ElapsedMilliseconds;
                openList.Insert(indexInsertion, extendedPath);
                insertToOpenListChrono += chrono.ElapsedMilliseconds;
            }
            foreachNeighborsChrono += chrono.ElapsedMilliseconds;
        }
        _ChronoInfos = StopChrono(_Start, _End, false);
        return null;
    }

    /// <summary>
    /// Search a path with Dijkstra algorithm.
    /// The open list is optimized with PathLink class and not Path class. This way, we do not duplicate a whole path when we extend it,
    /// we create PathLink insted and use it as LinkedList to simulate a path.
    /// </summary>
    /// <param name="_Start">Tile to start from</param>
    /// <param name="_End">Tile to reach</param>
    /// <param name="_ChronoInfos">Struct with times value to evaluate performance</param>
    /// <param name="_MaxSeconds">Max time allowed to search a solution. If value is negative, no time limit applied</param>
    /// <returns>the shortest path from start tile to end tile if it exists, null otherwise</returns>
    public static Path SearchDisjktraPathFromTo_LinkOpti(Tile _Start, Tile _End, out ChronoInfos _ChronoInfos, float _TimeLimit)
    {
        StartChrono();

        if (_Start == null || !_Start.IsAccessible || _End == null || !_End.IsAccessible)
        {
            _ChronoInfos = ChronoInfos.InfosNone;
            return null;
        }

        List<PathLink> openList = new List<PathLink>(100) { new PathLink(_Start) };
        HashSet<Tile> closeHashSet = new HashSet<Tile>();
        CustomComparer<PathLink> customComparer = new CustomComparer<PathLink>(false);

        while (openList.Count > 0)
        {
            // Stop search if the chrono reached the time limit.
            // It avoids to get infinite loop or too long to find a solution. Mainly usefull to debug
            if (_TimeLimit > 0 && chrono.Elapsed.TotalSeconds > _TimeLimit)
            {
                _ChronoInfos = StopChrono(_Start, _End, false);
                return null;
            }

            // Take the last (and so the shortest) path to extend
            PathLink pathToExtend = openList.Last();
            Tile tileToExtend = pathToExtend.Tile;

            // If the end is reach, return solution
            if (tileToExtend == _End)
            {
                _ChronoInfos = StopChrono(_Start, _End, true);
                return Path.CreatePath(pathToExtend);
            }

            // Remove the current path from open list
            removeFromOpenChrono -= chrono.ElapsedMilliseconds;
            openList.RemoveAt(openList.Count - 1);
            removeFromOpenChrono += chrono.ElapsedMilliseconds;

            // If the last tile was already reached, skip process to next path to extend
            if (closeHashSet.Contains(tileToExtend))
                continue;
            closeHashSet.Add(tileToExtend);

            foreachNeighborsChrono -= chrono.ElapsedMilliseconds;
            // Loop on  neighbors of last tile
            for (int i = 0; i < tileToExtend.Neighbors.Count; i++)
            {
                Tile neighbor = tileToExtend.Neighbors[i];
                // If neighbor is wall or already reached, skip process to next neighbor

                searchInCloseListChrono -= chrono.ElapsedMilliseconds;
                bool existInCloseList = closeHashSet.Contains(neighbor);
                searchInCloseListChrono += chrono.ElapsedMilliseconds;

                if (!neighbor.IsAccessible || existInCloseList)
                    continue;


                // Extend pathlink with the path to extend and the current neighbor
                extendPathChrono -= chrono.ElapsedMilliseconds;
                //PathLink extendedPath = new PathLink(pathToExtend, neighbor);
                PathLink extendedPath = pathToExtend.MakeExtensionWith(neighbor);
                extendPathChrono += chrono.ElapsedMilliseconds;

                // Search index to insert extended path in open list
                searchInsertionChrono -= chrono.ElapsedMilliseconds;
                int indexInsertion = openList.BinarySearch(extendedPath, customComparer);
                if (indexInsertion < 0)
                {
                    indexInsertion = ~indexInsertion;
                }
                searchInsertionChrono += chrono.ElapsedMilliseconds;

                // Insert the extended path in open list
                insertToOpenListChrono -= chrono.ElapsedMilliseconds;
                openList.Insert(indexInsertion, extendedPath);
                insertToOpenListChrono += chrono.ElapsedMilliseconds;
            }
            foreachNeighborsChrono += chrono.ElapsedMilliseconds;
        }

        _ChronoInfos = StopChrono(_Start, _End, false);
        return null;
    }

    /// <summary>
    /// Search a path with custom AStar algorithm
    /// (Path class is used to explore the grid)
    /// </summary>
    /// <param name="_Start">Tile to start from</param>
    /// <param name="_End">Tile to reach</param>
    /// <param name="_ChronoInfos">Struct with times value to evaluate performance</param>
    /// <param name="_MaxSeconds">Max time allowed to search a solution. If value is negative, no time limit applied</param>
    /// <returns>the shortest path from start tile to end tile if it exists, null otherwise</returns>
    public static Path AStarCustomBasic(Tile _Start, Tile _End, out ChronoInfos _ChronoInfos, float _MaxSeconds)
    {
        StartChrono();

        if (_Start == null || !_Start.IsAccessible || _End == null || !_End.IsAccessible)
        {
            _ChronoInfos = ChronoInfos.InfosNone;
            return null;
        }

        List<PathHeuristic> openList = new List<PathHeuristic>(50) { new PathHeuristic(_Start, Tile.GetManhattanDistance(_Start, _End)) };
        HashSet<Tile> closeHashSet = new HashSet<Tile>();
        CustomComparer<PathHeuristic> phComparer = new CustomComparer<PathHeuristic>(false);

        while (openList.Count > 0)
        {
            if (_MaxSeconds > 0 && chrono.Elapsed.TotalSeconds > _MaxSeconds)
            {
                _ChronoInfos = StopChrono(_Start, _End, false);
                return null;
            }

            PathHeuristic pathToExtend = openList.Last();

            removeFromOpenChrono -= chrono.ElapsedMilliseconds;
            openList.RemoveAt(openList.Count - 1);
            removeFromOpenChrono += chrono.ElapsedMilliseconds;

            Tile tileToExtend = pathToExtend.LastStep;
            if (tileToExtend == _End)
            {
                _ChronoInfos = StopChrono(_Start, _End, true);
                return pathToExtend;
            }

            if (closeHashSet.Contains(tileToExtend))
                continue;

            closeHashSet.Add(tileToExtend);

            foreachNeighborsChrono -= chrono.ElapsedMilliseconds;
            for (int i = 0; i < tileToExtend.Neighbors.Count; i++)
            {
                Tile neighbor = tileToExtend.Neighbors[i];
                if (!neighbor.IsAccessible)
                    continue;

                searchInCloseListChrono -= chrono.ElapsedMilliseconds;
                if (closeHashSet.Contains(neighbor))
                    continue;
                searchInCloseListChrono += chrono.ElapsedMilliseconds;

                clonePathChrono -= chrono.ElapsedMilliseconds;
                PathHeuristic extendedPath = new PathHeuristic(pathToExtend);
                clonePathChrono += chrono.ElapsedMilliseconds;

                extendPathChrono -= chrono.ElapsedMilliseconds;
                extendedPath.AddStep(neighbor, Tile.GetManhattanDistance(neighbor, _End));
                extendPathChrono += chrono.ElapsedMilliseconds;

                searchInsertionChrono -= chrono.ElapsedMilliseconds;
                int indexInsertion = openList.BinarySearch(extendedPath, phComparer);
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

        _ChronoInfos = StopChrono(_Start, _End, false);
        return null;
    }

    /// <summary>
    /// Search a path with custom AStar algorithm
    /// (PathLink is use to explore the grid. this way, there's no duplicate of tile array when we extend open list)
    /// </summary>
    /// <param name="_Start">Tile to start from</param>
    /// <param name="_End">Tile to reach</param>
    /// <param name="_ChronoInfos">Struct with times value to evaluate performance</param>
    /// <param name="_MaxSeconds">Max time allowed to search a solution. If value is negative, no time limit applied</param>
    /// <returns>the shortest path from start tile to end tile if it exists, null otherwise</returns>
    public static Path AStar_LinkOpti(Tile _Start, Tile _End, out ChronoInfos _ChronoInfos, float _MaxSeconds)
    {
        StartChrono();

        if (_Start == null || !_Start.IsAccessible || _End == null || !_End.IsAccessible)
        {
            _ChronoInfos = ChronoInfos.InfosNone;
            return null;
        }

        List<PathLinkHeuristic> openList = new List<PathLinkHeuristic>(100) { new PathLinkHeuristic(_Start, Tile.GetManhattanDistance(_Start, _End)) };
        HashSet<Tile> closeHashSet = new HashSet<Tile>();
        CustomComparer<PathLinkHeuristic> customComparer = new CustomComparer<PathLinkHeuristic>(true);

        while (openList.Count > 0)
        {
            if (_MaxSeconds > 0 && chrono.Elapsed.TotalSeconds > _MaxSeconds)
            {
                _ChronoInfos = StopChrono(_Start, _End, false);
                return null;
            }

            PathLinkHeuristic linkToExtend = openList.First();
            if (linkToExtend.Tile == _End)
            {
                _ChronoInfos = StopChrono(_Start, _End, true);
                createSolutionChrono -= chrono.ElapsedMilliseconds;
                Path solution = Path.CreatePath(linkToExtend);
                createSolutionChrono += chrono.ElapsedMilliseconds;
                return solution;
            }

            removeFromOpenChrono -= chrono.ElapsedMilliseconds;
            openList.RemoveAt(0);
            removeFromOpenChrono += chrono.ElapsedMilliseconds;

            if (closeHashSet.Contains(linkToExtend.Tile))
                continue;

            closeHashSet.Add(linkToExtend.Tile);
            Tile tileToExtend = linkToExtend.Tile;

            foreachNeighborsChrono -= chrono.ElapsedMilliseconds;
            for (int i = 0; i < tileToExtend.Neighbors.Count; i++)
            {
                Tile neighbor = tileToExtend.Neighbors[i];

                searchInCloseListChrono -= chrono.ElapsedMilliseconds;
                bool existInCloseList = closeHashSet.Contains(neighbor);
                searchInCloseListChrono += chrono.ElapsedMilliseconds;

                if (!neighbor.IsAccessible || existInCloseList)
                    continue;

                extendPathChrono -= chrono.ElapsedMilliseconds;
                //PathLinkHeuristic extendedLink = new PathLinkHeuristic(linkToExtend, neighbor, Tile.GetManhattanDistance(neighbor, _End));
                PathLinkHeuristic extendedLink = linkToExtend.MakeExtensionWith(neighbor, Tile.GetManhattanDistance(neighbor, _End));
                extendPathChrono += chrono.ElapsedMilliseconds;

                searchInsertionChrono -= chrono.ElapsedMilliseconds;
                int indexInsertion = openList.BinarySearch(extendedLink, customComparer);
                if (indexInsertion < 0)
                {
                    indexInsertion = ~indexInsertion;
                }
                searchInsertionChrono += chrono.ElapsedMilliseconds;

                insertToOpenListChrono -= chrono.ElapsedMilliseconds;
                openList.Insert(indexInsertion, extendedLink);
                insertToOpenListChrono += chrono.ElapsedMilliseconds;
            }
            foreachNeighborsChrono += chrono.ElapsedMilliseconds;
        }

        _ChronoInfos = StopChrono(_Start, _End, false);
        return null;
    }

    /// <summary>
    /// Search a path with HPA algorithm
    /// </summary>
    /// <param name="_Start">Tile to start from</param>
    /// <param name="_End">Tile to reach</param>
    /// <param name="_ChronoInfos">Struct with times value to evaluate performance</param>
    /// <param name="_MaxSeconds">Max time allowed to search a solution. If value is negative, no time limit applied</param>
    /// <returns>the shortest path from start tile to end tile if it exists, null otherwise</returns>
    public static Path SearchHPAFromTo(GridClustered _GridClustered, Tile _Start, Tile _End, out ChronoInfos _ChronoInfos, float _MaxSeconds)
    {
        StartChrono();

        if (_Start == null || !_Start.IsAccessible || _End == null || !_End.IsAccessible || _GridClustered == null)
        {
            _ChronoInfos = ChronoInfos.InfosNone;
            return null;
        }

        Cluster startCluster = _GridClustered.GetClusterOfTile(_Start);
        Cluster endCluster = _GridClustered.GetClusterOfTile(_End);
        Path instantSolution = null;
        if (startCluster == endCluster && (instantSolution = startCluster.ComputeInternalPathFromTo(_Start, _End)) != null)
        {
            _ChronoInfos = StopChrono(_Start, _End, true);
            return instantSolution;
        }

        List<BridgeStep> openList = new List<BridgeStep>();
        CustomComparer<BridgeStep> comparer = new CustomComparer<BridgeStep>(false);

        HashSet<Bridge> closeSet = new HashSet<Bridge>();

        // store all the possible bridges connected to the start tile with their path
        // populate the open list with these bridges as all possible first steps
        Dictionary<Tile, Path> allFirstParts = new Dictionary<Tile, Path>();
        for (int i = 0; i < startCluster.Bridges.Count; i++)
        {
            Path path = startCluster.ComputeInternalPathFromTo(_Start, startCluster.Bridges[i].Start);
            if (path != null)
            {
                allFirstParts.Add(startCluster.Bridges[i].Start, path);

                BridgeStep stepToAdd = new BridgeStep(startCluster.Bridges[i], path.Weight);
                int indexInsertion = openList.BinarySearch(stepToAdd, comparer);
                if (indexInsertion < 0)
                {
                    indexInsertion = ~indexInsertion;
                }
                openList.Insert(indexInsertion, stepToAdd);
            }
        }

        // store all the possible bridges connected to the end tile with their path
        Dictionary<Tile, Path> allLastParts = new Dictionary<Tile, Path>();
        for (int i = 0; i < endCluster.Bridges.Count; i++)
        {
            Path path = endCluster.ComputeInternalPathFromTo(endCluster.Bridges[i].Start, _End);
            if (path != null)
            {
                allLastParts.Add(endCluster.Bridges[i].Start, path);
            }
        }

        while (openList.Count > 0)
        {
            if (_MaxSeconds > 0 && chrono.Elapsed.TotalSeconds > _MaxSeconds)
            {
                _ChronoInfos = StopChrono(_Start, _End, false);
                return null;
            }

            BridgeStep stepToExtend = openList.Last();

            removeFromOpenChrono -= chrono.ElapsedMilliseconds;
            openList.RemoveAt(openList.Count - 1);
            removeFromOpenChrono += chrono.ElapsedMilliseconds;

            // if the step is already marked as solution, then it's the shortest solution
            if (stepToExtend.ReachedGoal)
            {
                // recreate whole path
                createSolutionChrono -= chrono.ElapsedMilliseconds;
                Path solution = stepToExtend.CreateCompletePath(_GridClustered, allFirstParts, allLastParts);
                createSolutionChrono += chrono.ElapsedMilliseconds;

                _ChronoInfos = StopChrono(_Start, _End, true);
                return solution;
            }

            Bridge bridgeToExtend = stepToExtend.Bridge;
            if (closeSet.Contains(bridgeToExtend))
                continue;

            closeSet.Add(bridgeToExtend);

            // when reach a bridge with an end connected to the goal (check in the dictionary allLastParts) for the first time, mark it as one possible solution
            // with the mark, add also the weight of the last part which connect the end bridge and the goal
            // then, re-insert it in the open list at the right index.
            // if we try to extend a bridge already marked, then it's the shortest solution

            // if the bridge to extend is a bridge with the end connected to the goal cluster (in dictionary allLastParts)
            if (endCluster.Bridges.Exists(bridge => bridge.Start == bridgeToExtend.End) && allLastParts.ContainsKey(bridgeToExtend.End))
            {
                // mark step as possible solution and add the weight of the right last part
                stepToExtend.MarkAsSolution(allLastParts[stepToExtend.Bridge.End].Weight);

                // find the right index to re-insert in open list
                // if we try to extend it again, it means that it's the shortest path to the goal
                int indexInsertion = openList.BinarySearch(stepToExtend, comparer);
                if (indexInsertion < 0)
                {
                    indexInsertion = ~indexInsertion;
                }
                openList.Insert(indexInsertion, stepToExtend);
            }
            else
            {
                // Get the cluster from the bridgeToExtend End
                Cluster cluster = _GridClustered.GetClusterOfTile(bridgeToExtend.End);

                // Get the right reversed bridge
                Bridge reverseBridge = cluster.Bridges.Find(b => b.Start == bridgeToExtend.End);

                // loop through all the reversed bridge neighbors
                foreachNeighborsChrono -= chrono.ElapsedMilliseconds;
                foreach (Bridge neighbor in reverseBridge.Neighbors.Keys)
                {
                    // if the bridge is already in close set, skip to next neighbor
                    searchInCloseListChrono -= chrono.ElapsedMilliseconds;
                    bool existInCloseList = closeSet.Contains(neighbor);
                    searchInCloseListChrono += chrono.ElapsedMilliseconds;
                    if (existInCloseList)
                        continue;

                    // create extension with neighbor
                    extendPathChrono -= chrono.ElapsedMilliseconds;
                    BridgeStep extension = stepToExtend.MakeExtensionWith(neighbor, reverseBridge.Neighbors[neighbor].Weight);
                    extendPathChrono += chrono.ElapsedMilliseconds;

                    searchInsertionChrono -= chrono.ElapsedMilliseconds;
                    int indexInsertion = openList.BinarySearch(extension, comparer);
                    if (indexInsertion < 0)
                    {
                        indexInsertion = ~indexInsertion;
                    }
                    searchInsertionChrono += chrono.ElapsedMilliseconds;

                    insertToOpenListChrono -= chrono.ElapsedMilliseconds;
                    openList.Insert(indexInsertion, extension);
                    insertToOpenListChrono += chrono.ElapsedMilliseconds;
                }
                foreachNeighborsChrono += chrono.ElapsedMilliseconds;
            }
        }

        // No path found in limited time
        _ChronoInfos = StopChrono(_Start, _End, false);
        return null;
    }
    #endregion

    #region Private Methods
    private static void StartChrono()
    {
        removeFromOpenChrono = 0f;
        foreachNeighborsChrono = 0f;
        searchInCloseListChrono = 0f;
        clonePathChrono = 0f;
        extendPathChrono = 0f;
        searchInsertionChrono = 0f;
        insertToOpenListChrono = 0f;
        createSolutionChrono = 0f;
        chrono.Restart();
    }

    private static ChronoInfos StopChrono(Tile _Start, Tile _End, bool _PathFound)
    {
        chrono.Stop();
        ChronoInfos infos = new ChronoInfos();
        infos.PathFound = _PathFound;
        infos.ElapsedTime = chrono.ElapsedMilliseconds;
        infos.ForeachNeighborsChrono = foreachNeighborsChrono;
        infos.ClonePathChrono = clonePathChrono;
        infos.ExtendPathChrono = extendPathChrono;
        infos.SearchInCloseListChrono = searchInCloseListChrono;
        infos.SearchInsertionChrono = searchInsertionChrono;
        infos.InsertToOpenListChrono = insertToOpenListChrono;
        infos.CreateSolutionChrono = createSolutionChrono;
        return infos;
    }
    #endregion

    #region Enumerations
    public enum PathfindingMode
    {
        PM_None,
        PM_Dijkstra,
        PM_Dijkstra_LinkOpti,
        PM_AStar,
        PM_AStar_LinkOpti,
        PM_HPA
    }
    #endregion
}
