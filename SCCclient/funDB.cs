using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

public class funDB
{
   public SqlConnection conAzure; // Azure - Direct
   public SqlConnection conTaco;    // Omeupredio
   public SqlConnection conTacoWO;    // Omeupredio

   private string sCatalog = "";
   private string sUserID = "";
   private string sPassword = "";
   private string sDataSource = "";
   private string sConkey = "";

   public funDB()
   {

   }

   #region conectarAzure() / fecharAzure() :: Cria a conexão com o banco de dados Azure
   public string conectarAzure()
   {
      sCatalog = "direct";
      //sUserID = "desktop";
      //sPassword = "D052720&0228ESK2114";
      sUserID = "direct";
      sPassword = "Sql159753@#!";

      sDataSource = "direct.database.windows.net";

      string sConexao = "";
      sConexao = "Initial Catalog=" + sCatalog + "; user id=" + sUserID + "; password=" + sPassword + "; Pooling=False; Data Source=" + sDataSource + "; Persist Security Info = False; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30; Max Pool Size=;Min Pool Size=";
      conAzure = new SqlConnection(sConexao);

      string strRetorno = "";
      try
      {
         int iCon = (int)conAzure.State;
         if (iCon == 0)
         {
            conAzure.Open();
            strRetorno = "OK";
         }
         else if (iCon == 1)
            strRetorno = "OK";
      }
      catch (Exception e)
      {
         strRetorno = e.Message;
      }
      return strRetorno;
   }


   public void fecharAzure()
   {
      try
      {
         if (conAzure.State != 0)
            conAzure.Close();
      }
      catch (Exception e)
      {
         throw new Exception("Não foi possível encerrar a conexão: " + e.Message);
      }
   }

   public DataSet BuscaSQLAzure(string sCmd, ref string sErro)
   {
      DataSet ds = new DataSet();
      sErro = "";
      try
      {
         SqlDataAdapter da = new SqlDataAdapter(sCmd, conAzure);
         da.SelectCommand.CommandTimeout = 0;
         da.Fill(ds, "busca");
      }
      catch (SqlException oErro)
      {
         sErro = oErro.Message;
      }
      finally
      {
         fecharAzure();
      }
      return ds;
   }
   #endregion

   #region conectarTaco() / fecharTaco() :: Cria a conexão com o banco de dados OMP - TACO
   public string conectarTaco()
   {
      sCatalog = "omp";
      sUserID = "ompweb2";
      sPassword = "Omp5683#1@4091712";
      sDataSource = "sql2k5.omeupredio.com.br,5683"; // IP externo

      string sConexao = "";
      sConexao = "Initial Catalog=" + sCatalog + ";user id=" + sUserID + ";password=" + sPassword + ";Data Source=" + sDataSource + ";Max Pool Size=;Min Pool Size=";
      conTaco = new SqlConnection(sConexao);

      string strRetorno = "";
      try
      {
         int iCon = (int)conTaco.State;
         if (iCon == 0)
         {
            conTaco.Open();
            strRetorno = "OK";
         }
         else if (iCon == 1)
            strRetorno = "OK";
      }
      catch (Exception e)
      {
         strRetorno = e.Message;
      }
      return strRetorno;
   }

   public void fecharTaco()
   {
      try
      {
         if (conTaco.State != 0)
            conTaco.Close();
      }
      catch (Exception e)
      {
         throw new Exception("Não foi possível encerrar a conexão: " + e.Message);
      }
   }

   public DataSet BuscaSQLTaco(string sCmd, ref string sErro)
   {
      DataSet ds = new DataSet();
      sErro = "";
      try
      {
         SqlDataAdapter da = new SqlDataAdapter(sCmd, conTaco);
         da.SelectCommand.CommandTimeout = 0;
         da.Fill(ds, "busca");
      }
      catch (SqlException oErro)
      {
         sErro = oErro.Message;
      }
      finally
      {
         fecharTaco();
      }
      return ds;
   }
   #endregion

   #region conectarTacoWO() / fecharTacoWO() :: Cria a conexão com o banco de dados WorkOffice - TACO
   public string conectarTacoWO()
   {
      sCatalog = "workoffice_prod";
      sUserID = "prgwo";
      sPassword = "#Wo$68@bE1";
      sDataSource = "sql.workoffice.com.br,5683"; // IP externo

      string sConexao = "";
      sConexao = "Initial Catalog=" + sCatalog + ";user id=" + sUserID + ";password=" + sPassword + ";Data Source=" + sDataSource + ";Max Pool Size=;Min Pool Size=";
      conTacoWO = new SqlConnection(sConexao);

      string strRetorno = "";
      try
      {
         int iCon = (int)conTacoWO.State;
         if (iCon == 0)
         {
            conTacoWO.Open();
            strRetorno = "OK";
         }
         else if (iCon == 1)
            strRetorno = "OK";
      }
      catch (Exception e)
      {
         strRetorno = e.Message;
      }
      return strRetorno;
   }

   public void fecharTacoWO()
   {
      try
      {
         if (conTacoWO.State != 0)
            conTacoWO.Close();
      }
      catch (Exception e)
      {
         throw new Exception("Não foi possível encerrar a conexão: " + e.Message);
      }
   }

   public DataSet BuscaSQLTacoWO(string sCmd, ref string sErro)
   {
      DataSet ds = new DataSet();
      sErro = "";
      try
      {
         SqlDataAdapter da = new SqlDataAdapter(sCmd, conTacoWO);
         da.SelectCommand.CommandTimeout = 0;
         da.Fill(ds, "busca");
      }
      catch (SqlException oErro)
      {
         sErro = oErro.Message;
      }
      finally
      {
         fecharTacoWO();
      }
      return ds;
   }
   #endregion

}