// Scripts/Board/BoardGenerator.cs
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BoardGenerator : MonoBehaviour 
{
    public GridLayoutGroup grid;        // assign the Grid object
    public GameObject slotPrefab;       // assign Slot prefab
    [Range(3,3)] public int size = 3;   // locked to 3 for this assignment
    public Vector2Int target = new(1,1);

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
                s.SetOccupiedVisual(false); // ensure initial visual
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

    // updated to call visual setter
    public void Occupy(Slot s) { if (s != null) s.SetOccupiedVisual(true); }
    public void Vacate(Slot s) { if (s != null) s.SetOccupiedVisual(false); }

    // ---------------------
    // Small helper API used by SaveLoadManager
    // ---------------------

    // returns flattened int[] (0/1) row-major
    public int[] GetFlattenedBoard() {
        int[] arr = new int[size * size];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                arr[y * size + x] = slots[x,y].occupied ? 1 : 0;
            }
        }
        return arr;
    }

    // load flattened board and update visuals
    public void LoadFromFlattened(int newSize, int[] flattened) {
        if (flattened == null) return;
        // assume same size (3)
        for (int i = 0; i < Mathf.Min(flattened.Length, size*size); i++) {
            int x = i % size;
            int y = i / size;
            bool occ = flattened[i] != 0;
            slots[x,y].SetOccupiedVisual(occ);
        }
    }

    // Return slot by flattened index
    public Slot GetSlotByIndex(int index) {
        if (index < 0 || index >= size * size) return null;
        int x = index % size;
        int y = index / size;
        return slots[x,y];
    }

    // Return flattened index for a slot
    public int GetSlotIndex(Slot s) {
        if (s == null) return -1;
        return s.gridPos.y * size + s.gridPos.x;
    }
}
