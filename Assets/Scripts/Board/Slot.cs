// Assets/Scripts/Board/Slot.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Vector2Int gridPos;
    public bool occupied;

    Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        SetOccupiedVisual(false);
    }

    public void SetOccupiedVisual(bool occ)
    {
        occupied = occ;

        if (!img)
            img = GetComponent<Image>();

        if (!img)
            return;

        if (occupied)
        {
            // Filled cell color
            img.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            // Empty cell color
            img.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        }
    }

    public void ResetSlot()
    {
        transform.localScale = Vector3.one;
        SetOccupiedVisual(false);
    }

    // --------- Clear visual effect ---------

    public void PlayClearEffect()
    {
        StartCoroutine(ClearEffectRoutine());
    }

    IEnumerator ClearEffectRoutine()
    {
        if (!img)
            img = GetComponent<Image>();

        Color startColor = img ? img.color : Color.white;
        float duration = 0.15f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float f = t / duration;

            // small pulse scale
            float scale = Mathf.Lerp(1f, 1.2f, f);
            transform.localScale = new Vector3(scale, scale, 1f);

            // fade alpha
            if (img)
            {
                float alpha = Mathf.Lerp(1f, 0f, f);
                img.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }

            yield return null;
        }

        // reset
        transform.localScale = Vector3.one;
        SetOccupiedVisual(false);
    }
}
