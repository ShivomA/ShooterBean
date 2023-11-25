using UnityEngine;
using UnityEngine.UI;

public class GrappleGun : MonoBehaviour
{
    public LayerMask grappleableLayer, grappleObjectLayer;
    public Transform cameraTransform, player, gunTip;
    public Gradient gradient;
    public PlayerStats playerStats;
    private LineRenderer lineRenderer;
    private float slowScale = 10;


    [HideInInspector]
    public bool slowMo = false;
    [HideInInspector]
    public float slowMoTime, maxSlowMoTime = 50f, refillRate = 1;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
        if (player == null)
            player = FindObjectOfType<MovementController>().transform;
        if (cameraTransform == null)
            cameraTransform = FindObjectOfType<CameraController>().transform;
    }

    private void Start()
    {
        slowMoTime = maxSlowMoTime;
        playerStats.setSlowMoTime(slowMoTime, maxSlowMoTime);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) slowMotion();
        if (slowMo)
        {
            slowMoTime -= Time.deltaTime * slowScale;
            if (slowMoTime <= 0) slowMotion();
            playerStats.setSlowMoTime(slowMoTime, maxSlowMoTime);
        }
        else
        {
            slowMoTime += Time.deltaTime * refillRate;
            if (slowMoTime >= maxSlowMoTime) slowMoTime = maxSlowMoTime;
            playerStats.setSlowMoTime(slowMoTime, maxSlowMoTime);
        }

        if (Input.GetMouseButtonDown(0))
            StartGrapple();
        if (Input.GetMouseButtonUp(0))
            StopGrapple();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    RaycastHit hit;
    private Vector3 grapplePoint, currentGrapplePosition, forceDirection;
    private Rigidbody connectedBody;
    private SpringJoint joint;
    float maxDistance = 1000, forceMultiplier = 1000;
    bool isGrapped = false;

    public void slowMotion()
    {
        if (slowMo)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02F;
            slowMo = !slowMo;
        }
        else
        {
            Time.timeScale = 1 / slowScale;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
            slowMo = !slowMo;
        }
    }

    void StartGrapple()
    {
        if (Physics.Raycast(cameraTransform.position + cameraTransform.forward, cameraTransform.forward, out hit, maxDistance, grappleableLayer))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();

            int layer = hit.collider.gameObject.layer;
            if (grappleObjectLayer == (grappleObjectLayer | (1 << layer))) isGrapped = true;
            else isGrapped = false;


            joint.autoConfigureConnectedAnchor = false;
            if (isGrapped)
            {
                connectedBody = hit.collider.attachedRigidbody;
                joint.connectedBody = connectedBody;
                float distanceFromGrapplePoint = Vector3.Distance(player.position, grapplePoint);
                joint.minDistance = 0;
                joint.maxDistance = distanceFromGrapplePoint * 0.4f;

                forceDirection = (player.position - grapplePoint).normalized;
                connectedBody.AddForce(forceMultiplier * forceDirection);
            }
            else
            {
                joint.connectedAnchor = grapplePoint;
                float distanceFromGrapplePoint = Vector3.Distance(player.position, grapplePoint);
                joint.minDistance = 0;
                joint.maxDistance = distanceFromGrapplePoint * 0.4f;
            }

            joint.spring = 5f;
            joint.damper = 1f;
            joint.massScale = 1f;
            joint.enableCollision = true;
            joint.connectedMassScale = 1f;

            currentGrapplePosition = gunTip.position;
            //initializeDynamicRope();
        }
    }

    void StopGrapple()
    {
        CancelInvoke(nameof(DrawDynamicRope));
        lineRenderer.positionCount = 0;
        Destroy(joint);
        //isConnected = false;
    }

    void DrawRope()
    {
        if (!joint) return;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, gunTip.position);
        if (hit.collider == null)
            StopGrapple();
        if (isGrapped)
            lineRenderer.SetPosition(1, hit.collider.transform.position);
        else
            lineRenderer.SetPosition(1, grapplePoint);
    }

    public bool isGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        if (isGrapped)
            return hit.collider.transform.position;
        return grapplePoint;
    }


    #region DynamicRope
    int i, positionCount = 100;
    //    bool isConnected = false;
    float offset = 0.1f;
    Vector3 diretion, initialPoint;
    void initializeDynamicRope()
    {
        i = 0;
        initialPoint = gunTip.position;
        diretion = (grapplePoint - gunTip.position);
        DrawDynamicRope();
    }
    void DrawDynamicRope()
    {
        lineRenderer.positionCount = i + 1;
        while (i < positionCount)
        {
            Vector3 noice = new Vector3(Random.Range(-.5f, .5f) * offset, Random.Range(-.5f, .5f) * offset, Random.Range(-.5f, .5f) * offset);
            lineRenderer.SetPosition(i, gunTip.position + diretion * i / diretion.magnitude + noice);
            i++;
            Invoke(nameof(DrawDynamicRope), 0.005f);
            return;
        }
        //isConnected = true;
    }
    #endregion

    void LaserTrap()
    {
        float offset = 100;
        lineRenderer.positionCount = positionCount;
        for (int i = 0; i < positionCount; i++)
        {
            Vector3 noice = new Vector3(Random.Range(-.5f, .5f) * offset, Random.Range(-.5f, .5f) * offset, Random.Range(-.5f, .5f) * offset);
            lineRenderer.SetPosition(i, gunTip.position + noice);
        }

    }

}
