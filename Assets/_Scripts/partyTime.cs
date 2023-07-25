using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class partyTime : MonoBehaviour
{
    [SerializeField] bool vistedBook;
    [SerializeField] bool vistedCard;
    [SerializeField]
    string sceneName;
    [SerializeField] Scene currentScene;
    [SerializeField] GameObject guests;
    public GameObject dontdestory;

    // Start is called before the first frame update
    void Start()
    {

    }
    void Awake()
    {
        DontDestroyOnLoad(dontdestory);
    }

    // Update is called once per frame
    void Update()
    {
        currentScene = SceneManager.GetActiveScene();

        sceneName = currentScene.name;

        cardWorld();
        timeToParty();
    }
    void cardWorld()
    {
        if (sceneName == "cardScene")
        {
            Debug.Log("in card scene");
            vistedCard = true;
        }
    }

    void BookWorld()
    {
        if (SceneManager.sceneCount == 2)
        {
            Debug.Log("in book scene");
            vistedBook = true;
        }
    }

    void timeToParty()
    {
        if (vistedBook == true && vistedCard == true)
        {
            Debug.Log("Party time");
            guests.gameObject.SetActive(true);
        }
    }

}
