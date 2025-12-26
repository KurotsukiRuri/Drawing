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

namespace BakaWater77.M10N;

[ScriptType(
       name: "M10N",
       territorys: new uint[] { 1322 },
       guid: "DC98AE77-83FB-4B76-ACA7-45BBCF05DEFE",
       version: "0.0.0.1",
       author: "Baka-Water77",
       note: null
    )]
public class 阿卡迪亚登天斗技场重量级2
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

    private uint 水浪急转tid = 0;
    private uint 火浪急转tid = 0;






    [ScriptMethod(
               name: "斗志昂扬",
               eventType: EventTypeEnum.StartCasting,
               eventCondition: new[] { "ActionId:regex:^(46466|46467)$" },
               userControl: true
           )]

    public void 斗志昂扬(Event @event, ScriptAccessory accessory)
    {
        if (isText) 
            accessory.Method.TextInfo("AOE", duration: 4700);
    }

    [ScriptMethod(
           name: "浪尖转体",
           eventType: EventTypeEnum.StartCasting,
           eventCondition: new[] { "ActionId:regex:^(46488)$" },
           userControl: true
       )]
    public void 浪尖转体(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;


        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "浪尖转体";
        dp.Owner = sid;
        dp.Scale = new Vector2(60);
        dp.Rotation = sourceObj.Rotation;
        dp.Position = sourceObj.Position;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6500;
        dp.Radian = MathF.PI * 2f / 3f;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

    }

    [ScriptMethod(
           name: "破势乘浪",
           eventType: EventTypeEnum.StartCasting,
           eventCondition: new[] { "ActionId:regex:^(46483)$" },
           userControl: true
       )]
    public void 破势乘浪(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;




        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "破势乘浪";
        dp.Owner = sid;
        dp.Scale = new Vector2(15f, 50f);
        dp.Position = @event.EffectPosition;
        dp.Rotation = sourceObj.Rotation;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    [ScriptMethod(
           name: "混合爆炸",//极限以太
           eventType: EventTypeEnum.StartCasting,
           eventCondition: new[] { "ActionId:regex:^(46507)$" },
           userControl: true
       )]
    public void 混合爆炸(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "混合爆炸";
        dp.Owner = sid;
        dp.Scale = new Vector2(9);
        dp.Position = @event.EffectPosition;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 2700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }
}
