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
       name: "M10N",
       territorys: new uint[] { 1322 },
       guid: "DC98AE77-83FB-4B76-ACA7-45BBCF05DEFE",
       version: "0.0.0.2",
       author: "Baka-Water77",
       note: null
    )]
public class M10N
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

    const string NoteStr =
            $"""
        {Version}
        说明 蛇
        """;

    const string UpdateInfo =
        $"""
         {Version}
         更新 蛇
         """;

    private const string Name = "M10N [阿卡狄亚 重量级2]";
    private const string Version = "0.0.0.0";
    private const string DebugVersion = "a";

    private const bool Debugging = true;
    private static readonly Vector3 Center = new Vector3(100, 0, 100);

    private static List<uint> _elementSnake = [];
    private static int _elementSnakeTargetCount = 0;
    private ManualResetEvent _elementSnakeManualEvent = new ManualResetEvent(false);


    //Usami老师的超绝画法desu
    public void Init(ScriptAccessory sa)
    {
        RefreshParams();
        sa.Method.RemoveDraw(".*");
        sa.Method.ClearFrameworkUpdateAction(this);
    }

    private void RefreshParams()
    {
        _elementSnake = [];
        _elementSnakeTargetCount = 0;
        _elementSnakeManualEvent = new ManualResetEvent(false);
    }

    [ScriptMethod(name: "———————— 《测试项》 ————————", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld:asdf"],
        userControl: true)]
    public void 测试项分割线(Event ev, ScriptAccessory sa)
    {
    }

    [ScriptMethod(name: "测试蛇范围", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld:asdf"],
        userControl: Debugging)]
    public void 测试蛇范围(Event ev, ScriptAccessory sa)
    {
        sa.DrawFan(new Vector3(87, 0, 113), sa.Data.Me, 0, 2000, $"蛇1211", 30f.DegToRad(), 0f, 40, 0);
    }

    [ScriptMethod(name: "测试TargetIcon", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:regex:^(029[56]|027[BC])$"],
        userControl: Debugging)]
    public void 测试TargetIcon(Event ev, ScriptAccessory sa)
    {
        var a = uint.Parse(ev["Id"], System.Globalization.NumberStyles.HexNumber);
        // sa.Log.Debug($"{a}");
    }

    [ScriptMethod(name: "———————— 《蛇》 ————————", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld:asdf"],
        userControl: true)]
    public void 蛇分割线(Event ev, ScriptAccessory sa)
    {
    }

    [ScriptMethod(name: "定位蛇", eventType: EventTypeEnum.EnvControl, eventCondition: ["Flag:regex:^(2|512)$", "Index:regex:^(1[456789]|2[012])$"],
        userControl: Debugging)]
    public void 定位蛇(Event ev, ScriptAccessory sa)
    {
        const uint WATER_ELEMENT = 2;
        const uint FIRE_ELEMENT = 512;

        lock (_elementSnake)
        {
            var elementTypeVal = JsonConvert.DeserializeObject<uint>(ev["Flag"]) == FIRE_ELEMENT ? 10 : 0;
            var region = JsonConvert.DeserializeObject<uint>(ev["Index"]) - 14;
            var val = (uint)(region + elementTypeVal);
            _elementSnake.Add(val);
            // sa.Log.Debug($"elementTypeVal: {elementTypeVal}, region: {region}, val: {val}");

            if (_elementSnake.Count != 2) return;
            _elementSnakeManualEvent.Set();
        }
    }

    [ScriptMethod(name: "转转蛇范围绘图", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:regex:^(029[56]|027[BC])$"],
        userControl: true)]
    public void 获取点名(Event ev, ScriptAccessory sa)
    {
        // 等待该行为被触发
        _elementSnakeManualEvent.WaitOne();

        lock (_elementSnake)
        {
            var isWaterIcon = uint.Parse(ev["Id"], System.Globalization.NumberStyles.HexNumber) is 0x0295 or 0x027B;
            var targetIndex = sa.Data.PartyList.IndexOf((uint)ev.TargetId);

            foreach (var snakeVal in _elementSnake)
            {
                // 火蛇+10
                if ((isWaterIcon && snakeVal >= 10) || (!isWaterIcon && snakeVal < 10)) continue;
                var region = snakeVal % 10;
                var regionCenter = new Vector3(87 + 13 * (region % 3), 0, 87 + 13 * (int)(region / 3));
                sa.DrawFan(regionCenter, ev.TargetId, 0, 10000, $"蛇点名{targetIndex}", 30f.DegToRad(), 0f, 60, 0);
            }
            _elementSnakeTargetCount++;
            // sa.Log.Debug($"点名数 {_elementSnakeTargetCount}，点 {targetIndex}");
            if (_elementSnakeTargetCount < 4) return;
            _elementSnakeTargetCount = 0;
            _elementSnake.Clear();
            _elementSnakeManualEvent.Reset();
        }
    }

    [ScriptMethod(name: "蛇绘图移除", eventType: EventTypeEnum.ActionEffect, eventCondition: ["TargetIndex:1", "ActionId:regex:^(4650[56])$"],
        userControl: Debugging)]
    public void 蛇绘图移除(Event ev, ScriptAccessory sa)
    {
        var targetIndex = sa.Data.PartyList.IndexOf((uint)ev.TargetId);
        sa.Method.RemoveDraw($"蛇点名{targetIndex}");
    }



    [ScriptMethod(
               name: "斗志昂扬",
               eventType: EventTypeEnum.StartCasting,
               eventCondition: new[] { "ActionId:regex:^(46466|46467)$" },
               userControl: true
           )]

    public void 斗志昂扬(Event @event, ScriptAccessory accessory)
    {
        if (isText) 
            accessory.Method.TextInfo("AOE", duration: 4700);
    }

    [ScriptMethod(
           name: "浪尖转体",
           eventType: EventTypeEnum.StartCasting,
           eventCondition: new[] { "ActionId:regex:^(46488)$" },
           userControl: true
       )]
    public void 浪尖转体(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;


        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "浪尖转体";
        dp.Owner = sid;
        dp.Scale = new Vector2(60);
        dp.Rotation = sourceObj.Rotation;
        dp.Position = sourceObj.Position;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6500;
        dp.Radian = MathF.PI * 2f / 3f;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

    }

    [ScriptMethod(
           name: "破势乘浪",
           eventType: EventTypeEnum.StartCasting,
           eventCondition: new[] { "ActionId:regex:^(46483)$" },
           userControl: true
       )]
    public void 破势乘浪(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var sourceObj = accessory.Data.Objects.FirstOrDefault(x => x.GameObjectId == sid);
        if (sourceObj == null) return;




        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "破势乘浪";
        dp.Owner = sid;
        dp.Scale = new Vector2(15f, 50f);
        dp.Position = @event.EffectPosition;
        dp.Rotation = sourceObj.Rotation;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 6700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }
    [ScriptMethod(
           name: "混合爆炸",//极限以太
           eventType: EventTypeEnum.StartCasting,
           eventCondition: new[] { "ActionId:regex:^(46507)$" },
           userControl: true
       )]
    public void 混合爆炸(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "混合爆炸";
        dp.Owner = sid;
        dp.Scale = new Vector2(9);
        dp.Position = @event.EffectPosition;
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 2700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }
}
#region 函数集

