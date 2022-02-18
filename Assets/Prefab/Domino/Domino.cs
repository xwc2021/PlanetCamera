using UnityEngine;
public class Domino : MonoBehaviour
{
    public float gravityScale = 0.05f;
    Vector3 gravityCenter;
    Rigidbody rigid;

    public bool useGravity = false;
    // Use this for initialization
    void Start()
    {
        gravityCenter = transform.parent.position;
        transform.parent = null;
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (useGravity)
        {
            Vector3 Force = gravityCenter - transform.position;
            rigid.AddForce(Force * gravityScale);
        }
    }
}