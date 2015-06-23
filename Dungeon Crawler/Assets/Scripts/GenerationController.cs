using UnityEngine;
using System.Collections;

// this class acts as a container/controller for the core
// map and scene generation classes
public class GenerationController : MonoBehaviour {

	public MapGenerator mapGenerator;
	public SceneBuilder sceneBuilder;
	
	// Use this for initialization
	void Start () {
		// initialize tile map, place rooms and build maze of corridors/connections
		mapGenerator.BuildMap ();
		// use map data to populate scene with prefabs
		sceneBuilder.BuildScene (mapGenerator.map);
		// place entities (player, enemies) within generated map
		sceneBuilder.PlaceEntities ();
	}
}
