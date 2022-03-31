using UnityEngine;

namespace Universal
{
    public class SmoothFollow : MonoBehaviour
    {
        public Transform target;
        public float smoothTime = 0.3F;
    
        private Vector3 velocity = Vector3.zero;
        private Vector3 lastPosition;
        private Vector3 initialOffset;

        private void Awake()
        {
            lastPosition = transform.position;
            initialOffset = transform.position - target.position;
            if (target == null)
                Debug.LogError("Please define the target for camera to follow.");
        }

        private void Update()
        {
            if (target == null)
                return;

            var newPos = Vector3.SmoothDamp(
                lastPosition,
                target.position + initialOffset,
                ref velocity,
                smoothTime * Time.deltaTime);
            lastPosition = newPos;
            transform.position = newPos;
        }
    }
}
