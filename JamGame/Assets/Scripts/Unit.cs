using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleAStarExample;
using DG.Tweening;

public class Unit : MonoBehaviour {

    public float speed;
    public string startState;

    public Tile Tile
    {
        get { return currentTile; }
        set { currentTile = value; }
    }

    Transform mTransform;
    Tile currentTile;
    SearchParameters searchParams;

    public char StartState
    {
        get { return startState[0]; }
    }

    void Start()
    {
        mTransform = this.transform;
    }

    public void MoveTo(Map map, Tile tile)
    {
        searchParams = new SearchParameters(currentTile.Location, tile.Location, map);
        PathFinder pathFinder = new PathFinder(searchParams);
        var path = pathFinder.FindPath();

        StartCoroutine(Move(map, path));
    }

    IEnumerator Move(Map map, List<System.Drawing.Point> path)
    {
        foreach (var point in path)
        {
            Vector3 dest = map.GetTilePosition(point.X, point.Y);
            yield return mTransform.DOMove(dest, speed).SetSpeedBased(true).SetEase(Ease.Linear).WaitForCompletion();
        }
    }
}
