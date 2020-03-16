using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour
{
    #region Properties
    /// <summary>
    /// This flock's entity model
    /// </summary>
    public Boid EntityPrefab;

    /// <summary>
    /// Flock width
    /// </summary>
    public float FlockWidth = 10;

    /// <summary>
    /// Flock height
    /// </summary>
    public float FlockHeight = 10;

    /// <summary>
    /// Flock depth
    /// </summary>
    public float FlockDepth = 10;

    /// <summary>
    /// This flock's entity number
    /// </summary>
    public int BoidNumber = 0;

    public bool FlyToPoint = true;

    /// <summary>
    /// Separation switch
    /// </summary>
    public bool DoSeparate = true;

    /// <summary>
    /// Alignment switch
    /// </summary>
    public bool DoAlign = true;

    /// <summary>
    /// Cohesion switch
    /// </summary>
    public bool DoCohere = true;
    #endregion

    #region Properties
    /// <summary>
    /// Flock center
    /// </summary>
    public Vector3 Position { get { return transform.position; } set { transform.position = value; } }

    /// <summary>
    /// All entities in this flock
    /// </summary>
    private List<Boid> m_lsBoids;

    private Vector3 m_v3FlockMoveDstPos;

    private float m_fMoveInternal = 5;
    private float m_fMoveCoolDown = 0;
    #endregion

    #region Interface
    /// <summary>
    /// Randomly generate point inside flock
    /// </summary>
    /// <returns> Point generated </returns>
    public Vector3 GenerateRandomPoint()
    {
        float fPointX = Random.Range(-1 * FlockWidth / 2, FlockWidth / 2);
        float fPointY = Random.Range(-1 * FlockHeight / 2, FlockHeight / 2);
        float fPointZ = Random.Range(-1 * FlockDepth / 2, FlockDepth / 2);

        return new Vector3(Position.x + fPointX, Position.y + fPointY, Position.z + fPointZ);
    }

    /// <summary>
    /// Is boid still in flock
    /// </summary>
    /// <param name="tBoid"> Boic </param>
    /// <returns> Boid in flock or not </returns>
    public bool Contain(Boid tBoid)
    {
        Vector3 v3LocalBoidPos = transform.worldToLocalMatrix * tBoid.transform.position;
        if (v3LocalBoidPos.x > FlockWidth / 2 || v3LocalBoidPos.x < -FlockWidth / 2)
            return false;
        if (v3LocalBoidPos.y > FlockHeight / 2 || v3LocalBoidPos.y < -FlockHeight / 2)
            return false;
        if (v3LocalBoidPos.z > FlockDepth / 2 || v3LocalBoidPos.z < -FlockDepth / 2)
            return false;
        return true;
    }
    #endregion

    #region Life Cycle
    // Use this for initialization
    void Start ()
    {
        /*********** Set position ************/
        Position = Vector3.zero;
        m_v3FlockMoveDstPos = GenerateRandomPoint();
        m_fMoveInternal = Random.Range(1, 3);

        /*********** Create boids ************/
        m_lsBoids = new List<Boid>(BoidNumber);
        for (int i = 0; i < BoidNumber; i++)
        {
            Boid etyChild = (Boid)Instantiate(EntityPrefab);
            etyChild.Belonging = this;
            etyChild.DstPoint = m_v3FlockMoveDstPos;
            etyChild.transform.position = GenerateRandomPoint();
            etyChild.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), 0);

            m_lsBoids.Add(etyChild);
        }
    }
    
    // Update is called once per frame
    void Update ()
    {
        if (m_fMoveCoolDown < m_fMoveInternal)
            m_fMoveCoolDown += Time.deltaTime;
        else
        {
            m_fMoveCoolDown = 0;
            m_fMoveInternal = Random.Range(1, 3);
            m_v3FlockMoveDstPos = GenerateRandomPoint();
            for (int i = 0; i < BoidNumber; i++)
                m_lsBoids[i].DstPoint = m_v3FlockMoveDstPos;
        }

        for (int i = 0; i < BoidNumber; i++)
        {
            /********** Add in sight boids ***********/
            m_lsBoids[i].OtherBoidInSight.Clear();
            for (int j = 0; j < BoidNumber; j++)
            {
                if (i == j) continue;
                if (m_lsBoids[i].IsInSight(m_lsBoids[j]))
                    m_lsBoids[i].OtherBoidInSight.Add(m_lsBoids[j]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Position, new Vector3(FlockWidth, FlockHeight, FlockDepth));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_v3FlockMoveDstPos, 0.5f);
    }
    #endregion
}
