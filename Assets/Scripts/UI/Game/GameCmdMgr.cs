using RT;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum BullGameProcess
{
    BullGPStart = 0,
    BullGPSGBank = 1,
    BullGPSGBankResult = 2,
    BullGPSGDeal = 3,
    BullGPSGShow = 4,
    BullGPSGCheckout = 5,
    BullGPNNBank = 6,
    BullGPNNBankResult = 7,
    BullGPNNDeal = 8,
    BullGPNNShow = 9,
    BullGPNNCheckout = 10
}

public class GameCmdMgr : MonoBehaviour
{
    const double waitTime = 1;

    const double bull_sg_bankerTime = 2000;
    const double bull_sg_DealTime = 1000;
    const double bull_sg_ResultTime = 3000;

    const double bull_nn_bankerTime = 2000;
    const double bull_nn_DealTime = 1000;
    const double bull_nn_ResultTime = 3000;

    public BullGameProcess bullNextProcess;
    DateTime lastTime;

    List<GameCommond> CmdList;

    void Awake()
    {
        CmdList = new List<GameCommond>();
        bullNextProcess = BullGameProcess.BullGPStart;
    }

    void Start () {

    }
	
	void Update () {
		
	}

    public void AddCmd(SimpleJson.JsonObject obj, DateTime dt) {
        GameCommond cmd = new GameCommond();
        cmd.SeqNum = int.Parse(obj["seqNum"].ToString());
        cmd.Data = obj;

        cmd.RecvTime = dt;
        dt = dt.AddSeconds(waitTime);
        cmd.OpTime = dt;

        CmdList.Add(cmd);
        CmdList.Sort((x,y)=> {
            return x.SeqNum > y.SeqNum ? 1 : x.SeqNum < y.SeqNum ? -1 : 0;
        });
    }

    public GameCommond GetCmd(DateTime dt) {
        if (CmdList == null || CmdList.Count == 0)
        {
            return null;
        }
        else {
            GameCommond cmd = CmdList[0];
            if (cmd.OpTime.CompareTo(dt)<0)
            {
                if (CmdList.Count > 1)
                {
                    GameCommond next = CmdList[1];
                    if (next.OpTime.Subtract(cmd.OpTime).TotalMilliseconds<500)
                    {
                        next.OpTime=next.OpTime.AddMilliseconds(500);
                    }
                }
                CmdList.Remove(cmd);
                return cmd;
            }
            else
            {
                return null;
            }
        }
    }

