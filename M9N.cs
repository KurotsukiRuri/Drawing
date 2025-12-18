using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using System.Reflection.Metadata;
using System.Net;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Generic;
using System.Timers;
using System.Reflection;
using Dalamud.Interface.Internal.UiDebug2.Browsing;
using System.Runtime.CompilerServices;

namespace Baka-Water77.M9N;
[ScriptType(name: "M9N", territorys: [1320], guid: "",
    version: "0.0.0.1", author: "Baka-Water77", note:noteStr)]

public class M9N
{
    const string noteStr =
    """
    写着玩w
    """;


}
[ScriptMethod(name: "月之半相", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:37714"])]
    public void DoubleEdgedSwords(Event @event, ScriptAccessory accessory)
    {
        if (DoubleEdgedSwordsCount == 0)
        {
            if (isDebug) accessory.Method.SendChat($"/e SourceRotation: {@event.SourceRotation()}");
            if (isText) accessory.Method.TextInfo($"去Boss{((@event.SourceRotation() == 3.14f) ? "后" : "前")}面，然后准备对穿", duration: 4500, true);
            accessory.TTS($"去Boss{((@event.SourceRotation() == 3.14f) ? "后" : "前")}面，然后准备对穿", isTTS, isDRTTS);
        }

        DoubleEdgedSwordsCount++;
        if (DoubleEdgedSwordsCount == 2)
        {
            DoubleEdgedSwordsCount = 0;
        }

        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "DoubleEdged Swords";
        dp.Color = new Vector4(255 / 255.0f, 0 / 255.0f, 0 / 255.0f, 0.5f);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(30);
        dp.Radian = float.Pi / 180 * 180;
        dp.DestoryAt = 4700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }