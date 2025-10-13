// Scripts/Board/Slot.cs
using UnityEngine;

public class Slot : MonoBehaviour 
{
    public Vector2Int gridPos;
    public bool occupied;
    public bool isTarget;

    public void SetAsTarget(bool on) 
    {
        var img = GetComponent<UnityEngine.UI.Image>();
        if (img) img.color = on ? new Color(0.8f, 1f, 0.8f) : new Color(0.9f,0.9f,0.9f);
    }
}
