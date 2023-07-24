using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hover : MonoBehaviour
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


    }

      void OnMouseExit()
    {
       testobject.GetComponent<Outline>().enabled = false;
    }
}
