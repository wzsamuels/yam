﻿/*
   Copyright 2014 W. Z. Samuels

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

 */

using System;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Yam
{
    /// <summary>
    /// Interaction logic for newWorld.xaml
    /// </summary>
    public partial class OpenWorldWindow : Window
    {
        public WorldInfo UI { get; set; } = new WorldInfo();
        public bool NewWorldSelect { get; set; } = false;
        private bool AutoLogin { get; set; } = false;
        public bool SaveLogin { get; set; } = false;  

        public OpenWorldWindow()
        {
            InitializeComponent();
            
            //Set up UI defaults
            usernameText.IsEnabled = false;
            passwordText.IsEnabled = false;
            usernameBlock.Foreground = Brushes.Gray;
            passwordBlock.Foreground = Brushes.Gray;

            ObservableCollection<ListBoxItem> listItems = new ObservableCollection<ListBoxItem>();

            var loadedWorlds = MainWindow.ReadConfig().Worlds;
            if (loadedWorlds != null)
            {
                foreach (WorldInfo world in MainWindow.ReadConfig().Worlds)
                {
                    ListBoxItem worldItem = new ListBoxItem
                    {
                        Content = world.WorldName,
                        Name = world.WorldName
                    };

                    listItems.Add(worldItem);
                }
                worldList.ItemsSource = listItems;
                //If there are saved worlds, assume the user
                //Wants to open one
                if (worldList.Items.Count > 0)
                {
                    worldList.SelectedIndex = 0;
                    savedWorldButton.IsChecked = true;
                    worldList.Focus();
                }
                //Otherwise default to a new world
                else
                {
                    newWorldButton.IsChecked = true;
                    worldNameText.Focus();
                }
            }
            else
            {
                newWorldButton.IsChecked = true;
                worldNameText.Focus();
            }

        }        
        
        public WorldInfo WorldInfo
        {
            get
            {
                return (UI);
            }
        }
        private void LoginCheck_Checked(object sender, RoutedEventArgs e)
        {
            LoginCheck_Handle(sender as CheckBox);
        }
        private void LoginCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            LoginCheck_Handle(sender as CheckBox);
        }

        private void LoginCheck_Handle(CheckBox checkBox)
        {
            // Use IsChecked.
            if (loginCheck.IsChecked.HasValue && loginCheck.IsChecked.Value)
            {
                usernameText.IsEnabled = true;
                passwordText.IsEnabled = true;

                usernameBlock.Foreground = Brushes.Black;
                passwordBlock.Foreground = Brushes.Black;

                AutoLogin = true;
            }
            else
            {
                usernameText.IsEnabled = false;
                passwordText.IsEnabled = false;

                usernameBlock.Foreground = Brushes.Gray;
                passwordBlock.Foreground = Brushes.Gray;

                AutoLogin = false;
            }
            
        }

        private void SaveLoginCheck_Checked(object sender, RoutedEventArgs e)
        {
            SaveLogin_Handle(sender as CheckBox);
        }

        private void SaveLoginCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveLogin_Handle(sender as CheckBox);
        }

        private void SaveLogin_Handle(CheckBox checkBox)
        {
            // Use IsChecked.
            if (passwordCheck.IsChecked.HasValue && passwordCheck.IsChecked.Value)
            {
                SaveLogin = true;
            }
            else
            {
                SaveLogin = false;
            }

        }

        private void OkNewWorldButton_Click(object sender, EventArgs e)
        {
            string worldNameTemp = worldNameText.Text.Trim();
            string worldURLTemp = worldURLText.Text.Trim();

            bool errorMessage = false;

            if (newWorldButton.IsChecked.HasValue && newWorldButton.IsChecked.Value)
            {
                NewWorldSelect = true;
                if (worldNameTemp.Length > 0)
                {
                    UI.WorldName = worldNameTemp;
                }
                else
                {
                    errorMessage = true;
                }

                if (worldURLTemp.Length > 0)
                {
                    UI.WorldURL = worldURLTemp;
                }
                else
                {
                    errorMessage = true;
                }

                if (worldPortText.Text.Trim().Length > 0)
                {
                    int worldPortTemp;
                    //Let's make sure the user actually entered a number for the port
                    try
                    {
                        worldPortTemp = Convert.ToInt32(worldPortText.Text.Trim());
                    }
                    catch (FormatException)
                    {
                        errorMessage = true;
                        worldPortTemp = 0;
                    }
                    UI.WorldPort = worldPortTemp;
                }
                else
                {
                    errorMessage = true;
                }
                if (errorMessage)
                {
                    MessageBox.Show("Input Field(s) are empty or invalid");
                }
                else
                {
                    UI.AutoLogin = AutoLogin;
                    NewWorldSelect = true;
                    CloseWindow();
                }                
            }
            else
            {
                if (worldList.SelectedValue != null)
                {
                    NewWorldSelect = false;
                    CloseWindow();
                }
                else
                {
                    this.DialogResult = false;
                    this.Close();
                }
                
            }
            if (AutoLogin)
            {
                UI.Username = usernameText.Text.Trim();
                string tempPass = passwordText.Text.Trim();
                byte[] bytePass = Encoding.UTF8.GetBytes(tempPass);
                UI.ProtectedPassword = bytePass;
            }
            
        }
        private void CancelNewWorldButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CloseWindow()
        {
           
            this.DialogResult = true;

            this.Close();
        }
    }
}
