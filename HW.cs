using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace HelloWorld;

[ScriptType(name: "HelloWorld", territorys: [], guid: "AF4E8C43-08BA-4C98-8DEF-70F023AF234D",
    version: "0.0.0.1", author: "Baka-Water77", note: null)]

public class HelloWorld
{
    [ScriptMethod(name: "HelloWorld", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:24286"])]
    public void HelloWorld(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("Hello World!", 5000);
    }

}
