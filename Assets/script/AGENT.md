# AGENT.md

## 檔案結構

此專案的檔案結構如下：

```
script/
	AI.meta
	Attack.meta
	Decal.meta
	AI/
		AIGroup.cs
		AIGroup.cs.meta
		AStarMover.cs
		AStarMover.cs.meta
		FollowAI.cs
		FollowAI.cs.meta
		FSM.meta
		MyAStar.cs
		MyAStar.cs.meta
		Node.cs
		Node.cs.meta
		PathNode.cs
		PathNode.cs.meta
		PlayerMove.cs
		PlayerMove.cs.meta
		SteeringBehavior.cs
		SteeringBehavior.cs.meta
		waypoint.cs
		waypoint.cs.meta
		WayPointManager.cs
		WayPointManager.cs.meta
		FSM/
			AI.cs
			AI.cs.meta
			AIData.cs
			AIData.cs.meta
			FSMState.cs
			FSMState.cs.meta
			FSMSystem.cs
			FSMSystem.cs.meta
	Attack/
		AttackManager.cs
		AttackManager.cs.meta
		HealthySystem.cs
		HealthySystem.cs.meta
		PlayerCombat.cs
		PlayerCombat.cs.meta
	Decal/
		AOEVisual_Solid.cs
		AOEVisual_Solid.cs.meta
		ESOTelegraphController.cs
		ESOTelegraphController.cs.meta
		TelegraphTrigger.cs
		TelegraphTrigger.cs.meta
```

## 描述

此專案包含以下主要模組：

### AI 模組
- **AIGroup.cs**: 負責管理 AI 群組的行為。
- **AStarMover.cs**: 實現 A* 路徑尋找演算法的移動邏輯。
- **FollowAI.cs**: 用於實現跟隨行為的 AI 腳本。
- **FSM/**: 包含有限狀態機 (Finite State Machine, FSM) 的相關實現。
  - **AI.cs**: 定義 AI 的基本行為。
  - **AIData.cs**: 儲存 AI 的數據。
  - **FSMState.cs**: 定義 FSM 的狀態。
  - **FSMSystem.cs**: 管理 FSM 的系統邏輯。

### 攻擊模組
- **AttackManager.cs**: 管理攻擊行為的邏輯。
- **HealthySystem.cs**: 處理角色的健康系統。
- **PlayerCombat.cs**: 實現玩家的戰鬥邏輯。

### 繪圖模組
- **AOEVisual_Solid.cs**: 負責顯示範圍效果 (AOE) 的視覺效果。
- **ESOTelegraphController.cs**: 控制範圍提示的行為。
- **TelegraphTrigger.cs**: 處理範圍提示的觸發邏輯。

## 使用說明

1. 將專案匯入到您的開發環境中。
2. 確保所有必要的依賴項已正確安裝。
3. 根據需求修改腳本，並執行測試以確保功能正常運作。

## 注意事項

- 修改任何腳本前，請先備份原始檔案。
- 測試所有更改以確保不會影響其他模組的功能。

## 聯絡方式

如有任何問題，請聯絡專案負責人或提交問題至專案的問題追蹤系統。
