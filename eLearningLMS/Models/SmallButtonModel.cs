using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace eLearningLMS.Models
{
    public class SmallButtonModel
    {
        public string Action { get; set; }

        public string Text { get; set; }

        public string Glyph { get; set; }

        public string ButtonType { get; set; }

        public int? Id { get; set; }
        public string UserId { get; set; }
        public string ActionParemeters
        {
            get
            {
                var param = new StringBuilder("?");
                if (Id != null && !Id.Equals(String.Empty))
                    param.Append(String.Format("{0}={1}&", "Id", Id));

                if (UserId != null && !UserId.Equals(String.Empty))
                    param.Append(String.Format("{0}={1}&", "UserId", UserId));

                return param.ToString().Substring(0, param.Length - 1);
            }
        }
    }
}