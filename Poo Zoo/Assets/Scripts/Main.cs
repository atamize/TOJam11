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
    public Text moneyText;
    public Text pooText;
    public Unit pooPrefab;
    public Unit visitorPrefab;

    SearchParameters searchParams;
    LinkedList<Unit> poos;
    Unit selectedUnit;
    int money;
    int pooCount;

	void Start()
    {
        map.InitTiles();
        foreach (Unit unit in units)
        {
            var tile = map.GetTileWithState(unit.StartState);
            unit.Tile = tile;
        }

        poos = new LinkedList<Unit>();

        SelectUnit(units[0]);

        var visitor = GameObject.Instantiate<Unit>(visitorPrefab);
        visitor.Tile = map.GetTile(6, 0);
        units.Add(visitor);
        visitor.Init(this);
	}

    void Reset()
    {
        money = 0;
        UpdateMoney();
    }

    void UpdateMoney()
    {
        moneyText.text = string.Format("${0}", money.ToString());
        if (money > 0)
            moneyText.color = Color.green;
        else
            moneyText.color = Color.red;
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
        selectedUnit.Action1(this);
    }

    public void Button2()
    {
        selectedUnit.Action2(this);
    }

    public void AddPoo(Tile tile)
    {
        var poo = GameObject.Instantiate<Unit>(pooPrefab);
        poo.Tile = tile;
        poos.AddLast(poo);
    }

    public void VisitSuccess(Unit unit, int amount)
    {
        money += amount;
        UpdateMoney();
    }

    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        Destroy(unit.gameObject);
    }
}
