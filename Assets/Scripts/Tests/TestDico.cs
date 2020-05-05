using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDico : MonoBehaviour
{
	#region Public Methods
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	[ContextMenu("Test Dico")]
	private void TestFunction()
	{
		Dictionary<Bridge, int> dico = new Dictionary<Bridge, int>();

		Tile t1 = new Tile(0, 0, 0);
		Tile t2 = new Tile(1, 1, 1);
		Bridge b1 = new Bridge(t1, t2);
		Bridge b2 = new Bridge(t1, t2);

		dico.Add(b1, 0);

		string message = $"b1 == b2 : {b1 == b2}\n";
		message += $"b1 equals b2 : {b1.Equals(b2)}\n";
		message += $"dico.containsKey(b1) : {dico.ContainsKey(b1)}\n";
		message += $"dico.containsKey(b2) : {dico.ContainsKey(b2)}\n";
		message += $"dico.containsKey(new Bridge) : {dico.ContainsKey(new Bridge(t1, t2))}\n";
		message += $"{dico[new Bridge(t1, t2)]}\n";
		Debug.Log(message);
	}
	#endregion

	#region Getters/Setters
	#endregion

	#region Public Attributes
	#endregion

	#region Protected Attributes
	#endregion

	#region Private Attributes
	#endregion
}
