using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public List<GameData> data;
    private FileDataHandler dataHandler;
    public int historyLength = 100;

    public DataManager() {
        dataHandler = new FileDataHandler(Application.persistentDataPath);
        LoadListOfData();
    }


    public void CreateNewData() {
        int seed = CrossSceneInformation.seed;
        int difficulty = CrossSceneInformation.difficulty;
        int sizeOfGrid = CrossSceneInformation.count;
        GameData newData = new GameData(seed, difficulty, sizeOfGrid);


        if (data.Count >= historyLength) {
            string dataFileName = "data-" + data[0].date;
            data.RemoveAt(data.Count - 1);
            DeleteTheData(dataFileName);   
        }
        data.Insert(0, newData);
        dataHandler.Save(newData);
        CrossSceneInformation.currentData = newData;
    }
    
    public void UpdateTheData() {
        dataHandler.Save(CrossSceneInformation.currentData);
    }

    public void DeleteTheData(string dataName) {
        dataHandler.DeleteData(dataName);
        GameData gameData = data.Find(x => x.date + "" == dataName.Substring(5)); // 5 because "data-" adds in the beginning of the file name
        Debug.Log(gameData.seed);
        data.Remove(gameData);


    }

    public void LoadListOfData() {
        data = dataHandler.LoadListOfAllData();
        data.Sort((x, y) => y.lastDate.CompareTo(x.lastDate));
    }






    public void OnApplicationQuit() {

    }

}
