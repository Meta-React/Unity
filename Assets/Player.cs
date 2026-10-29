using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public float moveSpeed = 0.1f;
    public float rotationSpeed = 10f;

    private Vector3 moveDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        //何かしらのキーが押されている時
        if (moveDir.sqrMagnitude > 0.01f)
        {
            // 1. 移動処理（normalizedを挟むことで斜め移動が速くなりすぎるのを防ぐ）
            transform.position += moveDir.normalized * moveSpeed;

            // 2. 移動方向へキャラクターの正面を向かせる回転処理
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }


    }


    public void OnReceiveInput(string action)
    {

        moveDir = Vector3.zero;

        switch (action)
        {
            case "W":
                moveDir = Vector3.forward;
                break;
            case "S":
                moveDir = Vector3.back;
                break;
            case "A":
                moveDir = Vector3.left;
                break;
            case "D":
                moveDir = Vector3.right;
                break;

        }
    }
}
