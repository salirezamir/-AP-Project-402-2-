using Restaurant_Manager.Models;
using System.Windows;
using System.Linq;
using Restaurant_Manager.DAL;
using System;
using System.Data.Entity;

namespace Restaurant_Manager.CustomerPannle
{
    public partial class RestaurantPanelForCustomer : Window
    {
        private Restaurant _restaurant;
        private User _currentUser;

        public RestaurantPanelForCustomer(Restaurant restaurant, User currentUser)
        {
            InitializeComponent();
            _restaurant = restaurant;
            _currentUser = currentUser;
            LoadMenu();
        }

        private void LoadMenu()
        {
            using (var context = new RestaurantContext())
            {
                var menuItems = context.Stuffs
                    .Where(s => s.Resturant.Id == _restaurant.Id)
                    .ToList();

                MenuListView.ItemsSource = menuItems;
            }
        }

        private void MenuListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MenuListView.SelectedItem is Stuff selectedStuff)
            {
                LoadComments(selectedStuff);
            }
        }

        private void LoadComments(Stuff stuff)
        {
            using (var context = new RestaurantContext())
            {
                var comments = context.Comments
                    .Where(c => c.Stuff.Id == stuff.Id)
                    .Include(c => c.Users)
                    .ToList();

                CommentTreeView.ItemsSource = comments;
            }
        }

        private void EditComment(int commentId, string newDetails)
        {
            using (var context = new RestaurantContext())
            {
                var comment = context.Comments.Find(commentId);
                if (comment != null)
                {
                    comment.Details = newDetails;
                    comment.IsEdited = true;
                    context.SaveChanges();
                }
            }
        }

        private void DeleteComment(int commentId)
        {
            using (var context = new RestaurantContext())
            {
                var comment = context.Comments.FirstOrDefault(c => c.Id == commentId);
                if (comment != null)
                {
                   
                    context.Comments.Remove(comment);
                    context.SaveChanges();
                }
            }
        }

        private void CommentTreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CommentTreeView.SelectedItem is Comment selectedComment)
            {
                // Show comment details or options
                var dialogResult = MessageBox.Show($"Selected Comment: {selectedComment.Details}\n\n" +
                                                   $"Date: {selectedComment.Date}\n" +
                                                   $"Edited: {(selectedComment.IsEdited ? "Yes" : "No")}",
                                                   "Comment Details", MessageBoxButton.OK, MessageBoxImage.Information);

                if (dialogResult == MessageBoxResult.OK && selectedComment.Users.Id == _currentUser.Id)
                {
                    var editResult = MessageBox.Show("Do you want to edit this comment?", "Edit Comment", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (editResult == MessageBoxResult.Yes)
                    {
                        var newDetails = "New comment details"; // Replace with actual edit logic
                        EditComment(selectedComment.Id, newDetails);
                    }
                }
            }
        }

        private void EditComment_Click(object sender, RoutedEventArgs e)
        {
            if (CommentTreeView.SelectedItem is Comment selectedComment && selectedComment.Users.Id == _currentUser.Id)
            {
                // Show edit dialog
                var newDetails = "New comment details"; // Get from user input
                EditComment(selectedComment.Id, newDetails);
            }
        }

        private void DeleteComment_Click(object sender, RoutedEventArgs e)
        {
            if (CommentTreeView.SelectedItem is Comment selectedComment && selectedComment.Users.Id == _currentUser.Id)
            {
                DeleteComment(selectedComment.Id);
            }
        }

        private void ReplyComment_Click(object sender, RoutedEventArgs e)
        {
            if (CommentTreeView.SelectedItem is Comment selectedComment)
            {
                // Show reply dialog
                var replyDetails = "Reply comment details"; // Get from user input
                AddReply(selectedComment.Id, replyDetails);
            }
        }

        private void AddReply(int commentId, string replyDetails)
        {
            using (var context = new RestaurantContext())
            {
                var parentComment = context.Comments.Find(commentId);
                if (parentComment != null)
                {
                    var reply = new Comment
                    {
                        Details = replyDetails,
                        Date = DateTime.Now,
                        IsEdited = false,
                        Users = _currentUser,
                        Stuff = parentComment.Stuff
                    };
                    
                    context.SaveChanges();
                }
            }
        }
    }
}