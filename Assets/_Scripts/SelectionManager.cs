using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField]
    private string selectableTag = "Selectable";

    [SerializeField]
    private Transform _selection;
    [SerializeField]
    private Animator chestAnimator;
    // Update is called once per frame
    void Update()
    {
        
        if (_selection != null || Input.GetMouseButtonDown(0))
        {
            var selectionRender = _selection.GetComponent<Outline>();
            selectionRender.enabled = false;
            _selection = null;
        }
        
       
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var newlySelected = hit.transform;
            if(newlySelected.CompareTag(selectableTag))
            {
                var selectionRender = newlySelected.GetComponent<Outline>();
                if (selectionRender != null &&
                    selectionRender.enabled == false)
                {
                    selectionRender.OutlineColor = new Color(154, 31, 31);
                    selectionRender.OutlineWidth = 5;
                    selectionRender.enabled = true;


                }
                _selection = newlySelected;
                if (Input.GetMouseButtonDown(0) && _selection != null)
                {
                    var loadLeveler = _selection.GetComponent<loadlevel>();
                    if(_selection.name == "BookStandinClosedFront")
                    {
                        loadLeveler.Loadbook();

                    }
                    if(_selection.name == "WoodChest")
                    {
                        chestAnimator = _selection.GetComponent<Animator>();
                        //loadLeveler.Loadcards();
                        loadLeveler.OpenChest();
                    }

                }
            }

        }

    }
}
