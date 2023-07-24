using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class hoverbook : MonoBehaviour
{
    public GameObject testobject;
    // Start is called before the first frame update
    void Start()
    {
        testobject.GetComponent<Outline>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseOver()
    {

        Debug.Log("hovering");
        // testobject.GetCompnent<Outline>().setActive(false)
        // testobject.GetComponent(Outline)
        testobject.GetComponent<Outline>().enabled = true;

        if (Input.GetMouseButton(0))
        {
          Debug.Log("test");
            SceneManager.LoadScene(2);
        }
    }

    void OnMouseExit()
    {
        testobject.GetComponent<Outline>().enabled = false;
    }
}
