using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UndoRedoManager : MonoBehaviour
{
    public GameObject fieldController;
    public GameObject wrongPanel;

    public void OnUndoClicked() {
        GameActionManager gameActionManager = fieldController.GetComponent<FieldController>().gameActionManager;
        if (gameActionManager.indexOfCurrentAction < 0) return;
        wrongPanel.SetActive(false);
        GamerAction processableAction = gameActionManager.GetPreviousAction();

        switch (processableAction.action) {
            case "PrintedInput" :
                DeletePrintedNum(processableAction);
                break;
            case "PencilInput" :
                DeletePencilNum(processableAction);
                break;
            case "PrintedDelete" :
                ReturnPrintedNum(processableAction);
                break;
            case "PencilDelete" :
                ReturnPencilNums(processableAction);
                break;
        }
    }

    public void OnRedoClicked() {
        GameActionManager gameActionManager = fieldController.GetComponent<FieldController>().gameActionManager;
        if (gameActionManager.indexOfCurrentAction >= gameActionManager.GetActionsCount() - 1) return;
        GamerAction processableAction = gameActionManager.GetNextAction();

        switch (processableAction.action) {
            case "PrintedInput" :
                ReturnPrintedNum(processableAction);
                break;
            case "PencilInput" :
                ReturnPencilNums(processableAction);
                break;
            case "PrintedDelete" :
                DeletePrintedNum(processableAction);
                break;
            case "PencilDelete" :
                DeletePencilNum(processableAction);
                break;
        }
    }

    void ReturnPrintedNum(GamerAction processableAction) {
        Transform cell = fieldController.transform.Find(processableAction.cell);
        cell.Find("Printed").gameObject.SetActive(true);
        cell.Find("Pencil").gameObject.SetActive(false);
        cell.Find("Printed").GetComponent<TextMeshProUGUI>().text = processableAction.number;
        fieldController.GetComponent<FieldController>().SelectedButtonUpdate();

        if (!CrossSceneInformation.currentData.placedPrintedNums.ContainsKey(processableAction.cell)) {
            CrossSceneInformation.currentData.placedPrintedNums.Add(processableAction.cell, processableAction.number); //Save and load system
        } else CrossSceneInformation.currentData.placedPrintedNums[processableAction.cell] = processableAction.number;
        CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
    }
    void ReturnPencilNums(GamerAction processableAction) {
        Transform cell = fieldController.transform.Find(processableAction.cell);
        cell.Find("Printed").gameObject.SetActive(false);
        cell.Find("Pencil").gameObject.SetActive(true);
        cell.Find("Pencil").GetComponent<TextMeshProUGUI>().text = processableAction.number;
        fieldController.GetComponent<FieldController>().SelectedButtonUpdate();

        if (!CrossSceneInformation.currentData.placedPencildNums.ContainsKey(processableAction.cell)) {
            CrossSceneInformation.currentData.placedPencildNums.Add(processableAction.cell, processableAction.number); //Save and load system
        } else CrossSceneInformation.currentData.placedPencildNums[processableAction.cell] = processableAction.number;
        CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
    }
    void DeletePrintedNum(GamerAction processableAction) {
        Transform cell = fieldController.transform.Find(processableAction.cell);
        cell.Find("Printed").GetComponent<TextMeshProUGUI>().text = "";
        fieldController.GetComponent<FieldController>().SelectedButtonUpdate();
        cell.Find("Pencil").gameObject.SetActive(true);

        if (CrossSceneInformation.currentData.placedPrintedNums.ContainsKey(processableAction.cell)) {
            CrossSceneInformation.currentData.placedPrintedNums.Remove(processableAction.cell); //Save and load system
            CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
        }
    }
    void DeletePencilNum(GamerAction processableAction) {
        Transform cell = fieldController.transform.Find(processableAction.cell);
        string pencilText = cell.Find("Pencil").GetComponent<TextMeshProUGUI>().text;
        if ( pencilText.Length == 0) return;
        pencilText = pencilText.Substring(0, pencilText.Length - 1);
        cell.Find("Pencil").GetComponent<TextMeshProUGUI>().text = pencilText;
        fieldController.GetComponent<FieldController>().SelectedButtonUpdate();

        if (!CrossSceneInformation.currentData.placedPencildNums.ContainsKey(processableAction.cell)) {
            CrossSceneInformation.currentData.placedPencildNums.Add(processableAction.cell, pencilText); //Save and load system
        } else CrossSceneInformation.currentData.placedPencildNums[processableAction.cell] = pencilText;
        CrossSceneInformation.dataManager.UpdateTheData(); //Save and load system
    }
}


