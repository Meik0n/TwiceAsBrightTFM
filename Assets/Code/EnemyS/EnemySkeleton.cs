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
 * Code by Marcos Butron, 15/06/2022 ©
 *********************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
public class EnemySkeleton : MonoBehaviour
{
    [Header("Navmesh")]
    [SerializeField]
    private bool m_patrol_wait;
    [SerializeField]
    private float m_wait_time = 2f;
    private NavMeshAgent m_agent;
    private int m_actual_patrol_index;
    private bool m_travelling;
    private bool m_waiting;
    private float m_wait_timer;
    private bool rotating = false;
    [SerializeField]
    private float angles_to_turn = 30;
    private bool rotating_right = false;
    private GameObject target;
    private GameObject[] players;

    private States current_state;

    private Animator anim;

    private enum States
    {
        opening, chasing
    }
    private Transform m_t_transform;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {

    }

    void Start()
    {
        anim = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        StartCoroutine(Contador());
        StartCoroutine(Contador2());
    }

    IEnumerator Contador()
    {
        yield return new WaitForSeconds(0.3f);

        players = GameObject.FindGameObjectsWithTag("Player");
        target = players[0];
        StartCoroutine(Chasing(target.transform));
    }
    IEnumerator Contador2()
    {
        yield return new WaitForSeconds(0.4f);
        while (true)
        {
            if (players[0] != null && players[1] != null)
            {
                float distanceP1 = Mathf.Sqrt((players[0].transform.position.x - transform.position.x) * (players[0].transform.position.x - transform.position.x) + (players[0].transform.position.z - transform.position.z) * (players[0].transform.position.z - transform.position.z));
                float distanceP2 = Mathf.Sqrt((players[1].transform.position.x - transform.position.x) * (players[1].transform.position.x - transform.position.x) + (players[1].transform.position.z - transform.position.z) * (players[1].transform.position.z - transform.position.z));

                Debug.Log("dist1" + distanceP1);
                Debug.Log("dist2" + distanceP2);

                if (distanceP1 > distanceP2)
                {
                    target = players[1];
                    StartCoroutine(Chasing(target.transform));
                }
                else if (distanceP1 <= distanceP2)
                {
                    target = players[0]; 
                    StartCoroutine(Chasing(target.transform));
                }
            }
            yield return null;
        }
    }

    private IEnumerator Chasing(Transform t)
    {
        current_state = States.chasing;
        while (current_state == States.chasing)
        {
            m_agent.SetDestination(t.position);
            yield return null;
        }
        yield return null;
    }

    private IEnumerator Opening(Collider other)
    {
        current_state = States.opening;
        m_agent.isStopped = true;
        Animator door_anim = other.GetComponent<Animator>();
        Collider[] cols;
        cols = other.gameObject.GetComponents<Collider>();
        while (current_state == States.opening)
        {
            anim.SetTrigger("Attack");
            door_anim.SetTrigger("Open");
            yield return new WaitForSeconds(2.0f);
            foreach (Collider c in cols)
            {
                c.enabled = false;
            }
            m_agent.isStopped = false;
            StartCoroutine(Chasing(target.transform));
            yield break;
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

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Door"))
        {
            StartCoroutine(Opening(other));
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.blue);
    }
}