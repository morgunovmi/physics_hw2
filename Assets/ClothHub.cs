using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ClothHub : MonoBehaviour
{
    // Start is called before the first frame update
    private List<ClothNode> clothNodes;
    public float k = 30;
    public float d = 0.9999f;
    public float restDist = 5;
    public float nodeMass = 1;
    public float nodeElasticity = 0.8f;
    public Vector3 gravity = new Vector3(0, -9.8f, 0);

    List<GameObject> walls = new List<GameObject>();
    void Start()
    {
        clothNodes = FindObjectsOfType<ClothNode>().ToList();
        Time.fixedDeltaTime = 1.0f / 500;

        walls = GameObject.FindGameObjectsWithTag("Wall").ToList();
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        for (int i = 0; i < clothNodes.Count; i++)
        {
            // Springs
            ClothNode main = clothNodes[i];
            if (main.isDynamic)
            {
                main.vel = main.vel +  1 / 2.0f * main.acc * dt;
                main.transform.position += main.vel * dt;
                Vector3 force = nodeMass * gravity;
                for (int j = 0; j < main.connectedNodes.Count; j++)
                {
                    ClothNode side = main.connectedNodes[j];
                    force += -k * (Vector3.Distance(main.transform.position, side.transform.position) - restDist)
                        * (main.transform.position - side.transform.position).normalized;
                }
                main.acc = force / nodeMass;
                main.vel = main.vel + 1 / 2.0f * main.acc * dt;
                main.vel *= d;
            }

            // Node collision
            for (int j = 0; j != i && j < clothNodes.Count; j++)
            {
                if (Vector3.Distance(clothNodes[i].transform.position, clothNodes[j].transform.position) <= 1.0f)
                {
                    ClothNode a = clothNodes[i];
                    ClothNode b = clothNodes[j];

                    // https://eugkenny.github.io/GADV8001/lectures/Collision%20Response%20-%20Impulse%20Methods.pdf
                    Vector3 Vab = a.vel - b.vel;
                    Vector3 n = (b.transform.position - a.transform.position).normalized;
                    float J = -(1 + nodeElasticity) * Vector3.Dot(Vab, n) / (Vector3.Dot(n, n) * (1 / nodeMass + 1 / nodeMass));

                    a.vel = a.vel + J / nodeMass * n;
                    b.vel = b.vel - J / nodeMass * n;
                }
            }

            // Wall collision
            for (int j = 0; j < walls.Count; ++j)
            {
                GameObject wall = walls[j];
                if (Vector3.Dot(main.transform.position - wall.transform.position, wall.transform.up.normalized) <= 0.5f)
                {
                    Vector3 Vab = main.vel;
                    Vector3 n = wall.transform.up;
                    main.vel -= 2 * n * Vector3.Dot(n, main.vel);
                    main.transform.position += main.vel * 0.01f;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
