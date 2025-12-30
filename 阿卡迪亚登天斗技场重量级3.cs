using KodakkuAssist.Data;
using KodakkuAssist.Extensions;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.GameEvent.Struct;
using KodakkuAssist.Module.GameEvent.Types;
using KodakkuAssist.Module.GameOperate;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace BakaWater77.M11N;

[ScriptType(
       name: "M11N",
       territorys: new uint[] { 1324 },
       guid: "B0058EC8-4429-4BB7-B223-68DE8B115D04",
       version: "0.0.0.1",
       author: "Baka-Water77",
       note: null
    )]
public class 阿卡迪亚登天斗技场重量级3
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

    [ScriptMethod(
               name: "天顶的主宰",
               eventType: EventTypeEnum.StartCasting,
               eventCondition: new[] { "ActionId:regex:^(46006)$" },
               userControl: true
           )]
    public void 天顶的主宰(Event @event, ScriptAccessory accessory)
    {
        if (isText) 
            accessory.Method.TextInfo("AOE", duration: 4700);
    }

    [ScriptMethod(
              name: "铸兵之力：猛攻 ",
              eventType: EventTypeEnum.StartCasting,//月环|钢铁|十字
              eventCondition: new[] { "ActionId:regex:^(46008|46007|46009)$" },
              userControl: true
          )]
    public void 铸兵之力猛攻(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        if (@event.ActionId == 46008)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "月环";
            dp.Owner = sid;
            dp.Scale = new Vector2(5);
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 10000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        else if (@event.ActionId == 46007)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "钢铁";
            dp.Owner = sid;
            dp.Scale = new Vector2(8);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 10000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        
    }   
    [ScriptMethod(
              name: "铸兵之力：猛攻十字 ",
              eventType: EventTypeEnum.StartCasting,//十字
              eventCondition: new[] { "ActionId:regex:^(46009)$" },
              userControl: true
          )]
    public void 十字(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        float[] rotations = { @event.SourceRotation, @event.SourceRotation+ MathF.PI, @event.SourceRotation + MathF.PI / 2, @event.SourceRotation - MathF.PI / 2 };
            foreach (var rot in rotations)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "十字";
                dp.Position = @event.EffectPosition;
                dp.Scale = new Vector2(10, 60);
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.Rotation = rot;
                dp.DestoryAt = 10000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
            }
    }
    [ScriptMethod(
              name: "连线依次进行的钢铁月环十字",
              eventType: EventTypeEnum.SetObjPos,//月环|钢铁|十字
              eventCondition: new[] { "Id:regex:^(0197)$" },
              userControl: true
          )]
    public void 连线依次进行的钢铁月环十字(Event @event, ScriptAccessory accessory)
    {
        if (@event.SourceDataId() == 19185)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "月环";
            dp.Owner = @event.SourceId;
            dp.Scale = new Vector2(5);
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 17000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        else if (@event.SourceDataId() == 19184)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "钢铁";
            dp.Owner = @event.SourceId;
            dp.Scale = new Vector2(8);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 17000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        else if (@event.SourceDataId() ==19186)
        {
            float[] rotations = { @event.SourceRotation, @event.SourceRotation + MathF.PI, @event.SourceRotation + MathF.PI / 2, @event.SourceRotation - MathF.PI / 2 };
            foreach (var rot in rotations)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "十字";
                dp.Position = @event.SourcePosition;
                dp.Scale = new Vector2(10, 60);
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.Rotation = rot;
                dp.DestoryAt = 17000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
            }
        }
    }
    [ScriptMethod(
       name: "铸兵突袭钢铁月环十字判定",
       eventType: EventTypeEnum.StartCasting, // 月环|钢铁|十字
       eventCondition: new[] { "ActionId:regex:^(46031|46030|46032)$" },
       userControl: true
   )]
    public async void 铸兵突袭钢铁月环十字判定(Event @event, ScriptAccessory accessory)
    {
        
        await Task.Delay(500);

        if (@event.ActionId == 46031)
        {
            accessory.Method.RemoveDraw("月环.*");
        }
        else if (@event.ActionId == 46030)
        {
            accessory.Method.RemoveDraw("钢铁.*");
        }
        else if (@event.ActionId == 46032)
        {
            accessory.Method.RemoveDraw("十字.*");
        }
    }
    [ScriptMethod(
              name: "清除画图",
              eventType: EventTypeEnum.StartCasting,//
              eventCondition: new[] { "ActionId:regex:^(46028)$" },
              userControl: true
          )]
    public void 清除画图(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }
}

public static class EventExtensions
{
    public static uint SourceDataId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["SourceDataId"]);
    }
}
