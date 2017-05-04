using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Data;
using System.Xml;
using System.Windows.Controls;
using System.Data;
using System.Xml.Linq;

namespace TWModer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string TwDir;
        private FileInfo[] Files;
        DataTable Table = new DataTable();

        public MainWindow()
        {
            InitializeComponent();
            TwDir = @"C:\Program Files (x86)\Steam\steamapps\common\Total War WARHAMMER\assembly_kit\raw_data\db";
            ReadTables();
        }

        private void ReadTables()
        {
            DirectoryInfo dir = new DirectoryInfo(TwDir);//TwDir);
            Files = dir.EnumerateFiles("*.xml").ToArray();

            for (int i = 0; i < Files.Length; i++)
            {
                var file = new TreeViewItem() { Header = Files[i].Name, Tag = i };
                file.Items.Add("Loading..."); 
                XmlFiles.Items.Add(file);
            }

        }

        private void EatDick_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ChooseDir_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                TwDir = dialog.SelectedPath;
            }

            if (!string.IsNullOrWhiteSpace(TwDir))
                ReadTables();
        }

        private void Node_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = e.Source as TreeViewItem;
            if ((node.Items.Count == 1) && (node.Items[0] is string))
            {
                XDocument doc = XDocument.Load(Files[(int)node.Tag].FullName);                                
                node.Items.Clear();
                XElement[] targetNode;
                targetNode = doc.Elements().ToArray();
                ReadNode(targetNode, node);
            }

            Output.DataContext = Table;
            
        }

        private void ReadNode(XElement[] node, TreeViewItem parent)
        {

            DataRow rw = Table.NewRow();
            TreeViewItem item;
            for (int i = 0; i < node.Length; i++)
            { 
                if (node[i].Elements().Count() == 0)
                {
                    if(!Table.Columns.Contains(node[i].Name.LocalName))
                        Table.Columns.Add(node[i].Name.LocalName);
                    rw[node[i].Name.LocalName] = node[i].Value;
                    item = new TreeViewItem { Header = node[i].Name.LocalName + " : " + node[i].Value, Tag = parent.Tag };                    
                }
                else
                    item = new TreeViewItem { Header = node[i].Name.LocalName, Tag = parent.Tag };



                parent.Items.Add(item);
                ReadNode(node[i].Elements().ToArray(), item);

            }
            if(rw.ItemArray.Any(i => !string.IsNullOrWhiteSpace(i as string)))
                Table.Rows.Add(rw);
            Table.AcceptChanges();

        }
    }
}
