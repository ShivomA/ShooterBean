using UnityEngine;

public class GrappleGun : MonoBehaviour
{
    public LayerMask grappleableLayer, grappleObjectLayer;
    public Transform cameraTransform, player, gunTip;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (player == null)
            player = FindObjectOfType<MovementController>().transform;
        if (cameraTransform == null)
            cameraTransform = FindObjectOfType<CameraController>().transform;
    }

    private void Update()
    {
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
    private Vector3 grapplePoint, currentGrapplePosition;
    private SpringJoint joint;
    float maxDistance = 500;
    bool isGrapped = false;
    void StartGrapple()
    {
        if(Physics.Raycast(cameraTransform.position + cameraTransform.forward, cameraTransform.forward, out hit, maxDistance, grappleableLayer))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            
            int layer = hit.collider.gameObject.layer;
            if (grappleObjectLayer == (grappleObjectLayer | (1 << layer)))
                isGrapped = true;
            else isGrapped = false;
            

            if (isGrapped)
            {
                joint.connectedBody = hit.collider.attachedRigidbody;
                float distanceFromGrapplePoint = Vector3.Distance(player.position, grapplePoint);
                joint.maxDistance = distanceFromGrapplePoint * 0.4f;
                joint.minDistance = 0;
            }
            else
            {
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;
                float distanceFromGrapplePoint = Vector3.Distance(player.position, grapplePoint);
                joint.maxDistance = distanceFromGrapplePoint * 0.6f;
                joint.minDistance = distanceFromGrapplePoint * 0.1f;
            }

            joint.spring = 5f;
            joint.damper = 0f;
            joint.massScale = 1f;
            
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
    bool isConnected = false;
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
        isConnected = true;
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
