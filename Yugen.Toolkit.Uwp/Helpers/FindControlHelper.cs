using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Yugen.Toolkit.Uwp.Helpers
{
    public static class FindControlHelper
    {
        /// <summary>
        /// Find the first ancestor of a control by type or by type and name
        /// </summary>
        /// <typeparam name="T">control type</typeparam>
        /// <param name="frameworkElement">control</param>
        /// <param name="name">name</param>
        /// <returns>control</returns>
        public static DependencyObject FindAncestor<T>(FrameworkElement frameworkElement, string name = null)
        {
            var element = frameworkElement.Parent;
            return InternalFindAncestor<T>(element, name);
        }

        /// <summary>
        /// Find the first ancestor of a control by type or by type and name
        /// </summary>
        /// <typeparam name="T">control type</typeparam>
        /// <param name="uiElement">control</param>
        /// <param name="name">name</param>
        /// <returns>control</returns>
        public static DependencyObject FindAncestor<T>(object uiElement, string name = null)
        {
            var element = ((FrameworkElement)uiElement).Parent;
            return InternalFindAncestor<T>(element, name);
        }

        /// <summary>
        /// Find the first ancestor of a control by type or by type and name
        /// </summary>
        /// <typeparam name="T">control type</typeparam>
        /// <param name="dependencyObject">control</param>
        /// <param name="name">name</param>
        /// <returns>control</returns>
        public static DependencyObject FindAncestor<T>(DependencyObject dependencyObject, string name = null)
        {
            var element = VisualTreeHelper.GetParent(dependencyObject);
            return InternalFindAncestor<T>(element, name);
        }


        private static DependencyObject InternalFindAncestor<T>(DependencyObject dependencyObject, string name = null)
        {
            if (dependencyObject == null)
                return null;

            var type = dependencyObject.GetType();
            var baseType = type.BaseType;

            if (type == typeof(T) || baseType == typeof(T))
            {
                if (name == null)
                    return dependencyObject;
                else if (((FrameworkElement)dependencyObject).Name.Equals(name))
                    return dependencyObject;
            }

            var element = ((FrameworkElement)dependencyObject).Parent;
            return InternalFindAncestor<T>(element);
        }


        /// <summary>
        /// Find the first Descendant of a control by type or by type and name
        /// </summary>
        /// <typeparam name="T">control type</typeparam>
        /// <param name="dependencyObject">control</param>
        /// <param name="name">name</param>
        /// <returns>control</returns>
        public static DependencyObject FindDescendant<T>(DependencyObject dependencyObject, string name = null)
        {
            var childNumber = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (var i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);

                // Not a framework element or is null
                if (!(child is FrameworkElement frameworkElement))
                    return null;

                // Found the control so return
                if (child is T)
                {
                    if (name == null)
                        return child;
                    else if (frameworkElement.Name.Equals(name))
                        return child;
                }

                // Not found it - search children
                DependencyObject nextLevel = FindDescendant<T>(child, name);
                if (nextLevel != null)
                    return nextLevel;
            }

            return null;
        }

        /// <summary>
        /// Get the list of controls by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        public static List<Control> GetControlList<T>(object uiElement)
        {
            List<Control> controlList = new List<Control>();
            FindControl<T>(controlList, uiElement);
            return controlList;
        }

        private static void FindControl<T>(List<Control> controlList, object uiElement)
        {
            switch (uiElement)
            {
                case T _:
                    controlList.Add((Control)uiElement);
                    break;
                case Panel _:
                    var uiElementAsCollection = (Panel)uiElement;
                    foreach (var element in uiElementAsCollection.Children)
                        FindControl<T>(controlList, element);
                    break;
                case UserControl _:
                    var uiElementAsUserControl = (UserControl)uiElement;
                    FindControl<T>(controlList, uiElementAsUserControl.Content);
                    break;
                case ContentControl _:
                    var uiElementAsContentControl = (ContentControl)uiElement;
                    FindControl<T>(controlList, uiElementAsContentControl.Content);
                    break;
                case ItemsControl _:
                    var uiElementAsItemsControl = (ItemsControl)uiElement;
                    foreach (var element in uiElementAsItemsControl.Items)
                        FindControl<T>(controlList, element);
                    break;
            }
        }

        /// <summary>
        /// Get the next contol of the list
        /// </summary>
        /// <param name="controlList"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static Control NextControl(List<Control> controlList, object sender)
        {
            var index = ((Control)sender).TabIndex + 1;
            return controlList.FirstOrDefault(x => x.TabIndex == index);
        }

        /// <summary>
        /// Focus on Next Control of the list
        /// </summary>
        /// <param name="controlList"></param>
        /// <param name="sender"></param>
        public static void FocusOnNextControl(List<Control> controlList, object sender)
        {
            NextControl(controlList, sender)?.Focus(FocusState.Programmatic);
        }


		/// <summary>
		/// No Mobile Anymore
		/// </summary>
		/// <param name="controlList"></param>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <param name="button"></param>
		/// <returns></returns>
		public static bool GoToNextControlIfMobileOrCheckEnterIfDesktop(List<Control> controlList, object sender, KeyRoutedEventArgs e, Button button)
        {
            if (!e.Key.Equals(Windows.System.VirtualKey.Enter))
                return false;

            if (!SystemHelper.IsMobile)
                return IsButtonEnabled(button, e);

            FocusOnNextControl(controlList, sender);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        private static bool IsButtonEnabled(Button button, KeyRoutedEventArgs _) => !SystemHelper.IsMobile && button.IsEnabled;
    }
}