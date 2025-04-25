using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class CatLogic : AgentLogic
{
    #region Static Variables

    private static readonly float _ChikenPoints = 2.0f;
    private static readonly float _DogPoints = -100.0f;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Chiken")) return;
        points += _ChikenPoints;
        Destroy(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Dog"))
        {
            points += _DogPoints;
        }
    }
}