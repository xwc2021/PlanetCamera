using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessCorridorManager : MonoBehaviour {

    public EndlessCorridorHolder[] prefabList;
    public Scalable player;

    public int createSize=5;
    LinkedList<EndlessCorridorHolder> createlist;
    public EndlessCorridorHolder Head;
    public EndlessCorridorHolder Tail;
    int halfIndex;

    // Use this for initialization
    void Start () {
        initEC();
    }

    EndlessCorridorHolder getRandomEndlessCorridorPrefab()
    {
        int nowIndex = Random.Range(0, prefabList.Length);
        return prefabList[nowIndex];
    }

    public bool doRescale = true;
    
    void initEC()
    {
        createlist = new LinkedList<EndlessCorridorHolder>();

        //建立最中間的
        EndlessCorridorHolder prefab = getRandomEndlessCorridorPrefab();
        EndlessCorridorHolder initObj = (EndlessCorridorHolder)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.Euler(-90, 0, 0));
        halfIndex = createSize / 2;
        initObj.initEC(halfIndex, this, doRescale);
        createlist.AddLast(initObj);

        //add head
        EndlessCorridorHolder refObj = initObj;
        for (int i = halfIndex - 1; i >= 0; i--)
        {
            EndlessCorridorHolder newObj=createEcByRef(i, refObj, refObj.getHeadDummy(), 2.0f);
            createlist.AddFirst(newObj);

            refObj = newObj;
        }
        Head = refObj;

        // add tail
        refObj = initObj;
        for (int i = halfIndex + 1; i < createSize; i++)
        {
            EndlessCorridorHolder newObj = createEcByRef(i, refObj, refObj.getTailDummy(), 0.5f);
            createlist.AddLast(newObj);

            refObj = newObj;
        }
        Tail = refObj;
    }

    bool doWorldReScale = false;
    float scaleValue;
    private void LateUpdate()
    {
        if (doWorldReScale)
        {
            worldReSacle(scaleValue);
            doWorldReScale = false;
        }
    }

    public void CallWorldReSacle(float value)
    {
        //在LateUpdate裡作，大平台才不會不正常的跳動
        doWorldReScale = true;
        scaleValue = value;
    }

    //因為player不可能無限縮小(或是放大)
    //當player離開triggerbox，並且由1縮小到0.5倍之後
    //會把player和所有EndlessCorrior都x2倍
    //這樣player到了下一個EndlessCorrior又可以再次縮小
    //藉此產生無限縮小的假象
    void worldReSacle(float scaleValue)
    {
        Vector3 offset = -scaleValue * player.transform.position;
        foreach (EndlessCorridorHolder element in createlist)
        {
            float localScale = element.transform.localScale.x;
            float temp = localScale * scaleValue;
            Vector3 scale = new Vector3(temp, temp, temp);
            element.transform.localScale = scale;
            element.transform.position = scaleValue * element.transform.position + offset;
        }

        player.resetScale();
        player.resetPos();
    }

    EndlessCorridorHolder createEcByRef(int index, EndlessCorridorHolder refObj,Transform dummy,float ScaleValue)
    {
        float nowScale = refObj.getGlobalScale() * ScaleValue;
        EndlessCorridorHolder nowPrefab = getRandomEndlessCorridorPrefab();
        EndlessCorridorHolder newObj = (EndlessCorridorHolder)GameObject.Instantiate(nowPrefab, dummy.position, dummy.rotation);
        newObj.transform.localScale = new Vector3(nowScale, nowScale, nowScale);
        newObj.initEC(index, this, doRescale);
        return newObj;
    }

    public void updateList(int nowIndex)
    {
        if (nowIndex < halfIndex)
            addHead();
        else if (nowIndex > halfIndex)
            addTail();
    }

    void addHead()
    {
        //修改其他元素序號
        // 0 1 2 3 4 變成
        // 1 2 3 4 5
        foreach (EndlessCorridorHolder element in createlist)
        {
            element.ListIndex = element.ListIndex + 1;
        }

        //刪除Tail元素
        createlist.RemoveLast();
        Destroy(Tail.gameObject);
        Tail = createlist.Last.Value;

        EndlessCorridorHolder newObj = createEcByRef(0, Head,Head.getHeadDummy(), 2.0f);
        createlist.AddFirst(newObj);
        Head = newObj;
    }

    void addTail()
    {
        //修改其他元素序號
        // 0 1 2 3 4 變成
        //-1 0 1 2 3
        foreach (EndlessCorridorHolder element in createlist)
        {
            element.ListIndex = element.ListIndex - 1;
        }

        //刪除Head元素
        createlist.RemoveFirst();
        Destroy(Head.gameObject);
        Head = createlist.First.Value;

        EndlessCorridorHolder newObj = createEcByRef(createSize-1, Tail, Tail.getTailDummy(), 0.5f);
        createlist.AddLast(newObj);
        Tail = newObj;
    }
}
