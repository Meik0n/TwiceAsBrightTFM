/*********************************************
 *⣿⠿⣛⣯⣭⣭⣭⣭⣭⣭⣥⣶⣶⣶⣶⣶⣮⣭⣭⣭⣭⣭⡛⢻⣿⣿⣿⣿⣿⣿⣿
 *⡇⣾⣿⣿⣿⣿⣿⠿⢛⣯⣭⣭⣷⣶⣶⣶⣶⣶⣶⣶⣶⣬⣭⢸⣿⣿⣿⣿⣿⣿⣿
 *⢰⣶⣶⣶⣶⣶⢰⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⢸⣿⣿⣿⣿⣿⣿⣿
 *⡏⣿⣿⣿⣿⣿⢸⣿⣿⣿⣿⡿⢋⣩⠭⠭⡙⢋⣭⠶⠒⠒⢍⡘⠻⣿⣿⣿⣿⣿⣿
 *⡇⣿⣿⣿⣿⣿⢸⣿⣿⡿⣋⣴⣯⡴⠚⠉⡡⠤⢄⣉⣅⡤⠄⢀⢺⡌⣻⣿⣿⣿⣿
 *⡇⣿⣿⣿⣿⣿⢸⣿⡏⡆⣿⣿⣉⣐⢴⣿⠈⠈⢀⠟⡿⠷⠄⢠⢎⢰⣿⣿⣿⣿⣿
 *⡇⣿⣿⣿⣿⣿⢸⣿⢸⣿⣿⣿⡫⣽⣒⣤⠬⠬⠤⠭⠭⢭⣓⣒⡏⣾⣿⣿⣿⣿⣿
 *⡇⣿⣿⣿⣿⣿⢸⡿⢸⣿⣿⣿⣿⣷⣾⣾⣭⣭⣭⣭⣭⣵⣵⡴⡇⠉⠹⣿⣿⣿⣿
 *⡇⣿⣿⣿⣿⣿⢸⠠⠄⠉⠉⠛⠛⠛⠛⠛⠊⠉⠉⠉⠉⠁⠄⠄⠄⠠⢤⡸⣿⣿⣿
 *⢇⡻⠿⣿⣿⣿⠘⣠⣤⣤⣀⡚⠿⢦⣄⡀⠤⠤⠤⣤⣤⣤⣤⣤⣤⣄⣘⠳⣭⢻⣿
 *⣎⢿⣿⣶⣬⣭⣀⠛⢿⣿⣿⣿⣷⣶⣬⣙⡳⠟⢗⣈⠻⠛⠛⠛⠛⢿⣿⣿⣦⢸⣿
 *⣿⣆⢿⣿⣿⣿⣽⣛⣲⠤⠤⢤⣤⣤⣤⣀⡙⣿⣿⣿⠇⣤⣤⣤⡶⢰⣿⣿⠃⣼⣿
 *⣿⣿⣆⢿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣶⣶⣶⡖⣸⣿⡟⣠⣶⣶⡖⣠⣿⡿⣡⣾⣿⣿
 *⣿⣿⣿⢸⣿⣿⣿⣿⣿⣿⣿⣽⣛⣛⡻⣿⠇⣿⣿⠃⣿⣟⡭⠁⣿⣯⣄⢻⣿⣿⣿
 *⣿⣿⣿⢸⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⣿⣷⣭⣙⠗⣸⣿⡇⣾⣮⣙⡛⣸⣿⣿⣿
 *
 *
 * 
 * Code by Marcos Butron, 07/04/2022 ©
 *********************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemySlime : MonoBehaviour
{
    [Header("Navmesh")]
    [SerializeField]
    private bool m_patrol_wait;
    [SerializeField]
    private float m_wait_time = 2f;
    [SerializeField]
    private List<RouteTarget> m_route;
    private NavMeshAgent m_agent;
    private int m_actual_patrol_index;
    private bool m_travelling;
    private bool m_waiting;
    private float m_wait_timer;
    private bool rotating = false;
    [SerializeField]
    private float angles_to_turn = 30;
    private bool rotating_right = false;

    private States current_state;

    private Animator anim;

    private enum States
    {
        patrolling
    }

    void Start()
    {
        anim = GetComponent<Animator>();

        StartCoroutine(Patrolling());
        StartCoroutine(Find_Targets_With_Delay(.2f));

        m_agent = GetComponent<NavMeshAgent>();
        if (m_route != null && m_route.Count >= 2)
        {
            m_actual_patrol_index = 0;
            Set_Destination();
        }
    }

    void Update()
    {
    }

    void LateUpdate()
    {

    }

    private IEnumerator Patrolling()
    {
        current_state = States.patrolling;
        while (current_state == States.patrolling)
        {
            if (m_travelling && m_agent.remainingDistance <= 0.5)
            {
                m_travelling = false;

                if (m_patrol_wait)
                {
                    m_waiting = true;
                    m_wait_timer = 0f;
                }
                else
                {
                    Change_Patrol_Point();
                    Set_Destination();
                }
            }

            if (m_waiting)
            {
                m_wait_timer += Time.deltaTime;
                if (rotating_right)
                {
                    StartCoroutine(Rotate_Enemy(new Vector3(0, angles_to_turn, 0), m_wait_time / 2));
                    rotating_right = false;
                }
                else if (!rotating_right)
                {
                    StartCoroutine(Rotate_Enemy(new Vector3(0, -angles_to_turn, 0), m_wait_time / 2));
                    rotating_right = true;
                }

                if (m_wait_timer >= m_wait_time)
                {
                    m_waiting = false;
                    Change_Patrol_Point();
                    Set_Destination();
                }
            }
            yield return null;
        }
        yield return null;
    }
    /// <Sumary>
    /// Rotates game object from his actual rotation to a desired one in a certain period of time
    /// </Sumary>
    /// <param name=" _euler_angles"> Angles to turn (Vector 3 (euler)) </param>
    /// <param name=" _duration"> Time to finish the rotation </param>

    private IEnumerator Rotate_Enemy(Vector3 _euler_angles, float _duration)
    {
        if (rotating)
        {
            yield break;
        }
        rotating = true;

        Vector3 desired_rotation = transform.eulerAngles + _euler_angles;
        Vector3 current_rotation = transform.eulerAngles;

        float time_counter = 0;
        while (time_counter < _duration)
        {
            time_counter += Time.deltaTime;
            transform.eulerAngles = Vector3.Lerp(current_rotation, desired_rotation, time_counter / _duration);
            yield return null;
        }
        rotating = false;
        yield break;
    }
    private IEnumerator Find_Targets_With_Delay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
        }
    }

    public Vector3 Dir_From_Agle(float angle_in_degrees, bool angle_is_global)
    {
        if (!angle_is_global)
        {
            angle_in_degrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle_in_degrees * Mathf.Deg2Rad), 0, Mathf.Cos(angle_in_degrees * Mathf.Deg2Rad));
    }

    private void Change_Patrol_Point()
    {
        m_actual_patrol_index++;
        if (m_actual_patrol_index >= m_route.Count)
        {
            m_actual_patrol_index = 0;
        }
    }
    private void Set_Destination()
    {
        Vector3 target_vector = m_route[m_actual_patrol_index].transform.position;
        m_agent.SetDestination(target_vector);
        m_travelling = true;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.blue);
    }
}
