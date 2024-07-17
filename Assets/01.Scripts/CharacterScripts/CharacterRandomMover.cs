using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class CharacterRandomMover : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float moveRadius = 3f; // 움직임 반경
    [SerializeField] private float minMoveTime = 2f; // 최소 대기 시간
    [SerializeField] private float maxMoveTime = 3f; // 최대 대기 시간
    [SerializeField] private float minMoveDuration = 1f; // 최소 이동 시간
    [SerializeField] private float maxMoveDuration = 3f; // 최대 이동 시간

    public float moveSpeed = 3.5f; // 이동 속도

    private NavMeshSurface navMeshSurface; // NavMeshSurface 참조
    private NavMeshAgent agent; // NavMeshAgent를 저장할 변수
    private void Awake()
    {
        navMeshSurface = FindObjectOfType<NavMeshSurface>(true);
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // 로테이션 업데이트 비활성화
        agent.speed = moveSpeed;
    }

    private void OnEnable()
    {
        RandomWarp();
        StartCoroutine(MoveRandomCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void RandomWarp()
    {
        Vector3 randomPosition = GetRandomPositionWithinNavMesh();
        agent.Warp(randomPosition); // NavMesh 위의 랜덤한 위치로 캐릭터를 이동시킴
    }
    Vector3 GetRandomPositionWithinNavMesh()
    {
        // NavMeshSurface의 바운딩 박스를 가져옴
        Bounds bounds = navMeshSurface.navMeshData.sourceBounds;
        Vector3 randomPosition;

        while (true)
        {
            randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.center.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
    }

    IEnumerator MoveRandomCoroutine()
    {
        while (true)
        {
            float moveDuration = Random.Range(minMoveDuration, maxMoveDuration);
            Vector3 destination = GetRandomDestination();
            agent.SetDestination(destination);
            // MoveToRandomPosition();
            animator.SetBool("isMoving", true);

            float startTime = Time.time;
            // 목적지에 도착할 때까지 기다림
            while (Time.time - startTime < moveDuration && agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            agent.ResetPath();
            animator.SetBool("isMoving", false);

            float waitTime = Random.Range(minMoveTime, maxMoveTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector3 GetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, moveRadius, 1);
        return hit.position;
    }
}
