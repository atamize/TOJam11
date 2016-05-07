using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class Lion : Animal
{

    public override void Init(Main main)
    {
        Tile = main.map.GetTile(1, 6);
        StartCoroutine(Prowl(main));
    }

    IEnumerator Prowl(Main main)
    {
        Tile lastTile = null;

        while (true)
        {
            List<Tile> adjacent = main.map.GetAdjacentTiles(Tile);
            adjacent.Remove(lastTile);
            adjacent = adjacent.Where(t => CanOccupyTile(t)).ToList();

            Tile next = null;
            if (adjacent.Count > 0)
            {
                next = adjacent[Random.Range(0, adjacent.Count)];
            }
            else
            {
                next = lastTile;
            }

            lastTile = Tile;

            yield return mTransform.DOMove(next.worldPosition, speed).SetSpeedBased(true).SetEase(Ease.Linear).WaitForCompletion();
            Tile = next;

            var visitor = Tile.GetOccupantOfType(UnitType.Visitor);
            if (visitor)
            {
                main.KillVisitor(visitor);
            }
            else
            {
                //var monkey = Tile.GetOccupantOfType(UnitType.)
            }
           
        }
    }
}
