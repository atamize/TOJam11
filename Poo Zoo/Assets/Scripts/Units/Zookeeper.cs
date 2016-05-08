using UnityEngine;
using System.Collections;

public class Zookeeper : Unit {
    
    public override void Action1(Main main)
    {
        main.AddPoo(Tile);
    }

    public override void Action2(Main main)
    {
        print("Action 2");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsBlocked(other.tag))
        {
            MoveBack(() => MoveTo(Main.Instance.map, destinationTile));
        }
    }
}
