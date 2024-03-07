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
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

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
   string configJson = "{\r\n   \"type\": \"service_account\",\r\n   \"project_id\": \"workoffice-drive\",\r\n   \"private_key_id\": \"e800e816c8e1178f0aed7b68abf83a9d00d83029\",\r\n   \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC4g7TwBrWkP4VR\\n8XaveY+d0jVPJ1PxGAaNIijqfuNzLTNjqejJFiD6DnCFVHF8ksw2XI0SjnBgFv07\\n5xkvQhoKcT9re0EpL34/+w5AJg/9f5nq1vebMIroRZfLdc+qQ3w8AdQm/8vQmtuA\\nDeChO7eOa5g1gA/EFmnuhoPT+EnZJG2V4rXoFxUrmnL/HSeQwLptt1rs2ZNjc7CV\\nWkpGmYqkRAQv43dgYtZj8b54TIhFA/gU2wPbZiS4VuueCqWiQuzTYzMUyuILx31L\\nmPYn4QsWxpkT/AvrK4iTr/hKedU6cB/E3SfPZOQkBC5w1zlqsn5BMrzTYvoDU9iI\\nHORDk/6hAgMBAAECggEAARdKRRcNEOUNQUBHvt7dogXBqe+mF+lX9JKsokJQzvgH\\n80FobnPyivf141vt9doaWgHvFZqBAJhyM934cKaX59LmmJiYeIle2jr+SRPQiWOt\\nQc22lhTR5XCv5pSSP0P54pLyMa7WgivAO4AZgWqMecuaEUrqDPNC+hWShvjTWvTR\\nFZgFOnoiIH5TNnCIMw+ieBLOzYd9ZqwiXnUYhUuab4SQtOqLwqIcdwtaZFwRLGaG\\n96LIBZaHJgrQJ8MJDchEvwIn04wibG7ZuROrGMhsK937ws+RjyX7LkP5Ml1V+LxC\\nQudHzeoVIQnioXU+NvS5ZeYswcbGHuXFUgK9+ZH6qQKBgQD8v8SSzX68Ug4wb64F\\n64Z64qMrTPWo9hOOcbufns1apUOI3GloEycGWLf8gzDjl6uLIVVUlDyJ35kfGGQT\\nRBY9zmF2mK/3ln13W1tiS6ago+V9Lljjm9JvswzMt1prYloymOGetN9WIzU2a+V2\\nVDeZ3Zj6gl4ttKiNuy1EfEkhPQKBgQC640LrnIDTUNN5OBE6W6zZqpteo1SaDTpz\\nAfM+Ya/1HmHHUAG0rMgihZCsrR/qZYho4TCwBmLsuGUkVp5veWDbGY091k8gQZPp\\nlCcAPP/6m7ywWQhScqp4LYAuUwTD+aBVclBKizyGU2Ewm6en3rInCGIpaMgWL7DD\\ngzubm7hhNQKBgQDQjHtqll0IjrxegwgYompoYzE3vVzGeaVRV870ule/f7Xl69id\\no5AD0JifprBkWvWU64A5NcduDC2QVtPcgcXIYc5RyVMI/AeywJL63Gk1C4eEbwWx\\naRWOTTM2h+P3z0OVlEg2aBAQRyTVLto7dOob75kWuxNyqyqZJ+UGKXc+EQKBgHm4\\nIpHJ8K2w6sr8lVvo8X8i+uZ8glGDZBobnw12GmAPVae2mCXQktjJHR0Z9Lt5PYrx\\ngABlaHC8+ELel3oLF+Ybkj5AInDjxS5Qa8Zf9GiInjBNDHqGbDixidaiA2yQXLjK\\nJzklzlm+XIKIHn3bMTTy5NwLfqXUkdAE3QHQPhoxAoGAMz+8hS4oYDkmDqJ9F2FZ\\nmKEZJGYRnmecBYo8DkJlfMthm/78sDGaA8U3hc1iLx4vJSE9nvY/vRCpG5Qnv6KC\\nQt4oBxF1QClL70S99NucvVhTSpTT1d0vyPxUvUSwKV+dX3xzOTw9JRXzYTlzifjR\\nOOrnpsIhAsQ2oYjLpbCDyUc=\\n-----END PRIVATE KEY-----\\n\",\r\n   \"client_email\": \"firebase-adminsdk-ktv5s@workoffice-drive.iam.gserviceaccount.com\",\r\n   \"client_id\": \"101051626630813008907\",\r\n   \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\r\n   \"token_uri\": \"https://oauth2.googleapis.com/token\",\r\n   \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\r\n   \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-ktv5s%40workoffice-drive.iam.gserviceaccount.com\"\r\n}\r\n";

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

   public bool uploadSCCToDrive(string nomeArquivo, string mimeType, byte[] bArquivo)
   {
      bool bRetorno = true;

      string bucketName = "workoffice-drive.appspot.com";

      GoogleCredential credential = null;
      credential = GoogleCredential.FromJson(configJson);
      var storageClient = StorageClient.Create(credential);

      var options = new ListObjectsOptions { Delimiter = "/" };

      try
      {
         MemoryStream msFile = new MemoryStream(bArquivo);

         storageClient.UploadObject(bucketName, nomeArquivo, mimeType, msFile);
      }
      catch
      {
         bRetorno = false;
      }

      return bRetorno;
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

   public string retornaMimeType(string sExtensao)
   {
      string sRetorno = "";

      sExtensao = sExtensao.ToLower();

      if (sExtensao.IndexOf("png") >= 0)
         sRetorno = "image/png";
      else if (sExtensao.IndexOf("jpg") >= 0)
         sRetorno = "image/jpeg";
      else if (sExtensao.IndexOf("jpeg") >= 0)
         sRetorno = "image/jpeg";
      else if (sExtensao.IndexOf("gif") >= 0)
         sRetorno = "image/gif";
      else if (sExtensao.IndexOf("svg") >= 0)
         sRetorno = "image/svg+xml";
      else if (sExtensao.IndexOf("webp") >= 0)
         sRetorno = "image/webp";
      else if (sExtensao.IndexOf("webm") >= 0)
         sRetorno = "audio/webm";
      else if (sExtensao.IndexOf("pdf") >= 0)
         sRetorno = "application/pdf";
      else if (sExtensao.IndexOf("doc") >= 0)
         sRetorno = "application/msword";
      else if (sExtensao.IndexOf("docx") >= 0)
         sRetorno = "application/msword";
      else if (sExtensao.IndexOf("xls") >= 0)
         sRetorno = "application/msexcel";
      else if (sExtensao.IndexOf("xlsx") >= 0)
         sRetorno = "application/msexcel";
      else if (sExtensao.IndexOf("ppt") >= 0)
         sRetorno = "application/vnd.ms-powerpoint";
      else if (sExtensao.IndexOf("pptx") >= 0)
         sRetorno = "application/vnd.ms-powerpoint";
      else if (sExtensao.IndexOf("pps") >= 0)
         sRetorno = "application/vnd.ms-powerpoint";
      else if (sExtensao.IndexOf("ppsx") >= 0)
         sRetorno = "application/vnd.ms-powerpoint";
      else if (sExtensao.IndexOf("zip") >= 0)
         sRetorno = "application/zip";
      else
         sRetorno = "text/plain";

      return sRetorno;
   }

   public class DriveApiPostResponse
   {
      string status;
      string url;
   }
}