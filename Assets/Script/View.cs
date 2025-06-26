using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMove))]
public class View : MonoBehaviour
{
    [SerializeField] float meshResolution;
    public float ViewAngle { private get; set; }
    public int ViewRange { private get; set; }
    List<Vector3> meshvector = new List<Vector3>();
    [SerializeField] protected MeshFilter filter;
    [SerializeField] int Detailtime = 10;
    [SerializeField] protected GameObject Head;
    RayInfo rayinfo = new();
    RayInfo rayinfo2 = new();
    UnitMove thismove;
    int layerMask;

    [SerializeField] Material defaultMaterial;
    protected virtual void OnEnable()
    {
        GameObject filterObject = ViewRendererPool.pool.Call();
        filterObject.transform.SetParent(filterObject.transform.parent.parent);
        filterObject.SetActive(true);
        filter = filterObject.GetComponent<MeshFilter>();
        filter.mesh = new Mesh();
        filter.mesh.name = "ViewAngleMesh";

    }
    private void Start()
    {
        layerMask = (1 << 7) | (1 << 9) | (1 << 10);
        thismove = GetComponent<UnitMove>();
    }
    private void LateUpdate()
    {
        ViewPoint();
        ViewRender();
    }

    struct RayInfo
    {
        public bool collide;
        public Vector3 colpoint;
        public Transform trasposi;
        public float angle;

        public RayInfo(float _angle, RaycastHit _hit)
        {
            angle = _angle;
            collide = true;
            colpoint = _hit.point;
            trasposi = _hit.transform;

        }
        public RayInfo(float _angle, Vector3 vector3)
        {
            angle = _angle;
            collide = false;
            colpoint = vector3;
            trasposi = default;
        }
    }

    void MoreRayfor(RayInfo _minRay, RayInfo _maxRay, int time)
    {
        RayInfo minRay = _minRay;
        RayInfo maxRay = _maxRay;

        RayInfo newrayinfo;

        for (int i = 0; i < time; i++)
        {
            float Newangle = (maxRay.angle + minRay.angle) * 0.5f;

            Vector3 vector3 = transform.position + AngletoPoint(Newangle) * ViewRange;
            if (Physics.Raycast(transform.position, AngletoPoint(Newangle), out RaycastHit Newhit, ViewRange, layerMask))
            {
                newrayinfo = new RayInfo(Newangle, Newhit);
                if (newrayinfo.trasposi.Equals(minRay.trasposi))
                {
                    minRay = newrayinfo;
                }
                else if (newrayinfo.trasposi.Equals(maxRay.trasposi))
                {
                    maxRay = newrayinfo;
                }
                else
                {
                    MoreRayfor(newrayinfo, maxRay, time - i);
                    MoreRayfor(minRay, newrayinfo, time - i);
                    return;
                }
            }
            else
            {
                newrayinfo = new RayInfo(Newangle, vector3);

                if (maxRay.collide && minRay.collide)
                {
                    MoreRayfor(newrayinfo, maxRay, time - i);
                    MoreRayfor(minRay, newrayinfo, time - i);
                    return;
                }
                else if (maxRay.collide)
                {
                    minRay = newrayinfo;
                }
                else if (minRay.collide)
                {
                    maxRay = newrayinfo;
                }
            }
        }
        meshvector.Add(minRay.colpoint);
        meshvector.Add(maxRay.colpoint);

    }
    protected virtual float HeadTurn()
    {
        return Head.transform.eulerAngles.y + 90;
    }

    Vector3 AngletoPoint(float Angle)
    {
        return new Vector3(Mathf.Sin(Angle * Mathf.Deg2Rad), 0, Mathf.Cos(Angle * Mathf.Deg2Rad));

    }

    void ViewPoint()
    {
        //RayInfo rayinfo = new();
        int stepCount = Mathf.RoundToInt(meshResolution * ViewAngle);
        float stepAngleSize = ViewAngle / stepCount;
        meshvector.Clear();

        for (float i = -stepCount / 2; i <= stepCount / 2; i++)
        {
            float angle = stepAngleSize * i + HeadTurn();


            if (Physics.Raycast(transform.position, AngletoPoint(angle), out RaycastHit hit, ViewRange, layerMask))
            {

                rayinfo2 = new RayInfo(angle, hit);
                if (i > -stepCount / 2 + 0.5f && !rayinfo2.trasposi.Equals(rayinfo.trasposi))
                {
                    MoreRayfor(rayinfo, rayinfo2, Detailtime);
                }
                rayinfo = rayinfo2;
                meshvector.Add(hit.point);
                GameObject target = hit.collider.gameObject;

                if (gameObject.CompareTag(target.tag)) continue;

                if (target.CompareTag("Player"))
                    GameManager.manager.Search(ObjectManager.CObjectType.Hero, target, thismove);
                else if (target.CompareTag("Enermy"))
                    GameManager.manager.Search(ObjectManager.CObjectType.Monster, target, thismove);

            }
            else
            {
                Vector3 vector3 = transform.position + AngletoPoint(angle) * ViewRange;
                RayInfo tempRayInfo = new(angle, vector3);
                if (i > -stepCount / 2 + 0.5f && rayinfo.collide)
                {
                    MoreRayfor(rayinfo, tempRayInfo, Detailtime);
                }
                rayinfo = tempRayInfo;
                meshvector.Add(vector3);

            }
        }
    }
    void ViewRender()
    {
        if (filter == default)
            return;
        filter.mesh.Clear();
        Vector3[] vertices = new Vector3[meshvector.Count];
        int[] triangles = new int[(meshvector.Count - 2) * 3];

        vertices[0] = transform.position;
        vertices[1] = meshvector[0];

        for (int i = 0; i < (meshvector.Count - 2); i++)
        {
            vertices[i + 2] = meshvector[i + 1];
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;

        }
        filter.mesh.vertices = vertices;
        filter.mesh.triangles = triangles;
    }

    void OnDisable()
    {
        if (!Application.isPlaying || filter == default)
            return;

        filter.GetComponent<MeshRenderer>().material = defaultMaterial;
        filter.gameObject.layer = 6;
        filter.transform.SetParent(ViewRendererPool.pool.transform.GetChild(0));
        filter.gameObject.SetActive(false);
        filter = null;
    }
}
