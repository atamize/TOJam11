using UnityEngine;
using System.Collections;

public class Zookeeper : Unit {
    
    public override void Action1(Main main)
    {
        main.AddPoo(Tile);
    }

    public override void Action2(Main main)
    {
        int x = Tile.x;
        int y = Tile.y;

        if (x == 10 && y == 0)
        {
            main.Release(UnitType.Monkey);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsBlocked(other.tag) && destinationTile != null)
        {
            MoveBack(() => MoveTo(Main.Instance.map, destinationTile));
        }
    }
}
