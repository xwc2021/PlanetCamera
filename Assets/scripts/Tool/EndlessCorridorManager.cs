using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessCorridorManager : MonoBehaviour {

    public EndlessCorridorHolder[] prefabList;
    public Transform scaleCenter;

    public int createSize=5;
    List<EndlessCorridorHolder> createlist;
    // Use this for initialization
    void Start () {
        createlist = new List<EndlessCorridorHolder>();

        //create 1st 
        EndlessCorridorHolder prefab = getRandomEndlessCorridorPrefab();
        EndlessCorridorHolder refObj =(EndlessCorridorHolder)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.Euler(-90,0,0));
        createlist.Add(refObj);

        float nowPrefabScale = 0.5f;
        while (createlist.Count < createSize)
        {
            Transform tailDummy =refObj.getTailDummy();
            EndlessCorridorHolder nowPrefab = getRandomEndlessCorridorPrefab();
            EndlessCorridorHolder newObj = (EndlessCorridorHolder)GameObject.Instantiate(nowPrefab, tailDummy.position, tailDummy.rotation);
            newObj.transform.localScale = new Vector3(nowPrefabScale, nowPrefabScale, nowPrefabScale);
            createlist.Add(newObj);

            refObj = newObj;
            nowPrefabScale = nowPrefabScale * 0.5f;
        }
    }

    EndlessCorridorHolder getRandomEndlessCorridorPrefab()
    {
        int nowIndex = Random.Range(0, prefabList.Length);
        return prefabList[nowIndex];
    }


	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateEC()
    {
    }

    //因為player不可能無限縮小(或是放大)
    //當player離開triggerbox，並且由1縮小到0.5倍之後
    //會把player和所有EndlessCorrior都x2倍
    //這樣player到了下一個EndlessCorrior又可以再次縮小
    //藉此產生無限縮小的假象
    public void worldReSacle(float scaleValue)
    {
        print("worldReSacle"+scaleValue);
    }

    void addHead()
    { }

    void addTail()
    { }


}
