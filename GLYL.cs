using Dalamud.Game;
    using Dalamud.Plugin.Services;
    using Dalamud.Utility.Numerics;
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
    using System.Threading;
    using System.Threading.Tasks;


    namespace BakaWater77.极格莱杨拉;



    [ScriptType(
       name: "极格莱杨拉",
       territorys: new uint[] { 1308 },
       guid: "125b0e7e-1fcc-412f-9d70-49d0ba2a6e3f",
       version: "0.0.0.2",
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
        private Dictionary<uint, Event> startCastingCache = new();

        private uint lastActionIdForScatter = 0;  // 分散
        private uint lastTargetIdForShare = 0;    // 分摊


        [ScriptMethod(
            name: "超增压",
            eventType: EventTypeEnum.StartCasting,
            eventCondition: new[] { "ActionId:regex:^(45663|45664|45670|45677|45696)$" },
            userControl: true
        )]
        public async void 超增压(Event @event, ScriptAccessory accessory)
        {
        if (!int.TryParse(@event["ActionId"], out var actionId))
            return;

        // 分散 45663
        if (actionId == 45663)
            {
                if (isText)
                    accessory.Method.TextInfo("稍后分散", duration: 4700);
            lastActionIdForScatter = (uint)actionId;


            return;
            }

            //分摊 45664
            if (actionId == 45664)
            {
                if (isText)
                    accessory.Method.TextInfo("稍后分摊", duration: 4700);

                if (ParseObjectId(@event["TargetId"], out uint TargetId))
                    lastTargetIdForShare = TargetId;
           
            return; 
            }

            // 击退/吸引 45670/45677/45696 
            if (actionId == 45670 || actionId == 45677 || actionId == 45696)
            {
                await Task.Delay(8500);


            if (lastTargetIdForShare == 0x400024A8) // 4TN
            {
                DrawMembers(accessory, new int[] { 0, 1, 2, 3 }, accessory.Data.DefaultSafeColor, "超增压");
            }
            else if (lastTargetIdForShare == 0x40002AF7) // 4DPS
            {
                DrawMembers(accessory, new int[] { 4, 5, 6, 7 }, accessory.Data.DefaultSafeColor, "超增压");
            }
            if(lastActionIdForScatter == 45563);//分散
                {
                    DrawMembers(accessory, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, accessory.Data.DefaultDangerColor, "超增压");
                }
            }
        }


        private void DrawMembers(ScriptAccessory accessory, int[] indices, Vector4 color, string name)
        {
            foreach (var index in indices)
            {
                var memberObj = accessory.Data.PartyList.ElementAtOrDefault(index);
                if (memberObj == null) continue;

                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = name;
                dp.Owner = memberObj;
                dp.Scale = new Vector2(5);
                dp.Color = color;
                dp.DestoryAt = 4000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
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
                accessory.Method.TextInfo("分散", duration: 4700);


            for (int i = 0; i < accessory.Data.PartyList.Count; i++)
            {
                if (i == 0 || i == 1) continue;

                var p = accessory.Data.PartyList[i];


                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = "以太炮";
                dp.Owner = p;
                dp.Scale = new Vector2(6);
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.DestoryAt = 6000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
        }


        [ScriptMethod(
        name: "以太冲击波",
        eventType: EventTypeEnum.TargetIcon,
        eventCondition: ["Id:027D"],
        userControl: true
    )]
        public void DrawH1H2Circle(Event ev, ScriptAccessory sa)
        {
            if (sa.Data.PartyList.Count < 2) return;

            if (isText)
                sa.Method.TextInfo("分摊", duration: 4700);


            var H1H2 = new[]
            {
            (Index: 2, Name: "H1"),
            (Index: 3, Name: "H2")
        };

            foreach (var (index, name) in H1H2)
            {
                var memberObj = sa.Data.PartyList[index];
                if (memberObj == 0) continue;
                var dp = sa.Data.GetDefaultDrawProperties();
                dp.Name = "以太冲击波";
                dp.Owner = memberObj;
                dp.Scale = new Vector2(6);
                dp.Color = sa.Data.DefaultSafeColor;
                dp.DestoryAt = 6000;
                sa.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
        }
    }




