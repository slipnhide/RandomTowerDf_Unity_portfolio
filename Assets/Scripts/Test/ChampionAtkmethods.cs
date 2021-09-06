using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface RangeChampionAtk { void Attack(RangeChampion rangeChampion); void AttackRpc(RangeChampion rangeChampion,string[] TargetNumbers); }
public abstract class RangeChampionAtkPclass : RangeChampionAtk
{
   // public abstract void Atk(RangeChampion rangeChampion);

    public static RangeChampionAtkPclass Product(RangeJob job)
    {
        switch (job)
        {
            case RangeJob.Novel:
                return new NovelAttack();
            case RangeJob.scout:
                return new ScoutAttack();
            case RangeJob.Mage:
                return new MageAttack();
        }
        return null;
    }

    public abstract void Attack(RangeChampion rangeChampion);
    public abstract void AttackRpc(RangeChampion rangeChampion, string[] TargetNumbers);

    public class NovelAttack : RangeChampionAtkPclass
    {
        public override void Attack(RangeChampion rangeChampion)
        {
            rangeChampion.novelAtk();
        }

        public override void AttackRpc(RangeChampion rangeChampion, string[] TargetNumbers)
        {
            rangeChampion.novelAtkRpc(TargetNumbers);
        }
    }
    public class MageAttack : RangeChampionAtkPclass
    {
        public override void Attack(RangeChampion rangeChampion)
        {
            rangeChampion.MageAtk();
        }

        public override void AttackRpc(RangeChampion rangeChampion, string[] TargetNumbers)
        {
            rangeChampion.MageAtkRpc(TargetNumbers);
        }
    }
    public class ScoutAttack : RangeChampionAtkPclass
    {
        public override void Attack(RangeChampion rangeChampion)
        {
            rangeChampion.ScoutAtk();
        }

        public override void AttackRpc(RangeChampion rangeChampion, string[] TargetNumbers)
        {
            rangeChampion.ScoutAtkRpc(TargetNumbers);
        }
    }
}
public interface MeleeChampionAtk { 
    void Attack(MeleeChampion meleeChampion); 
    void AttackRpc(MeleeChampion meleeChampion, string[] TargetNumbers); 
}
public abstract class MeleeChampionAtkPclass : MeleeChampionAtk
{

    public static MeleeChampionAtk Product(MeleeJob job)
    {
        switch (job)
        {
            case MeleeJob.assassin:
                return new AssasineAttack();
            case MeleeJob.Warrior:
                return new WorrierAttack();

        }
        return null;
    }

    public abstract void Attack(MeleeChampion meleeChampion);
    public abstract void AttackRpc(MeleeChampion meleeChampion, string[] TargetNumbers);

    public class AssasineAttack : MeleeChampionAtkPclass
    {
        public override void Attack(MeleeChampion meleeChampion)
        {
            meleeChampion.assasineAtk();
        }

        public override void AttackRpc(MeleeChampion meleeChampion, string[] TargetNumbers)
        {
            meleeChampion.assasineAtkRpc(TargetNumbers);
        }
    }
    public class WorrierAttack : MeleeChampionAtkPclass
    {
        public override void Attack(MeleeChampion meleeChampion)
        {
            meleeChampion.WorrierAtk();
        }

        public override void AttackRpc(MeleeChampion meleeChampion, string[] TargetNumbers)
        {
            meleeChampion.WorrierAtkRpc(TargetNumbers);
        }
    }
}


public class ChampionAtkmethods : MonoBehaviour
{ }


