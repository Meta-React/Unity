using UnityEngine;

public class CameraAction : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 10f, -7f);
    public float smoothSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        //1 目標位置を計算
        ///プレイヤーの回転は考慮しない
        Vector3 targetPosition = player.position + offset;

        //2 移動させる
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        //3 常にプレイヤーを見下ろす
        transform.LookAt(player.position);

    }
}
