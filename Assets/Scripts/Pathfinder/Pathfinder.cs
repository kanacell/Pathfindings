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

				// predicate to find a path with the same tile as last step and with a weight inferior or equals
				System.Predicate<Path> pred = (Path p) =>
				{
					return p.LastStep == neighbor && p.Weight <= weightExtended;
				};

				// search in open list if a path already reached the same tile with a inferior or equals weight
				int index = openList.FindIndex(pred);
				if (index == -1)
				{
					// if no path found in open list, search also in the close list
					index = closeList.FindIndex(pred);
				}

				// if no path found in any list, search index of first path greater than path extended
				if (index == -1)
				{
					System.Predicate<Path> predInsertion = (Path p) =>
					{
						return p.Weight > weightExtended;
					};
					int indexInsertion = openList.FindIndex(predInsertion);
					Path pathExtended = new Path(pathToExtend);
					pathExtended.AddStep(neighbor);
					// if no greater path found, add the extended path at end
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
    #endregion

    #region Public Attributes
    #endregion

    #region Protected Attributes
    #endregion

    #region Private Attributes
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
