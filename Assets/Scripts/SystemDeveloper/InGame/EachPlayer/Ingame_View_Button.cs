using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Ingame_Button { Roll, BuyXp, CheckSynergy,Ready,Stun,Angel,Scout }
public abstract class Ingame_Command
{
    //전략패턴- 패턴 이름을 잘 못 사용
    public static Ingame_Command MakeCommand(Ingame_Button ingame_Button)
    {
        switch (ingame_Button)
        {
            case Ingame_Button.Roll:
                return new RollButton();
            case Ingame_Button.BuyXp:
                return new BuyXp();
            case Ingame_Button.CheckSynergy:
                return new CheckSynergy();
            case Ingame_Button.Ready:
                return new Ready();
            case Ingame_Button.Stun:
                return new Stun();
            case Ingame_Button.Angel:
                return new AngelFall();
            case Ingame_Button.Scout:
                return new RainArrow();

        }
        return null;
    }

    public abstract void Excute();
}

public class RainArrow : Ingame_Command
{
    public override void Excute()
    {
        Ingame_View_Controler.ingame_View_Controler.ArrowSkill();
    }
}
public class AngelFall : Ingame_Command
{
    public override void Excute()
    {
        Ingame_View_Controler.ingame_View_Controler.AngelSkill();
    }
}
public class Stun : Ingame_Command
{
    public override void Excute()
    {
        Ingame_View_Controler.ingame_View_Controler.OrcSkill();
    }
}

[RequireComponent(typeof(GachaSlot))]
public class RollButton : Ingame_Command
{
    public override void Excute()
    {
        Ingame_View_Controler.ingame_View_Controler.Roll();
    }
}
public class BuyXp : Ingame_Command
{
    public override void Excute()
    {
        Ingame_View_Controler.ingame_View_Controler.BuyXp();
    }
}
public class CheckSynergy : Ingame_Command
{
    public override void Excute()
    {
        Ingame_View_Controler.ingame_View_Controler.SynergyPanelOn();
    }
}
public class Ready : Ingame_Command
{
    public override void Excute()
    {
        Ingame_View_Controler.ingame_View_Controler.Ready();
    }
}






public class Ingame_View_Button :MonoBehaviour
{
    public Ingame_Button Ingame_ButtonDo;
    Ingame_Command Command;
    private void Awake()
    {
        Command = Ingame_Command.MakeCommand(Ingame_ButtonDo);
        if (Command == null)
            Debug.LogError("Command is null" + gameObject.name);
    }
    public void Button()
    {
        Command.Excute();
    }

}
