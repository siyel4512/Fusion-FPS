using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class ExampleCharacter : NetworkBehaviour
{
    [SerializeField]
    private NetworkCharacterControllerPrototype cc;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private TMP_Text nickNameText;

    // 서버에 의해 변경되는(오직 서버만이 변경가능) / 동기화되는 변수
    //[Networked]
    //NetworkString<_16> NickName { get; set; } // 길이가 최대 16인 string

    // callback 만들기
    [Networked(OnChanged = nameof(OnNickNameChanged))]
    NetworkString<_16> NickName { get; set; } // 길이가 최대 16인 string

    [Networked]
    public NetworkButtons PrevButtons { get; set; }

    private NetworkButtons buttons; // 더 빠른 동기화?.... 없어도 된다...
    private NetworkButtons pressed; // 누름
    private NetworkButtons released; // 때짐

    private Vector2 inputDir; // 이동 방향
    private Vector3 moveDir;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
        {
            Destroy(cam.gameObject);
            return;
        }

        cam.gameObject.SetActive(true);
        RPC_SendNickName("Player_" + Random.Range(0, 10).ToString());

        GUI.enabled = false;
    }

    public override void Render()
    {
        // input에 대한 관한이 있는지 먼저 확인
        if (!Object.HasInputAuthority)
        {
            return;
        }

        cam.transform.rotation = Quaternion.Euler(0, NetworkCallback.NC.Yaw, 0); // 촤/우
        cam.transform.localRotation = Quaternion.Euler(NetworkCallback.NC.Pitch, cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z); // 위/아래
    }

    public override void FixedUpdateNetwork()
    {
        buttons = default;

        // network callback 함수인 OnInput()의 값을 가져온다.
        if (GetInput<NetworkInputData>(out var input))
        {
            buttons = input.buttons;
        }

        pressed = buttons.GetPressed(PrevButtons); // 눌러진거 감지
        released = buttons.GetReleased(PrevButtons); // 때진거 감지

        PrevButtons = buttons;

        inputDir = Vector2.zero;

        // 눌려졌는지 아닌지
        if (buttons.IsSet(Buttons.forward))
        {
            inputDir += Vector2.up; // 이동
        }
        if (buttons.IsSet(Buttons.back))
        {
            inputDir -= Vector2.up; // 이동
        }
        if (buttons.IsSet(Buttons.right))
        {
            inputDir += Vector2.right; // 이동
        }
        if (buttons.IsSet(Buttons.left))
        {
            inputDir -= Vector2.right; // 이동
        }

        if (pressed.IsSet(Buttons.jump))
        {
            cc.Jump();
        }

        moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;

        cc.Move(moveDir);

        transform.rotation = Quaternion.Euler(0, (float)input.yaw,0);
    }


    // callback 함수
    public static void OnNickNameChanged(Changed<ExampleCharacter> changed)
    {
        changed.Behaviour.SetNickName();
    }

    // callback할 실제함수
    public void SetNickName()
    {
        nickNameText.text = NickName.Value;
    }

    
    // RpcSources.InputAuthority = input 권환을 가진 사람이 (보내는 사람?)
    // RpcTarget.StateAuthority = 주로 서버가 그대상임 (받는 사람?)
    // 즉 InputAuthority (플레이어/클라이언트) 권한을 가진 사람이 StateAuthority (서버)를 가진 사람에게 RPC를 보낸다는 의미
    
    // 해당 RPC가 호출되면 [Networked]로 선언된 NickName 값을 서버가 수정한다.
    // 수정 후 callback함수로 사용될 OnNickNameChanged()가 호출되고 SetNickName()도 호출된다.
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SendNickName(NetworkString<_16> message)
    {
        NickName = message;
    }
}
