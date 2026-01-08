using System;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent.Struct;
using KodakkuAssist.Module.Draw;
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
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Vfx;

namespace BakaWater77.M10N;
public static class EventExtensions
{
    public static Vector3 SourcePosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
    }
}

[ScriptType(
       name: "M10S BlueLinePoint",
       territorys: new uint[] { 1323 },
       guid: "C328CC14-B074-4196-BA4C-161020987536",
       version: "0.0.0.1",
       author: "Baka-Water77",
       note: null
    )]


public class M10S
{
    public static Vector3? BluePos;
    public static Vector3? RedPos;

    public bool isText { get; set; } = true;

    [ScriptMethod(
        name: "极限浪波 记录",
        eventType: EventTypeEnum.StartCasting,
        eventCondition: new[] { "ActionId:regex:^(4653[34])$" }, // 3是红 4是蓝
        userControl: true
    )]
    public void RecordBoss(Event @event, ScriptAccessory accessory)
    {
        var pos = @event.SourcePosition();
        var actionId = @event["ActionId"];

        if (actionId == "46534") // 蓝
            BluePos = pos;
        else if (actionId == "46533") // 红
            RedPos = pos;

        TryDraw(accessory);
    }

    private void TryDraw(ScriptAccessory accessory)
    {
       
        if (RedPos == null || BluePos == null)
            return;

        
        var fireQuad = GetQuad(RedPos.Value);
        var targetQuad = GetOppositeQuad(fireQuad);
        var targetPos = GetTargetPosition(targetQuad);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "终点";
        dp.Position = targetPos;
        dp.Scale = new Vector2(4);
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.DestoryAt = 6000;

        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

    
        BluePos = null;
        RedPos = null;
    }

    private enum Quad
    {
        Q1, // 左上
        Q2, // 右上
        Q3, // 右下
        Q4  // 左下
    }

    private Quad GetQuad(Vector3 pos)
    {
        bool left = pos.X < 100;
        bool up = pos.Z < 100;

        if (left && up) return Quad.Q1;
        if (!left && up) return Quad.Q2;
        if (!left && !up) return Quad.Q3;
        return Quad.Q4;
    }

    private Quad GetOppositeQuad(Quad q)
    {
        return q switch
        {
            Quad.Q1 => Quad.Q3,
            Quad.Q2 => Quad.Q4,
            Quad.Q3 => Quad.Q1,
            Quad.Q4 => Quad.Q2,
            _ => throw new Exception("非法象限")
        };
    }

    private Vector3 GetTargetPosition(Quad q)
    {
        return q switch
        {
            Quad.Q1 => new Vector3(87.47f, 0.0f, 86.89f),
            Quad.Q2 => new Vector3(112.64f, 0.0f, 86.95f),
            Quad.Q3 => new Vector3(112.49f, 0.0f, 113.11f),
            Quad.Q4 => new Vector3(87.32f, 0.0f, 113.20f),
            _ => Vector3.Zero
        };
    }
}

