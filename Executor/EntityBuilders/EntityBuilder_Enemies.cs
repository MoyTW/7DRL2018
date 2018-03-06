using Executor.AI;
using Executor.Components;
using Executor.Enums;

using RogueSharp.Random;
using System;
using System.Collections.Generic;

namespace Executor.EntityBuilders
{
    public static class EntityBuilder_Enemies
    {

        #region Level 0 Foes

        public static Entity BuildRentACop(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_SeekMaxWeaponRange());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildPistol());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return entity;
        }

        public static Entity BuildRentACopBrave(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_AvoidMelee());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildPistol());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return entity;
        }

        public static Entity BuildRentACopCowardly(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Flee());

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildPistol());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return entity;
        }

        public static Entity BuildRentACopBaton(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildBaton());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            var cse = new StatusEffect_DamageResistance(-1, DamageType.SLASHING, Enums.ResistStrength.MAJOR);
            var buffable = entity.GetComponentOfType<Component_Buffable>();
            buffable.ForceAddActiveStatusEffect(cse);

            return entity;
        }

        public static Entity BuildRentACopBatonCowardly(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Flee());

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildBaton());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return entity;
        }

        public static Entity BuildLevel0Entity(IRandom rand, string designation)
        {
            return EntityBuilder_Enemies.BuildRentACopBaton(designation);

            /*
            var selection = rand.Next(4);
            switch (selection)
            {
                case 0:
                    return EntityBuilderEnemies.BuildRentACop(designation);
                case 1:
                    return EntityBuilderEnemies.BuildRentACopCowardly(designation);
                case 2:
                    return EntityBuilderEnemies.BuildRentACopBrave(designation);
                case 3:
                    return EntityBuilderEnemies.BuildRentACopBaton(designation);
                case 4:
                    return EntityBuilderEnemies.BuildRentACopBatonCowardly(designation);
                default:
                    throw new InvalidOperationException();
            }
            */
        }

        #endregion

        #region Level 1 Foes

        public static Entity BuildPistolPoliceman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_KeepMediumRange());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Policeman " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildPistol());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            return entity;
        }

        public static Entity BuildShotgunPoliceman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_KeepMediumRange());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Policeman " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildShotgun());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            return entity;
        }

        public static Entity BuildBraveShotgunPoliceman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_AvoidMelee());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Policeman " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildShotgun());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            return entity;
        }

        public static Entity BuildRiflePoliceman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_KeepMediumRange());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Policeman " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildRifle());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            return entity;
        }

        public static Entity BuildBraveRiflePoliceman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_AvoidMelee());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Policeman " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildRifle());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            return entity;
        }

        public static Entity BuildPolicemanBruiser(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Policeman " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildBaton());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            EntityBuilder.SlotAt(entity, BodyPartLocation.HEAD, EntityBuilder.BuildHelmet());
            EntityBuilder.SlotAt(entity, BodyPartLocation.TORSO, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.TORSO, EntityBuilder.BuildArmorPart());

            return entity;
        }

        public static Entity BuildPolicemanHeavy(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Policeman " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilder_Weapons.BuildPistol());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            EntityBuilder.SlotAt(entity, BodyPartLocation.HEAD, EntityBuilder.BuildHelmet());
            EntityBuilder.SlotAt(entity, BodyPartLocation.TORSO, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.TORSO, EntityBuilder.BuildArmorPart());

            return entity;
        }

        public static Entity BuildLevel1Entity(IRandom rand, string designation)
        {
            return BuildLevel0Entity(rand, designation);
            /*
            var selection = rand.Next(6);
            switch (selection)
            {
                case 0:
                    return EntityBuilderEnemies.BuildPistolPoliceman(designation);
                case 1:
                    return EntityBuilderEnemies.BuildShotgunPoliceman(designation);
                case 2:
                    return EntityBuilderEnemies.BuildBraveShotgunPoliceman(designation);
                case 3:
                    return EntityBuilderEnemies.BuildRiflePoliceman(designation);
                case 4:
                    return EntityBuilderEnemies.BuildBraveShotgunPoliceman(designation);
                case 5:
                    return EntityBuilderEnemies.BuildPolicemanBruiser(designation);
                case 6:
                    return EntityBuilderEnemies.BuildPolicemanHeavy(designation);
                default:
                    throw new InvalidOperationException();
            }
            */
        }

        #endregion

        #region Level 2 Foes

        private static Entity BuildBaseSWAT(List<ActionClause> clauses, Entity carryRight, string designation)
        {
            var entity = EntityBuilder.BuildNakedMech("SWAT Responder " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, carryRight);
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            EntityBuilder.SlotAt(entity, BodyPartLocation.HEAD, EntityBuilder.BuildHelmet());
            EntityBuilder.SlotAt(entity, BodyPartLocation.TORSO, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.TORSO, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.TORSO, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.LEFT_LEG, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.RIGHT_LEG, EntityBuilder.BuildArmorPart());

            return entity;
        }

        public static Entity BuildSWATAssaultRifleman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_KeepMediumRange());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = BuildBaseSWAT(clauses, EntityBuilder_Weapons.BuildAssaultRifle(), designation);
            
            return entity;
        }

        public static Entity BuildBraveSWATAssaultRifleman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_AvoidMelee());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = BuildBaseSWAT(clauses, EntityBuilder_Weapons.BuildAssaultRifle(), designation);

            return entity;
        }

        public static Entity BuildSWATMarksman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_KeepMediumRange());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = BuildBaseSWAT(clauses, EntityBuilder_Weapons.BuildCombatRifle(), designation);

            return entity;
        }

        public static Entity BuildSWATRiotPolice(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = BuildBaseSWAT(clauses, EntityBuilder_Weapons.HFBaton(), designation);

            return entity;
        }

        public static Entity BuildSWATShockPolice(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = BuildBaseSWAT(clauses, EntityBuilder_Weapons.HFBaton(), designation);

            EntityBuilder.SlotAt(entity, BodyPartLocation.TORSO, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.LEFT_LEG, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.RIGHT_LEG, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.TORSO, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.LEFT_LEG, EntityBuilder.BuildArmorPart());
            EntityBuilder.SlotAt(entity, BodyPartLocation.RIGHT_LEG, EntityBuilder.BuildArmorPart());

            return entity;
        }

        public static Entity BuildLevel2Entity(IRandom rand, string designation)
        {
            return BuildLevel0Entity(rand, designation);

            /*
            var selection = rand.Next(4);
            switch (selection)
            {
                case 0:
                    return EntityBuilderEnemies.BuildSWATAssaultRifleman(designation);
                case 1:
                    return EntityBuilderEnemies.BuildBraveSWATAssaultRifleman(designation);
                case 2:
                    return EntityBuilderEnemies.BuildSWATMarksman(designation);
                case 3:
                    return EntityBuilderEnemies.BuildSWATRiotPolice(designation);
                case 4:
                    return EntityBuilderEnemies.BuildSWATShockPolice(designation);
                default:
                    throw new InvalidOperationException();
            }
            */
        }

        #endregion

        public static Entity BuildRandomLevelledEntity(IRandom rand, string designation, int level)
        {
            switch(level)
            {
                case 0:
                    return EntityBuilder_Enemies.BuildLevel0Entity(rand, designation);
                case 1:
                    return EntityBuilder_Enemies.BuildLevel1Entity(rand, designation);
                case 2:
                    return EntityBuilder_Enemies.BuildLevel2Entity(rand, designation);
                default:
                    throw new InvalidOperationException();
            }
        }

    }
}

