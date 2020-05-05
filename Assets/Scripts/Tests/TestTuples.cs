using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTuples : MonoBehaviour
{
	#region Public Methods
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	private void Start()
	{
		/**
		Tuple<int, int> t1 = new Tuple<int, int>(5, 7);
		Tuple<int, int> t2 = new Tuple<int, int>(5, 7);

		Dictionary<Tuple<int, int>, string> dico = new Dictionary<Tuple<int, int>, string>();
		dico.Add(t1, "coucou");
		dico.Add(t2, "au revoir");
		/**/

		Tile a = new Tile(0, 0, 0);
		Tile b = new Tile(1, 1, 1);
		Tuple<Tile, Tile> t1 = new Tuple<Tile, Tile>(a, b);
		Tuple<Tile, Tile> t2 = new Tuple<Tile, Tile>(a, b);

		Dictionary<Tuple<Tile, Tile>, string> dico = new Dictionary<Tuple<Tile, Tile>, string>();
		dico.Add(t1, "coucou");


		string message = $"operator == : {t1 == t2}\n";
		message += $"equals : {t1.Equals(t2)}\n";
		message += $"dico.containsKey : {dico.ContainsKey(new Tuple<Tile, Tile>(b, a))}\n";
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
