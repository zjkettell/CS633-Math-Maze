using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public Maze mazePrefab;
    public Player playerPrefab;
    public MazePathfinder pathfinderPrefab;

    public CSVParser parse;

	private Maze mazeInstance;
    private Player playerInstance;
    private MazePathfinder pathfinderInstance;

    public List<Question>  questionList = new List<Question>();

    public TextAsset questionCSV;

    public static int difficulty; //1 = easy, 2 = medium, 3 = hard

    public QuestionCanvas questionCanvas;
    private MazeCell questionCell;

    private void Start ()
    {
        questionList = parse.readQuestions(questionCSV);
        
        foreach (Question quest in questionList)
        {
            if (quest.questionText == null)
                questionList.Remove(quest);
        }
	}

    void Awake()
    {
        //Debug.Log("Awake");
        DontDestroyOnLoad(transform.gameObject);
    }

    void OnEnable()
    {
        //Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OnSceneLoaded: " + scene.name);
        //Debug.Log(mode);

        if (scene.name == "MazeScene")
            BeginGame();
    }

    private void Update () {
    }
    
    public void LoadEasy()
    {
        difficulty = 1;
        SceneManager.LoadScene("MazeScene", LoadSceneMode.Single);
        BeginGame();
    }
    public void LoadMedium()
    {
        difficulty = 2;
        SceneManager.LoadScene("MazeScene", LoadSceneMode.Single);
        BeginGame();
    }
    public void LoadHard()
    {
        difficulty = 3;
        SceneManager.LoadScene("MazeScene", LoadSceneMode.Single);
        BeginGame();
    }
    public void LoadEnd()
    {
        StopAllCoroutines();
        Destroy(mazeInstance.gameObject);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    private void BeginGame () {

        pathfinderInstance = Instantiate(pathfinderPrefab) as MazePathfinder;
		mazeInstance = Instantiate(mazePrefab) as Maze;
        if (difficulty == 2)
        {
            mazeInstance.size.x = 30;
            mazeInstance.size.y = 30;
        } else if (difficulty == 3)
        {
            mazeInstance.size.x = 45;
            mazeInstance.size.y = 45;
        } else
        {
            mazeInstance.size.x = 20;
            mazeInstance.size.y = 20;
        }
        mazeInstance.Generate();
        playerInstance = Instantiate(playerPrefab, new Vector3(mazeInstance.mazeStart.transform.position.x, mazeInstance.mazeStart.transform.position.y +.1f, mazeInstance.mazeStart.transform.position.z), Quaternion.identity) as Player;
	}

	private void RestartGame () {
		StopAllCoroutines();
		Destroy(mazeInstance.gameObject);
		BeginGame();
	}

    void OnDisable()
    {
        //Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void DisplayQuestion(MazeCell _questionCell)
    {
        questionCell = _questionCell;
        List<string> directions = new List<string>();
        string correctDirection = pathfinderInstance.FindCorrectDirection(_questionCell, mazeInstance.mazeEnd);
        int questionNum = Random.Range(0, questionList.Count);
        while (questionList[questionNum].questionText==null)
            questionNum = Random.Range(0, questionList.Count);
        playerInstance.gameObject.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>().enabled = false;
        playerInstance.gameObject.GetComponent<Rigidbody>().Sleep();
        playerInstance.gameObject.transform.position = _questionCell.transform.position;

        print(correctDirection);

        MazePassage[] passages = _questionCell.GetComponentsInChildren<MazePassage>();

        foreach (MazePassage passage in passages)
        {
            if (passage.direction.ToString().Equals(correctDirection))
            {
                directions.Insert(0, passage.direction.ToString());
            }
            else
            {
                directions.Add(passage.direction.ToString());
            }
        }


        Cursor.visible = true;
        questionCanvas.gameObject.SetActive(true);
        questionCanvas.DisplayQuestion(questionList[questionNum], directions);
    }
    public void QuestionButton (string direction)
    {
        print(direction);
        foreach (MazePassage passage in questionCell.GetComponentsInChildren<MazePassage>())
        {
            print(passage.direction.ToString());
            if (passage.direction.ToString().Equals(direction))
            {
                playerInstance.gameObject.transform.position = passage.otherCell.transform.position;
            }
        }
        playerInstance.gameObject.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>().enabled = true;
        Cursor.visible = false;
        questionCanvas.gameObject.SetActive(false);
    }
}