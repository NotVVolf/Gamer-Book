using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadlevel : MonoBehaviour
{

    [SerializeField]
    Animator animator;

    [SerializeField]
    string name = null;

    private void Awake()
    {
        name = gameObject.name;
        animator = GetComponent<Animator>();
    }

    public void OpenChest()
    {
        StartCoroutine(WaitTilChestIsOpen());

    }

    private IEnumerator WaitTilChestIsOpen()
    {
        if (name == "WoodChest")
        {
            if (animator != null)
            {
                animator.SetBool("ChestMayOpen", true);
                yield return new WaitForSeconds(5.0f);
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Animated PBR Chest _Press") && stateInfo.normalizedTime > 1)
                {
                    Loadcards();
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
        else
        {
            yield return null;
        }
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

    public void LoadWhenDoneAnimating()
    {

    }




}
