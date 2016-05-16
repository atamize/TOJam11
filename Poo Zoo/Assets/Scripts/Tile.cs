using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

public class Tile {
    public int x;
    public int y;
    public Vector3 worldPosition;
    RaycastHit2D[] hits = new RaycastHit2D[8];

    char startState;
    public char StartState
    {
        get { return startState; }
    }

    public Point Location
    {
        get {  return new Point(x, y); }
    }

    //List<Unit> occupyingUnits;

    public Tile(int _x, int _y, char state)
    {
        x = _x;
        y = _y;
        startState = state;
        //occupyingUnits = new List<Unit>();
    }

    public void Occupy(Unit unit)
    {
        //occupyingUnits.Add(unit);
    }

    public void Remove(Unit unit)
    {
        //occupyingUnits.Remove(unit);
    }

    public bool OccupiedBy(params string[] tags)
    {
        int count = Physics2D.RaycastNonAlloc(worldPosition, Vector2.zero, hits);
        for (int i = 0; i < count; ++i)
        {
            for (int j = 0; j < tags.Length; ++j)
            {
                if (hits[i].collider.CompareTag(tags[j]))
                    return true;
            }
        }
    
        //foreach (var unit in occupyingUnits)
        //{
        //    if (unit.type == type)
        //        return true;
        //}
        return false;
    }

    public Unit GetOccupantOfType(UnitType type)
    {
        //foreach (var unit in occupyingUnits)
        //{
        //    if (unit.type == type)
        //        return unit;
        //}
        return null;
    }

    public bool IsFree()
    {
        return startState != 'X';
    }
}
