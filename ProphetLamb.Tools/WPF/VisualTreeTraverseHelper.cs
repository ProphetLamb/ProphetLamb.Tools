﻿using System.Windows;
using System.Windows.Media;

namespace ProphetLamb.Tools.WPF
{
    public static class VisualTreeTraverseHelper
    {
        /// <summary>
        /// Traverses the VisualTree parentage until the first element of the type <typeparamref name="T"/> is traversed, then returns the instance of that <see cref="DependencyObject"/>.
        /// </summary>
        /// <typeparam name="T">Type of the desired super object.</typeparam>
        /// <param name="object"><see cref="DependencyObject"/> in the VisualTree.</param>
        /// <returns>The first element in the VisualTree parentage eligable.</returns>
        public static T GetSuperElement<T>(in DependencyObject @object) where T : DependencyObject
        {
            var type = typeof(T);
            DependencyObject dep = @object;
            while (dep != null && dep.GetType() != type) dep = VisualTreeHelper.GetParent(dep);
            if (dep == null) return null;
            return dep as T;
        }
    }
}