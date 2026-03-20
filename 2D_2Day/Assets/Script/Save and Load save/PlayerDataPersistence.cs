using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// บันทึก / โหลดตำแหน่ง Player และชื่อ Scene ผ่าน IDataPersistence
/// ติดบน GameObject Player (ที่มี tag = \"Player\")
/// </summary>
public class PlayerDataPersistence : MonoBehaviour, IDataPersistence
{
    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = transform;
        
        // Debug ตรวจสอบว่าติดอยู่บน GameObject อะไร
        Debug.Log($"[PlayerDataPersistence] Awake on GameObject: {gameObject.name}, Tag: {gameObject.tag}");
        Debug.Log($"[PlayerDataPersistence] Initial position: {playerTransform.position}");
        
        // ตรวจสอบว่า GameObject นี้คือ Player จริงหรือไม่
        if (gameObject.tag != "Player")
        {
            Debug.LogWarning($"[PlayerDataPersistence] GameObject '{gameObject.name}' does not have 'Player' tag! Current tag: '{gameObject.tag}'. Please set tag to 'Player'.");
        }
        
        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.Register(this);
            Debug.Log("[PlayerDataPersistence] Successfully registered to DataPersistenceManager");
        }
        else
        {
            Debug.LogError("[PlayerDataPersistence] DataPersistenceManager.Instance is null! Make sure DataPersistenceManager exists in scene.");
        }
    }

    private void OnDestroy()
    {
        if (DataPersistenceManager.Instance != null)
            DataPersistenceManager.Instance.Unregister(this);
    }

    public void LoadData(GameData data)
    {
        if (data == null) 
        {
            // New Game: คงตำแหน่งเดิมที่วางไว้ใน scene
            Debug.Log("[PlayerDataPersistence] New Game detected - keeping original position");
            Debug.Log($"[PlayerDataPersistence] Current position: {playerTransform.position}");
            return;
        }

        Debug.Log($"[PlayerDataPersistence] Loading player data. Scene: {data.sceneName}, Pos: ({data.playerPosX}, {data.playerPosY})");
        Debug.Log($"[PlayerDataPersistence] Current position before load: {playerTransform.position}");

        // ถ้า Scene ชื่อตรงกัน (หรือยังว่าง) ให้ย้ายตำแหน่ง
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[PlayerDataPersistence] Current scene: {currentScene}");
        
        if (string.IsNullOrEmpty(data.sceneName) || data.sceneName == currentScene)
        {
            Vector3 newPos = new Vector3(data.playerPosX, data.playerPosY, playerTransform.position.z);
            playerTransform.position = newPos;
            Debug.Log($"[PlayerDataPersistence] Player position set to: {newPos}");
            Debug.Log($"[PlayerDataPersistence] Player position after load: {playerTransform.position}");
        }
        else
        {
            Debug.LogWarning($"[PlayerDataPersistence] Scene mismatch. Current: {currentScene}, Saved: {data.sceneName}");
            Debug.LogWarning("[PlayerDataPersistence] Player position not changed due to scene mismatch");
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data == null) return;

        string currentScene = SceneManager.GetActiveScene().name;
        data.sceneName = currentScene;
        
        // ตรวจสอบว่า transform ถูกต้อง
        if (playerTransform == null)
        {
            Debug.LogError("[PlayerDataPersistence] playerTransform is null!");
            return;
        }
        
        data.playerPosX = playerTransform.position.x;
        data.playerPosY = playerTransform.position.y;
        
        Debug.Log($"[PlayerDataPersistence] Saving player data. Scene: {currentScene}, Pos: ({data.playerPosX}, {data.playerPosY})");
        Debug.Log($"[PlayerDataPersistence] Full transform position: {playerTransform.position}");
    }
}

