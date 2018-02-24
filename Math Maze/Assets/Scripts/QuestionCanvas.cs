using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCanvas : MonoBehaviour {

    public Button northButton;
    public Button southButton;
    public Button eastButton;
    public Button westButton;

    public Text northText;
    public Text southText;
    public Text eastText;
    public Text westText;

    public Text questionText;


    void Start () {
        //this.gameObject.SetActive(false);
	}

    public void DisplayQuestion(Question quest, List<string> pathDirections)
    {

        questionText.text = quest.questionText;

        //Text[] buttonText = new Text[3];

        if (pathDirections[0] == "North")
        {
            northText.text = quest.answers[0];
        }
        if (pathDirections[0] == "South")
        {
            southText.text = quest.answers[0];
        }
        if (pathDirections[0] == "East")
        {
            eastText.text = quest.answers[0];
        }
        if (pathDirections[0] == "West")
        {
            westText.text = quest.answers[0];
        }
        pathDirections.RemoveAt(0);

        foreach (string direction in pathDirections)
        {
            List<int> selectedInts = new List<int>();

            int selected = Random.Range(1, 3);
            while (selectedInts.Contains(selected))
            {
                selected = Random.Range(1, 3);
            }
            if (direction == "North")
            {
                northText.text = quest.answers[selected];
            }
            if (direction == "South")
            {
                southText.text = quest.answers[selected];
            }
            if (direction == "East")
            {
                eastText.text = quest.answers[selected];
            }
            if (direction == "West")
            {
                westText.text = quest.answers[selected];
            }
            selectedInts.Add(selected);
        }
    }

}
