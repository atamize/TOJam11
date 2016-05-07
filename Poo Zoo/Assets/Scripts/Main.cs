using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleAStarExample;
using UnityEngine.UI;

public class Main : MonoBehaviour {
    public Map map;
    public List<Unit> units;
    public Text[] actionTexts;
    public Text dialogueText;

    SearchParameters searchParams;
    LinkedList<Unit> poos;
    Unit selectedUnit;

	void Start()
    {
        map.InitTiles();
        foreach (Unit unit in units)
        {
            var tile = map.GetTileWithState(unit.StartState);
            unit.Tile = tile;
            unit.transform.position = tile.worldPosition;
        }

        poos = new LinkedList<Unit>();

        SelectUnit(units[0]);
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

                selectedUnit.MoveTo(map, map.GetTile(to.x, to.y));
            }
        }
	}

    void SelectUnit(Unit unit)
    {
        selectedUnit = unit;

        // Update UI buttons
        int length = selectedUnit.buttonStrings.Length;
        for (int i = 0; i < 2; ++i)
        {
            if (i < length)
            {
                actionTexts[i].text = selectedUnit.buttonStrings[i];
                actionTexts[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                actionTexts[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void Button1()
    {
        selectedUnit.Action1();
    }

    public void Button2()
    {
        selectedUnit.Action2();
    }
}
