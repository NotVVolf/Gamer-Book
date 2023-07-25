using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class visitedLevel : MonoBehaviour
{
    GameObject selectionManager;

    SelectionManager selectionManagerInScene;
    // Start is called before the first frame update
    void Awake()
    {
        var selectionManager = GameObject.Find("SelectionManager");
        selectionManagerInScene = selectionManager.GetComponent<SelectionManager>();
    }

    public void visitedCard()
    {
        if (selectionManagerInScene != null)
        selectionManagerInScene.hasPlayedWithCard = true;
    }

    public void visitedBook()
    {
        if (selectionManagerInScene != null)
            selectionManagerInScene.hasSeenBook = true;
    }
}
