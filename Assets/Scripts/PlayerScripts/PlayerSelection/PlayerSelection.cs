using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSelection : MonoBehaviour
{
    public static PlayerSelection Instance;

    public Item ObjectInView;
    public List<GameObject> SelectableObjects;

    [SerializeField] private float Threshold = 0.97f;
    [SerializeField] private float MaxDistance = 2f;
    [SerializeField] private LayerMask LayerMask;
   
    [Header("Cache")]
    [SerializeField] private Camera Cam;

    private RaycastHit hit;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InvokeRepeating("GetObjectInView", .1f, .1f);
    }

    private void GetObjectInView()
    {
        var ray = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //gets center of cameras screen

        ObjectInView = null;

        for (int i = 0; i < SelectableObjects.Count; i++)
        {
            if (!SelectableObjects[i]) SelectableObjects.Remove(SelectableObjects[i]);
        }

        if (!Physics.Raycast(ray, out hit, MaxDistance, LayerMask)) return;

        if(!SelectableObjects.Contains(hit.transform.gameObject))
        {
            SelectableObjects.Add(hit.transform.gameObject);
        }

        var closest = 0f;

        for (int i = 0; i < SelectableObjects.Count; i++)
        {
            var vector1 = ray.direction;
            var vector2 = SelectableObjects[i].transform.position - ray.origin;

            var lookPercent = Vector3.Dot(vector1.normalized, vector2.normalized);

            if(lookPercent > Threshold && lookPercent > closest)
            {
                closest = lookPercent;
                ObjectInView = SelectableObjects[i].GetComponent<Item>();
            }
        }
    }

    private void ApplyOutline()
    {
        if (!ObjectInView) return;

        var objRend = ObjectInView.GetComponentInChildren<MeshRenderer>();
        var oldShader = objRend.material.shader;
    }

    private void OnDrawGizmos()
    {
        var ray = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawRay(ray.origin, ray.direction * MaxDistance);
    }

    public static bool IsPointerOverUiObject()
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}
