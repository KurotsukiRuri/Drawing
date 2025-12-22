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
    #region 抽雾/急行文字提示
    [ScriptMethod(
        name: "超增压抽雾",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45677|45696)$" },
        userControl: true
    )]
    public void 超增压抽雾(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("吸引", duration: 4700);
    }

    [ScriptMethod(
        name: "超增压急行",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45670)$" },
        userControl: true
    )]
    public void 超增压急行(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("击退", duration: 4700);
    }
    #endregion

    #region 分散绘制
    [ScriptMethod(
        name: "超增压分散",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45670)$" },
        userControl: true
    )]
    public async void 超增压分散(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("分散", duration: 4700);

        await Task.Delay(5700); // 延迟 5.7 秒

        var ALLmember = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        foreach (var i in ALLmember)
        {
            var memberObj = accessory.Data.PartyList[i];
            if (memberObj == null) continue;

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
    #endregion

    #region 分摊绘制
    [ScriptMethod(
        name: "超增压分摊",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45664)$" },
        userControl: true
    )]
    public async void 超增压分摊(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["TargetId"], out uint TargetId))
            return;

        if (isText)
            accessory.Method.TextInfo("分摊", duration: 4700);

        await Task.Delay(5700); // 延迟 5.7 秒

        (int Index, string Name)[]? pointGroup = null;

        // 判断 TargetId 决定 4TN / 4DPS
        if (@event.TargetId == 0x400024A8) // 4TN
        {
            pointGroup = new[] { (0, "MT"), (1, "ST"), (2, "H1"), (3, "H2") };
        }
        else if (@event.TargetId == 0x40002AF7) // 4DPS
        {
            pointGroup = new[] { (4, "D1"), (5, "D2"), (6, "D3"), (7, "D4") };
        }

        if (pointGroup != null)
        {
            foreach (var (index, name) in pointGroup)
            {
                var memberObj = accessory.Data.PartyList[index];
                if (memberObj == null) continue;

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
    #endregion



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




