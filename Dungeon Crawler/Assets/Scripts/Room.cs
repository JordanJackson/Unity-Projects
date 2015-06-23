using UnityEngine;
using System.Collections;

public class Room : Region {

	public int x, y, width, height;

	public Room (int x, int y, int width, int height) {
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public Room (float x, float y, float width, float height) {
		this.x = (int)x;
		this.y = (int)y;
		this.width = (int)width;
		this.height = (int)height;
	}

	public int xMax {
		get {
			return this.x + this.width-1;
		}
	}

	public int yMax {
		get {
			return this.y + this.height-1;
		}
	}

	public bool Overlaps(Room r) {
		if (x > r.xMax || xMax < r.x || y > r.yMax || yMax < r.y)
			return false;
		else
			return true;
	}
		    
    public bool Overlaps(Room r, int buffer) {
		if (x > r.xMax+buffer || xMax < r.x-buffer || y > r.yMax+buffer || yMax < r.y-buffer)
			return false;
		else
			return true;
	}
}
