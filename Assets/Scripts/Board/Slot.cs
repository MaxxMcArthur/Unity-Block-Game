using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour 
{
    public Vector2Int gridPos;
    public bool occupied;

    // Set the visual + flag when the slot is occupied / empty
    public void SetOccupiedVisual(bool occ) {
        occupied = occ;
        var img = GetComponent<Image>();
        if (img != null) {
            if (occ)
                img.color = new Color(0.35f, 0.35f, 0.35f);   // filled
            else
                img.color = new Color(0.9f, 0.9f, 0.9f);       // empty
        }
    }
}

