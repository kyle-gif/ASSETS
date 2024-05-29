using UnityEngine; //유니티엔진 불러오기
using System.Collections; // 시스템 어쩌구 불러오기
using System.Threading; // 시스템 어쩌구 불러오기
using System.Threading.Tasks; // 시스템 어쩌구 불러오기
//줄바꿈
public class HeroKnight : MonoBehaviour { // 모노비헤이비어를 상속한 HeroKnight 클래스
//줄바꿈
    [SerializeField] float      m_speed = 3.0f;  // ㄹㅇ 진짜 속도 변수 ( SerializeFeild는 private을 유지하면서도 에디터에서 쉽게 바꿔줄수 있음 )
    [SerializeField] float      m_jumpForce = 7.5f; // ㄹㅇ 진짜 점프하는 속도
    [SerializeField] float      m_rollForce = 6.0f; // ㄹㅇ 진짜 구르는 속도
    [SerializeField] bool       m_noBlood = false; // 유혈이 있을지 없을지 정하는 변수
    [SerializeField] GameObject m_slideDust; // GameObject를 private으로 호출해서 슬라이딩할때 먼지 출력
//줄바꿈
    private Animator            m_animator; //에니메이터 호출변수
    private Rigidbody2D         m_body2d; //리지드바디2D 호출변수
    private Sensor_HeroKnight   m_groundSensor; // 바닥과의 거리측정변수
    private Sensor_HeroKnight   m_wallSensorR1; // 마찬가지로 벽
    private Sensor_HeroKnight   m_wallSensorR2; // 마찬가지로 벽
    private Sensor_HeroKnight   m_wallSensorL1; // 마찬가지로 벽 
    private Sensor_HeroKnight   m_wallSensorL2; // 마찬가지로 벽
    private bool                m_isWallSliding = false; //벽에 슬라이딩하는지
    private bool                m_grounded = false; // 땅에 닿았는지
    private bool                m_rolling = false; // 구르고 있는지
    private int                 m_facingDirection = 1; // 바라보고 있는 방향. 1이 정면
    private int                 m_currentAttack = 0; // 공격 스택
    private float               m_timeSinceAttack = 0.0f; // 공격 스택을 끊을 시간을 측정하는 변수
    private float               m_delayToIdle = 0.0f; // 여러 애니메이션을 끝내고 IDLE 상태가 되는 시간
    private float               m_rollDuration = 8.0f / 14.0f; // 구르기 지속시간
    private float               m_rollCurrentTime; //구르면서 지난 시간
    private float               m_rollCoolTime = 2; //구르기 쿨타임
//줄바꿈
//줄바꿈
    // Use this for initialization //주석임
    void Start () // 엔트리 "시작버튼을 눌렀을때" 랑 같은거
    { // 중괄호임
        m_animator = GetComponent<Animator>(); //에니메이터의 현재 스테이터스 불러오는 변수
        m_body2d = GetComponent<Rigidbody2D>(); // 물리 불러오는 변수
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>(); // Sensor_HeroKnight (MonoBehavior) 클래스에서 GroundSensor 컴포넌트 가져오기
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>(); // Sensor_HeroKnight (MonoBehavior) 클래스에서 WallSensor_R1 컴포넌트 가져오기
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>(); // Sensor_HeroKnight (MonoBehavior) 클래스에서 WallSensor_R2 컴포넌트 가져오기
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>(); // Sensor_HeroKnight (MonoBehavior) 클래스에서 WallSensor_L1 컴포넌트 가져오기 
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>(); // Sensor_HeroKnight (MonoBehavior) 클래스에서 WallSensor_L2 컴포넌트 가져오기
    } // 중괄호임
//줄바꿈
    // Update is called once per frame
    void Update () // 매 업데이트(프레임)마다 실행되는 거
    { // 중괄호임
        m_timeSinceAttack += Time.deltaTime;
        //줄바꿈
        if(m_rolling)
            m_rollCurrentTime += Time.deltaTime;
        //줄바꿈
        if(m_rollCurrentTime > m_rollDuration)
            m_rolling = false;
        //줄바꿈
        if (!m_grounded && m_groundSensor.State())
        { // 중괄호임
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        } // 중괄호임
        //줄바꿈
        if (m_grounded && !m_groundSensor.State())
        { // 중괄호임
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        } // 중괄호임
        //줄바꿈
        float inputX = Input.GetAxis("Horizontal");
        //줄바꿈
        if (inputX > 0)
        { // 중괄호임
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        } // 중괄호임
        //줄바꿈
        else if (inputX < 0)
        { // 중괄호임
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        } // 중괄호임
        //줄바꿈
        if (!m_rolling )
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        //줄바꿈
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);
        //줄바꿈
        //줄바꿈
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);
        //줄바꿈
        if (Input.GetKeyDown("e") && !m_rolling)
        { // 중괄호임
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        } // 중괄호임
        //줄바꿈
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");
        //줄바꿈
        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        { // 중괄호임
            m_currentAttack++;
            //줄바꿈
            if (m_currentAttack > 3)
                m_currentAttack = 1;
            //줄바꿈
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;
            //줄바꿈
            m_animator.SetTrigger("Attack" + m_currentAttack);
            //줄바꿈
            m_timeSinceAttack = 0.0f;
        } // 중괄호임
        //줄바꿈
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        { // 중괄호임
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        } // 중괄호임
        //줄바꿈
        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);
        //줄바꿈
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding && m_rollCoolTime <= 0)
        { // 중괄호임
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
            //줄바꿈
        } // 중괄호임
        //줄바꿈
        //줄바꿈
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        { // 중괄호임
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        } // 중괄호임
        //줄바꿈
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        { // 중괄호임
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        } // 중괄호임
        //줄바꿈
        else
        { // 중괄호임
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        } // 중괄호임
    } // 중괄호임
//줄바꿈
//줄바꿈
    void AE_SlideDust() // AE는 Animation Event의 약자, 슬라이딩할때 먼지만들어주는 거
    { // 중괄호임
        Vector3 spawnPosition;
        //줄바꿈   
        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;
        //줄바꿈
        if (m_slideDust != null)
        { // 중괄호임
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        } // 중괄호임
    } // 중괄호임
} // 중괄호임
//끝