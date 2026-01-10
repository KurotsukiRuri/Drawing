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

namespace BakaWater77.M10N
{
    public static class EventExtensions
    {
        public static Vector3 SourcePosition(this Event @event)
            => JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);

        public static void DrawGuidance(this ScriptAccessory sa,
    Vector3 target, int delay, int destroy, string name, float rotation = 0, float width = 1f, bool isSafe = true)
        {
            var dp = sa.Data.GetDefaultDrawProperties();
            dp.Name = name;
            dp.Position = target;
            dp.Scale = new Vector2(width);
            dp.Color = sa.Data.DefaultSafeColor;
            dp.DestoryAt = destroy;

            sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        public static DrawPropertiesEdit WaypointDp(this ScriptAccessory sa, Vector3 pos, uint duration, uint delay = 0, string name = "Waypoint")
        {
            var dp = sa.Data.GetDefaultDrawProperties();
            dp.Name = name;
            dp.Color = sa.Data.DefaultSafeColor;
            dp.Owner = sa.Data.Me;
            dp.TargetPosition = pos;
            dp.DestoryAt = duration;
            dp.Delay = delay;
            dp.Scale = new Vector2(2);
            dp.ScaleMode = ScaleMode.YByDistance;
            return dp;
        }
        public static uint TargetId(this Event e)
        {
            return uint.Parse(e["TargetId"]);
        }
    }

    [ScriptType(
        name: "M10S 蓝线指路",
        territorys: new uint[] { 1323 },
        guid: "C328CC14-B074-4196-BA4C-161020987536",
        version: "0.0.0.1",
        author: "Baka-Water77"
    )]
    public class M10S
    {



        public static Vector3? BluePos;
        public static Vector3? RedPos;
 


        private static Vector3? TargetPos;

        [ScriptMethod(
            name: "极限浪波 记录",
            eventType: EventTypeEnum.StartCasting,
            eventCondition: new[] { "ActionId:regex:^(4653[3-6])$" }, // 3/5 红, 4/6 蓝
            userControl: true
        )]
        public void RecordBoss(Event @event, ScriptAccessory accessory)
        {
            var pos = @event.SourcePosition();
            var actionId = @event["ActionId"];

            if (actionId == "46534" || actionId == "46536") // 蓝 Boss
                BluePos = pos;
            else if (actionId == "46533" || actionId == "46535") // 红 Boss
                RedPos = pos;


            TryDraw(accessory);
        }


        private void TryDraw(ScriptAccessory accessory)
        {
            if (BluePos == null || RedPos == null)
                return;


            var blueQuad = GetQuad(BluePos.Value);


            TargetPos = blueQuad switch
            {
                Quad.Q1 => GetTargetPosition(Quad.Q4),
                Quad.Q2 => GetTargetPosition(Quad.Q3),
                Quad.Q3 => GetTargetPosition(Quad.Q2),
                Quad.Q4 => GetTargetPosition(Quad.Q1),
                _ => Vector3.Zero
            };

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "蓝线终点";
            dp.Position = TargetPos.Value;
            dp.Scale = new Vector2(4);
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            BluePos = null;
            RedPos = null;


            var dpR = accessory.Data.GetDefaultDrawProperties();
            dpR.Name = "红线终点";
            dpR.Position = GetFireTarget(TargetPos.Value);
            dpR.Scale = new Vector2(4);
            dpR.Color = accessory.Data.DefaultDangerColor;
            dpR.DestoryAt = 6000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        private Vector3 GetFireTarget(Vector3 blueTarget)
        {
            float threshold = 0.5f;
            if (blueTarget == GetTargetPosition(Quad.Q1)) return new Vector3(105.3f, -0.0f, 117.5f);  // 1点 {105.52, -0.00, 82.60}
            if (blueTarget == GetTargetPosition(Quad.Q2)) return new Vector3(92.6f, -0.0f, 117.7f);   // 2点
            if (blueTarget == GetTargetPosition(Quad.Q3)) return new Vector3(94f, 0.0f, 81.5f); // 3点94f, 0.0f, 81.5f
            if (blueTarget == GetTargetPosition(Quad.Q4)) return new Vector3(109f, 0.0f, 86f);// 4点 {109.25, -0.00, 83.82}
            return Vector3.Zero;
        }


        [ScriptMethod(
            name: "蓝线点名指路",
            eventType: EventTypeEnum.TargetIcon,
            eventCondition: new[] { "Id:027B" }, // 蓝线点名
            userControl: true
        )]
        public void BlueLineGuidance(Event @event, ScriptAccessory sa)
        {
            if (TargetPos == null)
                return;
            
            
            if (@event.TargetId() != sa.Data.Me)
                return;

            uint targetPlayerId = @event.TargetId();

            var dp = sa.WaypointDp(
                TargetPos.Value, 
                6000,            
                0,
                "水线指路"
            );

            sa.Method.SendDraw(
                DrawModeEnum.Imgui,
                DrawTypeEnum.Displacement,
                dp
            );
        }

        [ScriptMethod(
   name: "火圈点名指路",
   eventType: EventTypeEnum.TargetIcon,
   eventCondition: new[] { "Id:027C" },
   userControl: true
)]
        
        public void FireLineGuidance(Event @event, ScriptAccessory sa)
        {
            if (TargetPos == null)
                return;


            Vector3 fireTarget = GetFireTarget(TargetPos.Value);
            if (fireTarget == Vector3.Zero) return;


            var dp = sa.Data.GetDefaultDrawProperties();
            dp.Name = "火圈终点";
            dp.Position = fireTarget;
            dp.Scale = new Vector2(4);
            dp.Color = sa.Data.DefaultDangerColor;
            dp.DestoryAt = 6000;
            sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);


            if (@event.TargetId() == sa.Data.Me)
            {
                var line = sa.WaypointDp(fireTarget, 6000, 0, "火圈指路");
                sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, line);
            }
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

        private Vector3 GetTargetPosition(Quad q)
        {
            return q switch
            {
                Quad.Q1 => new Vector3(89.91f, 0.0f, 84.38f), //{ 89.91, -0.00, 84.38 }
                Quad.Q2 => new Vector3(110.18f, 0.00f, 83.94f),//{110.18, 0.00, 83.94}
                Quad.Q3 => new Vector3(110.32f, 0.00f, 115.26f),//{110.32, 0.00, 115.26}
                Quad.Q4 => new Vector3(89.44f, 0.0f, 115.32f),//{89.44, -0.00, 115.32}
                _ => Vector3.Zero
            };

        }
       

    }


}

