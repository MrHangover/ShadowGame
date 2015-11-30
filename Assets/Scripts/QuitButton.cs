using UnityEngine;
using System.Collections;

public class QuitButton : MonoBehaviour {

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Application.Quit();
        }
    }
}
