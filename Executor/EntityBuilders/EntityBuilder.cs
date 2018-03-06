using Executor.GameQueries;

using Executor.AI;
using Executor.Components;
using Executor.Enums;
using Executor.GameEvents;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor.EntityBuilders
{
    public static class EntityBuilder
    {
        #region Values

        // TODO: Hardcoded values all over!
        public const string SlottablePartTypeLabel = "Slottable Part";
        public const string BodyPartTypeLabel = "Body Part";
        public const string MechTypeLabel = "Mech";
        public const string WeaponTypeLabel = "Weapon";

        public static readonly List<BodyPartLocation> MechLocations = new List<BodyPartLocation> {
            BodyPartLocation.HEAD,
            BodyPartLocation.TORSO,
            BodyPartLocation.LEFT_ARM,
            BodyPartLocation.RIGHT_ARM,
            BodyPartLocation.LEFT_LEG,
            BodyPartLocation.RIGHT_LEG
        };

        #endregion

        #region Utilities

        private static Entity GetBodyPart(Entity mech, BodyPartLocation location)
        {
            return mech.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART))
                .SubEntities
                .Where(e => e.GetComponentOfType<Component_BodyPartLocation>().Location == location)
                .First();
        }

        public static void AddToInventory(Entity mech, Entity item)
        {
            mech.HandleEvent(new GameEvent_AddToInventory(mech, item));
        }

        public static void SlotAt(Entity mech, BodyPartLocation location, Entity slottable)
        {
            var bodyPart = GetBodyPart(mech, location);
            bodyPart.HandleEvent(new GameEvent_Slot(mech, bodyPart, slottable));
        }

        private static void FillLocationWith(Entity mech, BodyPartLocation location, Func<Entity> buildFn)
        {
            var container = GetBodyPart(mech, location).GetComponentOfType<Component_SlottedContainer>();
            var toAdd = buildFn();
            while (container.SlotsRemaining >= toAdd.GetComponentOfType<Component_Slottable>().SlotsRequired)
            {
                SlotAt(mech, location, toAdd);
                toAdd = buildFn();
            }
        }

        public static Entity MountOntoArm(Entity mech, BodyPartLocation location, Entity mountable)
        {
            var armActuator = GetBodyPart(mech, location)
                .TryGetSubEntities(SubEntitiesSelector.SWAPPABLE_ATTACH_POINTS)
                .Where(e => e.Label == Blueprints.HAND)
                .First();
            armActuator.HandleEvent(new GameEvent_Slot(mech, armActuator, mountable));
            return mech;
        }

        #endregion

        #region Slottable Parts

        public static Entity BuildHelmet()
        {
            return new Entity(label: "Helmet", typeLabel: "Armor")
                .AddComponent(new Component_Slottable(2))
                .AddComponent(new Component_InternalStructure(10));
        }

        public static Entity BuildArmorPart()
        {
            return BlueprintListing.BuildForLabel(Blueprints.ARMOR);
        }

        public static Entity BuildPhoneScanner()
        {
            return new Entity(label: "Phone Scanner", typeLabel: "Scanner")
                .AddComponent(new Component_Attachable(AttachmentSize.SMALL))
                .AddComponent(new Component_AttributeModifier(EntityAttributeType.DETECTION_RADIUS, ModifierType.FLAT, 2));
        }

        public static Entity BuildHandheldScanner()
        {
            return new Entity(label: "Hand Scanner", typeLabel: "Scanner")
                .AddComponent(new Component_Attachable(AttachmentSize.SMALL))
                .AddComponent(new Component_AttributeModifier(EntityAttributeType.DETECTION_RADIUS, ModifierType.FLAT, 3));
        }

        public static Entity BuildDutyScanner()
        {
            return new Entity(label: "Duty Scanner", typeLabel: "Scanner")
                .AddComponent(new Component_Attachable(AttachmentSize.SMALL))
                .AddComponent(new Component_AttributeModifier(EntityAttributeType.DETECTION_RADIUS, ModifierType.FLAT, 4));
        }

        #endregion

        #region Base

        public static Entity BuildBodyPart(BodyPartLocation location, int slotSpace, int internalStructure)
        {
            return new Entity(label: location.ToString(), typeLabel: "BodyPart")
                .AddComponent(new Component_BodyPartLocation(location))
                .AddComponent(new Component_SlottedContainer(slotSpace))
                .AddComponent(new Component_SlottedStructure())
                .AddComponent(new Component_InternalStructure(internalStructure));
        }

        public static Entity BuildNakedMech(string label, bool player, Guidebook book)
        {
            var mech = new Entity(label: label, typeLabel: MechTypeLabel)
                .AddComponent(new Component_Buffable())
                .AddComponent(new Component_ActionExecutor(Config.DefaultEntityAP))
                .AddComponent(new Component_Inventory())
                .AddComponent(new Component_Skeleton());

            if (player)
                mech.AddComponent(new Component_Player());
            else if (book != null)
                mech.AddComponent(new Component_AI(book));
            else
                throw new InvalidOperationException("book can't be null!");
            
            SlotAt(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.HAND));
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.HAND));

            // Slot in all the required components
            return mech;
        }

        public static Entity BuildPlayerEntity()
        {
            var mech = BuildNakedMech("You", true, null);

            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildHFBlade());

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildSuicidePotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildSuicidePotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildSuicidePotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildSuicidePotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            AddToInventory(mech, EntityBuilder_Inventoriables.BuildIronskinPotion());
            
            return mech;
        }

        #endregion
        
    }
}
