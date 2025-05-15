using UnityEngine;
using UnityEngine.InputSystem;

public class ParallaxObject : MonoBehaviour
{
    private const float PARALLAX_VERTICAL_POWER_MODIFIER = 0.3F;
    private const float PARALLAX_SPEED = 15f;

    [SerializeField] private float parallaxPower;

    private Vector2 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (Mouse.current is null)
        {
            return;
        }
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float x = (mousePosition.x - Screen.width / 2f) * parallaxPower / Screen.width;
        float y = (mousePosition.y - Screen.height / 2f) * parallaxPower * PARALLAX_VERTICAL_POWER_MODIFIER / Screen.height;

        Vector2 targetPosition = startPosition + new Vector2(x, y);
        Vector2 transformPosition = transform.position;
        float delta = (targetPosition - transformPosition).magnitude;

        transform.position =  Vector2.Lerp(transformPosition,
                                                          targetPosition,
                                                          PARALLAX_SPEED * delta * Time.unscaledDeltaTime);
    }
}
