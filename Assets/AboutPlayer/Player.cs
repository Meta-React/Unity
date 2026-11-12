using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;

public class Player : MonoBehaviour
{

    public float moveSpeed = 0.1f;
    public float rotationSpeed = 10f;

    private Vector3 keyboardMoveDir;
    private Vector3 webMoveDir;


    //他人描画用の変数
    [DllImport("__Internal")]
    private static extern void SendPosition(float x, float y, float z);

    float sendTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        //移動処理
        // 1. Unity上のキーボード入力を取得
        GetKeyboardInput();

        // 2. キーボード入力とWebからの入力を合算
        Vector3 totalMoveDir = keyboardMoveDir + webMoveDir;

        // 移動処理
        if (totalMoveDir.sqrMagnitude > 0.01f)
        {
            transform.position += totalMoveDir.normalized * moveSpeed;

            // 移動方向へキャラクターの正面を向かせる回転処理
            Quaternion targetRotation = Quaternion.LookRotation(totalMoveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //位置の送信
        sendTimer += Time.deltaTime;
        if (sendTimer >= 0.1f)
        {
            sendTimer = 0f;
#if UNITY_WEBGL && !UNITY_EDITOR
                SendPosition(transform.position.x, transform.position.y, transform.position.z);
#endif
        }


    }

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
        }
    }

    public void OnReactKey(string message)
    {
        Debug.Log(message);
    }
    public void OnReceiveInput(string action)
    {

        webMoveDir = Vector3.zero;

        // "STOP" には S が含まれるので、先に判定して抜ける
        if (action == "STOP") return;

        if (action.Contains("W")) webMoveDir += Vector3.forward;
        if (action.Contains("S")) webMoveDir += Vector3.back;
        if (action.Contains("A")) webMoveDir += Vector3.left;
        if (action.Contains("D")) webMoveDir += Vector3.right;
    }
}
