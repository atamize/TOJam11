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
    protected override IEnumerator Prowl(Main main)
    {
        var fuck = new DontCare(main.map);
        SearchParameters search = new SearchParameters(new Point(0,0), new Point(0,0), fuck);
        PathFinder pf = new PathFinder(search);
        Dictionary<Unit, List<Point>> paths = new Dictionary<Unit, List<Point>>();

        while (main.PooUnits.Any())
        {
            Unit targetPoo = main.PooUnits.OrderBy(p =>
            {
                search.StartLocation = Tile.Location;
                search.EndLocation = p.Tile.Location;
                pf.SetSearchParameters(search);
                var path = pf.FindPath();
                paths.Add(p, path);
                return path.Count;
                
            }).First();
            
            yield return MoveToPath(main.map, paths[targetPoo]);
            main.RemovePoo(targetPoo);
        }
    }
}
