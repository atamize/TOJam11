using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public enum AnimalState { Pacing, Escaped }

public abstract class Animal : Unit
{
    public AnimalState animalState;
    public int cageTileIndex;
    public int cageTileX;
    public int cageTileY;

    protected bool pacing;
    protected List<Tile> cageTiles;
    protected Coroutine paceRoutine;
    int startingCageIndex;
    Tile homeTile;

    public Tile HomeTile { get { return homeTile; } }

    public override void Init(Main main)
    {
        pacing = true;
        cageTiles = new List<Tile>();

        for (int i = 0; i < 2; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                var tile = main.map.GetTile(cageTileX + j, cageTileY + i);
                cageTiles.Add(tile);
            }
        }

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        if (moveTween != null)
            moveTween.Kill();

        Tile = cageTiles[cageTileIndex];

        moveRoutine = StartCoroutine(Pace(main.map));
    }

    IEnumerator Pace(Map map)
    {
        mTransform = this.transform;
        List<int> adjacent = new List<int>();
        startingCageIndex = cageTileIndex;
        animalState = AnimalState.Pacing;

        while (true)
        {
            adjacent.Clear();
            switch (cageTileIndex)
            {
                case 0:
                    adjacent.Add(1);
                    adjacent.Add(3);
                    break;
                case 1:
                    adjacent.Add(0);
                    adjacent.Add(2);
                    adjacent.Add(4);
                    break;
                case 2:
                    adjacent.Add(1);
                    adjacent.Add(5);
                    break;
                case 3:
                    adjacent.Add(0);
                    adjacent.Add(4);
                    break;
                case 4:
                    adjacent.Add(3);
                    adjacent.Add(1);
                    adjacent.Add(5);
                    break;
                case 5:
                    adjacent.Add(2);
                    adjacent.Add(4);
                    break;
            }

            cageTileIndex = adjacent[Random.Range(0, adjacent.Count)];
            var tile = cageTiles[cageTileIndex];
            yield return MoveTween(tile);
        }
    }

    public virtual void Escape(Main main)
    {
        animalState = AnimalState.Escaped;
        StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToGate(main));
    }

    IEnumerator MoveToGate(Main main)
    {
        var tile = cageTiles[cageTileIndex];
        if (cageTileIndex != startingCageIndex)
        {
            tile = cageTiles[startingCageIndex];
            yield return MoveTween(tile);
        }

        var gateTile = main.map.GetTile(tile.x, tile.y + (cageTileY == 1 ? -1 : 1));
        
        while (!CanOccupyTile(gateTile))
        {
            yield return new WaitForSeconds(0.5f);
        }

        // We're free!
        yield return MoveTween(gateTile);

        animalState = AnimalState.Escaped;
        Tile = gateTile;
        homeTile = gateTile;
        moveRoutine = StartCoroutine(Prowl(main));
    }

    protected abstract IEnumerator Prowl(Main main);

    protected IEnumerator GoHome(Main main)
    {
        var gateTile = main.map.GetTile(HomeTile.x, HomeTile.y + (cageTileY == 1 ? 1 : -1));
        yield return MoveTween(gateTile);
        animalState = AnimalState.Pacing;
        Tile = gateTile;
        moveRoutine = StartCoroutine(Pace(main.map));
    }

}
