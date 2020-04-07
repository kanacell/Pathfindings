using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
	#region Public Methods
	/// <summary>
	/// Search a path with Dijkstra algorithm
	/// </summary>
	/// <param name="_Start">Tile to start from</param>
	/// <param name="_End">Tile to reach</param>
	/// <returns>the shortest path from start tile to end tile if it exists, null otherwise</returns>
	public static Path SearchDijkstraPathFromTo(GameTile _Start, GameTile _End)
	{
		if (!_Start || !_End)
			return null;

		Path firstPath = new Path(_Start);
		if (_Start == _End)
			return firstPath;

		List<Path> openList = new List<Path> { firstPath };
		List<Path> closeList = new List<Path>();

		while(openList.Count > 0)
		{
			// get the path at index 0 to extend. The open list is assumed as always sorted by path weight
			Path pathToExtend = openList[0];

			// if last step is the end, then return this path
			if (pathToExtend.LastStep == _End)
				return pathToExtend;

			int weightToExtend = pathToExtend.Weight;
			GameTile stepToExtend = pathToExtend.LastStep;

			// loop through all neighbors from step to extend
			for(int i = 0; i < stepToExtend.Neighbors.Count; i++)
			{
				GameTile neighbor = stepToExtend.Neighbors[i];

				// if inaccessible neighbor, skip iteration
				if (!neighbor.IsAccessible)
					continue;

				int weightExtended = weightToExtend + neighbor.Weight;

				// predicate to find a shortest or same length path to reach the same tile
				System.Predicate<Path> pred = (Path p) =>
				{
					return p.LastStep == neighbor && p.Weight <= weightExtended;
				};

				// search in open list if a shortest path already reached the same tile
				int index = openList.FindIndex(pred);
				if (index == -1)
				{
					// if no path found in open list, search also in the close list
					index = closeList.FindIndex(pred);
				}

				// if no path found in any list, search index of first path longer than extended path
				if (index == -1)
				{
					System.Predicate<Path> predInsertion = (Path p) =>
					{
						return p.Weight > weightExtended;
					};
					int indexInsertion = openList.FindIndex(predInsertion);
					Path pathExtended = new Path(pathToExtend);
					pathExtended.AddStep(neighbor);

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

			closeList.Add(pathToExtend);
			openList.RemoveAt(0);
		}

		return null;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="_Start"></param>
	/// <param name="_End"></param>
	/// <returns></returns>
	public static Path SearchAStarPathFromTo(GameTile _Start, GameTile _End)
	{
		if (!_Start || !_End)
			return null;

		PathHeuristic firstPath = new PathHeuristic(_Start, GetManhattanDistance(_Start, _End));
		if (_Start == _End)
			return firstPath;

		List<PathHeuristic> openList = new List<PathHeuristic> { firstPath };
		List<PathHeuristic> closeList = new List<PathHeuristic>();

		while(openList.Count > 0)
		{
			// get the path at index 0 to extend. The open list is assumed as always sorted by path weight
			PathHeuristic pathToExtend = openList[0];

			// if last step is the end, then return this path
			if (pathToExtend.LastStep == _End)
				return pathToExtend;

			int weightToExtend = pathToExtend.Weight;
			GameTile stepToExtend = pathToExtend.LastStep;

			for (int i = 0; i < stepToExtend.Neighbors.Count; i++)
			{
				GameTile neighbor = stepToExtend.Neighbors[i];
				if (!neighbor.IsAccessible)
					continue;

				int heuristic = GetManhattanDistance(neighbor, _End);
				int weightExtendedWithHeuristic = weightToExtend + neighbor.Weight + heuristic;

				// predicate to find a shortest or same length path to reach the same tile
				System.Predicate<PathHeuristic> predShortest = (PathHeuristic p) =>
				{
					return p.LastStep == neighbor && p.WeightWithHeuristic <= weightExtendedWithHeuristic;
				};

				// search in open list if a shortest path already reached the same tile
				int index = openList.FindIndex(predShortest);
				if (index == -1)
				{
					// if no path found in open list, search also in the close list
					index = closeList.FindIndex(predShortest);
				}

				// if no path found in any list, search index of first path longer than extended path
				if (index == -1)
				{
					System.Predicate<PathHeuristic> predInsertion = (PathHeuristic p) =>
					{
						return p.Weight > weightExtendedWithHeuristic;
					};
					
					int indexInsertion = openList.FindIndex(predInsertion);
					PathHeuristic pathExtended = new PathHeuristic(pathToExtend);
					pathExtended.AddStep(neighbor, heuristic);

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

			closeList.Add(pathToExtend);
			openList.RemoveAt(0);
		}

		return null;
	}
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
	private static int GetManhattanDistance(GameTile _TileA, GameTile _TileB)
	{
		if (!_TileA || !_TileB)
			return -1;
		return Mathf.Abs(_TileA.GridPosition.x - _TileB.GridPosition.x) + Mathf.Abs(_TileA.GridPosition.y - _TileB.GridPosition.y);
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
