using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerOrKeyboard : MonoBehaviour
{
    public GameObject JumpController;
    public GameObject JumpKeyboard;

    public GameObject InteractController;
    public GameObject InteractKeyboard;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ControllerMode()
    {
        //JumpController.SetActive(true);
        //JumpKeyboard.SetActive(false);

        //InteractController.SetActive(true);
        //InteractKeyboard.SetActive(false);
    }

    public void KeyboardAndMouseMode()
    {
        //JumpController.SetActive(false);
        //JumpKeyboard.SetActive(true);

        //InteractController.SetActive(false);
        //InteractKeyboard.SetActive(true);
    }
}
