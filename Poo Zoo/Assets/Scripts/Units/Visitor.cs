using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

public class Visitor : Unit
{
    enum State { Moving, Waiting, Leaving }

    public float waitTime = 5f;

    List<Tile> stations;
    State state;

    public override void Init(Main main)
    {
        Point[] points = new Point[]
        {
            new Point(2, 0), new Point(10, 0), new Point(2, 6), new Point(10, 6)
        };

        stations = new List<Tile>();
        foreach (var p in points)
        {
            var tile = main.map.GetTile(p.X, p.Y);
            stations.Add(tile);
        }

        // shuffle
        for (int i = 0; i < stations.Count; ++i)
        {
            var temp = stations[i];
            int rnd = Random.Range(0, stations.Count);
            stations[i] = stations[rnd];
            stations[rnd] = temp;
        }
        
        state = State.Moving;
        StartCoroutine(Visit(main));
    }
	
    IEnumerator Visit(Main main)
    {
        foreach (var station in stations)
        {
            yield return MoveTo(main.map, station);

            state = State.Waiting;
            yield return new WaitForSeconds(waitTime);

            main.VisitSuccess(this, 10);
        }

        yield return MoveTo(main.map, main.map.GetTile(6, 0));
        main.RemoveUnit(this);
    }
}
