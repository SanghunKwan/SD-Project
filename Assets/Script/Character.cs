using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Unit;

//길찾기, 기본 스탠딩 애니메이션 등

public class Character : UnitMove
{
    enum LineAddType
    {
        Auto,
        Shift
    }
    //string[] Hero = new string[8];
    //0 - 이름
    //1 - 직업
    //2 - 레벨
    //3 - 등급
    //4 - 무기
    //5 - 갑옷
    //6 - 부대지정
    //7 - 이동속도


    UILineRenderer lineRenderer;
    UILineRenderer lineRenderer_miniMap;

    public string Formation;
    bool isDeQueue = false;
    bool isDeQueue2 = false;

    class ObjVec
    {
        public int nIndex;
        public CObject target;

        public ObjVec(CObject targetting)
        {
            nIndex = 0;
            target = targetting;
        }
        public void IndexAdd()
        {
            nIndex++;
        }
    }
    List<ObjVec> objVecs = new List<ObjVec>();
    Stack<Vector3> needUpdateRendererPosition;
    protected override void Awake()
    {
        base.Awake();
        unit_State.SelectSet(0);
        needUpdateRendererPosition = new Stack<Vector3>();
    }
    private void OnEnable()
    {
        StartCoroutine(DelayGetLineRenderer());
    }
    protected virtual IEnumerator DelayGetLineRenderer()
    {
        while (ObjectUIPool.isReady.Equals(false))
        {
            yield return null;
        }
        GetLineRenderer();
    }
    void GetLineRenderer()
    {
        GameObject lineObjct1 = ObjectUIPool.pool.Call(ObjectUIPool.Folder.UILineRenderer,
                                                       ObjectUIPool.UICanvasType.GroundCanvas);
        GameObject lineObjct2 = ObjectUIPool.pool.Call(ObjectUIPool.Folder.UILineRendererMiniMap,
                                                       ObjectUIPool.UICanvasType.MinimapCanvas);

        lineRenderer = lineObjct1.GetComponent<UILineRenderer>();
        lineRenderer_miniMap = lineObjct2.GetComponent<UILineRenderer>();
        lineRenderer_miniMap.Points[0] = lineRenderer.Points[0] = NewPoint(transform.position);


        Formation = "Q";
        StartCoroutine(CallLineRendering());
    }
    protected override void Start()
    {
        base.Start();
        //Hero[6] = "1";
        OrdertoReserve[1] += (move, vec) => { AddQueueLine(vec); };

        GameManager.manager.screenMove += ScreenMove;
    }
    IEnumerator CallLineRendering()
    {
        while (true)
        {
            LineRendering();
            TargettingFollowing();
            yield return null;
        }
    }



    void LineRendering()
    {

        lineRenderer_miniMap.Points[lineRenderer_miniMap.Points.Length - 1]
            = lineRenderer.Points[lineRenderer.Points.Length - 1]
            = NewPoint(transform.position);
        if (IsTargetActive)
        {
            lineRenderer_miniMap.enabled = lineRenderer.enabled = true;
            lineRenderer_miniMap.Points[lineRenderer_miniMap.Points.Length - 2]
                = lineRenderer.Points[lineRenderer.Points.Length - 2]
                = NewPoint(targetNode.Value.transform.position);
            lineRenderer_miniMap.color = lineRenderer.color = new Color(1, 0.4705882f, 0.4313726f);
        }
        else if (isFear)
        {
            lineRenderer_miniMap.color = lineRenderer.color = new Color(0.2681481f, 0.1579299f, 0.3679245f);
        }
        else
        {
            lineRenderer_miniMap.color = lineRenderer.color = new Color(0.9411765f, 0.9411765f, 0.8823529f);
        }
        lineRenderer.SetAllDirty();
        lineRenderer_miniMap.SetAllDirty();

    }
    private void TargettingFollowing()
    {
        foreach (var item in objVecs)
        {
            lineRenderer.Points[item.nIndex] = lineRenderer_miniMap.Points[item.nIndex] = NewPoint(item.target.transform.position);
        }
    }

