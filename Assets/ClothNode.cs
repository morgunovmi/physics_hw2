using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothNode : MonoBehaviour
{
    public List<ClothNode> connectedNodes = new List<ClothNode>();
    public bool isDynamic = true;

    public Vector3 acc;
    public Vector3 vel;

    List<GameObject> lineHolders = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < connectedNodes.Count; i++)
        {
            GameObject holder = new GameObject();
            holder.AddComponent<LineRenderer>();
            LineRenderer lineRend = holder.GetComponent<LineRenderer>();
            lineRend.positionCount = 2;
            lineRend.gameObject.SetActive(true);
            lineRend.startWidth = 0.1f;
            lineRend.endWidth = 0.1f;
            lineHolders.Add(holder);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < connectedNodes.Count; i++)
        {
            GameObject holder = lineHolders[i];
            LineRenderer lineRend = holder.GetComponent<LineRenderer>();
            lineRend.SetPosition(0, transform.position);
            lineRend.SetPosition(1, connectedNodes[i].transform.position);
        }
    }
}
