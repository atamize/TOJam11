using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleAStarExample;
using DG.Tweening;

public enum UnitType
{
    Zookeeper, Poo, Lion, Elephant, Monkey, Goat, Visitor
}

public class Unit : MonoBehaviour
{
    public UnitType type;
    public float speed;
    public string startState;
    public string[] buttonStrings;
    public List<UnitType> blockedBy;

    public Tile Tile
    {
        get { return currentTile; }
        set
        {
            if (currentTile != null)
            {
                currentTile.Remove(this);
            }
            currentTile = value;
            currentTile.Occupy(this);
            transform.position = currentTile.worldPosition;
        }
    }

    protected Transform mTransform;
    Tile currentTile;
    Tile destinationTile;
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

    public virtual void Init(Main main) { }
    public virtual void Action1(Main main) { }
    public virtual void Action2(Main main) { }

    protected virtual void OnBlockedPath(Map map) 
    {
        MoveTo(map, destinationTile); 
    }

    protected virtual void OnFullyBlocked(Map map)
    {

    }

    public bool CanOccupyTile(Tile tile)
    {
        foreach (var unitType in blockedBy)
        {
            if (tile.OccupiedBy(unitType))
                return false;
        }
        return true;
    }

    public Coroutine MoveTo(Map map, Tile tile)
    {
        destinationTile = tile;
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
        return moveRoutine;
    }

    IEnumerator Move(Map map, List<System.Drawing.Point> path)
    {
        moving = true;
        foreach (var point in path)
        {
            var tile = map.GetTile(point.X, point.Y);
            if (CanOccupyTile(tile))
            {
                moveTween = mTransform.DOMove(tile.worldPosition, speed).SetSpeedBased(true).SetEase(Ease.Linear);
                yield return moveTween.WaitForCompletion();
                Tile = map.GetTile(point.X, point.Y);
            }
            else if (path.Count > 1)
            {
                OnBlockedPath(map);
            }
            else
            {
                OnFullyBlocked(map);
            }
        }
        moving = false;
    }

    public void RemoveFromTile()
    {
        Tile.Remove(this);
    }
}
