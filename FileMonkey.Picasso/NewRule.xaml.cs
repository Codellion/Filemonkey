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
using System.Windows.Shapes;
using Elemental.pal.userControls;
using FileMonkey.Pandora.dal.entities;
using Elemental.pal.userControls.interfaces;

namespace FileMonkey.Picasso
{
    /// <summary>
    /// Lógica de interacción para NewRule.xaml
    /// </summary>
    public partial class NewRule : Window
    {
        public RuleFile rule { get; set; }

        public NewRule()
        {
            InitializeComponent();

            this.Owner = App.ActualInspectorDetail;
        }

        public NewRule(RuleFile rule)
        {
            InitializeComponent();
            this.rule = rule;

            rbtDate.IsEnabled = false;
            rbtExtension.IsEnabled = false;
            rbtName.IsEnabled = false;

            this.Owner = App.ActualInspectorDetail;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            txtName.Text = rule.Name;

            switch ((RuleFile.TypeFileRule) rule.RuleType)
            {
                case RuleFile.TypeFileRule.Date:
                    rbtDate.IsChecked = true;
                    break;
                case RuleFile.TypeFileRule.Extension:
                    rbtExtension.IsChecked = true;
                    break;
                case RuleFile.TypeFileRule.FileName:
                    rbtName.IsChecked = true;
                    break;
            }

            RuleControl rControl = (RuleControl)pnlExtensionControl.Children[0];
            rControl.SetRule(rule);
        }

        private void rbtDate_Checked(object sender, RoutedEventArgs e)
        {
            pnlExtensionControl.Children.Clear();
            pnlExtensionControl.Children.Add(new Date());
        }

        private void rbtExtension_Checked(object sender, RoutedEventArgs e)
        {
            pnlExtensionControl.Children.Clear();
            pnlExtensionControl.Children.Add(new Extension());
        }

        private void rbtName_Checked(object sender, RoutedEventArgs e)
        {
            pnlExtensionControl.Children.Clear();
            pnlExtensionControl.Children.Add(new Name());
        }

        private void SaveRule()
        {
            Boolean typeChoose = false;
            RuleFile ruleAux = null;

            if (rbtDate.IsChecked.HasValue && rbtDate.IsChecked.Value)
            {
                ruleAux = new RuleFileDate();
                ruleAux.RuleType = (int)RuleFile.TypeFileRule.Date;
                ruleAux.ImagePath = @"/images/icon_ruleFileDate.png";

                typeChoose = true;
            }
            else if (rbtExtension.IsChecked.HasValue && rbtExtension.IsChecked.Value)
            {
                ruleAux = new RuleFileExtension();
                ruleAux.RuleType = (int)RuleFile.TypeFileRule.Extension;
                ruleAux.ImagePath = @"/images/icon_ruleFileExtension.png";

                typeChoose = true;
            }
            else if (rbtName.IsChecked.HasValue && rbtName.IsChecked.Value)
            {
                ruleAux = new RuleFileName();
                ruleAux.RuleType = (int)RuleFile.TypeFileRule.FileName;
                ruleAux.ImagePath = @"/images/icon_ruleFileName.png";

                typeChoose = true;
            }

            if (!typeChoose)
            {
                MessageBox.Show("Necesita especificar el tipo de la regla");
                return;
            }

            RuleControl rControl = (RuleControl) pnlExtensionControl.Children[0];
            rule = rControl.GetRule();

            rule.RuleType = ruleAux.RuleType;
            rule.ImagePath = ruleAux.ImagePath;
            rule.Name = txtName.Text;            

            this.Close();
        }

        private void pnlSaveRule_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SaveRule();
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
    }
}
