using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int seed;
    public long date;
    public long lastDate;
    public long time;
    public string timeInText;
    public int difficulty;
    public int sizeOfGrid;
    public string isComleted = "in process";
    public int[][] grid;
    public bool[] gridOfViewNums;
    public SerializableDictionary<string, string> placedPrintedNums = new SerializableDictionary<string, string>();
    public SerializableDictionary<string, string> placedPencildNums = new SerializableDictionary<string, string>();
    public List<GamerAction> gamerActions;
    public int indexOfCurrentAction;
    
    

    public GameData(int seed, int difficulty, int sizeOfGrid) {
        date = DateTime.Now.Ticks;
        lastDate = date;
        time = 0;
        this.seed = seed;
        this.difficulty = difficulty;
        this.sizeOfGrid = sizeOfGrid;
    }
}
