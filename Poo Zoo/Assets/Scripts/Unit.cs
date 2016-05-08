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
            transform.position = currentTile.worldPosition;
        }
    }

    protected Transform mTransform;
    protected Tile lastTile;
    Tile currentTile;
    protected Tile destinationTile;
    SearchParameters searchParams;
    PathFinder pathFinder;
    protected Tweener moveTween;
    protected Coroutine moveRoutine;
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
        return MoveToPath(map, path);
    }

    public Coroutine MoveToPath(Map map, List<System.Drawing.Point> path)
    {
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
        RemoveFromTile();
        foreach (var point in path)
        {
            var tile = map.GetTile(point.X, point.Y);
            
            moveTween = mTransform.DOMove(tile.worldPosition, speed).SetSpeedBased(true).SetEase(Ease.Linear);
            yield return moveTween.WaitForCompletion();
            Tile = map.GetTile(point.X, point.Y);
        }

        Tile.Occupy(this);
        moving = false;
    }

    public void RemoveFromTile()
    {
        Tile.Remove(this);
    }

    protected Tweener MoveTweener(Tile tile)
    {
        return mTransform.DOMove(tile.worldPosition, speed).SetSpeedBased(true).SetEase(Ease.Linear);
    }

    protected YieldInstruction MoveTween(Tile tile)
    {
        return MoveTweener(tile).WaitForCompletion();
    }

    public bool IsBlocked(string collidingTag)
    {
        foreach (var e in blockedBy)
        {
            if (e.ToString() == collidingTag)
            {
                return true;
            }
        }
        return false;
    }

    protected void MoveBack(System.Action callback)
    {
        if (moveTween != null)
        {
            moveTween.Kill();
            moveTween = null;
        }

        StopCoroutine(moveRoutine);

        var tweener = MoveTweener(Tile);
        tweener.OnComplete(() =>
        {
            callback();
        });
    }
}
