﻿using RoR2;
using UnityEngine;
using TILER2;
using System.Linq;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace ThinkInvisible.TinkersSatchel {
    public class Recombobulator : Equipment<Recombobulator> {
        //TODO: maybe also allow on enemies/bosses?
        //TODO: keep existing cost for stage

        ////// Equipment Data //////

        public override bool isLunar => false;
        public override bool canBeRandomlyTriggered { get; protected set; } = false;
        public override float cooldown { get; protected set; } = 60f;



        ////// Config //////
        
        [AutoConfigRoOString()]
        [AutoConfig("Which object names are allowed for recombobulation (comma-delimited, leading/trailing whitespace will be ignored). Items not in this list will also not be selected as the new object. WARNING: May have unintended results on some untested objects!",
            AutoConfigFlags.PreventNetMismatch | AutoConfigFlags.DeferForever)]
        public string objectNamesConfig { get; private set; } = String.Join(", ", new[] {
            "Turret1Broken",
            "Drone1Broken",
            "Drone2Broken",
            "EquipmentDroneBroken",
            "MissileDroneBroken",
            "FlameDroneBroken",
            "MegaDroneBroken",
            "Chest1",
            "Chest2",
            "GoldChest",
            "CasinoChest",
            "ShrineHealing",
            "EquipmentBarrel",
            "ShrineBlood",
            "ShrineChance",
            "ShrineCombat",
            "ShrineBoss",
            "ShrineCleanse",
            "ShrineRestack",
            "ShrineGoldshoresAccess",
            "CategoryChestDamage",
            "CategoryChestHealing",
            "CategoryChestUtility",
            "Duplicator",
            "DuplicatorLarge",
            "DuplicatorWild",
            "Scrapper",
            "MultiShopTerminal",
            "MultiShopLargeTerminal",
            "MultiShopEquipmentTerminal",
            "LunarChest",
            "ItemDroneBroken",
            "BulwarkDroneBroken"
        });

        [AutoConfigRoOCheckbox()]
        [AutoConfig("If true, interactables will only reroll into other interactables of the same category (chest, shrine, drone, etc.).",
            AutoConfigFlags.PreventNetMismatch)]
        public bool respectCategory { get; private set; } = true;



        ////// Other Fields/Properties //////

        public static HashSet<string> validObjectNames { get; private set; } = new HashSet<string>();
        WeightedSelection<DirectorCard> mostRecentDeck = null;
        Dictionary<DirectorCard, string> mostRecentDeckCategories = null;
        internal static UnlockableDef unlockable;
        public GameObject idrPrefab { get; private set; }


        ////// TILER2 Module Setup //////

        public Recombobulator() {
            modelResource = TinkersSatchelPlugin.resources.LoadAsset<GameObject>("Assets/TinkersSatchel/Prefabs/Items/Recombobulator.prefab");
            iconResource = TinkersSatchelPlugin.resources.LoadAsset<Sprite>("Assets/TinkersSatchel/Textures/ItemIcons/recombobulatorIcon.png");
            idrPrefab = TinkersSatchelPlugin.resources.LoadAsset<GameObject>("Assets/TinkersSatchel/Prefabs/Items/Display/Recombobulator.prefab");
        }

        public override void SetupModifyEquipmentDef() {
            base.SetupModifyEquipmentDef();

            CommonCode.RetrieveDefaultMaterials(idrPrefab.GetComponent<ItemDisplay>());

            #region ItemDisplayRule Definitions

            /// Survivors ///
            displayRules.Add("Bandit2Body", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(-0.17631F, -0.11266F, -0.21485F),
                localAngles = new Vector3(358.1313F, 146.0584F, 33.64409F),
                localScale = new Vector3(0.4F, 0.4F, 0.4F)
            });
            displayRules.Add("CaptainBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(-0.21018F, 0.43699F, 0.06895F),
                localAngles = new Vector3(292.0317F, 223.4014F, 326.501F),
                localScale = new Vector3(0.25F, 0.25F, 0.25F)
            });
            displayRules.Add("CommandoBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(-0.17793F, 0.00849F, 0.08177F),
                localAngles = new Vector3(8.15742F, 17.04626F, 351.2686F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("CrocoBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Stomach",
                localPos = new Vector3(-2.14128F, -0.0825F, 0.46526F),
                localAngles = new Vector3(66.5906F, 251.9885F, 40.36312F),
                localScale = new Vector3(3F, 3F, 3F)
            });
            displayRules.Add("EngiBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Pelvis",
                localPos = new Vector3(-0.28101F, 0.07052F, 0.05267F),
                localAngles = new Vector3(21.36774F, 202.8975F, 193.7145F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("HuntressBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Pelvis",
                localPos = new Vector3(-0.21023F, -0.02377F, -0.06349F),
                localAngles = new Vector3(339.7073F, 185.8784F, 134.9163F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("LoaderBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "MechBase",
                localPos = new Vector3(-0.23316F, -0.1015F, 0.17438F),
                localAngles = new Vector3(282.004F, 302.0504F, 52.99162F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("MageBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Pelvis",
                localPos = new Vector3(-0.1797F, -0.11002F, -0.03818F),
                localAngles = new Vector3(26.87735F, 153.2775F, 164.7757F),
                localScale = new Vector3(0.25F, 0.25F, 0.25F)
            });
            displayRules.Add("MercBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "ThighL",
                localPos = new Vector3(0.1265F, -0.00041F, 0.06297F),
                localAngles = new Vector3(357.7091F, 7.96577F, 174.9853F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("ToolbotBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Chest",
                localPos = new Vector3(-1.55959F, -0.34538F, 0.03459F),
                localAngles = new Vector3(282.6143F, 69.58185F, 288.1964F),
                localScale = new Vector3(3F, 3F, 3F)
            });
            displayRules.Add("TreebotBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "AimOriginSyringe",
                localPos = new Vector3(0.30983F, -0.13462F, -0.38493F),
                localAngles = new Vector3(281.1873F, 61.77537F, 301.2128F),
                localScale = new Vector3(1F, 1F, 1F)
            });
            displayRules.Add("RailgunnerBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "BottomRail",
                localPos = new Vector3(0.04313F, 0.3421F, -0.05488F),
                localAngles = new Vector3(356.7396F, 298.4983F, 183.1082F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            displayRules.Add("VoidSurvivorBody", new ItemDisplayRule {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = idrPrefab,
                childName = "Center",
                localPos = new Vector3(-0.19607F, -0.04863F, -0.01808F),
                localAngles = new Vector3(37.17696F, 17.81638F, 29.12482F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F)
            });
            #endregion
        }

        public override void SetupAttributes() {
            base.SetupAttributes();

            var achiNameToken = $"ACHIEVEMENT_TKSAT_{name.ToUpper(System.Globalization.CultureInfo.InvariantCulture)}_NAME";
            var achiDescToken = $"ACHIEVEMENT_TKSAT_{name.ToUpper(System.Globalization.CultureInfo.InvariantCulture)}_DESCRIPTION";
            unlockable = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockable.cachedName = $"TkSat_{name}Unlockable";
            unlockable.sortScore = 200;
            unlockable.achievementIcon = TinkersSatchelPlugin.resources.LoadAsset<Sprite>("Assets/TinkersSatchel/Textures/UnlockIcons/recombobulatorIcon.png");
            ContentAddition.AddUnlockableDef(unlockable);
            equipmentDef.unlockableDef = unlockable;

            if(Compat_ClassicItems.enabled) {
                Compat_ClassicItems.RegisterEmbryoHook(equipmentDef, "TKSAT_RECOMBOBULATOR_CI_EMBRYO_APPEND", () => "TKSAT.QuantumRecombobulator");
            }
        }

        public override void SetupConfig() {
            base.SetupConfig();
            validObjectNames.UnionWith(objectNamesConfig.Split(',')
                .Select(x => x.Trim()));
        }

        public override void Install() {
            base.Install();
            IL.RoR2.SceneDirector.GenerateInteractableCardSelection += SceneDirector_GenerateInteractableCardSelection;
            On.RoR2.EquipmentSlot.UpdateTargets += EquipmentSlot_UpdateTargets;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            On.RoR2.ScrapperController.BeginScrapping += ScrapperController_BeginScrapping;
            IL.EntityStates.Drone.DeathState.OnImpactServer += DeathState_OnImpactServer;
        }

        public override void Uninstall() {
            base.Uninstall();
            IL.RoR2.SceneDirector.GenerateInteractableCardSelection -= SceneDirector_GenerateInteractableCardSelection;
            On.RoR2.EquipmentSlot.UpdateTargets -= EquipmentSlot_UpdateTargets;
            On.RoR2.PurchaseInteraction.OnInteractionBegin -= PurchaseInteraction_OnInteractionBegin;
            On.RoR2.ScrapperController.BeginScrapping -= ScrapperController_BeginScrapping;
            IL.EntityStates.Drone.DeathState.OnImpactServer -= DeathState_OnImpactServer;
        }



        ////// Private Methods //////

        Dictionary<DirectorCard, string> RetrieveDirectorCardCategories(DirectorCardCategorySelection dccs) {
            var retv = new Dictionary<DirectorCard, string>();
            for(var i = 0; i < dccs.categories.Length; i++) {
                ref var category = ref dccs.categories[i];
                float sumWeights = dccs.SumAllWeightsInCategory(category);
                if(sumWeights <= 0f) continue;
                foreach(DirectorCard directorCard in category.cards) {
                    if(directorCard.IsAvailable())
                        retv[directorCard] = category.name;
                }
            }
            return retv;
        }

        private void DeathState_OnImpactServer(ILContext il) {
            var c = new ILCursor(il);
            if(c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(
                    typeof(GameObject)
                    .GetMethod(nameof(GameObject.GetComponent), new Type[] { })
                    .MakeGenericMethod(typeof(PurchaseInteraction)))
                )) {
                c.Emit(OpCodes.Dup);
                c.EmitDelegate<Action<GameObject>>(obj => {
                    Debug.Log($"Adding RecombobulatorFlag to {obj}");
                    obj.AddComponent<RecombobulatorFlag>();
                });
            }
        }

        private GameObject FindNearestRerollable(GameObject senderObj, Ray aim, float maxAngle, float maxDistance, bool requireLoS) {
            aim = CameraRigController.ModifyAimRayIfApplicable(aim, senderObj, out float camAdjust);
            var results = Physics.OverlapSphere(aim.origin, maxDistance + camAdjust, Physics.AllLayers, QueryTriggerInteraction.Collide);
            var minDot = Mathf.Cos(Mathf.Clamp(maxAngle, 0f, 180f) * Mathf.PI / 180f);
            GameObject retv = null;
            var lowestC = float.MaxValue;
            foreach(var obj in results) {
                if(!obj || !obj.gameObject) continue;
                var root = MiscUtil.GetRootWithLocators(obj.gameObject);
                if(!validObjectNames.Contains(root.name.Replace("(Clone)", ""))) continue;
                var vdot = Vector3.Dot(aim.direction, (root.transform.position - aim.origin).normalized);
                if(vdot < minDot) continue;
                if(requireLoS && !Physics.Linecast(aim.origin, root.transform.position, LayerIndex.world.mask))
                    continue;
                var c = vdot * Vector3.Distance(root.transform.position, aim.origin);
                if(c < lowestC) {
                    lowestC = c;
                    retv = root;
                }
            }
            return retv;
        }



        ////// Hooks //////

        private void ScrapperController_BeginScrapping(On.RoR2.ScrapperController.orig_BeginScrapping orig, ScrapperController self, int intPickupIndex) {
            orig(self, intPickupIndex);
            if(self && !self.GetComponent<RecombobulatorFlag>())
                self.gameObject.AddComponent<RecombobulatorFlag>();
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator) {
            orig(self, activator);
            if(self && self.CanBeAffordedByInteractor(activator) && !self.GetComponent<RecombobulatorFlag>())
                self.gameObject.AddComponent<RecombobulatorFlag>();
        }

        private void SceneDirector_GenerateInteractableCardSelection(ILContext il) {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                i => i.MatchCallOrCallvirt<DirectorCardCategorySelection>(
                    nameof(DirectorCardCategorySelection.GenerateDirectorCardWeightedSelection)
                    )
                );
            c.EmitDelegate<Func<DirectorCardCategorySelection, DirectorCardCategorySelection>>(dccs => {
                mostRecentDeckCategories = RetrieveDirectorCardCategories(dccs);
                return dccs;
            });
            c.GotoNext(MoveType.After,
                i => i.MatchCallOrCallvirt<DirectorCardCategorySelection>(
                    nameof(DirectorCardCategorySelection.GenerateDirectorCardWeightedSelection)
                    )
                );
            c.EmitDelegate<Func<WeightedSelection<DirectorCard>, WeightedSelection<DirectorCard>>>(wsdc => {
                mostRecentDeck = wsdc;
                return wsdc;
            });
        }

        private void EquipmentSlot_UpdateTargets(On.RoR2.EquipmentSlot.orig_UpdateTargets orig, EquipmentSlot self, EquipmentIndex targetingEquipmentIndex, bool userShouldAnticipateTarget) {
            if(targetingEquipmentIndex != catalogIndex) {
                orig(self, targetingEquipmentIndex, userShouldAnticipateTarget);
                return;
            }

            var res = FindNearestRerollable(self.gameObject, self.GetAimRay(), 10f, 20f, false);
            Transform tsf = null;
            if(res) tsf = res.transform;
            self.currentTarget = new EquipmentSlot.UserTargetInfo {
                transformToIndicateAt = tsf,
                pickupController = null,
                hurtBox = null,
                rootObject = res
            };

            if(self.currentTarget.rootObject != null) {
                var purch = res.GetComponent<PurchaseInteraction>();
                if(!res.GetComponent<RecombobulatorFlag>() && (!purch || purch.available)) {
                    self.targetIndicator.visualizerPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/RecyclerIndicator");
                } else {
                    self.targetIndicator.visualizerPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/RecyclerBadIndicator");
                }

                self.targetIndicator.active = true;
                self.targetIndicator.targetTransform = self.currentTarget.transformToIndicateAt;
            } else {
                self.targetIndicator.active = false;
            }
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot) {
            if(slot.currentTarget.rootObject
                && validObjectNames.Contains(slot.currentTarget.rootObject.name.Replace("(Clone)",""))
                && !slot.currentTarget.rootObject.GetComponent<RecombobulatorFlag>()
                && mostRecentDeck != null
                && Run.instance) {

                var oldPurch = slot.currentTarget.rootObject.GetComponent<PurchaseInteraction>();
                if(oldPurch) {
                    if(!oldPurch.available) return false;
                }

                var shopcpt = slot.currentTarget.rootObject.GetComponent<ShopTerminalBehavior>();
                if(shopcpt && shopcpt.serverMultiShopController) {
                    slot.currentTarget.rootObject = shopcpt.serverMultiShopController.transform.root.gameObject;
                    foreach(var term in shopcpt.serverMultiShopController.terminalGameObjects)
                        GameObject.Destroy(term);
                }

                GameObject.Destroy(slot.currentTarget.rootObject);

                var pos = slot.currentTarget.rootObject.transform.position;

                WeightedSelection<DirectorCard> filteredDeck = new(8);
                var matchCategories = mostRecentDeckCategories.Where(kvp => kvp.Key.spawnCard.prefab.name == slot.currentTarget.rootObject.name.Replace("(Clone)","")).Select(kvp => kvp.Value);
                for(var i = 0; i < mostRecentDeck.Count; i++) {
                    var card = mostRecentDeck.GetChoice(i);
                    if(card.value == null || !card.value.IsAvailable()) continue;
                    if(!validObjectNames.Contains(card.value.spawnCard.prefab.name))
                        continue;
                    if(respectCategory && (
                        !mostRecentDeckCategories.TryGetValue(card.value, out var thisCategory)
                        || !matchCategories.Contains(thisCategory)
                        ))
                        continue;
                    filteredDeck.AddChoice(card);
                }
                if(filteredDeck.Count == 0)
                    return false;

                var draw = filteredDeck.Evaluate(rng.nextNormalizedFloat);

                if(Compat_ClassicItems.enabled) {
                    var rerolls = Compat_ClassicItems.CheckEmbryoProc(slot, equipmentDef);
                    for(var i = 0; i < rerolls; i++) {
                        var draw2 = filteredDeck.Evaluate(rng.nextNormalizedFloat);
                        if(draw2.selectionWeight < draw.selectionWeight)
                            draw = draw2;
                    }
                }

                var obj = DirectorCore.instance.TrySpawnObject(
                    new DirectorSpawnRequest(
                        draw.spawnCard,
                        new DirectorPlacementRule {
                            placementMode = DirectorPlacementRule.PlacementMode.Direct,
                            position = pos,
                            preventOverhead = false
                        },
                        this.rng
                        ));
                if(!obj) {
                    TinkersSatchelPlugin._logger.LogError("Recombobulator failed to replace interactable!");
                    return false;
                }
                var purch = obj.GetComponent<PurchaseInteraction>();
                if(purch && purch.costType == CostTypeIndex.Money) {
                    purch.Networkcost = Run.instance.GetDifficultyScaledCost(purch.cost);
                }
                obj.AddComponent<RecombobulatorFlag>();

                var shopcpt2 = obj.GetComponent<MultiShopController>();
                if(shopcpt2) {
                    foreach(var term in shopcpt2.terminalGameObjects)
                        term.AddComponent<RecombobulatorFlag>();
                }

                return true;
            }
            return false;
        }
    }

    public class RecombobulatorFlag : MonoBehaviour {}

    [RegisterAchievement("TkSat_Recombobulator", "TkSat_RecombobulatorUnlockable", "")]
    public class TkSatRecombobulatorAchievement : RoR2.Achievements.BaseAchievement {
        public override void OnInstall() {
            base.OnInstall();

            IL.RoR2.EquipmentSlot.FireRecycle += EquipmentSlot_FireRecycle;
        }

        public override void OnUninstall() {
            base.OnUninstall();

            IL.RoR2.EquipmentSlot.FireRecycle -= EquipmentSlot_FireRecycle;
        }

        private void EquipmentSlot_FireRecycle(ILContext il) {
            ILCursor c = new(il);

            if(c.TryGotoNext(MoveType.Before, x => x.MatchCallOrCallvirt<GenericPickupController>("set_NetworkpickupIndex"))) {
                c.Emit(OpCodes.Dup);
                c.EmitDelegate<Action<PickupIndex>>(pind => {
                    var pdef = PickupCatalog.GetPickupDef(pind);
                    if(pdef != null
                    && (pdef.itemTier == ItemTier.Tier3 || pdef.itemTier == ItemTier.VoidTier3
                    || pdef.itemTier == ItemTier.Boss || pdef.itemTier == ItemTier.VoidBoss))
                        Grant();
                });
            } else {
                TinkersSatchelPlugin._logger.LogError("TkSatRecombobulatorAchievement: failed to apply IL hook (EquipmentSlot_FireRecycle); could not find target instructions. Achievement will not trigger.");
            }
        }
    }
}