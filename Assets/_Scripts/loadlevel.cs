using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadlevel : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void Loadcards()
    {
        SceneManager.LoadScene(3);
    }

    public void Loadmain()
    {
        SceneManager.LoadScene(1);
    }

    public void Loadbook()
    {
        SceneManager.LoadScene(2);
    }
    
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }





}
