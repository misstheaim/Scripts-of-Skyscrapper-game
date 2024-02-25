using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryManager : MonoBehaviour
{
    public Button dataButton;
    public GameObject gridContent;
    public GameObject listIsEmpty;
    public float offset = 10;
    private bool isSet = false;
    public GameObject menuManager;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateHistoryList() {
        DataManager dataManager = CrossSceneInformation.dataManager;
        List<GameData> gameData = new List<GameData>(dataManager.data);

        if (isSet) {
            int i = 1;
            int countChild = gridContent.transform.childCount;
            while (i < countChild) {
                Transform child = gridContent.transform.GetChild(1);
                child.SetParent(null, false);
                Destroy(child.gameObject);
                i++;
            }
        }
        
        listIsEmpty.SetActive(false);

        if (gameData.Count == 0) {
            listIsEmpty.SetActive(true);
            return;
        }
        
        //gameData.Reverse();
        // for (int i = 0; i < 10; i++) {
        //     gameData.Add(new GameData(5555, 5, 5));
        // }
        float height = dataButton.GetComponent<RectTransform>().rect.height;
        float posX = dataButton.GetComponent<RectTransform>().localPosition.x;
        float posY = dataButton.GetComponent<RectTransform>().localPosition.y;
        float heightToAdd = -(height + offset);

        foreach (GameData data in gameData) {
            string dataFileName = "data-" + data.date;
            heightToAdd += height + offset;
            
            Button dataButtonVariant = Instantiate(dataButton, new Vector2(0, 0), Quaternion.identity, gridContent.transform);
            dataButtonVariant.GetComponent<RectTransform>().localPosition = new Vector2(posX, posY - heightToAdd);
            dataButtonVariant.name = dataFileName;
            dataButtonVariant.onClick.AddListener(() => menuManager.GetComponent<MenuManagerHub>().OnLoadClick(dataFileName));
            dataButtonVariant.transform.Find("Seed Panel").Find("Delete").GetComponent<Button>().onClick.AddListener(
                () => menuManager.GetComponent<MenuManagerHub>().OnDeleteClick(dataFileName));
                CreateSaveButton(dataButtonVariant, data);
        }

        heightToAdd += height + offset;
        gridContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, heightToAdd);

        isSet = true;
    }

    void CreateSaveButton(Button button, GameData gameData) {
        Color color = button.GetComponent<Image>().color;
        string complition = "";
        switch (gameData.isComleted) {
            case "in process":
                complition = "In process";
                color = new Color(0.4716f, 0.4716f, 0.4716f, 0.8509f);
                break;
            case "gived up":
                complition = "Gived up";
                color = new Color(0.4056f, 0.3039f, 0.3004f, 0.8509f);
                break;
            case "completed" :
                complition = "Completed";
                color = new Color(0.3019f, 0.4039f, 0.3139f, 0.8509f);
                break;
        }

        string difficulty;
        if (gameData.difficulty == 0) difficulty = "Medium";
        else difficulty = "Hard";

        string dateText = new DateTime(gameData.date).ToString("d");
        string gridSize = gameData.sizeOfGrid + " x " + gameData.sizeOfGrid;
        button.transform.Find("Seed Panel").Find("Seed").GetComponent<TextMeshProUGUI>().text = "Seed: " + gameData.seed;
        button.transform.Find("Date").Find("Text (TMP) (1)").GetComponent<TextMeshProUGUI>().text = "Date: " + dateText;
        button.transform.Find("Grid size").Find("Text (TMP) (1)").GetComponent<TextMeshProUGUI>().text = gridSize;
        button.transform.Find("Complition").Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = complition;
        button.transform.Find("Complition").Find("Difficulty").GetComponent<TextMeshProUGUI>().text = difficulty;
        button.transform.Find("Timer").Find("Text (TMP) (1)").GetComponent<TextMeshProUGUI>().text = gameData.timeInText;

        button.transform.Find("Complition").GetComponent<Image>().color = color;
    }
}
