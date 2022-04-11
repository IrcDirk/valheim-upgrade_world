using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
namespace UpgradeWorld;
///<summary>Runs the vegetation placement code for the filtered zones.</summary>
public class PlaceVegetation : VegetationOperation {
  public PlaceVegetation(Terminal context, HashSet<string> ids, FiltererParameters args) : base(context, args) {
    Operation = "Place vegetation";
    InitString = args.Print("Place vegetation at");
    args.TargetZones = TargetZones.Generated;
    Filterers = FiltererFactory.Create(args);
    New = GetWithOnlyIds(ids, true);
  }
  protected override bool ExecuteZone(Vector2i zone) {
    var zs = ZoneSystem.instance;
    if (zs.IsZoneLoaded(zone)) {
      Place(zone);
      return true;
    }
    zs.PokeLocalZone(zone);
    return false;
  }
  protected override void OnStart() {
    CountNewEntities.Counter = 0;
  }

  protected override void OnEnd() {
    base.OnEnd();
    var text = Operation + " completed.";
    if (Settings.Verbose) text += $" {CountNewEntities.Counter} vegetations placed.";
    if (Failed > 0) text += " " + Failed + " errors.";
    Print(text);
    CountNewEntities.Counter = 0;
  }
  private List<ZoneSystem.ClearArea> GetClearAreas(Vector2i zone) {
    List<ZoneSystem.ClearArea> clearAreas = new();
    var zs = ZoneSystem.instance;
    if (zs.m_locationInstances.TryGetValue(zone, out var location)) {
      if (location.m_location.m_location.m_clearArea)
        clearAreas.Add(new ZoneSystem.ClearArea(location.m_position, location.m_location.m_exteriorRadius));
    }
    return clearAreas;
  }
  private readonly List<GameObject> spawnedObjects = new();
  protected void Place(Vector2i zone) {
    var zs = ZoneSystem.instance;
    var root = zs.m_zones[zone].m_root;
    var zonePos = ZoneSystem.instance.GetZonePos(zone);
    var heightmap = Zones.GetHeightmap(root);
    var clearAreas = GetClearAreas(zone);
    spawnedObjects.Clear();
    zs.PlaceVegetation(zone, zonePos, root.transform, heightmap, clearAreas, ZoneSystem.SpawnMode.Full, spawnedObjects);
  }
}

[HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.CreateNewZDO), new[] { typeof(ZDOID), typeof(Vector3) })]
public class CountNewEntities {
  public static int Counter = 0;
  static void Prefix() {
    Counter++;
  }
}