#region 计算函数

public static class MathTools
{
    public static float DegToRad(this float deg) => (deg + 360f) % 360f / 180f * float.Pi;
    public static float RadToDeg(this float rad) => (rad + 2 * float.Pi) % (2 * float.Pi) / float.Pi * 180f;
}

#endregion 计算函数

#region 绘图函数

public static class DrawTools
{
    /// <summary>
    /// 返回绘图
    /// </summary>
    /// <param name="sa"></param>
    /// <param name="ownerObj">绘图基准，可为UID或位置</param>
    /// <param name="targetObj">绘图指向目标，可为UID或位置</param>
    /// <param name="delay">延时delay ms出现</param>
    /// <param name="destroy">绘图自出现起，经destroy ms消失</param>
    /// <param name="name">绘图名称</param>
    /// <param name="radian">绘制图形弧度范围</param>
    /// <param name="rotation">绘制图形旋转弧度，以owner面前为基准，逆时针增加</param>
    /// <param name="width">绘制图形宽度，部分图形可保持与长度一致</param>
    /// <param name="length">绘制图形长度，部分图形可保持与宽度一致</param>
    /// <param name="innerWidth">绘制图形内宽，部分图形可保持与长度一致</param>
    /// <param name="innerLength">绘制图形内长，部分图形可保持与宽度一致</param>
    /// <param name="drawModeEnum">绘图方式</param>
    /// <param name="drawTypeEnum">绘图类型</param>
    /// <param name="isSafe">是否使用安全色</param>
    /// <param name="byTime">动画效果随时间填充</param>
    /// <param name="byY">动画效果随距离变更</param>
    /// <param name="draw">是否直接绘图</param>
    /// <returns></returns>
    public static DrawPropertiesEdit DrawOwnerBase(this ScriptAccessory sa,
        object ownerObj, object targetObj, int delay, int destroy, string name,
        float radian, float rotation, float width, float length, float innerWidth, float innerLength,
        DrawModeEnum drawModeEnum, DrawTypeEnum drawTypeEnum, bool isSafe = false,
        bool byTime = false, bool byY = false, bool draw = true)
    {
        var dp = sa.Data.GetDefaultDrawProperties();
        dp.Name = name;
        dp.Scale = new Vector2(width, length);
        dp.InnerScale = new Vector2(innerWidth, innerLength);
        dp.Radian = radian;
        dp.Rotation = rotation;
        dp.Color = isSafe ? sa.Data.DefaultSafeColor : sa.Data.DefaultDangerColor;
        dp.Delay = delay;
        dp.DestoryAt = destroy;
        dp.ScaleMode |= byTime ? ScaleMode.ByTime : ScaleMode.None;
        dp.ScaleMode |= byY ? ScaleMode.YByDistance : ScaleMode.None;

        switch (ownerObj)
        {
            case uint u:
                dp.Owner = u;
                break;
            case ulong ul:
                dp.Owner = ul;
                break;
            case Vector3 spos:
                dp.Position = spos;
                break;
            default:
                throw new ArgumentException($"ownerObj {ownerObj} 的目标类型 {ownerObj.GetType()} 输入错误");
        }

        switch (targetObj)
        {
            case 0:
            case 0u:
                break;
            case uint u:
                dp.TargetObject = u;
                break;
            case ulong ul:
                dp.TargetObject = ul;
                break;
            case Vector3 tpos:
                dp.TargetPosition = tpos;
                break;
            default:
                throw new ArgumentException($"targetObj {targetObj} 的目标类型 {targetObj.GetType()} 输入错误");
        }

        if (draw)
            sa.Method.SendDraw(drawModeEnum, drawTypeEnum, dp);
        return dp;
    }

