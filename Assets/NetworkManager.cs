using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData { public string id; public float x, y, z; }

[System.Serializable]
public class PlayersWrapper { public PlayerData[] players; }

public class NetworkManager : MonoBehaviour
{
    public GameObject remotePlayerPrefab;   // Inspectorで指定する

    public float rotationSpeed = 10f;

    Dictionary<string, GameObject> remotes = new Dictionary<string, GameObject>();
    Dictionary<string, Vector3> targets = new Dictionary<string, Vector3>();

    public void OnPlayersUpdate(string json)
    {
        var data = JsonUtility.FromJson<PlayersWrapper>(json);
        var alive = new HashSet<string>();

        foreach (var p in data.players)
        {
            alive.Add(p.id);
            var pos = new Vector3(p.x, p.y, p.z);

            if (!remotes.ContainsKey(p.id))
                remotes[p.id] = Instantiate(remotePlayerPrefab, pos, Quaternion.identity);

            targets[p.id] = pos;
        }

        // いなくなった人を消す
        var toRemove = new List<string>();
        foreach (var id in remotes.Keys)
            if (!alive.Contains(id)) toRemove.Add(id);
        foreach (var id in toRemove)
        {
            Destroy(remotes[id]);
            remotes.Remove(id);
            targets.Remove(id);
        }
    }

    void Update()
    {
        foreach (var kv in remotes)
        {
            var t = kv.Value.transform;
            Vector3 target = targets[kv.Key];

            // 進む方向(水平方向)へ向きを回す
            Vector3 dir = target - t.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0025f)   // ほぼ止まっているときは回さない
            {
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                t.rotation = Quaternion.Slerp(t.rotation, rot, rotationSpeed * Time.deltaTime);
            }

            // 位置をなめらかに寄せる
            t.position = Vector3.Lerp(t.position, target, 10f * Time.deltaTime);
        }
    }
}