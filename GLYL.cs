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
    name: "以太炮",
    eventType: EventTypeEnum.TargetIcon,
    eventCondition: ["Id:027E"],
    userControl: true
)]
    public void 以太炮(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("分散", duration: 4700, true);

        if (!ParseObjectId(@event["TargetId"], out var pid))
            return;

        var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "以太炮";
            dp.Owner = pid;
            dp.Scale = new Vector2(6);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 4000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }

    [ScriptMethod(
    name: "以太冲击波",
    eventType: EventTypeEnum.TargetIcon,
    eventCondition: ["Id:027D"],
    userControl: true
)]
    public void 以太冲击波(Event @event, ScriptAccessory accessory)
    {
        if (isText)
            accessory.Method.TextInfo("分摊", duration: 4700, true);

        if (!ParseObjectId(@event["TargetId"], out var pid))
            return;


        var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "以太冲击波";
                dp.Owner = pid;
                dp.Scale = new Vector2(6);
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.DestoryAt = 4000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
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
    } }
