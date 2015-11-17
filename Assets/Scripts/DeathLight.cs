using UnityEngine;
using System.Collections;

public class DeathLight : MonoBehaviour {

    public float angle = 10f;
    public float width = 1.02f;
    public float range = 60f;
    Vector3[] vertices;
    MeshCollider meshCol;

    Mesh mesh;
    int[,,,] meshPos = new int[2, 2, 2, 3] { { { { -1, -1, -1 }, { -1, -1, -1 } },
                                               { { -1, -1, -1 }, { -1, -1, -1 } } },
                                             { { { -1, -1, -1 }, { -1, -1, -1 } },
                                               { { -1, -1, -1 }, { -1, -1, -1 } } } };

	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            int x = mesh.vertices[i].x < 0f ? 0 : 1;
            int y = mesh.vertices[i].y < 0f ? 0 : 1;
            int z = mesh.vertices[i].z < 0f ? 0 : 1;
            if(meshPos[x, y, z, 0] == -1){
                meshPos[x, y, z, 0] = i;
            }else if (meshPos[x, y, z, 1] == -1){
                meshPos[x, y, z, 1] = i;
            }else{
                meshPos[x, y, z, 2] = i;
            }
        }

        for(int x = 0; x < 2; x++)
        {
            for(int y = 0; y < 2; y++)
            {
                for(int z = 0; z < 2; z++)
                {
                    for(int w = 0; w < 3; w++)
                    {
                        float xPos, yPos, zPos;
                        if(x == 0)
                        {
                            xPos = transform.position.x - width / 2f;
                        }
                        else
                        {
                            xPos = transform.position.x + width / 2f;
                        }
                        if(y == 0)
                        {
                            yPos = transform.position.y;
                        }
                        else
                        {
                            yPos = transform.position.y - range;
                        }
                        if(z == 0)
                        {
                            if(y == 0)
                            {
                                zPos = transform.position.z + 0.4f;
                            }
                            else
                            {
                                zPos = transform.position.z + Mathf.Tan(angle * Mathf.Deg2Rad) * range;
                            }
                        }
                        else
                        {
                            if (y == 0)
                            {
                                zPos = transform.position.z - 0.4f;
                            }
                            else
                            {
                                zPos = transform.position.z - Mathf.Tan(angle * Mathf.Deg2Rad) * range;
                            }   
                        }
                        vertices[meshPos[x, y, z, w]] = transform.InverseTransformPoint(new Vector3(xPos, yPos, zPos));
                    }
                }
            }
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        meshCol = GetComponent<MeshCollider>();
        meshCol.sharedMesh = null;
        meshCol.sharedMesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
        meshCol = GetComponent<MeshCollider>();
        meshCol.sharedMesh = null;
        meshCol.sharedMesh = mesh;
    }
}
