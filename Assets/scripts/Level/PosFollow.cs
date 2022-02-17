using UnityEngine;

public class PosFollow : MonoBehaviour
{

    public Transform followTarget;
    public float speed;
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, followTarget.position, speed * Time.fixedDeltaTime);
    }
}
