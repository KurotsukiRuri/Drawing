using System;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent.Struct;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Data;
using KodakkuAssist.Module.Draw.Manager;
using KodakkuAssist.Module.GameOperate;
using KodakkuAssist.Module.GameEvent.Types;
using KodakkuAssist.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel;
using Dalamud.Utility.Numerics;
using Dalamud.Plugin.Services;


namespace BakaWater77.极格莱杨拉;



[ScriptType(
   name: "极格莱杨拉",
   territorys: new uint[] { 1308 },
   guid: "125b0e7e-1fcc-412f-9d70-49d0ba2a6e3f",
   version: "0.0.0.2",
   author: "Baka-Water77",
   note: null
)]

public class 极格莱杨拉
{
    public bool isText { get; set; } = true;
     private static bool ParseObjectId(string? idStr, out uint id)
    {
        id = 0;
        if (string.IsNullOrEmpty(idStr)) return false;

        try
        {
            id = uint.Parse(idStr.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
            return true;
        }
        catch
        {
            return false;
        }
        

    }

    private Dictionary<uint, Action<ScriptAccessory>> _drawActions = new();

    public interface IActionEffectJudge
    {
        void OnKnockback(Event @event, ScriptAccessory accessory);
        void OnPullIn(Event @event, ScriptAccessory accessory);

        bool IsKnockback(Event @event, ScriptAccessory accessory);
        bool IsPullIn(Event @event, ScriptAccessory accessory);
    }
    public class ActionEffectJudgeImpl : IActionEffectJudge
    {
        private readonly HashSet<uint> _knockbackSkills = new(); // 存储触发过击退的技能ID
        private readonly HashSet<uint> _pullInSkills = new();    // 存储触发过吸引的技能ID

        public void OnKnockback(Event @event, ScriptAccessory accessory)
        {
            _knockbackSkills.Add(@event.ActionId);
        }

        public void OnPullIn(Event @event, ScriptAccessory accessory)
        {
            _pullInSkills.Add(@event.ActionId);
        }

        public bool IsKnockback(Event @event, ScriptAccessory accessory)
        {
            return _knockbackSkills.Contains(@event.ActionId);
        }

        public bool IsPullIn(Event @event, ScriptAccessory accessory)
        {
            return _pullInSkills.Contains(@event.ActionId);
        }
    }

    private readonly IActionEffectJudge _aeJudge = new ActionEffectJudgeImpl();

    [ScriptMethod(
    name: "超增压抽雾|急行判定",
    eventType: EventTypeEnum.ActionEffect,
    eventCondition: new[] { "ActionId:regex:^(45677|45696|45670)$" },
    userControl: true
)]
    public void 超增压抽雾判定(Event @event, ScriptAccessory accessory)
    {
        if (@event.ActionId == 45670 || @event.ActionId == 45677)
            _aeJudge.OnKnockback(@event, accessory);
        else if (@event.ActionId == 45696)
            _aeJudge.OnPullIn(@event, accessory);

    }


    [ScriptMethod(
      name: "超增压",//分散
      eventType: EventTypeEnum.StartCasting,
      eventCondition: new[] { "ActionId:regex:^(45670)$" },
      userControl: true
  )]
    public async void 超增压分散(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("分散", duration: 4700);
        await Task.Delay(50);
        if (_aeJudge.IsKnockback(@event,accessory))
        {



            var ALLmember = new[]
        {
        (Index: 0, Name: "MT"),
        (Index: 1, Name: "ST"),
        (Index: 2, Name: "H1"),
        (Index: 3, Name: "H2"),
        (Index: 4, Name: "D1"),
        (Index: 5, Name: "D2"),
        (Index: 6, Name: "D3"),
        (Index: 7, Name: "D4")
    };


            foreach (var (index, name) in ALLmember)
            {
                var memberObj = accessory.Data.PartyList[index];
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "超增压分散";
                dp.Owner = memberObj;
                dp.Scale = new Vector2(5);
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 6000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                accessory.Method.TextInfo("分散", duration: 4700);
            }
        }
    }
    [ScriptMethod(
    name: "超增压",//分摊
    eventType: EventTypeEnum.StartCasting,
    eventCondition: new[] { "ActionId:regex:^(45664)$" },
    userControl: true
)]
    public async void 超增压分摊4TN(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("分摊", duration: 4700);
        await Task.Delay(50);
        if (!_aeJudge.IsKnockback(@event,accessory)) return;

            if (!ParseObjectId(@event["TargetId"], out uint TargetId))
                return;
            if (@event.TargetId == 0x400024A8)
            {
                var fourTN = new[]
            {
        (Index: 0, Name: "MT"),
        (Index: 1, Name: "ST"),
        (Index: 2, Name: "H1"),
        (Index: 3, Name: "H2")

    };

                foreach (var (index, name) in fourTN)
                {
                    var memberObj = accessory.Data.PartyList[index];
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "超增压分摊";
                    dp.Owner = memberObj;
                    dp.Scale = new Vector2(5);
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 6000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                    accessory.Method.TextInfo("分摊", duration: 4700);
                }
            }
        }


    

    [ScriptMethod(
    name: "超增压",//分摊
    eventType: EventTypeEnum.StartCasting,
    eventCondition: new[] { "ActionId:regex:^(45664)$" },
    userControl: true
)]
    public async void 超增压分摊4DPS(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("分摊", duration: 4700);
        await Task.Delay(50);
        if (!_aeJudge.IsKnockback(@event,accessory)) return;




        if (!ParseObjectId(@event["TargetId"], out uint TargetId))
                return;
            if (@event.TargetId == 0x40002AF7)
            {
                var fourDPS = new[]
            {
        (Index: 4, Name: "D1"),
        (Index: 5, Name: "D2"),
        (Index: 6, Name: "D3"),
        (Index: 7, Name: "D4")

    };

                foreach (var (index, name) in fourDPS)
                {
                    var memberObj = accessory.Data.PartyList[index];
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = "超增压分摊";
                    dp.Owner = memberObj;
                    dp.Scale = new Vector2(5);
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.DestoryAt = 6000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                    accessory.Method.TextInfo("分摊", duration: 4700);
                }
            }
        }
    



    [ScriptMethod(
    name: "以太炮",
    eventType: EventTypeEnum.TargetIcon,
    eventCondition: ["Id:027E"],
    userControl: true
)]
    public void 以太炮(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("分散", duration: 4700);

       
        for (int i = 0; i < accessory.Data.PartyList.Count; i++)
        {
            if (i == 0 || i == 1) continue; 

            var p = accessory.Data.PartyList[i];


            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "以太炮";
            dp.Owner = p;
            dp.Scale = new Vector2(6);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }


    [ScriptMethod(
    name: "以太冲击波",
    eventType: EventTypeEnum.TargetIcon,
    eventCondition: ["Id:027D"],
    userControl: true
)]
    public void DrawH1H2Circle(Event ev, ScriptAccessory sa)
    {
        if (sa.Data.PartyList.Count < 2) return; 

        if (isText)
            sa.Method.TextInfo("分摊", duration: 4700);

      
        var H1H2 = new[]
        {
        (Index: 2, Name: "H1"),
        (Index: 3, Name: "H2")
    };

        foreach (var (index, name) in H1H2)
        {
            var memberObj = sa.Data.PartyList[index]; 
            if (memberObj == 0) continue;
            var dp = sa.Data.GetDefaultDrawProperties();
            dp.Name = "以太冲击波";
            dp.Owner = memberObj;
            dp.Scale = new Vector2(6);
            dp.Color = sa.Data.DefaultSafeColor;
            dp.DestoryAt = 6000;
            sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }
}




