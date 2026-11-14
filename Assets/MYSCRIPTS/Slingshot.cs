using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public GameObject aimIndicator;
    public GameObject projectilePrefab;
    public float velocityMultiplier = 10f;
    
    [SerializeField] private BoxCollider ground;

    private GameObject projectile;
    private float minimumProjectileY = float.NegativeInfinity;
    private Vector3 launchPos;
    private bool aimingMode;
    private Vector3 mouseStartPos;

    void Awake()
    {
        Transform launchPoint = transform.Find("LaunchPoint");
        
        launchPos = launchPoint.position;

        aimIndicator = launchPoint.Find("AimIndicator").gameObject;
        aimIndicator.SetActive(false);
    }
    void OnMouseEnter()
    {
        aimIndicator.SetActive(true);
    }
    
    void OnMouseExit()
    {
        aimIndicator.SetActive(false);
    }

    void OnMouseDown()
    {
        if (aimingMode) return;
        
        Camera cam = Camera.main;
        if (cam == null) return;
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = launchPos.z - cam.transform.position.z;

        mouseStartPos = cam.ScreenToWorldPoint(mousePos);
        
        aimingMode = true;

        projectile = Instantiate(projectilePrefab);
        projectile.transform.position = launchPos;
        
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        if (ground != null && projectile.TryGetComponent<SphereCollider>(out var sphere))
        {
            Bounds bounds = sphere.bounds;
            minimumProjectileY = ground.bounds.max.y + projectile.transform.position.y - bounds.min.y;
        }
    }

    private void Update()
    {
        if (!aimingMode) return;
        
        Camera cam = Camera.main;
        if (cam == null) return;
        
        Vector3 mousePos2D = Input.mousePosition;
        
        mousePos2D.z = launchPos.z - cam.transform.position.z;
        
        Vector3 mousePos3D = cam.ScreenToWorldPoint(mousePos2D);
        
        Vector3 mouseDelta = mousePos3D - mouseStartPos;
        mouseDelta.z = 0f;
        
        float maxMagnitude = GetComponent<SphereCollider>().radius;
        
        mouseDelta = Vector3.ClampMagnitude(mouseDelta, maxMagnitude);
        
        mouseDelta.y = Mathf.Max(mouseDelta.y, minimumProjectileY - launchPos.y);
        projectile.transform.position = launchPos + mouseDelta;

        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            
            projectile.GetComponent<Projectile>().Launch(mouseDelta, velocityMultiplier);

            projectile = null;
        }

    }
}
