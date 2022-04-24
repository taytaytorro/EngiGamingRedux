using EngiShotgun;
using JetBrains.Annotations;
using R2API;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static EngiShotgun.EngineerShotgunPlugin;
using static RoR2.UI.CrosshairController;

namespace ShotgunengiREDUX.SkillDefs
{
    public class GaussShotgun_SkillDef : ReloadSkillDef
	{
		public static GaussShotgun_SkillDef instance;
		static HorizontalLayoutGroup stockGroup;
		static int ShotgunStock => EngiShotgunSlot.shotgunStock.Value;

		private static GameObject _crosshairPrefab;
		private static GameObject CrosshairPrefab
		{
			get
			{
				if (_crosshairPrefab == null)
				{
					_crosshairPrefab = PrefabAPI.InstantiateClone(MainAssets.LoadAsset<UnityEngine.GameObject>("GaussCrosshair"), "Gauss Crosshair");
					GenerateCrosshair();
				}
				return _crosshairPrefab;
			}
		}

		public static void GenerateCrosshair()
		{
			Debug.LogWarning("Generating Crosshair!");
			stockGroup = _crosshairPrefab.GetComponentInChildren<HorizontalLayoutGroup>();
			CrosshairController stockController = _crosshairPrefab.GetComponent<CrosshairController>();
			var displays = stockController.skillStockSpriteDisplays;
			var newDisplays = new SkillStockSpriteDisplay[2 + ShotgunStock];
			newDisplays[0] = displays[0];
			newDisplays[1] = displays[1];


			var min = Mathf.Min(ShotgunStock, EngiShotgunSlot.maxVisibleStocks.Value);
			Debug.LogWarning($"Up to {min} stocks will be shown.");
			//Shotgun stock is 8
			//Max visible stock is 10
			//maximum between both is 10
			newDisplays[1].maximumStockCountToBeValid = ShotgunStock;

			//"stock" int goes from zero to maximum showable amount OR shotgun magazine amount, whichever is less
			for (int stock = 0; stock < min; stock++)
			{
				//load asset for each stock icon
				var stockPrefab = MainAssets.LoadAsset<UnityEngine.GameObject>("GaussStock");
				//create instance of stock icon, with Horizontal Layout Group as parent
				var stockInstance = Instantiate(stockPrefab, stockGroup.transform);
				//name stock icon after its stock index
				stockInstance.name = $"Stock Icon {stock + 1}";
				//skip over two starting icons- then create new sprite display
				//target is current stock icon- skill def is Shotgun- minimum to appear is this current integer- maximum to appear is
					//equal to the shotgun's magazine size.
				newDisplays[stock + 2] = new SkillStockSpriteDisplay()
				{
					target = stockInstance,
					requiredSkillDef = instance,
					minimumStockCountToBeValid = stock + 1,
					maximumStockCountToBeValid = ShotgunStock
				};
			}
			stockController.skillStockSpriteDisplays = newDisplays;
		}
		public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
			CrosshairUtils.RequestOverrideForBody(skillSlot.GetComponent<CharacterBody>(), CrosshairPrefab, CrosshairUtils.OverridePriority.Skill);
			return base.OnAssigned(skillSlot);
        }
    }
}
