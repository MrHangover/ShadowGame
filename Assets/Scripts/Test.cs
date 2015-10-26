using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {

	Renderer localRenderer;
	Light[] lights;
	List<GameObject> shadowObjectsCasterSide;
	List<GameObject> shadowObjectsReceiverSide;
	public GameObject shadowObject;
    public GameObject shadowFake;
    public Material shadowMaterial;

	// Use this for initialization
	void Start () {
		localRenderer = GetComponent<Renderer>();
		lights = FindObjectsOfType<Light>() as Light[];
		shadowObjectsCasterSide = new List<GameObject>();
		shadowObjectsReceiverSide = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if(localRenderer.isVisible){    //Should only run if the object can be seen by the camera, doesn't seem to work though.
			Mesh mesh = GetComponent<MeshFilter>().mesh;
            //Initialize the vertex arrays. The caster vertices need to be twice length, since we want the platform to go through the wall (one on each side.)
            Vector3[] casterVertices = new Vector3[mesh.vertices.Length * 2];
            Vector3[] recieverVertices = mesh.vertices;

            //Count the number of lights in the scene that affect shadows.
			int numOfLights = 0;
			for(int m = 0; m < lights.Length; m++){
				if(lights[m].tag == "ShadowCast"){
					numOfLights++;
				}
			}

            //Instantiate 2 shadow objects for each light (one on each side).
			while(shadowObjectsCasterSide.Count < numOfLights){
				shadowObjectsCasterSide.Add(Instantiate(shadowObject, transform.position, transform.rotation) as GameObject);
				shadowObjectsReceiverSide.Add(Instantiate(shadowFake, transform.position, transform.rotation) as GameObject);
			}

            //Failsafe, in case lights are removed during runtime.
			while(shadowObjectsCasterSide.Count > numOfLights){
				shadowObjectsCasterSide.RemoveAt(shadowObjectsCasterSide.Count);
				shadowObjectsReceiverSide.RemoveAt(shadowObjectsReceiverSide.Count);
			}

            //For every light, calculate the shadows cast on the wall.
			int shadowIndex = -1;
			for(int j = 0; j < lights.Length; j++){
				if(lights[j].enabled && lights[j].tag == "ShadowCast"){
					for(int i = 0; i < casterVertices.Length / 2; i++){
						RaycastHit hit;
						Ray ray = new Ray(transform.position + mesh.vertices[i], transform.position + mesh.vertices[i] - lights[j].transform.position);
						//Debug.DrawRay(transform.position + mesh.vertices[i], transform.position + mesh.vertices[i] - Camera.main.transform.position, Color.red);
						if(Physics.Raycast(ray, out hit)){
							casterVertices[i] = transform.InverseTransformPoint(new Vector3(-21.01f, hit.point.y, hit.point.z));
                            recieverVertices[i] = casterVertices[i] + Vector3.right * 1.02f;
                            casterVertices[casterVertices.Length/2 + i] = recieverVertices[i];
						}
					}
					shadowIndex++;

                    //Assign meshes and colliders
					Mesh shadowMesh = shadowObjectsCasterSide[shadowIndex].GetComponent<MeshFilter>().mesh;
					shadowMesh.vertices = casterVertices;
					shadowMesh.RecalculateBounds();
					MeshCollider meshCol = shadowObjectsCasterSide[shadowIndex].GetComponent<MeshCollider>();
					meshCol.sharedMesh = null;
					meshCol.sharedMesh = shadowMesh;

					shadowMesh = shadowObjectsReceiverSide[shadowIndex].GetComponent<MeshFilter>().mesh;
					shadowMesh.vertices = recieverVertices;
					shadowMesh.RecalculateBounds();
				}
			}
		}
	}
}
