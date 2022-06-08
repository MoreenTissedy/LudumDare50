using System;
using UnityEngine;

    public class Rotate : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        private void Update()
        {
            transform.Rotate(Vector3.forward, speed*Time.deltaTime);
        }
    }
