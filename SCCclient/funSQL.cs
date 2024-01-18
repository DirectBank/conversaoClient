using System;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.IO;

public class funSQL
{
   funDB funDB1;
   DataSet ds;

   public funSQL()
   {
      funDB1 = new funDB();
   }

   public DataSet BuscaSQLAzure(string sCmd, ref string sErro)
   {
      DataSet ds = new DataSet();
      sErro = "";
      try
      {
         SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conAzure);
         da.SelectCommand.CommandTimeout = 0;
         da.Fill(ds, "busca");
      }
      catch (SqlException oErro)
      {
         sErro = oErro.Message;
      }
      finally
      {
         funDB1.fecharAzure();
      }
      return ds;
   }

   public DataSet BuscaSQLTaco(string sCmd, ref string sErro)
   {
      DataSet ds = new DataSet();
      sErro = "";
      try
      {
         SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTaco);
         da.SelectCommand.CommandTimeout = 0;
         da.Fill(ds, "busca");
      }
      catch (SqlException oErro)
      {
         sErro = oErro.Message;
      }
      finally
      {
         funDB1.fecharTaco();
      }
      return ds;
   }
}