using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    #region Params
    /// <summary>
    /// View distance
    /// </summary>
    public float SenseRange = 10;

    /// <summary>
    /// Boid's view angle
    /// </summary>
    public float FeildOfView = 300;

    /// <summary>
    /// Boid's move speed
    /// </summary>
    public float MoveSpeed = 30;

    /// <summary>
    /// Boid's angular velocity (in angle)
    /// </summary>
    public float AngularVelocity = 1.5f;

    /// <summary>
    /// Is draw nearby boids
    /// </summary>
    public bool IsDrawGismoz = false;
    #endregion

    #region Properties
    /// <summary>
    /// The flock this boid belongs to
    /// </summary>
    private FlockController m_fcBelonging;
    public FlockController Belonging { get { return m_fcBelonging; } set { m_fcBelonging = value; } }

    /// <summary>
    /// Other boids in sight
    /// </summary>
    private List<Boid> m_lsOtherBoidInSight;
    public List<Boid> OtherBoidInSight { get { return m_lsOtherBoidInSight; } }

    /// <summary>
    /// Fly destination
    /// </summary>
    private Vector3 m_v3DstPoint = Vector3.zero;
    public Vector3 DstPoint { get { return m_v3DstPoint; } set { m_v3DstPoint = value; } }
    #endregion

    #region Interface
    /// <summary>
    /// Is other boid in sight
    /// </summary>
    /// <param name="tBoid"> Other boid </param>
    /// <returns> In sight or not </returns>
    public bool IsInSight(Boid tBoid)
    {
        Vector3 v3Dir = tBoid.transform.position - transform.position;
        if (v3Dir.magnitude > SenseRange)
            return false;
        if (Vector3.Angle(transform.forward, v3Dir) > FeildOfView / 2)
            return false;
        return true;
    }
    #endregion

    #region Flock Three Elements + Fly Towards Target
    private void FlyToTarget()
    {
        Vector3 dirTurnTo = m_v3DstPoint - transform.position;
        float v3Angle = Vector3.Angle(dirTurnTo, transform.forward);
        transform.forward = Quaternion.AngleAxis(v3Angle > AngularVelocity ? AngularVelocity : v3Angle, Vector3.Cross(transform.forward, dirTurnTo)) * transform.forward;
    }

    private void Seperation()
    {
        Vector3 dirTurnTo = Vector3.zero;
        for (int i = 0; i < m_lsOtherBoidInSight.Count; i++)
            dirTurnTo += transform.position - m_lsOtherBoidInSight[i].transform.position;
        float v3Angle = Vector3.Angle(dirTurnTo, transform.forward);
        transform.forward = Quaternion.AngleAxis(v3Angle > AngularVelocity ? AngularVelocity : v3Angle, Vector3.Cross(transform.forward, dirTurnTo)) * transform.forward;
    }

    private void Alignment()
    {
        Vector3 dirTurnTo = Vector3.zero;
        for (int i = 0; i < m_lsOtherBoidInSight.Count; i++)
            dirTurnTo += m_lsOtherBoidInSight[i].transform.forward;
        float v3Angle = Vector3.Angle(dirTurnTo, transform.forward);
        transform.forward = Quaternion.AngleAxis(v3Angle > AngularVelocity ? AngularVelocity : v3Angle, Vector3.Cross(transform.forward, dirTurnTo)) * transform.forward;
    }

    private void Cohesion()
    {
        Vector3 dirTurnTo = Vector3.zero;
        for (int i = 0; i < m_lsOtherBoidInSight.Count; i++)
            dirTurnTo += m_lsOtherBoidInSight[i].transform.position;
        dirTurnTo /= m_lsOtherBoidInSight.Count;
        float v3Angle = Vector3.Angle(dirTurnTo, transform.forward);
        transform.forward = Quaternion.AngleAxis(v3Angle > AngularVelocity ? AngularVelocity : v3Angle, Vector3.Cross(transform.forward, dirTurnTo)) * transform.forward;
    }
    #endregion

    #region Method
    /// <summary>
    /// Change boid's direction
    /// </summary>
    private void ChangeDir()
    {
        if (m_fcBelonging.FlyToPoint)
            FlyToTarget();
        if (m_fcBelonging.DoSeparate)
            Seperation();
        if (m_fcBelonging.DoAlign)
            Alignment();
        if (m_fcBelonging.DoCohere)
            Cohesion();
    }

    /// <summary>
    /// Boid move along with it's direction
    /// </summary>
    private void MoveForward()
    {
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;
    }
    #endregion

    #region UnityLifecycle
    // Use this for initialization
    void Start()
    {
        m_lsOtherBoidInSight = new List<Boid>();
        MoveSpeed += Random.Range(-5, 5);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeDir();
        MoveForward();
    }

    private void OnDrawGizmos()
    {
        if (IsDrawGismoz)
        {
            Gizmos.color = Color.green;
            foreach (var boid in m_lsOtherBoidInSight)
                Gizmos.DrawLine(transform.position, boid.transform.position);
        }
    }
    #endregion
}