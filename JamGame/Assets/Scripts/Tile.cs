using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

public class Tile : MonoBehaviour {
    public int x;
    public int y;
    public Vector3 worldPosition;

    char startState;
    public char StartState
    {
        get { return startState; }
    }

    public Point Location
    {
        get {  return new Point(x, y); }
    }

    List<Unit> occupyingUnits;

    public Tile(int _x, int _y, char state)
    {
        x = _x;
        y = _y;
        startState = state;
    }

    public bool IsFree()
    {
        return startState == '-';
    }
}
