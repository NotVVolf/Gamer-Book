using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private static SelectionManager instance;

    [SerializeField]
    private string selectableTag = "Selectable";

    [SerializeField]
    private Transform _selection;
    [SerializeField]
    private Animator chestAnimator;

    public bool hasPlayedWithCard = false;

    [SerializeField]
    public bool hasSeenBook = false;

    public bool partyBegins = false;

    public GameObject PartyGuests;

    public GameObject thingWithButton;
    public GameObject musicMananger;
    public GameObject endsong;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        StartParty();
        if (_selection != null)
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
            if (newlySelected.CompareTag(selectableTag))
            {
                var selectionRender = newlySelected.GetComponent<Outline>();
                if (selectionRender != null &&
                    selectionRender.enabled == false)
                {
                    //selectionRender.OutlineColor = new Color(154, 31, 31);
                    //selectionRender.OutlineWidth = 5;
                    selectionRender.enabled = true;


                }
                _selection = newlySelected;
                if (_selection.name == "BackButton")
                {
                    thingWithButton = _selection.gameObject;
                    Debug.Log("Found the button!");
                    var showImage = _selection.GetComponent<Image>();
                    var showButton = _selection.GetComponent<Button>();
                    showImage.enabled = true;
                    showButton.enabled = true;
                }
                if (Input.GetMouseButtonDown(0) && _selection != null)
                {
                    var loadLeveler = _selection.GetComponent<loadlevel>();
                    if (_selection.name == "BookStandinClosedFront")
                    {

                        loadLeveler.Loadbook();

                    }
                    if (_selection.name == "WoodChest")
                    {
                        chestAnimator = _selection.GetComponent<Animator>();

                        loadLeveler.OpenChest();
                    }
                }
            }


        }
        else if (thingWithButton != null && _selection == null)
        {
            var showImage = thingWithButton.GetComponent<Image>();
            var showButton = thingWithButton.GetComponent<Button>();
            showImage.enabled = false;
            showButton.enabled = false;
            Debug.Log("Turning Off Button");
            thingWithButton = null;
        }


    }

    public void StartParty()
    {
        Debug.Log("Checking Party?");
        if (hasPlayedWithCard && hasSeenBook)
        {
            Debug.Log("PartyCanStart");
            if (partyBegins == false)
            {
                Debug.Log("Starting Party!!!");
                var guests = GameObject.Find("PartyGuests");
                guests.transform.localScale = new Vector3(1, 1, 1);

                musicMananger.gameObject.SetActive(false);
                endsong.gameObject.SetActive(true);

            }

        }
    }
}
