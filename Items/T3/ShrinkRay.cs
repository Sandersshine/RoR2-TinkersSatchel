﻿using RoR2;
using UnityEngine;
using System.Collections.ObjectModel;
using TILER2;
using static TILER2.MiscUtil;
using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace ThinkInvisible.TinkersSatchel {
	public class ShrinkRay : Item<ShrinkRay> {

		////// Item Data //////

		public override string displayName => "Shrink Ray";
		public override ItemTier itemTier => ItemTier.Tier3;
		public override ReadOnlyCollection<ItemTag> itemTags => new(new[] { ItemTag.Utility });

		protected override string GetNameString(string langid = null) => displayName;
		protected override string GetPickupString(string langid = null) => "Suppress a single target's non-primary skills and damage.";
		protected override string GetDescString(string langid = null) => $"Once every {icd:N1} seconds, hitting an enemy disables their <style=cIsUtility>non-primary skills</style> and reduces their <style=cIsDamage>damage</style> by 50% for {duration:N1} seconds <style=cStack>(+{duration:N1} seconds per stack)</style>.";
		protected override string GetLoreString(string langid = null) => "";



		////// Config //////

		[AutoConfigRoOSlider("{0:N1} s", 0f, 30f)]
		[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
		[AutoConfig("Effect duration per stack.",
			AutoConfigFlags.None, 0f, float.MaxValue)]
		public float duration { get; private set; } = 3f;

		[AutoConfigRoOSlider("{0:N1} s", 0f, 30f)]
		[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
		[AutoConfig("Internal cooldown of applying the effect. Does not stack.",
			AutoConfigFlags.None, 0f, float.MaxValue)]
		public float icd { get; private set; } = 2.5f;



		////// Other Fields/Properties //////

		public BuffDef shrinkDebuff { get; private set; }
        public GameObject idrPrefab { get; private set; }



        ////// TILER2 Module Setup //////

        public ShrinkRay() {
			modelResource = TinkersSatchelPlugin.resources.LoadAsset<GameObject>("Assets/TinkersSatchel/Prefabs/Items/ShrinkRay.prefab");
			iconResource = TinkersSatchelPlugin.resources.LoadAsset<Sprite>("Assets/TinkersSatchel/Textures/ItemIcons/shrinkRayIcon.png");
            idrPrefab = TinkersSatchelPlugin.resources.LoadAsset<GameObject>("Assets/TinkersSatchel/Prefabs/Items/Display/ShrinkRay.prefab");
        }

        public override void SetupModifyItemDef() {
            base.SetupModifyItemDef();

            CommonCode.RetrieveDefaultMaterials(idrPrefab.GetComponent<ItemDisplay>());

            #region ItemDisplayRule Definitions

            /// Survivors ///
            displayRules.Add("Bandit2Body", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(-0.31565F, -0.0445F, 0.04523F),
                localAngles = new Vector3(300.5455F, 297.5691F, 91.7804F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("CaptainBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(-0.22635F, 0.26941F, 0.01304F),
                localAngles = new Vector3(13.4581F, 291.0686F, 35.81881F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("CommandoBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(-0.20543F, -0.01136F, -0.1254F),
                localAngles = new Vector3(335.1445F, 238.071F, 73.80905F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("CrocoBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(2.18319F, -0.78877F, 0.74221F),
                localAngles = new Vector3(7.56097F, 89.35254F, 108.851F),
                localScale = new Vector3(3F, 3F, 3F)
            });
            displayRules.Add("EngiBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Pelvis",
                localPos = new Vector3(-0.26052F, 0.13688F, 0.07865F),
                localAngles = new Vector3(354.8487F, 286.5102F, 251.6627F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("HuntressBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(-0.16105F, 0.00864F, -0.1383F),
                localAngles = new Vector3(18.0823F, 42.60661F, 76.35323F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("LoaderBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "MechBase",
                localPos = new Vector3(-0.25048F, -0.03595F, -0.00376F),
                localAngles = new Vector3(11.4835F, 82.80101F, 352.5358F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("MageBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Pelvis",
                localPos = new Vector3(-0.18394F, -0.07509F, 0.02655F),
                localAngles = new Vector3(8.07984F, 251.9991F, 228.8695F),
                localScale = new Vector3(0.25F, 0.25F, 0.25F)
            });
            displayRules.Add("MercBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "ThighL",
                localPos = new Vector3(0.15595F, 0.13259F, 0.04387F),
                localAngles = new Vector3(356.5925F, 253.9449F, 262.8833F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("ToolbotBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Chest",
                localPos = new Vector3(-2.47602F, 0.03398F, -1.0808F),
                localAngles = new Vector3(332.6047F, 273.7401F, 72.44123F),
                localScale = new Vector3(3F, 3F, 3F)
            });
            displayRules.Add("TreebotBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "WeaponPlatformEnd",
                localPos = new Vector3(-0.28183F, -0.3983F, 0.16674F),
                localAngles = new Vector3(1.82438F, 278.0178F, 269.5744F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            displayRules.Add("RailgunnerBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "BottomRail",
                localPos = new Vector3(0.00004F, 0.73052F, -0.08509F),
                localAngles = new Vector3(353.429F, 140.3837F, 282.2221F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("VoidSurvivorBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(0.17554F, -0.13447F, -0.0436F),
                localAngles = new Vector3(15.08189F, 9.51543F, 15.89409F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            #endregion
        }

        public override void SetupAttributes() {
			base.SetupAttributes();

			shrinkDebuff = ScriptableObject.CreateInstance<BuffDef>();
			shrinkDebuff.buffColor = Color.white;
			shrinkDebuff.canStack = true;
			shrinkDebuff.isDebuff = true;
			shrinkDebuff.name = "TKSATShrink";
			shrinkDebuff.iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texDifficultyEasyIcon.png")
				.WaitForCompletion();
			ContentAddition.AddBuffDef(shrinkDebuff);
		}

		public override void Install() {
			base.Install();

            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
            On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
		}

        public override void Uninstall() {
			base.Uninstall();

			On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
			On.RoR2.CharacterBody.RecalculateStats -= CharacterBody_RecalculateStats;
			On.RoR2.CharacterBody.OnBuffFinalStackLost -= CharacterBody_OnBuffFinalStackLost;
			On.RoR2.CharacterBody.OnBuffFirstStackGained -= CharacterBody_OnBuffFirstStackGained;
		}



		////// Hooks //////

		private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim) {
			orig(self, damageInfo, victim);
			if(NetworkServer.active && damageInfo != null && damageInfo.attacker) {
				var count = GetCount(damageInfo.attacker.GetComponent<CharacterBody>());
				if(count > 0 && damageInfo.attacker != victim) {
					var sricd = damageInfo.attacker.GetComponent<ShrinkRayICDComponent>();
					if(!sricd)
						sricd = damageInfo.attacker.AddComponent<ShrinkRayICDComponent>();
					if(Time.fixedTime - sricd.lastHit > icd) {
						sricd.lastHit = Time.fixedTime;
						var stsd = victim.GetComponent<ServerTimedSkillDisable>();
						if(!stsd) stsd = victim.AddComponent<ServerTimedSkillDisable>();
						stsd.ServerApply(duration * count, SkillSlot.Secondary);
						stsd.ServerApply(duration * count, SkillSlot.Utility);
						stsd.ServerApply(duration * count, SkillSlot.Special);
						if(victim.TryGetComponent<CharacterBody>(out var vbody)) {
							vbody.AddTimedBuff(shrinkDebuff, duration * count);
						}
					}
				}
			}
		}

		private void CharacterBody_OnBuffFirstStackGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef) {
			orig(self, buffDef);
			if(self && buffDef == shrinkDebuff && self.modelLocator) {
				self.modelLocator.modelTransform.localScale *= 0.5f;
			}
		}

		private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef) {
			orig(self, buffDef);
			if(self && buffDef == shrinkDebuff && self.modelLocator) {
				self.modelLocator.modelTransform.localScale *= 2f;
			}
		}

		private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self) {
			orig(self);
			if(self && self.HasBuff(shrinkDebuff)) {
				self.damage *= 0.5f;
			}
		}
	}

	public class ShrinkRayICDComponent : MonoBehaviour {
		public float lastHit = 0f;
    }
}