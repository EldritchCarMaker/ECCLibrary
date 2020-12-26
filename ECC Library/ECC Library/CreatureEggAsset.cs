﻿using SMLHelper.V2.Assets;
using SMLHelper.V2.Utility;
using SMLHelper.V2.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;
using HarmonyLib;
using ECCLibrary;

namespace ECCLibrary
{
    public class CreatureEggAsset : Spawnable
    {
        private GameObject model;
        protected GameObject prefab;
        private TechType hatchingCreature;
        private Atlas.Sprite sprite;
        Texture2D spriteTexture;
        static LiveMixinData eggLiveMixinData;
        float hatchingTime;

        public CreatureEggAsset(string classId, string friendlyName, string description, GameObject model, TechType hatchingCreature, Texture2D spriteTexture, float hatchingTime) : base(classId, friendlyName, description)
        {
            this.model = model;
            this.hatchingCreature = hatchingCreature;
            this.spriteTexture = spriteTexture;
            this.hatchingTime = hatchingTime;
        }

        public override WorldEntityInfo EntityInfo => new WorldEntityInfo()
        {
            slotType = EntitySlot.Type.Small,
            cellLevel = LargeWorldEntity.CellLevel.Near,
            classId = ClassID,
            techType = TechType
        };

        new public void Patch()
        {
            sprite = ImageUtils.LoadSpriteFromTexture(spriteTexture);
            if(eggLiveMixinData == null)
            {
                eggLiveMixinData = ECCHelpers.CreateNewLiveMixinData();
                eggLiveMixinData.destroyOnDeath = true;
                eggLiveMixinData.explodeOnDestroy = true;
                eggLiveMixinData.maxHealth = GetMaxHealth;
                eggLiveMixinData.knifeable = true;
            }
            base.Patch();
            if (AcidImmune)
            {
                ECCHelpers.MakeAcidImmune(TechType);
            }
            ScannableSettings.AttemptPatch(this, GetEncyTitle, GetEncyDesc);
        }

        public override GameObject GetGameObject()
        {
            if(prefab == null)
            {
                prefab = model;
                prefab.AddComponent<PrefabIdentifier>().ClassId = ClassID;
                prefab.AddComponent<TechTag>().type = TechType;
                prefab.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
                SkyApplier skyApplier = prefab.AddComponent<SkyApplier>();
                skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();

                Pickupable pickupable = prefab.AddComponent<Pickupable>();

                LiveMixin lm = prefab.AddComponent<LiveMixin>();
                lm.data = eggLiveMixinData;
                lm.health = GetMaxHealth;

                VFXSurface surface = prefab.AddComponent<VFXSurface>();
                surface.surfaceType = VFXSurfaceTypes.organic;

                WaterParkItem waterParkItem = prefab.AddComponent<WaterParkItem>();
                waterParkItem.pickupable = pickupable;

                Rigidbody rb = prefab.EnsureComponent<Rigidbody>();
                rb.mass = 10f;
                rb.isKinematic = true;

                WorldForces worldForces = prefab.EnsureComponent<WorldForces>();
                worldForces.useRigidbody = rb;

                CreatureEgg egg = prefab.AddComponent<CreatureEgg>();
                egg.animator = prefab.GetComponentInChildren<Animator>();
                egg.hatchingCreature = hatchingCreature;
                egg.overrideEggType = TechType;
                egg.daysBeforeHatching = hatchingTime;

                EntityTag entityTag = prefab.AddComponent<EntityTag>();
                entityTag.slotType = EntitySlot.Type.Small;
                ECCHelpers.ApplySNShaders(prefab);
            }
            return prefab;
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            if (prefab == null)
            {
                prefab = model;
                prefab.AddComponent<PrefabIdentifier>().ClassId = ClassID;
                prefab.AddComponent<TechTag>().type = TechType;
                prefab.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
                SkyApplier skyApplier = prefab.AddComponent<SkyApplier>();
                skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();

                Pickupable pickupable = prefab.AddComponent<Pickupable>();

                LiveMixin lm = prefab.AddComponent<LiveMixin>();
                lm.data = eggLiveMixinData;
                lm.health = GetMaxHealth;

                VFXSurface surface = prefab.AddComponent<VFXSurface>();
                surface.surfaceType = VFXSurfaceTypes.organic;

                WaterParkItem waterParkItem = prefab.AddComponent<WaterParkItem>();
                waterParkItem.pickupable = pickupable;

                Rigidbody rb = prefab.EnsureComponent<Rigidbody>();
                rb.mass = 10f;
                rb.isKinematic = true;

                WorldForces worldForces = prefab.EnsureComponent<WorldForces>();
                worldForces.useRigidbody = rb;

                CreatureEgg egg = prefab.AddComponent<CreatureEgg>();
                egg.animator = prefab.GetComponentInChildren<Animator>();
                egg.hatchingCreature = hatchingCreature;
                egg.overrideEggType = TechType;
                egg.daysBeforeHatching = hatchingTime;

                EntityTag entityTag = prefab.AddComponent<EntityTag>();
                entityTag.slotType = EntitySlot.Type.Small;
                ECCHelpers.ApplySNShaders(prefab);
            }
            yield return null;
            gameObject.Set(prefab);
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return sprite;
        }

        public virtual bool AcidImmune
        {
            get
            {
                return false;
            }
        }

        public virtual float GetMaxHealth
        {
            get
            {
                return 20f;
            }
        }

        public virtual string GetEncyTitle
        {
            get
            {
                return FriendlyName;
            }
        }

        public virtual string GetEncyDesc
        {
            get
            {
                return string.Format("The egg of a {0}.", FriendlyName);
            }
        }

        public virtual ScannableItemData ScannableSettings
        {
            get
            {
                return new ScannableItemData(true, 2f, "Lifeforms/Fauna/Eggs", new string[] { "Lifeforms", "Fauna", "Eggs" }, Sprite.Create(sprite.texture, new Rect(Vector2.zero, new Vector2(sprite.texture.width, sprite.texture.height)), new Vector2(0.5f, 0.5f)), null);
            }
        }
    }
}
