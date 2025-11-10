using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public BoardGenerator board;     // assign in Inspector
    public GameController game;      // assign in Inspector (optional)

    // Shape definition: cell offsets from the "pivot" cell (0,0)
    // Example 3x3: (0,0)..(2,2). Example 2x3: (0,0),(1,0)...(1,2)
    public Vector2Int[] shapeCells = new Vector2Int[] { new Vector2Int(0,0) };

    RectTransform rt;
    Canvas canvas;
    Vector2 startPos;
    bool isPlaced = false;     // once placed, we don't allow re-drag

    void Awake() 
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData e) 
    {
        if (isPlaced) return;  // don't allow dragging again once placed

        startPos = rt.anchoredPosition;
        var cg = GetComponent<CanvasGroup>();
        if (cg) cg.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData e) 
    {
        if (isPlaced) return;

        rt.anchoredPosition += e.delta / (canvas ? canvas.scaleFactor : 1f);
    }

    public void OnEndDrag(PointerEventData e) 
    {
        if (isPlaced) return;

        var nearest = board.FindNearestFreeSlot(rt.position);
        if (nearest != null)
        {
            Vector2Int basePos = nearest.gridPos;

            // Check if the entire shape fits and all cells are free
            if (board.CanPlaceShapeAt(basePos, shapeCells)) {
                // Snap the visual block so its pivot is aligned with the base slot
                rt.position = nearest.GetComponent<RectTransform>().position;

                // Mark all shape cells as occupied
                board.PlaceShapeAt(basePos, shapeCells);

                // Piece is now used up (Block Blast style)
                isPlaced = true;
                gameObject.SetActive(false);

                // Clear any full rows/columns after placing
                board.ClearCompletedLines();
            } else {
                // Invalid placement for this shape; return to start
                rt.anchoredPosition = startPos;
            }
        }
        else
        {
            // No nearby slot; return to start
            rt.anchoredPosition = startPos;
        }

        var cg = GetComponent<CanvasGroup>();
        if (cg) cg.alpha = 1f;
    }
}
