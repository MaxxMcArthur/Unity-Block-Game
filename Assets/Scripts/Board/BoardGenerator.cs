// Scripts/Board/BoardGenerator.cs
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BoardGenerator : MonoBehaviour 
{
    public GridLayoutGroup grid;        // assign the Grid object
    public GameObject slotPrefab;       // assign Slot prefab
    [Range(3,3)] public int size = 3;   // locked to 3 for this assignment
    public Vector2Int target = new(1,1); // Example_A uses (1,1), Example_B uses (0,2)

    public Slot[,] slots;

    void Awake() 
    {
        if (!grid) grid = GetComponent<GridLayoutGroup>();
        slots = new Slot[size,size];

        // Clear any existing children (dev convenience)
        for (int i = grid.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(grid.transform.GetChild(i).gameObject);

        for (int y = 0; y < size; y++) 
        {
            for (int x = 0; x < size; x++) 
            {
                var go = Instantiate(slotPrefab, grid.transform);
                var s = go.GetComponent<Slot>();
                s.gridPos = new Vector2Int(x,y);
                s.isTarget = (x == target.x && y == target.y);
                s.SetAsTarget(s.isTarget);
                slots[x,y] = s;
            }
        }
    }

    public Slot FindNearestFreeSlot(Vector3 screenPos) 
    {
        Slot best = null;
        float bestDist = float.MaxValue;

        foreach (var s in slots) 
        {
            if (s.occupied) continue;
            var rt = s.GetComponent<RectTransform>();
            var d = (rt.position - screenPos).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = s; }
        }
        return best;
    }

    public void Occupy(Slot s) { s.occupied = true; }
    public void Vacate(Slot s) { s.occupied = false; }
}

