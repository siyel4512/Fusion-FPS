using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class NetworkCallback : MonoBehaviour, INetworkRunnerCallbacks
{
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
        
    }

    // 플레이어가 나갔을때
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    // 데이터를 받았을때
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    // 씬로드가 완료되었을때
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    // 씬로드가 시작되었을때
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
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