    public override void Navi_Destination(Vector3 vec)
    {
        base.Navi_Destination(vec);
        StartCoroutine(LIneRendererDestinationSet(vec));
    }
    IEnumerator LIneRendererDestinationSet(Vector3 vec)
    {
        yield return new WaitWhile(() => isDeQueue2);
        lineRenderer_miniMap.enabled = lineRenderer.enabled = true;
        lineRenderer_miniMap.Points[lineRenderer_miniMap.Points.Length - 2]
            = lineRenderer.Points[lineRenderer.Points.Length - 2]
            = NewPoint(vec);

        if (GameManager.manager.questManager.isBuildingUnderControl)
        {
            GameManager.manager.questManager.onBuildingControlFinish += DelayNewVectorWhileWindowOpen;
            needUpdateRendererPosition.Push(vec);
        }

        lineRenderer.SetAllDirty();
        lineRenderer_miniMap.SetAllDirty();
    }
    Vector2 NewPoint(Vector3 vec)
    {
        Vector3 vector3 = Camera.main.WorldToScreenPoint(vec);
        Vector2 vector2 = new Vector2();
        vector2.x = vector3.x;
        vector2.y = vector3.y;

        return vector2;
    }
    void DelayNewVectorWhileWindowOpen()
    {
        int length = Mathf.Min(lineRenderer.Points.Length - 1, needUpdateRendererPosition.Count);

        for (int i = 0; i < length; i++)
        {
            lineRenderer.Points[i] = lineRenderer_miniMap.Points[i] = NewPoint(needUpdateRendererPosition.Pop());
        }
        needUpdateRendererPosition.Clear();
        lineRenderer.SetAllDirty();
        lineRenderer_miniMap.SetAllDirty();
    }
    public void ScreenMove(Vector3 vector3)
    {
        Vector2 vec = new Vector2(vector3.x, vector3.y);
        for (int i = 0; i < lineRenderer.Points.Length - 1; i++)
        {
            lineRenderer_miniMap.Points[i] = lineRenderer.Points[i] -= vec;
        }
    }
    protected override void SearchAct2()
    {
        GameManager.manager.SetOtheronBattle(GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.Hero]);
    }
    protected override void CallTargetting(CUnit gameObject)
    {
        //같은 스테이지에 있는 대상 중 가장 가까이 있는 대상 타겟팅.

        //GameManager.manager.NewTargetting(GameManager.manager.dicNpcCharacter, this, (CUnit) => CUnit.detected, cUnit.curstat.ViewRange);
        GameManager.manager.NewTargetting(GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.Monster], this, Mathf.Pow(cUnit.curstat.ViewRange, 2));
    }

    public override void Death(in Vector3 attacker_position, bool isLoaded)
    {
        base.Death(attacker_position, isLoaded);

        lineRenderer_miniMap.enabled = lineRenderer.enabled = false;
        lineRenderer_miniMap.transform.SetParent(ObjectUIPool.pool.transform.GetChild((int)ObjectUIPool.Folder.UILineRendererMiniMap), false);
        lineRenderer.transform.SetParent(ObjectUIPool.pool.transform.GetChild((int)ObjectUIPool.Folder.UILineRenderer), false);
        GameManager.manager.screenMove -= ScreenMove;
    }

    public override void AttackPosition(Vector3 vec)
    {
        base.AttackPosition(vec);
        StartCoroutine(LIneRendererDestinationSet(vec));
    }
    void AddQueueLine(Vector3 vec)
    {
        AddQueueLine(LineAddType.Shift);

        StartCoroutine(AddQueueLineDelay(vec));
    }
    void AddQueueLine(LineAddType type)
    {
        isDeQueue2 = true;
        StartCoroutine(AddQueueLineDelay(type));

    }
    IEnumerator AddQueueLineDelay(LineAddType type)
    {

        yield return new WaitWhile(() => isDeQueue);


        Vector2[] arVec = new Vector2[2 + orderQueue.Count + (int)orderCount];

        //arVec[arVec.Length - 2] = NewPoint(transform.position);

        for (int i = 0; i <= lineRenderer.Points.Length - 2; i++)
        {
            arVec[i + (int)type] = lineRenderer.Points[i];
            //action 없을 때 shift로
            //length - 3부터 추가 arVec.length[1]까지 채움
            //action 있을 때 shift로
            //length - 4부터 추가 arVec.length[1]까지 채움

            //auto는 -3에 추가.
            //action 있을 때 auto로
            //length -4에 추가 arVec.length[0]까지 채움
        }

        lineRenderer.Points = arVec;
        lineRenderer_miniMap.Points = arVec;
        isDeQueue2 = false;

        for (int i = 0; i < objVecs.Count; i++)
        {
            objVecs[i].IndexAdd();
        }
    }
    IEnumerator AddQueueLineDelay(Vector3 vec)
    {
        yield return new WaitWhile(() => isDeQueue2);

        lineRenderer.Points[0] = lineRenderer_miniMap.Points[0] = NewPoint(vec);
        if (GameManager.manager.questManager.isBuildingUnderControl)
            needUpdateRendererPosition.Push(vec);
        lineRenderer.SetAllDirty();
        lineRenderer_miniMap.SetAllDirty();
    }


    protected override void DequeueOrder()
    {
        isDeQueue = true;
        base.DequeueOrder();
        StartCoroutine(DequeueOrderDelay());
    }
    IEnumerator DequeueOrderDelay()
    {
        yield return new WaitWhile(() => isDeQueue2);
        Vector2[] arVec = new Vector2[2 + orderQueue.Count];
        for (int i = 0; i < arVec.Length; i++)
        {
            arVec[i] = lineRenderer.Points[i];
        }
        for (int i = objVecs.Count - 1; i >= 0; i--)
        {
            if (objVecs[i].nIndex >= orderQueue.Count)
                objVecs.RemoveAt(i);
        }
        lineRenderer.Points = arVec;
        lineRenderer_miniMap.Points = arVec;
        lineRenderer.SetAllDirty();
        lineRenderer_miniMap.SetAllDirty();
        isDeQueue = false;
    }
    protected override void QueueClear()
    {
        isDeQueue = true;
        base.QueueClear();
        StartCoroutine(DequeueOrderDelay());
    }
    protected override void AddNextBehaviour()
    {
        AddQueueLine(LineAddType.Auto);

        StartCoroutine(AddNextBehaviourDelay());

    }
    IEnumerator AddNextBehaviourDelay()
    {
        yield return new WaitWhile(() => isDeQueue2);

        if (lineRenderer.Points.Length < 3)
            yield break;

        lineRenderer_miniMap.Points[lineRenderer_miniMap.Points.Length - 3]
            = lineRenderer.Points[lineRenderer.Points.Length - 3]
            = NewPoint(destination);
        lineRenderer.SetAllDirty();
        lineRenderer_miniMap.SetAllDirty();
    }

    protected override void LineClear()
    {
        StartCoroutine(LineClearDelay());
        needUpdateRendererPosition.Clear();
    }
    IEnumerator LineClearDelay()
    {
        yield return new WaitWhile(() => isDeQueue2);

        lineRenderer_miniMap.enabled = lineRenderer.enabled = false;
    }
    protected override void AddQueue(LinkedListNode<CObject> node)
    {

        AddQueueLine(node.Value.transform.position);
        StartCoroutine(AddRefPos(node));

    }
    IEnumerator AddRefPos(LinkedListNode<CObject> node)
    {
        yield return new WaitUntil(() => isDeQueue2);
        objVecs.Add(new ObjVec(node.Value));


    }
}
