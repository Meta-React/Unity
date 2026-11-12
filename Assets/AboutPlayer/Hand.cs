using UnityEngine;

public class Hand : MonoBehaviour
{

    [Header("手の設定")]
    public Transform leftHand;   // 左手オブジェクトのTransform
    public Transform rightHand;  // 右手オブジェクトのTransform
    [Header("浮遊の設定")]
    public float floatSpeed = 5f;    // 浮遊の速さ
    public float floatAmount = 0.1f; // 浮遊の揺れの幅

    private Vector3 leftHandDefaultLocalPos;
    private Vector3 rightHandDefaultLocalPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 手の初期のローカル位置を記憶しておく
        if (leftHand != null) leftHandDefaultLocalPos = leftHand.localPosition;
        if (rightHand != null) rightHandDefaultLocalPos = rightHand.localPosition;

    }

    // Update is called once per frame
    void Update()
    {

        // 時間経過で上下するオフセットを計算 (Mathf.Sinを使用)
        float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;

        if (leftHand != null)
        {
            // 基準位置に対してY軸方向に揺れを加える（左右で少しタイミングをずらすと自然になります）
            Vector3 pos = leftHandDefaultLocalPos;
            pos.y += floatOffset;
            leftHand.localPosition = pos;
        }

        if (rightHand != null)
        {
            Vector3 pos = rightHandDefaultLocalPos;
            pos.y += floatOffset;
            rightHand.localPosition = pos;
        }

    }
}
