using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerAll : MonoBehaviourPun
{
    public PlayerSO playerSo;
    public int curWpNum;

    CharacterController characterCtr;
    Transform cam;          // 카메라 위치
    Vector3 moveDir;        // 방향벡터
    PhotonView pv;

    public Animator ani;
    public GameObject playerModel; // 모델
    public Rigidbody[] ragdollSet;

    float gravity = -9.8f;  // 중력값

    [System.Serializable]
    struct PlayerAction
    {
        public bool isAim;     // 조준 확인
        public bool isShift;   // 시프트 클릭 확인
        public bool isBack;    // 뒤로 이동
        public bool isJump;    // 점프 확인
        public bool isMove;    // 움직임 확인
        public bool isDie;     // 죽음 확인
        public bool isAimWalk; // 에임 걷기
        public bool isAimBackWalk; // 에임 뒤로
    }
    PlayerAction playerAction;

    public bool Aim { get { return playerAction.isAim; } set { playerAction.isAim = value; } }
    public bool GetShift { get { return playerAction.isShift; } }
    public bool GetJump { get { return playerAction.isJump; } }
    public bool GetMove { get { return playerAction.isMove; } }

    //public PlayerSO GetHP { get { return playerSo; } }

    private void Awake()
    {
        playerSo = new PlayerSO();
    }

    void Start()
    {
        characterCtr = GetComponent<CharacterController>();
        pv = GetComponent<PhotonView>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void Update()
    {
        curWpNum = playerSo.weaponSO.weaponNum; // 소지중인 총 넘버 변경

        //Debug.Log(playerSo.weaponSO.curBullet);
        if (pv.IsMine)
        {
            // UI에 띄울 hp
            UIManager.instance.MyHPText((int)playerSo.hp);

            Move();

            // 땅 착지
            if (GroundCheck())
            {
                playerAction.isJump = false;
            }

            // 플레이어 죽음
            if (playerSo.hp <= 0 && !playerAction.isDie)
            {
                UIManager.instance.playerCnt--;

                pv.RPC("DieAni", RpcTarget.All);
            }
        }
        else
        {
            return;
        }

    }

    [PunRPC]
    void DieAni()
    {
        playerAction.isDie = true;
        ani.enabled = false;            // 애니메이션 비활성화
        ani.gameObject.GetComponent<PlayerBodyRot>().enabled = false;   // 상체 이동 비활성화
        characterCtr.enabled = false;   // 캐릭터 컨트롤러 비활성화

        // 고정시켰던 position, rotation 비활성화
        for (int i = 0; i < ragdollSet.Length; i++)
        {
            ragdollSet[i].constraints = RigidbodyConstraints.None;
        }      
    }

    // 불안정한 그라운드 체크로.. 대체 함수 사용 (캐릭터 컨트롤러와 같은 오브젝트에 있으면 땅 인식을 못함)
    bool GroundCheck()
    {
        if(characterCtr.isGrounded)
        {
            return true;
        }

        // 레이를 이용해 땅 레이어 검사 
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * 0.11f, Color.red); // (발사위치, 발사방향 및 길이, 색)
        return Physics.Raycast(ray, 0.11f, LayerMask.GetMask("Plane"));
    }

    void Move()
    {
        // 땅 체크
        if (GroundCheck() && playerSo.hp > 0)
        {
            float z = Input.GetAxisRaw("Horizontal");
            float x = Input.GetAxisRaw("Vertical");

            ActionValue(x, z);

            moveDir = new Vector3(z, 0, x);

            // Shift로 전방 이동속도 증가
            playerSo.moveSpeed = ActionSpeed(x, z);
            
            // 점프
            Jump();

            // 애니메이션 관리
            pv.RPC("AniControl", RpcTarget.All, x, z);

            // 플레이어 회전
            Rot(x, z);

        }
        else
        {
            // 중력부여
            moveDir.y += gravity * Time.deltaTime;
        }

        // Move는 절대값(월드좌표)로만 이동하니 카메라가 바라보는 방향으로는 로컬좌표가 필요
        characterCtr.Move(transform.TransformDirection(moveDir.normalized) * playerSo.moveSpeed * Time.deltaTime); // TransformDirection가 로컬 -> 월드로 변환해줌
    }

    // 모션에 따른 수치변경(애니메이션 관리) ■■ 애니메이션 코드에서 단축 많이 시켰어 굿굿 ■■
    void ActionValue(float x, float z)
    {
        playerAction.isMove = ((x > 0 || z != 0) && !playerAction.isShift && !Aim && !playerAction.isJump) ? true : false;
        playerAction.isBack = (x < 0 && !Aim && !playerAction.isJump) ? true : false;

        playerAction.isAimWalk = ((x > 0 || z != 0) && Aim && !playerAction.isJump) ? true : false;
        playerAction.isAimBackWalk = (x < 0 && Aim && !playerAction.isJump) ? true : false;
    }

    // 플레이어 회전
    void Rot(float x, float z)
    {
        // 카메라가 바라보는 방향으로 플레이어도 바라보게.. (없으면 플레이어가 통통 튐)
        transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

        if (z != 0 && x > 0 && !Aim)
        {
            // 방향벡터로 회전각 구하기
            float angle = Mathf.Rad2Deg * (Mathf.Atan2(z, x));

            playerModel.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);
        }
        else
        {
            // 상하좌우 이동시엔 정면 바라봄
            playerModel.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }

        /* tip)
        Quaternion.Lerp(움직일 오브젝트 회전값, 바라볼 회전값, 속도)

        Quaternion s = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(moveDir.x, 0, moveDir.z)), rotSpeed * Time.deltaTime);
        */
    }

    // 점프 함수
    void Jump()
    {
        // 스페이스바 터치
        if (Input.GetKeyDown(KeyCode.Space) && !playerAction.isJump)
        {
            moveDir.y = playerSo.jumpPower;

            playerAction.isJump = true;
            Aim = false;
        }
    }

    // 동작별 이동속도 (기본이속, 달리기, 뒤로)
    float ActionSpeed(float x, float z)
    {
        // 기본 속도
        playerSo.moveSpeed = 2.5f;

        playerAction.isShift = (Input.GetKey(KeyCode.LeftShift) && !Aim && x > 0) ? true : false;

        // 달리기
        if (playerAction.isShift)
        {
            playerSo.moveSpeed = 3.75f;
        }

        // 뒤로 걷기
        if (moveDir.z < 0)
        {
            playerSo.moveSpeed = 1.7f;
        }

        return playerSo.moveSpeed;
    }

    // 애니메이션 관리 함수 (+ 움직임 변수 체크)
    [PunRPC]
    void AniControl(float x, float z)
    {
        ani.SetBool("Walk", playerAction.isMove);
        ani.SetBool("Run", playerAction.isShift);
        ani.SetBool("BackMove", playerAction.isBack);
        ani.SetBool("AimWalk", playerAction.isAimWalk);
        ani.SetBool("AimBackWalk", playerAction.isAimBackWalk);
        ani.SetBool("AimIdle", Aim);

        // 점프
        if (playerAction.isJump)
        {
            ani.SetTrigger("Jump");
        }

        // 움직임
        if (z != 0 || x != 0)
        {
            ani.SetBool("Idle", false);
            ani.SetBool("AimIdle", false);
        }
        // 멈춤
        else
        {
            if (Aim)
            {
                ani.SetBool("Idle", false);
            }
            else
            {
                ani.SetBool("Idle", true);
            }
        }
    }
}

// player 상속 위한 클래스
public class MostPlayer : MonoBehaviour
{
    protected PlayerAll playerScript;

    void Awake()
    {
        playerScript = GetComponent<PlayerAll>();
    }
}

