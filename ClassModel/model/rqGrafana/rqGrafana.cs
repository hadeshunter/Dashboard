using System;
using System.Collections.Generic;
using System.Text;

namespace ClassModel.model.RqGrafana
{
    public class RqGrafana
    {
        public long panelId { get; set; }
        public Range range { get; set; }
        public RangeRaw rangeRaw { get; set; }
        public string interval { get; set; }
        public string intervalMs { get; set; }
        public List<Targets> targets { get; set; }
        public List<AdhocFilters> adhocFilters { get; set; }
        public string format { get; set; }
        public long maxDataPoints { get; set; }
    }
    public class Range
    {
        public string from { get; set; }
        public string to { get; set; }
        public Raw raw { get; set; }
    }
    public class Raw
    {
        public string from { get; set; }
        public string to { get; set; }
    }
    public class RangeRaw
    {
        public string from { get; set; }
        public string to { get; set; }
    }
    public class Targets
    {
        public string datasource { get; set; }
        public dynamic target { get; set; }
        public string refId { get; set; }
        public string type { get; set; }
        public dynamic data { get; set; }
    }
    public class AdhocFilters
    {
        public string key { get; set; }
        public string @operator { get; set; }
        public string value { get; set; }
    }
}
