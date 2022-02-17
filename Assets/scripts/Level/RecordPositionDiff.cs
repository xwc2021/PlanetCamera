using UnityEngine;

public class RecordPositionDiff : MonoBehaviour
{
    Vector3 oldPos;
    Vector3 diff;
    private void Awake()
    {
        oldPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 newPos = transform.position;
        diff = newPos - oldPos;
        oldPos = newPos;
    }

    void OnTriggerStay(Collider other)
    {
        if (!TagDefined.canOnMovableSet(other.gameObject.tag))
            return;
    }

    void OnTriggerExit(Collider other)
    {
        if (!TagDefined.canOnMovableSet(other.gameObject.tag))
            return;

    }

    public Vector3 getDiff()
    {
        return diff;
    }

    public Vector3 getHelperDiff()
    {
        return getDiff();
    }
}