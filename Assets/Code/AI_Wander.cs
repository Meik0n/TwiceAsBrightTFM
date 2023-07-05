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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Wander : MonoBehaviour
{
    public float wander_radius;
    public float wander_timer;

    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wander_timer;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wander_timer)
        {
            Vector3 new_pos = RandomNavSphere(transform.position, wander_radius, -1);
            agent.SetDestination(new_pos);
            timer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 rand_direction = Random.insideUnitSphere * dist;

        rand_direction += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(rand_direction, out navHit, dist, layermask);

        return navHit.position;
    }
}
