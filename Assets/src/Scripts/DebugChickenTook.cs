using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChickenTook : MonoBehaviour
{
    public Chicken chicken;
    public bool took;

    // Update is called once per frame
    void Update()
    {
        if (this.took != this.chicken.took)
        {
            this.chicken.took = this.took;
        }    
    }
}
