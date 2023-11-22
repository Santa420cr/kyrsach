﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace GameDatabase_App
{
    /// <summary>
    /// Логика взаимодействия для EditGameWindow.xaml
    /// </summary>
    public partial class EditGameWindow : Window
    {
        public EditGameWindow(int gameId = 0)
        {
            InitializeComponent();
            Tag = gameId;
            ShowGame(gameId);
        }

        // Отмена изменений
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Сохранение изменений
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditGameTitle.Text.Length != 0 && EditGameSummary.Text.Length != 0 && EditGameOfficial.Text.Length != 0)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.userConnection))
                    {
                        connection.Open();

                        // Добавление/Редактирование игры
                        using (SqlCommand command = new SqlCommand() { Connection = connection })
                        {
                            command.Parameters.Add(new SqlParameter("@title", EditGameTitle.Text));
                            command.Parameters.Add(new SqlParameter("@release", EditGameRelease.SelectedDate != null ? (object)EditGameRelease.SelectedDate : DBNull.Value));
                            command.Parameters.Add(new SqlParameter("@website", EditGameOfficial.Text));
                            command.Parameters.Add(new SqlParameter("@summary", EditGameSummary.Text));

                            if ((int)Tag > 0)
                            {
                                command.Parameters.Add(new SqlParameter("@id", (int)Tag));
                                command.CommandText = @"UPDATE dbo.Games SET title = @title, release_date = @release, website = @website, summary = @summary WHERE id = @id";
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                command.CommandText = "StrProc_AddGame";
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.ExecuteNonQuery();
                                command.CommandType = System.Data.CommandType.Text;
                                command.CommandText = @"SELECT TOP(1) id FROM dbo.Games ORDER BY id DESC";
                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    dataReader.Read();
                                    Tag = dataReader.GetInt32(0);
                                }
                            }
                        }

                        // Добавление/Удаление разработчиков игры
                        using (SqlCommand command = new SqlCommand(@"SELECT dbo.Games_Developers.game_id, dbo.Games_Developers.developer_id FROM dbo.Games_Developers WHERE game_id = @id AND developer_id = @dev_id", connection))
                        {
                            command.Parameters.Add(new SqlParameter("@id", (int)Tag));
                            command.Parameters.Add(new SqlParameter("@dev_id", 0));
                            foreach (CheckBox item in GameEditDevelopers.Children)
                            {
                                command.Parameters[1].Value = item.TabIndex;
                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    if ((bool)item.IsChecked)
                                    {
                                        if (!dataReader.HasRows)
                                        {
                                            using (SqlConnection connection1 = new SqlConnection(Properties.Settings.Default.userConnection))
                                            {
                                                connection1.Open();
                                                using (SqlCommand insertCommand = new SqlCommand(@"StrProc_AddGameDeveloper", connection1))
                                                {
                                                    insertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                                    insertCommand.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                    insertCommand.Parameters.Add(new SqlParameter("@developer_id", item.TabIndex));
                                                    insertCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dataReader.HasRows)
                                        {
                                            using (SqlConnection connection1 = new SqlConnection(Properties.Settings.Default.userConnection))
                                            {
                                                connection1.Open();
                                                using (SqlCommand insertCommand = new SqlCommand(@"DELETE FROM dbo.Games_Developers WHERE game_id = @game_id AND developer_id = @dev_id", connection1))
                                                {
                                                    insertCommand.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                    insertCommand.Parameters.Add(new SqlParameter("@dev_id", item.TabIndex));
                                                    insertCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Добавление/Удаление издателей игры
                        using (SqlCommand command = new SqlCommand(@"SELECT dbo.Games_Publishers.game_id, dbo.Games_Publishers.publisher_id FROM dbo.Games_Publishers WHERE game_id = @id AND publisher_id = @dev_id", connection))
                        {
                            command.Parameters.Add(new SqlParameter("@id", (int)Tag));
                            command.Parameters.Add(new SqlParameter("@dev_id", 0));
                            foreach (CheckBox item in GameEditPublishers.Children)
                            {
                                command.Parameters[1].Value = item.TabIndex;
                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    if ((bool)item.IsChecked)
                                    {
                                        if (!dataReader.HasRows)
                                        {
                                            using (SqlConnection connection1 = new SqlConnection(Properties.Settings.Default.userConnection))
                                            {
                                                connection1.Open();
                                                using (SqlCommand insertCommand = new SqlCommand(@"StrProc_AddGamePublisher", connection1))
                                                {
                                                    insertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                                    insertCommand.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                    insertCommand.Parameters.Add(new SqlParameter("@publisher_id", item.TabIndex));
                                                    insertCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dataReader.HasRows)
                                        {
                                            using (SqlConnection connection1 = new SqlConnection(Properties.Settings.Default.userConnection))
                                            {
                                                connection1.Open();
                                                using (SqlCommand insertCommand = new SqlCommand(@"DELETE FROM dbo.Games_Publishers WHERE game_id = @game_id AND publisher_id = @dev_id", connection1))
                                                {
                                                    insertCommand.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                    insertCommand.Parameters.Add(new SqlParameter("@dev_id", item.TabIndex));
                                                    insertCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Добавление/Удаление жанров игры
                        using (SqlCommand command = new SqlCommand(@"SELECT dbo.Games_Genres.game_id, dbo.Games_Genres.genre_id FROM dbo.Games_Genres WHERE game_id = @id AND genre_id = @dev_id", connection))
                        {
                            command.Parameters.Add(new SqlParameter("@id", (int)Tag));
                            command.Parameters.Add(new SqlParameter("@dev_id", 0));
                            foreach (CheckBox item in GameEditGenres.Children)
                            {
                                command.Parameters[1].Value = item.TabIndex;
                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    if ((bool)item.IsChecked)
                                    {
                                        if (!dataReader.HasRows)
                                        {
                                            using (SqlConnection connection1 = new SqlConnection(Properties.Settings.Default.userConnection))
                                            {
                                                connection1.Open();
                                                using (SqlCommand insertCommand = new SqlCommand(@"StrProc_AddGameGenre", connection1))
                                                {
                                                    insertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                                    insertCommand.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                    insertCommand.Parameters.Add(new SqlParameter("@genre_id", item.TabIndex));
                                                    insertCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dataReader.HasRows)
                                        {
                                            using (SqlConnection connection1 = new SqlConnection(Properties.Settings.Default.userConnection))
                                            {
                                                connection1.Open();
                                                using (SqlCommand insertCommand = new SqlCommand(@"DELETE FROM dbo.Games_Genres WHERE game_id = @game_id AND genre_id = @dev_id", connection1))
                                                {
                                                    insertCommand.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                    insertCommand.Parameters.Add(new SqlParameter("@dev_id", item.TabIndex));
                                                    insertCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Добавление/Удаление платформ игры
                        using (SqlCommand command = new SqlCommand(@"SELECT dbo.Games_Platforms.game_id, dbo.Games_Platforms.platform_id FROM dbo.Games_Platforms WHERE game_id = @id AND platform_id = @dev_id", connection))
                        {
                            command.Parameters.Add(new SqlParameter("@id", (int)Tag));
                            command.Parameters.Add(new SqlParameter("@dev_id", 0));
                            foreach (CheckBox item in GameEditPlatforms.Children)
                            {
                                command.Parameters[1].Value = item.TabIndex;
                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    if ((bool)item.IsChecked)
                                    {
                                        if (!dataReader.HasRows)
                                        {
                                            using (SqlConnection connection1 = new SqlConnection(Properties.Settings.Default.userConnection))
                                            {
                                                connection1.Open();
                                                using (SqlCommand insertCommand = new SqlCommand(@"StrProc_AddGamePlatform", connection1))
                                                {
                                                    insertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                                    insertCommand.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                    insertCommand.Parameters.Add(new SqlParameter("@platform_id", item.TabIndex));
                                                    insertCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dataReader.HasRows)
                                        {
                                            using (SqlConnection connection1 = new SqlConnection(Properties.Settings.Default.userConnection))
                                            {
                                                connection1.Open();
                                                using (SqlCommand insertCommand = new SqlCommand(@"DELETE FROM dbo.Games_Platforms WHERE game_id = @game_id AND platform_id = @dev_id", connection1))
                                                {
                                                    insertCommand.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                    insertCommand.Parameters.Add(new SqlParameter("@dev_id", item.TabIndex));
                                                    insertCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Добавление/Изменение/Удаление рецензий на игру
                        using (SqlCommand command = new SqlCommand(@"SELECT dbo.Reviews.game_id, dbo.Reviews.reviewer_id FROM dbo.Reviews WHERE game_id = @id AND reviewer_id = @reviewer_id", connection))
                        {
                            command.Parameters.Add(new SqlParameter("@id", (int)Tag));
                            command.Parameters.Add(new SqlParameter("@reviewer_id", 0));
                            using (SqlConnection connection1 = new SqlConnection(Properties.Settings.Default.userConnection))
                            {
                                connection1.Open();

                                using (SqlCommand command1 = new SqlCommand(@"SELECT id FROM dbo.Reviewers", connection1))
                                {
                                    using (SqlDataReader dataReader1 = command1.ExecuteReader())
                                    {
                                        while (dataReader1.Read())
                                        {
                                            command.Parameters[1].Value = dataReader1.GetInt32(0);

                                            using (SqlDataReader dataReader = command.ExecuteReader())
                                            {
                                                if (dataReader.HasRows)
                                                {
                                                    bool having = false;
                                                    foreach (StackPanel item in ReviewsList.Children)
                                                    {
                                                        if ((int)(item.Tag) == dataReader1.GetInt32(0))
                                                        {
                                                            using (SqlConnection connection2 = new SqlConnection(Properties.Settings.Default.userConnection))
                                                            {
                                                                connection2.Open();
                                                                using (SqlCommand command2 = new SqlCommand(@"UPDATE dbo.Reviews SET score = @score, summary = @summary, web_address = @url 
                                                                    WHERE dbo.Reviews.game_id = @game_id AND dbo.Reviews.reviewer_id = @reviewer_id", connection2))
                                                                {
                                                                    command2.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                                    command2.Parameters.Add(new SqlParameter("@reviewer_id", dataReader1.GetInt32(0)));
                                                                    DockPanel dock = (DockPanel)item.Children[2];
                                                                    Slider slider = (Slider)dock.Children[1];
                                                                    if (slider.Value > 0)
                                                                    {
                                                                        command2.Parameters.Add(new SqlParameter("@score", slider.Value));
                                                                    }
                                                                    else
                                                                    {
                                                                        command2.Parameters.Add(new SqlParameter("@score", DBNull.Value));
                                                                    }
                                                                    command2.Parameters.Add(new SqlParameter("@summary", ((TextBox)item.Children[4]).Text));
                                                                    command2.Parameters.Add(new SqlParameter("@url", ((TextBox)item.Children[6]).Text));
                                                                    command2.ExecuteNonQuery();
                                                                }
                                                            }
                                                            having = true;
                                                        }
                                                    }
                                                    if (!having)
                                                    {
                                                        using (SqlConnection connection2 = new SqlConnection(Properties.Settings.Default.userConnection))
                                                        {
                                                            connection2.Open();
                                                            using (SqlCommand command2 = new SqlCommand(@"DELETE FROM dbo.Reviews WHERE dbo.Reviews.game_id = @game_id 
                                                                AND dbo.Reviews.reviewer_id = @reviewer_id", connection2))
                                                            {
                                                                command2.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                                command2.Parameters.Add(new SqlParameter("@reviewer_id", dataReader1.GetInt32(0)));
                                                                command2.ExecuteNonQuery();
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    foreach (StackPanel item in ReviewsList.Children)
                                                    {
                                                        if ((int)(item.Tag) == dataReader1.GetInt32(0))
                                                        {
                                                            using (SqlConnection connection2 = new SqlConnection(Properties.Settings.Default.userConnection))
                                                            {
                                                                connection2.Open();
                                                                using (SqlCommand command2 = new SqlCommand(@"StrProc_AddReview", connection2))
                                                                {
                                                                    command2.CommandType = System.Data.CommandType.StoredProcedure;
                                                                    command2.Parameters.Add(new SqlParameter("@game_id", (int)Tag));
                                                                    command2.Parameters.Add(new SqlParameter("@reviewer_id", dataReader1.GetInt32(0)));
                                                                    DockPanel dock = (DockPanel)item.Children[2];
                                                                    Slider slider = (Slider)dock.Children[1];
                                                                    if (slider.Value > 0)
                                                                    {
                                                                        command2.Parameters.Add(new SqlParameter("@score", slider.Value));
                                                                    }
                                                                    else
                                                                    {
                                                                        command2.Parameters.Add(new SqlParameter("@score", DBNull.Value));
                                                                    }
                                                                    command2.Parameters.Add(new SqlParameter("@summary", ((TextBox)item.Children[4]).Text));
                                                                    command2.Parameters.Add(new SqlParameter("@web_address", ((TextBox)item.Children[6]).Text));
                                                                    command2.ExecuteNonQuery();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"В процессе обработки данных произошла ошибка:\n{ex}", "Ошибка обработки данных", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Поля заполнены некорректно", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Добавление рецензента
        private void AddReview_Click(object sender, RoutedEventArgs e)
        {
            if (Reviewers.SelectedItem != null)
            {
                int reviewer = (int)((ListBoxItem)(Reviewers.SelectedItem)).Tag;

                // Панель компоновки рецензии
                StackPanel reviewTile = new StackPanel() { Margin = new Thickness(10), Tag = reviewer };
                ReviewsList.Children.Add(reviewTile);

                DockPanel dock1 = new DockPanel()
                {
                    VerticalAlignment = VerticalAlignment.Center
                };
                reviewTile.Children.Add(dock1);

                // Название издания/рецензента
                Label reviewerTitle = new Label()
                {
                    FontSize = 18,
                    Content = ((ListBoxItem)(Reviewers.SelectedItem)).Content
                };
                dock1.Children.Add(reviewerTitle);

                Button deleteReviewButton = new Button()
                {
                    Tag = reviewer,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 100,
                    Content = "Удалить"
                };
                deleteReviewButton.Click += DeleteReview_Click;
                DockPanel.SetDock(deleteReviewButton, Dock.Right);
                dock1.Children.Add(deleteReviewButton);

                // Оценка рецензии
                // --------------------------------------------------------
                Label scoreLabel = new Label()
                {
                    Content = "Оценка:"
                };
                reviewTile.Children.Add(scoreLabel);

                DockPanel dock = new DockPanel()
                {
                    VerticalAlignment = VerticalAlignment.Center
                };
                reviewTile.Children.Add(dock);

                TextBox scoreTextBox = new TextBox()
                {
                    Width = 40,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    IsEnabled = false
                };
                DockPanel.SetDock(scoreTextBox, Dock.Right);
                dock.Children.Add(scoreTextBox);

                Slider scoreSlider = new Slider()
                {
                    Minimum = 0,
                    Maximum = 100,
                    TickFrequency = 1,
                    IsSnapToTickEnabled = true,
                    TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight
                };
                dock.Children.Add(scoreSlider);

                Binding binding = new Binding
                {
                    Source = scoreSlider,
                    Path = new PropertyPath("Value"),
                    Mode = BindingMode.TwoWay
                };
                scoreTextBox.SetBinding(TextBox.TextProperty, binding);
                // --------------------------------------------------------

                // Краткое описание
                // --------------------------------------------------------
                Label summaryLabel = new Label() { Content = "Краткое содержание:" };
                reviewTile.Children.Add(summaryLabel);
                TextBox reviewSummary = new TextBox()
                {
                    Height = 150,
                    TextWrapping = TextWrapping.Wrap
                };
                reviewTile.Children.Add(reviewSummary);
                // --------------------------------------------------------

                // Сайт
                // --------------------------------------------------------
                Label siteLabel = new Label() { Content = "Ссылка на полный текст статьи:" };
                reviewTile.Children.Add(siteLabel);

                TextBox reviewSite = new TextBox()
                {

                };
                reviewTile.Children.Add(reviewSite);

                foreach (ListBoxItem listBoxItem in Reviewers.Items)
                {
                    if ((int)listBoxItem.Tag == reviewer)
                    {
                        Reviewers.Items.Remove(listBoxItem);
                        break;
                    }
                }

            }
        }

        // Удаление рецензии
        private void DeleteReview_Click(object sender, RoutedEventArgs e)
        {
            foreach (StackPanel item in ReviewsList.Children)
            {
                if ((int)item.Tag == (int)((Button)sender).Tag)
                {
                    ReviewsList.Children.Remove(item);
                    Label label = (Label)((DockPanel)item.Children[0]).Children[0];
                    ListBoxItem reviewer = new ListBoxItem()
                    {
                        Tag = item.Tag,
                        Content = label.Content
                    };
                    Reviewers.Items.Add(reviewer);
                    break;
                }
            }
        }

        // Открытие окна редактирования рецензентов
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Внесенные вами изменения не сохранятся. Вы действительно хотите перейти в окно редактирования рецензентов?", 
                    "Предупреждение", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                EditWindow window = new EditWindow(4);
                window.ShowDialog();
                UpdateReviews((int)Tag);
            }
        }


        // Загрузка списков, а также информации о игре при редактировании
        private void ShowGame(int id = 0)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.userConnection))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand() { Connection = connection })
                    {
                        // Отображение списка разработчиков
                        command.CommandText = @"SELECT id, title FROM dbo.Developers";
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    CheckBox item = new CheckBox()
                                    {
                                        TabIndex = dataReader.GetInt32(0),
                                        Content = dataReader.GetString(1)
                                    };
                                    GameEditDevelopers.Children.Add(item);
                                }
                            }
                        }

                        // Отображение списка издателей
                        command.CommandText = @"SELECT id, title FROM dbo.Publishers";
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    CheckBox item = new CheckBox()
                                    {
                                        TabIndex = dataReader.GetInt32(0),
                                        Content = dataReader.GetString(1)
                                    };
                                    GameEditPublishers.Children.Add(item);
                                }
                            }
                        }

                        // Отображение списка жанров
                        command.CommandText = @"SELECT id, title FROM dbo.Genres";
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    CheckBox item = new CheckBox()
                                    {
                                        TabIndex = dataReader.GetInt32(0),
                                        Content = dataReader.GetString(1)
                                    };
                                    GameEditGenres.Children.Add(item);
                                }
                            }
                        }

                        // Отображение списка платформ
                        command.CommandText = @"SELECT id, title FROM dbo.Platforms";
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    CheckBox item = new CheckBox()
                                    {
                                        TabIndex = dataReader.GetInt32(0),
                                        Content = dataReader.GetString(1)
                                    };
                                    GameEditPlatforms.Children.Add(item);
                                }
                            }
                        }

                        if (id > 0)
                        {
                            command.Parameters.Add(new SqlParameter("@id", id));
                            // Получение информации о игре
                            command.CommandText = @"SELECT title, website, release_date, summary FROM dbo.Games WHERE id = @id";
                            using (SqlDataReader dataReader = command.ExecuteReader())
                            {
                                if(dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        EditGameTitle.Text = dataReader.GetString(0);
                                        EditGameOfficial.Text = dataReader.GetString(1);
                                        if (!dataReader.IsDBNull(2))
                                        {
                                            EditGameRelease.SelectedDate = dataReader.GetDateTime(2);
                                        }
                                        else
                                        {
                                            EditGameRelease.SelectedDate = null;
                                        }
                                        EditGameSummary.Text = dataReader.GetString(3);
                                    }
                                }
                            }

                            // Отображение разработчиков игры
                            command.CommandText = @"SELECT developer_id FROM dbo.Games_Developers WHERE game_id = @id";
                            using (SqlDataReader dataReader = command.ExecuteReader())
                            {
                                if (dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        foreach (CheckBox item in GameEditDevelopers.Children)
                                        {
                                            if (item.TabIndex == dataReader.GetInt32(0))
                                            {
                                                item.IsChecked = true;
                                            }
                                        }
                                    }
                                }
                            }

                            // Отображение издателей игры
                            command.CommandText = @"SELECT publisher_id FROM dbo.Games_Publishers WHERE game_id = @id";
                            using (SqlDataReader dataReader = command.ExecuteReader())
                            {
                                if (dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        foreach (CheckBox item in GameEditPublishers.Children)
                                        {
                                            if (item.TabIndex == dataReader.GetInt32(0))
                                            {
                                                item.IsChecked = true;
                                            }
                                        }
                                    }
                                }
                            }

                            // Отображение жанров игры
                            command.CommandText = @"SELECT genre_id FROM dbo.Games_Genres WHERE game_id = @id";
                            using (SqlDataReader dataReader = command.ExecuteReader())
                            {
                                if (dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        foreach (CheckBox item in GameEditGenres.Children)
                                        {
                                            if (item.TabIndex == dataReader.GetInt32(0))
                                            {
                                                item.IsChecked = true;
                                            }
                                        }
                                    }
                                }
                            }

                            // Отображение платформ игры
                            command.CommandText = @"SELECT platform_id FROM dbo.Games_Platforms WHERE game_id = @id";
                            using (SqlDataReader dataReader = command.ExecuteReader())
                            {
                                if (dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        foreach (CheckBox item in GameEditPlatforms.Children)
                                        {
                                            if (item.TabIndex == dataReader.GetInt32(0))
                                            {
                                                item.IsChecked = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"В процессе обработки данных произошла ошибка:\n{ex}", "Ошибка обработки данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateReviews(id);
        }

        // Обновление вкладки рецензии
        private void UpdateReviews(int id = 0)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.userConnection))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand() { Connection = connection })
                    {
                        // Отображение списка рецензентов
                        Reviewers.Items.Clear();
                        command.CommandText = @"SELECT id, title FROM dbo.Reviewers";
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                ListBoxItem reviewer = new ListBoxItem()
                                {
                                    Tag = dataReader.GetInt32(0),
                                    Content = dataReader.GetString(1)
                                };
                                Reviewers.Items.Add(reviewer);
                            }
                        }

                        if (id > 0)
                        {
                            ReviewsList.Children.Clear();
                            // Отображение рецензий игры
                            command.Parameters.Add(new SqlParameter("@id", id));
                            command.CommandText = @"SELECT dbo.Reviewers.id, dbo.Reviewers.title, dbo.Reviews.summary, dbo.Reviews.score, dbo.Reviews.web_address
                                FROM dbo.Reviews INNER JOIN dbo.Reviewers ON dbo.Reviews.reviewer_id = dbo.Reviewers.id WHERE game_id = @id";
                            using (SqlDataReader dataReader = command.ExecuteReader())
                            {
                                if (dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        foreach (ListBoxItem listBoxItem in Reviewers.Items)
                                        {
                                            if ((int)listBoxItem.Tag == dataReader.GetInt32(0))
                                            {
                                                Reviewers.Items.Remove(listBoxItem);
                                                break;
                                            }
                                        }

                                        // Панель компоновки рецензии
                                        StackPanel reviewTile = new StackPanel() { Margin = new Thickness(10), Tag = dataReader.GetInt32(0) };
                                        ReviewsList.Children.Add(reviewTile);

                                        DockPanel dock1 = new DockPanel()
                                        {
                                            VerticalAlignment = VerticalAlignment.Center
                                        };
                                        reviewTile.Children.Add(dock1);

                                        // Название издания/рецензента
                                        Label reviewerTitle = new Label()
                                        {
                                            FontSize = 18,
                                            Content = dataReader.GetString(1)
                                        };
                                        dock1.Children.Add(reviewerTitle);

                                        Button deleteReviewButton = new Button()
                                        {
                                            Tag = dataReader.GetInt32(0),
                                            HorizontalAlignment = HorizontalAlignment.Right,
                                            Width = 100,
                                            Content = "Удалить"
                                        };
                                        deleteReviewButton.Click += DeleteReview_Click;
                                        DockPanel.SetDock(deleteReviewButton, Dock.Right);
                                        dock1.Children.Add(deleteReviewButton);

                                        // Оценка рецензии
                                        // --------------------------------------------------------
                                        Label scoreLabel = new Label()
                                        {
                                            Content = "Оценка:"
                                        };
                                        reviewTile.Children.Add(scoreLabel);

                                        DockPanel dock = new DockPanel()
                                        {
                                            VerticalAlignment = VerticalAlignment.Center
                                        };
                                        reviewTile.Children.Add(dock);

                                        TextBox scoreTextBox = new TextBox()
                                        {
                                            Width = 40,
                                            VerticalContentAlignment = VerticalAlignment.Center,
                                            IsEnabled = false
                                        };
                                        DockPanel.SetDock(scoreTextBox, Dock.Right);
                                        dock.Children.Add(scoreTextBox);

                                        Slider scoreSlider = new Slider()
                                        {
                                            Value = dataReader.IsDBNull(3) ? 0 : dataReader.GetByte(3),
                                            Minimum = 0,
                                            Maximum = 100,
                                            TickFrequency = 1,
                                            IsSnapToTickEnabled = true,
                                            TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight
                                        };
                                        dock.Children.Add(scoreSlider);

                                        Binding binding = new Binding
                                        {
                                            Source = scoreSlider,
                                            Path = new PropertyPath("Value"),
                                            Mode = BindingMode.TwoWay
                                        };
                                        scoreTextBox.SetBinding(TextBox.TextProperty, binding);
                                        // --------------------------------------------------------

                                        // Краткое описание
                                        // --------------------------------------------------------
                                        Label summaryLabel = new Label() { Content = "Краткое содержание:" };
                                        reviewTile.Children.Add(summaryLabel);
                                        TextBox reviewSummary = new TextBox()
                                        {
                                            Height = 150,
                                            Text = dataReader.GetString(2),
                                            TextWrapping = TextWrapping.Wrap
                                        };
                                        reviewTile.Children.Add(reviewSummary);
                                        // --------------------------------------------------------

                                        // Сайт
                                        // --------------------------------------------------------
                                        Label siteLabel = new Label() { Content = "Ссылка на полный текст статьи:" };
                                        reviewTile.Children.Add(siteLabel);

                                        TextBox reviewSite = new TextBox()
                                        {
                                            Text = dataReader.GetString(4)
                                        };
                                        reviewTile.Children.Add(reviewSite);
                                        // --------------------------------------------------------
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"В процессе обработки данных произошла ошибка:\n{ex}", "Ошибка обработки данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
