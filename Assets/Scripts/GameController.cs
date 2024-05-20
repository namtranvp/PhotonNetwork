using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class GameController : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    [SerializeField] Transform ballParent;
    [SerializeField] Ball ballPre;
    [SerializeField] Color[] ballcolors;

    Vector3 startPoint, endPoint;
    Ball mainBall;
    private List<Ball> balls = new List<Ball>();

    PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitGame();
        }


        Player p = PhotonNetwork.Instantiate(PlayerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Player>();
        p.SetGameControl(this);
    }


    public void ClickDown()
    {
        Debug.Log("Click down");
        startPoint = Input.mousePosition;

        for (int i = 0; i < balls.Count; i++)
        {
            Vector3 position = balls[i].transform.position;
            Quaternion rot = balls[i].transform.rotation;
            view.RPC(nameof(SetZeroVelocityBalls), RpcTarget.AllViaServer, i, position, rot);
        }
    }

    public void ClickUp()
    {
        Debug.Log("Click up");
        endPoint = Input.mousePosition;

        Vector3 direction = CalculateDirection(endPoint, startPoint);
        view.RPC(nameof(PushMainBall), RpcTarget.AllViaServer, direction);

        /*if (PhotonNetwork.IsMasterClient)
        {
            ApplyForceToMainBall(direction);
        }
        else
        {
            view.RPC(nameof(RequestPushMainBall), RpcTarget.MasterClient, direction);
        }*/
    }

    [PunRPC]
    public void InitGame()
    {
        view.RPC(nameof(SpawnMainBall), RpcTarget.AllViaServer);

        for (int i = 0; i < 7; i++)
        {
            view.RPC(nameof(SpawnBall), RpcTarget.AllViaServer, i);
        }
    }

    [PunRPC]
    public void SpawnMainBall()
    {
        mainBall = Instantiate(ballPre);
        mainBall.SetControl(this);
        mainBall.transform.SetParent(ballParent);
        balls.Add(mainBall);
    }

    [PunRPC]
    public void SpawnBall(int colorIndex)
    {
        Ball b = Instantiate(ballPre);
        b.SetColor(ballcolors[colorIndex]);
        b.transform.SetParent(ballParent);
        balls.Add(b);
    }

    [PunRPC]
    public void SetZeroVelocityBalls(int i, Vector3 pos, Quaternion rot)
    {
        balls[i].SetZeroVel(true);
        balls[i].transform.position = pos;
        balls[i].transform.rotation = rot;
    }

    [PunRPC]
    public void PushMainBall(Vector3 force)
    {
        // set all ball velocity zero
        foreach (var ball in balls)
            ball.SetZeroVel(false);

        mainBall.AddForce(force * 55f);
    }
    [PunRPC]
    public void RequestPushMainBall(Vector3 force)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ApplyForceToMainBall(force);
        }
    }

    private void ApplyForceToMainBall(Vector3 force)
    {
        mainBall.AddForce(force * 22f);
        view.RPC(nameof(SyncBallPosition), RpcTarget.Others, mainBall.transform.position, mainBall.GetVelocity());
    }
    [PunRPC]
    public void SyncBallPosition(Vector3 position, Vector3 velocity)
    {
        mainBall.transform.position = position;
        mainBall.SetVel(velocity);
    }

    private Vector3 CalculateDirection(Vector3 start, Vector3 end)
    {
        Vector3 startWorld = Camera.main.ScreenToWorldPoint(new Vector3(start.x, start.y, Camera.main.nearClipPlane));
        Vector3 endWorld = Camera.main.ScreenToWorldPoint(new Vector3(end.x, end.y, Camera.main.nearClipPlane));
        Vector3 direction = (endWorld - startWorld).normalized;

        direction.y = 0;

        return direction;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Gửi trạng thái trò chơi hiện tại cho người chơi mới
            int indexBall = 0;
            foreach (Ball ball in balls)
            {
                Vector3 position = ball.transform.position;
                Vector3 vel = ball.GetVelocity();
                //int colorIndex = System.Array.IndexOf(ballcolors, ball.GetComponent<Renderer>().material.color);
                view.RPC(nameof(SyncBall), newPlayer, position, vel, indexBall);

                indexBall++;
            }
        }

    }
    [PunRPC]
    public void SyncBall(Vector3 position, Vector3 velocity, int colorIndex)
    {
        Ball b = Instantiate(ballPre, position, Quaternion.identity);
        if (colorIndex > 0)
            b.SetColor(ballcolors[colorIndex - 1]);
        else
            mainBall = b;
        b.transform.SetParent(ballParent);
        b.SetVel(velocity);
        balls.Add(b);
    }
}
