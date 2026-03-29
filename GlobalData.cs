namespace ImpostersOrdeal
{
    /// <summary>
    ///  Stores data for easy access.
    /// </summary>
    public static class GlobalData
    {
        public static string GetZoneName(int index)
        {
            if (index > -1 && index < Zones.zoneNames.Length)
                return Zones.zoneNames[index];

            return Zones.defaultName;
        }
    }
}
