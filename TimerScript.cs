using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{

    private int time = 0;
    public decimal deltaTime = 0;
    public bool isGameEnded = false;

    // Update is called once per frame
    void Update()
    {
        if (!isGameEnded) TimeUpdate();
    }

    void Start() {
        TimeUpdateForStart();
    }

    void TimeUpdate() {
        deltaTime += (decimal)Time.deltaTime;
        time = (int)System.Math.Floor(deltaTime);
        float minutes = 0;
        float hours = 0;
        float seconds = 0;
        string timeString = "";
        if (time > 59) {
            minutes = Mathf.Floor(time / 60);
            seconds = time % 60;
            string minutesST = minutes + "";
            string secondsST = seconds + "";
            if (minutes < 10) minutesST = "0" + minutes;
            if (seconds < 10) secondsST = "0" + seconds;
            timeString = minutesST + " : " + secondsST;
        } else {
            string secondsST = time + "";
            if (time < 10) secondsST = "0" + time;
            timeString = "00 : " + secondsST;
        }
        if (minutes > 59) {
            hours = Mathf.Floor(minutes / 60);
            minutes = minutes % 60;
            string minutesST = minutes + "";
            string secondsST = seconds + "";
            if (minutes < 10) minutesST = "0" + minutes;
            if (seconds < 10) secondsST = "0" + seconds;
            timeString = hours + " : " + minutesST + " : " + secondsST;
        }
        
        CrossSceneInformation.currentData.time = time; // Save and Load system
        CrossSceneInformation.currentData.timeInText = timeString; // Save and Load system
        CrossSceneInformation.dataManager.UpdateTheData(); // Save and Load system
        gameObject.GetComponent<TextMeshProUGUI>().text = timeString;
    }

    void TimeUpdateForStart() {
        deltaTime = CrossSceneInformation.currentData.time;
        time = (int)System.Math.Floor(deltaTime);
        float minutes = 0;
        float hours = 0;
        float seconds = 0;
        string timeString = "";
        if (time > 59) {
            minutes = Mathf.Floor(time / 60);
            seconds = time % 60;
            string minutesST = minutes + "";
            string secondsST = seconds + "";
            if (minutes < 10) minutesST = "0" + minutes;
            if (seconds < 10) secondsST = "0" + seconds;
            timeString = minutesST + " : " + secondsST;
        } else {
            string secondsST = time + "";
            if (time < 10) secondsST = "0" + time;
            timeString = "00 : " + secondsST;
        }
        if (minutes > 59) {
            hours = Mathf.Floor(minutes / 60);
            minutes = minutes % 60;
            string minutesST = minutes + "";
            string secondsST = seconds + "";
            if (minutes < 10) minutesST = "0" + minutes;
            if (seconds < 10) secondsST = "0" + seconds;
            timeString = hours + " : " + minutesST + " : " + secondsST;
        }
        
        gameObject.GetComponent<TextMeshProUGUI>().text = timeString;
    }
}
