using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using Desktop.Entity;

namespace Desktop
{
    public partial class Form1 : Form
    {
        Dictionary<string, int> dStatus = new Dictionary<string, int>();
        string url = "https://localhost:44335/";
        string acao = "";
        string idCliente = "0";
        public Form1()
        {
            InitializeComponent();
            listStatus();
            ListCliente();
        }

        private void cadastrarClienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pnCad.Visible = true;
            acao = "insert";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            pnCad.Visible = false;
            clear();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            saveCliente();
            clear();
        }

        void clear()
        {
            txtEmail.Text = "";
            txtNome.Text = "";
            txtSobrenome.Text = "";
            cbStatus.SelectedIndex = 0;
            acao = "";
            pnCad.Visible = false;
        }

        async void getClientes()
        {
            using (var Client = new HttpClient())
            {
                using (var response = await Client.GetAsync(url + "Clientes"))
                {
                    string str = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<Mensagem>(str);
                    ListCliente();

                    MessageBox.Show(result.texto);


                }
            }
        }

        async void saveCliente()
        {
            string _url = url;
            if (acao == "insert")
            {
                _url += "Clientes/" + txtNome.Text + "​/" + txtSobrenome.Text + "​/" + txtEmail.Text + "​/" + dStatus[cbStatus.Text];
                IEnumerable<KeyValuePair<string, string>> clientes = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("Nome", txtNome.Text.Trim()),
                new KeyValuePair<string, string>("Sobrenome", txtSobrenome.Text.Trim()),
                new KeyValuePair<string, string>("Email", txtEmail.Text.Trim()),
                new KeyValuePair<string, string>("Status", dStatus[cbStatus.Text].ToString())
                };


                HttpContent content = new FormUrlEncodedContent(clientes);

                using (var Client = new HttpClient())
                {
                    using (var response = await Client.PostAsync(_url, content))
                    {
                        string str = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<Mensagem>(str);

                        MessageBox.Show(result.texto);
                    }
                }
            }

            if (acao == "update")
            {
                _url += "Clientes/" + idCliente + "/" + txtNome.Text + "/" + txtSobrenome.Text + "/" + txtEmail.Text + "/" + dStatus[cbStatus.Text];

                IEnumerable<KeyValuePair<string, string>> clientes = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("id", idCliente),
                new KeyValuePair<string, string>("Nome", txtNome.Text.Trim()),
                new KeyValuePair<string, string>("Sobrenome", txtSobrenome.Text.Trim()),
                new KeyValuePair<string, string>("Email", txtEmail.Text.Trim()),
                new KeyValuePair<string, string>("Status", dStatus[cbStatus.Text].ToString())
                };

                HttpContent content = new FormUrlEncodedContent(clientes);

                using (var Client = new HttpClient())
                {
                    using (var response = await Client.PutAsync(_url, content))
                    {
                        string str = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<Mensagem>(str);

                        MessageBox.Show(result.texto);

                    }
                }
            }

            ListCliente();
            if (acao == "delete")
            {
                Int32 selectRowCount = gridClient.Rows.GetRowCount(DataGridViewElementStates.Selected);


                if (selectRowCount > 0)
                {
                    idCliente = "";

                    for (int i = 0; i < selectRowCount; i++)
                    {
                        if (idCliente != "")
                            idCliente += ",";

                        idCliente += gridClient.SelectedRows[i].Cells[0].Value.ToString();



                    }

                    try
                    {

                        using (var Client = new HttpClient())
                        {

                            _url += "Clientes/DELETE/" + idCliente;
                            using (var response = await Client.DeleteAsync(_url))
                            {
                                string str = await response.Content.ReadAsStringAsync();

                                var result = JsonConvert.DeserializeObject<Mensagem>(str);

                                if (result.texto != "")
                                    MessageBox.Show(result.texto);

                                ListCliente();
                            }
                        }

                    }
                    catch
                    {
                        MessageBox.Show("Não foi possivel conectar ao servidor!");
                    }
                }
            }
        }

        async void ListCliente()
        {
            Status status = new Status();
            List<Clientes> clientes;
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url + "Clientes"))
                {
                    string str = await response.Content.ReadAsStringAsync();

                    clientes = JsonConvert.DeserializeObject<List<Clientes>>(str);

                    gridClient.DataSource = clientes;
                    /*    for (int i = 0; i < clientes.Count; i++)
                        {
                            gridClient.Rows[i].Cells[0].Value = clientes[i].Id;
                   }*/
                }
            }
        }

        async void listStatus()
        {
            Status status = new Status();
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url + "Status"))
                {
                    string str = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<List<Status>>(str);

                    for (int i = 0; i < result.Count; i++)
                    {
                        dStatus.Add(result[i].status, result[i].id);
                        cbStatus.Items.Add(result[i].status);
                        cbStatus.SelectedIndex = 0;
                    }
                }
            }
        }

        private void editarClienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Int32 selectRowCount = gridClient.Rows.GetRowCount(DataGridViewElementStates.Selected);

            idCliente = gridClient.SelectedRows[0].Cells[0].Value.ToString();
            txtNome.Text = gridClient.SelectedRows[0].Cells[1].Value.ToString();
            txtSobrenome.Text = gridClient.SelectedRows[0].Cells[2].Value.ToString();
            txtEmail.Text = gridClient.SelectedRows[0].Cells[3].Value.ToString();
            cbStatus.Text = gridClient.SelectedRows[0].Cells[4].Value.ToString();

            pnCad.Visible = true;
            acao = "update";
        }

        private void deletarClienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            acao = "delete";
            saveCliente();
            ListCliente();
        }
    }

}
