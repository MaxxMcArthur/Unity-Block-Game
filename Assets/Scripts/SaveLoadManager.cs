// Assets/Scripts/SaveLoadManager.cs
using System;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour {
    [Header("References")]
    public BoardGenerator board;            // assign the BoardGrid (object with BoardGenerator)
    public RectTransform[] draggableBlocks; // assign Block, Block (1), Block (2) in order

    [Header("Save file")]
    public string saveFileName = "save_slot_1.json";

    string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    public void SaveToFile() {
        if (board == null) { Debug.LogError("[SaveLoad] Save failed: board not assigned."); return; }

        SaveData data = new SaveData();
        data.boardCells = board.GetFlattenedBoard();

        int count = (draggableBlocks != null) ? draggableBlocks.Length : 0;
        data.blocks = new BlockSave[count];

        for (int i = 0; i < count; i++) {
            var rt = draggableBlocks[i];
            var bs = new BlockSave();
            bs.placedIndex = GetBlockPlacedIndexFromBoard(rt);
            Vector2 anchored = rt != null ? rt.anchoredPosition : Vector2.zero;
            bs.anchoredPos = Vector2Serializable.FromVector2(anchored);
            data.blocks[i] = bs;
        }

        string json = JsonUtility.ToJson(data, true);
        try {
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveLoad] Saved game to {SavePath}\n{json}");
        } catch (Exception ex) {
            Debug.LogError("[SaveLoad] Failed to write save file: " + ex.Message);
        }
    }

    public void LoadFromFile() {
        if (board == null) { Debug.LogError("[SaveLoad] Load failed: board not assigned."); return; }
        if (!File.Exists(SavePath)) { Debug.LogWarning("[SaveLoad] No save file at " + SavePath); return; }

        string json = File.ReadAllText(SavePath);
        SaveData data = null;
        try { data = JsonUtility.FromJson<SaveData>(json); } 
        catch (Exception ex) { Debug.LogError("[SaveLoad] Parse failed: " + ex.Message); return; }

        if (data == null) { Debug.LogError("[SaveLoad] Parsed save was null."); return; }

        // Apply board
        board.LoadFromFlattened(board.size, data.boardCells);

        // Apply blocks
        int count = (draggableBlocks != null) ? draggableBlocks.Length : 0;
        int savedCount = (data.blocks != null) ? data.blocks.Length : 0;
        int iter = Math.Min(count, savedCount);

        for (int i = 0; i < iter; i++) {
            var rt = draggableBlocks[i];
            var bdata = data.blocks[i];
            if (rt == null) continue;

            if (bdata.placedIndex >= 0) {
                Slot s = board.GetSlotByIndex(bdata.placedIndex);
                if (s != null) {
                    RectTransform slotRT = s.GetComponent<RectTransform>();
                    if (slotRT != null) rt.position = slotRT.position;
                } else {
                    rt.anchoredPosition = bdata.anchoredPos.ToVector2();
                }
            } else {
                rt.anchoredPosition = bdata.anchoredPos.ToVector2();
            }
        }

        Debug.Log("[SaveLoad] Loaded save from " + SavePath);
    }

    int GetBlockPlacedIndexFromBoard(RectTransform rt) {
        if (rt == null || board == null) return -1;
        Vector3 blockWorld = rt.position;
        int bestIdx = -1;
        float bestDist = float.MaxValue;
        int total = board.size * board.size;
        for (int i = 0; i < total; i++) {
            Slot s = board.GetSlotByIndex(i);
            if (s == null) continue;
            Vector3 slotWorld = s.GetComponent<RectTransform>().position;
            float d = (blockWorld - slotWorld).sqrMagnitude;
            if (d < bestDist) { bestDist = d; bestIdx = i; }
        }
        if (bestIdx >= 0 && bestDist < 40f * 40f) return bestIdx;
        return -1;
    }
}
