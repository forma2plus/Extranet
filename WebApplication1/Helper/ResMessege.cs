using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Helper
{
    public class ResMessege
    {

        public static MvcHtmlString getDanger(string message)
        {
            return new MvcHtmlString(
                @"<div class='alert alert-info alert-dismissible'>
  <a href='#' class='close mb-1' data-dismiss='alert' aria-label='close'>&times;</a>
               " + message + @"
</div>"
                );

        }
      

    }
}