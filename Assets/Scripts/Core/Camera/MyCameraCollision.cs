using UnityEngine;
using Unity.Cinemachine;

public class MyCameraCollision : CinemachineExtension
{
    [Header("避障設定")]
    public LayerMask penetrableLayers; // 可穿模物件的層
    public LayerMask nonPenetrableNoAvoidLayers; // 不可穿模但無須避障物件的層
    public LayerMask nonPenetrableAvoidLayers; // 不可穿模必須避障物件的層
    public float cameraRadius = 0.5f; // 鏡頭碰撞半徑
    public float minDistance = 1f; // 鏡頭與角色最小距離
    public float damping = 0.5f; // 阻尼因子，用於平滑移動

    // internal state for wall‑locking behaviour
    private bool wallLocked = false;
    private Vector3 lockedPosition;
    private Vector3 lockedDirection;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            Vector3 targetPos = state.ReferenceLookAt; // 角色位置
            Vector3 desiredPos = state.RawPosition; // 期望鏡頭位置

            // 計算期望的垂直偏移，用於保持高度關係一致
            float desiredHeight = desiredPos.y - targetPos.y;

            Vector3 direction = desiredPos - targetPos; // 從角色到鏡頭的方向
            float maxDistance = direction.magnitude; // 最大距離

            // 碰撞檢查層：不可穿模的層
            LayerMask collisionLayers = nonPenetrableNoAvoidLayers | nonPenetrableAvoidLayers;

            // 檢查從角色到期望鏡頭位置之間的障礙物
            RaycastHit hit;
            bool didCollide = Physics.SphereCast(targetPos, cameraRadius, direction.normalized, out hit, maxDistance, collisionLayers);

            // 計算水平方向（XZ 平面）的單位向量，供牆邊鎖定判斷使用
            Vector3 horizontalDir = new Vector3(direction.x, 0f, direction.z);
            Vector3 horizontalDirNorm = horizontalDir.normalized;

            if (wallLocked)
            {
                // 如果目前處於牆角鎖定中，檢查玩家是否旋轉出鎖定方向
                if (Vector3.Angle(horizontalDirNorm, lockedDirection) > 10f)
                {
                    wallLocked = false;
                }
                else
                {
                    // 鎖定位置上平滑移動
                    state.RawPosition = Vector3.Lerp(state.RawPosition, lockedPosition, damping * deltaTime);
                    return;
                }
            }

            if (didCollide)
            {
                int hitLayer = hit.collider.gameObject.layer;
                bool isAvoidLayer = ((1 << hitLayer) & nonPenetrableAvoidLayers) != 0;

                Vector3 correctedPos;

                if (isAvoidLayer)
                {
                    // 不可穿模且必須避障：移動到player前，保持最小距離
                    float correctedDistance = Mathf.Max(hit.distance, minDistance);
                    correctedPos = targetPos + direction.normalized * correctedDistance;

                    // 強制高度維持原本偏移
                    correctedPos.y = targetPos.y + desiredHeight;

                    // 判斷是否應該啟動牆邊鎖定 (碰到接近最小距離的垂直牆)
                    if (hit.distance <= minDistance + 0.01f && Mathf.Abs(hit.normal.y) < 0.1f)
                    {
                        wallLocked = true;
                        lockedPosition = correctedPos;
                        lockedDirection = horizontalDirNorm;
                    }
                }
                else
                {
                    // 不可穿模但無須避障：設置到碰撞點，不保持最小距離
                    correctedPos = targetPos + direction.normalized * hit.distance;
                    correctedPos.y = targetPos.y + desiredHeight;
                }

                state.RawPosition = Vector3.Lerp(state.RawPosition, correctedPos, damping * deltaTime);
            }
            else
            {
                // 無碰撞，檢查是否需要解鎖牆邊鎖定
                if (wallLocked)
                {
                    wallLocked = false;
                }
            }
        }
    }
}
