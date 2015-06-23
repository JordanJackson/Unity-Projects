using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// class containing all map data and initialization
public class Map {

	public Tile[,] tiles;				// tile list (map data)
	public List<Room> roomList;			// list of Room regions in map
	public List<Corridor> corridorList;	// list of Corridor regions connecting rooms
	public int sizeX, sizeY;			// map dimensions

	// constructor
	public Map (int x, int y) {
		sizeX = x;
		sizeY = y;

		tiles = new Tile[sizeX,sizeY];
		roomList = new List<Room> ();
		corridorList = new List<Corridor> ();
		InitializeTileMap ();
		SetTileAdjacency ();
	}

	// initializes the TileMap to default type of Wall
	private void InitializeTileMap() {
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				tiles[x,y] = new Tile(Tile.TileType.Wall);
			}
		}
	}

	// initializes each tiles adjacent references
	private void SetTileAdjacency() {
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				if (x - 1 >= 0) 
					tiles[x,y].westAdj = tiles[x-1,y];
				if (x + 1 < sizeX)
					tiles[x,y].eastAdj = tiles[x+1,y];
				if (y - 1 >= 0)
					tiles[x,y].northAdj = tiles[x,y-1];
				if (y + 1 < sizeY)
					tiles[x,y].southAdj = tiles[x,y+1];
			}
		}
	}
}
