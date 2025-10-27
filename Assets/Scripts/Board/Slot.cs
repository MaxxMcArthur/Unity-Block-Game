// Scripts/Board/Slot.cs
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour 
{
    public Vector2Int gridPos;
    public bool occupied;
    public bool isTarget;

    public void SetAsTarget(bool on) 
    {
        var img = GetComponent<Image>();
        if (img) img.color = on ? new Color(0.8f, 1f, 0.8f) : new Color(0.9f,0.9f,0.9f);
    }

    public void SetOccupiedVisual(bool occ) {
        occupied = occ;
        var img = GetComponent<Image>();
        if (img != null) {
            if (occ) img.color = new Color(0.35f, 0.35f, 0.35f); // dark when occupied
            else img.color = isTarget ? new Color(0.8f,1f,0.8f) : new Color(0.9f,0.9f,0.9f);
        }
    }
}
