using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonGrid : MonoBehaviour
{
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;

    [SerializeField] HexagonCell cellPrefab;

    Dictionary<Vector2, HexagonCell> cells = new Dictionary<Vector2, HexagonCell>();


    public void InitializeGrid() {

        for (int z = 1; z < height; z++) {
            int i = 0;
            for (int x = GetHexLow(z); x < GetHexHigh(z); x++) {
                CreateCell (z, x, i++);
            }
        }
    }

    int GetHexLow (int z) {
        switch (z) {
            case 0:
            return 3;
            case 1:
            return 2;
            case 2:
            return 2;
            case 3:
            return 1;
            case 4:
            return 1;
            case 5:
            return 0;
            case 6:
            return 1;
            case 7:
            return 1;
            case 8:
            return 2;
            case 9:
            return 2;
            case 10:
            return 3;
        }
        return 0;
    }

    int GetHexHigh (int z) {
        switch (z) {
            case 0:
            return 7;
            case 1:
            return 7;
            case 2:
            return 8;
            case 3:
            return 8;
            case 4:
            return 9;
            case 5:
            return 9;
            case 6:
            return 9;
            case 7:
            return 8;
            case 8:
            return 8;
            case 9:
            return 7;
            case 10:
            return 7;
        }
        return 0;
    }

    void CreateCell (int z, int x, int i) {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexagonMetrics.innerRadius * 2f) - width/2 + 1.2f;
        position.y = 0f;
        position.z = z * (HexagonMetrics.outerRadius * 1.5f) - height/3f;

        HexagonCell cell = Instantiate<HexagonCell> (cellPrefab);

        Vector2Int cellPos = new Vector2Int (z, (i + GetHexLow (z)) * 2 + ((z % 2 == 0)? 0 : 1));
        cells.Add(cellPos, cell);
        cell.transform.name = "Cell: " + cellPos.ToString();
        cell.SetCoordinates (cellPos);

        cell.transform.SetParent(transform, false);

        cell.transform.localPosition = position;
        cell.Position = cellPos;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dictionary<Vector2, HexagonCell> Cells {get {return cells;}}
}
