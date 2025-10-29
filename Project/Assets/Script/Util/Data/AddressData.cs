using System.Collections.Generic;

namespace Backend.Util.Data
{
	public static class AddressData
	{
		public const string Assets_Prefab_Character_Enemy_Normal_Skeleton_Sword_Prefab = "Assets/Prefab/Character/Enemy/Normal/Skeleton_Sword.prefab";
		public const string Assets_Prefab_Character_Player_Player_Character_Beta_Prefab = "Assets/Prefab/Character/Player/Player_Character_(Beta).prefab";
		public const string Assets_Prefab_Character_Enemy_Boss_NineTail_Human_NineTail_Human_Prefab = "Assets/Prefab/Character/Enemy/Boss/NineTail_Human/NineTail_Human.prefab";
		public const string Assets_Prefab_Character_Enemy_Normal_Skeleton_Bow_Prefab = "Assets/Prefab/Character/Enemy/Normal/Skeleton_Bow.prefab";
		public const string Assets_Prefab_UI_Status_Window_Prefab = "Assets/Prefab/UI/Status_Window.prefab";
		public const string Assets_Prefab_UI_Shop_Window_Prefab = "Assets/Prefab/UI/Shop_Window.prefab";
		public const string Assets_Prefab_UI_Inventory_Window_Prefab = "Assets/Prefab/UI/Inventory_Window.prefab";
		public const string Assets_Prefab_UI_Level_Up_Window_Prefab = "Assets/Prefab/UI/Level_Up_Window.prefab";
		public const string Assets_Prefab_UI_Equipment_Window_Prefab = "Assets/Prefab/UI/Equipment_Window.prefab";
		public const string Assets_Prefab_UI_Setting_Window_Prefab = "Assets/Prefab/UI/Setting_Window.prefab";
		
		public static Dictionary<string, HashSet<string>> Groups = new ()
		{
			{ "Forest_01_Scene", new HashSet<string> { "Assets/Prefab/Character/Enemy/Normal/Skeleton_Sword.prefab","Assets/Prefab/Character/Player/Player_Character_(Beta).prefab","Assets/Prefab/Character/Enemy/Normal/Skeleton_Bow.prefab","Assets/Prefab/UI/Status_Window.prefab","Assets/Prefab/UI/Shop_Window.prefab","Assets/Prefab/UI/Inventory_Window.prefab","Assets/Prefab/UI/Level_Up_Window.prefab","Assets/Prefab/UI/Equipment_Window.prefab","Assets/Prefab/UI/Setting_Window.prefab" } },
			{ "Village_01_Scene", new HashSet<string> { "Assets/Prefab/Character/Enemy/Normal/Skeleton_Sword.prefab","Assets/Prefab/Character/Player/Player_Character_(Beta).prefab","Assets/Prefab/Character/Enemy/Normal/Skeleton_Bow.prefab","Assets/Prefab/UI/Status_Window.prefab","Assets/Prefab/UI/Shop_Window.prefab","Assets/Prefab/UI/Inventory_Window.prefab","Assets/Prefab/UI/Level_Up_Window.prefab","Assets/Prefab/UI/Equipment_Window.prefab","Assets/Prefab/UI/Setting_Window.prefab" } },
			{ "Boss_01_Scene", new HashSet<string> { "Assets/Prefab/Character/Player/Player_Character_(Beta).prefab","Assets/Prefab/Character/Enemy/Boss/NineTail_Human/NineTail_Human.prefab","Assets/Prefab/UI/Status_Window.prefab","Assets/Prefab/UI/Shop_Window.prefab","Assets/Prefab/UI/Inventory_Window.prefab","Assets/Prefab/UI/Level_Up_Window.prefab","Assets/Prefab/UI/Equipment_Window.prefab","Assets/Prefab/UI/Setting_Window.prefab" } },
			{ "Gwanghwamun_Scene", new HashSet<string> { "Assets/Prefab/UI/Status_Window.prefab","Assets/Prefab/UI/Shop_Window.prefab","Assets/Prefab/UI/Inventory_Window.prefab","Assets/Prefab/UI/Level_Up_Window.prefab","Assets/Prefab/UI/Equipment_Window.prefab","Assets/Prefab/UI/Setting_Window.prefab" } },
			{ "Main_Scene", new HashSet<string> { "Assets/Prefab/UI/Setting_Window.prefab" } },
		};
	}
}
