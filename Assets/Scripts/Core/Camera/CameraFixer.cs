using UnityEngine;
using Unity.Cinemachine;

public class CameraFixer : MonoBehaviour
{
    public CinemachineCamera virCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (virCam != null && virCam.Follow != null)
        {
            var deoccluder = virCam.GetComponent<CinemachineDeoccluder>();
            if (deoccluder != null)
            {
                // 強制 Deoccluder 忘記之前的遮蔽狀態，重新計算
                // 有些版本是透過直接 Disable 再 Enable 來重置
                deoccluder.enabled = false;
                deoccluder.enabled = true;
            }

            //// 1. 先確保角色座標已經定位 (如果剛從別處傳送過來)
            //Vector3 delta = virCam.Follow.position - transform.position;

            //// 2. 這是最重要的一行：這會讓相機維持目前在 Inspector 設定好的距離，
            //// 並「跳過」中間的追蹤過程。
            //virCam.OnTargetObjectWarped(virCam.Follow, delta);

            //// 3. 強制立即刷新，這會讓相機在第一幀就處於計算後的正確位置
            //// 而不是使用 ForceCameraPosition 去覆蓋位置
            //// 注意：新版 Cinemachine (v3) 使用 InternalUpdate()
            //// 舊版或通用版則可以用：
            //virCam.gameObject.SetActive(false);
            //virCam.gameObject.SetActive(true);


            // 1. 通知 Cinemachine 目標發生了「瞬間移動」（Warp）
            // 這會清除所有平滑追蹤（Damping）的快取，讓相機不再嘗試緩衝移動
            virCam.OnTargetObjectWarped(virCam.Follow, virCam.Follow.position - virCam.transform.position);

            // 2. 強制相機「立刻」執行一次狀態更新
            // 這行非常關鍵！它會強迫 Cinemachine 忽略 Update 順序，現在就算出位置
            //virCam.ForceCameraPosition(virCam.Follow.position, virCam.Follow.rotation);

            // 3. (選用) 如果你使用的是新版 Cinemachine，可以呼叫 InternalUpdate
            // 確保所有位移組件（如 Transposer）都已經就位
            // virCam.InternalUpdate(); 
        }

        //if (virCam != null)
        //{
        //    virCam.ForceCameraPosition(virCam.Follow.position + virCam.State.RawPosition, Quaternion.identity);

        //    // �Ϊ̳�²�檺��k�G
        //    virCam.OnTargetObjectWarped(virCam.Follow, virCam.Follow.position - transform.position);
        //}
    }

}
