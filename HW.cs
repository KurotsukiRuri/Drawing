using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;

namespace HelloWorld;

[ScriptType(name: "HelloWorld", territorys: [], guid: "343daaf1-d3f8-4aa5-8c7f-0e08571dab33",
    version: "0.0.0.1", author: "Baka-Water77", note: null)]

public class SampleMethod
{
   [ScriptMethod(name: "SampleMethod", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:24286"])]
public void SampleMethod(Event @event, ScriptAccessory accessory)
{   
	accessory.Method.TextInfo("Hello World!", 5000);
}
    }

}
