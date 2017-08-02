using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour {

    public string scene;
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
