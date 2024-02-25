using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject fieldCanvas;
    public GameObject blure;
    public GameObject menu;
    private FieldController fieldController;

    public GameObject endpanel;
    public GameObject choosingPanel;
    public GameObject timer;
    
    // Start is called before the first frame update
    void Start()
    {
        fieldController = fieldCanvas.GetComponent<FieldController>();
    }



    public void OnRestartClick() {
        CrossSceneInformation.seed = fieldController.seed;
        CrossSceneInformation.count = fieldController.count;
        CrossSceneInformation.difficulty = fieldController.difficulty;
        CrossSceneInformation.currentData.gamerActions = null;
        CrossSceneInformation.currentData.placedPrintedNums = new SerializableDictionary<string, string>();;
        CrossSceneInformation.currentData.placedPencildNums = new SerializableDictionary<string, string>();;
        CrossSceneInformation.currentData.indexOfCurrentAction = -1;
        CrossSceneInformation.currentData.isComleted = "in process";
        //CrossSceneInformation.currentData.time = 0;
        CrossSceneInformation.dataManager.UpdateTheData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void OnGiveUpClick() {
        fieldController.SetAllBuildings();
        fieldController.KeyBoardDisable();
        CrossSceneInformation.currentData.isComleted = "gived up";
        CrossSceneInformation.dataManager.UpdateTheData();

        fieldController.GameState();
        MenuClose();
    }

    public void OnNewGameClick() {
        MenuClose();
        blure.SetActive(true);
        //endpanel.SetActive(true);
        timer.GetComponent<TimerScript>().isGameEnded = true;


        choosingPanel.SetActive(true);
        choosingPanel.transform.Find("Choose Difficulty").gameObject.SetActive(true);
    }

    public void OnNextGameClick() {
        CrossSceneInformation.seed = GenerateSeed();
        CrossSceneInformation.count = fieldController.count;
        CrossSceneInformation.difficulty = fieldController.difficulty;
        CrossSceneInformation.dataManager.CreateNewData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitGameClick() {
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
    }

    public void OnMainMenuClick() {
        SceneManager.LoadScene("HubScene");
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

        CrossSceneInformation.dataManager.CreateNewData();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MenuClose() {
        blure.SetActive(false);
        menu.SetActive(false);
    }

    private int GenerateSeed() {
        System.Random rnd = new System.Random();
        int seed = rnd.Next(System.Int32.MinValue, System.Int32.MaxValue);
        return seed;
    }

}
