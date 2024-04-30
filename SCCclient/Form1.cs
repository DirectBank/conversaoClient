using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Serialization;

namespace conversaoClient
{
   public partial class frmMain : Form
   {
      funDB funDB1 = new funDB();
      funSQL funSQL1 = new funSQL();
      protected Funcoes funcoes1 = new Funcoes();

      funDrive funDrive1 = new funDrive();

      // Dataset que contém os CLIENTES do AZURE
      DataSet dsClientesAzure = new DataSet();
      DataSet dsArquivosAzure = new DataSet();
      DataSet dsArquivosTACO = new DataSet();

      DataSet dsOcorrenciasTACO = new DataSet();
      DataSet dsOcorrenciasAzure = new DataSet();

      // DataSet para conversão de foto dos usuarios Workoffice
      DataSet dsUsuariosWorkoffice = new DataSet();

      private DateTime dDataInclusao, dDataParaEnviar, dDataHoje;

      //private string sVersao = "21.07.16.16:30"; //ano.mes.dia.hora
      private string sVersao = "24.03.21.16:00"; //ano.mes.dia.hora

      private BackgroundWorker bgw;
      private bool bIsCancel = false;
      private string sCodigoAdm = "", sCodigoCliente = "", sCliente = "";
      private string sEdificio = "", sBloco = "", sApto = "";
      private string id_empresa = "", id_cliente = "", id_usuario = "";
      private string sTotal, sTotalArquivos, sEnviados, sTotalDoc, sConvertidos,
                     sLblStatus_0, sLblStatus_1, sLblStatus_2, sLblStatus_3, sErroCatch, sDataInicio, sTipo = "", sId_azure = "", sId_conversaoWO = "";
      private string sTotalOcorrencias = "";

      private int _iBgwProgress;

      // Contador entre processamentos
      private TimeSpan tDelay = new TimeSpan(0, 0, 30);

      public frmMain()
      {
         InitializeComponent();

         Text = "Conversão Arquivos - Versão: " + sVersao;

         //bgw = new BackgroundWorker();
         //bgw.WorkerReportsProgress = true;
         //bgw.WorkerSupportsCancellation = true;
         //bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
         //bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
         //bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);

      }

      private void frmMain_Load(object sender, EventArgs e)
      {
         dtInicio.Text = "01/01/2020";
      }

      int iBgwProgress
      {
         get { _iBgwProgress++; return _iBgwProgress; }
         set { _iBgwProgress = value; }
      }

      private void radioButton1_CheckedChanged(object sender, EventArgs e)
      {

      }

      void clearFields()
      {
         listBox1.ResetText();
      }

      private void gbConversao_Enter(object sender, EventArgs e)
      {

      }

      private void rbOmeupredioTaco_CheckedChanged(object sender, EventArgs e)
      {

      }

      #region Botão Manual, Stop

      private void cmdStop_Click(object sender, EventArgs e)
      {
         clearFields();
         bIsCancel = true;
      }

      private void rbSCCanexos_CheckedChanged(object sender, EventArgs e)
      {

      }

      private void registraConversaoWO(String sId_empresa, string sId_usuario, String sUrlFotoUsuario)
      {
         try
         {
            listBox1.Items.Add("    - Registrando arquivo convertido WO.");

            if (funDB1.conectarTacoWO() == "OK")
            {
               try
               {

                  string sCmd = "";
                  //sCmd = "update WO_usuario SET imgFoto=null, imgFotoType=null, imgFotoSize=null, urlFotoUsuario='" + sUrlFotoUsuario + "' WHERE id_empresa=" + sId_empresa + " and id_usuario=" + sId_usuario;
                  sCmd = "update WO_usuario SET urlFotoUsuario='" + sUrlFotoUsuario + "' WHERE id_empresa=" + sId_empresa + " and id_usuario=" + sId_usuario;

                  SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTacoWO);
                  da.SelectCommand.CommandTimeout = 0;
                  da.Fill(dsArquivosAzure, "arquivoOK");
                  funDB1.fecharTacoWO();

               }
               catch (SqlException ex)
               {
                  listBox1.Items.Add("    - Erro enviando lista de arquivos WO");
               }
               finally
               {
                  funDB1.fecharTacoWO();
               }
            }
         }
         catch (SqlException ex)
         {
            listBox1.Items.Add("    - Erro ao regitsrar WO - id_usuario: " + sId_usuario);
         }

      }

      private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
      {

      }

      private void converteFotoLoginUsuario()
      {
         dsUsuariosWorkoffice.Clear();

         // LOCALIZA USUARIOS A SEREM CONVERTIDOS
         if (funDB1.conectarTacoWO() == "OK")
         {
            try
            {
               listBox1.Items.Add("**************** Iniciando processo ****************");
               listBox1.Items.Add("(TACO) - Consultando usuarios com fotos...");

               string sCmd = "";

               sCmd = "SELECT emp.id_empresa, emp.codigo as codigoEmpresa, emp.nome, usu.id_usuario, usu.nome as NomeUsuario, usu.login, urlFotoUsuario, imgFotoType, imgFotoSize, urlFotoUsuario " +
                      "FROM WO_usuario usu " +
                      "LEFT OUTER JOIN WO_empresa emp ON emp.id_empresa = usu.id_empresa " +
                      "WHERE urlFotoUsuario is null and imgFoto is not null  ";

               //emp.id_empresa = 10 and

               SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTacoWO);
               da.SelectCommand.CommandTimeout = 0;
               da.Fill(dsUsuariosWorkoffice, "usuarios");
               funDB1.fecharTacoWO();
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("Erro no procuraUsuariosWoTaco()");
            }
            finally
            {
               funDB1.fecharTacoWO();
            }
         }

