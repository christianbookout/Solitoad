using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    public float speed = 0.2f;
    private bool movingDown = true;

    void Start()
    {
        originalPosition = transform.position;
        targetPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
        StartCoroutine(MoveArrow());
    }

    IEnumerator MoveArrow()
    {
        while (true)
        {
            if (transform.position == targetPosition)
            {
                movingDown = false;
                yield return new WaitForSeconds(0.3f);
            }
            else if (transform.position == originalPosition)
            {
                movingDown = true;
                yield return new WaitForSeconds(0.3f);
            }

            if (movingDown)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, speed * Time.deltaTime);
            }

        }
    }
}
