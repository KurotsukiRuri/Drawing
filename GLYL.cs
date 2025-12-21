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
   version: "0.0.0.1",
   author: "Baka-Water77",
   note: null
)]
public class 极格莱杨拉
{
    public bool isText { get; set; } = true;

    
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




