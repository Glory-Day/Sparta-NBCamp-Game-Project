using System.Collections.Generic;

namespace Backend.Util.Data
{
	public static class AddressData
	{
		public const string Assets_Prefab_Character_Enemy_Normal_Skeleton_Sword_Prefab = "Assets/Prefab/Character/Enemy/Normal/Skeleton_Sword.prefab";
		public const string Assets_Prefab_Character_Player_Player_Character_Beta_Prefab = "Assets/Prefab/Character/Player/Player_Character_(Beta).prefab";
		public const string Assets_Prefab_Character_Enemy_Boss_NineTail_Human_NineTail_Human_Prefab = "Assets/Prefab/Character/Enemy/Boss/NineTail_Human/NineTail_Human.prefab";
		public const string Assets_Prefab_Character_Enemy_Normal_Skeleton_Bow_Prefab = "Assets/Prefab/Character/Enemy/Normal/Skeleton_Bow.prefab";

		public static Dictionary<string, HashSet<string>> Groups = new ()
		{
            { "Main_Scene", new HashSet<string>() },
			{ "Forest_01_Scene", new HashSet<string> { "Assets/Prefab/Character/Enemy/Normal/Skeleton_Sword.prefab","Assets/Prefab/Character/Player/Player_Character_(Beta).prefab","Assets/Prefab/Character/Enemy/Normal/Skeleton_Bow.prefab" } },
			{ "Village_01_Scene", new HashSet<string> { "Assets/Prefab/Character/Enemy/Normal/Skeleton_Sword.prefab","Assets/Prefab/Character/Player/Player_Character_(Beta).prefab","Assets/Prefab/Character/Enemy/Normal/Skeleton_Bow.prefab" } },
			{ "Boss_01_Scene", new HashSet<string> { "Assets/Prefab/Character/Player/Player_Character_(Beta).prefab","Assets/Prefab/Character/Enemy/Boss/NineTail_Human/NineTail_Human.prefab" } },
		};
	}
}
