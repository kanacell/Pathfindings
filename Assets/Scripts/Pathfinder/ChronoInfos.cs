using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChronoInfos
{
	#region Public Methods
	public static ChronoInfos InfosNone
	{
		get
		{
			ChronoInfos infos = new ChronoInfos();
			infos.ElapsedTime = 0f;
			infos.RemoveFromOpenChrono = 0f;
			infos.ForeachNeighborsChrono = 0f;
			infos.ClonePathChrono = 0f;
			infos.ExtendPathChrono = 0f;
			infos.SearchInOpenListChrono = 0f;
			infos.SearchInCloseListChrono = 0f;
			infos.SearchInsertionChrono = 0f;
			infos.InsertToOpenListChrono = 0f;
			infos.CreateSolutionChrono = 0f;
			return infos;
		}
	}
	#endregion
	#region Public Attributes
	public float ElapsedTime;
	public float RemoveFromOpenChrono;
	public float ForeachNeighborsChrono;
	public float ClonePathChrono;
	public float ExtendPathChrono;
	public float SearchInOpenListChrono;
	public float SearchInCloseListChrono;
	public float SearchInsertionChrono;
	public float InsertToOpenListChrono;
	public float CreateSolutionChrono;
	#endregion
}
