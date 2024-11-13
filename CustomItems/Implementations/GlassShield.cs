using CustomItems.Registry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomItems.Implementations
{
    internal class GlassShield : BaseCustomItem
    {
        private static bool active = false;
        private static readonly Sprite scaledSprite = SpriteRegistry.ScaleSprite("Glass Shield", 4);

        public override void TriggerEffect(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem)
        {
            if (!active) return;

            int damageDealt = battleStats[BattleTurn.ENEMY].damageDoneThisAttack;

            battleSystem.EnqueueEffect(
                battleSystem.CO_DamageTrigger(
                    BattleTurn.ENEMY,
                    damageDealt, 
                    DamageSource.NORMAL, 
                    scaledSprite, 
                    this
                )
            );

            active = false;
        }

        public void TakeDamage(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem, EffectBase effectSource)
        {
            battleSystem.PushToBattleStack(this, new Action<Dictionary<BattleTurn, BattleStats>, BattleSystem>(this.TriggerEffect));
        }

        public override void TriggerOnStartOfBattle(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem, List<InventoryItem> equippedItems, List<EffectBase> allEquippedEffects, BattleTurn turn)
        {
            active = true;
            battleSystem.battleEvents.Subscribe("TakeDamage", new BattleEvents.EventAction(this.TakeDamage), this, turn, false);
        }

        public override void TriggerOnEndOfBattle(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem, List<InventoryItem> equippedItems, List<EffectBase> allEquippedEffects, BattleTurn turn)
        {
            battleSystem.battleEvents.Unsubscribe("TakeDamage", new BattleEvents.EventAction(this.TakeDamage), this, turn, false);
        }

        public override void SetGoldenItem(InventoryItem item)
        {
            this.attack = 1;
            this.armor = 2;
            itemRarity = ItemRarity.GOLDEN;
        }
    }
}
