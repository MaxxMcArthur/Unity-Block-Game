using UnityEngine;
using UnityEngine.UI;

public class BoardGenerator : MonoBehaviour 
{
    public GridLayoutGroup grid;        // assign the Grid object
    public GameObject slotPrefab;       // assign Slot prefab
    public int size = 8;                // 8x8 board

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
                s.SetOccupiedVisual(false); // start empty
                slots[x,y] = s;
            }
        }
    }

    // -------- Drag/drop helpers --------

    // For drag/drop: find nearest EMPTY slot to a screen-space position
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

    public void Occupy(Slot s) { if (s != null) s.SetOccupiedVisual(true); }
    public void Vacate(Slot s) { if (s != null) s.SetOccupiedVisual(false); }

    // -------- Save/Load helpers --------

    public int[] GetFlattenedBoard() {
        int[] arr = new int[size * size];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                arr[y * size + x] = slots[x,y].occupied ? 1 : 0;
            }
        }
        return arr;
    }

    public void LoadFromFlattened(int newSize, int[] flattened) {
        if (flattened == null) return;
        for (int i = 0; i < Mathf.Min(flattened.Length, size*size); i++) {
            int x = i % size;
            int y = i / size;
            bool occ = flattened[i] != 0;
            slots[x,y].SetOccupiedVisual(occ);
        }
    }

    public Slot GetSlotByIndex(int index) {
        if (index < 0 || index >= size * size) return null;
        int x = index % size;
        int y = index / size;
        return slots[x,y];
    }

    public int GetSlotIndex(Slot s) {
        if (s == null) return -1;
        return s.gridPos.y * size + s.gridPos.x;
    }

    // -------- Shape placement for multi-cell blocks --------

    // Checks if all cells of a shape can be placed at a base grid position
    public bool CanPlaceShapeAt(Vector2Int basePos, Vector2Int[] shapeCells) {
        if (shapeCells == null || shapeCells.Length == 0) return false;

        foreach (var off in shapeCells) {
            int x = basePos.x + off.x;
            int y = basePos.y + off.y;
            if (x < 0 || x >= size || y < 0 || y >= size)
                return false;
            if (slots[x,y].occupied)
                return false;
        }
        return true;
    }

    // Marks all cells of a shape as occupied
    public void PlaceShapeAt(Vector2Int basePos, Vector2Int[] shapeCells) {
        foreach (var off in shapeCells) {
            int x = basePos.x + off.x;
            int y = basePos.y + off.y;
            Occupy(slots[x,y]);
        }
    }

    // -------- Row & column clearing --------

    public void ClearCompletedLines() {
        ClearFullRows();
        ClearFullColumns();
    }

    void ClearFullRows() {
        for (int y = 0; y < size; y++) {
            bool full = true;
            for (int x = 0; x < size; x++) {
                if (!slots[x, y].occupied) {
                    full = false;
                    break;
                }
            }

            if (full) {
                for (int x = 0; x < size; x++) {
                    Vacate(slots[x, y]);
                }
            }
        }
    }

    void ClearFullColumns() {
        for (int x = 0; x < size; x++) {
            bool full = true;
            for (int y = 0; y < size; y++) {
                if (!slots[x, y].occupied) {
                    full = false;
                    break;
                }
            }

            if (full) {
                for (int y = 0; y < size; y++) {
                    Vacate(slots[x, y]);
                }
            }
        }
    }
}
