using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agacharse : MonoBehaviour
{
    Animator anim;
    public bool agacharse;
    readonly int p_Agacharse = Animator.StringToHash("agacharse");

    // Start is called before the first frame update
    void Start()
    {
        anim= GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            agacharse = !agacharse;
        }
        anim.SetBool(p_Agacharse, agacharse);
    }
}
