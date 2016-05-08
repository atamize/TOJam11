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

[System.Serializable]
public class AudioData
{
    public string id;
    public List<AudioClip> clips;
}

public enum GameState { Title, Playing, Paused, GameOver }

public class Main : MonoBehaviour {
    public Map map;
    public List<Unit> units;
    public List<Animal> animals;
    public Text[] actionTexts;
    public GameObject pauseBox;
    public GameObject dialogueBox;
    public Text pauseText;
    public Text dialogueName;
    public Text dialogueText;
    public Image dialogueImage;
    public Text moneyText;
    public Unit pooPrefab;
    public Unit visitorPrefab;
    public Animal lion;
    public Animal monkey;
    public Image clockImage;
    public float gameTime = 120f;
    public float initialLionTime = 10;
    public float minLionTime;
    public float maxLionTime;
    public float minVisitorTime;
    public float maxVisitorTime;
    public int lawsuitCost = 100;
    public DialogueData[] dialogueData;
    public AudioData[] audioData;
    public Sprite[] visitorSprites;
    public string[] successStrings;
    public GameObject title;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    SearchParameters searchParams;
    LinkedList<Unit> poos;
    LinkedList<Unit> visitors;
    Unit selectedUnit;
    int money;
    int pooCount;
    GameState gameState;

    static Main instance;
    public static Main Instance { get { return instance; } }

    public LinkedList<Unit> PooUnits { get { return poos; } }

    void Awake()
    {
        instance = this;
        gameState = GameState.Title;
    }

	void Start()
    {
        map.InitTiles();
        visitors = new LinkedList<Unit>();
        poos = new LinkedList<Unit>();
	}

    public void Restart()
    {
        Reset();
    }

    public void Reset()
    {
        gameState = GameState.Playing;
        Time.timeScale = 1f;
        pauseBox.SetActive(false);
        money = 0;
        pauseText.text = "PAUSED";
        Dialogue("boss", "Watch out for lions, and try not to stink up the place!");

        foreach (var v in visitors)
        {
            v.RemoveFromTile();
            Destroy(v.gameObject);
        }
        visitors.Clear();

        foreach (var p in poos)
        {
            p.RemoveFromTile();
            Destroy(p.gameObject);
        }
        poos.Clear();
        
        UpdateMoney();
        CancelInvoke();
        StopAllCoroutines();

        foreach (Unit unit in units)
        {
            if (unit.type == UnitType.Zookeeper)
            {
                var tile = map.GetTileWithState(unit.StartState);
                unit.Tile = tile;
            }
            unit.Init(this);
        }

        clockImage.fillAmount = 1f;
        SelectUnit(units[0]);

        Invoke("CheckLion", initialLionTime);
        Invoke("CheckVisitors", 2f);
        gameState = GameState.Playing;
        title.SetActive(false);

        StartCoroutine(RunClock());
    }

    IEnumerator RunClock()
    {
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < gameTime)
        {
            clockImage.fillAmount = (Time.realtimeSinceStartup - startTime) / gameTime;
            yield return null;
        }

        pauseText.text = "GAME OVER";
        Time.timeScale = 0;
        gameState = GameState.Paused;
        pauseBox.SetActive(true);
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
        if (visitors.Count(u => u.type == UnitType.Visitor) < 2)
        {
            var visitor = GameObject.Instantiate<Unit>(visitorPrefab);
            visitor.Tile = map.GetTile(6, 0);
            visitor.spriteRenderer.sprite = visitorSprites[Random.Range(0, visitorSprites.Length)];
            visitors.AddLast(visitor);
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

    public void Quit()
    {
        Time.timeScale = 1f;
        title.SetActive(true);
    }
	
	void Update()
    {
        if (gameState == GameState.Playing)
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (gameState)
            {
                case GameState.Playing:
                    Time.timeScale = 0;
                    gameState = GameState.Paused;
                    pauseBox.SetActive(true);
                    break;

                case GameState.Paused:
                    Time.timeScale = 1f;
                    gameState = GameState.Playing;
                    pauseBox.SetActive(false);
                    break;

                case GameState.Title:
                    Application.Quit();
                    break;
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
        PlayAudio("DropPoo");
    }

    public void RemovePoo(Unit poo)
    {
        poo.RemoveFromTile();
        poos.Remove(poo);
        Destroy(poo.gameObject);
        PlayAudio("EatPoo");
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
        PlayAudio("Satisfy");
    }

    public void KillVisitor(Unit unit)
    {
        money -= lawsuitCost;
        unit.RemoveFromTile();
        visitors.Remove(unit);
        Destroy(unit.gameObject);
        UpdateMoney();
        Dialogue("boss", "Oh no! That's another lawsuit! Oink!");
        PlayAudio("Scream");
    }

    public void RemoveUnit(Unit unit)
    {
        visitors.Remove(unit);
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

    public void PlayAudio(string id)
    {
        foreach (var data in audioData)
        {
            if (data.id == id)
            {
                sfxSource.PlayOneShot(data.clips[Random.Range(0, data.clips.Count)]);
            }
        }
    }
}
