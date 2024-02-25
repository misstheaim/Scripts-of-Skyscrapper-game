
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// public enum TypeOfAction {
//     PencilDelete,
//     PrintedDelete,
//     PrintedInput,
//     PencilInput
// }
// public class GamerAction
// {
//     public TypeOfAction action;
//     public string number;
//     public string cell;

//     public GamerAction(TypeOfAction action, string number, string cell) {
//         this.action = action;
//         this.number = number;
//         this.cell = cell;
//     }
// }
[System.Serializable]
public class GamerAction {
    public string action;
    public string number;
    public string cell;

    public GamerAction(string action, string number, string cell) { 
        this.action = action;
        this.number = number;
        this.cell = cell;
    }
}

public class GameActionManager
{
    private List<GamerAction> actions;
    public int indexOfCurrentAction;
    public int lengthOfActionsList;
    public GameObject fieldControllerObject;

    public int GetActionsCount() {
        return this.actions.Count;
    }

    public GameActionManager (int length) {
        actions = new List<GamerAction>(length);
        lengthOfActionsList = length;
        indexOfCurrentAction = -1;

        CrossSceneInformation.currentData.gamerActions = new List<GamerAction>();
    }
    public GameActionManager (List<GamerAction> actions, int indexOfCurrentAction, int length) {
        this.actions = new List<GamerAction>(actions);
        lengthOfActionsList = length; 
        this.indexOfCurrentAction = indexOfCurrentAction;
    }

    public void AddAction(string action, string number, string cell) {
        if (indexOfCurrentAction < actions.Count - 1) ClearActionsAfterCurrent();
        GamerAction gameAct = new GamerAction(action, number, cell);
        if (actions.Count == lengthOfActionsList) {
            actions.RemoveAt(0);
            indexOfCurrentAction--;

            CrossSceneInformation.currentData.gamerActions.RemoveAt(0); // Save and Load system
        }
        actions.Add(gameAct);
        indexOfCurrentAction++;

        CrossSceneInformation.currentData.indexOfCurrentAction = indexOfCurrentAction; // Save and Load system
        CrossSceneInformation.currentData.gamerActions.Add(gameAct); // Save and Load system
        CrossSceneInformation.dataManager.UpdateTheData(); // Save and Load system
    }
    public GamerAction GetPreviousAction() {
        GamerAction previousAction = actions[indexOfCurrentAction];
        indexOfCurrentAction--;
        CrossSceneInformation.currentData.indexOfCurrentAction = indexOfCurrentAction; // Save and Load system
        CrossSceneInformation.dataManager.UpdateTheData(); // Save and Load system
        return previousAction;
    }
    public GamerAction GetNextAction() {
        if (indexOfCurrentAction < actions.Count - 1) {
            indexOfCurrentAction++;
        }
        GamerAction nextAction = actions[indexOfCurrentAction];
        CrossSceneInformation.currentData.indexOfCurrentAction = indexOfCurrentAction; // Save and Load system
        CrossSceneInformation.dataManager.UpdateTheData(); // Save and Load system
        return nextAction;
    }
    public void ClearActionsAfterCurrent() {
        int range = actions.Count - indexOfCurrentAction - 1;
        if (range > 0) {
            actions.RemoveRange(indexOfCurrentAction + 1, range);
            CrossSceneInformation.currentData.gamerActions.RemoveRange(indexOfCurrentAction + 1, range); // Save and Load system
        }
        indexOfCurrentAction = actions.Count - 1;

        CrossSceneInformation.currentData.indexOfCurrentAction = indexOfCurrentAction; // Save and Load system
        CrossSceneInformation.dataManager.UpdateTheData(); // Save and Load system
    }
    public List<GamerAction> GetCopyOfListActions() {
        List<GamerAction> newList = new List<GamerAction>(actions);
        return newList;
    }
}
