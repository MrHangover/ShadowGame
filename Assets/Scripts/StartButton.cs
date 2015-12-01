using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (Application.levelCount > Application.loadedLevel + 1)
            {
                Application.LoadLevel(Application.loadedLevel + 1);
            }
            else
            {
                Application.LoadLevel(0);
            }
        }
    }
}
