using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Data.SqlClient;
using System.Xml;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public class Funcoes
{
	public Funcoes()
	{
	}

   /// <summary>
   /// Converte int em string, completando zeros a esquerda
   /// </summary>
   /// <param name="Fnumero">Número inteiro a ser convertido</param>
   /// <param name="Ftamanho">Tamanho da string de retorno</param>
   /// <returns></returns>
   public string STRZERO(int Fnumero, int Ftamanho)
   {
      string Wnumero = Fnumero.ToString();
      string Wretorno = "";
      int Wzeros = (Ftamanho - Wnumero.Length);

      if (Wzeros > 0)
      {
         for (int i = 1; i <= Wzeros; i++)
         {
            Wretorno += "0";
         }
         Wretorno += Wnumero;
      }
      else
      {
         Wretorno = Wnumero.Substring(0, Ftamanho);
      }
      return Wretorno;
   }

   /// <summary>
   /// Retorna a data de hoje como uma string no formato DD/MM/AAAA ou MM/AAAA
   /// </summary>
   /// <param name="Fmodo">
   /// 0 - MM/DD/AAAA
   /// 1 - DD/MM/AAAA 
   /// 2 - MM/AAAA
   /// 3 - AAAAMM
   /// </param>
   /// <param name="Fincremento">
   /// 0 - Data atual
   /// Números positivos, acrescentam dias na data de hoje...
   /// 1 = Data de amanhã
   /// Número negativos, decrementam dias na data de hoje...
   /// -1 = Data de ontem
   /// </param>
   /// <returns></returns>
   public string dataHoje(int Fmodo, int Fincremento)
   {
      string Wretorno = "";
      DateTime Whoje = DateTime.Now;
      DateTime Wdata = Whoje.AddDays(Fincremento);

      switch (Fmodo)
      {
         case 0:
            Wretorno = STRZERO(Wdata.Month, 2) + "/";
            Wretorno += STRZERO(Wdata.Day, 2) + "/";
            Wretorno += Convert.ToString(Wdata.Year);
            break;
         case 1:
            Wretorno = STRZERO(Wdata.Day, 2) + "/";
            Wretorno += STRZERO(Wdata.Month, 2) + "/";
            Wretorno += Convert.ToString(Wdata.Year);
            break;
         case 2:
            Wretorno = STRZERO(Wdata.Month, 2) + "/";
            Wretorno += Convert.ToString(Wdata.Year);
            break;
         case 3:
            Wretorno = Convert.ToString(Wdata.Year);
            Wretorno += STRZERO(Wdata.Month, 2);
            break;
      }
      return Wretorno;
   }

   /// <summary>
   /// Retorna o resultado de uma data acrescida ou subtraida de um inteiro como uma string no formato DD/MM/AAAA ou MM/AAAA
   /// </summary>
   /// <param name="Fdata">
   /// Data base para adição ou subtração
   /// </param>
   /// <param name="Fmodo">
   /// 0 - MM/DD/AAAA
   /// 1 - DD/MM/AAAA 
   /// 2 - MM/AAAA
   /// 3 - AAAAMM
   /// </param>
   /// <param name="Fincremento">
   /// 0 - Data atual
   /// Números positivos, acrescentam dias na data de hoje...
   /// 1 = Data de amanhã
   /// Número negativos, decrementam dias na data de hoje...
   /// -1 = Data de ontem
   /// </param>
   /// <returns></returns>
   public string somaData(DateTime dData, int Fmodo, int Fincremento)
   {
      string Wretorno = "";
      DateTime Whoje = dData;
      DateTime Wdata = Whoje.AddDays(Fincremento);

      switch (Fmodo)
      {
         case 0:
            Wretorno = STRZERO(Wdata.Month, 2) + "/";
            Wretorno += STRZERO(Wdata.Day, 2) + "/";
            Wretorno += Convert.ToString(Wdata.Year);
            break;
         case 1:
            Wretorno = STRZERO(Wdata.Day, 2) + "/";
            Wretorno += STRZERO(Wdata.Month, 2) + "/";
            Wretorno += Convert.ToString(Wdata.Year);
            break;
         case 2:
            Wretorno = STRZERO(Wdata.Month, 2) + "/";
            Wretorno += Convert.ToString(Wdata.Year);
            break;
         case 3:
            Wretorno = Convert.ToString(Wdata.Year);
            Wretorno += STRZERO(Wdata.Month, 2);
            break;
      }
      return Wretorno;
   }

   /// <summary>
   /// Incrementa uma data em 'n' dias
   /// </summary>
   /// <param name="Fdata">Data a ser incrementada</param>
   /// <param name="Fincremento">nr de dias a ser acrescentado (quando positivo) ou decrementado (quando negativo)</param>
   /// <returns></returns>
   public string dataSomaDia(string Fdata, int Fincremento)
   {
      string Wretorno = "";

      int Wano = Convert.ToInt32(Fdata.Substring(6));
      int Wmes = Convert.ToInt32(Fdata.Substring(3, 2));
      int Wdia = Convert.ToInt32(Fdata.Substring(0, 2));
      DateTime Wdata = new DateTime(Wano, Wmes, Wdia);
      DateTime Wnovadata = Wdata.AddDays(Fincremento);

      Wretorno = STRZERO(Wnovadata.Day, 2) + "/";
      Wretorno += STRZERO(Wnovadata.Month, 2) + "/";
      Wretorno += Convert.ToString(Wnovadata.Year);

      return Wretorno;
   }

   /// <summary>
   /// Retorna a data do ultimo dia do mes ano selecionado
   /// </summary>
   /// <param name="Fmes">Mes</param>
   /// <param name="Fano">Ano</param>
   /// <returns>String no formato DD/MM/AAAA</returns>
   public string dataUltimoDiaMes(int Fmes, int Fano)
   {
      string Wretorno = "";
      Fmes++;
      if (Fmes == 13)
      {
         Fmes = 1;
         Fano++;
      }
      DateTime Wdata = new DateTime(Fano, Fmes, 1).AddDays(-1);
      Wretorno = STRZERO(Wdata.Day, 2) + "/";
      Wretorno += STRZERO(Wdata.Month, 2) + "/";
      Wretorno += Convert.ToString(Wdata.Year);

      return Wretorno;
   }

   /// <summary>
   /// Ajusta a data de pagamento para um dia útil. Se for sábado ou domingo, muda a data para próxima segunda-feira
   /// </summary>
   /// <param data => data de pagamento
   /// <returns>Retora a data de pagamento válida</returns>
   public string dataPagamentoValida(string data)
   {
      string sData = data;
      if (sData != "")
      {
         DateTime fData = Convert.ToDateTime(sData);

         if (fData.DayOfWeek.ToString().ToUpper() == "SUNDAY")
            sData = dataSomaDia(sData, 1);
         else if (fData.DayOfWeek.ToString().ToUpper() == "SATURDAY")
            sData = dataSomaDia(sData, 2);
      }
      return sData;
   }

   public System.Drawing.Color corHexToDec(string FcorHexa)
   {
      int r = 0, g = 0, b = 0;
      if (FcorHexa.Trim() != "")
      {
         r = Convert.ToInt32(FcorHexa.Substring(1, 2), 16);
         g = Convert.ToInt32(FcorHexa.Substring(3, 2), 16);
         b = Convert.ToInt32(FcorHexa.Substring(5, 2), 16);
      }
      return System.Drawing.Color.FromArgb(255, r, g, b);
   }

   /// <summary>
   /// Pega o valor de um nó dentro de um XML e pode trazer ou nao o id_...
   /// </summary>
   /// <param name="sConteudoXML">
   /// Conteudo Xml a ser enviado
   /// </param>
   /// <param name="no">
   /// Nome do campo a ser pesquisado dentro do XML
   /// </param>
   /// <param name="sCampo">
   /// Nome do campo a ser pego o ID...
   /// </param>
   /// <param name="sTabela">
   /// Nome da tabela a ser pesquisado o ID...
   /// </param>
    /// <returns></returns>
   public string busca_dados_xml(string sConteudoXML, string no, string sCampo, string sTabela)
   {
      string strRetorno = "";
      XmlDocument xmlConfig = new XmlDocument();
      xmlConfig.LoadXml(sConteudoXML);
      XmlNodeList nl = xmlConfig.GetElementsByTagName(no);
      strRetorno = nl.Item(0).InnerText;
      return strRetorno;
   }

   public bool IsNumeric(object obj)
   {
      bool isNum;
      // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
      double retNum;

      // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
      // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
      isNum = Double.TryParse(Convert.ToString(obj), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
      return isNum;
   }

   public string LimpaLinha(string sTexto)
   {
      string sComAcento = "ÁÉÍÓÚáéíóúÀÈÌÒÙàèìòùÂÊÎÔÛâêîôûÄËÏÖÜäëïöüÃÕãõÇçÑñºª'&°´–¸§µ“”";
      string sSemAcento = "AEIOUaeiouAEIOUaeiouAEIOUaeiouAEIOUaeiouAOaoCcNnoa e  -     ";
      string sNovoTexto = "";
      for (int i = 0; i <= (sTexto.Length - 1); i++)
         if (sComAcento.IndexOf(sTexto.Substring(i, 1)) >= 0)
            sNovoTexto += sSemAcento.Substring(sComAcento.IndexOf(sTexto.Substring(i, 1)), 1);
         else
            sNovoTexto += sTexto.Substring(i, 1);
      if (sNovoTexto.IndexOf("\r") > 0)
         sNovoTexto = sNovoTexto.Substring(0, sNovoTexto.IndexOf("\r") - 0) + sNovoTexto.Substring(sNovoTexto.IndexOf("\r") + 2);
      if (sNovoTexto.IndexOf("\n") > 0)
         if (sNovoTexto.IndexOf("\n") + 1 > Convert.ToInt32(sNovoTexto.Length))
            sNovoTexto = sNovoTexto.Substring(0, sNovoTexto.IndexOf("\n") - 1) + sNovoTexto.Substring(sNovoTexto.IndexOf("\n") + 2);
         else
            sNovoTexto = sNovoTexto.Substring(0, sNovoTexto.IndexOf("\n") - 1);
      return sNovoTexto;
   }

    public string LimpaString(string sTexto)
    {
        string sComAcento = "ÁÉÍÓÚáéíóúÀÈÌÒÙàèìòùÂÊÎÔÛâêîôûÄËÏÖÜäëïöüÃÕãõÇçÑñºª'&´°";
        string sSemAcento = "AEIOUaeiouAEIOUaeiouAEIOUaeiouAEIOUaeiouAOaoCcNnoa e  ";
        string sNovoTexto = "";
        for (int i = 0; i <= (sTexto.Length - 1); i++)
        {
            if (sComAcento.IndexOf(sTexto.Substring(i, 1)) >= 0)
                sNovoTexto += sSemAcento.Substring(sComAcento.IndexOf(sTexto.Substring(i, 1)), 1);
            else
                sNovoTexto += sTexto.Substring(i, 1);
        }
        return sNovoTexto;
    }

    /// <summary>
    /// Troca determinada sequencia de caracteres por outra dentro de uma string
    /// </summary>
    /// <param name="sTexto">String a ser analisada</param>
    /// <param name="sBusca">Texto a ser trocado ('/r/n')</param>
    /// <param name="sTroca">Texto de substituição ('<br>')</param>
    /// <returns></returns>
    public string trocaCaracter(string sTexto, string sBusca, string sTroca)
   {
      int x = 0;
      x = sTexto.IndexOf(sBusca);
      sTexto = sTexto.Replace("\r", "");
      sTexto = sTexto.Replace("\n", "<br>");
      return sTexto;
   }

   /// <summary>
   /// Troca determinada sequencia de caracteres por outra dentro de uma string
   /// </summary>
   /// <param name="sTexto">String a ser analisada</param>
   /// <param name="sBusca">Texto a ser trocado</param>
   /// <param name="sTroca">Texto de substituição</param>
   /// <returns></returns>
   public string trocaCaracterNovo(string sTexto, string sBusca, string sTroca)
   {
      int x = 0;
      x = sTexto.IndexOf(sBusca);
      if (x > 0)
         while (x > 0)
         {
            sTexto = sTexto.Substring(0, x) + sTroca + sTexto.Substring(x + 1);
            x = sTexto.IndexOf(sBusca);
         }
      return sTexto;
   }

   public string SomenteNumero(string sTexto)
   {
      string sNumeros = "0123456789";
      string sTextoNew = "";

      for (int i = 0; i <= (sTexto.Length - 1); i++)
         if (sNumeros.IndexOf(sTexto.Substring(i, 1)) >= 0)
            sTextoNew += sTexto.Substring(i, 1);

      return sTextoNew;
   }

   public string TrocaAcentuacao(string sTexto)
   {
      string sTextoNovo = "", sCaracter;
      for (int i = 0; i < sTexto.Length; i++)
      {
         sCaracter = sTexto.Substring(i, 1);

         if (sCaracter.Equals("á"))
            sTextoNovo += "&aacute;";
         else if (sCaracter.Equals("â"))
            sTextoNovo += "&acirc;";
         else if (sCaracter.Equals("é"))
            sTextoNovo += "&eacute;";
         else if (sCaracter.Equals("í"))
            sTextoNovo += "&iacute;";
         else if (sCaracter.Equals("õ"))
            sTextoNovo += "&otilde;";
         else if (sCaracter.Equals("ú"))
            sTextoNovo += "&uacute;";
         else if (sCaracter.Equals("ç"))
            sTextoNovo += "&ccedil;";
         else if (sCaracter.Equals("Á"))
            sTextoNovo += "&Aacute;";
         else if (sCaracter.Equals("Â"))
            sTextoNovo += "&Acirc;";
         else if (sCaracter.Equals("É"))
            sTextoNovo += "&Eacute;";
         else if (sCaracter.Equals("Í"))
            sTextoNovo += "&Iacute;";
         else if (sCaracter.Equals("Õ"))
            sTextoNovo += "&Otilde;";
         else if (sCaracter.Equals("Ú"))
            sTextoNovo += "&Uacute;";
         else if (sCaracter.Equals("Ç"))
            sTextoNovo += "&Ccedil;";
         else if (sCaracter.Equals("ã"))
            sTextoNovo += "&atilde;";
         else if (sCaracter.Equals("à"))
            sTextoNovo += "&agrave;";
         else if (sCaracter.Equals("ê"))
            sTextoNovo += "&ecirc;";
         else if (sCaracter.Equals("ó"))
            sTextoNovo += "&oacute;";
         else if (sCaracter.Equals("ô"))
            sTextoNovo += "&ocirc;";
         else if (sCaracter.Equals("ü"))
            sTextoNovo += "&uuml;";
         else if (sCaracter.Equals("Ã"))
            sTextoNovo += "&Atilde;";
         else if (sCaracter.Equals("À"))
            sTextoNovo += "&Agrave;";
         else if (sCaracter.Equals("Ê"))
            sTextoNovo += "&Ecirc;";
         else if (sCaracter.Equals("Ó"))
            sTextoNovo += "&Oacute;";
         else if (sCaracter.Equals("Ô"))
            sTextoNovo += "&Ocirc;";
         else if (sCaracter.Equals("Ü"))
            sTextoNovo += "&Uuml;";
         else
            sTextoNovo += sCaracter;
      }
      return sTextoNovo;
   }
   public string gerarSenha()
   {
      //string caracteres = "ABCDEFGHIJLMNOPQRSTUWVYXZabcdefghijklmonpqrstuwvyxz1234567890@#$&*!%_";
      string caracteresLetra = "ABCDEFGHIJLMNOPQRSTUWVYXZabcdefghijklmonpqrstuwvyxz";
      char[] letras = caracteresLetra.ToCharArray();
      embaralharSenha(ref letras, 5);

      string caracteresNumero = "1234567890";
      char[] numeros = caracteresNumero.ToCharArray();
      embaralharSenha(ref numeros, 5);

      string caracteresSimbolo = "@#$&*!%_";
      char[] simbolos = caracteresSimbolo.ToCharArray();
      embaralharSenha(ref simbolos, 5);

      // Monta uma nova string com 5 letras, 2 números e 1 simbolo
      string sLetras = new String(letras).Substring(0, 5);
      string sNumeros = new String(numeros).Substring(0, 2);
      string sSimbolos = new String(simbolos).Substring(0, 1);

      string caracteres = sLetras + sNumeros + sSimbolos;
      char[] caracter = caracteres.ToCharArray();
      embaralharSenha(ref caracter, 5);
      
      string senha = new String(caracter).Substring(0, 8);
      return senha;
   }
   static void embaralharSenha(ref char[] array, int vezes)
   {
      Random random = new Random(DateTime.Now.Millisecond);

      for (int i = 1; i <= vezes; i++)
      {
         for (int x = 1; x <= array.Length; x++)
         {
            trocarSenha(ref array[random.Next(0, array.Length)], ref array[random.Next(0, array.Length)]);
         }
      }

   }
   static void trocarSenha(ref char arg1, ref char arg2)
   {
      char strTemp = arg1;
      arg1 = arg2;
      arg2 = strTemp;
   }

   public bool validarEmail(string email)
   {
      bool validEmail = false;
      int indexArr = email.IndexOf('@');
      if (indexArr > -1)
      {
         int indexDot = email.IndexOf('.', indexArr);
         if (indexDot > -1 && email.Length - 1 > indexDot)
         {
            validEmail = true;
         }
      }
      return validEmail;
   }

   public string formataCPF(string sCPF)
   {
      string cpf = sCPF.TrimStart('0');
      if (cpf.Length == 11)
         cpf = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9);
      else if (cpf.Length == 14 || cpf.Length == 13)
      {
         cpf = cpf.PadLeft(14, '0');
         cpf = cpf.Substring(0, 2) + "." + cpf.Substring(2, 3) + "." + cpf.Substring(5, 3) + "/" + cpf.Substring(8, 4) + "-" + cpf.Substring(12, 2);
      }
      else if (cpf.Length < 11)
      {
         cpf = cpf.PadLeft(11, '0');
         cpf = cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9);
      }

      string retorno = cpf;

      return retorno;
   }

   public string formataFone(string sFone)
   {
      if (sFone.Trim().Length == 13)
      {
         sFone = sFone.Replace("-", "");
         sFone = sFone.Substring(0, 8) + "-" + sFone.Substring(8, 4);
      }
      return sFone;
   }

   public string I25Encode(string sBarra)
   {
      string retorno = "";
      string sStart = "NNNN";
      string sStop = "WNN";

      string[] objto = new string[10];
      objto[0] = "NNWWN"; // 00110 = 0
      objto[1] = "WNNNW"; // 10001 = 1
      objto[2] = "NWNNW"; // 01001 = 2
      objto[3] = "WWNNN"; // 11000 = 3
      objto[4] = "NNWNW"; // 00101 = 4
      objto[5] = "WNWNN"; // 10100 = 5
      objto[6] = "NWWNN"; // 01100 = 6
      objto[7] = "NNNWW"; // 00011 = 7
      objto[8] = "WNNWN"; // 10010 = 8
      objto[9] = "NWNWN"; // 01010 = 9

      int iLenBarra = sBarra.Length;  //44
      if ((sBarra.Trim().Length == 44) && ((iLenBarra %= 2) == 0))
      {
         string sEncode;
         string sUnit;
         string sDigit1;
         string sDigit2;
         string sObj1;
         string sObj2;
         int iObj1;
         int iObj2;
         int i;
         int g;

         sEncode = "";
         for (i = 0; i <= sBarra.Length - 1; i = i + 2)
         {
            sObj1 = sBarra.Substring(i, 1);
            sObj2 = sBarra.Substring(i + 1, 1);

            iObj1 = Convert.ToInt32(sObj1);
            iObj2 = Convert.ToInt32(sObj2);

            sDigit1 = objto[iObj1].ToString();
            sDigit2 = objto[iObj2].ToString();

            sUnit = "";

            for (g = 0; g <= 4; g++)
               sUnit += sDigit1.Substring(g, 1) + sDigit2.Substring(g, 1);

            sEncode += sUnit;
         }
         retorno += sStart + sEncode + sStop;
      }
      return retorno;
   }

   /// <summary>
   /// Converte uma imagem em um formato específico.
   /// </summary>
   /// <param name="imgStream">Stream da figura</param>
   /// <param name="filters">Aplica filtros de correção na imagem</param>
   /// <param name="imgFormat">Formato a ser convertido</param>
   /// <returns></returns>
   public Stream imageConvert(Stream imgStream, Color background, bool filters = true, ImageFormat imgFormat = null) {

      if (imgFormat == null) { imgFormat = ImageFormat.Jpeg; }

      using (System.Drawing.Image oldImage = System.Drawing.Image.FromStream(imgStream)) {

         using (Bitmap newImage = new Bitmap(oldImage.Width, oldImage.Height, PixelFormat.Format32bppArgb)) {
            using (Graphics canvas = Graphics.FromImage(newImage)) {

               canvas.Clear(background);

               if (filters) {
                  canvas.SmoothingMode = SmoothingMode.AntiAlias;
                  canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                  canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
               }

               canvas.DrawImage(oldImage, new Rectangle(0, 0, oldImage.Width, oldImage.Height));
               MemoryStream stream = new MemoryStream();
               newImage.Save(stream, imgFormat);
               return stream;
            }
         }
      }
   }

   /// <summary>
   /// Retorna o tamanho proporcional de uma imagem.
   /// </summary>
   /// <param name="cmdFotoASHX">Mesmo comando utilizado pela página foto.ashx<para>Valores necessárias: id, dbo, dbf, colunaFoto, chv</para></param>
   /// <param name="maxWidth">Largura máxima</param>
   /// <param name="maxHeight">Altura máxima</param>
   /// <returns></returns>
   public double[] imageGetSize(string cmdFotoASHX, double maxWidth = 0, double maxHeight = 0) {

      string sErro = "", id = "", dbo = "", dbf = "", colunaFoto = "", chv = "";
      byte[] bFoto;

      foreach (var str in cmdFotoASHX.Split('&')) {

         string _str = (str.Contains("?") ? str.Split('?')[1].Split('=')[0] : str.Split('=')[0]);
         string valor = (str.Contains("?") ? str.Split('?')[1].Split('=')[1] : str.Split('=')[1]);

         switch (_str) {
            case "id": id = valor; break;
            case "dbo": dbo = valor; break;
            case "dbf": dbf = valor; break;
            case "colunaFoto": colunaFoto = valor; break;
            case "chv": chv = valor; break;
         }
      }

      try {
         DataSet ds = new funSQL().BuscaSQLTaco("SELECT " + colunaFoto + " FROM " + dbo + "_" + dbf + " WHERE " + chv + "=" + id, ref sErro);
         bFoto = (byte[])ds.Tables[0].Rows[0][colunaFoto];
      }
      catch { return new double[2] { 0, 0 }; }

      return getImgSize(bFoto, maxWidth, maxHeight);
   }

   /// <summary>
   /// Retorna o tamanho proporcional de uma imagem.
   /// </summary>
   /// <param name="img">Stream de uma imagem em uma array de bytes</param>
   /// <param name="maxWidth">Largura máxima</param>
   /// <param name="maxHeight">Altura máxima</param>
   /// <returns></returns>
   public double[] getImgSize(byte[] img, double maxWidth = 0, double maxHeight = 0) {

      Bitmap imgLogo = null;
      double width, height;//, imgFactor = 0;

      try {
         MemoryStream stream = new MemoryStream();
         stream.Write(img, 0, img.Length);
         imgLogo = new Bitmap(stream);
      }
      catch { return new double[2] { 0, 0 }; }

      width = imgLogo.Width;
      height = imgLogo.Height;

      if (maxWidth.Equals(0) && maxHeight.Equals(0)) { return new double[] { width, height }; }

      double ratioX = (maxWidth.Equals(0) ? width : maxWidth) / width;
      double ratioY = (maxHeight.Equals(0) ? height : maxHeight) / height;
      double ratio = Math.Min(ratioX, ratioY);

      return new double[] { width * ratio, height * ratio };
   }
}
