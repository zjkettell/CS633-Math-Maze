using System.Collections.Generic;
using UnityEngine;

public class CSVParser : MonoBehaviour
{

    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter

    public Queue<string> dataQueue = new Queue<string>();

    public List<Question> readQuestions(TextAsset questionCSV)
    {
        List<Question> questions = new List<Question>();
        
        string[] lines = questionCSV.text.Split(lineSeperater);

        foreach (string line in lines)
        {
            dataQueue.Clear();

            Question question = new Question();

            string[] fields = line.Split(fieldSeperator);
            foreach (string field in fields)
            {
                dataQueue.Enqueue(field);
            }
            question.questionText = dataQueue.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                if (dataQueue.Count > 0)
                {
                    question.answers[i] = dataQueue.Dequeue();
                }
            }

            questions.Add(question);
        }
        return questions;
    }
}
