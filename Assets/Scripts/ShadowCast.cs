using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowCast : MonoBehaviour {

	Renderer localRenderer;
	Light[] lights;
	List<GameObject> shadowObjectsCasterSide;
	List<GameObject> shadowObjectsReceiverSide;
    Mesh mesh;

	GameObject shadowObject;
    GameObject shadowFake;
    LayerMask collisionLayer;

    public bool isEnemy = false;
	public LayerMask wallLayer;
    public Material shadowMaterial;
    public bool startButton = false;
    public bool quitButton = false;

	// Use this for initialization
	void Start () {
        collisionLayer =  LayerMask.NameToLayer("Shadows");
        localRenderer = GetComponent<Renderer>();
		lights = FindObjectsOfType<Light>() as Light[];
		shadowObjectsCasterSide = new List<GameObject>();
		shadowObjectsReceiverSide = new List<GameObject>();
        mesh = GetComponent<MeshFilter>().mesh;
        if (shadowMaterial == null)
        {
            shadowMaterial = GetComponent<MeshRenderer>().material;
        }

        //Initializing GameObjects
        shadowObject = new GameObject("shadowObject");
        if (isEnemy)
            shadowObject.tag = "Enemy";
        shadowObject.layer = collisionLayer.value;
        MeshFilter objMeshFilter = shadowObject.AddComponent<MeshFilter>();
        MeshRenderer objMeshRend = shadowObject.AddComponent<MeshRenderer>();
        MeshCollider objMeshCol = shadowObject.AddComponent<MeshCollider>();

        objMeshFilter.mesh = mesh;
        objMeshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        objMeshRend.receiveShadows = false;
        objMeshRend.material = shadowMaterial;
        objMeshCol.convex = true;
        objMeshCol.isTrigger = false;
        objMeshCol.sharedMesh = null;
        objMeshCol.sharedMesh = objMeshFilter.mesh;

        shadowObject.SetActive(false);

        shadowFake = new GameObject("shadowFake");
        MeshFilter fakeMeshFilter = shadowFake.AddComponent<MeshFilter>();
        MeshRenderer fakeMeshRend = shadowFake.AddComponent<MeshRenderer>();

        fakeMeshFilter.mesh = mesh;
        fakeMeshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        fakeMeshRend.receiveShadows = false;
        fakeMeshRend.material = shadowMaterial;

        shadowFake.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		if(localRenderer.isVisible){    //Should only run if the object can be seen by the camera, doesn't seem to work though.
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
                shadowObjectsCasterSide[shadowObjectsCasterSide.Count - 1].SetActive(true);
                if (startButton) shadowObjectsCasterSide[shadowObjectsCasterSide.Count - 1].AddComponent<StartButton>();
                if (quitButton) shadowObjectsCasterSide[shadowObjectsCasterSide.Count - 1].AddComponent<QuitButton>();
                shadowObjectsReceiverSide.Add(Instantiate(shadowFake, transform.position, transform.rotation) as GameObject);
                shadowObjectsReceiverSide[shadowObjectsReceiverSide.Count - 1].SetActive(true);
            }

            for (int i = 0; i < shadowObjectsCasterSide.Count; i++){
                shadowObjectsCasterSide[i].transform.position = transform.position;
                //shadowObjectsCasterSide[i].transform.localScale = transform.localScale;
                //shadowObjectsCasterSide[i].transform.rotation = transform.rotation;
                shadowObjectsReceiverSide[i].transform.position = transform.position;
                //shadowObjectsReceiverSide[i].transform.localScale = transform.localScale;
                //shadowObjectsReceiverSide[i].transform.rotation = transform.rotation;
            }
            
            //Failsafe, in case lights are removed during runtime.
			while(shadowObjectsCasterSide.Count > numOfLights){
				shadowObjectsCasterSide.RemoveAt(shadowObjectsCasterSide.Count);
				shadowObjectsReceiverSide.RemoveAt(shadowObjectsReceiverSide.Count);
			}

            //For every light, calculate the shadows cast on the wall.
            int shadowIndex = 0;
            Vector3[] worldVertices = mesh.vertices;
            for (int j = 0; j < lights.Length; j++){
				if(lights[j].enabled && lights[j].tag == "ShadowCast"){
                    Ray transRay = new Ray(transform.position, transform.position - lights[j].transform.position);
                    RaycastHit transHit;
                    if(Physics.Raycast(transRay, out transHit, 1000f, wallLayer))
                    {
                        shadowObjectsCasterSide[shadowIndex].transform.position = transHit.point;
                        shadowObjectsReceiverSide[shadowIndex].transform.position = transHit.point;
                    }
                    for (int i = 0; i < casterVertices.Length / 2; i++){
						RaycastHit hit;
                        worldVertices[i] = transform.TransformPoint(new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z));
                        //Ray ray = new Ray(transform.position + mesh.vertices[i], transform.position + mesh.vertices[i] - lights[j].transform.position);
                        Ray ray = new Ray(worldVertices[i], worldVertices[i] - lights[j].transform.position);
						if(Physics.Raycast(ray, out hit, 1000f, wallLayer)){
                            Vector3 hitPoint = new Vector3(0.51f, hit.point.y, hit.point.z);
                            casterVertices[i] = shadowObjectsCasterSide[shadowIndex].transform.InverseTransformPoint(hitPoint);
                            recieverVertices[i] = shadowObjectsCasterSide[shadowIndex].transform.InverseTransformPoint(hitPoint - Vector3.right * 1.02f);
                            casterVertices[casterVertices.Length/2 + i] = recieverVertices[i];
						}
					}   

                    //Assign meshes and colliders
					Mesh shadowMesh = shadowObjectsCasterSide[shadowIndex].GetComponent<MeshFilter>().mesh;
					shadowMesh.vertices = casterVertices;
                    //shadowMesh.triangles = mesh.triangles;
                    shadowMesh.RecalculateBounds();
                    shadowMesh.RecalculateNormals();
                    MeshCollider meshCol = shadowObjectsCasterSide[shadowIndex].GetComponent<MeshCollider>();
					meshCol.sharedMesh = null;
					meshCol.sharedMesh = shadowMesh;

					shadowMesh = shadowObjectsReceiverSide[shadowIndex].GetComponent<MeshFilter>().mesh;
					shadowMesh.vertices = recieverVertices;
                    //shadowMesh.triangles = mesh.triangles;
                    shadowMesh.RecalculateBounds();
                    shadowMesh.RecalculateNormals();

                    shadowIndex++;
                }
			}
        }
	}
}
