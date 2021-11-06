using System.Collections.Generic;

namespace UpgradeWorld {

  public partial class Operations {
    /// <summary>Destroys everything in a zone so that the world generator can regenerate it.</summary>
    public static void RegenerateZone(Vector2i zone) {
      var zoneSystem = ZoneSystem.instance;
      if (!Settings.DestroyLoadedAreas && zoneSystem.IsZoneLoaded(zone)) {
        return;
      }
      var sectorObjects = new List<ZDO>();
      ZDOMan.instance.FindObjects(zone, sectorObjects);
      foreach (var zdo in sectorObjects) {
        if (Player.m_localPlayer && Player.m_localPlayer.GetZDOID() == zdo.m_uid) {
          continue;
        }
        var position = zdo.GetPosition();
        if (zoneSystem.GetZone(position) == zone) {
          ZDOMan.instance.RemoveFromSector(zdo, zone);
        }
      }
      var locations = zoneSystem.m_locationInstances;
      if (locations.TryGetValue(zone, out var location)) {
        location.m_placed = false;
        zoneSystem.m_locationInstances[zone] = location;
      }
      zoneSystem.m_generatedZones.Remove(zone);
    }

  }
}