
public struct ChronoInfos
{
	#region Public Methods
	public static ChronoInfos InfosNone
	{
		get
		{
			ChronoInfos infos = new ChronoInfos();
			infos.PathFound = false;
			infos.ElapsedTime = 0f;
			infos.RemoveFromOpenChrono = 0f;
			infos.ForeachNeighborsChrono = 0f;
			infos.ClonePathChrono = 0f;
			infos.ExtendPathChrono = 0f;
			infos.SearchInCloseListChrono = 0f;
			infos.SearchInsertionChrono = 0f;
			infos.InsertToOpenListChrono = 0f;
			infos.CreateSolutionChrono = 0f;
			return infos;
		}
	}

	public string ToLogMessage()
	{
		string log = $"Path found : {PathFound}\n";
		log += $"Elapsed Time : {ElapsedTime} ms\n";
		log += $"Remove from open : {RemoveFromOpenChrono} ms\n";
		log += $"Foreach neighbors : {ForeachNeighborsChrono} ms\n";
		log += $"Clone current path : {ClonePathChrono} ms\n";
		log += $"Extend current path : {ExtendPathChrono} ms\n";
		log += $"Check exist closeList : {SearchInCloseListChrono} ms\n";
		log += $"Search index openList : {SearchInsertionChrono} ms\n";
		log += $"Insert openList : {InsertToOpenListChrono} ms\n";
		log += $"Create solution : {CreateSolutionChrono} ms\n";
		return log;
	}
	#endregion

	#region Public Attributes
	public bool PathFound;
	public float ElapsedTime;
	public float RemoveFromOpenChrono;
	public float ForeachNeighborsChrono;
	public float ClonePathChrono;
	public float ExtendPathChrono;
	public float SearchInCloseListChrono;
	public float SearchInsertionChrono;
	public float InsertToOpenListChrono;
	public float CreateSolutionChrono;
	#endregion
}
