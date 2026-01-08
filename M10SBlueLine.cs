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

[ScriptType(
       name: "M10S BlueLinePoint",
       territorys: new uint[] { 1323 },
       guid: "C328CC14-B074-4196-BA4C-161020987536",
       version: "0.0.0.1",
       author: "Baka-Water77",
       note: null
    )]

public static class EventExtensions
{
    public static Vector3 SourcePosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
    }
}
public class M10S
{
    public bool isText { get; set; } = true;

    [ScriptMethod(
               name: "极限浪波",
               eventType: EventTypeEnum.StartCasting,
               eventCondition: new[] { "ActionId:regex:^(46534)$" },//；34蓝   33红
               userControl: true
           )]
    public void Line(Event @event, ScriptAccessory accessory)
    {
        if (@event.SourcePosition().X < 100 && @event.SourcePosition().Z < 100)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = "蓝线拉线终点";
            dp.Position = new Vector3(92, 0, 116);//{92.01, 0.00, 116.81}
            dp.Scale = new Vector2(4);
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 4600;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        else if (@event.SourcePosition().X < 100 && @event.SourcePosition().Z > 100)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();//
            dp.Name = "蓝线拉线终点";
            dp.Position = new Vector3(118, 0, 97);//{118.94, 0.00, 96.85}
            dp.Scale = new Vector2(4);
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 4600;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        else if (@event.SourcePosition().X > 100 && @event.SourcePosition().Z < 100)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();//
            dp.Name = "蓝线拉线终点";
            dp.Position = new Vector3(94, 0, 118);//{94.62, -0.00, 118.33}
            dp.Scale = new Vector2(4);
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 4600;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }
}