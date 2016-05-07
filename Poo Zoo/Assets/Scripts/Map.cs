using UnityEngine;
using System.Collections;
using SimpleAStarExample;

[ExecuteInEditMode]
public class Map : MonoBehaviour, IBoolMap {
    public float tileSize;
    public int width = 13;
    public int height = 7;
    public Transform gridParent;

    string[] gridData = new string[] {
        "-------------",
        "-XXX-XXX-XXX-",
        "-XXX-XXX-XXX-",
        "------Z------",
        "-XXX-XXX-XXX-",
        "-XXX-XXX-XXX-",
        "-------------"
    };

    Tile[,] tiles;

	void Start()
    {
	
	}

    public void InitTiles()
    {
        tiles = new Tile[width, height];
        IEnumerator ie = gridParent.GetEnumerator();

        float x = transform.position.x - (width / 2f) * tileSize + (tileSize / 2f);
        float y = transform.position.y + (height / 2f) * tileSize + (tileSize / 2f);

        for (int i = 0; i < gridData.Length; ++i)
        {
            for (int j = 0; j < gridData[i].Length; ++j)
            {
                tiles[j, i] = new Tile(j, i, gridData[i][j]);
                tiles[j, i].worldPosition = new Vector3(x + tileSize * j, y - tileSize * i, 0);

                ie.MoveNext();
                Transform t = (Transform)ie.Current;
                TileObject to = t.GetComponent<TileObject>();
                to.x = j;
                to.y = i;
            }
        }
    }

    public Tile GetTileWithState(char state)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.StartState == state)
                return tile;
        }
        return null;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public bool Get(int x, int y)
    {
        return tiles[x, y].IsFree();
    }

    public Tile GetTile(int x, int y)
    {
        return tiles[x, y];
    }

    public Vector3 GetTilePosition(int x, int y)
    {
        return GetTile(x, y).worldPosition;
    }

    [ContextMenu("Make Grid")]
    public void MakeGrid()
    {
        IEnumerator ie = gridParent.GetEnumerator();
        float x = gridParent.position.x - (width / 2f) * tileSize + (tileSize / 2f);
        float y = gridParent.position.y + (height / 2f) * tileSize + (tileSize / 2f);

        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                ie.MoveNext();
                Transform t = (Transform)ie.Current;
                t.position = new Vector3(x + tileSize * j, y - tileSize * i, 0);
            }
        }
    }
}
