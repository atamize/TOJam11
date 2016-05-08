using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class Lion : Animal
{
    Tile lastTile;

    protected override IEnumerator Prowl(Main main)
    {
        while (true)
        {
            List<Tile> adjacent = main.map.GetAdjacentTiles(Tile);
            adjacent.Remove(lastTile);

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

            moveTween = MoveTweener(next);
            yield return moveTween.WaitForCompletion();
            Tile = next;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (animalState == AnimalState.Escaped)
        {
            if (IsBlocked(other.tag))
            {
                lastTile = other.GetComponent<Unit>().Tile;
                MoveBack(() =>
                {
                    moveRoutine = StartCoroutine(Prowl(Main.Instance));
                });
            }
        }
    }
}
