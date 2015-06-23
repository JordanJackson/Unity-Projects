using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Region {
	
	public List<Tile> tileList;
	public List<Region> connectionsList;
	public bool isConnected;
	
	public int identifier { get; set; }
	
	public Region() {
		tileList = new List<Tile> ();
		connectionsList = new List<Region> ();
		isConnected = false;
	}
	
	public void AddTileToList (Tile t) {
		tileList.Add (t);
	}
	
	public void RemoveTileFromList (Tile t) {
		tileList.Remove (t);
	}
	
	public void AddRegionToList (Region r) {
		connectionsList.Add (r);
		isConnected = true;
	}
	
	public void RemoveRegionFromList (Region r) {
		connectionsList.Remove (r);
		if (connectionsList.Count <= 0) {
			isConnected = false;
		}
	}
}
