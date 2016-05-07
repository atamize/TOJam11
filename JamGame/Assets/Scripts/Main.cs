using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleAStarExample;

public class Main : MonoBehaviour {
    public Map map;
    public List<Unit> units;

    SearchParameters searchParams;

	void Start()
    {
        map.InitTiles();
        foreach (Unit unit in units)
        {
            var tile = map.GetTileWithState(unit.StartState);
            unit.Tile = tile;
            unit.transform.position = tile.worldPosition;
        }
	}
	
	void Update()
    {
	    if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.forward, out hit))
            {
                var to = hit.collider.GetComponent<TileObject>();
                print("Hit " + to.x + ", " + to.y);

                units[0].MoveTo(map, map.GetTile(to.x, to.y));
            }
        }
	}

    public void Button1()
    {
        print("Button1");
    }

    public void Button2()
    {
        print("Button2");
    }
}
