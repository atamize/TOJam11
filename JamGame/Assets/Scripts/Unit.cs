using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleAStarExample;
using DG.Tweening;

public enum UnitType
{
    Zookeeper, Poop, Lion, Elephant, Monkey, Goat, Visitor
}

public class Unit : MonoBehaviour {
    public UnitType type;
    public float speed;
    public string startState;
    public string[] buttonStrings;

    public Tile Tile
    {
        get { return currentTile; }
        set { currentTile = value; }
    }

    Transform mTransform;
    Tile currentTile;
    SearchParameters searchParams;
    PathFinder pathFinder;
    Tweener moveTween;
    Coroutine moveRoutine;
    bool moving;

    public char StartState
    {
        get { return startState[0]; }
    }

    void Start()
    {
        mTransform = this.transform;
    }

    public virtual void Action1() { }
    public virtual void Action2() { }

    public void MoveTo(Map map, Tile tile)
    {
        if (searchParams == null)
        {
            searchParams = new SearchParameters(currentTile.Location, tile.Location, map);
        }
        else
        {
            searchParams.StartLocation = currentTile.Location;
            searchParams.EndLocation = tile.Location;
        }

        if (pathFinder == null)
        {
            pathFinder = new PathFinder(searchParams);
        }
        else
        {
            pathFinder.SetSearchParameters(searchParams);
        }

        var path = pathFinder.FindPath();

        if (moving)
        {
            moveTween.Kill();
            StopCoroutine(moveRoutine);
        }
        moveRoutine = StartCoroutine(Move(map, path));
    }

    IEnumerator Move(Map map, List<System.Drawing.Point> path)
    {
        moving = true;
        foreach (var point in path)
        {
            Vector3 dest = map.GetTilePosition(point.X, point.Y);
            moveTween = mTransform.DOMove(dest, speed).SetSpeedBased(true).SetEase(Ease.Linear);
            yield return moveTween.WaitForCompletion();
            Tile = map.GetTile(point.X, point.Y);
        }
        moving = false;
    }
}
