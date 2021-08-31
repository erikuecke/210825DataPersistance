using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    //public static MainManager Instance;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    public string currentPlayer;
    public string highestPlayer;
    public int highestScore;

    // Start is called before the first frame update

    //private void Awake()
    //{
    //    if (Instance != null)
    //    {
    //        MainManager.Instance.Ball = Ball;
    //        MainManager.Instance.m_Started = false;
    //        MainManager.Instance.m_GameOver = false;
    //        MainManager.Instance.SetUpNewGame();
    //        MainManager.Instance.ScoreText = ScoreText;
    //        Destroy(gameObject);
    //        return;
    //    }

    //    Instance = this;
    //    DontDestroyOnLoad(gameObject);
        
    //    Debug.Log("RESTART" + System.DateTime.UtcNow.ToString("HH:mm:ss dd MMMM, yyyy"));
    //}


    void Start()
    {
        LoadBestPlayer();
        GetCurrentPlayerName();
        //ScoreText.text = $"Score : {currentPlayer} : {m_Points}";
        SetUpNewGame();
    }

    private void Update()
    {
        //Debug.Log("Currentl player" + currentPlayer);
        //GetCurrentPlayerName();

        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetUpNewGame();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Debug.Log("RESTART" + System.DateTime.UtcNow.ToString("HH:mm:ss dd MMMM, yyyy"));
                
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {currentPlayer} : {m_Points}";
        CheckHighestScore();
    }

    void CheckHighestScore()
    {
        if (m_Points > highestScore)
        {
            highestPlayer = currentPlayer;
            highestScore = m_Points;
            UpdateBestScore();
            SaveCurrentPlayer();
        }
    }

    void UpdateBestScore()
    { 
        BestScoreText.text = $"Best Score : {highestPlayer} : {highestScore}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    private void GetCurrentPlayerName()
    {
        currentPlayer = CurrentPlayerManager.Instance.currentPlayer;
        ScoreText.text = $"Score : {currentPlayer} : {m_Points}";
    }

    private void SetUpNewGame()
    {
        
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    [System.Serializable]
    class SavePlayerData
    {
        public string player;
        public int highScore;
    }

    public void SaveCurrentPlayer()
    {
        SavePlayerData data = new SavePlayerData();
        data.player = currentPlayer;
        data.highScore = m_Points;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadBestPlayer()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SavePlayerData data = JsonUtility.FromJson<SavePlayerData>(json);
            highestPlayer = data.player;
            highestScore = data.highScore;
            UpdateBestScore();
        }
    }
}
