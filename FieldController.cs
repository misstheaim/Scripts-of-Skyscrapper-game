using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FieldController : MonoBehaviour
{
    public Button cell;
    public TextMeshProUGUI number;
    public Canvas mainUI;
    public Image keyboardArea;
    public Button buttonNumber;
    public GameObject panel;
    public GameObject endPanel;
    public GameObject timer;
    public GameObject wrongPanel;
    public int count = 3;
    public int difficulty = 0;
    public int seed;
    public float widthOfFrame = 30.75f;
    private float coeff = 5.2f;
    private int constantNum = 3; //it needed because I calculate grid from 3x3
    private NumberGenerator numGen;
    public int buttonsPerRow = 5;
    private List<Button> listOfKeyNums = new List<Button>();
    private Button selectedButton;
    private bool pencilActivayed = false;
    private bool eraseActivated = false;
    public GameActionManager gameActionManager;
    public int lengthOfBuffer = 10;
    private Dictionary<string, string> placedBuildings;
    private Dictionary<string, string> placedPencilBuildings;
    private List<Button> listOfIncorrectCells;


    // Start is called before the first frame update
    void Start()
    {
        if (CrossSceneInformation.currentData.gamerActions != null) {
            gameActionManager = new GameActionManager( CrossSceneInformation.currentData.gamerActions, CrossSceneInformation.currentData.indexOfCurrentAction, lengthOfBuffer);
        } else {
            gameActionManager = new GameActionManager( lengthOfBuffer );
        }        

        count = CrossSceneInformation.count;
        difficulty = CrossSceneInformation.difficulty;
        seed = CrossSceneInformation.seed;


        numGen = new NumberGenerator(seed, count);
        
        
        GenerateGrid(count);
        if (CrossSceneInformation.currentData.gridOfViewNums == null) SetUpDifficulty(difficulty);
        else SetUpDifficulty(CrossSceneInformation.currentData.gridOfViewNums);
        SetUpNumKeyboard();

        placedBuildings = CrossSceneInformation.currentData.placedPrintedNums;
        if (placedBuildings.Count != 0) SetPlacedBuidings();
        placedPencilBuildings = CrossSceneInformation.currentData.placedPencildNums;
        if (placedPencilBuildings.Count != 0) SetPlacedPencilBuildings();

        panel.GetComponent<Renderer>().sharedMaterial.SetFloat("_Tiling", count);

        GameState();
    }

    // Update is called once per frame
    void Update()
    {
        ButtonDeselect();
    }

    public void GameState() {
        switch (CrossSceneInformation.currentData.isComleted) {
            case "in process":
                break;
            case "gived up":
                endPanel.transform.Find("End panel").GetComponent<Image>().color = new Color(0.4245f, 0.1702f, 0.1702f, 0.5882f);
                endPanel.SetActive(true);
                timer.GetComponent<TimerScript>().isGameEnded = true;
                KeyBoardDisable();
                SetAllBuildings();
                break;
            case "completed" :
                endPanel.transform.Find("End panel").GetComponent<Image>().color = new Color(0.2173f, 0.4235f, 0.1736f, 0.5882f);
                endPanel.SetActive(true);
                timer.GetComponent<TimerScript>().isGameEnded = true;
                KeyBoardDisable();
                break;
        }
    }

    void ListenerForButtonsNums(Button button) {
        string typeOfAction; //--------Undo Redo
        string text = selectedButton.transform.Find("Printed").GetComponent<TextMeshProUGUI>().text;
        if (!System.String.IsNullOrEmpty(text)) {
            if (pencilActivayed) {
                return;
            }
            foreach (Button key in listOfKeyNums) {
                if (key.name == text) key.interactable = true;
            }  
        }
        Transform printedText;
        string textToAdd = "";
        if (pencilActivayed) {
            typeOfAction = "PencilInput"; //--------Undo Redo
            printedText = selectedButton.transform.Find("Pencil");
            textToAdd = printedText.GetComponent<TextMeshProUGUI>().text;
            string newText = "";
            bool excist = false;
            foreach (char num in textToAdd) {
                if (num != button.name[0]) {
                    newText += num;
                } else {
                    excist = true;
                }
            }
            if (!excist) newText += button.name;
            textToAdd = newText;
            selectedButton.transform.Find("Printed").gameObject.SetActive(false);

            if (!placedPencilBuildings.ContainsKey(selectedButton.name)) { //Save and load system
                placedPencilBuildings.Add(selectedButton.name, textToAdd); //Save and load system
            } else placedPencilBuildings[selectedButton.name] = textToAdd; //Save and load system
            CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
        }
        else {
            typeOfAction = "PrintedInput"; //--------Undo Redo
            printedText = selectedButton.transform.Find("Printed");
            selectedButton.transform.Find("Pencil").gameObject.SetActive(false);
            button.interactable = false;
            textToAdd += button.name;

            if (!placedBuildings.ContainsKey(selectedButton.name)) { //Save and load system
                placedBuildings.Add(selectedButton.name, textToAdd); //Save and load system
            } else placedBuildings[selectedButton.name] = textToAdd; //Save and load system
            CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
        }
        printedText.gameObject.SetActive(true);
        printedText.GetComponent<TextMeshProUGUI>().text = textToAdd;
        gameActionManager.AddAction(typeOfAction, textToAdd , selectedButton.name); //--------Undo Redo
        
        bool[] isGridFullANDIsGridCorrect = IsGridFullANDIsGridCorrect();
        //Debug.Log(isGridFullANDIsGridCorrect[0] + " " + isGridFullANDIsGridCorrect[1]);
        if (isGridFullANDIsGridCorrect[0]) {
            if (isGridFullANDIsGridCorrect[1]) {
                KeyBoardDisable();
                CrossSceneInformation.currentData.isComleted = "completed";
                CrossSceneInformation.dataManager.UpdateTheData();

                GameState();
            } else {
                wrongPanel.SetActive(true);
                Color color = new Color(0.4245f, 0.1702f, 0.1702f, 0.5882f);
                selectedButton = null;
                EventSystem.current.SetSelectedGameObject(null);
                StartCoroutine(TimeDelay(listOfIncorrectCells, color));   
            }
        } else wrongPanel.SetActive(false);
    }

    IEnumerator TimeDelay(List<Button> cells, Color colorRed)
    {
        Debug.Log(listOfIncorrectCells.Count);
        Color color = new Color(0.4245f, 0.1702f, 0.1702f, 0);
        foreach (Button cell in cells) {
            var colors = cell.colors;
            colors.normalColor = colorRed;
            cell.colors = colors;
        }
        yield return new WaitForSeconds(1);
        foreach (Button cell in cells) {
            var colors = cell.colors;
            colors.normalColor = color;
            cell.colors = colors;
        }
        yield return new WaitForSeconds(1);
        foreach (Button cell in cells) {
            var colors = cell.colors;
            colors.normalColor = colorRed;
            cell.colors = colors;
        }
        yield return new WaitForSeconds(1);
        foreach (Button cell in cells) {
            var colors = cell.colors;
            colors.normalColor = color;
            cell.colors = colors;
        }
        // yield return new WaitForSeconds(1);
        // foreach (Button cell in cells) {
        //     var colors = cell.colors;
        //     colors.normalColor = colorRed;
        //     cell.colors = colors;
        // }
        // yield return new WaitForSeconds(1);
        // foreach (Button cell in cells) {
        //     var colors = cell.colors;
        //     colors.normalColor = color;
        //     cell.colors = colors;
        // }
        yield return new WaitForSeconds(1);
        foreach (Button cell in cells) {
            var colors = cell.colors;
            colors.normalColor = new Color(1, 1, 1, 0);
            cell.colors = colors;
        }
    }

    public bool[] IsGridFullANDIsGridCorrect() {
        listOfIncorrectCells = new List<Button>();
        bool isCorrect;
        bool isCurrapted = false;
        bool isFull;
        int indexLvl1 = 0;
        int indexLvl2 = 0;
        int notEmptyCells = count * count;
        foreach (Transform child in transform) {
            if (child.CompareTag("Cell")) {
                TextMeshProUGUI cellNum = child.Find("Printed").GetComponent<TextMeshProUGUI>();
                string cellText = cellNum.text;
                if (!System.String.IsNullOrEmpty(cellText)) {
                    notEmptyCells--;
                    if (cellText == "" + numGen.gridOfRows[indexLvl1][indexLvl2]) {
                        isCorrect = true;
                    } else {
                        listOfIncorrectCells.Add(child.GetComponent<Button>());
                        isCurrapted = true;
                    }
                }
                indexLvl2++;
                if (indexLvl2 == numGen.gridOfRows.Length) { indexLvl2 = 0; indexLvl1++; }
            }
        }
        isCorrect = !isCurrapted;
        if (notEmptyCells == 0) isFull = true;
        else isFull = false;
        return new bool[2] { isFull, isCorrect };
    }

    public void KeyBoardDisable() {
        foreach (Button key in listOfKeyNums) {
            key.interactable = false;
        }
    }

    public void SelectedButtonUpdate() {
        if (selectedButton == null) return;
        
        int id = System.Int32.Parse(selectedButton.name);
        int rowId = (int)System.Math.Ceiling((decimal)id/count);
        int columnId = count - (count * rowId - id);
        Debug.Log(rowId + "  " + columnId);
        int[] arrayOfNum = new int[0];
        int indexOfCell = 1;
        int currentRow = 1;
        for (int i = 0 ; i < transform.childCount; i++) {
            var child = transform.GetChild(i);
            if (child.CompareTag("Cell")) {
                if (indexOfCell > count * (rowId - 1) && indexOfCell <= count * rowId || indexOfCell == count * currentRow - (count - columnId)) {
                    string text = child.Find("Printed").GetComponent<TextMeshProUGUI>().text;
                    if (!System.String.IsNullOrEmpty(text)) {
                        System.Array.Resize(ref arrayOfNum, arrayOfNum.Length + 1);
                        arrayOfNum[arrayOfNum.Length - 1] = System.Int32.Parse(text);
                    }
                } 
                indexOfCell++;
                if (indexOfCell > count * currentRow) {
                    currentRow++;
                }
            } 
        }


        int[] sortedArrayOfNum = arrayOfNum.Distinct().ToArray();
        
        foreach (Button key in listOfKeyNums) {
            int number = System.Int32.Parse(key.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text);
            bool isCompared = false;
            for (int i = 0 ; i < sortedArrayOfNum.Length; i++) {
                if (number == sortedArrayOfNum[i]) isCompared = true;
            }
            if (!isCompared) key.interactable = true;
            else key.interactable = false;
        }
    }

    public void ListenerForButtonPencil() {
        var color = mainUI.transform.Find("Pencil Button").GetComponent<Image>().color;
        if (!pencilActivayed) {
            pencilActivayed = true;
            eraseActivated = false;
            color = new Color(0.9803f, 0.9176f, 0.5686f, 1f);
            mainUI.transform.Find("Pencil Button").GetComponent<Image>().color = color;
            color = new Color(0.5849f, 0.4887f, 0.3614f, 1f);
            mainUI.transform.Find("Eraser Button").GetComponent<Image>().color = color;
            //if (selectedButton != null) EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);         
        } else {
            color = new Color(0.5849f, 0.4887f, 0.3614f, 1f);
            mainUI.transform.Find("Pencil Button").GetComponent<Image>().color = color;
            pencilActivayed = false;
        }
    }

    public void ListenerForButtonEraser() {
        var color = mainUI.transform.Find("Eraser Button").GetComponent<Image>().color;
        if (!eraseActivated) {
            eraseActivated = true;
            pencilActivayed = false;
            color = new Color(0.9803f, 0.9176f, 0.5686f, 1f);
            mainUI.transform.Find("Eraser Button").GetComponent<Image>().color = color;
            color = new Color(0.5849f, 0.4887f, 0.3614f, 1f);
            mainUI.transform.Find("Pencil Button").GetComponent<Image>().color = color;
        }
        else {
            color = new Color(0.5849f, 0.4887f, 0.3614f, 1f);
            mainUI.transform.Find("Eraser Button").GetComponent<Image>().color = color;
            eraseActivated = false;
        }
    }

    public void ListenerForButtonDelete() {
        wrongPanel.SetActive(false);
        string typeOfAction = "PrintedDelete"; //--------Undo Redo
        if (pencilActivayed) {
            typeOfAction = "PencilDelete"; //--------Undo Redo
            string pencilText = selectedButton.transform.Find("Pencil").GetComponent<TextMeshProUGUI>().text;
            string deletingNums = pencilText;
            if ( pencilText.Length == 0) return;
            pencilText = pencilText.Substring(0, pencilText.Length - 1);
            selectedButton.transform.Find("Pencil").GetComponent<TextMeshProUGUI>().text = pencilText;
            gameActionManager.AddAction(typeOfAction, deletingNums , selectedButton.name); //--------Undo Redo

            if (!placedPencilBuildings.ContainsKey(selectedButton.name)) { //Save and load system
                placedPencilBuildings.Add(selectedButton.name, pencilText); //Save and load system
            } else placedPencilBuildings[selectedButton.name] = pencilText; //Save and load system
            CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
            return;
        }
        string text = selectedButton.transform.Find("Printed").GetComponent<TextMeshProUGUI>().text;
        if (!System.String.IsNullOrEmpty(text)) {
            foreach (Button key in listOfKeyNums) {
                if (key.name == text) key.interactable = true;
            }
        }
        string deletingNum = selectedButton.transform.Find("Printed").GetComponent<TextMeshProUGUI>().text; //--------Undo Redo
        selectedButton.transform.Find("Printed").GetComponent<TextMeshProUGUI>().text = "";
        selectedButton.transform.Find("Pencil").gameObject.SetActive(true);
        gameActionManager.AddAction(typeOfAction, deletingNum, selectedButton.name); //--------Undo Redo

        if (placedBuildings.ContainsKey(selectedButton.name)) {
            placedBuildings.Remove(selectedButton.name); //Save and load system
            CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
        }
    }

    //I can't find the event of Deselecting button, so i have to write this crutch ¯\_(ツ)_/¯ 
    void ButtonDeselect() {
        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.currentSelectedGameObject == null) {
                selectedButton = null;
                mainUI.transform.Find("Delete Button").GetComponent<Button>().interactable = false;
                foreach (Button key in listOfKeyNums) {
                    key.interactable = false;
                }
            } else if (EventSystem.current.currentSelectedGameObject.name == "Pencil Button" && selectedButton != null) {
                selectedButton.Select();
            } else if (EventSystem.current.currentSelectedGameObject.name == "Eraser Button" && selectedButton != null) {
                EventSystem.current.SetSelectedGameObject(null);
                selectedButton = null;
                mainUI.transform.Find("Delete Button").GetComponent<Button>().interactable = false;
                foreach (Button key in listOfKeyNums) {
                    key.interactable = false;
                }
            } else if (EventSystem.current.currentSelectedGameObject.CompareTag("Cell")){
                //Debug.Log("Hello");
            } else if (selectedButton != null) {
                selectedButton.Select();
            }
        }
    }

    void ListenerForCells(Button cell) {
        if (eraseActivated) {
            string typeOfAction; //--------Undo Redo
            if (cell.transform.Find("Printed").GetComponent<TextMeshProUGUI>().text == "") {
                typeOfAction = "PencilDelete"; //--------Undo Redo
                string deletingNums = cell.transform.Find("Pencil").GetComponent<TextMeshProUGUI>().text; //--------Undo Redo
                cell.transform.Find("Pencil").GetComponent<TextMeshProUGUI>().text = "";
                EventSystem.current.SetSelectedGameObject(null);
                gameActionManager.AddAction(typeOfAction, deletingNums, cell.name); //--------Undo Redo

                if (placedPencilBuildings.ContainsKey(cell.name)) {
                    placedPencilBuildings.Remove(cell.name); //Save and load system
                    CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
                }
                return;
            } else if (cell.transform.Find("Printed").GetComponent<TextMeshProUGUI>().text != "") {
                wrongPanel.SetActive(false);
                typeOfAction = "PrintedDelete"; //--------Undo Redo
                string deletingNum = cell.transform.Find("Printed").GetComponent<TextMeshProUGUI>().text; //--------Undo Redo
                cell.transform.Find("Printed").GetComponent<TextMeshProUGUI>().text = "";
                EventSystem.current.SetSelectedGameObject(null);
                cell.transform.Find("Pencil").gameObject.SetActive(true);
                gameActionManager.AddAction(typeOfAction, deletingNum, cell.name); //--------Undo Redo
                
                if (placedBuildings.ContainsKey(cell.name)) {
                    placedBuildings.Remove(cell.name); //Save and load system
                    CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
                }
                return;
            }
        }
        selectedButton = cell;

        SelectedButtonUpdate();

        mainUI.transform.Find("Delete Button").GetComponent<Button>().interactable = true;
    }

    void SetUpNumKeyboard() {
        int index = 1;
        int countOfRows = 1;
        int countNumsInLastRow = count;
        bool isIncreased = false;
        int countPerRow = count;
        int countToDeleteFromPreLastRow = 0;
        float width = buttonNumber.GetComponent<RectTransform>().rect.width;
        float height = buttonNumber.GetComponent<RectTransform>().rect.height;
        float posX = buttonNumber.GetComponent<RectTransform>().localPosition.x;
        float posY = buttonNumber.GetComponent<RectTransform>().localPosition.y;
        if (count > buttonsPerRow) {
            countOfRows = (int)System.Math.Ceiling((decimal)count/buttonsPerRow);
            countPerRow = buttonsPerRow;
        }
        if (countOfRows > 1) {
            countNumsInLastRow = count - buttonsPerRow * (countOfRows - 1);
            if (countNumsInLastRow < 3) {
                countToDeleteFromPreLastRow = 3 - countNumsInLastRow;
                countNumsInLastRow += countToDeleteFromPreLastRow;
                isIncreased = true;
            }
        }
        int heightOffsetForBackground = 0;
        for (int i = 0; i < countOfRows; i++) {
            heightOffsetForBackground = i;
            if (isIncreased && i == countOfRows - 2) countPerRow -= countToDeleteFromPreLastRow;
            else if (i == countOfRows - 1) countPerRow = countNumsInLastRow;
            int side = 1;
            float offsetX = (float)System.Math.Floor((decimal)countPerRow/2);
            float offsetY = height * i;
            for (int j = 0; j < countPerRow; j++) {           
                float localOffsetX = offsetX * width * side;
                if (countPerRow % 2 == 0) localOffsetX -= width/2;
                Button button = Instantiate(buttonNumber, new Vector2(0, 0), Quaternion.identity, mainUI.transform);
                button.GetComponent<RectTransform>().localPosition = new Vector2(posX - localOffsetX, posY + offsetY);
                button.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = index + "";
                button.name = index + "";
                button.onClick.AddListener(() => ListenerForButtonsNums(button));
                listOfKeyNums.Add(button);
                index++;
                offsetX--;
            }           
        }
        buttonNumber.gameObject.SetActive(false);
        RectTransform rectOfKeyboard = keyboardArea.GetComponent<RectTransform>();
        float offset = rectOfKeyboard.rect.height * heightOffsetForBackground;
        rectOfKeyboard.sizeDelta = new Vector2(rectOfKeyboard.rect.width, height + offset);
        rectOfKeyboard.localPosition = new Vector2(rectOfKeyboard.localPosition.x, rectOfKeyboard.localPosition.y + offset/2);

    }

    void SetUpDifficulty(int diff) {
        int difficulty;
        int minimumNums = count;
        if (diff == 0) difficulty = 0;
        else {
            difficulty = 4;
            minimumNums = (int)Mathf.Floor(count/2);
            minimumNums += (int)Mathf.Floor(count/4);
        }
        switch (count) {
            case 3 :
            case 4 :
            case 5 :
                difficulty += 5;
                break;
        }


        bool[] difficultyMap = new bool[count * 4];
        int indexLvl1 = 0;
        int indexLvl2 = 0;
        int i = 0;
        int limit = count * 4;
        foreach (Transform child in transform) {
            if (child.CompareTag("num_of_buildings")) {
                TextMeshProUGUI numObject = child.GetComponent<TextMeshProUGUI>();
                numObject.text = "" + numGen.gridOfVisibleBuildings[indexLvl1][indexLvl2];
                indexLvl2++;
                if (indexLvl2 == numGen.gridOfVisibleBuildings[indexLvl1].Length) { indexLvl2 = 0; indexLvl1++; }
                int randomNumber = Random.Range(1, 11);
                if (randomNumber <= difficulty) {
                    difficultyMap[i] = false;
                    numObject.gameObject.SetActive(false);
                    limit--;
                } else {
                    difficultyMap[i] = true;
                }
                i++;
            }
        }
        if (limit < minimumNums) DifficultyHelper(difficultyMap, minimumNums, difficulty);
        CrossSceneInformation.currentData.gridOfViewNums = difficultyMap;
        CrossSceneInformation.dataManager.UpdateTheData();
    }
    private void DifficultyHelper(bool[] difficultyMap, int minimumNums, int difficulty) {
        Debug.Log("LIMIT");
        int i = 0;
        int limit = count * 4;
        foreach (Transform child in transform) {
            if (child.CompareTag("num_of_buildings")) {
                TextMeshProUGUI numObject = child.GetComponent<TextMeshProUGUI>();
                int randomNumber = Random.Range(1, 11);
                if (randomNumber <= difficulty) {
                    difficultyMap[i] = false;
                    numObject.gameObject.SetActive(false);
                    limit--;
                } else {
                    difficultyMap[i] = true;
                    numObject.gameObject.SetActive(true);
                }
                i++;
            }
        }
        if (limit < minimumNums) DifficultyHelper(difficultyMap, minimumNums, difficulty);
    }
    void SetUpDifficulty(bool[] difficultyMap) {
        int indexLvl1 = 0;
        int indexLvl2 = 0;
        int i = 0;
        foreach (Transform child in transform) {
            if (child.CompareTag("num_of_buildings")) {
                TextMeshProUGUI numObject = child.GetComponent<TextMeshProUGUI>();
                numObject.text = "" + numGen.gridOfVisibleBuildings[indexLvl1][indexLvl2];
                indexLvl2++;
                if (indexLvl2 == numGen.gridOfVisibleBuildings[indexLvl1].Length) { indexLvl2 = 0; indexLvl1++; }
                if (!difficultyMap[i]) numObject.gameObject.SetActive(false);
                i++;
            }
        }
    }

    public void SetAllBuildings() {
        int indexLvl1 = 0;
        int indexLvl2 = 0;
        foreach (Transform child in transform) {
            if (child.CompareTag("Cell")) {
                TextMeshProUGUI cellNum = child.Find("Printed").GetComponent<TextMeshProUGUI>();
                cellNum.text = "" + numGen.gridOfRows[indexLvl1][indexLvl2];
                cellNum.gameObject.SetActive(true);
                child.Find("Pencil").GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
                indexLvl2++;
                if (indexLvl2 == numGen.gridOfRows.Length) { indexLvl2 = 0; indexLvl1++; }
            }
        }
    }

    public void SetPlacedBuidings() {
        foreach (var member in placedBuildings) {
            Transform cell = transform.Find(member.Key);
            TextMeshProUGUI cellNum = cell.Find("Printed").GetComponent<TextMeshProUGUI>();
            cellNum.text = member.Value;
            cellNum.gameObject.SetActive(true);
            cell.Find("Pencil").GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
        }
    }

    public void SetPlacedPencilBuildings() {
        foreach (var member in placedPencilBuildings) {
            Transform cell = transform.Find(member.Key);
            TextMeshProUGUI cellNum = cell.Find("Pencil").GetComponent<TextMeshProUGUI>();
            cellNum.text = member.Value;
            cellNum.gameObject.SetActive(true);
            cell.Find("Printed").GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
        }
    }


    void GenerateGrid(int count) {
        float height = cell.GetComponent<RectTransform>().rect.height;
        float width = cell.GetComponent<RectTransform>().rect.width;
        float basedPosX = cell.GetComponent<RectTransform>().localPosition.x;
        float basedPosY = cell.GetComponent<RectTransform>().localPosition.y;
        int index = 1;

        //Here I calculate a distance on which cells will be generated
        float widthOfPlane = width * constantNum + widthOfFrame;
        float heightIfPlane = height * constantNum + widthOfFrame;

        float realWidth = widthOfPlane / count;
        float realHeight = heightIfPlane / count;
        // end of block

        //Here I calculate the sizes of fields per count of cells
        Button original = Instantiate(cell, new Vector2(0, 0), Quaternion.identity, gameObject.transform); // I copy the original prefab because script changes it not only for play time
        Vector3 scale = original.GetComponent<RectTransform>().localScale;
        TextMeshProUGUI copyOfNumberPrefab = Instantiate(number, new Vector2(0, 0), Quaternion.identity, gameObject.transform);

        float scaleOfFrame = scale.x * coeff / 100;
        float scaleOfField = (scale.x + scaleOfFrame) * constantNum / count - scaleOfFrame * constantNum / count;

        original.GetComponent<RectTransform>().localScale = new Vector3(scaleOfField, scaleOfField, 1);
        copyOfNumberPrefab.GetComponent<RectTransform>().localScale = new Vector3(scaleOfField, scaleOfField / 1.3f, 1);
        // end of block

        //Here I calculate center of cells
        float difference = widthOfPlane / constantNum / 2 - widthOfPlane / count / 2 ;
        float posX = basedPosX - difference;
        float posY = basedPosY + difference;

        original.GetComponent<RectTransform>().localPosition = new Vector2(posX, posY);
        copyOfNumberPrefab.GetComponent<RectTransform>().localPosition = new Vector2(posX, posY);
        // end of block

        //Cicle generates first horizontal line
        for (int j = 0; j < count; j++) {
            Button cellVariant1 = Instantiate(original, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
            cellVariant1.GetComponent<RectTransform>().localPosition = new Vector2(posX + realWidth * j, posY);
            cellVariant1.name = index + "";
            cellVariant1.onClick.AddListener(() => ListenerForCells(cellVariant1));
            index++;
        }

        //Cicles generate 4 rows of numbers of visible skyscrapers
        //Order of cicles is VERY imoprtant, it bundled with gridOfVisibleBuildings of NumberGenerator. In other words they have the same order
        for (int j = 0; j < count; j++) {
            TextMeshProUGUI numVariant = Instantiate(copyOfNumberPrefab, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
            numVariant.GetComponent<RectTransform>().localPosition = new Vector2(posX - realWidth/1.3f, posY - realHeight * j);
        }
        for (int j = 0; j < count; j++) {
            TextMeshProUGUI numVariant = Instantiate(copyOfNumberPrefab, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
            numVariant.GetComponent<RectTransform>().localPosition = new Vector2(posX + realWidth / 1.3f + realWidth * (count - 1), posY - realHeight * j);
        }
        for (int j = 0; j < count; j++) {
            TextMeshProUGUI numVariant = Instantiate(copyOfNumberPrefab, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
            numVariant.GetComponent<RectTransform>().localPosition = new Vector2(posX + realWidth * j, posY + realHeight/1.25f);
        }
        for (int j = 0; j < count; j++) {
            TextMeshProUGUI numVariant = Instantiate(copyOfNumberPrefab, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
            numVariant.GetComponent<RectTransform>().localPosition = new Vector2(posX + realWidth * j, posY - realHeight/1.25f - realHeight * (count - 1));
        }

        //Cicle generates first vertical line
        for (int i = 1; i < count; i++) {
            Button cellVariant = Instantiate(original, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
            cellVariant.GetComponent<RectTransform>().localPosition = new Vector2(posX, posY - realHeight * i);
            cellVariant.name = index + "";
            cellVariant.onClick.AddListener(() => ListenerForCells(cellVariant));
            index++;

            //Cicle generates horizontal lines
            for (int j = 1; j < count; j++) {
                Button cellVariant1 = Instantiate(original, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
                cellVariant1.GetComponent<RectTransform>().localPosition = new Vector2(posX + realWidth * j, posY - realHeight * i);
                cellVariant1.name = index + "";
                cellVariant1.onClick.AddListener(() => ListenerForCells(cellVariant1));
                index++;
            }
        }

        original.transform.SetParent(null, false);
        copyOfNumberPrefab.transform.SetParent(null, false);
        Destroy(original.gameObject);
        Destroy(copyOfNumberPrefab.gameObject);


        

        // for (int i = 1; i < count; i++) {
        //      TextMeshProUGUI numVariant0 = Instantiate(number, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
        //      TextMeshProUGUI numVariant1 = Instantiate(number, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
        //      TextMeshProUGUI numVariant2 = Instantiate(number, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
        //      TextMeshProUGUI numVariant3 = Instantiate(number, new Vector2(0, 0), Quaternion.identity, gameObject.transform);


        //      numVariant0.GetComponent<RectTransform>().localPosition = new Vector2(numPosX, numPosY - height * i);
        //      numVariant1.GetComponent<RectTransform>().localPosition = new Vector2(numPosX + width + width * i, numPosY + height);
        //      numVariant2.GetComponent<RectTransform>().localPosition = new Vector2(numPosX + width * count, numPosY - height * i);
        //      numVariant3.GetComponent<RectTransform>().localPosition = new Vector2(numPosX + width + width * i, numPosY + height * count);
        // }

        



            //Button cellVariant = Instantiate(cell, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
            //Button cellVariant1 = Instantiate(cell, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
            //Button cellVariant2 = Instantiate(cell, new Vector2(0, 0), Quaternion.identity, gameObject.transform);
            //Button cellVariant3 = Instantiate(cell, new Vector2(0, 0), Quaternion.identity, gameObject.transform);

            //cellVariant.GetComponent<RectTransform>().localPosition = new Vector2(posX, posY + height * i);
            //cellVariant1.GetComponent<RectTransform>().localPosition = new Vector2(posX + width * i, posY);
            //cellVariant2.GetComponent<RectTransform>().localPosition = new Vector2(posX - width *i, posY);
            //cellVariant3.GetComponent<RectTransform>().localPosition = new Vector2(posX, posY - height * i);
        



        
        //cellVariant.transform.position = new Vector2(1, 1);
    }
}
