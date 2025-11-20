using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Drone : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Vision Settings")]
    public float viewDistance = 5f;
    public float viewAngle = 60f;
    public int rayCount = 30;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    public Transform visionCone;

    [Header("Eye")]
    public SpriteRenderer eyeSprite;
    public Color normalEyeColor = Color.white;
    public Color alertEyeColor = Color.red;

    [Header("Sound")]
    public AudioClip droneAlertSound;
    private AudioSource droneAudioSource;

    [Header("Missile")]
    public GameObject missilePrefab;
    public Transform missileSpawnPoint;

    private bool missileLaunched = false;
    private Transform player;
    private Mesh visionMesh;
    private Camera mainCam;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        visionMesh = new Mesh();
        if (visionCone != null)
            visionCone.GetComponent<MeshFilter>().mesh = visionMesh;

        mainCam = Camera.main;

        if (eyeSprite != null)
            eyeSprite.color = normalEyeColor;

        droneAudioSource = GetComponent<AudioSource>();
        if (droneAudioSource == null) droneAudioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        MoveLeft();
        ScanAndRenderCone();
    }

    void MoveLeft()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        if (viewportPos.x < -0.1f)
        {
            Destroy(gameObject);
        }
    }
    //------------------FIX DONE-----------
    void ScanAndRenderCone()
    {
        if (visionCone == null) return;

        bool spotted = false;
        float halfAngle = viewAngle / 2f;

        Vector3[] vertices = new Vector3[rayCount + 1];
        int[] triangles = new int[(rayCount - 1) * 3];

        vertices[0] = Vector3.zero;
        int triangleIndex = 0;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -halfAngle + (viewAngle / (rayCount - 1)) * i;
            Vector2 dir = DirFromAngle(angle, -transform.up);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewDistance, obstacleMask | playerMask);

            float dst = viewDistance;

            if (hit)
            {
                dst = hit.distance;

                if (hit.collider.CompareTag("Player"))
                {
                    var pc = hit.collider.GetComponent<PlayerController>();
                    if (pc == null || pc.isMoving || !pc.isHuggingWall)
                        spotted = true;
                }
            }

            Vector3 vertex = transform.InverseTransformPoint(transform.position + (Vector3)(dir * dst));
            vertices[i + 1] = vertex;

            Debug.DrawRay(transform.position, dir * dst, spotted ? Color.red : Color.yellow);

            if (i < rayCount - 1)
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = i + 1;
                triangles[triangleIndex++] = i + 2;
            }
        }

        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();

        if (spotted)
        {
            if (eyeSprite != null)
                eyeSprite.color = alertEyeColor;

            if (!missileLaunched && missilePrefab != null && missileSpawnPoint != null)
            {
                LaunchMissile();
                missileLaunched = true;
            }
        }
        else if (eyeSprite != null)
        {
            eyeSprite.color = normalEyeColor;
        }
    }

    void LaunchMissile()
    {
        if (droneAlertSound != null) droneAudioSource.PlayOneShot(droneAlertSound);

        GameObject missile = Instantiate(missilePrefab, missileSpawnPoint.position, Quaternion.identity);
        HomingMissile missileScript = missile.GetComponent<HomingMissile>();
        if (missileScript != null)
        {
            missileScript.SetTarget(player);
        }
    }

    Vector2 DirFromAngle(float angleDegrees, Vector2 facingDirection)
    {
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(
            facingDirection.x * Mathf.Cos(angleRad) - facingDirection.y * Mathf.Sin(angleRad),
            facingDirection.x * Mathf.Sin(angleRad) + facingDirection.y * Mathf.Cos(angleRad)
        ).normalized;
    }


    //DONT DEL---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
