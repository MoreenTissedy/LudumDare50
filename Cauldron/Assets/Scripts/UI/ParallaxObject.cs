using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    private const float PARALLAX_VERTICAL_POWER_MODIFIER = 0.3F;
    [SerializeField] private float parallaxPower;
    private Vector3 StartPosition;

    private void Start()
    {
        StartPosition = transform.position;
    }

    private void LateUpdate()
    {
        float x, y;
        x = (Input.mousePosition.x - Screen.width / 2) * parallaxPower / Screen.width;
        y = (Input.mousePosition.y - Screen.height / 2) * parallaxPower * PARALLAX_VERTICAL_POWER_MODIFIER / Screen.height;
        
        transform.position = StartPosition + new Vector3(x, y, 0);
    }
}
