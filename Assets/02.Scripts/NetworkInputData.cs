using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// 효율적인 네트워킹을 하기 위해서 지정된 형식이 존재한다.
// 그래서 비트연산을 사용함 (1byte는 8bit 이다)
// 아래의 enum은 비트 연산을 쉽게 하기 위해서...
// 사용가능한 데이터 타입도 별도로 존재한다.
// 참고 페이지 https://doc.photonengine.com/ko-kr/fusion/current/manual/inetworkstruct
enum Buttons
{
    forward = 0, // 0000 0000
    back = 1, // 0000 0001
    right = 2, // 0000 0010
    left = 3, // 0000 0011
    jump = 4,
}

// class 형태가 아닌 struct 형태로 사용되야 한다.
public struct NetworkInputData : INetworkInput
{
    public NetworkButtons buttons;

    // 진행방향이 x축(정면으로 향함) 일때 x축 기준으로 회전 => Roll
    // 진행방향이 x축 일때 y축 기준으로 회전 => Pitch
    // 진행방향이 x축(아래 방향으로 향함) 일때 z축 기준으로 회전 => Yaw
    // 참고 블로그 https://blog.naver.com/milkysc/221754450137
    public Angle yaw;
    public Angle pitch;
}
