using UnityEngine;

public class BigPlayer : MonoBehaviour, FactoryPlugin
{
    [SerializeField]
    float scale = 1;
    public void doIt(GameObject gameObject)
    {
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}