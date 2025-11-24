using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public BoardGenerator board;     // assign in Inspector
    public GameController game;      // assign in Inspector

    // shape offsets from top-left cell (pivot)
    public Vector2Int[] shapeCells = new Vector2Int[] { new Vector2Int(0, 0) };

    RectTransform rt;
    Canvas canvas;

    Vector2 startPos;                // for cancelling a drag
    Vector2 initialAnchoredPos;      // for resetting level

    bool isPlaced = false;
    public bool IsPlaced => isPlaced;

void Awake() 
{
    // If the block is active at scene load, initialize normally
    if (gameObject.activeInHierarchy)
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        initialAnchoredPos = rt.anchoredPosition;
    }
}


public void ResetBlock() 
{
    // Ensure initialization even when object was disabled at scene load
    if (rt == null)
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    isPlaced = false;

    // Restore block to its starting position
    gameObject.SetActive(true);

    // If initial position was never saved (Awake never ran), save it now
    if (initialAnchoredPos == Vector2.zero)
        initialAnchoredPos = rt.anchoredPosition;

    rt.anchoredPosition = initialAnchoredPos;
}


    public void OnBeginDrag(PointerEventData e) {
        if (isPlaced) return;

        startPos = rt.anchoredPosition;
        var cg = GetComponent<CanvasGroup>();
        if (cg) cg.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData e) {
        if (isPlaced) return;

        rt.anchoredPosition += e.delta / (canvas ? canvas.scaleFactor : 1f);
    }

    public void OnEndDrag(PointerEventData e) {
        if (isPlaced) return;

        var nearest = board.FindNearestFreeSlot(rt.position);
        if (nearest != null) {
            Vector2Int basePos = nearest.gridPos;

            if (board.CanPlaceShapeAt(basePos, shapeCells)) {
                // snap top-left of piece to slot
                rt.position = nearest.GetComponent<RectTransform>().position;

                // occupy all cells of this shape
                board.PlaceShapeAt(basePos, shapeCells);

                // consume piece
                isPlaced = true;
                gameObject.SetActive(false);

                // clear full lines
                board.ClearCompletedLines();

                // notify controller
                if (game != null) game.OnBlockPlaced();
            } else {
                // invalid placement
                rt.anchoredPosition = startPos;
            }
        } else {
            rt.anchoredPosition = startPos;
        }

        var cg = GetComponent<CanvasGroup>();
        if (cg) cg.alpha = 1f;
    }
}
