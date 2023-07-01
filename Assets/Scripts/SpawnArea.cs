using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public Vector2 size;
    private float left, right, top, bottom;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        left = transform.position.x - size.x / 2;
        top = transform.position.y + size.y / 2;
        right = transform.position.x + size.x / 2;
        bottom = transform.position.y - size.y / 2;

        Gizmos.DrawLine(new Vector3(left, top), new Vector3(right, top));
        Gizmos.DrawLine(new Vector3(left, bottom), new Vector3(right, bottom));

        Gizmos.DrawLine(new Vector3(left, top), new Vector3(left, bottom));
        Gizmos.DrawLine (new Vector3(right, top), new Vector3(right, bottom));
    }

    public void SpawnBall(Transform ballTransform)
    {
        float x = Random.Range(left, right);
        float y = Random.Range(top, bottom);

        ballTransform.position = new Vector3(x, y, 0);
    }
}
