using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    public Animator animator;           // Assign your Animator
    public string introStateName;       // Name of the intro animation state
    public GameObject continueButton;   // Your button
    public GameObject screenObject;     // The cutscene screen to hide

    void Start()
    {
        // Hide the button at start
        continueButton.SetActive(false);
    }

    void Update()
    {
        // Check if the Animator is in the intro state AND finished
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(introStateName) && stateInfo.normalizedTime >= 1f)
        {
            // Show the button
            continueButton.SetActive(true);

            // Disable the cutscene screen
            if (screenObject != null)
            {
                screenObject.SetActive(false);
            }

            // Stop checking
            enabled = false;
        }
    }
}