// Scripts/Input/DraggableBlock.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public BoardGenerator board;     // assign in Inspector
    public GameController game;      // assign in Inspector (for success check)

    RectTransform rt;
    Canvas canvas;
    Vector2 startPos;
    Slot snappedTo;

    void Awake() 
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData e) 
    {
        startPos = rt.anchoredPosition;
        if (snappedTo != null) { board.Vacate(snappedTo); snappedTo = null; }
        var cg = GetComponent<CanvasGroup>();
        if (cg) cg.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData e) 
    {
        rt.anchoredPosition += e.delta / (canvas ? canvas.scaleFactor : 1f);
    }

    public void OnEndDrag(PointerEventData e) 
    {
        var nearest = board.FindNearestFreeSlot(rt.position);
        if (nearest != null)
        {
            // Snap
            rt.position = nearest.GetComponent<RectTransform>().position;
            board.Occupy(nearest);
            snappedTo = nearest;
            game.CheckWin(snappedTo);
        }
        else
        {
            // Return
            rt.anchoredPosition = startPos;
        }
        var cg = GetComponent<CanvasGroup>();
        if (cg) cg.alpha = 1f;
    }
}
