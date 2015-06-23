using UnityEngine;
using System.Collections;

public class Corridor : Region {

	public Corridor() {

	}

	public Corridor(int id) {
		identifier = id;
		isConnected = false;
	}
}
