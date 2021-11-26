using gyak06_n0adbr.Entities;
using gyak06_n0adbr.MnbServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace gyak06_n0adbr
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        BindingList<string> Currencies = new BindingList<string>();

        public Form1()
        {
            InitializeComponent();
            GetCurrenciesRequestBody request = new GetCurrenciesRequestBody();
            foreach (XmlElement element in xml.DocumentElement.ChildNodes[0])
            {
                string newItem = element.InnerText;
                Currencies.Add(newItem);
            };
            RefresData();
        }

        private void RefresData()
        {
            Rates.Clear();
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames = (comboBox1.SelectedItem).ToString(),
                startDate = (dateTimePicker1.Value).ToString(),
                endDate = (dateTimePicker2.Value).ToString()
            };
            var response = mnbService.GetExchangeRates(request);
            var result = response.GetExchangeRatesResult;
            dataGridView1.DataSource = Rates;

            var xml = new XmlDocument();
            xml.LoadXml(result);


            foreach (XmlElement element in xml.DocumentElement)
            {
                var rate = new RateData();
                Rates.Add(rate);

                rate.Date = DateTime.Parse(element.GetAttribute("date"));

                var childElement = (XmlElement)element.ChildNodes[0];
                if (childElement == null)
                    continue;

                rate.Currency = childElement.GetAttribute("curr");

                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                    rate.Value = value / unit;
            }
            Diagram();
            comboBox1.DataSource = Currencies;
        }

        private void Diagram()
        {
            chartRateData.DataSource = Rates;

            var adatsor = chartRateData.Series[0];
            adatsor.ChartType = SeriesChartType.Line;
            adatsor.XValueMember = "Date";
            adatsor.YValueMembers = "Value";
            adatsor.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefresData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RefresData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefresData();
        }
    }
}
