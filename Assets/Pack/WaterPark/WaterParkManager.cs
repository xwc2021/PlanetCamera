using UnityEngine;

public class WaterParkManager : MonoBehaviour
{

    [SerializeField]
    WaterHeightSetter groundPrefab;

    [SerializeField]
    int W;

    [SerializeField]
    int H;

    [SerializeField]
    int showW;

    [SerializeField]
    int showH;

    [SerializeField]
    float waterFollowPerSecond;

    WaterHeightSetter[,] waterHeightSetter;

    float[,] nowWater, newWater;
    float[,] waterHeightField;
    float[,] waterHeightField2;
    int[,] terrainHeightField;

    int flow_index_x;
    int flow_index_z;
    public void resetWaterFollowPerSecond(float value)
    {
        flow_index_x = Random.Range(0, showW);
        flow_index_z = Random.Range(0, showH);
        waterFollowPerSecond = value;
    }

    float getTotalHeight(int x, int z, float refHeight)
    {
        if (x < 0 || x >= showW || z < 0 || z >= showH)
            return refHeight - 0.25f;//這樣才不會積水

        float waterH = nowWater[x, z];
        float totalH = waterH + terrainHeightField[x, z];
        if (waterH == 0 && (totalH >= refHeight))//沒水
            return refHeight;
        else
            return totalH;
    }

    float diffuse(int x, int z)
    {
        float waterHeight = nowWater[x, z];
        float terrainHeight = terrainHeightField[x, z];
        float nowHeight = waterHeight + terrainHeight;

        float left = getTotalHeight(x - 1, z, nowHeight);
        float right = getTotalHeight(x + 1, z, nowHeight);
        float down = getTotalHeight(x, z - 1, nowHeight);
        float up = getTotalHeight(x, z + 1, nowHeight);

        float right_down = getTotalHeight(x + 1, z - 1, nowHeight);
        float right_up = getTotalHeight(x + 1, z + 1, nowHeight);
        float left_down = getTotalHeight(x - 1, z - 1, nowHeight);
        float left_up = getTotalHeight(x - 1, z + 1, nowHeight);

        float average = 0;

        average = (nowHeight + left + right + down + up + right_down + right_up + left_down + left_up) / 9;
        newWater[x, z] = Mathf.Max(average - terrainHeight, 0);//不可能<0
        return newWater[x, z];
    }

    void swapBuffer()
    {
        float[,] temp = nowWater;
        nowWater = newWater;
        newWater = temp;
    }

    float getWave1(int x, int z)
    {
        float t = (float)x / W;
        float h = 6 * Mathf.Sin(4 * Mathf.PI * t);
        return h;
    }

    float getWave2(int x, int z)
    {
        float t = (float)(x + z) / L;
        float h = 1.5f * Mathf.Sin(10 * Mathf.PI * t);
        return h;
    }

    float getHill(int x, int z, int hX, int hZ, int height)
    {
        int far = Mathf.Abs(hX - x) + Mathf.Abs(hZ - z);
        float h = Mathf.Max(0, height - far);
        return h;
    }

    float L;
    void Awake()
    {
        DrawInstance.getWorker(DrawInstance.KEY_water_park_block).initMatrix(showW * showH, true);
        waterHeightSetter = new WaterHeightSetter[showW, showH];
        terrainHeightField = new int[showW, showH];
        waterHeightField = new float[showW, showH];
        waterHeightField2 = new float[showW, showH];

        nowWater = waterHeightField;
        newWater = waterHeightField2;

        L = Mathf.Sqrt(W * W + H * H);
        for (int x = 0; x < showW; x++)
        {
            for (int z = 0; z < showH; z++)
            {

                WaterHeightSetter newGround = GameObject.Instantiate<WaterHeightSetter>(groundPrefab, this.transform);
                waterHeightSetter[x, z] = newGround;

                float h = 0;
                h += getWave1(x, z);
                h += getWave2(x, z);

                h += -getHill(x, z, 25, 25, 5);
                h += -getHill(x, z, 20, 25, 5);
                h += -getHill(x, z, 15, 22, 7);
                h += -getHill(x, z, 10, 17, 10);

                h += getHill(x, z, 35, 20, 10);
                h += getHill(x, z, 35, 25, 10);
                h += getHill(x, z, 35, 35, 10);
                terrainHeightField[x, z] = (int)h;
                Vector3 newPos = new Vector3(x, (int)h, z);

                newGround.transform.localPosition = newPos;
                newGround.name = "obj[" + x + "," + z + "]";

                DrawInstance.getWorker(DrawInstance.KEY_water_park_block).pushTrasform(newGround.transform);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        // todo: 之後可以改到shader做
        for (int x = 0; x < showW; x++)
        {
            for (int z = 0; z < showH; z++)
            {
                float h = diffuse(x, z);
                waterHeightSetter[x, z].updateWater(h);
            }
        }

        swapBuffer();

        nowWater[flow_index_x, flow_index_z] += waterFollowPerSecond * Time.deltaTime;

        DrawInstance.getWorker(DrawInstance.KEY_water_park_block).draw(mesh, materials);
    }

    public Mesh mesh;
    public Material[] materials;
}