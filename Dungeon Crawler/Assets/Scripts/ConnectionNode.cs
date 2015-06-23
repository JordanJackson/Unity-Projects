using UnityEngine;
using System.Collections;

public class ConnectionNode {

	public Region r1;
	public Region r2;

	public int x;
	public int y;

	public ConnectionNode(Region region1, Region region2) {
		r1 = region1;
		r2 = region2;
	}

	public ConnectionNode(Region region1, Region region2, int xPos, int yPos) {
		r1 = region1;
		r2 = region2;
		x = xPos;
		y = yPos;
	}

	public Region GetOtherRegion(Region r) {
		if (r == r1)
			return r2;
		else
			return r1;
	}

	public bool EquivalentNodes(ConnectionNode otherNode) {

		if (this.r1 == otherNode.r1 && this.r2 == otherNode.r2)
			return true;

		if (this.r1 == otherNode.r2 && this.r2 == otherNode.r1)
			return true;

		return false;
	}
}
