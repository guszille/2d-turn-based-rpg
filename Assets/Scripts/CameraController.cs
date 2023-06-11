using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 maxDistance = new Vector2(15f, 10f);
    [SerializeField] private Vector2 movementSpeed = new Vector2(25f, 25f);

    private void Start()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    private void Update()
    {
        Vector3 newPosition = new Vector3(0f, 0f, transform.position.z);

        float xAxisOffset = Input.GetAxis("Horizontal") * movementSpeed.x * Time.deltaTime;
        float yAxisOffset = Input.GetAxis("Vertical") * movementSpeed.y * Time.deltaTime;

        float minXPosition = target.position.x - maxDistance.x;
        float maxXPosition = target.position.x + maxDistance.x;
        float minYPosition = target.position.y - maxDistance.y;
        float maxYPosition = target.position.y + maxDistance.y;

        newPosition.x = Mathf.Clamp(transform.position.x + xAxisOffset, minXPosition, maxXPosition);
        newPosition.y = Mathf.Clamp(transform.position.y + yAxisOffset, minYPosition, maxYPosition);

        transform.position = newPosition;
    }
}
