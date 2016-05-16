﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using SimpleAStarExample;

public class Visitor : Unit
{
    enum State { Moving, Waiting, Leaving }

    public float waitTime = 5f;
    public int baseMoney = 10;

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
        for (int i = 0; i < stations.Count; ++i)
        {
            yield return MoveTo(main.map, stations[i]);

            state = State.Waiting;
            yield return new WaitForSeconds(waitTime);

            main.VisitSuccess(this, baseMoney * (i + 1));
        }

        yield return MoveTo(main.map, main.map.GetTile(6, 0));
        RemoveFromTile();
        main.RemoveUnit(this);

        main.Dialogue("boss", main.successStrings[Random.Range(0, main.successStrings.Length)]);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsBlocked(other.tag))
        {
            Main.Instance.PlayAudio("Scream");
            StartCoroutine(Disgust());
            //MoveBack(() => MoveTo(Main.Instance.map, destinationTile));
        }
    }

    IEnumerator Disgust()
    {
        yield return MoveTo(Main.Instance.map, Main.Instance.map.GetTile(6, 0));
        Main.Instance.VisitorDisgusted(this);
    }
}
