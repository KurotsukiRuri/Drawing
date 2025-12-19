using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using KodakkuAssist.Script;
using KodakkuAssist.Module.Draw;

namespace BakaWater77.M9N
{
    [ScriptType(name: "M9N", territorys: new[] { 1320 }, guid: "9af9ac60-1d6e-4247-a144-c6273417fea9",
        version: "0.0.0.1", author: "Baka-Water77", note: null)]
    public class M9N
    {
        private const string Name = "M9N";
        private const string Version = "0.0.0.1";
        private const string DebugVersion = "a";
        private const bool Debugging = false;

        private static readonly Vector3 Center = new Vector3(100, 0, 100);
        private AutoResetEvent _nightFallAutoEvent = new AutoResetEvent(false);

        public bool isText { get; set; } = true;

        [ScriptMethod(name: "魅亡之音", eventType: EventTypeEnum.StartCasting, eventCondition: new[] { "ActionId:regex:^([45921])$" })]
        public void 魅亡之音(Event @event, ScriptAccessory accessory)
        {
            if (isText) accessory.Method.TextInfo("AOE", duration: 4700, true);
        }

        [ScriptMethod(name: "以太流失", eventType: EventTypeEnum.StartCasting, eventCondition: new[] { "ActionId:regex:^([45896])$" })]
        public void 以太流失(Event @event, ScriptAccessory accessory)
        {
            if (!M9NExtensions.ParseObjectId(@event["TargetId"], out var tid)) return;
            var playerObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == tid);
            if (playerObj == null) return;

            float rotation = playerObj.Rotation;

            var dp1 = accessory.Data.GetDefaultDrawProperties(); // 前后
            dp1.Name = "以太流失";
            dp1.Scale = new Vector2(6, 40);
            dp1.ScaleMode = ScaleMode.ByTime;
            dp1.Owner = tid;
            dp1.Color = accessory.Data.DefaultDangerColor;
            dp1.DestoryAt = 5000;
            dp1.Rotation = rotation;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp1);

            var dp2 = accessory.Data.GetDefaultDrawProperties(); // 左右
            dp2.Name = "以太流失";
            dp2.Owner = tid;
            dp2.Scale = new Vector2(6, 40);
            dp2.Rotation = rotation + MathF.PI / 2;
            dp2.Color = accessory.Data.DefaultDangerColor;
            dp2.DestoryAt = 5000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp2);
        }

        [ScriptMethod(name: "施虐的尖啸", eventType: EventTypeEnum.StartCasting, eventCondition: new[] { "ActionId:regex:^([45875])$" })]
        public void 施虐的尖啸(Event @event, ScriptAccessory accessory)
        {
            if (isText) accessory.Method.TextInfo("AOE", duration: 4700, true);
        }

        [ScriptMethod(name: "共振波", eventType: EventTypeEnum.StartCasting, eventCondition: new[] { "ActionId:regex:^([45901])$" })]
        public void 共振波(Event @event, ScriptAccessory accessory)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "共振波";
            dp.Color = new Vector4(1f, 1f, 0f, 0.5f);
            dp.ScaleMode = ScaleMode.ByTime;
            dp.Position = @event.SourcePosition();
            dp.Scale = new Vector2(8);
            dp.DestoryAt = 7700;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }

    // 顶级静态类，存放扩展方法
    public static class M9NExtensions
    {
        public static bool ParseObjectId(this string? idStr, out uint id)
        {
            id = 0;
            if (string.IsNullOrEmpty(idStr)) return false;
            try
            {
                var idStr2 = idStr.Replace("0x", "");
                id = uint.Parse(idStr2, System.Globalization.NumberStyles.HexNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Vector3 SourcePosition(this Event @event)
        {
            if (@event == null || string.IsNullOrEmpty(@event["SourcePosition"]))
                return Vector3.Zero;

            return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]) ?? Vector3.Zero;
        }
    }
}
