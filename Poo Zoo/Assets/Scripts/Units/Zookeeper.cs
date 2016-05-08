using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum ZookeeperState { Normal, Eating };

public class Zookeeper : Unit
{
    public Text pooText;
    public int eatingTime = 5;

    int pooCount = 2;
    ZookeeperState state;

    public override void Init(Main main)
    {
        base.Init(main);
        state = ZookeeperState.Normal;
    }

    public override void Action1(Main main)
    {
        if (pooCount > 0)
        {
            main.AddPoo(Tile);
            --pooCount;
            UpdatePoo();
        }
        else
        {
            main.Dialogue("emily", "I don't have enough poo! Time to go to the Snack Bar!");
        }
    }

    public override void Action2(Main main)
    {
        int x = Tile.x;
        int y = Tile.y;

        if (x == 10 && y == 0)
        {
            main.Release(UnitType.Monkey);
        }
        else if (Tile.x == 6 && Tile.y == 6)
        {
            state = ZookeeperState.Eating;
            main.HideButton(0);
            main.HideButton(1);
            StartCoroutine(Eating(main));
        }
    }

    IEnumerator Eating(Main main)
    {
        for (int i = 0; i < eatingTime; ++i)
        {
            main.Dialogue("emily", "Yumm! I love snacks...will be done in " + (eatingTime - i));
            yield return new WaitForSeconds(1);
        }

        pooCount++;
        UpdatePoo();
        main.Dialogue("emily", "Yaayyyyy! Now I have poo!");
        state = ZookeeperState.Normal;
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

            return base.MoveTo(map, tile);
        }
        return null;
    }

    public override void Arrived(Map map)
    {
        Main.Instance.ShowButton(0, buttonStrings[0]);
        
        if (Tile.x == 10 && Tile.y == 0)
        {
            Main.Instance.ShowButton(1, "Free Monkey");
        }
        else if (Tile.x == 6 && Tile.y == 6)
        {
            Main.Instance.ShowButton(1, "Eat Snacks");
        }
        else
        {
            Main.Instance.HideButton(1);
        }
    }
}
