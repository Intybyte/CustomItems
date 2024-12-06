using CustomItems.Items;
using System.Collections.Generic;
using System;

namespace CustomItems.Implementations
{
    internal class GoldWarrior : BaseCustomSet
    {
        public override void TriggerEffect(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem)
        {
            int hp = battleStats[player].health;
            int gold = StatsManager.Instance.playerGold;
            int damageDealt = battleStats[BattleTurn.ENEMY].damageDoneThisAttack;

            if(hp <= damageDealt && gold <= damageDealt)
            {
                StatsManager.Instance.playerGold -= damageDealt;
                battleSystem.SetHealthTrigger(this.player, hp);
                battleSystem.EnqueueEffect(battleSystem.CO_IconTrigger(
                    this.player,
                    StatsManager.Instance.playerGold, 
                    this.GetSprite(), 
                    AffectedTriggerStat.HEALTH)
                );
            }            
        }

        public void Resilience(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem, EffectBase effectSource)
        {
            int num = 0;
            while ((float)num < this.ongoingTriggerEffectMultiplier)
            {
                battleSystem.PushToBattleStack(this, new Action<Dictionary<BattleTurn, BattleStats>, BattleSystem>(this.TriggerEffect));
                num++;
            }
        }

        public override void TriggerOnStartOfBattle(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem, List<InventoryItem> equippedItems, List<EffectBase> allEquippedEffects, BattleTurn turn)
        {
            battleSystem.battleEvents.Subscribe(EventTypes.Resilience, new BattleEvents.EventAction(this.Resilience), this, turn, false);
        }

        public override void TriggerOnEndOfBattle(Dictionary<BattleTurn, BattleStats> battleStats, BattleSystem battleSystem, List<InventoryItem> equippedItems, List<EffectBase> allEquippedEffects, BattleTurn turn)
        {
            battleSystem.battleEvents.Unsubscribe(EventTypes.Resilience, new BattleEvents.EventAction(this.Resilience), this, turn, false);
        }
    }
}
