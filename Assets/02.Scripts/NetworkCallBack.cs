using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

// 참고 영상 : https://www.youtube.com/watch?v=z3VG_nDBRU0

public class Player
{
    public PlayerRef playerRef;
    public NetworkObject playerObject;

    public Player()
    {

    }

    public Player(PlayerRef player, NetworkObject obj)
    {
        playerRef = player;
        playerObject = obj;
    }
}

public class NetworkCallback : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkCallback NC;
    public NetworkRunner runner; // 전체적으로 네트워크 통신을 담당
    public List<Player> runningPlayers = new List<Player>(); // 플레이어 그 자체의 값을 의미한다.

    public NetworkPrefabRef playerPrefab; // fusion에서 사용하는 gameobject 같은 개념

    private float yaw;
    public float Yaw
    { 
        get 
        { 
            return yaw; 
        }
        set 
        { 
            yaw = value;
            
            if (yaw < 0)
            {
                yaw = 360f;
            }
        } 
    }


    private float pitch;
    public float Pitch
    { 
        get
        {
            return pitch;
        }
        set
        {
            pitch = value;

            pitch = Mathf.Clamp(pitch, -80, 80);
        } 
    }

    private void Awake()
    {
        if (NC == null)
        {
            NC = this;
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true; // OnInput()를 사용하기 위해서는 ProvideInput을 true로 설정해야한다.
        }
        else if (NC != this)
        {
            Destroy(gameObject);
        }
    }
    
    // 직접적으로 서버를 열게함
    private async void RunGame(GameMode mode)
    {
        // 방생성/게임 시작시 필요한 설정 세팅
        var gameArgs = new StartGameArgs();
        gameArgs.GameMode = mode;
        gameArgs.SessionName = "Test Room"; // 생성할 방이름
        gameArgs.PlayerCount = 10; // 참석할 수 있는 플레이어 수
        //gameArgs.Scene = 0; // runner.SetActiveScene(0); 와 비슷한 형태라고 생각하면된다.
        gameArgs.SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(); // SceneObjectProvider가 StartGameArgs에 없음 대신 SceneManager를 사용하면 된다.....

        await runner.StartGame(gameArgs);

        runner.SetActiveScene(1); // 로드할 씬 설정
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 400, 200), "Host"))
        {
            RunGame(GameMode.Host);
        }

        if (GUI.Button(new Rect(0, 200, 400, 200), "Client"))
        {
            RunGame(GameMode.Client);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 입력 받음
        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");
    }

    // 연결 성공
    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    // 연결 실패
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    // 연결 요청
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    // 외부 플러그인 인증
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    // 클라이언트 기준 서버와 연결을 끊었을때
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        
    }

    // 호스트가 바뀌었을때
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    // 해당 플레이어의 입력값을 받을때
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var myInput = new NetworkInputData();

        // 이동
        myInput.buttons.Set(Buttons.forward, Input.GetKey(KeyCode.W)); // W 키를 눌렸을때 Buttons.forward 즉 0값이 set된다.
        myInput.buttons.Set(Buttons.back, Input.GetKey(KeyCode.S));
        myInput.buttons.Set(Buttons.right, Input.GetKey(KeyCode.D));
        myInput.buttons.Set(Buttons.left, Input.GetKey(KeyCode.A));
        myInput.buttons.Set(Buttons.jump, Input.GetKey(KeyCode.Space));

        // 회전
        myInput.pitch = Pitch;
        myInput.yaw = Yaw;

        input.Set(myInput);
    }

    // 해당 플레이어의 입력값을 받지 못할때
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    // 플레이어가 들어왔을때
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // 씬이 로드되었을때 플레이어 재생성
        // 서버가 아닐때
        if (!this.runner.IsServer)
        {
            return;
        }

        runningPlayers.Add(new Player(player, null));

        // 서버만 해당작업을 수행해야하기 때문에...
        foreach (var players in runningPlayers)
        {
            if (players.playerObject != null)
            {
                continue;
            }

            var obj = this.runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, players.playerRef); // 생성시 input 권한을 줌

            players.playerObject = obj;

            var cc = obj.GetComponent<CharacterController>();
            cc.enabled = false;
            obj.transform.position = new Vector3(0, 10, 0);
            cc.enabled = true;
        }
    }

    // 플레이어가 나갔을때
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
        {
            return;
        }

        foreach (var players in runningPlayers)
        {
            if (players.playerRef.Equals(player))
            {
                runner.Despawn(players.playerObject);
                runningPlayers.Remove(players);
            }
            break;
        }
    }

    // 데이터를 받았을때
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    // 씬로드가 완료되었을때
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (!this.runner.IsServer)
        {
            return;
        }

        foreach (var players in runningPlayers)
        {
            var obj = this.runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, players.playerRef); // 생성시 input 권한을 줌

            players.playerObject = obj;

            var cc = obj.GetComponent<CharacterController>();
            cc.enabled = false;
            obj.transform.position = new Vector3(0, 10, 0);
            cc.enabled = true;
        }
    }

    // 씬로드가 시작되었을때
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        if (!this.runner.IsServer)
        {
            return;
        }
    }

    // 세션(방) 리스트 업데이트
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

   
}
