using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPositionDiff : MonoBehaviour {

    [SerializeField]
    RecordPositionDiff helper;
    Vector3 oldPos;
    Vector3 diff;
    private void Awake()
    {
        oldPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate () {

        Vector3 newPos = transform.position;
        diff = newPos - oldPos;
        oldPos = newPos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!TagDefined.canOnMovableSet(other.gameObject.tag))
            return;

        PlanetPlayerController controller = other.gameObject.GetComponent<PlanetPlayerController>();
        if (controller != null)
            controller.setPlatform(this);
    }

    void OnTriggerExit(Collider other)
    {
        if (!TagDefined.canOnMovableSet(other.gameObject.tag))
            return;

        PlanetPlayerController controller = other.gameObject.GetComponent<PlanetPlayerController>();
        if (controller != null)
            controller.clearPlatform();
    }

    public Vector3 getDiff()
    {
        return diff;
    }

    public Vector3 getHelperDiff()
    {
        Debug.Assert(helper != null);
        return helper.getDiff();
    }
}
