using System.Collections.Generic;
using System.Linq;
using Service;

namespace UpgradeWorld;
/// <summary>Counts the amounts of given entities.</summary>
public class CountObjects : EntityOperation
{
  public CountObjects(Terminal context, List<string> ids, DataParameters args, Range<int> countRange) : base(context)
  {
    Count(ids, args, countRange);
  }
  private void Count(List<string> ids, DataParameters args, Range<int> countRange)
  {
    var prefabs = GetPrefabs(ids);
    var zdos = GetZDOs(args, prefabs);
    var total = 0;
    var counts = prefabs.ToDictionary(prefab => prefab, prefab => 0);
    foreach (var zdo in zdos)
    {
      if (!args.Roll()) continue;
      counts[zdo.m_prefab] += 1;
      total += 1;
    }
    var linq = counts.Where(kvp => kvp.Value >= countRange.Min && kvp.Value < countRange.Max).Select(kvp => $"{GetName(kvp.Key)}: {kvp.Value}");
    string[] texts = [$"Total: {total}", .. linq];
    if (args.Log) Log(texts);
    else Print(texts, false);
    PrintPins();
  }
}
