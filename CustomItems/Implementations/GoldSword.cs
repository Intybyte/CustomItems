using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomItems.Implementations
{
    internal class GoldSword : BaseCustomItem
    {

        public override void TriggerEffect(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem)
        {
            Debug.Log("Start Gold");
            int gold = StatsManager.Instance.playerGold;
            Debug.Log("Got Gold");
            int bonusAtk = gold / 10;
            if(bonusAtk == 0) return;
            Debug.Log("Gold nope");
            battleSystem.EnqueueEffect(battleSystem.CO_DamageTrigger(HicGeneric.GetOppositeTurn(this.player), bonusAtk, DamageSource.ATTACK, this.sprite, this));
            Debug.Log("Enqueued");
        }

        public void OnHit(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem, EffectBase effectSource)
        {
            battleSystem.PushToBattleStack(null, new Action<Dictionary<BattleTurn, BattleStats>, BattleSystem>(battleSystem.UpdatePlayerAttack));
            battleSystem.PushToBattleStack(this, new Action<Dictionary<BattleTurn, BattleStats>, BattleSystem>(this.TriggerEffect));
        }

        public override void TriggerOnStartOfBattle(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem, List<InventoryItem> equippedItems, List<EffectBase> allEquippedEffects, BattleTurn turn)
        {
            battleSystem.battleEvents.Subscribe("OnHit", new BattleEvents.EventAction(this.OnHit), this, turn, false);
        }

        public override void TriggerOnEndOfBattle(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem, List<InventoryItem> equippedItems, List<EffectBase> allEquippedEffects, BattleTurn turn)
        {
            battleSystem.battleEvents.Unsubscribe("OnHit", new BattleEvents.EventAction(this.OnHit), this, turn, false);
        }
    }
}
