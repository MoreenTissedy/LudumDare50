using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace CauldronCodebase
{
    public class Diamond : MonoBehaviour
    {
        public float distance = 10;
        public float time = 1;    
        private void Start()
        {
            Vector2 dir = Random.insideUnitCircle; 
            transform.DOMove(dir*distance, time);
            transform.DOScale(Vector3.one, time).From(0.2f);
            StartCoroutine(Delete());
        }

        IEnumerator Delete()
        {
            yield return new WaitForSeconds(time + time);
            Destroy(gameObject);
        }
    }
}