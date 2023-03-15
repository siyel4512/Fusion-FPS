using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEditor.UIElements;

public class ExampleCharacter : NetworkBehaviour
{
    [SerializeField]
    private NetworkCharacterControllerPrototype cc;

    [SerializeField]
    private Camera cam;

    // 서버에 의해 변경되는 / 동기화되는 변수
    [Networked]
    NetworkString<_16> NickName { get; set; } // 길이가 최대 16인 string

    [Networked]
    public NetworkButtons PrevButtons { get; set; }

    private NetworkButtons buttons; // 더 빠른 동기화?.... 없어도 된다...
    private NetworkButtons pressed; // 누름
    private NetworkButtons released; // 때짐

    private Vector2 inputDir; // 이동 방향
    private Vector3 moveDir;

    public override void Render()
    {
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

        moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;

        cc.Move(moveDir);

        transform.rotation = Quaternion.Euler(0, (float)input.yaw,0);
    }
}
