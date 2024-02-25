using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath = "";

    public FileDataHandler(string dataDirPath) {
        this.dataDirPath = Path.Combine(dataDirPath, "data");
        if (!Directory.Exists(this.dataDirPath)) {
            Directory.CreateDirectory(this.dataDirPath);
        }
    }

    public GameData Load(string dataFileName) {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData dataToReturn = null;

        try 
        {
            string dataInJson = "";

            using (StreamReader reader = new StreamReader(fullPath)) {
                dataInJson = reader.ReadToEnd();
            }
            dataToReturn = JsonUtility.FromJson<GameData>(dataInJson);
        }
        catch (Exception ex) 
        {
            Debug.LogError("Error during loading the data: " + ex);
        }
        return dataToReturn;
    }

    public void Save(GameData data) {
        string dataFileName = "data-" + data.date;
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try 
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataInJson = JsonUtility.ToJson(data, true);

            using (StreamWriter sw = new StreamWriter(fullPath, false)) {
                sw.Write(dataInJson);
            }

        }
        catch (Exception ex) 
        {
            Debug.LogError("Error during saving the data: " + ex);
        }
    }

    public List<GameData> LoadListOfAllData() {
        List<GameData> dataList = new List<GameData>();

        string[] dataFileNames = Directory.GetFiles(dataDirPath);

        foreach (string file in dataFileNames) {
            string fullPath = Path.Combine(dataDirPath, file);
            GameData gameData = null;

            try 
            {
                string dataInJson = "";

                using (StreamReader reader = new StreamReader(fullPath)) {
                    dataInJson = reader.ReadToEnd();
                }
                gameData = JsonUtility.FromJson<GameData>(dataInJson);

            }
            catch (Exception ex) 
            {
                Debug.LogError("Error during loading all the data: " + ex);
            }

            dataList.Add(gameData);
        }

        return dataList;
    }

    public void DeleteData(string dataFileName) {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            FileInfo file = new FileInfo(fullPath);
            if (file.Exists) {
                file.Delete();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during Deleting the data: " + ex);
        }
    }

}
