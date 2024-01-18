using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace json {
   public class firebase {
      public object to, collapse_key;
      public data data;
      public notification notification;
   }
   public class data { public object body, title, click_action; }
   public class notification { public object body, title, icon; }

   public class response {
      public object multicast_id, success, failure, canonical_ids;
      public object[] results;
   }

   public class results { public object message_id; }

}
