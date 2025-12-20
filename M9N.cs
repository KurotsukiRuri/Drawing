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
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Vfx;
using Lumina.Excel.Sheets;

namespace BakaWater77.M9N;




    [ScriptType(
    name: "M9N",
    territorys: new uint[] { 1320 },
    guid: "9af9ac60-1d6e-4247-a144-c6273417fea9",
    version: "0.0.0.9",
    author: "Baka-Water77",
    note: null
)]
public class M9N
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
        name: "月之半相左",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(48823)$" },
        userControl: true
    )]
    public void 月之半相左(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("先去 右 稍后对穿", duration: 4700, true);

        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "月之半相左";
        dp.Owner = sid;
        dp.Scale = new Vector2(60, 60);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Rotation = sourceObj.Rotation - MathF.PI / 2;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(
        name: "月之半相右",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(48825)$" },
        userControl: true
    )]
    public void 月之半相右(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("先去 左 稍后对穿", duration: 4700, true);

        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "月之半相右";
        dp.Owner = sid;
        dp.Scale = new Vector2(60, 60);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Rotation = sourceObj.Rotation + MathF.PI / 2;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    #region 月之半相（大左半场刀）
    [ScriptMethod(
        name: "大左半场刀",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(48824)$" },
        userControl: true
    )]
    public void 大左半场刀(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("先去 右 目标圈外 稍后对穿", duration: 4700, true);

        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "大左半场刀";
        dp.Owner = sid;
        dp.Scale = new Vector2(60, 60);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Rotation = sourceObj.Rotation - MathF.PI / 2;
        dp.Offset = new Vector3(4, 0, 0); // 修正
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(
        name: "大右半场刀",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(48826)$" },
        userControl: true
    )]
    public void 大右半场刀(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("先去 左 目标圈外 稍后对穿", duration: 4700, true);

        if (!ParseObjectId(@event["SourceId"], out var sid)) return;

        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "大右半场刀";
        dp.Owner = sid;
        dp.Scale = new Vector2(60, 60);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Rotation = sourceObj.Rotation + MathF.PI / 2;
        dp.Offset = new Vector3(-4, 0, 0); // 修正
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
        #endregion

        [ScriptMethod(
            name: "以太流失",//实际上是地上的那几根黄条TAT|||||||，以太流失之后的预警黄线
            eventType: EventTypeEnum.StartCasting,
            eventCondition: new[] { "ActionId:regex:^(45897)$" }//这个是实际判定出来的那个AOE
        )]
    public void 以太流失(Event @event, ScriptAccessory accessory)
    {
        float rotation = @event.SourceRotation();
        if (!ParseObjectId(@event["TargetId"], out uint targetId))
            return;

        Task.Run(() =>
        {
            var targetObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == targetId);
            if (targetObj == null) return;

            Vector3 targetPos = targetObj.Position;

            DrawCrossAOE(accessory, targetPos, rotation, 7000);
        });
    }

    private void DrawCrossAOE(ScriptAccessory accessory, Vector3 position, float rotation,int duration = 0)
            {   
                Vector2 scale = new Vector2(6, 40);
                var color = accessory.Data.DefaultDangerColor;

            float[] rotations = { rotation, rotation + MathF.PI, rotation + MathF.PI / 2, rotation - MathF.PI / 2 };

            foreach (var rot in rotations)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();

                dp.Name = "以太流失";
                dp.Position = position; 
                dp.Scale = scale;
                dp.Rotation = rot;
                dp.Color = color;
                dp.DestoryAt = duration;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
            }
        }


    //AOE
    [ScriptMethod(
        name: "施虐的尖啸",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45875)$" }
    )]
    public void 施虐的尖啸(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("AOE", duration: 4700, true);
    }


    [ScriptMethod(
        name: "魅亡之音",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45921)$" },
        userControl: true
    )]
    public void 魅亡之音(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("AOE", duration: 4700, true);
    }

    [ScriptMethod(
        name: "全场杀伤",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45886)$" },
        userControl: true
    )]
    public void 全场杀伤(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("AOE", duration: 4700, true);
    }

    [ScriptMethod(
        name: "致命的闭幕曲",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45888)$" },
        userControl: true
    )]
    public void 致命的闭幕曲(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("AOE", duration: 4700, true);
    }

    [ScriptMethod(
        name: "贪欲无厌",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45892)$" },
        userControl: true
    )]
    public void 贪欲无厌(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("AOE", duration: 4700, true);
    }
}







public static class EventExtensions
{
    public static Vector3 SourcePosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
    }
    public static float SourceRotation(this Event @event)
    {
        return float.Parse(@event["SourceRotation"]);
    }
    public static uint SourceDataId(this Event @event)
    {
        return uint.Parse(@event["SourceDataId"]);
    }
    public static Vector3 EffectPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
    }
}
