using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    private const float PARALLAX_VERTICAL_POWER_MODIFIER = 0.3F;
    [SerializeField] private float parallaxPower;
    private float parallaxSpeed = 1.3f;
    private Vector2 StartPosition;

    private void Start()
    {
        StartPosition = transform.position;
    }

    private void LateUpdate()
    {
        float x = (Input.mousePosition.x - Screen.width / 2) * parallaxPower / Screen.width;
        float y = (Input.mousePosition.y - Screen.height / 2) * parallaxPower * PARALLAX_VERTICAL_POWER_MODIFIER / Screen.height;

        transform.position =  Vector2.Lerp(transform.position,
                                                          StartPosition + new Vector2(x, y),
                                                          parallaxSpeed * Time.unscaledDeltaTime);
    }
}
