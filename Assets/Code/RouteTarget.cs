using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteTarget : MonoBehaviour
{
    [SerializeField]
    private float m_radius = .7f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }
}
