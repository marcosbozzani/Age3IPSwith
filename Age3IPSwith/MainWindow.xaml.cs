using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Age3IPSwith
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            LoadData();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Info.Text = "Saving...";
            await Task.Delay(200);
            SaveData();
        }

        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            Info.Text = "Loading...";
            await Task.Delay(200);
            LoadData();
        }

        private void SaveData()
        {
            if (IpCombobox.Items.Count > 0)
            {
                Utils.WriteAddress(IpCombobox.SelectedValue.ToString());
                Info.Text = "Saved";
            }
        }

        private void LoadData()
        {
            var saved = Utils.ReadAddress();

            IpCombobox.Items.Clear();

            foreach (var ip in Utils.GetMachineIPs())
            {
                IpCombobox.Items.Add(ip);
                if (ip == saved)
                {
                    IpCombobox.SelectedValue = ip;
                }
            }

            if (IpCombobox.Items.Count > 0 && IpCombobox.SelectedIndex == -1)
            {
                IpCombobox.SelectedIndex = 0;
            }

            Info.Text = "Loaded";
        }
    }
}
