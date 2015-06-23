using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
	
	public int desiredNumberOfRooms = 4;
	public int numRoomTries;
	
	public bool preview = false;
	
	public int roomSizeMin;
	public int roomSizeMax;
	public int roomSizeVariance;
	
	public int roomBuffer;

	public Map map;
	
	public int mapSizeX;
	public int mapSizeY;

	public float multipleConnectionChance;
	public float linearity;
	public bool useExcessCorridors;
	
	public SceneBuilder sceneBuilder;
	private List<ConnectionNode> connections;
	
	public void BuildMap() {

		sceneBuilder = GetComponent<SceneBuilder> ();

		map = new Map (mapSizeX, mapSizeY);
		// TODO: initialize starting room and ending room (most likely pre-built
		// also will wish to maximize/optimize distance between these two rooms
		AddRooms ();
		CarveRooms ();
		BuildMaze ();
		
		connections = new List<ConnectionNode> ();
		PopulateConnectionsList ();
		BuildConnections ();
		
		CleanUpCorridors ();
		
		SetUnseenToVoid ();
		Debug.Log ("Map built");
	}


	
	void AddRooms() {
		for (int i = 0; i < (numRoomTries*desiredNumberOfRooms) && map.roomList.Count < desiredNumberOfRooms; i++) {
			int size = Random.Range(roomSizeMin, roomSizeMax + roomSizeVariance) * 2 + 1;	// guarantees oddness
			int rectangular = Random.Range(0, 1 + size / 2) * 2;
			
			int width = size;
			int height = size;
			
			if ((int) Random.Range (0,1) == 1) {
				width += rectangular;
			}
			else {
				height += rectangular;
			}
			
			int xPos = Random.Range (0, (mapSizeX - width) / 2) * 2 + 1;
			int yPos = Random.Range (0, (map.tiles.GetLength (1) - height) / 2) * 2 + 1;
			
			Room newRoom = new Room(xPos, yPos, width, height);
			
			bool overlapping = false;
			
			foreach (Room otherRoom in map.roomList) {
				if (newRoom.Overlaps(otherRoom, roomBuffer)) {
					overlapping = true;
					break;
				}
			}
			
			if (overlapping) continue;
			
			map.roomList.Add(newRoom);
		}
	}

	void CarveRooms() {
		for (int i = 0; i < map.roomList.Count; i++) {
			map.roomList[i].identifier = i;
			CarveRoom(map.roomList[i]);
		}
	}
	
	// update tiles belonging to rooms
	void CarveRoom(Room room) {
		for (int x = room.x; x <= (int)room.xMax; x++) {
			for (int y = room.y; y <= room.yMax; y++) {
				map.tiles [x, y].type = Tile.TileType.Room;
				room.AddTileToList(map.tiles[x,y]);
				map.tiles[x,y].parentRegion = room;
			}
		}
	}
	
	enum Direction {Up, Right, Down, Left, All, None};
	
	// locate an empty and valid space to begin creating a corridor, calling GrowMaze
	// on each valid space
	void BuildMaze() {
		int corridorIndex = 0;
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				Tile tile = map.tiles[x,y];
				if (tile.type == Tile.TileType.Wall) {
					if (ValidToCarve(tile, Direction.All)) {
						Corridor c = new Corridor(corridorIndex);
						int[] order = {0,1,2,3};
						GrowMaze(c, tile, Direction.None, order);
						map.corridorList.Add(c);
					}
				}
			}
		}
	}
	
	void GrowMaze(Corridor c, Tile tile, Direction lastDirection, int[] lastOrder) {
		tile.type = Tile.TileType.Corridor;
		c.AddTileToList (tile);
		tile.parentRegion = c;
		int[] order = lastOrder;
		if (Random.Range (0.0f, 1.0f) >= linearity) {
			order = Shuffle (order);
		}
		
		foreach (int i in order) {
			
			switch (i) {
			case 0: {
				if (tile.eastAdj != null && lastDirection != Direction.Left) {
					if (ValidToCarve (tile.eastAdj, Direction.Up) && ValidToCarve (tile.eastAdj, Direction.Right) && ValidToCarve (tile.eastAdj, Direction.Down))
						GrowMaze (c, tile.eastAdj, Direction.Right, order);
				}
				break;
			}
			case 1: {
				if (tile.southAdj != null && lastDirection != Direction.Up) {
					if (ValidToCarve (tile.southAdj, Direction.Down) && ValidToCarve (tile.southAdj, Direction.Right) && ValidToCarve (tile.southAdj, Direction.Left))
						GrowMaze (c, tile.southAdj, Direction.Down, order);
				}
				break;
			}
			case 2: {
				if (tile.westAdj != null && lastDirection != Direction.Right) {
					if (ValidToCarve (tile.westAdj, Direction.Up) && ValidToCarve (tile.westAdj, Direction.Left) && ValidToCarve (tile.westAdj, Direction.Down))
						GrowMaze (c, tile.westAdj, Direction.Left, order);
				}
				break;
			}
			case 3: {
				if (tile.northAdj != null && lastDirection != Direction.Down) {
					if (ValidToCarve (tile.northAdj, Direction.Right) && ValidToCarve (tile.northAdj, Direction.Up) && ValidToCarve (tile.northAdj, Direction.Left))
						GrowMaze (c, tile.northAdj, Direction.Up, order);
				}
				break;
			}
			default:
				break;
			}
		}
	}

	int[] Shuffle(int[] a) {
		int[] array = a;
		int n = array.Length;

		for (int i = 0; i < n; i++) {
			int r = i + (int)(Random.Range (0.0f, 1.0f)*(n-i));
			int t = array[r];
			array[r] = array[i];
			array[i] = t;
		}

		return array;
	}
	
	void PopulateConnectionsList() {
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				if (map.tiles[x,y].type == Tile.TileType.Wall) {
					MarkPossibleConnections(x,y);
				}
			}
		}
	}
	
	int NumberOfCarvedNeighbours(int x, int y) {
		
		int numNeighbours = 0;
		
		if (x - 1 >= 0) {
			if (map.tiles [x - 1, y].type != Tile.TileType.Wall && map.tiles [x - 1, y].type != Tile.TileType.Void) {
				numNeighbours += 1;
			}
		}
		if (x + 1 < mapSizeX) {
			if (map.tiles [x+1, y].type != Tile.TileType.Wall && map.tiles [x+1, y].type != Tile.TileType.Void) {
				numNeighbours += 1;
			}
		}
		if (y - 1 >= 0) {
			if (map.tiles [x, y-1].type != Tile.TileType.Wall && map.tiles [x, y-1].type != Tile.TileType.Void) {
				numNeighbours += 1;
			}
		}
		if (y + 1 < mapSizeY) {
			if (map.tiles [x, y+1].type != Tile.TileType.Wall && map.tiles [x, y+1].type != Tile.TileType.Void) {
				numNeighbours += 1;
			}
		}
		return numNeighbours;
	}

	void MarkPossibleConnections(int x, int y) {
		
		if (NumberOfCarvedNeighbours (x, y) >= 2) {
			List<Region> neighbouringRegions = new List<Region>();
			
			if (x - 1 >= 0) {
				if (map.tiles [x - 1, y].type != Tile.TileType.Wall) {
					neighbouringRegions.Add (map.tiles[x-1, y].parentRegion);
				}
			}
			if (x + 1 < mapSizeX) {
				if (map.tiles [x+1, y].type != Tile.TileType.Wall) {
					neighbouringRegions.Add (map.tiles[x+1, y].parentRegion);
				}
			}
			if (y - 1 >= 0) {
				if (map.tiles [x, y-1].type != Tile.TileType.Wall) {
					neighbouringRegions.Add (map.tiles[x, y-1].parentRegion);
				}
			}
			if (y + 1 < mapSizeY) {
				if (map.tiles [x, y+1].type != Tile.TileType.Wall) {
					neighbouringRegions.Add (map.tiles[x, y+1].parentRegion);
				}
			}
			
			for (int i = 0; i < neighbouringRegions.Count; i++) {
				for (int j = 0; j < neighbouringRegions.Count; j++) {
					if (i != j && neighbouringRegions[i] != neighbouringRegions[j]) {
						connections.Add (new ConnectionNode(neighbouringRegions[i], neighbouringRegions[j], x, y));
					}
				}
			}
		}
	}
	/*
	void ConnectMap() {
		BuildConnections (map.roomList [0]);
	}

	void BuildConnections(Region region) {
		// initialize connection list for this room
		List<ConnectionNode> nodes = new List<ConnectionNode> ();

		bool done = false;
		while (!done) {
			nodes.Clear();
				// populate nodes with connections that contain room as a region
			nodes = GetRelatedConnections(region);

			if (nodes.Count == 0) {
				done = true;
				continue;
			}
			int k = (int) Random.Range (0, nodes.Count);

			ConnectionNode currentNode = nodes[k];

			Region otherRegion = currentNode.GetOtherRegion(region);

			if (otherRegion.isConnected) {
				float random = Random.Range (0.0f, 1.0f);
				Debug.Log ("random: " + random + ", mcc: " + multipleConnectionChance);
				if (random < multipleConnectionChance)
					Debug.Log ("BINGO");
				if (random < multipleConnectionChance) {
					// connect regions anyways
					map.tiles[currentNode.x, currentNode.y].type = Tile.TileType.Connection;
					Region r1 = currentNode.r1;
					Region r2 = currentNode.r2;
					
					Corridor tempC = new Corridor();
					
					if (r1.GetType() == tempC.GetType()) {
						map.tiles[nodes[k].x, nodes[k].y].parentRegion = r1;
						r1.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
						r2.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
					}
					else if (r2.GetType() == tempC.GetType()) {
						map.tiles[nodes[k].x, nodes[k].y].parentRegion = r2;
						r1.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
						r2.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
					}
					else {
						tempC.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
						map.tiles[nodes[k].x, nodes[k].y].parentRegion = tempC;
						r1.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
						r2.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
					}
					
					r1.AddRegionToList(r2);
					r2.AddRegionToList(r1);

					List<ConnectionNode> tempNodes = new List<ConnectionNode>();

					for (int j = 0; j < nodes.Count; j++) {
						if (currentNode.EquivalentNodes(nodes[j])) {
							tempNodes.Add(nodes[j]);
						}
					}
					
					for (int j = 0; j < tempNodes.Count; j++) {
						connections.Remove (tempNodes[j]);
						nodes.Remove(tempNodes[j]);
					}
					nodes.Remove(currentNode);
				}
			}
			else if (!useExcessCorridors && otherRegion.GetType() == typeof (Corridor) && GetNumberOfUnconnectedNeighbours(region, otherRegion, GetRelatedConnections(otherRegion)) >= 1) {
				List<ConnectionNode> tempNodes = new List<ConnectionNode>();
				
				for (int j = 0; j < nodes.Count; j++) {
					if (currentNode.EquivalentNodes(nodes[j])) {
						tempNodes.Add(nodes[j]);
					}
				}
				
				for (int j = 0; j < tempNodes.Count; j++) {
					connections.Remove (tempNodes[j]);
					nodes.Remove(tempNodes[j]);
				}
				nodes.Remove(currentNode);
			}
			else {
				// connect regions
				map.tiles[currentNode.x, currentNode.y].type = Tile.TileType.Connection;
				Region r1 = currentNode.r1;
				Region r2 = currentNode.r2;
				
				Corridor tempC = new Corridor();
				
				if (r1.GetType() == tempC.GetType()) {
					map.tiles[nodes[k].x, nodes[k].y].parentRegion = r1;
					r1.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
					r2.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
				}
				else if (r2.GetType() == tempC.GetType()) {
					map.tiles[nodes[k].x, nodes[k].y].parentRegion = r2;
					r1.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
					r2.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
				}
				else {
					tempC.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
					map.tiles[nodes[k].x, nodes[k].y].parentRegion = tempC;
					r1.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
					r2.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
				}
				
				r1.AddRegionToList(r2);
				r2.AddRegionToList(r1);
				
				List<ConnectionNode> tempNodes = new List<ConnectionNode>();
				
				for (int j = 0; j < nodes.Count; j++) {
					if (currentNode.EquivalentNodes(nodes[j])) {
						tempNodes.Add(nodes[j]);
					}
				}
				
				for (int j = 0; j < tempNodes.Count; j++) {
					connections.Remove (tempNodes[j]);
					nodes.Remove(tempNodes[j]);
				}
				nodes.Remove(currentNode);
			}

			BuildConnections(otherRegion);

			
			if (nodes.Count <= 0)
				done = true;
		}

	}

	List<ConnectionNode> GetRelatedConnections(Region region) {
		List<ConnectionNode> nodes = new List<ConnectionNode> ();

		for (int j = 0; j < connections.Count; j++) {
			if (connections[j].r1 == region || connections[j].r2 == region) {
				nodes.Add (connections[j]);
			}
		}

		return nodes;
	}

	int GetNumberOfUnconnectedNeighbours(Region startRegion, Region currentRegion, List<ConnectionNode> nodes) {
		List<Region> neighbours = new List<Region> ();

		foreach (ConnectionNode n in nodes) {
			Region r = n.GetOtherRegion (currentRegion);
			if (!neighbours.Contains (r) && !r.isConnected && r != currentRegion) {
				neighbours.Add (r);
			}
		}

		return neighbours.Count;
	}*/

	void BuildConnections() {

		for (int i = 0; i < map.roomList.Count; i++) {
			List<ConnectionNode> nodes = new List<ConnectionNode>();
			for (int j = 0; j < connections.Count; j++) {
				if (connections[j].r1 == map.roomList[i] || connections[j].r2 == map.roomList[i]) {
					nodes.Add (connections[j]);
				}
			}
			bool done = false;
			while (!done) {
				
				int k = (int) Random.Range(0, nodes.Count);
				
				Region r1 = nodes[k].r1;
				Region r2 = nodes[k].r2;
				map.tiles[nodes[k].x, nodes[k].y].type = Tile.TileType.Connection;
				
				Corridor tempC = new Corridor();
				
				if (r1.GetType() == tempC.GetType()) {
					map.tiles[nodes[k].x, nodes[k].y].parentRegion = r1;
				}
				else if (r2.GetType() == tempC.GetType()) {
					map.tiles[nodes[k].x, nodes[k].y].parentRegion = r2;
				}
				else {
					Corridor c = new Corridor();
					c.AddTileToList(map.tiles[nodes[k].x, nodes[k].y]);
				}
				
				r1.AddRegionToList(r2);
				r2.AddRegionToList(r1);
				
				ConnectionNode currentNode = nodes[k];
				List<ConnectionNode> tempNodes = new List<ConnectionNode>();
				
				for (int j = 0; j < nodes.Count; j++) {
					if (currentNode.EquivalentNodes(nodes[j])) {
						tempNodes.Add(nodes[j]);
					}
				}
				
				for (int j = 0; j < tempNodes.Count; j++) {
					connections.Remove (tempNodes[j]);
					nodes.Remove(tempNodes[j]);
				}
				
				nodes.Remove(currentNode);
				
				if (nodes.Count <= 0) {
					done = true;
				}
			}
		}
	}
	
	void CleanUpCorridors() {
		int numCleanedThisIteration = 0;
		
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				if (map.tiles [x, y].type != Tile.TileType.Wall) {
					if (NumberOfCarvedNeighbours (x, y) < 2) {
						map.tiles [x, y].type = Tile.TileType.Wall;
						if (map.tiles[x,y].parentRegion != null) {
							map.tiles [x, y].parentRegion.RemoveTileFromList(map.tiles[x,y]);
							map.tiles [x, y].parentRegion = null;
						}
						numCleanedThisIteration++;
					}
				}
			}
		}
		
		if (numCleanedThisIteration > 0) {
			CleanUpCorridors ();
		}
	}
	
	// determines if tile at position x,y has non-wall neighbours in given direction(s)
	bool ValidToCarve(Tile tile, Direction dir) {
		switch (dir) {
		case Direction.All:
			if (tile.northAdj == null || tile.eastAdj == null || tile.southAdj == null || tile.westAdj == null) {
				return false;
			}
			else if (tile.northAdj.type != Tile.TileType.Wall || tile.eastAdj.type != Tile.TileType.Wall
			         || tile.southAdj.type != Tile.TileType.Wall || tile.westAdj.type != Tile.TileType.Wall){
				return false;
			}
			else {
				return true;
			}
			break;
		case Direction.Up:
			if (tile.northAdj == null || tile.northAdj.type != Tile.TileType.Wall) {
				return false;
			}
			else {
				return true;
			}
			break;
		case Direction.Right:
			if (tile.eastAdj == null || tile.eastAdj.type != Tile.TileType.Wall) {
				return false;
			}
			else {
				return true;
			}
			break;
		case Direction.Down:
			if (tile.southAdj == null || tile.southAdj.type != Tile.TileType.Wall) {
				return false;
			} 
			else {
				return true;
			}
			break;
		case Direction.Left:
			if (tile.westAdj == null || tile.westAdj.type != Tile.TileType.Wall) {
				return false;
			} 
			else {
				return true;
			}
			break;
		default:
			return true;
			break;
		}
	}
	
	void SetUnseenToVoid() {
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				if (map.tiles[x,y].type == Tile.TileType.Wall && NumberOfCarvedNeighbours(x,y) <= 0) {
					map.tiles[x,y].type = Tile.TileType.Void;
				}
			}
		}
	}
	
	
	void OnDrawGizmos() {
		if (preview) {
			if (map != null && map.tiles != null) {
				for (int x = 0; x < mapSizeX; x++) {
					for (int y = 0; y < mapSizeY; y++) {
						Gizmos.color = Color.black;
						if (map.tiles [x, y].type == Tile.TileType.Room) {
							Gizmos.color = Color.white;
							if (map.tiles[x,y].parentRegion.connectionsList.Count < 1) {
								Gizmos.color = Color.red;
							}
							Gizmos.DrawCube (new Vector3 (x, y, 0), Vector3.one);
						} else if (map.tiles [x, y].type == Tile.TileType.Corridor) {
							Gizmos.color = Color.blue;
							Gizmos.DrawCube (new Vector3 (x, y, 0), Vector3.one);
						} else if (map.tiles [x, y].type == Tile.TileType.Wall) {
							Gizmos.color = Color.grey;
							Gizmos.DrawCube (new Vector3 (x, y, 0), Vector3.one);
						}
						else if (map.tiles [x, y].type == Tile.TileType.Connection) {
							Gizmos.color = Color.green;
							Gizmos.DrawCube (new Vector3 (x, y, 0), Vector3.one);
						}
						else {
							Gizmos.DrawWireCube (new Vector3 (x, y, 0), Vector3.one);
						}
					}
				}
			}
		}
	}
}
