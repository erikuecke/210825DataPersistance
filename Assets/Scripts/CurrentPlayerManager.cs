using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentPlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static CurrentPlayerManager Instance;
    public string currentPlayer;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
       

    }
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
