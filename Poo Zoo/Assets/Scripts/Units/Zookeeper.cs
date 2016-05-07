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
}
