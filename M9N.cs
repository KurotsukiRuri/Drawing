using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using System.Collections.Generic;
using System.Threading;

namespace BakaWater77.M9N;

public static class EventExtensions
{
    public static Vector3 SourcePosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
    }
}

[ScriptType(
    name: "M9N",
    territorys: new uint[] { 1320 },
    guid: "9af9ac60-1d6e-4247-a144-c6273417fea9",
    version: "0.0.0.2",
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

    #region 月之半相（左半场刀）
    [ScriptMethod(
        name: "月之半相左",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(48823)$" },
        userControl: true
    )]
    public void 月之半相左(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("先去 左 稍后对穿", duration: 4700, true);

        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "月之半相左";
        dp.Owner = sid;
        dp.Scale = new Vector2(40, 40);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Rotation = sourceObj.Rotation + MathF.PI / 2;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    #endregion

    #region 月之半相（右半场刀）
    [ScriptMethod(
        name: "月之半相右",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(48825)$" },
        userControl: true
    )]
    public void 月之半相右(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("先去 右 稍后对穿", duration: 4700, true);

        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "月之半相右";
        dp.Owner = sid;
        dp.Scale = new Vector2(40, 40);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Rotation = sourceObj.Rotation - MathF.PI / 2;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    #endregion

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
        dp.Scale = new Vector2(40, 40);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Rotation = sourceObj.Rotation + MathF.PI / 2;
        dp.Offset = @event.SourcePosition.WithX(4);
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    #endregion

    #region 月之半相（大右半场刀）
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
        dp.Scale = new Vector2(40, 40);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Rotation = sourceObj.Rotation - MathF.PI / 2;
        dp.Offset = @event.SourcePosition.WithX(-4);// new Vector3(-4, 0, 0)
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    #endregion

    #region 以太流失
    [ScriptMethod(
        name: "以太流失",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45896)$" }
    )]
    public void 以太流失(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["TargetId"], out var tid)) return;

        var playerObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == tid);
        if (playerObj == null) return;

        float rotation = playerObj.Rotation;

        // 前
        var dp1 = accessory.Data.GetDefaultDrawProperties();
        dp1.Name = "以太流失";
        dp1.Owner = tid;
        dp1.Scale = new Vector2(6, 40);
        dp1.ScaleMode = ScaleMode.ByTime;
        dp1.Rotation = rotation;
        dp1.Color = accessory.Data.DefaultDangerColor;
        dp1.DestoryAt = 12000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp1);

        // 后
        var dp3 = accessory.Data.GetDefaultDrawProperties();
        dp3.Name = "以太流失";
        dp3.Owner = tid;
        dp3.Scale = new Vector2(6, 40);
        dp3.ScaleMode = ScaleMode.ByTime;
        dp3.Rotation = rotation + MathF.PI;
        dp3.Color = accessory.Data.DefaultDangerColor;
        dp3.DestoryAt = 12000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp3);

        // 左
        var dp2 = accessory.Data.GetDefaultDrawProperties();
        dp2.Name = "以太流失";
        dp2.Owner = tid;
        dp2.Scale = new Vector2(6, 40);
        dp2.Rotation = rotation + MathF.PI / 2;
        dp2.Color = accessory.Data.DefaultDangerColor;
        dp2.DestoryAt = 12000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp2);

        // 右
        var dp4 = accessory.Data.GetDefaultDrawProperties();
        dp4.Name = "以太流失";
        dp4.Owner = tid;
        dp4.Scale = new Vector2(6, 40);
        dp4.Rotation = rotation - MathF.PI / 2;
        dp4.Color = accessory.Data.DefaultDangerColor;
        dp4.DestoryAt = 12000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp4);
    }
    #endregion

    #region 其他技能示例
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
        name: "共振波",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(45901)$" }
    )]
    public void 共振波(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "共振波";
        dp.Color = new Vector4(1f, 1f, 0f, 0.5f);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Position = @event.SourcePosition();
        dp.Scale = new Vector2(8);
        dp.DestoryAt = 3000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
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
    #endregion
}
