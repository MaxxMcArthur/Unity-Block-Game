// Assets/Scripts/Board/BoardGenerator.cs
using UnityEngine;
using UnityEngine.UI;

public class BoardGenerator : MonoBehaviour
{
    public GridLayoutGroup grid;      // assign BoardGrid object
    public GameObject slotPrefab;     // assign Slot prefab
    [Range(8, 8)] public int size = 8;

    public Slot[,] slots;

    void Awake()
    {
        if (!grid)
            grid = GetComponent<GridLayoutGroup>();

        slots = new Slot[size, size];

        // Clear any existing children (editor convenience)
        for (int i = grid.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(grid.transform.GetChild(i).gameObject);
        }

        // Build grid of slots
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var go = Instantiate(slotPrefab, grid.transform);
                var s = go.GetComponent<Slot>();
                s.gridPos = new Vector2Int(x, y);
                s.SetOccupiedVisual(false);
                slots[x, y] = s;
            }
        }
    }

    // Used by DraggableBlock to pick a base slot near the cursor
    public Slot FindNearestFreeSlot(Vector3 screenPos)
    {
        if (slots == null) return null;

        Slot best = null;
        float bestDist = float.MaxValue;

        foreach (var s in slots)
        {
            if (s == null) continue;

            var rt = s.GetComponent<RectTransform>();
            float d = (rt.position - screenPos).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = s;
            }
        }
        return best;
    }

    // --------- Shape placement helpers (for multi-cell blocks) ---------

    public bool CanPlaceShapeAt(Vector2Int basePos, Vector2Int[] shapeCells)
    {
        if (slots == null) return false;

        foreach (var offset in shapeCells)
        {
            int x = basePos.x + offset.x;
            int y = basePos.y + offset.y;

            if (x < 0 || x >= size || y < 0 || y >= size)
                return false;

            if (slots[x, y].occupied)
                return false;
        }
        return true;
    }

    public void PlaceShapeAt(Vector2Int basePos, Vector2Int[] shapeCells)
    {
        foreach (var offset in shapeCells)
        {
            int x = basePos.x + offset.x;
            int y = basePos.y + offset.y;

            slots[x, y].SetOccupiedVisual(true);
        }
    }

    // --------- Reset / state helpers ---------

    public void ClearAllSlots()
    {
        if (slots == null) return;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                slots[x, y].ResetSlot();
            }
        }
    }

    public bool HasAnyOccupied()
    {
        if (slots == null) return false;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (slots[x, y].occupied)
                    return true;
            }
        }
        return false;
    }

    // --------- Line clear logic ---------

    public void ClearCompletedLines()
    {
        if (slots == null) return;

        bool anyCleared = false;

        if (ClearFullRows())     anyCleared = true;
        if (ClearFullColumns())  anyCleared = true;

        if (anyCleared && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLineClearSfx();
        }
    }

    bool ClearFullRows()
    {
        bool clearedAny = false;

        for (int y = 0; y < size; y++)
        {
            bool full = true;

            for (int x = 0; x < size; x++)
            {
                if (!slots[x, y].occupied)
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                clearedAny = true;
                for (int x = 0; x < size; x++)
                {
                    slots[x, y].PlayClearEffect();
                }
            }
        }

        return clearedAny;
    }

    bool ClearFullColumns()
    {
        bool clearedAny = false;

        for (int x = 0; x < size; x++)
        {
            bool full = true;

            for (int y = 0; y < size; y++)
            {
                if (!slots[x, y].occupied)
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                clearedAny = true;
                for (int y = 0; y < size; y++)
                {
                    slots[x, y].PlayClearEffect();
                }
            }
        }

        return clearedAny;
    }
}
