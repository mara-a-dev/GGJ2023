using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    // Settings
    public float MoveSpeed = 5;
    public float SteerSpeed = 180;
    public float BodySpeed = 5;
    public int Gap = 10;

    // References
    public GameObject BodyPrefab;

    // Lists
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();

    private PlayerStats playerStats;

    private Coroutine dyingCoroutine, spawningCoroutine;
    private float damage = 1;
    private bool playerIsAlive = true;
    private float posYStart = 6;
    private float posYEnd = 0;
    private float dyingDuration = 1;

    private float minFloorRange = -2000;
    private float maxFloorRange = 2000;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    void Start()
    {
        GrowSnake();

    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsAlive)
        {
            // Move forward
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;

            // Steer
            float steerDirection = Input.GetAxis("Horizontal"); // Returns value -1, 0, or 1
            transform.Rotate(Vector3.up * steerDirection * SteerSpeed * Time.deltaTime);

            // Store position history
            PositionsHistory.Insert(0, transform.position);

            // Move body parts
            int index = 0;
            foreach (var body in BodyParts)
            {
                Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];

                // Move body towards the point along the snakes path
                Vector3 moveDirection = point - body.transform.position;
                body.transform.position += moveDirection * BodySpeed * Time.deltaTime;

                // Rotate body towards the point along the snakes path
                body.transform.LookAt(point);

                index++;
            }
        }
    }

    private void GrowSnake()
    {
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Rock")
        {
            playerIsAlive = false;
            LoseLife();
        }
    }

    private void LoseLife()
    {
        Debug.Log("You hit a rock ... lost 1 life");
        //TODO:  show you lost life text and UI



        //if you have extra roots growing then respawn to another .... if you dont then show GAME OVER

        if (playerStats.Health > 0)
        {
            playerStats.TakeDamage(damage);// minus life 
            if (dyingCoroutine != null)
            {
                StopCoroutine(dyingCoroutine);
            }
            dyingCoroutine = StartCoroutine(DyingRoot(true));  // killing the root
        }
        else
        {
            if (dyingCoroutine != null)
            {
                StopCoroutine(dyingCoroutine);
            }
            dyingCoroutine = StartCoroutine(DyingRoot(false));  // killing the root

            //YOU ARE DEAD GAME OVER
            GameManager.Instance.GameOver();
        }
    }

    private IEnumerator DyingRoot(bool respawnStatus)
    {
        float progress = 0;
        while (progress < 1f)
        {
            yield return null;
            progress += Time.deltaTime / dyingDuration;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(posYStart, posYEnd, progress), transform.position.z);
        }

        if (respawnStatus) Respawn();
    }

    private void Respawn()
    {
        //TODO cam down

        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
        }
        spawningCoroutine = StartCoroutine(SpawningRoot());

        transform.position = new Vector3(Random.Range(minFloorRange, maxFloorRange), transform.position.y, Random.Range(minFloorRange, maxFloorRange));
        playerIsAlive = true;
    }

    private IEnumerator SpawningRoot()
    {
        float progress = 0;
        while (progress < 1f)
        {
            yield return null;
            progress += Time.deltaTime / dyingDuration;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(posYEnd, posYStart, progress), transform.position.z);
        }
    }
}