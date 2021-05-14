namespace ClassModel.model.unit
{
    public class Unit
    {
        public int donvi_id { get; set; }
        public string ten_dv { get; set; }
        public int donvi_cha_id { get; set; }
        public string ten_dvql { get; set; }
    }
    public class UnitRequest
    {
        public string target { get; set; }
    }
    public class TagRequest
    {
        public string key { get; set; }
    }
}
