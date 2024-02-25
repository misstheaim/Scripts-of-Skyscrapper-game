using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManagerHub : MonoBehaviour
{

    public GameObject choosingPanel;
    public Button continueButton;
    private DataManager dataManager;
    public GameObject historyManager;
    // Start is called before the first frame update
    void Start()
    {
        dataManager = CrossSceneInformation.dataManager;
        if (dataManager.data.Count == 0) {
            Debug.Log("set false");
            continueButton.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnStartClick() {
        choosingPanel.transform.Find("Choose Difficulty").gameObject.SetActive(true);
        choosingPanel.transform.Find("Choose grid").gameObject.SetActive(false);
    }

    public void OnContinueClick() {
        long lastDate = DateTime.Now.Ticks;
        GameData loadingData = dataManager.data.First();
        
        CrossSceneInformation.currentData = loadingData;
        CrossSceneInformation.seed = loadingData.seed;
        CrossSceneInformation.difficulty = loadingData.difficulty;
        CrossSceneInformation.count = loadingData.sizeOfGrid;
        CrossSceneInformation.isLoading = true;

        CrossSceneInformation.currentData.lastDate = lastDate;

        SceneManager.LoadScene("GameScene");
    }

    public void OnLoadClick(string fileName) {
        long lastDate = DateTime.Now.Ticks;
        GameData loadingData = dataManager.data.Find(x => x.date + "" == fileName.Substring(5)); // 5 because "data-" adds in the beginning of the file name;

        CrossSceneInformation.currentData = loadingData;
        CrossSceneInformation.seed = loadingData.seed;
        CrossSceneInformation.difficulty = loadingData.difficulty;
        CrossSceneInformation.count = loadingData.sizeOfGrid;
        CrossSceneInformation.isLoading = true;

        loadingData.lastDate = lastDate;
        dataManager.data.Remove(loadingData);
        dataManager.data.Insert(0, loadingData);

        SceneManager.LoadScene("GameScene");
    }

    public void OnHistoryClick() {
        historyManager.GetComponent<HistoryManager>().CreateHistoryList();
    }

    public void OnDeleteClick(string dataName) {
        dataManager.DeleteTheData(dataName);
        historyManager.GetComponent<HistoryManager>().CreateHistoryList();
    }

    public void OnQuitGameClick() {
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
    }

    public void OnDifficultyClick(Button button) {
        string difficulty = button.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text;

        switch (difficulty) 
        {
            case "Medium":
                CrossSceneInformation.difficulty = 0;
                break;
            case "Hard":
                CrossSceneInformation.difficulty = 1;
                break;
        }

        choosingPanel.transform.Find("Choose Difficulty").gameObject.SetActive(false);
        choosingPanel.transform.Find("Choose grid").gameObject.SetActive(true);
    }

    public void OnGridClick(Button button) {
        CrossSceneInformation.seed = GenerateSeed();

        string grid = button.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text;

        switch (grid) 
        {
            case "3X3":
                CrossSceneInformation.count = 3;
                break;
            case "4X4":
                CrossSceneInformation.count = 4;
                break;
            case "5X5":
                CrossSceneInformation.count = 5;
                break;
            case "6X6":
                CrossSceneInformation.count = 6;
                break;
            case "7X7":
                CrossSceneInformation.count = 7;
                break;
            case "8X8":
                CrossSceneInformation.count = 8;
                break;        
        }

        dataManager.CreateNewData();
        CrossSceneInformation.isLoading = false;

        SceneManager.LoadScene("GameScene");
    }


    private int GenerateSeed() {
        System.Random rnd = new System.Random();
        int seed = rnd.Next(System.Int32.MinValue, System.Int32.MaxValue);
        return seed;
    }
}
