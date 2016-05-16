using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class Lion : Animal
{
    bool wasBlocked;

    public override void Escape(Main main)
    {
        lastTile = null;
        wasBlocked = false;
        main.PlayAudio("LionEscape");
        base.Escape(main);
    }

    protected override IEnumerator Prowl(Main main)
    {
        GetComponent<Collider2D>().enabled = true;
        while (true)
        {
            var tile = Tile;
            List<Tile> adjacent = main.map.GetAdjacentTiles(tile);
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

            if (wasBlocked && next == main.map.GetTile(2, 6))
            {
                StartCoroutine(GoHome(main));
                wasBlocked = false;
                main.Dialogue("boss", "Great! Now clean up the poo! The monkey will help!");
                if (main.PooUnits.Any())
                {
                    main.monkeyHint.SetActive(true);
                }
                yield break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (animalState == AnimalState.Escaped)
        {
            if (IsBlocked(other.tag))
            {
                if (other.GetComponent<Unit>().Tile != HomeTile)
                {
                    wasBlocked = true;
                    GetComponent<Collider2D>().enabled = false;
                    float oldSpeed = speed;
                    speed *= 2f;
                    MoveBack(() =>
                    {
                        speed = oldSpeed;
                        moveRoutine = StartCoroutine(Prowl(Main.Instance));
                    });
                }
            }
            else if (other.CompareTag("Visitor"))
            {
                Main.Instance.KillVisitor(other.GetComponent<Unit>());
            }
            else if (other.CompareTag("Monkey"))
            {
                other.GetComponent<Monkey>().Scare();
            }
        }
    }
}
