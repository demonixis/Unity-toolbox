using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshMerger : MonoBehaviour 
{
	public Material material;
	public bool addMeshCollider = true;
	private Transform _transform;
	
	void Awake() 
	{
		_transform = transform;
		
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;

		while (i < meshFilters.Length) 
		{
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.SetActive(false);
			i++;
		}
		
		_transform.GetComponent<MeshFilter>().mesh = new Mesh();
		_transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		_transform.GetComponent<MeshRenderer>().sharedMaterial = material;
		
		if (addMeshCollider)
			_transform.gameObject.AddComponent<MeshCollider>();
		
		_transform.gameObject.SetActive(true);
	}
}