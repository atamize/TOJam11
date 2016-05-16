using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleAStarExample;

public enum ZookeeperState { Normal, Eating };

public class AvoidPoo : IBoolMap
{
    Map map;

    public AvoidPoo(Map _map)
    {
        map = _map;
    }

    public int GetWidth()
    {
        return map.width;
    }

    public int GetHeight()
    {
        return map.height;
    }

    public bool Get(int x, int y)
    {
        var tile = map.GetTile(x, y);
        if (!tile.IsFree() || tile.OccupiedBy("Poo"))
            return false;
        
        return true;
    }
}

public class Zookeeper : Unit
{
    public Text pooText;
    public Image snackFill;
    public int initialPoo = 2;
    public int eatingTime = 5;

    int pooCount;
    ZookeeperState state;

    public override void Init(Main main)
    {
        base.Init(main);
        pooCount = initialPoo;
        UpdatePoo();
        state = ZookeeperState.Normal;
        snackFill.transform.parent.gameObject.SetActive(false);
    }

    public override void Action1(Main main)
    {
        if (pooCount > 0)
        {
            var tile = Tile;
            if (!tile.OccupiedBy("Lion", "Poo"))
            {
                main.AddPoo(tile);
                --pooCount;
                UpdatePoo();
            }
        }
        else
        {
            main.Dialogue("emily", "I don't have enough poo! Time to go to the Snack Bar!");
            main.snackHint.SetActive(true);
        }
    }

    public override void Action2(Main main)
    {
        var tile = Tile;
        int x = tile.x;
        int y = tile.y;

        if (x == 10 && y == 0)
        {
            main.Release(UnitType.Monkey);
            main.monkeyHint.SetActive(false);
        }
        else if (tile.x == 6 && tile.y == 6)
        {
            state = ZookeeperState.Eating;
            main.HideButton(0);
            main.HideButton(1);
            StartCoroutine(Eating(main));
            main.PlayAudio("Eating");
        }
    }

    IEnumerator Eating(Main main)
    {
        main.snackHint.SetActive(false);
        snackFill.transform.parent.gameObject.SetActive(true);
        snackFill.fillAmount = 1f;

        main.Dialogue("emily", "Yumm! I love snacks...munch munch munch");

        for (float f = 0f; f < eatingTime; f += Time.deltaTime)
        {
            snackFill.fillAmount = 1f - (f / eatingTime);
            yield return null;
        }

        pooCount += 2;
        UpdatePoo();
        main.Dialogue("emily", "Yaayyyyy! Now I have poo!");
        state = ZookeeperState.Normal;
        snackFill.transform.parent.gameObject.SetActive(false);
        Arrived(main.map);
    }

    void UpdatePoo()
    {
        pooText.text = string.Format("x{0}", pooCount);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsBlocked(other.tag) && destinationTile != null)
        {
            MoveBack(() => MoveTo(Main.Instance.map, destinationTile));
        }
    }

    public override Coroutine MoveTo(Map map, Tile tile)
    {
        if (state == ZookeeperState.Normal)
        {
            Main.Instance.HideButton(0);
            Main.Instance.HideButton(1);

            destinationTile = tile;

            if (searchParams == null)
            {
                AvoidPoo avoidPoo = new AvoidPoo(Main.Instance.map);
                searchParams = new SearchParameters(Tile.Location, tile.Location, avoidPoo);
            }
            else
            {
                searchParams.StartLocation = Tile.Location;
                searchParams.EndLocation = tile.Location;
            }

            if (pathFinder == null)
            {
                pathFinder = new PathFinder(searchParams);
            }
            else
            {
                pathFinder.SetSearchParameters(searchParams);
            }

            var path = pathFinder.FindPath();
            return MoveToPath(map, path);
        }
        return null;
    }

    public override void Arrived(Map map)
    {
        Main.Instance.ShowButton(0, buttonStrings[0]);
        var tile = Tile;

        if (tile.x == 10 && tile.y == 0)
        {
            Main.Instance.ShowButton(1, "Free Monkey");
        }
        else if (tile.x == 6 && tile.y == 6)
        {
            Main.Instance.ShowButton(1, "Eat Snacks");
        }
        else
        {
            Main.Instance.HideButton(1);
        }
    }
}
