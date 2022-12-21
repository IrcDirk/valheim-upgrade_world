using UnityEngine;

namespace UpgradeWorld;
/// <summary>Generates zones.</summary>
public class Generate : ZoneOperation
{
  public Generate(Terminal context, FiltererParameters args) : base(context, args)
  {
    Operation = "Generate";
    InitString = args.Print("Generate");
    Filterers = FiltererFactory.Create(args);
  }

  protected override bool ExecuteZone(Vector2i zone)
  {
    var zs = ZoneSystem.instance;
    if (zs.IsZoneGenerated(zone)) return true;
    GameObject go;
    return zs.SpawnZone(zone, ZoneSystem.SpawnMode.Ghost, out go);
  }
  protected override void OnEnd()
  {
    var generated = ZonesToUpgrade.Length - Failed;
    var text = Operation + " completed.";
    if (Settings.Verbose) text += " " + generated + " zones generated.";
    if (Failed > 0) text += " " + Failed + " errors.";
    Print(text);
  }
}