    public void AddBullCmd(SimpleJson.JsonObject obj, DateTime dt)
    {
        GameCommond cmd = new GameCommond();
        cmd.SeqNum = int.Parse(obj["seqNum"].ToString());
        cmd.Data = obj;

        cmd.RecvTime = dt;
        dt = dt.AddSeconds(waitTime);
        cmd.OpTime = dt;

        if (cmd.OpTime.Subtract(lastTime).TotalMilliseconds < 500)
        {
            cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
        }
        Debug.Log("AddCmd --- "+ cmd.SeqNum+" ---op--->"+cmd.Data["op"]);
        
        if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "cow_checkout" && CmdList.Count>3) {
            Debug.Log("ClearCmd ---> " + cmd.SeqNum +"------CmdList Count---" + CmdList.Count);
            CmdList.Clear();
        }
        CmdList.Add(cmd);
        CmdList.Sort((x, y) => {
            return x.SeqNum > y.SeqNum ? 1 : x.SeqNum < y.SeqNum ? -1 : 0;
        });
    }
    public GameCommond GetBullCmd(DateTime dt)
    {
        if (CmdList == null || CmdList.Count == 0)
        {
            return null;
        }
        else
        {
            GameCommond cmd = CmdList[0];
            if (cmd.OpTime.CompareTo(dt) < 0)
            {
                cmd = GetBullCmd(cmd);
                if (cmd == null)
                {
                    return null;
                }
                if (CmdList.Count > 1)
                {
                    GameCommond next = CmdList[1];
                    if (next.OpTime.Subtract(cmd.OpTime).TotalMilliseconds < 500)
                    {
                        next.OpTime = next.OpTime.AddMilliseconds(500);
                    }
                }
                lastTime = cmd.OpTime;
                CmdList.Remove(cmd);
                return cmd;
            }
            else
            {
                return null;
            }
        }
    }

    public GameCommond GetBullCmd(GameCommond cmd) {
        if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "sg_start")
        {
            bullNextProcess = BullGameProcess.BullGPStart;
            return cmd;
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "nextRound" && cmd.Data["round"].ToString() == "step_callBanker_SG")
        {
            if (bullNextProcess == BullGameProcess.BullGPStart)
            {
                bullNextProcess = BullGameProcess.BullGPSGBank;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "sg_callBanker")
        {
            if (bullNextProcess == BullGameProcess.BullGPSGBank)
            {
                bullNextProcess = BullGameProcess.BullGPSGBankResult;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "sg_showCards")
        {
            if (bullNextProcess == BullGameProcess.BullGPSGBankResult)
            {
                if (cmd.OpTime.Subtract(lastTime).TotalMilliseconds < bull_sg_bankerTime)
                {
                    cmd.OpTime = lastTime.AddMilliseconds(bull_sg_bankerTime);
                    return null;
                }
                bullNextProcess = BullGameProcess.BullGPSGDeal;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "sg_self_showCards")
        {
            if (bullNextProcess == BullGameProcess.BullGPSGDeal)
            {
                if (cmd.OpTime.Subtract(lastTime).TotalMilliseconds < bull_sg_DealTime)
                {
                    cmd.OpTime = lastTime.AddMilliseconds(bull_sg_DealTime);
                    return null;
                }
                bullNextProcess = BullGameProcess.BullGPSGShow;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "nextRound" && cmd.Data["round"].ToString() == "step_showCard_SG")
        {
            if (bullNextProcess == BullGameProcess.BullGPSGDeal || bullNextProcess == BullGameProcess.BullGPSGShow)
            {
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "sg_checkout")
        {
            if (bullNextProcess == BullGameProcess.BullGPSGShow)
            {
                bullNextProcess = BullGameProcess.BullGPSGCheckout;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "nextRound" && cmd.Data["round"].ToString() == "step_callBanker_COW")
        {
            if (bullNextProcess == BullGameProcess.BullGPSGCheckout)
            {
                if (cmd.OpTime.Subtract(lastTime).TotalMilliseconds < bull_sg_ResultTime)
                {
                    cmd.OpTime = lastTime.AddMilliseconds(bull_sg_ResultTime);
                    return null;
                }
                bullNextProcess = BullGameProcess.BullGPNNBank;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "cow_callBanker")
        {
            if (bullNextProcess == BullGameProcess.BullGPNNBank)
            {
                bullNextProcess = BullGameProcess.BullGPNNBankResult;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "cow_self_showCards")
        {
            if (bullNextProcess == BullGameProcess.BullGPNNBankResult)
            {
                if (cmd.OpTime.Subtract(lastTime).TotalMilliseconds < bull_nn_bankerTime)
                {
                    cmd.OpTime = lastTime.AddMilliseconds(bull_nn_bankerTime);
                    return null;
                }
                bullNextProcess = BullGameProcess.BullGPNNDeal;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "nextRound" && cmd.Data["round"].ToString() == "step_putCard_COW")
        {
            if (bullNextProcess == BullGameProcess.BullGPNNDeal)
            {
                if (cmd.OpTime.Subtract(lastTime).TotalMilliseconds < bull_nn_DealTime)
                {
                    cmd.OpTime = lastTime.AddMilliseconds(bull_nn_DealTime);
                    return null;
                }
                bullNextProcess = BullGameProcess.BullGPNNShow;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else if (cmd.Data.ContainsKey("op") && cmd.Data["op"].ToString() == "cow_checkout")
        {
            if (bullNextProcess == BullGameProcess.BullGPNNShow)
            {
                bullNextProcess = BullGameProcess.BullGPNNCheckout;
                return cmd;
            }
            else
            {
                cmd.OpTime = cmd.OpTime.AddMilliseconds(500);
            }
        }
        else {
            return cmd;
        }
        return null;
    }

    public void ClearCmd() {
        CmdList.Clear();
    }
}
