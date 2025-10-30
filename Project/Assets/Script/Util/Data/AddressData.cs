using System.Collections.Generic;

namespace Backend.Util.Data
{
	public static class AddressData
	{
		public const string Assets_Data_Spawn_Village_01_Spawn_Data_Asset = "Assets/Data/Spawn/Village_01_Spawn_Data.asset";
		public const string Assets_Data_Spawn_Forest_01_Spawn_Data_Asset = "Assets/Data/Spawn/Forest_01_Spawn_Data.asset";
		public const string Assets_Data_Spawn_Boss_01_Spawn_Data_Asset = "Assets/Data/Spawn/Boss_01_Spawn_Data.asset";
		public const string Assets_Prefab_UI_Level_Up_Window_Prefab = "Assets/Prefab/UI/Level_Up_Window.prefab";
		public const string Assets_Prefab_UI_Status_Window_Prefab = "Assets/Prefab/UI/Status_Window.prefab";
		public const string Assets_Prefab_UI_Setting_Window_Prefab = "Assets/Prefab/UI/Setting_Window.prefab";
		public const string Assets_Prefab_UI_Shop_Window_Prefab = "Assets/Prefab/UI/Shop_Window.prefab";
		public const string Assets_Prefab_UI_Equipment_Window_Prefab = "Assets/Prefab/UI/Equipment_Window.prefab";
		public const string Assets_Prefab_UI_Battle_Interface_Window_Prefab = "Assets/Prefab/UI/Battle_Interface_Window.prefab";
		public const string Assets_Prefab_UI_Inventory_Window_Prefab = "Assets/Prefab/UI/Inventory_Window.prefab";
		public const string Assets_Prefab_Character_Enemy_Normal_Tiger_Prefab = "Assets/Prefab/Character/Enemy/Normal/Tiger.prefab";
		public const string Assets_Prefab_Character_Enemy_Normal_Skeleton_Sword_Prefab = "Assets/Prefab/Character/Enemy/Normal/Skeleton_Sword.prefab";
		public const string Assets_Prefab_Character_Player_Player_Character_Beta_Prefab = "Assets/Prefab/Character/Player/Player_Character_(Beta).prefab";
		public const string Assets_Prefab_Character_Enemy_Boss_NineTail_Human_NineTail_Human_Prefab = "Assets/Prefab/Character/Enemy/Boss/NineTail_Human/NineTail_Human.prefab";
		public const string Assets_Prefab_Character_Enemy_Normal_Skeleton_Bow_Prefab = "Assets/Prefab/Character/Enemy/Normal/Skeleton_Bow.prefab";
		
		public static Dictionary<string, Dictionary<string, HashSet<string>>> Groups = new ()
		{
			{
				"Data",
				new Dictionary<string, HashSet<string>>()
				{
					{ "Gwanghwamun_Scene", new HashSet<string> { } },
					{ "Main_Scene", new HashSet<string> { } },
					{ "Village_01_Scene", new HashSet<string> { "Assets/Data/Spawn/Village_01_Spawn_Data.asset" } },
					{ "Forest_01_Scene", new HashSet<string> { "Assets/Data/Spawn/Forest_01_Spawn_Data.asset" } },
					{ "Boss_01_Scene", new HashSet<string> { "Assets/Data/Spawn/Boss_01_Spawn_Data.asset" } },
				}
			},
			{
				"UI",
				new Dictionary<string, HashSet<string>>()
				{
					{ "Boss_01_Scene", new HashSet<string> { "Assets/Prefab/UI/Level_Up_Window.prefab","Assets/Prefab/UI/Status_Window.prefab","Assets/Prefab/UI/Setting_Window.prefab","Assets/Prefab/UI/Shop_Window.prefab","Assets/Prefab/UI/Equipment_Window.prefab","Assets/Prefab/UI/Battle_Interface_Window.prefab","Assets/Prefab/UI/Inventory_Window.prefab" } },
					{ "Forest_01_Scene", new HashSet<string> { "Assets/Prefab/UI/Level_Up_Window.prefab","Assets/Prefab/UI/Status_Window.prefab","Assets/Prefab/UI/Setting_Window.prefab","Assets/Prefab/UI/Shop_Window.prefab","Assets/Prefab/UI/Equipment_Window.prefab","Assets/Prefab/UI/Battle_Interface_Window.prefab","Assets/Prefab/UI/Inventory_Window.prefab" } },
					{ "Gwanghwamun_Scene", new HashSet<string> { "Assets/Prefab/UI/Level_Up_Window.prefab","Assets/Prefab/UI/Status_Window.prefab","Assets/Prefab/UI/Setting_Window.prefab","Assets/Prefab/UI/Shop_Window.prefab","Assets/Prefab/UI/Equipment_Window.prefab","Assets/Prefab/UI/Battle_Interface_Window.prefab","Assets/Prefab/UI/Inventory_Window.prefab" } },
					{ "Village_01_Scene", new HashSet<string> { "Assets/Prefab/UI/Level_Up_Window.prefab","Assets/Prefab/UI/Status_Window.prefab","Assets/Prefab/UI/Setting_Window.prefab","Assets/Prefab/UI/Shop_Window.prefab","Assets/Prefab/UI/Equipment_Window.prefab","Assets/Prefab/UI/Battle_Interface_Window.prefab","Assets/Prefab/UI/Inventory_Window.prefab" } },
					{ "Main_Scene", new HashSet<string> { "Assets/Prefab/UI/Setting_Window.prefab" } },
				}
			},
			{
				"Game Object",
				new Dictionary<string, HashSet<string>>()
				{
					{ "Gwanghwamun_Scene", new HashSet<string> { } },
					{ "Main_Scene", new HashSet<string> { } },
					{ "Forest_01_Scene", new HashSet<string> { "Assets/Prefab/Character/Enemy/Normal/Tiger.prefab","Assets/Prefab/Character/Enemy/Normal/Skeleton_Sword.prefab","Assets/Prefab/Character/Player/Player_Character_(Beta).prefab","Assets/Prefab/Character/Enemy/Normal/Skeleton_Bow.prefab" } },
					{ "Village_01_Scene", new HashSet<string> { "Assets/Prefab/Character/Enemy/Normal/Skeleton_Sword.prefab","Assets/Prefab/Character/Player/Player_Character_(Beta).prefab","Assets/Prefab/Character/Enemy/Normal/Skeleton_Bow.prefab" } },
					{ "Boss_01_Scene", new HashSet<string> { "Assets/Prefab/Character/Player/Player_Character_(Beta).prefab","Assets/Prefab/Character/Enemy/Boss/NineTail_Human/NineTail_Human.prefab" } },
				}
			},
		};

		#region NESTED STRUCTURE API

		public struct Group
		{
			public const string Data = "Data";
			public const string UI = "UI";
			public const string Game_Object = "Game Object";
		}

		public struct Label
		{
			public const string Village_01_Scene = "Village_01_Scene";
			public const string Forest_01_Scene = "Forest_01_Scene";
			public const string Boss_01_Scene = "Boss_01_Scene";
			public const string Gwanghwamun_Scene = "Gwanghwamun_Scene";
			public const string Main_Scene = "Main_Scene";
		}

		#endregion
	}
}