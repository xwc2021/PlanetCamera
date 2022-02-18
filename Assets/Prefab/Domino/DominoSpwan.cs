using UnityEngine;
public class DominoSpwan : MonoBehaviour
{
    public Transform spawn_obj;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Quaternion rotDiff = Quaternion.AngleAxis(i, transform.forward);
            Instantiate(spawn_obj, transform.position, rotDiff * transform.rotation);
        }
    }

    public int count = 360;
    // Update is called once per frame
}