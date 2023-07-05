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

public class Enemy : MonoBehaviour
{
    [Header("FOV")]
    [SerializeField]
    private float m_view_radius;
    [SerializeField]
    [Range(0, 360)]
    private float m_view_angle;
    [SerializeField]
    private LayerMask m_target_mask;
    [SerializeField]
    private LayerMask m_obstacle_mask;
    private List<Transform> m_visible_targets = new List<Transform>();

    [SerializeField]
    private float m_mesh_resolution;
    [SerializeField]
    private int m_edge_resolve_iterations;
    [SerializeField]
    private float m_edge_dist_threshold;
    [SerializeField]
    private float m_mask_cutaway_dist = .1f;
    [SerializeField]
    private MeshFilter m_view_mesh_filter;

    private Mesh m_view_mesh;

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
        patrolling, breaking, chasing
    }
    private Transform m_t_transform;


    [Header("CameraShake")]
    [SerializeField]
    private CameraShake m_camera_shake;
    [SerializeField]
    private float m_duration;
    [SerializeField]
    private float m_intensity;

    public struct View_Cast_Info
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public View_Cast_Info(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    public struct Edge_Info
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public Edge_Info(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    void Start()
    {
        m_view_mesh = new Mesh();
        m_view_mesh.name = "View Mesh";
        m_view_mesh_filter.mesh = m_view_mesh;

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
        if (m_visible_targets.Count == 0 && current_state == States.chasing)
        {
            StopCoroutine(Chasing(m_t_transform));
            StartCoroutine(Patrolling());
        }
    }

    void LateUpdate()
    {
        Draw_Field_Of_View();
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

    private IEnumerator Breaking(Collider other)
    {
        current_state = States.breaking;
        m_agent.isStopped = true;
        while (current_state == States.breaking)
        {
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(2.19f);
            Destroy(other.gameObject);
            StartCoroutine(m_camera_shake.Shake(m_duration, m_intensity));
            m_agent.isStopped = false;
            StartCoroutine(Patrolling());
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
    private IEnumerator Find_Targets_With_Delay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Find_Visible_Targets();
        }
    }

    /// <Sumary>
    /// Moves the game object to the target position
    /// </Sumary>
    private void Find_Visible_Targets()
    {
        m_visible_targets.Clear();
        Collider[] targets_in_view_radius = Physics.OverlapSphere(transform.position, m_view_radius, m_target_mask);

        for (int i = 0; i < targets_in_view_radius.Length; i++)
        {
            Transform target = targets_in_view_radius[i].transform;
            GameObject target_go = targets_in_view_radius[i].gameObject;
            Vector3 dir_to_target = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dir_to_target) < m_view_angle / 2)
            {
                float dist_to_target = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dir_to_target, dist_to_target, m_obstacle_mask))
                {
                    m_visible_targets.Add(target);
                }

                if (m_visible_targets != null)
                {
                    foreach (Transform t in m_visible_targets)
                    {
                        m_t_transform = t;
                        StartCoroutine(Chasing(t));
                        if (transform.position == t.position)
                        {
                            // m_waiting = true;
                            anim.SetTrigger("Attack");
                        }
                    }
                }
            }
        }
    }

    private void Draw_Field_Of_View()
    {
        int step_count = Mathf.RoundToInt(m_view_angle * m_mesh_resolution);
        float step_angle_size = m_view_angle / step_count;
        List<Vector3> view_points = new List<Vector3>();
        View_Cast_Info old_view_cast = new View_Cast_Info();
        for (int i = 0; i <= step_count; i++)
        {
            float angle = transform.eulerAngles.y - m_view_angle / 2 + step_angle_size * i;
            View_Cast_Info new_view_cast = View_Cast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(old_view_cast.dst - new_view_cast.dst) > m_edge_dist_threshold;
                if (old_view_cast.hit != new_view_cast.hit || (old_view_cast.hit && new_view_cast.hit && edgeDstThresholdExceeded))
                {
                    Edge_Info edge = Find_Edge(old_view_cast, new_view_cast);
                    if (edge.pointA != Vector3.zero)
                    {
                        view_points.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        view_points.Add(edge.pointB);
                    }
                }

            }

            view_points.Add(new_view_cast.point);
            old_view_cast = new_view_cast;
        }

        int vertex_count = view_points.Count + 1;
        Vector3[] vertices = new Vector3[vertex_count];
        int[] triangles = new int[(vertex_count - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertex_count - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(view_points[i]) + Vector3.forward * m_mask_cutaway_dist;

            if (i < vertex_count - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        m_view_mesh.Clear();

        m_view_mesh.vertices = vertices;
        m_view_mesh.triangles = triangles;
        m_view_mesh.RecalculateNormals();
    }

    private Edge_Info Find_Edge(View_Cast_Info min_view_cast, View_Cast_Info max_view_cast)
    {
        float min_angle = min_view_cast.angle;
        float max_angle = max_view_cast.angle;
        Vector3 min_point = Vector3.zero;
        Vector3 max_point = Vector3.zero;

        for (int i = 0; i < m_edge_resolve_iterations; i++)
        {
            float angle = (min_angle + max_angle) / 2;
            View_Cast_Info new_view_cast = View_Cast(angle);

            bool edge_dist_threshold_exceeded = Mathf.Abs(min_view_cast.dst - new_view_cast.dst) > m_edge_dist_threshold;
            if (new_view_cast.hit == min_view_cast.hit && !edge_dist_threshold_exceeded)
            {
                min_angle = angle;
                min_point = new_view_cast.point;
            }
            else
            {
                max_angle = angle;
                max_point = new_view_cast.point;
            }
        }

        return new Edge_Info(min_point, max_point);
    }

    private View_Cast_Info View_Cast(float global_angle)
    {
        Vector3 dir = Dir_From_Agle(global_angle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, m_view_radius, m_obstacle_mask))
        {
            return new View_Cast_Info(true, hit.point, hit.distance, global_angle);
        }
        else
        {
            return new View_Cast_Info(false, transform.position + dir * m_view_radius, m_view_radius, global_angle);
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

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Wall"))
        {
            StartCoroutine(Breaking(other));
        }
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.blue);
    }
}