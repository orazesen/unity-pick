using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    LeftOrRightAIController aiController;

    // Start is called before the first frame update
    void Start()
    {
        aiController = GameObject.FindGameObjectWithTag("GM").GetComponent<LeftOrRightAIController>();
    }

    public void Call()
    {
        aiController.RemoveButton(this.gameObject);
    }
}
