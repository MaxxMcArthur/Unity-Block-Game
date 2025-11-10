// Assets/Scripts/SaveData.cs
using System;
using UnityEngine;

[Serializable]
public class SaveData {
    public int[] boardCells;
    public BlockSave[] blocks;
}

[Serializable]
public class BlockSave {
    public int placedIndex;
    public Vector2Serializable anchoredPos;
}

[Serializable]
public struct Vector2Serializable {
    public float x;
    public float y;
    public Vector2Serializable(float x, float y) { this.x = x; this.y = y; }

    // helper conversion
    public Vector2 ToVector2() => new Vector2(x, y);
    public static Vector2Serializable FromVector2(Vector2 v) => new Vector2Serializable(v.x, v.y);
}
