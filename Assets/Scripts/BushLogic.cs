using UnityEngine;

public class BushLogic : MonoBehaviour
{
public float range;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cat"))
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-range, range),
                0,
                Random.Range(-range, range)
            );

            Vector3 teleportPosition = transform.position + randomOffset;

            other.transform.position = teleportPosition;
        }
    }

}
