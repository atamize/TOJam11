using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleAStarExample;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class DialogueData
{
    public string id;
    public string name;
    public string title;
    public Sprite sprite;
}

public class Main : MonoBehaviour {
    public Map map;
    public List<Unit> units;
    public List<Animal> animals;
    public Text[] actionTexts;
    public GameObject dialogueBox;
    public Text dialogueName;
    public Text dialogueText;
    public Image dialogueImage;
    public Text moneyText;
    public Unit pooPrefab;
    public Unit visitorPrefab;
    public Animal lion;
    public Animal monkey;
    public float initialLionTime = 10;
    public float minLionTime;
    public float maxLionTime;
    public float minVisitorTime;
    public float maxVisitorTime;
    public int lawsuitCost = 100;
    public DialogueData[] dialogueData;
    public Sprite[] visitorSprites;
    public string[] successStrings;

    SearchParameters searchParams;
    LinkedList<Unit> poos;
    Unit selectedUnit;
    int money;
    int pooCount;

    static Main instance;
    public static Main Instance { get { return instance; } }

    public LinkedList<Unit> PooUnits { get { return poos; } }

    void Awake()
    {
        instance = this;
    }

	void Start()
    {
        map.InitTiles();

        foreach (Unit unit in units)
        {
            if (unit.type == UnitType.Zookeeper)
            {
                var tile = map.GetTileWithState(unit.StartState);
                unit.Tile = tile;
            }
            unit.Init(this);
        }

        poos = new LinkedList<Unit>();

        SelectUnit(units[0]);

        Invoke("CheckLion", initialLionTime);
        Invoke("CheckVisitors", 2f);
	}

    void Reset()
    {
        money = 0;
        UpdateMoney();
    }

    void CheckLion()
    {
        if (lion.animalState == AnimalState.Pacing)
        {
            lion.Escape(this);
            Dialogue("boss", "The lion has escaped! Use your poo to block his path!");
        }

        Invoke("CheckLion", Random.Range(minLionTime, maxLionTime));
    }

    void CheckVisitors()
    {
        if (units.Count(u => u.type == UnitType.Visitor) < 2)
        {
            var visitor = GameObject.Instantiate<Unit>(visitorPrefab);
            visitor.Tile = map.GetTile(6, 0);
            visitor.spriteRenderer.sprite = visitorSprites[Random.Range(0, visitorSprites.Length)];
            units.Add(visitor);
            visitor.Init(this);
        }

        Invoke("CheckVisitors", Random.Range(minVisitorTime, maxVisitorTime));
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var animal in animals)
            {
                if (animal.animalState == AnimalState.Pacing)
                {
                    animal.Escape(this);
                    break;
                }
            }
        }
	}

    void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
        selectedUnit.Arrived(map);
        // Update UI buttons
        //int length = selectedUnit.buttonStrings.Length;
        //for (int i = 0; i < 2; ++i)
        //{
        //    if (i < length)
        //    {
        //        actionTexts[i].text = selectedUnit.buttonStrings[i];
        //        actionTexts[i].transform.parent.gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        actionTexts[i].transform.parent.gameObject.SetActive(false);
        //    }
        //}
    }

    public void ShowButton(int index, string label)
    {
        actionTexts[index].text = label;
        actionTexts[index].transform.parent.gameObject.SetActive(true);
    }

    public void HideButton(int index)
    {
        actionTexts[index].transform.parent.gameObject.SetActive(false);
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
        tile.Occupy(poo);
        poos.AddLast(poo);
    }

    public void RemovePoo(Unit poo)
    {
        poo.RemoveFromTile();
        poos.Remove(poo);
        Destroy(poo.gameObject);
    }

    public void Dialogue(string id, string message)
    {
        for (int i = 0; i < dialogueData.Length; ++i)
        {
            if (dialogueData[i].id == id)
            {
                dialogueName.text = dialogueData[i].name;
                dialogueImage.sprite = dialogueData[i].sprite;
                dialogueText.text = message;
                dialogueBox.SetActive(true);
                break;
            }
        }
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
    }

    public void VisitSuccess(Unit unit, int amount)
    {
        money += amount;
        UpdateMoney();
    }

    public void KillVisitor(Unit unit)
    {
        money -= lawsuitCost;
        unit.RemoveFromTile();
        units.Remove(unit);
        Destroy(unit.gameObject);
        UpdateMoney();
        Dialogue("boss", "Oh no! That's another lawsuit! Oink!");
    }

    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        Destroy(unit.gameObject);
    }

    public void Release(UnitType type)
    {
        foreach (var animal in animals)
        {
            if (animal.animalState == AnimalState.Pacing && animal.type == type)
            {
                animal.Escape(this);
                break;
            }
        }
    }
}
