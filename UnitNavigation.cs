using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitNavigation : MonoBehaviour
{
    
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Vector3 target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        agent.SetDestination(target);
    }
}
