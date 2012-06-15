using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileMonkey.Pandora.dal;
using FileMonkey.Pandora.dal.entities;
using Pandora.dpl;

namespace FileMonkey.Picasso
{
    /// <summary>
    /// Lógica de interacción para Inspectors.xaml
    /// </summary>
    public partial class Inspectors : Window
    {
        public Inspectors()
        {
            InitializeComponent();

            this.Owner = App.Home;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InspectorsRefresh(null);
        }

        private void InspectorsRefresh(Inspector inspector, Boolean added = false)
        {
            List<Inspector> lstInspectors = new List<Inspector>();

            if (inspector == null)
            {
                using (var db = new DataContext())
                {
                    lstInspectors = db.Inspectors.ToList();

                    lstInspectors.ForEach(insp => FillRulesAux(insp));
                }
            }
            else
            {
                lstInspectors = (List<Inspector>)lstVInspectors.ItemsSource;

                if (added)
                {
                    FillRulesAux(inspector);
                    lstInspectors.Add(inspector);
                }
                else
                    lstInspectors.Remove(inspector);                
            }

            lstVInspectors.ItemsSource = null;
            lstVInspectors.ItemsSource = lstInspectors;
        }

        private void FillRulesAux(Inspector inspector)
        {
            if (inspector.Enable)
            {
                inspector.ImageEnable = @"/Resources/play.png";
            }
            else
            {
                inspector.ImageEnable = @"/Resources/pausa.png";
            }

            inspector.CheckPeriodText = InspectorDetail.GetPeriodText(inspector.CheckPeriod) + " ";

            inspector.RulesAux = new List<InspectorHelper>(3);

            var queryRule = from rule in inspector.Rules
                             where rule.RuleType.Equals((int)RuleFile.TypeFileRule.FileName)
                             select rule;

            InspectorHelper inspHelper = new InspectorHelper();
            inspHelper.CountRuleType = queryRule.Count();
            inspHelper.Type = RuleFile.TypeFileRule.FileName;
            inspHelper.ImagePath = @"/images/icon_ruleFileName.png";

            inspector.RulesAux.Add(inspHelper);

            queryRule = from rule in inspector.Rules
                        where rule.RuleType.Equals((int)RuleFile.TypeFileRule.Extension)
                         select rule;

            inspHelper = new InspectorHelper();
            inspHelper.CountRuleType = queryRule.Count();
            inspHelper.Type = RuleFile.TypeFileRule.Extension;
            inspHelper.ImagePath = @"/images/icon_ruleFileExtension.png";

            inspector.RulesAux.Add(inspHelper);

            queryRule = from rule in inspector.Rules
                        where rule.RuleType.Equals((int)RuleFile.TypeFileRule.Date)
                         select rule;

            inspHelper = new InspectorHelper();
            inspHelper.CountRuleType = queryRule.Count();
            inspHelper.Type = RuleFile.TypeFileRule.Date;
            inspHelper.ImagePath = @"/images/icon_ruleFileDate.png";

            inspector.RulesAux.Add(inspHelper);
        }

        private void lstVInspectors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstVInspectors.SelectedIndex != -1)
            {
                Inspector inspSelected = (Inspector)lstVInspectors.SelectedValue;

                InspectorDetail inspDetail = new InspectorDetail(inspSelected);
                inspDetail.ShowDialog();

                List<Inspector> lstInspectors = (List<Inspector>)lstVInspectors.ItemsSource;

                inspSelected = inspDetail.inspector;

                FillRulesAux(inspSelected);

                lstVInspectors.ItemsSource = null;
                lstVInspectors.ItemsSource = lstInspectors;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Home.Focus();
        }

        private void pnlNewInspector_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var inspector = new Inspector { Path = "Seleccione una carpeta para rastrear" };

            InspectorDetail inspDetail = new InspectorDetail(inspector);
            inspDetail.ShowDialog();

            inspector = inspDetail.inspector;

            InspectorsRefresh(inspector, true);
        }

        private void pnlDeleteInspector_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lstVInspectors.SelectedIndex != -1)
            {
                Inspector inspSelected = (Inspector)lstVInspectors.SelectedValue;

                using (var db = new DataContext())
                {
                    db.RemoveEntity(inspSelected);
                    App.Single.RemoveWork(inspSelected);

                    db.SaveChanges();
                }

                InspectorsRefresh(inspSelected);
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un inspector de la lista");
            }    
        }

        private void panels_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StackPanel panel = (StackPanel)sender;
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)panel.Children[1];
            label.FontWeight = FontWeights.ExtraBold;
        }

        private void panels_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StackPanel panel = (StackPanel)sender;
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)panel.Children[1];
            label.FontWeight = FontWeights.Normal;
        }

        private void pnlActDesactInspector_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lstVInspectors.SelectedIndex != -1)
            {
                Inspector inspSelected = (Inspector)lstVInspectors.SelectedValue;

                using (var db = new DataContext())
                {
                    inspSelected.Enable = !inspSelected.Enable;
                    
                    db.PersistEntity(inspSelected);
                    db.SaveChanges();
                }

                if (inspSelected.Enable)
                {
                    inspSelected.ImageEnable = @"/Resources/play.png";

                    App.Single.AddWork(inspSelected);
                }
                else
                {
                    inspSelected.ImageEnable = @"/Resources/pausa.png";

                    App.Single.RemoveWork(inspSelected);
                }

                List<Inspector> lstInspectors = (List<Inspector>)lstVInspectors.ItemsSource;

                lstVInspectors.ItemsSource = null;
                lstVInspectors.ItemsSource = lstInspectors;
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un inspector de la lista");
            }    
        }
    }
}
