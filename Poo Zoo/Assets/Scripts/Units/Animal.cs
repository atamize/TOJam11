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

    void Start()
    {
        StartCoroutine(CheckDirection());
        startingCageIndex = cageTileIndex;
    }

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

        homeTile = main.map.GetTile(cageTiles[cageTileIndex].x, cageTiles[cageTileIndex].y + (cageTileY == 1 ? -1 : 1));

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        if (moveTween != null)
            moveTween.Kill();

        mTransform.position = cageTiles[cageTileIndex].worldPosition;
        moveRoutine = StartCoroutine(Pace(main.map));
    }

    IEnumerator CheckDirection()
    {
        SpriteRenderer spriteRenderer = mTransform.GetChild(0).GetComponent<SpriteRenderer>();
        Vector3 oldPos = mTransform.position;

        while (true)
        {
            if (mTransform.position.x > oldPos.x)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;

            oldPos = mTransform.position;
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator Pace(Map map)
    {
        mTransform = this.transform;
        List<int> adjacent = new List<int>();
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
            moveTween = MoveTweener(tile);
            yield return moveTween.WaitForCompletion();
        }
    }

    public virtual void Escape(Main main)
    {
        animalState = AnimalState.Escaped;
        StopCoroutine(moveRoutine);
        if (moveTween != null)
        {
            moveTween.Kill();
        }
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

        // We're free!
        yield return MoveTween(homeTile);

        animalState = AnimalState.Escaped;
        moveRoutine = StartCoroutine(Prowl(main));
    }

    protected abstract IEnumerator Prowl(Main main);

    protected IEnumerator GoHome(Main main)
    {
        animalState = AnimalState.Pacing;
        var gateTile = main.map.GetTile(HomeTile.x, HomeTile.y + (cageTileY == 1 ? 1 : -1));
        yield return MoveTween(gateTile);
        moveRoutine = StartCoroutine(Pace(main.map));
    }

}
