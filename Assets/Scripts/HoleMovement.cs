using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class HoleMovement : MonoBehaviour
{
	[Header ("Hole mesh")]
	[SerializeField] MeshFilter meshFilter;
	[SerializeField] MeshCollider meshCollider;

	[Header ("Hole vertices radius")]
	[SerializeField] Vector2 moveLimits;
	//Hole vertices radius from the hole's center
	[SerializeField] float radius;
	[SerializeField] Transform holeCenter;
	//rotating circle arround hole (animation)
	[SerializeField] Transform rotatingCircle;

	[Space]
	[SerializeField] float moveSpeed;

	Mesh mesh;
	List<int> holeVertices;
	//hole vertices offsets from hole center
	List<Vector3> offsets;
	int holeVerticesCount;

	float x, y;
	Vector3 touch, targetPos;

	void Start ()
	{
		RotateCircleAnim ();

		Game.isMoving = false;
		Game.isGameover = false;

		
		holeVertices = new List<int> ();
		offsets = new List<Vector3> ();

	
		mesh = meshFilter.mesh;

		FindHoleVertices ();
	}

	void RotateCircleAnim ()
	{

		rotatingCircle
			.DORotate (new Vector3 (90f, 0f, -90f), .2f)
			.SetEase (Ease.Linear)
			.From (new Vector3 (90f, 0f, 0f))
			.SetLoops (-1, LoopType.Incremental);
	}

	void Update ()
	{
	
		#if UNITY_EDITOR //mouse use 

		Game.isMoving = Input.GetMouseButton (0);

		if (!Game.isGameover && Game.isMoving) {

			MoveHole ();
	
			UpdateHoleVerticesPosition ();
		}



		#else
		
		Game.isMoving = Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved;

		if (!Game.isGameover && Game.isMoving) {
		
		MoveHole ();
		
		UpdateHoleVerticesPosition ();
		}
		#endif


	}

	void MoveHole ()
	{
		x = Input.GetAxis ("Mouse X");
		y = Input.GetAxis ("Mouse Y");

		touch = Vector3.Lerp (
			holeCenter.position, 
			holeCenter.position + new Vector3 (x, 0f, y), //move hole on x & z 
			moveSpeed * Time.deltaTime
		);

		targetPos = new Vector3 (
			
			Mathf.Clamp (touch.x, -moveLimits.x, moveLimits.x),//limit X
			touch.y,
			Mathf.Clamp (touch.z, -moveLimits.y, moveLimits.y)//limit Z
		);

		holeCenter.position = targetPos;
	}

	void UpdateHoleVerticesPosition ()
	{
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < holeVerticesCount; i++) {
			vertices [holeVertices [i]] = holeCenter.position + offsets [i];
		}

		mesh.vertices = vertices;
		
		meshFilter.mesh = mesh;

		meshCollider.sharedMesh = mesh;
	}

	void FindHoleVertices ()
	{
		for (int i = 0; i < mesh.vertices.Length; i++) {
			//Calculate distance between holeCenter & each Vertex
			float distance = Vector3.Distance (holeCenter.position, mesh.vertices [i]);

			if (distance < radius) {
				holeVertices.Add (i);
				offsets.Add (mesh.vertices [i] - holeCenter.position);
			}
		}
		holeVerticesCount = holeVertices.Count;
	}


	void OnDrawGizmos ()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (holeCenter.position, radius);
	}
}
