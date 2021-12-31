using System.Collections.Generic;
using System.Linq;

namespace UpgradeWorld {
  /// <summary>Removes given entity ids within a given distance.</summary>
  public class RemoveEntities : EntityOperation {
    public RemoveEntities(Terminal context, IEnumerable<string> ids, FiltererParameters args) : base(context) {
      if (Validate(ids))
        Remove(ids, args);
    }
    private void Remove(IEnumerable<string> ids, FiltererParameters args) {
      var prefabs = ids.Select(GetPrefabs).Aggregate((acc, list) => acc.Concat(list));
      var texts = prefabs.Select(id => {
        var zdos = GetZDOs(id, args);
        foreach (var zdo in zdos) Helper.RemoveZDO(zdo);
        return "Removed " + zdos.Count() + " of " + id + ".";
      });
      Log(texts);
    }
  }
}