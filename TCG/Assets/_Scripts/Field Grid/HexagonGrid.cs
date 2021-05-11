using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonGrid : MonoBehaviour
{
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;

    [SerializeField] HexagonCell cellPrefab;

    List<HexagonCell> cells = new List<HexagonCell>();


    public void InitializeGrid() {

        for (int z = 0; z < height; z++) {
            for (int x = GetHexLow(z); x < GetHexHigh(z); x++) {
                CreateCell (z, x);
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

    void CreateCell (int z, int x) {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexagonMetrics.innerRadius * 2f) - width/2;
        position.y = 0f;
        position.z = z * (HexagonMetrics.outerRadius * 1.5f) - height/3f;

        HexagonCell cell = Instantiate<HexagonCell> (cellPrefab);
        cells.Add(cell);
        cell.transform.name = "Cell: " + z + ", " + x;
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
