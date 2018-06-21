using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class clicktomove : MonoBehaviour {

    private Animator nAnimator;

    private NavMeshAgent nNavMeshAgent;

    private bool isRunning = false;


	// Use this for initialization
	void Start () {

        nAnimator = GetComponent<Animator>();
        nNavMeshAgent = GetComponent<NavMeshAgent>();
		
	}
	
	// Update is called once per frame
	void Update () {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                nNavMeshAgent.destination = hit.point;
            }
        }

        if (nNavMeshAgent.remainingDistance <= nNavMeshAgent.stoppingDistance)
        {
            isRunning = false;
        }
        else
        {
            isRunning = true;

        }

        nAnimator.SetBool("running", isRunning);
	}
}
