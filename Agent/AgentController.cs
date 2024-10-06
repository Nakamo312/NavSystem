using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


namespace Sample
{
    public class AgentController : MonoBehaviour
    {
        [SerializeField]
        private Agent[] agents;
        void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var Ground = new Plane(Vector3.up, Vector3.zero);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Ground.Raycast(ray, out float position))
                {
                    Vector3 worldPosition = ray.GetPoint(position);          
                    MoveAgents(worldPosition);
                }
            }
        }
        private void FixedUpdate()
        {
            foreach (var agent in agents)
            {
                agent.OnFixedUpdate();
            }
        }
        private void MoveAgents(Vector3 point)
        {
            foreach (var agent in agents)
            {
                agent.MoveToPosition(point);
            }
        }
    }
}