    /// <summary>
    /// 返回扇形绘图
    /// </summary>
    /// <param name="sa"></param>
    /// <param name="ownerObj">圆心</param>
    /// <param name="targetObj">目标</param>
    /// <param name="delay">延时</param>
    /// <param name="destroy">消失时间</param>
    /// <param name="name">绘图名字</param>
    /// <param name="radian">弧度</param>
    /// <param name="rotation">旋转角度</param>
    /// <param name="outScale">外径</param>   
    /// <param name="innerScale">内径</param>
    /// <param name="byTime">是否随时间扩充</param>
    /// <param name="isSafe">是否安全色</param>
    /// <param name="draw">是否直接绘制</param>
    /// <returns></returns>
    public static DrawPropertiesEdit DrawFan(this ScriptAccessory sa,
        object ownerObj, object targetObj, int delay, int destroy, string name, float radian, float rotation,
        float outScale, float innerScale, bool isSafe = false, bool byTime = false, bool draw = true)
        => sa.DrawOwnerBase(ownerObj, targetObj, delay, destroy, name, radian, rotation, outScale, outScale, innerScale,
            innerScale, DrawModeEnum.Default, innerScale == 0 ? DrawTypeEnum.Fan : DrawTypeEnum.Donut, isSafe, byTime, false, draw);

    public static DrawPropertiesEdit DrawFan(this ScriptAccessory sa,
        object ownerObj, int delay, int destroy, string name, float radian, float rotation,
        float outScale, float innerScale, bool isSafe = false, bool byTime = false, bool draw = true)
        => sa.DrawFan(ownerObj, 0, delay, destroy, name, radian, rotation, outScale, innerScale, isSafe, byTime, draw);
}

#endregion 绘图函数

#endregion 函数集
