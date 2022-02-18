using UnityEngine;
public class ManyBike : MonoBehaviour
{
    public int count;
    public Mesh mesh;
    public Material[] materials;
    public int W = 100;
    public int H = 100;

    public Bike bitePrefab;
    void Awake()
    {
        DrawInstance.getWorker(DrawInstance.KEY_bike).initMatrix(10000, false);
        for (int x = 0; x < W; x++)
        {
            for (int z = 0; z < H; z++)
            {
                var bike = GameObject.Instantiate<Bike>(bitePrefab, this.transform);
                Vector3 newPos = new Vector3(x, 50, z);

                bike.transform.localPosition = newPos;
                bike.name = "bike[" + x + "," + z + "]";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        var worker = DrawInstance.getWorker(DrawInstance.KEY_bike);
        worker.draw(mesh, materials);
        count = worker.getCount();
    }
}