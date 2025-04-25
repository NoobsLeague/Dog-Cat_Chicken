using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DogLogic : AgentLogic
{
    #region Static Variables

    private static float _ChikenPoints = 0.1f;
    private static float _CatPoints = 5.0f;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Chiken")) return;
        points += _ChikenPoints;
        Destroy(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.tag.Equals("Cat")) return;
        points += _CatPoints;
        Destroy(other.gameObject);
    }
}