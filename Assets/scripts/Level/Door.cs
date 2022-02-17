using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{

    public string scene;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != TagDefined.Player)
            return;

        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