         // BUSCA E CONVERTE IMAGEM DO USUARIO
         if (dsUsuariosWorkoffice.Tables["usuarios"].Rows.Count > 0)
         {
            listBox1.Items.Add("Foram localizado(s) " + dsUsuariosWorkoffice.Tables["usuarios"].Rows.Count.ToString() + " usuarios.");
            listBox1.Items.Add("");

            int iUsuarios = dsUsuariosWorkoffice.Tables["usuarios"].Rows.Count;
            sTotal = iUsuarios.ToString();

            listBox1.Items.Add("(TACO) - Consultando imagens...");

            // PROCURA IMAGEM POR USUARIO
            for (int i = 0; i < iUsuarios; i++)
            {

               // ID_empresa e ID_cliente (Azure)
               this.id_empresa = dsUsuariosWorkoffice.Tables["usuarios"].Rows[i]["id_empresa"].ToString();
               this.id_usuario = dsUsuariosWorkoffice.Tables["usuarios"].Rows[i]["id_usuario"].ToString();
               this.sCodigoCliente = dsUsuariosWorkoffice.Tables["usuarios"].Rows[i]["codigoEmpresa"].ToString().PadLeft(8, Convert.ToChar("0"));

               String sUsuario = "Usuário: " + dsUsuariosWorkoffice.Tables["usuarios"].Rows[i]["codigoEmpresa"].ToString() + "-" + dsUsuariosWorkoffice.Tables["usuarios"].Rows[i]["NomeUsuario"].ToString();

               try
               {
                  listBox1.Items.Add("" + sUsuario);

                  dsArquivosTACO.Clear();
                  if (funDB1.conectarTacoWO() == "OK")
                  {
                     try
                     {
                        string sCmd = "";
                        sCmd = "SELECT imgFoto as arquivo, imgFotoType, imgFotoSize, urlFotoUsuario FROM WO_usuario WHERE id_usuario=" + this.id_usuario + " AND id_empresa=" + this.id_empresa;

                        SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTacoWO);
                        da.SelectCommand.CommandTimeout = 0;
                        da.Fill(dsArquivosTACO, "imagem");
                        funDB1.fecharTacoWO();
                     }
                     catch (SqlException ex)
                     {
                        listBox1.Items.Add("    - Erro no procuraArquivosTACO()");
                     }
                     finally
                     {
                        funDB1.fecharTacoWO();
                     }
                  }
               }
               catch (SqlException ex)
               {
                  listBox1.Items.Add("    - Erro no procuraArquivosTACO()");
               }
               finally
               {
                  funDB1.fecharTacoWO();
               }

               // ENVIA IMAGEMS
               if (dsArquivosTACO != null && dsArquivosTACO.Tables.Count > 0)
               {
                  if (dsArquivosTACO.Tables["imagem"].Rows.Count > 0)
                  {
                     listBox1.Items.Add("       - (TACO) - Localizado arquivo.");

                     string sNomeArquivoEditado = "WO/" + this.id_empresa + "/WO_USUARIO/" + this.id_usuario + ".jpg";

                     byte[] arquivoM = new byte[0];
                     DataRow linhaM = dsArquivosTACO.Tables[0].Rows[0];
                     arquivoM = (byte[])linhaM["arquivo"];

                     // Enviar o Bytes para o FIREBASE.
                     // Sobe para o Firebase o documento
                     if (Convert.ToInt32(this.id_usuario) > 0 && sNomeArquivoEditado != "")
                     {
                        listBox1.Items.Add("       - (Firebase) Enviando arquivo...");

                        // Faz a rolagem da lista
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;

                        if (!funDrive1.uploadToDriveWO(this.id_empresa, "", "WO_usuario", this.id_usuario, arquivoM, sNomeArquivoEditado))
                        {
                           listBox1.Items.Add("          - Já realizado envio anteriormente");
                        }
                        else
                        {
                           listBox1.Items.Add("          - Enviado!");
                        }

                        registraConversaoWO(this.id_empresa, this.id_usuario, sNomeArquivoEditado);

                        // Faz a rolagem da lista
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;

                        // Pausa de 2 segundos, evitar TimeOut.
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

                     }
                     else
                     {
                        listBox1.Items.Add("    - Não existe imagem para esse usuario.");
                     }

                     listBox1.Items.Add("");

                     // Faz a lista correr.
                     listBox1.SelectedIndex = listBox1.Items.Count - 1;
                  }
               }
            }
         }
      }

      private void label3_Click(object sender, EventArgs e)
      {

      }

      private void cmdManual_Click(object sender, EventArgs e)
      {
         bIsCancel = false;
         clearFields();

         if (rbWorkOffice.Checked)  // Conversão FOTO DO LOGIN (Bytes/Firebase) 
         {
            converteFotoLoginUsuario();
            MessageBox.Show("Processo concluído!", "Conversão");
         }
         else if (rbOmeupredioLocal.Checked) // Download TACO arquivos, para pasta.
         {
            this.dtInicio.Text = dtInicio.Value.ToShortDateString();
            this.sCodigoAdm = this.txtCodigoAdm.Text.Trim();
            this.sCliente = this.txtCodigoCliente.Text.Trim();

            DateTime localDate = DateTime.Now;

            if (!this.sCodigoAdm.Trim().Equals("") && !this.sCliente.Trim().Equals(""))
            {
               this.sCodigoAdm = this.sCodigoAdm.PadLeft(8, Convert.ToChar("0"));
               //procuraClientesAzure();
               buscaIdsArquivosTaco();
               MessageBox.Show("Download finalizado", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
               MessageBox.Show("Informe o código da adm e do condomínio.", "Conversão");
            }
         }
         else if (rbLocacaoTaco.Checked) // Conversao WO SCL - TACO/AZURE
         {
            this.dtInicio.Text = dtInicio.Value.ToShortDateString();
            this.sCodigoAdm = this.txtCodigoAdm.Text.Trim();

            DateTime localDate = DateTime.Now;

            if (!this.sCodigoAdm.Trim().Equals(""))
            {
               this.sCodigoAdm = this.sCodigoAdm.PadLeft(8, Convert.ToChar("0"));
               procuraClientesLocacaoAzure();
            }
            else
            {
               MessageBox.Show("Informe o código da administradora.", "Conversão");
            }
         }
         else  // Conversão Omeupredio - TACO/AZURE
         {
            this.dtInicio.Text = dtInicio.Value.ToShortDateString();
            this.sCodigoAdm = this.txtCodigoAdm.Text.Trim();

            DateTime localDate = DateTime.Now;

            if (!this.sCodigoAdm.Trim().Equals(""))
            {
               this.sCodigoAdm = this.sCodigoAdm.PadLeft(8, Convert.ToChar("0"));
               procuraClientesAzure();
            }
            else
            {
               MessageBox.Show("Informe o código da administradora.", "Conversão");
            }
         }
      }
      #endregion



      // Localiza clientes cadastrados no SCL ONLINE
      private void procuraClientesLocacaoAzure()
      {
         dsClientesAzure.Clear();

         if (funDB1.conectarAzure() == "OK")
         {
            try
            {
               listBox1.Items.Add("**************** Iniciando processo ****************");
               listBox1.Items.Add("(AZURE) - Consultando clientes  de locação cadastrados...");

               string sCmd = "";
               sCmd = "EXEC SCLSP_arquivo @codigoAdm='" + (this.sCodigoAdm.Equals("00001777") ? "00000004" : this.sCodigoAdm) + "', " +
                      "@modo=40";
               SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conAzure);
               da.SelectCommand.CommandTimeout = 0;
               da.Fill(dsClientesAzure, "clientes");
               funDB1.fecharAzure();
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("Erro no procuraClientesAzure()");
            }
            finally
            {
               funDB1.fecharAzure();
            }
         }

         if (dsClientesAzure.Tables["clientes"].Rows.Count > 0)
         {
            listBox1.Items.Add("Foram localizado(s) " + dsClientesAzure.Tables["clientes"].Rows.Count.ToString() + " clientes.");
            listBox1.Items.Add("");

            procuraArquivosLocacaoTACO(dsClientesAzure);

            listBox1.Items.Add("**************** Procedimento realizado! ****************");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            MessageBox.Show("Procedimento realizado!", "Conversão");
         }
         else
         {
            MessageBox.Show("Não existe clientes no SCL online para conversão (Azure).", "Conversão");
         }
      }
      //------------------------------------------------------------------------------------------------------------------//
      #region CONVERSÃO LOCACAO - ARQUIVOS BD TACO/BD AZURE

      private void procuraArquivosLocacaoTACO(DataSet dsCliente)
      {
         int iClientes = dsCliente.Tables["clientes"].Rows.Count;
         sTotal = iClientes.ToString();

         listBox1.Items.Add("(TACO) - Consultando registros de locação...");

         // Procura registros por Cliente
         for (int i = 0; i < iClientes; i++)
         {
            if (bIsCancel) { return; }

            // ID_empresa e ID_cliente (Azure)
            this.id_empresa = dsCliente.Tables["clientes"].Rows[i]["id_empresa"].ToString();
            this.sCodigoCliente = dsCliente.Tables["clientes"].Rows[i]["codigo"].ToString().PadLeft(8, Convert.ToChar("0"));
            this.sTipo = dsCliente.Tables["clientes"].Rows[i]["tipo"].ToString();
            this.sId_azure = dsCliente.Tables["clientes"].Rows[i]["id"].ToString();
            this.sId_conversaoWO = dsCliente.Tables["clientes"].Rows[i]["id_conversaoWO"].ToString();

            String sCliente = "Cliente: " + dsCliente.Tables["clientes"].Rows[i]["codigo"].ToString() + "-" + dsCliente.Tables["clientes"].Rows[i]["nome"].ToString();

            #region CONVERSÃO DOCUMENTOS DE LOCAÇÃO (TACO/AZURE)
            try
            {
               listBox1.Items.Add("" + sCliente);

               dsArquivosTACO.Clear();
               if (funDB1.conectarTacoWO() == "OK")
               {
                  try
                  {
                     string sCmd = "";
                     sCmd = "EXEC SCLSP_documento @codigoAdm='" + (this.sCodigoAdm.Equals("00001777") ? "00000004" : this.sCodigoAdm) + "', " +
                            "@tipo='" + this.sTipo.ToString() + "', " +
                            "@id=" + this.sId_conversaoWO.ToString() + ", " +
                            "@data='" + this.dtInicio.Text + "', " +
                            "@modo=40";

                     SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTacoWO);
                     da.SelectCommand.CommandTimeout = 0;
                     da.Fill(dsArquivosTACO, "arquivos");
                     funDB1.fecharTacoWO();
                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro no procuraArquivosLocacaoTACO()");
                  }
                  finally
                  {
                     funDB1.fecharTacoWO();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro no procuraArquivosLocacaoTACO()");
            }
            finally
            {
               funDB1.fecharTacoWO();
            }

            if (dsArquivosTACO.Tables["arquivos"].Rows.Count > 0)
            {
               // Insere lista arquivos TACO em AZURE
               // Retorna lista do SCL_arquivo inseridas (id_arquivo e id_conversaoTaco).
               //DataSet dsRetornoAzure = insereArquivosLocacaoAZURE(dsArquivosTACO);

               DataSet dsRetornoAzure = new DataSet();
               dsRetornoAzure.Clear();
               dsRetornoAzure = insereArquivosLocacaoAZURE(dsArquivosTACO);

               if (dsRetornoAzure.Tables["arquivos"].Rows.Count <= 0)
               {
                  listBox1.Items.Add("    - Não existem registros a serem enviados.");
               }
               else
               {
                  registraLocacaoConversao(dsRetornoAzure);
               }
            }
            else
            {
               listBox1.Items.Add("    - Não existem registros para esse cliente.");
            }
            #endregion

            listBox1.Items.Add("");

            // Faz a lista correr.
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
         }
      }

      private DataSet insereArquivosLocacaoAZURE(DataSet dsArquivosTACO)
      {
         dsArquivosAzure.Clear();

         listBox1.Items.Add("    - Foram encontrado(s) " + dsArquivosTACO.Tables["arquivos"].Rows.Count + " arquivos.");
         listBox1.Items.Add("    - Inserindo lista e retornando referência (AZURE)...");

         int iArquivos = dsArquivosTACO.Tables["arquivos"].Rows.Count;
         sTotalArquivos = iArquivos.ToString();
         string sXml = "";

         if (iArquivos > 0)
         {
            // Monta o XML com a Lista de ID_arquivo (TACO) a ser enviada.
            listBox1.Items.Add("    - Montando XML lista TACO/AZURE...");
            sXml = montaXMLArquivosLocacaoTACO(dsArquivosTACO);

            // Envia ID_arquivoTACO (TACO) para SCC_arquivo (AZURE) .
            // Retorna ID_arquivo oficial (AZURE)
            try
            {
               listBox1.Items.Add("    - Enviando lista de arquivos TACO/AZURE (insereArquivosAZURE)");

               if (funDB1.conectarAzure() == "OK")
               {
                  try
                  {
                     string sCmd = "";
                     sCmd = "EXEC SCLSP_arquivo @id_empresa=" + this.id_empresa + ", " +
                            //"@id_cliente=" + this.id_cliente + ", " +
                            "@tipo='" + this.sTipo.ToString() + "', " +
                            "@id=" + this.sId_azure.ToString() + ", " +
                            "@strXml='" + sXml + "', " +
                            "@modo=41";

                     SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conAzure);
                     da.SelectCommand.CommandTimeout = 0;
                     da.Fill(dsArquivosAzure, "arquivos");
                     funDB1.fecharAzure();
                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro enviando lista de arquivos TACO/AZURE (insereArquivosAZURE)");
                  }
                  finally
                  {
                     funDB1.fecharAzure();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro enviando lista de arquivos TACO/AZURE (insereArquivosAZURE)");
            }
            finally
            {
               funDB1.fecharAzure();
            }

         }
         return dsArquivosAzure;
      }

      private string montaXMLArquivosLocacaoTACO(DataSet ds)
      {
         int iArquivos = ds.Tables["arquivos"].Rows.Count;

         string sXml = "";

         sXml = "<arquivo>";
         sXml += "<cliente>";
         sXml += "<id_empresa>" + this.id_empresa + "</id_empresa>";
         sXml += "<tipo>" + this.sTipo + "</tipo>";
         sXml += "<id>" + this.sId_azure + "</id>";

         for (int i = 0; i < iArquivos; i++)
         {
            string sId_documentosTaco = "", sId_tipoDocumento = "", sIdClienteTaco = "", sCodigoCliente = "",
                   sNomeArquivo = "", sDescricao = "", sDataCadastro = "", sDtDocumento = "",
                   sTipoDocumento = "", sUrlArquivo = "", sImagemType = "", sImagemSize = "", sExtensao = "";


            sId_documentosTaco = ds.Tables["arquivos"].Rows[i]["id_documentos"].ToString();
            sId_tipoDocumento = ds.Tables["arquivos"].Rows[i]["id_tipoDocumento"].ToString();
            sIdClienteTaco = ds.Tables["arquivos"].Rows[i]["id_cliente"].ToString();
            sCodigoCliente = ds.Tables["arquivos"].Rows[i]["codigoCliente"].ToString();
            sNomeArquivo = ds.Tables["arquivos"].Rows[i]["nomeArquivo"].ToString();
            sDescricao = funcoes1.LimpaLinha(ds.Tables["arquivos"].Rows[i]["descricao"].ToString());
            sDataCadastro = ds.Tables["arquivos"].Rows[i]["dataCadastro"].ToString();
            sDtDocumento = ds.Tables["arquivos"].Rows[i]["dtDocumento"].ToString();
            sTipoDocumento = funcoes1.LimpaLinha(ds.Tables["arquivos"].Rows[i]["tipoDocumento"].ToString());
            sUrlArquivo = funcoes1.LimpaLinha(ds.Tables["arquivos"].Rows[i]["urlArquivo"].ToString());
            sImagemType = ds.Tables["arquivos"].Rows[i]["imagemType"].ToString();
            sImagemSize = ds.Tables["arquivos"].Rows[i]["imagemSize"].ToString();

            //// Retira a extensão do nome do arquivo
            int iPosicaoExtensao = 0;
            int iTam = sNomeArquivo.ToString().Length;
            for (int t = 0; t < iTam; t++)
            {
               string sCaractere = sNomeArquivo.Substring(t, 1);
               if (sCaractere.Equals("."))
               {
                  iPosicaoExtensao = t;
               }
            }

            sExtensao = sNomeArquivo.Substring(iPosicaoExtensao);

            if (sExtensao.Equals(""))
            {
               //// Retira a extensão do nome do arquivo
               iPosicaoExtensao = 0;
               iTam = sUrlArquivo.ToString().Length;
               for (int t = 0; t < iTam; t++)
               {
                  string sCaractere = sUrlArquivo.Substring(t, 1);
                  if (sCaractere.Equals("."))
                  {
                     iPosicaoExtensao = t;
                  }
               }

               sExtensao = sUrlArquivo.Substring(iPosicaoExtensao);
            }

            ///////////////////////

            sXml += "<registro>";

            sXml += "<id_documentos>" + sId_documentosTaco + "</id_documentos>";
            sXml += "<id_tipoDocumentos>" + sId_tipoDocumento + "</id_tipoDocumentos>";
            sXml += "<id_cliente>" + sIdClienteTaco + "</id_cliente>";
            sXml += "<codigoCliente>" + sCodigoCliente + "</codigoCliente>";
            sXml += "<nomeArquivo>" + sNomeArquivo + "</nomeArquivo>";
            sXml += "<descricao>" + sDescricao + "</descricao>";
            sXml += "<dataCadastro>" + sDataCadastro + "</dataCadastro>";
            sXml += "<dtDocumento>" + sDtDocumento + "</dtDocumento>";
            sXml += "<tipoDocumento>" + sTipoDocumento + "</tipoDocumento>";
            sXml += "<urlArquivo>" + sUrlArquivo + "</urlArquivo>";
            sXml += "<imagemType>" + sImagemType + "</imagemType>";
            sXml += "<imagemSize>" + sImagemSize + "</imagemSize>";
            sXml += "<extensao>" + sExtensao + "</extensao>";

            sXml += "</registro>";

         }
         sXml += "</cliente>";
         sXml += "</arquivo>";


         return sXml;
      }

      private void registraLocacaoConversao(DataSet ds)
      {

         int iArquivos = ds.Tables["arquivos"].Rows.Count;
         string sXml = "";

         sXml = "<arquivo>";
         sXml += "<cliente>";

         for (int i = 0; i < iArquivos; i++)
         {
            string sId_documentosTaco = ds.Tables["arquivos"].Rows[i]["id_conversaoTaco"].ToString(); ;
            string sId_arquivoAzure = ds.Tables["arquivos"].Rows[i]["id_arquivo"].ToString(); ;

            sXml += "<registro>";
            sXml += "<id_documentos>" + sId_documentosTaco + "</id_documentos>"; // TACO
            sXml += "<id_arquivo>" + sId_arquivoAzure + "</id_arquivo>"; // AZURE
            sXml += "</registro>";
         }

         sXml += "</cliente>";
         sXml += "</arquivo>";

         try
         {
            listBox1.Items.Add("    - Registrando arquivo convertido AZURE.");

            if (funDB1.conectarTacoWO() == "OK")
            {
               try
               {

                  string sCmd = "";
                  sCmd = "EXEC SCLSP_documento @codigoAdm='" + (this.sCodigoAdm.Equals("00001777") ? "00000004" : this.sCodigoAdm) + "', " +
                         "@strXml='" + sXml + "', " +
                         "@modo=41";

                  SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTacoWO);
                  da.SelectCommand.CommandTimeout = 0;
                  da.Fill(dsArquivosAzure, "arquivoOK");
                  funDB1.fecharTacoWO();

               }
               catch (SqlException ex)
               {
                  listBox1.Items.Add("    - Erro enviando lista de arquivos TACO/AZURE (insereArquivosAZURE)");
               }
               finally
               {
                  funDB1.fecharTacoWO();
               }
            }
         }
         catch (SqlException ex)
         {
            listBox1.Items.Add("    - Erro ao regitsrar TACO - id_documentos");
         }
      }

      #endregion
      //------------------------------------------------------------------------------------------------------------------//


      // Localiza clientes cadastrados no SCC ONLINE
      private void procuraClientesAzure()
      {
         dsClientesAzure.Clear();

         if (funDB1.conectarAzure() == "OK")
         {
            try
            {
               listBox1.Items.Add("**************** Iniciando processo ****************");
               listBox1.Items.Add("(AZURE) - Consultando clientes cadastrados...");

               string sCmd = "";
               sCmd = "SELECT emp.id_empresa, cli.id_cliente, cli.codigo, cli.nome " +
                      "FROM WO_cliente cli " +
                      "LEFT OUTER JOIN WO_empresa emp ON cli.id_empresa = emp.id_empresa " +
                      "WHERE emp.codigo ='" + this.sCodigoAdm + "' and crm_tipo = 1 ORDER BY cli.codigo ";

               //sCmd = "SELECT emp.id_empresa, cli.id_cliente, cli.codigo, cli.nome " +
               //       "FROM WO_cliente cli " +
               //       "LEFT OUTER JOIN WO_empresa emp ON cli.id_empresa = emp.id_empresa " +
               //       "WHERE emp.codigo ='" + this.sCodigoAdm + "' and crm_tipo = 1 and cli.codigo='00000069' ORDER BY cli.codigo ";

               SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conAzure);
               da.SelectCommand.CommandTimeout = 0;
               da.Fill(dsClientesAzure, "clientes");
               funDB1.fecharAzure();
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("Erro no procuraClientesAzure()");
            }
            finally
            {
               funDB1.fecharAzure();
            }
         }

         if (dsClientesAzure.Tables["clientes"].Rows.Count > 0)
         {
            listBox1.Items.Add("Foram localizado(s) " + dsClientesAzure.Tables["clientes"].Rows.Count.ToString() + " clientes.");
            listBox1.Items.Add("");

            // Conversão de arquivos Omeupredio TACO para AZURE
            if (rbOmeupredioTaco.Checked)
            {
               procuraArquivosTACO(dsClientesAzure);

               listBox1.Items.Add("**************** Procedimento realizado! ****************");
               listBox1.SelectedIndex = listBox1.Items.Count - 1;
               MessageBox.Show("Procedimento realizado!", "Conversão");
            }
            else
            {
               //// Conversão de arquivos locais \PRG\SCC\ANEXOS\ para AZURE
               procuraArquivosLOCAL(dsClientesAzure);

               listBox1.Items.Add("**************** Procedimento realizado! ****************");
               listBox1.SelectedIndex = listBox1.Items.Count - 1;
               MessageBox.Show("Procedimento realizado!", "Conversão");
            }
         }
         else
         {
            MessageBox.Show("Não existe clientes no SCC online / Unidadez para conversão (Azure).", "Conversão");
         }
      }

      //------------------------------------------------------------------------------------------------------------------//
      #region CONVERSÃO OMEUPREDIO - ARQUIVOS  BD TACO/BD AZURE

      private void procuraArquivosTACO(DataSet dsCliente)
      {
         int iClientes = dsCliente.Tables["clientes"].Rows.Count;
         sTotal = iClientes.ToString();

         listBox1.Items.Add("(TACO) - Consultando registros...");

         // Procura registros por Cliente
         for (int i = 0; i < iClientes; i++)
         {
            if (bIsCancel) { return; }

            // ID_empresa e ID_cliente (Azure)
            this.id_empresa = dsCliente.Tables["clientes"].Rows[i]["id_empresa"].ToString();
            this.id_cliente = dsCliente.Tables["clientes"].Rows[i]["id_cliente"].ToString();
            this.sCodigoCliente = dsCliente.Tables["clientes"].Rows[i]["codigo"].ToString().PadLeft(8, Convert.ToChar("0"));

            String sCliente = "Cliente: " + dsCliente.Tables["clientes"].Rows[i]["codigo"].ToString() + "-" + dsCliente.Tables["clientes"].Rows[i]["nome"].ToString();

            #region CONVERSÃO DOCUMENTOS  (TACO/AZURE)
            try
            {
               listBox1.Items.Add("" + sCliente);

               dsArquivosTACO.Clear();
               if (funDB1.conectarTaco() == "OK")
               {
                  try
                  {
                     string sCmd = "";
                     sCmd = "EXEC OMPSP_documento @codigoAdm='" + (this.sCodigoAdm.Equals("00001777") ? "00000004" : this.sCodigoAdm) + "', " +
                            "@codigoCliente='" + this.sCodigoCliente.Substring(4, 4) + "', " +
                            "@data1='" + this.dtInicio.Text + "', " +
                            "@modo=50";

                     SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTaco);
                     da.SelectCommand.CommandTimeout = 0;
                     da.Fill(dsArquivosTACO, "arquivos");
                     funDB1.fecharTaco();
                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro no procuraArquivosTACO()");
                  }
                  finally
                  {
                     funDB1.fecharTaco();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro no procuraArquivosTACO()");
            }
            finally
            {
               funDB1.fecharTaco();
            }

            if (dsArquivosTACO.Tables["arquivos"].Rows.Count > 0)
            {
               // Insere lista arquivos TACO em AZURE
               // Retorna lista do SCC_arquivo inseridas (id_arquivo e id_arquivoTaco).
               DataSet dsRetornoAzure = insereArquivosAZURE(dsArquivosTACO);
               if (dsRetornoAzure.Tables["arquivos"].Rows.Count > 0)
               {
                  // Percorre lista de arquivo AZURE e localiza arquivo bytes TACO
                  // Envia para o Firebase
                  procuraArquivoBinarioTaco(dsRetornoAzure);
               }
               else
               {
                  listBox1.Items.Add("    - Não existem registros a serem enviados.");
               }
            }
            else
            {
               listBox1.Items.Add("    - Não existem registros para esse cliente.");
            }
            #endregion

            #region CONVERSÃO OCORRÊNCIAS  (TACO/AZURE)
            try
            {
               listBox1.Items.Add("" + sCliente);

               dsOcorrenciasTACO.Clear();
               if (funDB1.conectarTaco() == "OK")
               {
                  try
                  {
                     string sCmd = "";
                     sCmd = "EXEC OMPSP_ocorrencia @codigoAdm='" + (this.sCodigoAdm.Equals("00001777") ? "00000004" : this.sCodigoAdm) + "', " +
                            "@codigoCliente='" + this.sCodigoCliente.Substring(4, 4) + "', " +
                            "@data1='" + this.dtInicio.Text + "', " +
                            "@isRel=1, " +
                            "@modo=12";

                     SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTaco);
                     da.SelectCommand.CommandTimeout = 0;
                     da.Fill(dsOcorrenciasTACO, "ocorrencias");
                     funDB1.fecharTaco();
                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro no procuraOcorrenciasTACO()");
                  }
                  finally
                  {
                     funDB1.fecharTaco();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro no procuraOcorrenciasTACO()");
            }
            finally
            {
               funDB1.fecharTaco();
            }

            if (dsOcorrenciasTACO.Tables.Count > 0)
            {
               if (dsOcorrenciasTACO.Tables["ocorrencias"].Rows.Count > 0)
               {
                  // Insere lista ocorrencias TACO em AZURE
                  // Retorna lista do OM_ocorrenciaArquivo inseridas (id_ocorrenciasArquivo e id_ocorrenciaArquivoTaco).
                  DataSet dsRetornoAzure = insereOcorrenciasAZURE(dsOcorrenciasTACO);
                  if (dsRetornoAzure.Tables.Count > 0)
                  {
                     if (dsRetornoAzure.Tables["ocorrencias"].Rows.Count > 0)
                     {
                        // Percorre lista de ocorrencias AZURE e localiza ocorrencia arquivo bytes TACO
                        // Envia para o Firebase
                        procuraOcorrenciasBinarioTaco(dsRetornoAzure);
                     }
                     else
                     {
                        listBox1.Items.Add("    - Não existem registros de ocorrências a serem enviados.");
                     }
                  }
               }
               else
               {
                  listBox1.Items.Add("    - Não existem registros de ocorrências para esse cliente.");
               }
            }
            #endregion

            listBox1.Items.Add("");

            // Faz a lista correr.
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
         }
      }

      private DataSet insereOcorrenciasAZURE(DataSet dsOcorrenciasTACO)
      {
         dsOcorrenciasAzure.Clear();

         listBox1.Items.Add("    - Foram encontrado(s) " + dsOcorrenciasTACO.Tables["ocorrencias"].Rows.Count + " ocorrências.");
         listBox1.Items.Add("    - Inserindo lista e retornando referência (AZURE)...");

         int iOcorrencias = dsOcorrenciasTACO.Tables["ocorrencias"].Rows.Count;
         sTotalOcorrencias = iOcorrencias.ToString();
         string sXml = "";

         if (iOcorrencias > 0)
         {
            // Monta o XML com a Lista de ID_arquivo (TACO) a ser enviada.
            listBox1.Items.Add("    - Montando XML lista TACO/AZURE...");
            sXml = montaXMLOcorrenciasTACO(dsOcorrenciasTACO);

            // Envia ID_ocorrenciaTACO (TACO) para OM_ocorrenciaArquivo (AZURE) .
            // Retorna ID_ocorrenciaArquivo oficial (AZURE)
            try
            {
               listBox1.Items.Add("    - Enviando lista de ocorrências TACO/AZURE (insereOcorrenciasAZURE)");

               if (funDB1.conectarAzure() == "OK")
               {
                  try
                  {
                     string sCmd = "";
                     // Voltar Aqui....
                     sCmd = "EXEC OMPSP_ocorrencia @id_empresa=" + this.id_empresa + ", " +
                            "@id_cliente=" + this.id_cliente + ", " +
                            "@strXml='" + sXml + "', " +
                            "@modo=30";

                     SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conAzure);
                     da.SelectCommand.CommandTimeout = 0;
                     da.Fill(dsOcorrenciasAzure, "ocorrencias");
                     funDB1.fecharAzure();
                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro 1 enviando lista de ocorrências TACO/AZURE (insereOcorrenciasAZURE)");
                  }
                  finally
                  {
                     funDB1.fecharAzure();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro 2 enviando lista de ocorrências TACO/AZURE (insereOcorrenciasAZURE)");
            }
            finally
            {
               funDB1.fecharAzure();
            }
         }

         return dsOcorrenciasAzure;
      }
      private string montaXMLOcorrenciasTACO(DataSet ds)
      {
         int iOcorrencias = ds.Tables["ocorrencias"].Rows.Count;

         string sXml = "";

         sXml = "<arquivo>";
         sXml += "<cliente>";
         sXml += "<id_empresa>" + this.id_empresa + "</id_empresa>";
         sXml += "<id_cliente>" + this.id_cliente + "</id_cliente>";

         for (int i = 0; i < iOcorrencias; i++)
         {
            string sId_ocorrenciaTaco = "", sId_usuario = "", sBloco = "", sApto = "", sTipo = "", sDescricaoTipo = "", sData = "", sHora = "",
                   sOcorrencia = "", sNome = "", sExtensao = "", sTamanho = "",
                   sId_usuarioCitado = "", sDataResposta = "", sHoraResposta = "", sRespondido = "", sResposta = "", sId_usuarioResposta = "",
                   sBlocoCitada = "", sAptoCitada = "", sTipoCitada = "", sBaixarImagem = "", sDataCadastro = "", sHoraCadastro = "",
                   sId_ocorrenciaArquivoTaco = "", sImagemTipo = "", sImagemSize = "", sId_conversao = "",
                   sBlocoResposta = "", sAptoResposta = "";

            sId_ocorrenciaTaco = ds.Tables["ocorrencias"].Rows[i]["id_ocorrencia"].ToString();
            sId_usuario = ds.Tables["ocorrencias"].Rows[i]["id_usuario"].ToString();
            sBloco = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["bloco"].ToString());
            sApto = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["apto"].ToString());
            sTipo = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["tipo"].ToString());
            sData = ds.Tables["ocorrencias"].Rows[i]["data"].ToString();
            sHora = ds.Tables["ocorrencias"].Rows[i]["hora"].ToString();
            sId_usuarioCitado = ds.Tables["ocorrencias"].Rows[i]["id_usuarioCitado"].ToString();
            sOcorrencia = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["ocorrencia"].ToString());
            sDataResposta = ds.Tables["ocorrencias"].Rows[i]["dataResposta"].ToString();
            sHoraResposta = ds.Tables["ocorrencias"].Rows[i]["horaResposta"].ToString();
            sRespondido = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["respondido"].ToString());
            sResposta = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["resposta"].ToString());
            sId_usuarioResposta = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["id_usuarioResposta"].ToString());

            sBlocoCitada = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["blocoCitada"].ToString());
            sAptoCitada = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["aptoCitada"].ToString());
            sTipoCitada = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["tipoCitada"].ToString());
            sBlocoResposta = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["blocoResposta"].ToString());
            sAptoResposta = funcoes1.LimpaLinha(ds.Tables["ocorrencias"].Rows[i]["aptoResposta"].ToString());
            sBaixarImagem = ds.Tables["ocorrencias"].Rows[i]["baixarImagem"].ToString();
            sDataCadastro = ds.Tables["ocorrencias"].Rows[i]["dataCadastro"].ToString();
            sHoraCadastro = ds.Tables["ocorrencias"].Rows[i]["horaCadastro"].ToString();
            sId_ocorrenciaArquivoTaco = ds.Tables["ocorrencias"].Rows[i]["id_ocorrenciaArquivo"].ToString();
            sImagemTipo = ds.Tables["ocorrencias"].Rows[i]["imagemTipo"].ToString();
            sImagemSize = ds.Tables["ocorrencias"].Rows[i]["imagemSize"].ToString();
            sId_conversao = ds.Tables["ocorrencias"].Rows[i]["id_conversao"].ToString();

            sXml += "<registro>";
            sXml += "<id_ocorrenciaTaco>" + sId_ocorrenciaTaco + "</id_ocorrenciaTaco>";
            sXml += "<id_usuario>" + sId_usuario + "</id_usuario>";
            sXml += "<bloco>" + sBloco + "</bloco>";
            sXml += "<apto>" + sApto + "</apto>";
            sXml += "<tipo>" + sTipo + "</tipo>";
            sXml += "<data>" + sData + "</data>";
            sXml += "<hora>" + sHora + "</hora>";
            sXml += "<id_usuarioCitado>" + sId_usuarioCitado + "</id_usuarioCitado>";
            sXml += "<ocorrencia>" + sOcorrencia + "</ocorrencia>";
            sXml += "<dataResposta>" + sDataResposta + "</dataResposta>";
            sXml += "<respondido>" + sRespondido + "</respondido>";
            sXml += "<resposta>" + sResposta + "</resposta>";
            sXml += "<id_usuarioResposta>" + sId_usuarioResposta + "</id_usuarioResposta>";
            sXml += "<blocoCitada>" + sBlocoCitada + "</blocoCitada>";
            sXml += "<aptoCitada>" + sAptoCitada + "</aptoCitada>";
            sXml += "<tipoCitada>" + sTipoCitada + "</tipoCitada>";

            sXml += "<blocoResposta>" + sBlocoResposta + "</blocoResposta>";
            sXml += "<aptoResposta>" + sAptoResposta + "</aptoResposta>";

            sXml += "<baixarImagem>" + sBaixarImagem + "</baixarImagem>";
            sXml += "<dataCadastro>" + sDataCadastro + "</dataCadastro>";
            sXml += "<horaCadastro>" + sHoraCadastro + "</horaCadastro>";
            sXml += "<id_ocorrenciaArquivoTaco>" + sId_ocorrenciaArquivoTaco + "</id_ocorrenciaArquivoTaco>";
            sXml += "<imagemTipo>" + sImagemTipo + "</imagemTipo>";
            sXml += "<imagemSize>" + sImagemSize + "</imagemSize>";
            sXml += "<id_conversao>" + sId_conversao + "</id_conversao>";
            sXml += "</registro>";

         }
         sXml += "</cliente>";
         sXml += "</arquivo>";

         return sXml;
      }

      private void procuraOcorrenciasBinarioTaco(DataSet dsRetornoOcorrenciasAzure)
      {
         listBox1.Items.Add("    - Procura arquivo binário (TACO).");

         string sNomeArquivoEditado = "";
         int iOcorrencias = dsRetornoOcorrenciasAzure.Tables["ocorrencias"].Rows.Count;

         // Procura arquivos por Cliente
         for (int a = 0; a < iOcorrencias; a++)
         {
            if (bIsCancel) { return; }

            //dsOcorrenciasTACO.Clear();
            dsArquivosTACO.Clear();
            listBox1.SelectedIndex = listBox1.Items.Count - 1;

            try
            {
               if (funDB1.conectarTaco() == "OK")
               {
                  try
                  {
                     string sId_ocorrenciaTaco = dsRetornoOcorrenciasAzure.Tables["ocorrencias"].Rows[a]["id_ocorrenciaTaco"].ToString();
                     string sId_ocorrenciaAzure = dsRetornoOcorrenciasAzure.Tables["ocorrencias"].Rows[a]["id_ocorrencia"].ToString();
                     sNomeArquivoEditado = sId_ocorrenciaAzure.ToString(); // Não tem nome de arquivo, aderiri o ID
                     string sUrlArquivo = dsRetornoOcorrenciasAzure.Tables["ocorrencias"].Rows[a]["urlArquivo"].ToString(); ; // Se tivesse no Firebase (Taco), viria a informação...não é o caso.

                     listBox1.Items.Add("");

                     listBox1.Items.Add("       " + DateTime.Now.ToString("dd/mm/yyyy H:mm:ss"));
                     listBox1.Items.Add("       ******* (TACO) id_ocorrencia: " + sId_ocorrenciaTaco + " - (AZURE) id_ocorrencia : " + sId_ocorrenciaAzure + " *******");
                     listBox1.Items.Add("       Url: " + sUrlArquivo);

                     if (sUrlArquivo.Equals("") || sUrlArquivo.Equals("0"))
                     {
                        listBox1.Items.Add("       - (TACO) Arquivo Não possui anexo.");

                     }
                     else
                     {


                        listBox1.Items.Add("       - (TACO) Buscando arquivo byte.");

                        string sCmd = "";
                        // VOLTAR AQUI
                        sCmd = "EXEC OMPSP_ocorrencia @id_ocorrencia=" + sId_ocorrenciaTaco + ", " +
                               "@modo=20";
                        Boolean bSegue = false;
                        try
                        {
                           SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTaco);
                           da.SelectCommand.CommandTimeout = 0;
                           da.Fill(dsArquivosTACO, "arquivos");
                           bSegue = true;
                        }
                        catch (Exception ex)
                        {
                           bSegue = false;
                        }
                        finally
                        {
                           funDB1.fecharTaco();
                        }


                        if (dsArquivosTACO.Tables["arquivos"].Rows.Count > 0)
                        {
                           listBox1.Items.Add("       - (TACO) - Localizado arquivo ocorrência.");

                           // Caso tenha urlArquivo já está no Firebase, não é necessário realizar a conversão (Download/Upload).
                           // Alguns arquivos onvertidos já não estão mais em Bytes na Taco.
                           //string sUrlArquivoTaco = dsArquivosTACO.Tables["ocorrencias"].Rows[0]["urlArquivo"].ToString();

                           //if (sUrlArquivoTaco != "")
                           //{
                           //   registraConversao(sId_arquivo, sUrlArquivoTaco);

                           //   // Faz a rolagem da lista
                           //   listBox1.SelectedIndex = listBox1.Items.Count - 1;

                           //   // Pausa de 2 segundos, evitar TimeOut.
                           //   System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

                           //}
                           //else
                           //{

                           byte[] arquivoM = new byte[0];
                           DataRow linhaM = dsArquivosTACO.Tables[0].Rows[0];
                           arquivoM = (byte[])linhaM["imagem"];

                           // Enviar o Bytes para o FIREBASE.
                           // Sobe para o Firebase o documento
                           if (Convert.ToInt32(sId_ocorrenciaTaco) > 0 && bSegue)
                           {
                              listBox1.Items.Add("       - (Firebase) Enviando arquivo...");

                              // Faz a rolagem da lista
                              listBox1.SelectedIndex = listBox1.Items.Count - 1;

                              //if (!funDrive1.uploadToDrive(this.id_empresa, "GED", "OM_ocorrenciaArquivo", sId_ocorrenciaAzure, arquivoM, sNomeArquivoEditado))
                              //{
                              //   listBox1.Items.Add("          - Já realizado envio anteriormente");
                              //}
                              //else
                              //{
                              //   listBox1.Items.Add("          - Enviado!");
                              //}

                              string sArquivoCompleto = sUrlArquivo;
                              //string sMimeType = funDrive1.retornaMimeType(sExtensao);
                              string sMimeType = "image/jpeg";

                              if (!funDrive1.uploadSCCToDrive(sArquivoCompleto, sMimeType, arquivoM))
                              {
                                 listBox1.Items.Add("          - Já realizado envio anteriormente");
                              }
                              else
                              {
                                 listBox1.Items.Add("          - Enviado!");
                              }

                              //registraConversaoOcorrencia(sId_ocorrenciaAzure);

                              // Faz a rolagem da lista
                              listBox1.SelectedIndex = listBox1.Items.Count - 1;

                              // Pausa de 2 segundos, evitar TimeOut.
                              System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                           }
                           //}
                        }
                        else
                        {
                           listBox1.Items.Add("       - (TACO) - Não localizado arquivo byte");
                        }
                     }

                     registraConversaoOcorrencia(sId_ocorrenciaAzure);
                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro procura arquivo bytes (TACO) procuraArquivoBinarioTaco() - 2");
                  }
                  finally
                  {
                     funDB1.fecharTaco();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro procura arquivo bytes (TACO) procuraArquivoBinarioTaco() - 3");
            }
            finally
            {
               funDB1.fecharTaco();
            }

            // Faz a lista correr.
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
         }
      }

      private DataSet insereArquivosAZURE(DataSet dsArquivosTACO)
      {
         dsArquivosAzure.Clear();

         listBox1.Items.Add("    - Foram encontrado(s) " + dsArquivosTACO.Tables["arquivos"].Rows.Count + " arquivos.");
         listBox1.Items.Add("    - Inserindo lista e retornando referência (AZURE)...");

         int iArquivos = dsArquivosTACO.Tables["arquivos"].Rows.Count;
         sTotalArquivos = iArquivos.ToString();
         string sXml = "";

         if (iArquivos > 0)
         {
            // Monta o XML com a Lista de ID_arquivo (TACO) a ser enviada.
            listBox1.Items.Add("    - Montando XML lista TACO/AZURE...");
            sXml = montaXMLArquivosTACO(dsArquivosTACO);

            // Envia ID_arquivoTACO (TACO) para SCC_arquivo (AZURE) .
            // Retorna ID_arquivo oficial (AZURE)
            try
            {
               listBox1.Items.Add("    - Enviando lista de arquivos TACO/AZURE (insereArquivosAZURE)");

               if (funDB1.conectarAzure() == "OK")
               {
                  try
                  {
                     string sCmd = "";
                     sCmd = "EXEC SCCSP_arquivo @id_empresa=" + this.id_empresa + ", " +
                            "@id_cliente=" + this.id_cliente + ", " +
                            "@strXml='" + sXml + "', " +
                            "@modo=20";

                     SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conAzure);
                     da.SelectCommand.CommandTimeout = 0;
                     da.Fill(dsArquivosAzure, "arquivos");
                     funDB1.fecharAzure();
                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro enviando lista de arquivos TACO/AZURE (insereArquivosAZURE)");
                  }
                  finally
                  {
                     funDB1.fecharAzure();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro enviando lista de arquivos TACO/AZURE (insereArquivosAZURE)");
            }
            finally
            {
               funDB1.fecharAzure();
            }

         }

         return dsArquivosAzure;
      }

      private void buscaIdsArquivosTaco()
      {
         string erro = "0", msg = "", url = "";
         string pastaDestino = "C:\\PRG\\SCC\\ARQUIVOS_OMP\\" + this.sCliente;
         DataTable dataTable = new DataTable();
         if (funDB1.conectarTaco() == "OK")
         {
            try
            {
               if (!Directory.Exists(pastaDestino))
               {
                  Directory.CreateDirectory(pastaDestino);
               }
               string query = "select a.id_arquivo, a.nome, a.extensao from OM_arquivo a \r\n\tleft outer join OM_empresa emp on emp.codigo=@codigo\r\n\tleft outer join OM_cliente cli on cli.id_empresa=emp.id_empresa and cli.codigo=@codigoCliente\r\n\twhere a.id_empresa=emp.id_empresa and a.id_cliente=cli.id_cliente and isnull(extensao,'')!='' and extensao like '.%'";
               SqlCommand cmdSql = new SqlCommand(query, funDB1.conTaco);
               cmdSql.Parameters.AddWithValue("@codigo", this.sCodigoAdm);
               cmdSql.Parameters.AddWithValue("@codigoCliente", this.sCliente);
               SqlDataReader dr;
               dr = cmdSql.ExecuteReader();
               dataTable.Load(dr);
               dr.Close();
               int quantidadeDeLinhas = dataTable.Rows.Count;
               if (quantidadeDeLinhas > 0)
               {
                  funDB1.fecharTaco();
                  //while (dr.Read()) {
                  listBox1.Items.Add("**************** Iniciando processo ****************");
                  foreach (DataRow row in dataTable.Rows)
                  {
                     string arqNome = row["nome"].ToString() != null && row["nome"].ToString() != "" ? (row["id_arquivo"].ToString() + " - " + row["nome"].ToString()) : (row["id_arquivo"].ToString() + row["extensao"].ToString());
                     string caminhoCompletoDestino = Path.Combine(pastaDestino, arqNome);
                     if (funDB1.conectarTaco() == "OK")
                     {
                        string queryArquivo = "select arquivo from OM_arquivo where id_arquivo=@id_arquivo and arquivo is not null";
                        SqlCommand cmdArqSql = new SqlCommand(queryArquivo, funDB1.conTaco);
                        cmdArqSql.Parameters.AddWithValue("@id_arquivo", row["id_arquivo"].ToString());
                        SqlDataReader arquivo;
                        arquivo = cmdArqSql.ExecuteReader();
                        //arquivo = cmdArqSql.ExecuteScalar();
                        if (arquivo.Read())
                        {
                           try
                           {
                              byte[] arquivoM = new byte[0];
                              arquivoM = (byte[])arquivo["arquivo"];
                              File.WriteAllBytes(caminhoCompletoDestino, arquivoM);
                              listBox1.Items.Add("");
                              listBox1.Items.Add("Adicionado: " + caminhoCompletoDestino);
                           }
                           catch (Exception err)
                           {
                              listBox1.Items.Add("");
                              listBox1.Items.Add("ERRO: " + caminhoCompletoDestino);
                              //MessageBox.Show("Erro" + err.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                              //arquivo.Close();
                           }
                        }
                        //arquivo.Close();
                     }
                     else
                     {
                        listBox1.Items.Add("");
                        listBox1.Items.Add("ERRO: " + caminhoCompletoDestino);
                     }
                     funDB1.fecharTaco();
                  }
               }
               listBox1.Items.Add("");
               listBox1.Items.Add("**************** Processo finalizado ***************");
            }
            catch (SqlException err)
            {
               erro = "1";
               msg = err.Message;
            }
            finally
            {
               funDB1.fecharTaco();
            }
         }

      }

      private void procuraArquivoBinarioTaco(DataSet dsRetornoArquivosAzure)
      {
         listBox1.Items.Add("    - Procura arquivo binário (TACO).");

         string sNomeArquivoEditado = "";
         int iArquivos = dsRetornoArquivosAzure.Tables["arquivos"].Rows.Count;

         // Procura arquivos por Cliente
         for (int a = 0; a < iArquivos; a++)
         {
            if (bIsCancel) { return; }

            dsArquivosTACO.Clear();
            listBox1.SelectedIndex = listBox1.Items.Count - 1;

            try
            {
               if (funDB1.conectarTaco() == "OK")
               {
                  try
                  {
                     string sId_arquivo = dsRetornoArquivosAzure.Tables["arquivos"].Rows[a]["id_arquivo"].ToString();
                     string sId_arquivoTaco = dsRetornoArquivosAzure.Tables["arquivos"].Rows[a]["id_arquivoTaco"].ToString();
                     sNomeArquivoEditado = dsRetornoArquivosAzure.Tables["arquivos"].Rows[a]["nomeArquivo"].ToString().Replace(",", " ");
                     string sUrlArquivo = dsRetornoArquivosAzure.Tables["arquivos"].Rows[a]["urlArquivo"].ToString();

                     listBox1.Items.Add("");

                     //listBox1.Items.Add("       ******* (TACO) id_arquivo: " + sId_arquivoTaco + " - (AZURE) id_arquivo : " + sId_arquivo + " *******");
                     //listBox1.Items.Add("       Url: " + sUrlArquivo);

                     listBox1.Items.Add("       " + DateTime.Now.ToString("dd/mm/yyyy H:mm:ss"));
                     listBox1.Items.Add("       ******* (TACO) id_arquivo: " + sId_arquivoTaco + " - (AZURE) id_arquivo : " + sId_arquivo + " *******");
                     listBox1.Items.Add("       Url: " + sUrlArquivo);


                     listBox1.Items.Add("       - (TACO) Buscando arquivo byte.");

                     string sCmd = "";
                     sCmd = "EXEC OMPSP_documento @id_arquivo=" + sId_arquivoTaco + ", " +
                            "@modo=21";

                     //SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTaco);
                     //da.SelectCommand.CommandTimeout = 0;
                     //da.Fill(dsArquivosTACO, "arquivos");
                     //funDB1.fecharTaco();
                     Boolean bSegue = false;

                     try
                     {
                        SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTaco);
                        da.SelectCommand.CommandTimeout = 0;
                        da.Fill(dsArquivosTACO, "arquivos");
                        bSegue = true;
                     }
                     catch (Exception ex)
                     {
                        bSegue = false;
                     }
                     finally
                     {
                        funDB1.fecharTaco();
                     }


                     if (dsArquivosTACO.Tables["arquivos"].Rows.Count > 0)
                     {
                        listBox1.Items.Add("       - (TACO) - Localizado arquivo.");

                        // Caso tenha urlArquivo já está no Firebase, não é necessário realizar a conversão (Download/Upload).
                        // Alguns arquivos onvertidos já não estão mais em Bytes na Taco.
                        string sUrlArquivoTaco = dsArquivosTACO.Tables["arquivos"].Rows[0]["urlArquivo"].ToString();

                        if (sUrlArquivoTaco != "")
                        {
                           registraConversao(sId_arquivo, sUrlArquivoTaco);

                           // Faz a rolagem da lista
                           listBox1.SelectedIndex = listBox1.Items.Count - 1;

                           // Pausa de 2 segundos, evitar TimeOut.
                           System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

                        }
                        else
                        {

                           byte[] arquivoM = new byte[0];
                           DataRow linhaM = dsArquivosTACO.Tables[0].Rows[0];
                           arquivoM = (byte[])linhaM["arquivo"];

                           // Enviar o Bytes para o FIREBASE.
                           // Sobe para o Firebase o documento
                           if (Convert.ToInt32(sId_arquivo) > 0 && sNomeArquivoEditado != "" && bSegue)
                           {
                              listBox1.Items.Add("       - (Firebase) Enviando arquivo...");

                              // Faz a rolagem da lista
                              listBox1.SelectedIndex = listBox1.Items.Count - 1;

                              if (!funDrive1.uploadToDrive(this.id_empresa, "GED", "SCC_ARQUIVO", sId_arquivo, arquivoM, sNomeArquivoEditado))
                              {
                                 listBox1.Items.Add("          - Já realizado envio anteriormente");
                              }
                              else
                              {
                                 listBox1.Items.Add("          - Enviado!");
                              }

                              registraConversao(sId_arquivo, "");

                              // Faz a rolagem da lista
                              listBox1.SelectedIndex = listBox1.Items.Count - 1;

                              // Pausa de 2 segundos, evitar TimeOut.
                              System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                           }
                        }
                     }
                     else
                     {
                        listBox1.Items.Add("       - (TACO) - Não localizado arquivo byte");
                     }

                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro procura arquivo bytes (TACO) procuraArquivoBinarioTaco() - 2");
                  }
                  finally
                  {
                     funDB1.fecharTaco();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro procura arquivo bytes (TACO) procuraArquivoBinarioTaco() - 3");
            }
            finally
            {
               funDB1.fecharTaco();
            }

            // Faz a lista correr.
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
         }
      }

      private string montaXMLArquivosTACO(DataSet ds)
      {
         int iArquivos = ds.Tables["arquivos"].Rows.Count;

         string sXml = "";

         sXml = "<arquivo>";
         sXml += "<cliente>";
         sXml += "<id_empresa>" + this.id_empresa + "</id_empresa>";
         sXml += "<id_cliente>" + this.id_cliente + "</id_cliente>";

         for (int i = 0; i < iArquivos; i++)
         {
            string sId_arquivoTaco = "", sId_usuario = "", sBloco = "", sApto = "", sDescricaoTipo = "", sData = "",
                   sDescricao = "", sNome = "", sExtensao = "", sTamanho = "", sUrlArquivo="";

            sId_arquivoTaco = ds.Tables["arquivos"].Rows[i]["id_arquivo"].ToString();
            sId_usuario = ds.Tables["arquivos"].Rows[i]["id_usuario"].ToString();
            sBloco = funcoes1.LimpaLinha(ds.Tables["arquivos"].Rows[i]["bloco"].ToString());
            sApto = funcoes1.LimpaLinha(ds.Tables["arquivos"].Rows[i]["apto"].ToString());
            sDescricaoTipo = funcoes1.LimpaLinha(ds.Tables["arquivos"].Rows[i]["descricaoTipo"].ToString());
            sData = ds.Tables["arquivos"].Rows[i]["data"].ToString();
            sDescricao = funcoes1.LimpaLinha(ds.Tables["arquivos"].Rows[i]["descricao"].ToString());
            sNome = funcoes1.LimpaLinha(ds.Tables["arquivos"].Rows[i]["nome"].ToString());
            sTamanho = ds.Tables["arquivos"].Rows[i]["tamanho"].ToString();
            sExtensao = ds.Tables["arquivos"].Rows[i]["extensao"].ToString();
            sUrlArquivo = ds.Tables["arquivos"].Rows[i]["urlArquivo"].ToString();

            // Retira a extensão do nome do arquivo
            int iPosicaoExtensao = 0;
            int iTam = sNome.ToString().Length;
            for (int t = 0; t < iTam; t++)
            {
               string sCaractere = sNome.Substring(t, 1);
               if (sCaractere.Equals("."))
               {
                  iPosicaoExtensao = t;
               }
            }

            sExtensao = sNome.Substring(iPosicaoExtensao);
            ///////////////////////


            sXml += "<registro>";

            sXml += "<id_arquivoTaco>" + sId_arquivoTaco + "</id_arquivoTaco>";
            sXml += "<id_usuario>" + sId_usuario + "</id_usuario>";
            sXml += "<bloco>" + sBloco + "</bloco>";
            sXml += "<apto>" + sApto + "</apto>";
            sXml += "<descricaoTipo>" + sDescricaoTipo + "</descricaoTipo>";
            sXml += "<data>" + sData + "</data>";
            sXml += "<descricao>" + sDescricao + "</descricao>";
            sXml += "<nome>" + sNome + "</nome>";
            sXml += "<tamanho>" + sTamanho + "</tamanho>";
            sXml += "<extensao>" + sExtensao + "</extensao>";
            sXml += "<urlArquivo>" + sUrlArquivo + "</urlArquivo>";

            sXml += "</registro>";

         }
         sXml += "</cliente>";
         sXml += "</arquivo>";


         return sXml;
      }
      #endregion
      //------------------------------------------------------------------------------------------------------------------//
      #region CONVERSÃO SCC4W - ARQUIVOS LOCAL/AZURE
      public Collection<string[]> arrayListaArquivos = new Collection<string[]>();

      //public string sPath = "C:\\PRG\\SCC\\ANEXOS\\";
      //public string[] pathAnexos = Directory.GetDirectories("C:\\PRG\\SCC\\ANEXOS\\");

      public string sPath = "C:\\PRG\\SCC\\ANEXOS\\";
      public string[] pathAnexos = Directory.GetDirectories("C:\\PRG\\SCC\\ANEXOS\\");

      private void procuraArquivosLOCAL(DataSet dsCliente)
      {
         arrayListaArquivos.Clear();

         int iClientes = dsCliente.Tables["clientes"].Rows.Count;
         sTotal = iClientes.ToString();

         listBox1.Items.Add("(LOCAL) - Consultando arquivos...");

         // Procura arquivos por Cliente
         for (int i = 0; i < iClientes; i++)
         {
            // Envio por Edificio
            arrayListaArquivos.Clear();

            if (bIsCancel) { return; }

            // ID_empresa e ID_cliente (Azure)
            this.id_empresa = dsCliente.Tables["clientes"].Rows[i]["id_empresa"].ToString();
            this.id_cliente = dsCliente.Tables["clientes"].Rows[i]["id_cliente"].ToString();
            this.sCodigoCliente = dsCliente.Tables["clientes"].Rows[i]["codigo"].ToString().PadLeft(4, Convert.ToChar("0"));

            this.sEdificio = dsCliente.Tables["clientes"].Rows[i]["codigo"].ToString().Substring(4, 4);
            string sCliente = "Cliente: " + dsCliente.Tables["clientes"].Rows[i]["codigo"].ToString() + "-" + dsCliente.Tables["clientes"].Rows[i]["nome"].ToString();

            string[] pathEdificios = Directory.GetDirectories(sPath);
            string sPathEdificio = sPath + this.sEdificio;

            for (int y = 0; y < pathEdificios.Length; y++)
            {
               if (pathEdificios[y].Equals(sPathEdificio))
               {
                  // Caso exista a pasta
                  if (Directory.Exists(sPathEdificio))
                  {
                     // Arquivos do Edificio
                     percorrePastaEdificio(sPathEdificio);

                     // Arquivos das Unidades
                     percorrePastaEdificioUnidades(sPathEdificio);
                  }
               }
            }

            listBox1.Items.Add("");

            // Faz a lista correr.
            listBox1.SelectedIndex = listBox1.Items.Count - 1;


            if (arrayListaArquivos.Count > 0)
            {
               // Insere lista arquivos TACO em AZURE
               // Retorna lista do SCC_arquivo inseridas (id_arquivo e id_arquivoTaco).
               DataSet dsRetornoAzure = insereArquivosLocalAZURE();
               if (dsRetornoAzure.Tables["arquivos"].Rows.Count > 0)
               {
                  // Percorre lista de arquivo AZURE e localiza arquivo bytes TACO
                  // Envia para o Firebase
                  procuraArquivoBinarioLOCAL(dsRetornoAzure);
               }
               else
               {
                  listBox1.Items.Add("    - Não existem registros a serem enviados.");
               }
            }
            else
            {
               listBox1.Items.Add("    - Não existem registros para esse cliente.");
            }
         }

         listBox1.Items.Add("");

         // Faz a lista correr.
         listBox1.SelectedIndex = listBox1.Items.Count - 1;

         string sTeste = "";
      }

      private DataSet insereArquivosLocalAZURE()
      {
         dsArquivosAzure.Clear();

         listBox1.Items.Add("    - Foram encontrado(s) " + arrayListaArquivos.Count + " arquivos.");
         listBox1.Items.Add("    - Inserindo lista e retornando referência (AZURE)...");

         int iArquivos = arrayListaArquivos.Count;
         sTotalArquivos = iArquivos.ToString();
         string sXml = "";

         if (iArquivos > 0)
         {
            // Monta o XML com a Lista de ID_arquivo (TACO) a ser enviada.
            listBox1.Items.Add("    - Montando XML lista LOCAL/AZURE...");
            sXml = montaXMLArquivosLOCAL();

            // Envia ID_arquivoTACO (LOCAL) para SCC_arquivo (AZURE) .
            // Retorna ID_arquivo oficial (AZURE)
            try
            {
               listBox1.Items.Add("    - Enviando lista de arquivos LOCAL/AZURE (insereArquivosAZURE)");

               if (funDB1.conectarAzure() == "OK")
               {
                  try
                  {
                     string sCmd = "";
                     sCmd = "EXEC SCCSP_arquivo @id_empresa=" + this.id_empresa + ", " +
                            "@id_cliente=" + this.id_cliente + ", " +
                            "@strXml='" + sXml + "', " +
                            "@modo=21";

                     SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conAzure);
                     da.SelectCommand.CommandTimeout = 0;
                     da.Fill(dsArquivosAzure, "arquivos");
                     funDB1.fecharAzure();
                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro enviando lista de arquivos LOCAL/AZURE (insereArquivosAZURE)");
                  }
                  finally
                  {
                     funDB1.fecharAzure();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro enviando lista de arquivos LOCAL/AZURE (insereArquivosAZURE)");
            }
            finally
            {
               funDB1.fecharAzure();
            }

         }

         return dsArquivosAzure;
      }

      private string montaXMLArquivosLOCAL()
      {
         int iArquivos = arrayListaArquivos.Count;

         string sXml = "";

         sXml = "<arquivo>";
         sXml += "<cliente>";
         sXml += "<id_empresa>" + this.id_empresa + "</id_empresa>";
         sXml += "<id_cliente>" + this.id_cliente + "</id_cliente>";

         for (int i = 0; i < iArquivos; i++)
         {
            string sTipo = "", sEdificio = "", sUnidade = "", sNome = "", sExtensao = "", sCaminho = "",
                   sDescricaoTipo = "", sDescricao = "";

            sTipo = arrayListaArquivos[i][0].ToString();
            sEdificio = arrayListaArquivos[i][1].ToString();
            sUnidade = arrayListaArquivos[i][2].ToString();
            sNome = arrayListaArquivos[i][3].ToString();
            sExtensao = arrayListaArquivos[i][4].ToString();
            sCaminho = arrayListaArquivos[i][5].ToString();
            sDescricaoTipo = "SCC4W - GED LOCAl";
            sDescricao = sNome.Substring(0, sNome.IndexOf("."));

            // Retira a extensão do nome do arquivo
            int iPosicaoExtensao = 0;
            int iTam = sNome.ToString().Length;
            for (int t = 0; t < iTam; t++)
            {
               string sCaractere = sNome.Substring(t, 1);
               if (sCaractere.Equals("."))
               {
                  iPosicaoExtensao = t;
               }
            }

            sExtensao = sNome.Substring(iPosicaoExtensao);
            ///////////////////////

            sXml += "<registro>";

            sXml += "<tipo>" + sTipo + "</tipo>";
            sXml += "<edificio>" + sEdificio + "</edificio>";
            sXml += "<unidade>" + sUnidade + "</unidade>";
            sXml += "<nome>" + sNome + "</nome>";
            sXml += "<extensao>" + sExtensao + "</extensao>";
            sXml += "<caminho>" + sCaminho + "</caminho>";
            sXml += "<descricaoTipo>" + sDescricaoTipo + "</descricaoTipo>";
            sXml += "<descricao>" + sDescricao + "</descricao>";

            sXml += "</registro>";

         }
         sXml += "</cliente>";
         sXml += "</arquivo>";


         return sXml;
      }

      public void percorrePastaEdificio(string dirEdificio)
      {
         // Localiza arquivos do Edificio
         string[] dirArquivosEdificio = Directory.GetFiles("" + dirEdificio);
         int pos = 0;
         string sNomeArquivo = "";
         string sExtensaoArquivo = "";

         foreach (string dirArqEdificio in dirArquivosEdificio)
         {
            if (File.Exists(dirArqEdificio))
            {
               pos = dirArqEdificio.LastIndexOf("\\");
               sNomeArquivo = dirArqEdificio.Substring(pos + 1);

               pos = sNomeArquivo.LastIndexOf(".");
               sExtensaoArquivo = sNomeArquivo.Substring(pos);

               // Estrutura {tipo, edificio, unidade, nome do arquivo, extensão, apto, diretório}
               arrayListaArquivos.Add(new string[] { "1", this.sEdificio, "", sNomeArquivo, sExtensaoArquivo, dirArqEdificio });
            }
         }
      }
      public void percorrePastaEdificioUnidades(string dirEdificio)
      {
         // Localiza arquivos das Unidades
         string[] pathUnidades = Directory.GetDirectories(dirEdificio);
         int pos = 0;
         string sNomeArquivo = "";
         string sExtensaoArquivo = "";
         string sUnidade = "";

         for (int y = 0; y < pathUnidades.Length; y++)
         {
            string dirUnidades = pathUnidades[y];

            string[] dirArquivosEdificioUnidades = Directory.GetFiles("" + dirUnidades);
            foreach (string dirArqEdificioUnidade in dirArquivosEdificioUnidades)
            {
               if (File.Exists(dirArqEdificioUnidade))
               {
                  // UNIDADE
                  pos = dirUnidades.LastIndexOf("\\");
                  sUnidade = dirUnidades.Substring(pos + 1);

                  // NOME DO ARQUIVO
                  pos = dirArqEdificioUnidade.LastIndexOf("\\");
                  sNomeArquivo = dirArqEdificioUnidade.Substring(pos + 1);

                  // EXTENSAO
                  pos = sNomeArquivo.LastIndexOf(".");
                  sExtensaoArquivo = sNomeArquivo.Substring(pos);

                  // Estrutura {tipo, edificio, bloco, nome do arquivo, extensão, apto, diretório}
                  arrayListaArquivos.Add(new string[] { "2", this.sEdificio, sUnidade, sNomeArquivo, sExtensaoArquivo, dirArqEdificioUnidade });
               }
            }
         }
      }
      private void procuraArquivoBinarioLOCAL(DataSet dsRetornoArquivosAzure)
      {
         listBox1.Items.Add("    - Procura arquivo binário (LOCAL).");

         string sNomeArquivoEditado = "";
         int iArquivos = dsRetornoArquivosAzure.Tables["arquivos"].Rows.Count;

         // Procura arquivos por Cliente
         for (int a = 0; a < iArquivos; a++)
         {
            if (bIsCancel) { return; }

            listBox1.SelectedIndex = listBox1.Items.Count - 1;

            try
            {
               if (funDB1.conectarTaco() == "OK")
               {
                  try
                  {
                     string sId_arquivo = dsRetornoArquivosAzure.Tables["arquivos"].Rows[a]["id_arquivo"].ToString();
                     string sId_arquivoTaco = dsRetornoArquivosAzure.Tables["arquivos"].Rows[a]["id_arquivoTaco"].ToString();
                     sNomeArquivoEditado = dsRetornoArquivosAzure.Tables["arquivos"].Rows[a]["nomeArquivo"].ToString().Replace(",", " ");
                     string sUrlArquivo = dsRetornoArquivosAzure.Tables["arquivos"].Rows[a]["urlArquivo"].ToString();

                     listBox1.Items.Add("");

                     listBox1.Items.Add("       " + DateTime.Now.ToString("dd/mm/yyyy H:mm:ss"));
                     listBox1.Items.Add("       ******* (LOCAL) nome: " + sNomeArquivoEditado + " - (AZURE) id_arquivo : " + sId_arquivo + " *******");
                     listBox1.Items.Add("       Url: " + sUrlArquivo);


                     listBox1.Items.Add("       - (LOCAL) Transformando arquivo em byte.");

                     //string sCmd = "";
                     //sCmd = "EXEC OMPSP_documento @id_arquivo=" + sId_arquivoTaco + ", " +
                     //       "@modo=21";

                     //SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conTaco);
                     //da.SelectCommand.CommandTimeout = 0;
                     //da.Fill(dsArquivosTACO, "arquivos");
                     //funDB1.fecharTaco();

                     string sCaminho = dsRetornoArquivosAzure.Tables["arquivos"].Rows[a]["caminho"].ToString();
                     if (File.Exists(sCaminho))
                     {
                        listBox1.Items.Add("       - (LOCAL) - Localizado arquivo.");

                        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                        //byte[] arquivoM = encoding.GetBytes(sCaminho);
                        byte[] arquivoM = File.ReadAllBytes(sCaminho);

                        //if (sId_arquivo.Equals("520"))
                        //{
                        //   String img64Produto = Convert.ToBase64String(arquivoM);
                        //}

                        //byte[] arquivoM = new byte[0];
                        //DataRow linhaM = dsArquivosTACO.Tables[0].Rows[0];
                        //arquivoM = (byte[])linhaM["arquivo"];

                        // Enviar o Bytes para o FIREBASE.
                        // Sobe para o Firebase o documento
                        if (Convert.ToInt32(sId_arquivo) > 0 && sNomeArquivoEditado != "")
                        {
                           listBox1.Items.Add("       - (Firebase) Enviando arquivo...");

                           // Faz a rolagem da lista
                           listBox1.SelectedIndex = listBox1.Items.Count - 1;

                           if (!funDrive1.uploadToDrive(this.id_empresa, "GED", "SCC_ARQUIVO", sId_arquivo, arquivoM, sNomeArquivoEditado))
                           {
                              listBox1.Items.Add("          - Já realizado envio anteriormente");
                           }
                           else
                           {
                              listBox1.Items.Add("          - Enviado!");
                           }

                           // Faz a rolagem da lista
                           listBox1.SelectedIndex = listBox1.Items.Count - 1;

                           // Pausa de 2 segundos, evitar TimeOut.
                           System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                        }
                     }
                     else
                     {
                        listBox1.Items.Add("       - (TACO) - Não localizado arquivo byte");
                     }

                  }
                  catch (SqlException ex)
                  {
                     listBox1.Items.Add("    - Erro procura arquivo bytes (TACO) procuraArquivoBinarioTaco() - 2");
                  }
                  finally
                  {
                     funDB1.fecharTaco();
                  }
               }
            }
            catch (SqlException ex)
            {
               listBox1.Items.Add("    - Erro procura arquivo bytes (TACO) procuraArquivoBinarioTaco() - 3");
            }
            finally
            {
               funDB1.fecharTaco();
            }

            // Faz a lista correr.
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
         }
      }
      #endregion
      //------------------------------------------------------------------------------------------------------------------//

      private string Aviso(string descricao, int espaco)
      {
         String sAviso = "";

         sAviso = (DateTime.Now.ToString("dd/mm/yyyy H:mm:ss") + " - " + descricao.Trim()).PadLeft(espaco, ' ');

         return sAviso;
      }

      private void registraConversao(string sId_arquivo, string sUrlArquivoTaco = "")
      {
         try
         {
            listBox1.Items.Add("    - Registrando arquivo convertido AZURE.");

            if (funDB1.conectarAzure() == "OK")
            {
               try
               {

                  string sCmd = "";
                  sCmd = "EXEC SCCSP_arquivo @id_empresa = " + this.id_empresa + ", " +
                         " @id_arquivo=" + sId_arquivo + ", " +
                         " @urlArquivoTaco='" + sUrlArquivoTaco + "', " +
                         "@modo=23";

                  SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conAzure);
                  da.SelectCommand.CommandTimeout = 0;
                  da.Fill(dsArquivosAzure, "arquivoOK");
                  funDB1.fecharAzure();

               }
               catch (SqlException ex)
               {
                  listBox1.Items.Add("    - Erro enviando lista de arquivos TACO/AZURE (insereArquivosAZURE)");
               }
               finally
               {
                  funDB1.fecharAzure();
               }
            }
         }
         catch (SqlException ex)
         {
            listBox1.Items.Add("    - Erro ao regitsrar AZURE - id_arquivo: " + sId_arquivo);
         }

      }
      private void registraConversaoOcorrencia(string sId_ocorrencia)
      {
         try
         {
            listBox1.Items.Add("    - Registrando arquivo convertido AZURE.");

            if (funDB1.conectarAzure() == "OK")
            {
               try
               {

                  string sCmd = "";
                  sCmd = "EXEC OMPSP_ocorrencia @id_empresa = " + this.id_empresa + ", " +
                         " @id_ocorrencia=" + sId_ocorrencia + ", " +
                         "@modo=31";

                  SqlDataAdapter da = new SqlDataAdapter(sCmd, funDB1.conAzure);
                  da.SelectCommand.CommandTimeout = 0;
                  da.Fill(dsArquivosAzure, "ocorrenciaOK");
                  funDB1.fecharAzure();

               }
               catch (SqlException ex)
               {
                  listBox1.Items.Add("    - Erro enviando lista de ocorrencias TACO/AZURE (insereOcorrenciasAZURE)");
               }
               finally
               {
                  funDB1.fecharAzure();
               }
            }
         }
         catch (SqlException ex)
         {
            listBox1.Items.Add("    - Erro ao regitsrar AZURE - id_ocorrencia: " + sId_ocorrencia);
         }

      }

   }

}
