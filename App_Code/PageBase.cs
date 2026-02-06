using System;
using System.Web;
using System.Web.UI;
using PropertyOps.App;

namespace PropertyOpsWebForms
{
    public class PageBase : System.Web.UI.Page
    {
        protected int? CurrentUserId
        {
            get
            {
                if (Context == null || Context.User == null || Context.User.Identity == null) return null;
                if (!Context.User.Identity.IsAuthenticated) return null;

                int id;
                if (int.TryParse(Context.User.Identity.Name, out id)) return id;
                return null;
            }
        }

        protected bool IsAdmin()
        {
            int? id = CurrentUserId;
            if (!id.HasValue) return false;

            // ovde ide logika admina (primer)
            return id.Value == 1;
        }
    }

}