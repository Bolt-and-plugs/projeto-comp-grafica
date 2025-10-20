using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    [Header("Camera movement for rendering")]
    public float radius, speed;
    private float x, z, tetha;
    public Transform targetObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      tetha = 0.0f;
      radius = 288.0f;
      speed = 0.5f;
      x = transform.position.x;
      z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        tetha = Time.time * speed % 360;

        x = Mathf.Sin(tetha) * radius;
        z = Mathf.Cos(tetha) * radius;

        transform.position = new Vector3(x, transform.position.y, z);
        transform.LookAt(targetObject);
    }
}
