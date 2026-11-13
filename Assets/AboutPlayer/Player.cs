using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;

public class Player : MonoBehaviour
{
    private CharacterController controller;

    [Header("移動設定")]
    public float moveSpeed = 0.1f;  //移動速度
    public float rotationSpeed = 10f;  //回転速度

    [Header("ジャンプ・重力設定")]
    public float jumpHeight = 2.0f;     // ジャンプの高さ (m)
    public float gravityValue = -25.0f; // 重力の強さ

    private float verticalVelocity = 0f;

    //キーボード入力とWebからの入力を格納する変数
    private Vector3 keyboardMoveDir;
    private Vector3 webMoveDir;
    private bool jumpRequested; // ジャンプ要求フラグ


    //他人描画用の変数
    [DllImport("__Internal")]
    private static extern void SendPosition(float x, float y, float z);

    float sendTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Awake()
    {
        // PlayerオブジェクトにアタッチされているCharacterControllerを取得する
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {


        //移動処理
        // 1. Unity上のキーボード入力を取得
        GetKeyboardInput();

        // 2. 移動とジャンプをひとまとめにして処理する
        ProcessMovementAndJump();

        // 3. 位置のネットワーク送信
        HandleNetworkSync();


    }

    // 移動とジャンプを統合した処理
    void ProcessMovementAndJump()
    {
        bool isGrounded = controller.isGrounded;

        // 1. 接地中は微小な下向き速度で地面に密着させる
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // 2. キーボード入力とWebからの入力を合算(斜め移動が速くならないよう長さを1以内に制限)
        Vector3 totalMoveDir = keyboardMoveDir + webMoveDir;
        Vector3 horizontalMove = Vector3.ClampMagnitude(
            new Vector3(totalMoveDir.x, 0f, totalMoveDir.z), 1f);

        // 3. ジャンプ(Webからのフラグ or スペースキー、かつ接地中のみ)
        bool jumpInput = jumpRequested ||
            (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);
        if (jumpInput && isGrounded)
        {
            // 指定の高さに到達する初速: v = sqrt(h * -2 * g)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }
        jumpRequested = false; // フラグをリセット

        // 4. 重力を加算
        verticalVelocity += gravityValue * Time.deltaTime;

        // 5. 水平+垂直を合成して1回だけ Move する
        Vector3 velocity = horizontalMove * moveSpeed;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // 6. 移動方向へキャラクターを向ける
        if (horizontalMove.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalMove, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // ネットワーク同期処理
    void HandleNetworkSync()
    {
        sendTimer += Time.deltaTime;
        if (sendTimer >= 0.1f)
        {
            sendTimer = 0f;
#if UNITY_WEBGL && !UNITY_EDITOR
            SendPosition(transform.position.x, transform.position.y, transform.position.z);
#endif
        }
    }

    // Unityでキーボード入力を取得する関数
    void GetKeyboardInput()
    {
        keyboardMoveDir = Vector3.zero;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) keyboardMoveDir += Vector3.forward;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) keyboardMoveDir += Vector3.back;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) keyboardMoveDir += Vector3.left;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) keyboardMoveDir += Vector3.right;
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                jumpRequested = true;
            }
        }
    }

    /*public void OnReactKey(string message)
    {
        Debug.Log(message);
    }*/


    //Webからの入力を受け取るための関数
    public void OnReceiveInput(string action)
    {

        webMoveDir = Vector3.zero;

        // "STOP" には S が含まれるので、先に判定して抜ける
        if (action == "STOP") return;

        if (action.Contains("W")) webMoveDir += Vector3.forward;
        if (action.Contains("S")) webMoveDir += Vector3.back;
        if (action.Contains("A")) webMoveDir += Vector3.left;
        if (action.Contains("D")) webMoveDir += Vector3.right;
        // Web側から "SPACE" が送られてきた場合ジャンプさせる
        if (action.Contains("SPACE"))
        {
            jumpRequested = true;
        }
    }
}
