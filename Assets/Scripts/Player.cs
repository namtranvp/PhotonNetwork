using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PhotonView view;
    Rigidbody rb;

    [SerializeField] float speed = 5f;
    [SerializeField] GameObject bullet;

    [SerializeField] int heal = 100;
    [SerializeField] float radius = 10;

    GameController gameController;
    void Start()
    {
        view = GetComponent<PhotonView>();
        //rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!view.IsMine) return;


        if (Input.GetMouseButtonDown(0))
        {
            gameController.ClickDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            gameController.ClickUp();
        }

        /*float v = Input.GetAxisRaw("Vertical") * speed;
        float h = Input.GetAxisRaw("Horizontal") * speed;

        rb.velocity = new Vector3(h, rb.velocity.y, v);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }*/
    }

    public void SetGameControl(GameController controller)
    {
        gameController = controller;
    }

    private void OnTriggerEnter(Collider other)
    {
        heal -= 10;
    }

    private void Shoot()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.CompareTag("Player") && hitCollider.transform != transform)
            {
                Vector3 dir = (hitCollider.transform.position - transform.position).normalized;
                view.RPC(nameof(SpawnBullet), RpcTarget.AllViaServer, dir);
            }
        }
    }

    [PunRPC]
    void SpawnBullet(Vector3 direction)
    {
        Debug.Log("spawn bullet");
        /*GameObject b = Instantiate(bullet, transform.position, Quaternion.EulerRotation(direction));
        b.GetComponent<Rigidbody>().velocity = direction * 55f;*/
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
