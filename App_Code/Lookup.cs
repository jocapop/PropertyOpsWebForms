using System;

namespace PropertyOps.App
{
    public static class Lookup
    {
        public static int GetCategoryIdByName(string name)
        {
            var o = Db.Scalar("SELECT TOP 1 CategoryId FROM dbo.Categories WHERE Name=@n", Db.P("@n", name));
            return Convert.ToInt32(o);
        }

        public static int GetBusinessLineIdByName(string name)
        {
            var o = Db.Scalar("SELECT TOP 1 BusinessLineId FROM dbo.BusinessLines WHERE Name=@n", Db.P("@n", name));
            return Convert.ToInt32(o);
        }
    }
}