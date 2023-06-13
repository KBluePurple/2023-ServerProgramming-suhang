using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    private Coroutine _coSkillCooltime;
    private bool _moveKeyPressed;

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }

        base.UpdateController();
    }

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (_moveKeyPressed)
        {
            State = CreatureState.Moving;
            return;
        }

        if (_coSkillCooltime == null && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Skill !");

            var skill = new C_Skill
            {
                Info = new SkillInfo
                {
                    SkillId = 2
                }
            };
            Managers.Network.Send(skill);

            _coSkillCooltime = StartCoroutine("CoInputCooltime", 0.2f);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var skill = new C_Skill
            {
                Info = new SkillInfo
                {
                    SkillId = 3
                }
            };
            Managers.Network.Send(skill);
        }
    }

    private IEnumerator CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
    }

    // 키보드 입력
    private void GetDirInput()
    {
        _moveKeyPressed = true;

        if (Input.GetKey(KeyCode.W))
            Dir = MoveDir.Up;
        else if (Input.GetKey(KeyCode.S))
            Dir = MoveDir.Down;
        else if (Input.GetKey(KeyCode.A))
            Dir = MoveDir.Left;
        else if (Input.GetKey(KeyCode.D))
            Dir = MoveDir.Right;
        else
            _moveKeyPressed = false;
    }

    protected override void MoveToNextPos()
    {
        if (_moveKeyPressed == false)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();
            return;
        }

        var destPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.CanGo(destPos))
            if (Managers.Object.FindCreature(destPos) == null)
                CellPos = destPos;

        CheckUpdatedFlag();
    }

    protected override void CheckUpdatedFlag()
    {
        if (_updated)
        {
            var movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }
}