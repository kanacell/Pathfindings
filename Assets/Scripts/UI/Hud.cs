using Boo.Lang;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
	#region Public Methods
	public void SetHoveredCoords(Tile _Tile)
	{
		if (!m_HoveredCoords || _Tile == null)
			return;
		m_HoveredCoords.text = $"[{_Tile.Row}; {_Tile.Column}]";
	}

	public void CancelHoveredCoords()
	{
		if (!m_HoveredCoords)
			return;

		m_HoveredCoords.text = "[--;--]";
	}

	public void SetStartCoords(Tile _Tile)
	{
		if (!m_StartCoords || _Tile == null)
			return;
		m_StartCoords.text = $"[{_Tile.Row}; {_Tile.Column}]";
	}

	public void SetEndCoords(Tile _Tile)
	{
		if (!m_GoalCoords || _Tile == null)
			return;
		m_GoalCoords.text = $"[{_Tile.Row}; {_Tile.Column}]";
	}

	public void SetChronoLog(ChronoInfos _Infos)
	{
		if (!m_ChronoLog)
			return;
		m_ChronoLog.text = _Infos.ToLogMessage();
	}
	#endregion

	#region Private Methods
	private void Start()
	{
		if (m_GenerateMapButton && m_MapNameInputField)
		{
			m_GenerateMapButton.onClick.AddListener(() => GenerateMap());
		}

		if (m_GenerateClustersButton && m_ClusterRowsInputField && m_ClusterColumnsInputField)
		{
			m_GenerateClustersButton.onClick.AddListener(() => GenerateClusters());
		}

		if (m_SearchButton && m_SearchModeChoice)
		{
			m_SearchButton.onClick.AddListener(() => Search());
		}
	}

	private void GenerateMap()
	{
		OnCreateMap?.Invoke(m_MapNameInputField.text);
	}

	private void GenerateClusters()
	{
		int rowValue = 0;
		int.TryParse(m_ClusterRowsInputField.text, out rowValue);
		int columnValue = 0;
		int.TryParse(m_ClusterColumnsInputField.text, out columnValue);
		OnGenerateCluster?.Invoke(rowValue, columnValue);
	}

	private void Search()
	{
		string searchChoice = m_SearchModeChoice.options[m_SearchModeChoice.value].text;

		Debug.Log(searchChoice);

		OnSearch?.Invoke(Pathfinder.ConvertToPathMode(searchChoice));
	}
	#endregion

	#region Private Attributes
	[Header("Map Generation")]
	[SerializeField] private TMP_InputField m_MapNameInputField = null;
	[SerializeField] private Button m_GenerateMapButton = null;

	[Header("Clusters Settings")]
	[SerializeField] private TMP_InputField m_ClusterRowsInputField = null;
	[SerializeField] private TMP_InputField m_ClusterColumnsInputField = null;
	[SerializeField] private Button m_GenerateClustersButton = null;

	[Header("Tiles Infos")]
	[SerializeField] private TextMeshProUGUI m_HoveredCoords = null;
	[SerializeField] private TextMeshProUGUI m_StartCoords = null;
	[SerializeField] private TextMeshProUGUI m_GoalCoords = null;
	[SerializeField] private TextMeshProUGUI m_ChronoLog = null;

	[Header("Search Settings")]
	[SerializeField] private TMP_Dropdown m_SearchModeChoice = null;
	[SerializeField] private Button m_SearchButton = null;
	#endregion

	#region Events
	public event System.Action<string> OnCreateMap = null;
	public event System.Action<int, int> OnGenerateCluster = null;
	public event System.Action<Pathfinder.PathfindingMode> OnSearch = null;
	#endregion
}
