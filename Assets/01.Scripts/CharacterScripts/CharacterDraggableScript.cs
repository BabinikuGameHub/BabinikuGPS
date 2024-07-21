using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterDraggableScript : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private CharacterRandomMover randomMover;

    void Start()
    {
        mainCamera = Camera.main;
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        randomMover = GetComponent<CharacterRandomMover>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    originalPosition = transform.position;
                    offset = hit.point - transform.position;
                    // randomMover.enabled = false;
                    randomMover.Hold();
                    // navMeshAgent.enabled = false;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            // navMeshAgent.enabled = true; // Re-enable NavMeshAgent
            // randomMover.enabled = true;
            randomMover.Resume();

            // Move character to the closest position on the NavMesh
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(transform.position, out navHit, 3.0f, NavMesh.AllAreas))
            {
                // navMeshAgent.SetDestination(navHit.position);
            }
            else
            {
                // If no valid NavMesh position is found, return to the original position
                // navMeshAgent.SetDestination(originalPosition);
            }
        }

        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // Vector3 targetPosition = hit.point - offset;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }
        }
    }
}
