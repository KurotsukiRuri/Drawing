using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using System.Collections.Generic;

namespace BakaWater77.M9N
{
    [ScriptType(
        name: "M9N",
        territorys: new[] { 1320 },
        guid: "9af9ac60-1d6e-4247-a144-c6273417fea9",
        version: "0.0.0.1",
        author: "Baka-Water77",
        note: null
    )]
    public class M9N
    {
        public bool isText { get; set; } = true;

        [ScriptMethod(
            name: "魅亡之音",
            eventType: EventTypeEnum.StartCasting,
            eventCondition: new[] { "ActionId:regex:^(45921)$" }
        )]
        public void 魅亡之音(Event @event, ScriptAccessory accessory)
        {
            if (isText)
                accessory.Method.TextInfo("AOE", 4700, true);
        }

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

            // 前后
            var dp1 = accessory.Data.GetDefaultDrawProperties();
            dp1.Name = "以太流失_前后";
            dp1.Owner = tid;
            dp1.Scale = new Vector2(6, 40);
            dp1.Rotation = rotation;
            dp1.Color = accessory.Data.DefaultDangerColor;
            dp1.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp1);

            // 左右
            var dp2 = accessory.Data.GetDefaultDrawProperties();
            dp2.Name = "以太流失_左右";
            dp2.Owner = tid;
            dp2.Scale = new Vector2(6, 40);
            dp2.Rotation = rotation + MathF.PI / 2;
            dp2.Color = accessory.Data.DefaultDangerColor;
            dp2.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp2);
        }

        [ScriptMethod(
            name: "施虐的尖啸",
            eventType: EventTypeEnum.StartCasting,
            eventCondition: new[] { "ActionId:regex:^(45875)$" }
        )]
        public void 施虐的尖啸(Event @event, ScriptAccessory accessory)
        {
            if (isText)
                accessory.Method.TextInfo("AOE", 4700, true);
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
            dp.Position = @event.SourcePosition();
            dp.Scale = new Vector2(8);
            dp.Color = new Vector4(1f, 1f, 0f, 0.5f);
            dp.DestoryAt = 7700;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

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
    }

    public static class EventExtensions
    {
        public static Vector3 SourcePosition(this Event @event)
        {
            return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
        }
    }
}
