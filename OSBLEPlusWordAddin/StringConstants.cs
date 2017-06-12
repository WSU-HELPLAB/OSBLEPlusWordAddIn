namespace OSBLEPlusWordAddin
{
    internal class StringConstants
    {
#if DEBUG
        internal static readonly string DataServiceRoot = "http://localhost/plusservices/";
#else
        internal static readonly string DataServiceRoot = "https://plus.osble.org/plusservices/";
#endif
    }
}