using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public abstract class Animal : Unit
{
    public int cageTileIndex;
    public int cageTileX;
    public int cageTileY;

    protected bool pacing;
    protected List<Tile> cageTiles;
    protected Coroutine paceRoutine;

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

        Tile = cageTiles[cageTileIndex];
        StartCoroutine(Pace(main.map));
    }

    IEnumerator Pace(Map map)
    {
        mTransform = this.transform;
        List<int> adjacent = new List<int>();

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

            yield return mTransform.DOMove(tile.worldPosition, speed).SetSpeedBased(true).SetEase(Ease.Linear).WaitForCompletion();
        }
    }
}
