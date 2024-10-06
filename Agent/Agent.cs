using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

namespace Sample
{
    public class Agent : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        private readonly Vector3[] bufferPath = new Vector3[128];

        public int pathLength;
        public int pointer;
        public void MoveToPosition(Vector3 groundPoint)
        {
            this.pathLength = GroundAreaSystem.pathFinder.CalculatePath(this.transform.position, groundPoint, bufferPath);
           
            this.pointer = 0;
        }
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < pathLength; i++)
            {
                Gizmos.DrawLine(bufferPath[i], bufferPath[i + 1]);
            }
        }

        public void OnFixedUpdate()
        {
            if (this.pointer >= this.pathLength)
            {
                return;
            }
            Vector3 nextPoint = this.bufferPath[this.pointer];
            var delta = nextPoint - this.transform.position;
            if (delta.magnitude <= 0.1f)
            {
                this.pointer++;
                return;
            }

            this.transform.position += delta.normalized * speed * Time.fixedDeltaTime;
        }
    }
}