using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleAStarExample;
using System.Drawing;

public class DontCare : IBoolMap
{
    Map map;
    public DontCare(Map map)
    {
        this.map = map;
    }

    public int GetWidth()
    {
        return map.width;
    }

    public int GetHeight()
    {
        return map.height;
    }

    public bool Get(int x, int y)
    {
        var tile = map.GetTile(x, y);
        return tile.StartState != 'X';
    }
}

public class Monkey : Animal
{
    bool scared = false;

    protected override IEnumerator Prowl(Main main)
    {
        var fuck = new DontCare(main.map);
        SearchParameters search = new SearchParameters(new Point(0,0), new Point(0,0), fuck);
        PathFinder pf = new PathFinder(search);

        main.PlayAudio("MonkeyEscape");
        
        while (main.PooUnits.Any())
        {
            List<Point> leastPath = null;
            Unit targetPoo = null;
            int min = int.MaxValue;

            foreach (var poonit in main.PooUnits)
            {
                search.StartLocation = Tile.Location;
                search.EndLocation = poonit.Tile.Location;
                pf.SetSearchParameters(search);

                var path = pf.FindPath();
                if (path.Count < min)
                {
                    min = path.Count;
                    targetPoo = poonit;
                    leastPath = path;
                }
            }
            
            yield return MoveToPath(main.map, leastPath);
            main.RemovePoo(targetPoo);
        }

        // No more poo; go home
        yield return ReturnHome();
    }


    public void Scare()
    {
        if (!scared)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            StartCoroutine(ReturnHome());
            scared = true;
        }
    }

    IEnumerator ReturnHome()
    {
        yield return MoveTo(Main.Instance.map, HomeTile);
        yield return StartCoroutine(GoHome(Main.Instance));
        scared = false;
    }
}
