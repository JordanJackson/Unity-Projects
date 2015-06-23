using UnityEngine;
using System.Collections;

public class Tile {

	// adjacent Tiles
	public Tile northAdj { get; set; }
	public Tile eastAdj { get; set; }
	public Tile southAdj { get; set; }
	public Tile westAdj { get; set; }
	
	public enum TileType { Void, Wall, Room, Corridor, Connection };
	
	public TileType type { get; set; }
	
	public Region parentRegion { get; set; }
	
	public Tile (TileType t) {
		type = t;
	}
	
}
