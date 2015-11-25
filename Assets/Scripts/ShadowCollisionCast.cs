﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowCollisionCast : MonoBehaviour {

	Light[] lights;
	List<GameObject> shadowObjectsCasterSide;
    Mesh mesh;

	GameObject shadowObject;
    LayerMask collisionLayer;

    public bool isEnemy = false;
	public LayerMask wallLayer;

	// Use this for initialization
	void Start () {
        collisionLayer =  LayerMask.NameToLayer("Shadows");
		lights = FindObjectsOfType<Light>() as Light[];
		shadowObjectsCasterSide = new List<GameObject>();
        mesh = GetComponent<MeshFilter>().mesh;

        //Initializing GameObjects
        shadowObject = new GameObject("shadowCollider");
        if (isEnemy)
            shadowObject.tag = "Enemy";
        shadowObject.layer = collisionLayer.value;
        MeshFilter objMeshFilter = shadowObject.AddComponent<MeshFilter>();
        MeshCollider objMeshCol = shadowObject.AddComponent<MeshCollider>();

        objMeshFilter.mesh = mesh;
        objMeshCol.convex = true;
        objMeshCol.isTrigger = false;
        objMeshCol.sharedMesh = null;
        objMeshCol.sharedMesh = objMeshFilter.mesh;

        shadowObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        //Initialize the vertex arrays. The caster vertices need to be twice length, since we want the platform to go through the wall (one on each side.)
        Vector3[] casterVertices = new Vector3[mesh.vertices.Length * 2];

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
        }

        for (int i = 0; i < shadowObjectsCasterSide.Count; i++){
            shadowObjectsCasterSide[i].transform.position = transform.position;
            //shadowObjectsCasterSide[i].transform.localScale = transform.localScale;
            //shadowObjectsCasterSide[i].transform.rotation = transform.rotation;
        }
        
        //Failsafe, in case lights are removed during runtime.
		while(shadowObjectsCasterSide.Count > numOfLights){
			shadowObjectsCasterSide.RemoveAt(shadowObjectsCasterSide.Count);
		}

        //For every light, calculate the shadows cast on the wall.
        int shadowIndex = 0;
        Vector3[] worldVertices = mesh.vertices;
        for (int j = 0; j < lights.Length; j++){
			if(lights[j].enabled && lights[j].tag == "ShadowCast"){
				for(int i = 0; i < casterVertices.Length / 2; i++){
					RaycastHit hit;
                    worldVertices[i] = transform.TransformPoint(new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z));
                    //Ray ray = new Ray(transform.position + mesh.vertices[i], transform.position + mesh.vertices[i] - lights[j].transform.position);
                    Ray ray = new Ray(worldVertices[i], worldVertices[i] - lights[j].transform.position);
					if(Physics.Raycast(ray, out hit, 1000f, wallLayer)){
                        Vector3 hitPoint = new Vector3(0.51f, hit.point.y, hit.point.z);
                        casterVertices[i] = shadowObjectsCasterSide[shadowIndex].transform.InverseTransformPoint(hitPoint);
                        casterVertices[casterVertices.Length/2 + i] = shadowObjectsCasterSide[shadowIndex].transform.InverseTransformPoint(hitPoint - Vector3.right * 1.02f);
                    }
				}

                //Assign meshes and colliders
				Mesh shadowMesh = shadowObjectsCasterSide[shadowIndex].GetComponent<MeshFilter>().mesh;
				shadowMesh.vertices = casterVertices;
                //shadowMesh.triangles = mesh.triangles;
                shadowMesh.RecalculateBounds();
                MeshCollider meshCol = shadowObjectsCasterSide[shadowIndex].GetComponent<MeshCollider>();
				meshCol.sharedMesh = null;
				meshCol.sharedMesh = shadowMesh;

                shadowIndex++;
            }
		}
    }
}