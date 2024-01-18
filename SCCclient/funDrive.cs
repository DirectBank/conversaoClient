using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Globalization;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RestSharp;

/// <summary>
/// Descrição resumida de funDrive
/// </summary>
public class funDrive
{
   //Url de produção ou desenvolvimento
   //string URLBase = "http://localhost:3030/unidadez/api/drive";
   //string URLBase = "https://woserviceapi.brazilsouth.cloudapp.azure.com/unidadez/api/drive";
   //string URLBase = "https://woserviceapi.brazilsouth.cloudapp.azure.com/unidadez/api/v3/drive";
   string  URLBase = "https://ompapi.azurewebsites.net/unidadez/api/v3"; // docker

    public funDrive()
   {
      //
      // TODO: Adicionar lógica do construtor aqui
      //
   }

   #region Upload
   public bool uploadToDrive(string id_empresa, string id_cliente, string nomeTabela, string id_tabela, byte[] arquivo, string nomeArquivo)
   {
      using (var client = new HttpClient())
      {
         var content = new MultipartFormDataContent("Upload-" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
         content.Add(new StreamContent(new MemoryStream(arquivo)), "file", nomeArquivo);
         content.Headers.Add("x-api-key", "e841c.83ad575efcaf18a33079b6170ebe94ed");

         String queryParams = "id_empresa=" + id_empresa;
         queryParams += "&id_cliente=" + id_cliente;
         queryParams += "&file_name=" + Uri.EscapeUriString(id_tabela + Path.GetExtension(nomeArquivo).ToUpper());
         queryParams += "&file_path=" + Uri.EscapeUriString(nomeTabela.ToUpper());

         client.Timeout = TimeSpan.FromMinutes(30);

         var message = client.PostAsync(URLBase + "/upload?" + queryParams, content).Result;

         if (message.IsSuccessStatusCode)
         {
            var responseContent = message.Content;
            string responseString = responseContent.ReadAsStringAsync().Result;
            DriveApiPostResponse jsonString = JsonConvert.DeserializeObject<DriveApiPostResponse>(responseString);
            return true;
         }
         else
         {
            return false;
         }
      }
   }
   #endregion

   #region UploadWO
   public bool uploadToDriveWO(string id_empresa, string id_cliente, string nomeTabela, string id_tabela, byte[] arquivo, string nomeArquivo)
   {
      URLBase = "https://woserviceapi.brazilsouth.cloudapp.azure.com/wo/api/drive";

      using (var client = new HttpClient())
      {
         var content = new MultipartFormDataContent("Upload-" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
         content.Add(new StreamContent(new MemoryStream(arquivo)), "file", nomeArquivo);
         content.Headers.Add("x-api-key", "e841c.83ad575efcaf18a33079b6170ebe94ed");

         String queryParams = "id_empresa=" + id_empresa;
         queryParams += "&id_cliente=" + id_cliente;
         queryParams += "&file_name=" + Uri.EscapeUriString(id_tabela + Path.GetExtension(nomeArquivo).ToUpper());
         queryParams += "&file_path=" + Uri.EscapeUriString(nomeTabela.ToUpper());

         client.Timeout = TimeSpan.FromMinutes(30);

         var message = client.PostAsync(URLBase + "/upload?" + queryParams, content).Result;

         if (message.IsSuccessStatusCode)
         {
            var responseContent = message.Content;
            string responseString = responseContent.ReadAsStringAsync().Result;
            DriveApiPostResponse jsonString = JsonConvert.DeserializeObject<DriveApiPostResponse>(responseString);
            return true;
         }
         else
         {
            return false;
         }
      }
   }
   #endregion


   #region Download
   public byte[] downloadFromDrive(string id_empresa, string id_cliente, string nomeTabela, string nomeArquivo)
   {
      using (var client = new HttpClient())
      {

         String queryParams = "id_empresa=" + id_empresa;
         queryParams += "&id_cliente=" + id_cliente;
         queryParams += "&file_name=" + Uri.EscapeUriString(nomeArquivo);
         queryParams += "&file_path=" + Uri.EscapeUriString(nomeTabela);

         var message = client.GetAsync(URLBase + "/download?" + queryParams).Result;

         if (message.IsSuccessStatusCode)
         {
            var responseContent = message.Content;
            return responseContent.ReadAsByteArrayAsync().Result;
         }
         else
         {
            return null;
         }
      }
   }

   public byte[] downloadFromURL(string url)
   {
      using (var client = new HttpClient())
      {

         String queryParams = "file_url=" + url;

         var message = client.GetAsync(URLBase + "/download-url?" + queryParams).Result;

         if (message.IsSuccessStatusCode)
         {
            var responseContent = message.Content;
            return responseContent.ReadAsByteArrayAsync().Result;
         }
         else
         {
            return null;
         }
      }
   }

   public string downloadLinkFromDrive(string id_empresa, string id_cliente, string nomeTabela, string nomeArquivo)
   {
      using (var client = new HttpClient())
      {

         String queryParams = "id_empresa=" + id_empresa;
         queryParams += "&id_cliente=" + id_cliente;
         queryParams += "&file_name=" + Uri.EscapeUriString(nomeArquivo);
         queryParams += "&file_path=" + Uri.EscapeUriString(nomeTabela);

         var message = client.GetAsync(URLBase + "/link?" + queryParams).Result;

         if (message.IsSuccessStatusCode)
         {
            var responseContent = message.Content;
            return responseContent.ReadAsStringAsync().Result;
         }
         else
         {
            return null;
         }
      }
   }

   public string downloadLinkFromDriveUrl(string url)
   {

      //var message = client.GetAsync(URLBase+"/link?" + queryParams).Result;

      var client = new RestClient(URLBase + "/link-url");
      client.Timeout = -1;
      var request = new RestRequest(Method.GET);
      //client.AddDefaultHeader("file");
      client.AddDefaultParameter("file_url", url, ParameterType.QueryString);
      IRestResponse response = client.Execute(request);
      System.Diagnostics.Debug.WriteLine(response.Content);

      if (response.IsSuccessful)
      {
         dynamic bodyJson = JsonConvert.DeserializeObject(response.Content);
         return bodyJson.url[0];
      }
      else
      {
         return "";
      }


   }
   #endregion

   #region Delete
   public bool deleteFromUrl(string url)
   {
      try
      {
         var client = new RestClient(URLBase + "/url");
         client.Timeout = -1;
         var request = new RestRequest(Method.DELETE);
         client.AddDefaultParameter("file_url", url, ParameterType.QueryString);
         IRestResponse response = client.Execute(request);
         System.Diagnostics.Debug.WriteLine(response.Content);

         if (response.IsSuccessful)
         {
            return true;
         }
         else
         {
            return false;
         }
      }
      catch (Exception e)
      {
         return false;
      }

      return false;
   }

   public bool deleteFromDrive(string id_empresa, string id_cliente, string nomeTabela, string nomeArquivo)
   {
      using (var client = new HttpClient())
      {

         String queryParams = "id_empresa=" + id_empresa;
         queryParams += "&id_cliente=" + id_cliente;
         queryParams += "&file_path=" + Uri.EscapeUriString(nomeTabela);
         queryParams += "&file_name=" + Uri.EscapeUriString(nomeArquivo);

         var message = client.DeleteAsync(URLBase + "?" + queryParams).Result;

         if (message.IsSuccessStatusCode)
         {
            return true;
         }
         else
         {
            return false;
         }
      }
   }
   #endregion

   public class DriveApiPostResponse
   {
      string status;
      string url;
   }
}